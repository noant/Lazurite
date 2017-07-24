using LazuriteUI.Icons;
using System;
using System.Collections.Generic;
using System.IO;
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

        private static Dictionary<Icon, BitmapImage> Cache = new Dictionary<Icon, BitmapImage>();

        static IconView()
        {
            IconProperty = DependencyProperty.Register(nameof(Icon), typeof(Icon), typeof(IconView), 
                new FrameworkPropertyMetadata() {
                    PropertyChangedCallback = (o,e) =>
                    {   
                        var icon = (Icon)e.NewValue;
                        var control = (IconView)o;
                        if (icon != Icon.None)
                        {
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
                        else control.iconControl.Source = null;
                    }
                });
        }

        public IconView()
        {
            InitializeComponent();
            this.Icon = Icon.None;
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
