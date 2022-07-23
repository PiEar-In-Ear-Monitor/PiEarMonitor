using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PiEarManager.Helpers
{
    public static class Networking
    {
        private static readonly UdpClient UdpClient = new UdpClient();
        private const int Port = 9090;
        private static readonly IPAddress MulticastIpAddress = IPAddress.Parse("224.0.0.69");
        private const int MulticastPort = 6666;
        public static string ServerIp { get; private set; }
        public static void FindServerIp()
        {
            // The following three lines allow multiple clients on the same PC
            UdpClient.ExclusiveAddressUse = false;
            UdpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            UdpClient.ExclusiveAddressUse = false;
            // Bind, Join
            UdpClient.Client.Bind(new IPEndPoint(IPAddress.Any, MulticastPort));
            UdpClient.JoinMulticastGroup(MulticastIpAddress, IPAddress.Any);

            // Start listening for incoming data
            UdpClient.BeginReceive(ReceivedCallback, null);
        }
        private static void ReceivedCallback(IAsyncResult ar)
        {
            // Get received data
            IPEndPoint sender = new IPEndPoint(0, 0);
            UdpClient.EndReceive(ar, ref sender);
            ServerIp = sender.Address.ToString();
        }
        
        public static async Task<string> GetRequest(string endpoint)
        {
            if (ServerIp == null) return null;
            try
            {
                WebRequest request = WebRequest.Create ($"http://{ServerIp}:{Port}{endpoint}");
                request.Timeout = 5000;
                return await _getResp(request);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static async Task<string> PutRequest(string endpoint)
        {
            if (ServerIp == null) return null;
            try
            {
                WebRequest request = WebRequest.Create($"http://{ServerIp}:{Port}{endpoint}");
                request.Method = "PUT";
                request.Timeout = 5000;
                return await _getResp(request);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        private static async Task<string> _getResp(WebRequest req)
        {
            try
            {
                HttpWebResponse response = (HttpWebResponse)await req.GetResponseAsync();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream ?? throw new InvalidOperationException());
                string responseFromServer = await reader.ReadToEndAsync();
                reader.Close();
                dataStream.Close();
                response.Close();
                return responseFromServer;
            }
            catch (Exception e)
            {
                return "";
            }
        }
    }
}