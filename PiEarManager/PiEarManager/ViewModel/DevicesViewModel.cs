using System.ComponentModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PiEarManager.Helpers;
using PiEarManager.Model;
using Device = PiEarManager.Model.Device;

namespace PiEarManager.ViewModel
{
    public class DevicesViewModel
    {
        private bool _initialized = false;
        public JsonObject JsonData { get; private set; }
        public void LoadDevices()
        {
            while (Networking.ServerIp == null)
            {
                Task.Delay(100);
            }
            Task<string> req = Networking.GetRequest("/devices");
            req.Wait();
            string json = req.Result;
            JsonData = JsonConvert.DeserializeObject<JsonObject>(json);
            if (JsonData == null)
            {
                return;
            }
            foreach (var device in JsonData.Devices)
            {
                device.PropertyChanged += ListenForPropChange;
                if (device.Index == JsonData.Device)
                {
                    device.SetSelected();
                }
            }
        }
        private void ListenForPropChange(object sender, PropertyChangedEventArgs e)
        {
            if (!_initialized)
            {
                _initialized = true;
                return;
            }
            var device = (Device)sender;
            if (e.PropertyName == nameof(device.IsSelected))
            {
                if (device.IsSelected)
                {
                    int toMarkNotSelected = JsonData.Device;
                    JsonData.Device = device.Index;
                    foreach (var dev in JsonData.Devices)
                    {
                        if (dev.Index == toMarkNotSelected)
                        {
                            dev.SetNotSelected();
                        }
                    }
                }
            }
        }
    }
}