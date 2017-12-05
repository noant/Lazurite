using Lazurite.MainDomain;
using LazuriteMobile.App.Controls;
using System;

using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public partial class StatusView : ContentView
    {
        public StatusView()
        {
            InitializeComponent();
        }

        public StatusView(ScenarioInfo scenario) : this()
        {
            this.BindingContext = new SwitchScenarioModel(scenario);
            itemView.Click += ItemView_Click;
        }

        private void ItemView_Click(object sender, EventArgs e)
        {
            var statusSwitch = new StatusViewSwitch((SwitchScenarioModel)this.BindingContext);
            var dialog = new DialogView(statusSwitch);
            statusSwitch.StateChanged += (o, e2) => dialog.Close();
            dialog.Show(Helper.GetLastParent(this));
        }
    }
}
