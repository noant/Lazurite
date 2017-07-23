using LazuriteUI.Icons;
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

            this.MouseEnter += (o, e) => iconView.Background = Brushes.SteelBlue;
            this.MouseLeave += (o, e) => iconView.Background = Brushes.Transparent;
            this.MouseLeftButtonUp += (o, e) => Click?.Invoke(this, new RoutedEventArgs());
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
