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
        private ScenarioModel _model;
        public FloatView()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                var a = "as";
            }
        }
        
        public FloatView(ScenarioInfo scenario, UserVisualSettings visualSettings) : this()
        {
            _model = new ScenarioModel(scenario, visualSettings);
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
