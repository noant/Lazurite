using Lazurite.Data;
using Lazurite.IOC;
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
        private static SaviorBase Savior = Singleton.Resolve<SaviorBase>();

        private static WarnType? _maxShowingWarnType = null;
        public static WarnType MaxShowingWarnType
        {
            get
            {
                if (_maxShowingWarnType == null)
                {
                    if (Savior.Has(nameof(MaxShowingWarnType)))
                        _maxShowingWarnType = Savior.Get<WarnType>(nameof(MaxShowingWarnType));
                    else
                        MaxShowingWarnType = WarnType.Error;
                }
                return _maxShowingWarnType.Value;
            }
            set
            {
                _maxShowingWarnType = value;
                Savior.Set(nameof(MaxShowingWarnType), _maxShowingWarnType);
            }
        }

        private static readonly object _locker = new object();

        public static void Set(string message, WarnType type, Exception e = null)
        {
            JournalView.Set(message, type);
            if (type <= MaxShowingWarnType)
                JournalLightWindow.Show(message, type);
            if (type == WarnType.Error || type == WarnType.Fatal)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
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
                                    MessageView.ShowMessage(message + "\r\n" + e?.Message, "Ошибка!", Icons.Icon.Bug, mainWindow.Content as Panel);
                                    break;
                                }
                        }
                    }
                }));
            }
        }
    }
}
