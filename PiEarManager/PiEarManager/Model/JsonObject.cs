using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace PiEarManager.Model
{
    public class JsonObject : INotifyPropertyChanged
    {
        [JsonProperty("devices")]
        public List<Device> Devices { get; private set; }
        [JsonProperty("device")]
        public int Device { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}