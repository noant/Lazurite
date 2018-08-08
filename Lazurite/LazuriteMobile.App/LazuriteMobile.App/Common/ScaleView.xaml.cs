using System;

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
            SizeChanged += (o, e) => RefreshScale();
        }
                
        private void RefreshScale()
        {
            var percent = (Value - Min) / (Max - Min);
            var marginBottom = Height * percent;
            if (marginBottom < 0)
                marginBottom = 0;
            gridValue.HeightRequest = marginBottom;
            if (percent < 0.25)
                gridValue.BackgroundColor = Color.Gray;
            else if (percent >= 0.25 && percent < 0.9)
                gridValue.BackgroundColor = Color.MediumOrchid;
            else
                gridValue.BackgroundColor = Color.Red;
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
