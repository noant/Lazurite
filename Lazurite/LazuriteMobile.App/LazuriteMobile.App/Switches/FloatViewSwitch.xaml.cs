﻿using Lazurite.MainDomain;
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

        public FloatViewSwitch(ScenarioModel model) : this()
        {
            this.BindingContext = model;
            //binding works incorrectly
            slider.Value = double.Parse(model.ScenarioValue);
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