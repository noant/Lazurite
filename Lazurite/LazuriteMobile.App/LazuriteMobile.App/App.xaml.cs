using Lazurite.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LazuriteMobile.App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Task.Delay(3000).Wait();
            
            Singleton.Add(new ScenariosManager());
            Singleton.Resolve<ScenariosManager>().Initialize("192.168.0.100", 8087, "Lazurite", "user1", "pass", "0123456789123456");

            MainPage = new LazuriteMobile.App.MainPage();
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
