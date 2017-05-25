using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LazuriteMobile.App.Common
{
    public class NumericEntry: Entry
    {
        public static BindableProperty MaxProperty = BindableProperty.Create(nameof(Max), typeof(double), typeof(NumericEntry), (double)0);
        public static BindableProperty MinProperty = BindableProperty.Create(nameof(Min), typeof(double), typeof(NumericEntry), (double)100);
        public static BindableProperty ValueProperty;

        static NumericEntry()
        {
            ValueProperty = BindableProperty.Create(nameof(Value), typeof(double), typeof(NumericEntry), (double)0, BindingMode.TwoWay, 
                (sender, value) => {
                    var entry = sender as NumericEntry;
                    var val = (double)value;
                    return val <= entry.Max && val >= entry.Min;
                }, 
                (sender, oldVal, newVal) => {
                    var entry = sender as NumericEntry;
                    entry.Text = newVal.ToString();
                });
        }

        public NumericEntry()
        {
            this.Keyboard = Keyboard.Numeric;
            this.TextChanged += NumericEntry_TextChanged;
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

        private void NumericEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
                this.Value = this.Min;
            else
            {
                double value;
                if (!double.TryParse(e.NewTextValue, out value))
                {
                    this.Text = e.OldTextValue;
                }
                else
                {
                    if (value >= Min && value <= Max)
                        Value = value;
                    else
                        this.Text = e.OldTextValue;
                }
            }
        }
    }
}
