using LazuriteUI.Icons;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace LazuriteUI.Windows.Controls
{
    /// <summary>
    /// Логика взаимодействия для IconView.xaml
    /// </summary>
    public partial class IconView : UserControl
    {
        public static readonly DependencyProperty IconProperty;

        private static Dictionary<Icon, BitmapImage> Cache = new Dictionary<Icon, BitmapImage>();

        static IconView()
        {
            IconProperty = DependencyProperty.Register(nameof(Icon), typeof(Icon), typeof(IconView),
                new FrameworkPropertyMetadata(Icon._None) {
                    PropertyChangedCallback = (o,e) =>
                    {   
                        var icon = (Icon)e.NewValue;
                        var control = (IconView)o;
                        if (Cache.ContainsKey(icon))
                        {
                            control.iconControl.Source = Cache[icon];
                        }
                        else
                        {
                            var image = new BitmapImage();
                            image.BeginInit();
                            image.StreamSource = LazuriteUI.Icons.Utils.GetIconData(icon);
                            image.CacheOption = BitmapCacheOption.OnDemand;
                            image.EndInit();
                            Cache.Add(icon, image);
                            control.iconControl.Source = image;
                        }
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
