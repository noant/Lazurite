using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Controls
{
    public static class Utils
    {
        public static Panel GetMainWindowPanel()
        {
            return GetMainWindow()?.Content as Panel;
        }

        public static Window GetMainWindow()
        {
            var activatedWindow = App.Current.Windows.Cast<Window>().FirstOrDefault(x => x.IsActive);
            if (activatedWindow != null)
                return activatedWindow;
            return App.Current.Windows.Cast<Window>().OrderBy(x => x.Name == "MainWindow").FirstOrDefault();
        }
        
        public static System.Windows.Point GetMousePosition()
        {
            System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
            return new System.Windows.Point(point.X, point.Y);
        }
       
        public static double GetWindowTopBorderWithCaption(Window window)
        {
            var topBorder = GetWindowTopBorder(window);
            if (window.WindowStyle == WindowStyle.SingleBorderWindow)
                topBorder += SystemParameters.CaptionHeight;
            else if (window.WindowStyle == WindowStyle.ToolWindow)
                topBorder += SystemParameters.SmallCaptionHeight;
            else if (window.WindowStyle == WindowStyle.ThreeDBorderWindow)
                topBorder = topBorder*2 + SystemParameters.CaptionHeight;

            return topBorder;
        }

        private static double GetWindowTopBorder(Window window)
        {
            var topBorder = SystemParameters.FixedFrameHorizontalBorderHeight;
            if (window.ResizeMode == ResizeMode.CanResize)
                topBorder = SystemParameters.ResizeFrameHorizontalBorderHeight;
            return topBorder;
        }
    }
}
