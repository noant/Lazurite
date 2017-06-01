﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace LazuriteMobile.App.Common
{
    public partial class ScaleView : ContentView
    {
        public static BindableProperty ValueProperty;
        public static BindableProperty MaxProperty;
        public static BindableProperty MinProperty;

        static ScaleView()
        {
            ValueProperty = BindableProperty.Create(nameof(Value), typeof(double), typeof(ScaleView), 0.0, BindingMode.Default, null, 
                (o, oldVal, newVal) => {
                    ((ScaleView)o).RefreshScale();
                }
            );
            MaxProperty = BindableProperty.Create(nameof(Max), typeof(double), typeof(ScaleView), 100.0);
            MinProperty = BindableProperty.Create(nameof(Min), typeof(double), typeof(ScaleView), 0.0);
        }

        public ScaleView()
        {
            InitializeComponent();
            this.SizeChanged += (o, e) => RefreshScale();
        }
                
        private void RefreshScale()
        {
            var percent = Value / (this.Max - this.Min);
            var marginBottom = this.Height * percent;
            this.gridValue.HeightRequest = marginBottom;
        }

        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public double Min
        {
            get
            {
                return (double)GetValue(MinProperty);
            }
            set
            {
                SetValue(MinProperty, value);
            }
        }

        public double Max
        {
            get
            {
                return (double)GetValue(MaxProperty);
            }
            set
            {
                SetValue(MaxProperty, value);
            }
        }
    }
}
