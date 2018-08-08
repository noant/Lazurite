using Lazurite.MainDomain;
using LazuriteMobile.App.Controls;
using LazuriteMobile.App.Switches.Bases;
using System;

using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public partial class DateTimeView : SwitchBase
    {
        public DateTimeView()
        {
            InitializeComponent();
        }

        public DateTimeView(ScenarioInfo scenario) : this()
        {
            BindingContext = new SwitchScenarioModel(scenario);
            itemView.Click += ItemView_Click;
        }

        private void ItemView_Click(object sender, EventArgs e)
        {
            var dateTime = DateTime.Now;
            DateTime.TryParse(((SwitchScenarioModel)BindingContext).ScenarioValue, out dateTime);
            var dateTimeSwitch = new DateTimeViewSwitch((SwitchScenarioModel)BindingContext)
            {
                DateTime = dateTime
            };
            var dialog = new DialogView(dateTimeSwitch);
            dateTimeSwitch.Apply += (o, args) =>
            {
                dialog.Close();
                ((SwitchScenarioModel)BindingContext).ScenarioValue = dateTimeSwitch.DateTime.ToString();
            };
            dialog.Show(Helper.GetLastParent(this));
        }
    }
}
