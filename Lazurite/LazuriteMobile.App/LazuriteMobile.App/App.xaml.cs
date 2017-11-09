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
            _manager = Singleton.Resolve<IScenariosManager>();
            InitializeComponent();
            MainPage = new LazuriteMobile.App.MainPage();
        }
        
        protected override void OnStart()
        {
            if (!_manager.Connected)
                _manager.Initialize();
        }
        
        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
