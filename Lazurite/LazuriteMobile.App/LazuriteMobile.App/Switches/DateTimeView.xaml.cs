using Lazurite.MainDomain;
using LazuriteMobile.App.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public partial class DateTimeView : ContentView
    {
        public DateTimeView()
        {
            InitializeComponent();
        }

        public DateTimeView(ScenarioInfo scenario, UserVisualSettings visualSettings) : this()
        {
            this.BindingContext = new ScenarioModel(scenario, visualSettings);
            itemView.Click += ItemView_Click;
        }

        private void ItemView_Click(object sender, EventArgs e)
        {
            var dateTimeSwitch = new DateTimeViewSwitch()
            {
                DateTime = DateTime.Parse(((ScenarioModel)this.BindingContext).ScenarioValue)
            };
            var dialog = new DialogView(dateTimeSwitch);
            dateTimeSwitch.Apply += (o, args) =>
            {
                dialog.Close();
                ((ScenarioModel)this.BindingContext).ScenarioValue = dateTimeSwitch.DateTime.ToString();
            };
            dialog.Show(Helper.GetLastParent(this));
        }
    }
}
