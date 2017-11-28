using Lazurite.MainDomain;
using Lazurite.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public partial class FloatViewSwitch : ContentView
    {
        public FloatViewSwitch()
        {
            InitializeComponent();
        }

        public FloatViewSwitch(SwitchScenarioModel model) : this()
        {
            this.BindingContext = model;
            //binding works incorrectly
            slider.Maximum = model.Max; //crutch
            slider.Minimum = model.Min; //crutch
            slider.Value = double.Parse(model.ScenarioValue);
            slider.ValueChanged += (o, e) => RaiseSliderValueChanged();
        }

        //binding works incorrectly
        private void RaiseSliderValueChanged()
        {
            var model = ((SwitchScenarioModel)this.BindingContext);
            var newVal = slider.Value.ToString();
            if (!model.ScenarioValue.Equals(newVal))
                SomeOtherUtils.DoManyTimes(this, () => model.ScenarioValue = newVal);
        }
    }
}
