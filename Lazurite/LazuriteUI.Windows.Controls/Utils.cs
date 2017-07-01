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
            var mainWindow = App.Current.Windows.Cast<Window>().OrderBy(x => x.Name == "MainWindow").FirstOrDefault();
            return mainWindow?.Content as Panel;
        }
    }
}
