using Lazurite.IOC;
using Lazurite.MainDomain;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace LazuriteMobile.App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new LazuriteMobile.App.MainPage();
        }
        
        protected override void OnStart()
        {
        }
        
        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
