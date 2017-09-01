using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main
{
    public static class Utils
    {
        public static void RestartApp()
        {
            System.Diagnostics.Process.Start(Lazurite.Windows.Utils.Utils.GetAssemblyPath(typeof(App).Assembly));
            App.Current.Shutdown();
        }
    }
}