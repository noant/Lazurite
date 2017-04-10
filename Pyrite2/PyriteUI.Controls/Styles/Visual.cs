using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PyriteUI.Controls
{
    public static class Visual
    {
        public static readonly Brush Background = new SolidColorBrush(Color.FromRgb(37,37,37));
        public static readonly Brush BrightBackground = new SolidColorBrush(Color.FromRgb(60,60,60));
        public static readonly Brush Foreground = Brushes.White;
        public static readonly int FontSize = 19;
        public static readonly int BigFontSize = 23;
        public static readonly FontFamily FontFamily = new FontFamily("Calibri");
        public static readonly FontWeight FontWeight = System.Windows.FontWeight.FromOpenTypeWeight(2); 
    }
}
