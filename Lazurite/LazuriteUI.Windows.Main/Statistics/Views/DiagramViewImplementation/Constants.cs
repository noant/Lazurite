using System.Windows;
using System.Windows.Media;

namespace LazuriteUI.Windows.Main.Statistics.Views.DiagramViewImplementation
{
    public static class Constants
    {
        public static readonly Thickness DiagramsMargin = new Thickness(10);
        public static readonly int ScaleLeftMargin = 40;
        public static readonly GridLength ScaleLeftMarginGrid = new GridLength(ScaleLeftMargin, GridUnitType.Pixel);

        public static readonly Brush GraphicScaleColor = Brushes.Gray;
        public static readonly Brush GraphicLineColor = Brushes.SteelBlue;
    }
}
