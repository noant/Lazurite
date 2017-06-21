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
            slider.ValueChanged += (o, e) => RaiseSliderValueChanged();
        }

        public FloatViewSwitch(ScenarioModel model) : this()
        {
            this.BindingContext = model;
            //binding works incorrectly
            slider.Value = double.Parse(model.ScenarioValue);
        }

        //binding works incorrectly
        private void RaiseSliderValueChanged()
        {
            var model = ((ScenarioModel)this.BindingContext);
            var newVal = slider.Value.ToString();
            if (!model.ScenarioValue.Equals(newVal))
                SomeOtherUtils.DoManyTimes(this, () => model.ScenarioValue = newVal);
        }
    }
}
