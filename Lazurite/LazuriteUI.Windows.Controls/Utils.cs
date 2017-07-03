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
            var activatedWindow = App.Current.Windows.Cast<Window>().FirstOrDefault(x => x.IsActive);
            if (activatedWindow != null)
                return activatedWindow.Content as Panel;
            var mainWindow = App.Current.Windows.Cast<Window>().OrderBy(x => x.Name == "MainWindow").FirstOrDefault();
            return mainWindow?.Content as Panel;
        }
    }
}
