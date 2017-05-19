using Lazurite.IOC;
using Lazurite.MainDomain.MessageSecurity;
using LazuriteMobile.App.Controls;
using LazuriteMobile.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LazuriteMobile.App
{
	public partial class MainPage : ContentPage
	{
        IServiceClientManager _clientManager;
        IServiceClient _client;
        public void InitializeClientManager()
        {
            _clientManager = Singleton.Resolve<IServiceClientManager>();
            _client = _clientManager.Create("192.168.0.100", 8080, "Lazurite", "0123456789123456", "user1", "pass");
        }

        public MainPage()
		{
            this.InitializeComponent();

            InitializeClientManager();

            var listItems = new ListItemsView();

            listItems.Children.Add(new ItemView() { Text = "adds" });
            listItems.Children.Add(new ItemView() { Text = "ad12ds" });
            listItems.Children.Add(new ItemView() { Text = "adsds" });
            listItems.Children.Add(new ItemView() { Text = "asdds" });

            new DialogView(listItems).Show(grid);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            _client.BeginGetScenariosInfo((result) => {
                var res = _client.EndGetScenariosInfo(result).Decrypt("0123456789123456");
                var scenario = res[0];
                if (res[0].CurrentValue == "ON" || res[0].CurrentValue == "True")
                    _client.BeginExecuteScenario(new Encrypted<string>(res[0].ScenarioId, "0123456789123456"), new Encrypted<string>("OFF", "0123456789123456"), null, null);
                else
                    _client.BeginExecuteScenario(new Encrypted<string>(res[0].ScenarioId, "0123456789123456"), new Encrypted<string>("ON", "0123456789123456"), null, null);

            }, null);
        }
    }
}