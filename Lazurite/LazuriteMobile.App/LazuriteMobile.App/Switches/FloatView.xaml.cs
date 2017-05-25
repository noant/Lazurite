using Lazurite.MainDomain;
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
        public FloatView()
        {
            InitializeComponent();
        }

        public FloatView(ScenarioInfo scenario, UserVisualSettings visualSettings) : this()
        {
            this.BindingContext = new ScenarioModel(scenario, visualSettings);
            //binding works incorrectly
            slider.Value = double.Parse(scenario.CurrentValue);
        }

        //binding works incorrectly
        private void slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var model = ((ScenarioModel)this.BindingContext);
            var newVal = slider.Value.ToString();
            if (!model.ScenarioValue.Equals(newVal))
                model.ScenarioValue = newVal;
        }
    }
}
