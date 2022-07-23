using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using PiEarManager.Helpers;

namespace PiEarManager.Model
{
    public class Device : INotifyPropertyChanged
    {
        private string _name;
        [JsonProperty("name")]
        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }
        private int _index;
        [JsonProperty("index")]
        public int Index
        {
            get => _index;
            set
            {
                if (value == _index) return;
                _index = value;
                OnPropertyChanged();
            }
        }
        private int _channels;
        [JsonProperty("channels")]
        public int Channels
        {
            get => _channels;
            set
            {
                if (value == _channels) return;
                _channels = value;
                OnPropertyChanged();
            }
        }
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value == false)
                {
                    OnPropertyChanged();
                    return;
                }
                Task.Run(async () =>
                    {
                        var task = await Networking.PutRequest($"/devices?id={Index}");
                        var resp = task;
                        var jsonData = JsonConvert.DeserializeObject<JsonObject>(resp);
                        if (jsonData == null) return;
                        if (jsonData.Device == Index)
                        {
                            _isSelected = true;
                            OnPropertyChanged();
                        }
                        else
                        {
                            Debug.WriteLine("Failed to select device");
                        }
                    }
                );
            }
        }
        public void SetNotSelected()
        {
            _isSelected = false;
            OnPropertyChanged(nameof(IsSelected));
        }
        public void SetSelected()
        {
            _isSelected = true;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}