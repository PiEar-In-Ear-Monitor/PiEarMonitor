using System.Threading.Tasks;
using Xamarin.Essentials;

namespace PiEarManager.View
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            Task.Run(() =>
            {
                ViewModel.LoadDevices();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ListView.ItemsSource = ViewModel.JsonData.Devices;
                });
            });
        }
    }
}