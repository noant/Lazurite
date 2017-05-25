using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public partial class ButtonView : ContentView
    {
        private ScenarioModel _model;
        public ButtonView()
        {
            InitializeComponent();
        }
        
        public ButtonView(ScenarioInfo scenario, UserVisualSettings visualSettings) : this()
        {
            _model = new ScenarioModel(scenario, visualSettings);
            this.BindingContext = _model;
            itemView.Click += ItemView_Click;
        }

        private void ItemView_Click(object arg1, EventArgs arg2)
        {
            _model.ScenarioValue = string.Empty;
        }
    }
}
