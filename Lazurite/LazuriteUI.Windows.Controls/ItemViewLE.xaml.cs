using LazuriteUI.Icons;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LazuriteUI.Windows.Controls
{
    /// <summary>
    /// Логика взаимодействия для ItemViewLE.xaml
    /// </summary>
    public partial class ItemViewLE : UserControl
    {
        public static readonly DependencyProperty IconProperty;

        static ItemViewLE()
        {
            IconProperty = DependencyProperty.Register(nameof(Icon), typeof(Icon), typeof(ItemViewLE), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = (o, e) =>
                {
                    ((ItemViewLE)o).iconView.Icon = (Icon)e.NewValue;
                }
            });
        }

        public ItemViewLE()
        {
            InitializeComponent();

            MouseEnter += (o, e) => iconView.Background = Brushes.SteelBlue;
            MouseLeave += (o, e) => iconView.Background = Brushes.Transparent;
            MouseLeftButtonUp += (o, e) => Click?.Invoke(this, new RoutedEventArgs());
        }

        public Icon Icon
        {
            get
            {
                return (Icon)GetValue(IconProperty);
            }
            set
            {
                SetValue(IconProperty, value);
            }
        }

        public event RoutedEventHandler Click;
    }
}
