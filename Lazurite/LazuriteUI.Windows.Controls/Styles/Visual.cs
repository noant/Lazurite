using System;
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

        public static readonly Brush ItemBackground = new SolidColorBrush(Color.FromRgb(37,37,45));
        public static readonly Brush ItemSelection = Brushes.SlateBlue;
        public static readonly Brush BrightItemBackground = new SolidColorBrush(Color.FromRgb(60,60,68));
        public static readonly Brush Background = new SolidColorBrush(Color.FromRgb(26,26,34));
        public static readonly Brush SplitterBrush = new SolidColorBrush(Color.FromRgb(38, 38, 46));
        public static readonly Brush BackgroundLazurite = new SolidColorBrush(Color.FromRgb(29, 25, 39));
        public static readonly Brush Foreground = Brushes.LightGray;
        public static readonly Brush BorderBrush = new SolidColorBrush(Color.FromRgb(17,34,39));
        public static readonly Brush CaptionForeground = Brushes.SteelBlue;
        public static readonly double FontSize = 14;
        public static readonly double BigFontSize = 17;
        public static readonly FontFamily FontFamily = new FontFamily("Segoe UI");
        public static readonly FontWeight FontWeight = System.Windows.FontWeight.FromOpenTypeWeight(2);

        public static readonly Brush SwitchBackground = ItemBackground;
        public static readonly Brush SwitchBackgroundReadonly = new SolidColorBrush(Color.FromRgb(0, 49, 83));

        public static readonly Brush SelectedSwitchBackground = ItemSelection;
        public static readonly Brush SelectedSwitchBackgroundReadonly = new SolidColorBrush(Color.FromRgb(0, 80, 130));

        public static readonly Brush BackgroundCubes;
    }
}
