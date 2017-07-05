using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
