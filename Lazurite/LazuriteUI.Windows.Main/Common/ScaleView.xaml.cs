using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LazuriteUI.Windows.Main.Common
{
    /// <summary>
    /// Логика взаимодействия для ScaleView.xaml
    /// </summary>
    public partial class ScaleView : UserControl
    {
        public static DependencyProperty ValueProperty;
        public static DependencyProperty MaxProperty;
        public static DependencyProperty MinProperty;

        static ScaleView()
        {
            ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(ScaleView),
                new FrameworkPropertyMetadata() {
                    DefaultValue = 0.0,
                    PropertyChangedCallback = (o,e) => {
                        ((ScaleView)o).AllocateScaleViewSize();
                    }
                });
            MaxProperty = DependencyProperty.Register(nameof(Max), typeof(double), typeof(ScaleView), new FrameworkPropertyMetadata(100.0));
            MinProperty = DependencyProperty.Register(nameof(Min), typeof(double), typeof(ScaleView), new FrameworkPropertyMetadata(0.0));
        }

        public ScaleView()
        {
            InitializeComponent();
        }
        
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            AllocateScaleViewSize();
        }

        private void AllocateScaleViewSize()
        {
            var value = Value;
            var percent = (value - Min) / (Max - Min);
            var marginBottom = ActualHeight * percent;
            if (marginBottom < 0)
                marginBottom = 0;
            borderValue.Height = marginBottom;
            
            if (percent < 0.25)
                borderValue.BorderBrush = new SolidColorBrush(Colors.Gray);
            else if (percent >= 0.25 && percent < 0.9)
                borderValue.BorderBrush = Brushes.LightSkyBlue;
            else
                borderValue.BorderBrush = new SolidColorBrush(Colors.Red);
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
