using Lazurite.Windows.Logging;
using LazuriteUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Journal
{
    public static class JournalManager
    {
        public static void Set(string message, WarnType type, Exception e = null)
        {
            JournalView.Set(message, type);
            if (type != WarnType.Debug)
                JournalLightWindow.Show(message, type);
            if (type == WarnType.Error || type == WarnType.Fatal || type == WarnType.Warn)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                    if (Application.Current.Windows.Cast<Window>().Any(x => x is MainWindow))
                    {
                        var mainWindow = Application.Current.Windows.Cast<Window>().First(x => x is MainWindow);
                        switch (type)
                        {
                            case WarnType.Fatal:
                                {
                                    MessageView.ShowMessage(message + "\r\n" + e?.Message, "Критическая ошибка!", Icons.Icon.Close, mainWindow.Content as Panel, () => Application.Current.Shutdown(1));
                                    break;
                                }
                            case WarnType.Error:
                                {
                                    MessageView.ShowMessage(e.Message + "\r\n" + e?.Message, "Ошибка!", Icons.Icon.Bug, mainWindow.Content as Panel);
                                    break;
                                }
                            case WarnType.Warn:
                                {
                                    MessageView.ShowMessage(e.Message + "\r\n" + e?.Message, "Внимание!", Icons.Icon.Alert, mainWindow.Content as Panel);
                                    break;
                                }
                        }
                    }
                }));
            }
        }
    }
}
