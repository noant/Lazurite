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

        DialogView _tempDialog;

        SynchronizationContext _currentContext = SynchronizationContext.Current;

        public MainPage()
		{
            this.InitializeComponent();
            _manager.NewScenarios += _manager_NewScenarios;
        }

        private void _manager_NewScenarios(Lazurite.MainDomain.ScenarioInfo[] scensInfos)
        {
            _currentContext.Post((state) => {
                if (_tempDialog != null)
                    _tempDialog.Close();

                //var listItems = new ListItemsView();

                //foreach (var scenarioInfo in obj)
                //    listItems.Children.Add(new ItemView() { Text = scenarioInfo.Name });
                //var view = new StackLayout() {
                //    Orientation = StackOrientation.Vertical
                //};
                //foreach (var sinfo in obj)
                //    view.Children.Add(SwitchesCreator.CreateScenarioControl(sinfo, null));


                var grid = new SwitchesGrid();
                grid.Initialize(scensInfos, new Lazurite.MainDomain.UserVisualSettings[0]);

                _tempDialog = new DialogView(grid);
                _tempDialog.Show(this.grid);
            }, null);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
        }
    }
}