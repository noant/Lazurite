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
    public partial class FloatView : ContentView
    {
        private SwitchScenarioModel _model;
        public FloatView()
        {
            InitializeComponent();
        }
        
        public FloatView(ScenarioInfo scenario) : this()
        {
            _model = new SwitchScenarioModel(scenario);
            this.BindingContext = _model;
            itemView.Click += itemView_Click;
        }
        private void itemView_Click(object sender, EventArgs e)
        {
            var floatSwitch = new FloatViewSwitch(_model);
            var dialog = new DialogView(floatSwitch);
            dialog.Show(Helper.GetLastParent(this));
        }
    }
}
