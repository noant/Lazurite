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
    /// Логика взаимодействия для IconView.xaml
    /// </summary>
    public partial class IconView : UserControl
    {
        public static readonly DependencyProperty IconProperty;

        static IconView()
        {
            IconProperty = DependencyProperty.Register(nameof(Icon), typeof(Icon), typeof(IconView), 
                new FrameworkPropertyMetadata() {
                    PropertyChangedCallback = (o,e) =>
                    {
                        var icon = (Icon)e.NewValue;
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.StreamSource = Utils.GetIconData(icon);
                        image.EndInit();
                        ((IconView)o).iconControl.Source = image;
                    }
                });
        }

        public IconView()
        {
            InitializeComponent();
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
    }
}
