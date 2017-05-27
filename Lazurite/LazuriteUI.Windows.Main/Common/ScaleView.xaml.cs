using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                        var value = (double)e.NewValue;
                        var scaleView = o as ScaleView;
                        var percent = value / (scaleView.Max - scaleView.Min);
                        var marginBottom = scaleView.ActualHeight * percent;
                        scaleView.borderValue.Height = marginBottom;
                    }
                });
            MaxProperty = DependencyProperty.Register(nameof(Max), typeof(double), typeof(ScaleView));
            MinProperty = DependencyProperty.Register(nameof(Min), typeof(double), typeof(ScaleView));
        }

        public ScaleView()
        {
            InitializeComponent();
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
