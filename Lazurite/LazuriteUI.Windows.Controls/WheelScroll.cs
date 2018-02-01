using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Controls
{
    public class WheelScroll: ScrollViewer
    {
        static WheelScroll()
        {
            App.InitializeResources();
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WheelScroll), new FrameworkPropertyMetadata(typeof(WheelScroll)));
        }

        public WheelScroll()
        {
            VerticalScrollBarVisibility =
                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }
    }
}