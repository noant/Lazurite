using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Controls
{
    /// <summary>
    /// Логика взаимодействия для NumericUpDown.xaml
    /// </summary>
    /// platform dependent
    public partial class NumericUpDown : UserControl
    {
        static DependencyProperty MaxValueProperty;
        static DependencyProperty MinValueProperty;
        static DependencyProperty ValueProperty;

        static NumericUpDown()
        {
            MaxValueProperty = DependencyProperty.Register(nameof(MaxValue), typeof(decimal), typeof(NumericUpDown),
                new FrameworkPropertyMetadata((decimal)100) {
                    PropertyChangedCallback = (o, e) => {
                        ((NumericUpDown)o).GetNumericUD().Maximum = (decimal)e.NewValue;
                    }
                });
            MinValueProperty = DependencyProperty.Register(nameof(MinValue), typeof(decimal), typeof(NumericUpDown),
                new FrameworkPropertyMetadata((decimal)0)
                {
                    PropertyChangedCallback = (o, e) => {
                        ((NumericUpDown)o).GetNumericUD().Minimum = (decimal)e.NewValue;
                    }
                });
            ValueProperty = DependencyProperty.Register(nameof(Value), typeof(decimal), typeof(NumericUpDown),
                new FrameworkPropertyMetadata((decimal)0)
                {
                    PropertyChangedCallback = (o, e) => {
                        ((NumericUpDown)o).GetNumericUD().Value = (decimal)e.NewValue;
                    }
                });
        }

        public decimal MaxValue
        {
            get
            {
                return (decimal)GetValue(MaxValueProperty);
            }
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }

        public decimal MinValue
        {
            get
            {
                return (decimal)GetValue(MinValueProperty);
            }
            set
            {
                SetValue(MinValueProperty, value);
            }
        }

        public decimal Value
        {
            get
            {
                return (decimal)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public NumericUpDown()
        {
            InitializeComponent();
            var nud = new System.Windows.Forms.NumericUpDown()
            {
                Dock = System.Windows.Forms.DockStyle.Fill
            };
            nud.ValueChanged += (o, e) => Value = nud.Value;
            wfHost.Child = nud;
        }

        private System.Windows.Forms.NumericUpDown GetNumericUD()
        {
            return wfHost.Child as System.Windows.Forms.NumericUpDown;
        }
    }
}
