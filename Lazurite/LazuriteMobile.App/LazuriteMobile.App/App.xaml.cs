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
        public App()
        {
            if (!Singleton.Any<IScenariosManager>())
                Singleton.Add(new ScenariosManager());

            InitializeComponent();
            MainPage = new LazuriteMobile.App.MainPage();

            Singleton.Resolve<IScenariosManager>().Initialize();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
