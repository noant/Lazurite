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
        public static DependencyProperty IsTextVisibleProperty;

        static ScaleView()
        {
            ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(ScaleView),
                new FrameworkPropertyMetadata() {
                    DefaultValue = 0.0,
                    PropertyChangedCallback = (o,e) => {
                        ((ScaleView)o).AllocateScaleViewSize();
                    }
                });
            IsTextVisibleProperty = DependencyProperty.Register(nameof(IsTextVisible), typeof(bool), typeof(ScaleView),
                new FrameworkPropertyMetadata()
                {
                    DefaultValue = true,
                    PropertyChangedCallback = (o, e) => {
                        ((ScaleView)o).tbValue.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
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
            var percent = (value - this.Min) / (this.Max - this.Min);
            var marginBottom = this.ActualHeight * percent;
            if (marginBottom < 0)
                marginBottom = 0;
            this.borderValue.Height = marginBottom;

            tbValue.Text = Math.Round(Value).ToString();
            if (percent < 0.25)
                tbValue.Foreground = new SolidColorBrush(Colors.Gray);
            else if (percent >= 0.25 && percent < 0.9)
                tbValue.Foreground = new SolidColorBrush(Color.FromRgb(80,144,180));
            else
                tbValue.Foreground = new SolidColorBrush(Colors.Red);
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
        
        public bool IsTextVisible
        {
            get
            {
                return (bool)GetValue(IsTextVisibleProperty);
            }
            set
            {
                SetValue(IsTextVisibleProperty, value);
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
