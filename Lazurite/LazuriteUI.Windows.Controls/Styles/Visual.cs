using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LazuriteUI.Windows.Controls
{
    public static class Visual
    {
        static Visual()
        {
            var resource = new ResourceDictionary();
            resource.Source = new Uri(@"/LazuriteUI.Windows.Controls;component/Styles/Styles.xaml", UriKind.RelativeOrAbsolute);
            BackgroundCubes = resource["BackCubes"] as Brush;
        }

        public static readonly Brush ItemBackground = new SolidColorBrush(Color.FromRgb(37,37,37));
        public static readonly Brush BrightItemBackground = new SolidColorBrush(Color.FromRgb(60,60,60));
        public static readonly Brush Background = new SolidColorBrush(Color.FromRgb(26,26,26));
        public static readonly Brush SplitterBrush = new SolidColorBrush(Color.FromRgb(38, 38, 38));
        public static readonly Brush BackgroundLazurite = new SolidColorBrush(Color.FromRgb(29,25,29));
        public static readonly Brush Foreground = Brushes.White;
        public static readonly int FontSize = 14;
        public static readonly int BigFontSize = 17;
        public static readonly FontFamily FontFamily = new FontFamily("Calibri");
        public static readonly FontWeight FontWeight = System.Windows.FontWeight.FromOpenTypeWeight(2);

        public static readonly Brush BackgroundCubes;
    }
}
