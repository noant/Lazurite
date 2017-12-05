using Lazurite.MainDomain;
using LazuriteMobile.App.Controls;
using System;

using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public partial class DateTimeView : ContentView
    {
        public DateTimeView()
        {
            InitializeComponent();
        }

        public DateTimeView(ScenarioInfo scenario) : this()
        {
            this.BindingContext = new SwitchScenarioModel(scenario);
            itemView.Click += ItemView_Click;
        }

        private void ItemView_Click(object sender, EventArgs e)
        {
            var dateTime = DateTime.Now;
            DateTime.TryParse(((SwitchScenarioModel)this.BindingContext).ScenarioValue, out dateTime);
            var dateTimeSwitch = new DateTimeViewSwitch()
            {
                DateTime = dateTime
            };
            var dialog = new DialogView(dateTimeSwitch);
            dateTimeSwitch.Apply += (o, args) =>
            {
                dialog.Close();
                ((SwitchScenarioModel)this.BindingContext).ScenarioValue = dateTimeSwitch.DateTime.ToString();
            };
            dialog.Show(Helper.GetLastParent(this));
        }
    }
}
