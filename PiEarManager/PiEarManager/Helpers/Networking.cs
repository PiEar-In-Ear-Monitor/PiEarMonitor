using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PiEar.Helpers
{
    public static class Networking
    {
        private static bool _foundIp;
        private static string _serverIp;
        private static UdpClient _udpClient;
        public static string ServerIp => (_foundIp) ? _serverIp : null;
        public const int Port = 9090;
        public const int MulticastPort = 6666;
        public const string MulticastIp = "224.0.0.69";
        public static async Task<string> GetRequest(string endpoint, bool forDiscovery = false)
        {
            if (!_foundIp && !forDiscovery) return null;
            try
            {
                WebRequest request = WebRequest.Create ($"http://{_serverIp}:{Port}{endpoint}");
                request.Timeout = 5000;
                return await _getResp(request);
            }
            catch (Exception e)
            {
                App.Logger.DebugWrite(e.Message);
                return null;
            }
        }
        public static async Task<string> PutRequest(string endpoint)
        {
            if (!_foundIp) return null;
            try
            {
                WebRequest request = WebRequest.Create($"http://{ServerIp}:{Port}{endpoint}");
                request.Method = "PUT";
                request.Timeout = 5000;
                return await _getResp(request);
            }
            catch (Exception e)
            {
                App.Logger.DebugWrite(e.Message);
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
                if (e != null)
                {
                    Debug.WriteLine($"Failed to get response from server IP {_serverIp}:{Port}");
                }
                return "";
            }
        }
        private static void OnUdpDataReceived(IAsyncResult result)
        {
            Debug.WriteLine($">>> in receive");

            var udpClient = result.AsyncState as UdpClient;
            if (udpClient == null)
                return;

            IPEndPoint remoteAddr = null;
            var recvBuffer = udpClient.EndReceive(result, ref remoteAddr);

            Debug.WriteLine($"MESSAGE FROM: {remoteAddr.Address}:{remoteAddr.Port}, MESSAGE SIZE: {recvBuffer?.Length ?? 0}");

            udpClient.BeginReceive(OnUdpDataReceived, udpClient);
        }
        public static void FindServerIp()
        {
            _udpClient = new UdpClient()
            {
                ExclusiveAddressUse = false,
                EnableBroadcast = true
            };
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, MulticastPort));
            _udpClient.BeginReceive((result) =>
            {
                var udpClient = result.AsyncState as UdpClient;
                if (udpClient == null)
                    return;
                IPEndPoint remoteAddr = null;
                udpClient.EndReceive(result, ref remoteAddr);
                _serverIp = remoteAddr.Address.ToString();
                _foundIp = true;
                Debug.WriteLine($"Found server IP: {_serverIp}");
            }, _udpClient);
        }
    }
}