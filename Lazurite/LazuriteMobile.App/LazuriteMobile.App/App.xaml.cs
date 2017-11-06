using Lazurite.IOC;
using LazuriteMobile.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace LazuriteMobile.App
{
    public partial class App : Application
    {
        private IScenariosManager _manager;

        public App()
        {
            if (!Singleton.Any<IScenariosManager>())
                Singleton.Add(_manager = new ScenariosManager());

            InitializeComponent();
            MainPage = new LazuriteMobile.App.MainPage();
        }
        
        protected override void OnStart()
        {
            _manager.Initialize();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            if (!_manager.Connected)
                _manager.Initialize();
        }
    }
}
