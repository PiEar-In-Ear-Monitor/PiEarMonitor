using System.Threading.Tasks;
using PiEarManager.Helpers;
using PiEarManager.Interfaces;
using PiEarManager.View;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace PiEarManager
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();
            MainPage = new MainPage();
            Task.Run(Init);
        }
        private static void Init()
        {
            var service = DependencyService.Get<IMulticastLock>();
            service.Acquire();
            Networking.FindServerIp();
            while (Networking.ServerIp == null)
            {
                Task.Delay(500).Wait();
            }
            service.Release();
        }
    }
}