using Lazurite.IOC;
using Lazurite.MainDomain.MessageSecurity;
using LazuriteMobile.App.Controls;
using LazuriteMobile.App.Switches;
using LazuriteMobile.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LazuriteMobile.App
{
	public partial class MainPage : ContentPage
	{
        IScenariosManager _manager = Singleton.Resolve<IScenariosManager>();
        
        SynchronizationContext _currentContext = SynchronizationContext.Current;

        public MainPage()
		{
            this.InitializeComponent();
            _manager.NewScenarios += _manager_NewScenarios;
        }

        private void _manager_NewScenarios(Lazurite.MainDomain.ScenarioInfo[] scensInfos)
        {
            _currentContext.Post((state) => {
                var swgrid = new SwitchesGrid();
                swgrid.Initialize(scensInfos);
                this.grid.Children.Add(swgrid);
                var bt = new Button();
                bt.Clicked += (o, e) =>
                {
                    swgrid.EditMode = !swgrid.EditMode;
                };
                bt.WidthRequest = bt.HeightRequest = 40;
                bt.VerticalOptions = new LayoutOptions(LayoutAlignment.Start, false);
                bt.HorizontalOptions = new LayoutOptions(LayoutAlignment.Start, false);
                this.grid.Children.Add(bt);
            }, null);
        }
        
    }
}