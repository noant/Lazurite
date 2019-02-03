using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Windows.Logging;
using System;
using System.Windows;

namespace LazuriteUI.Windows.Main.Journal
{
    public static class JournalManager
    {
        private static DataManagerBase DataManager = Singleton.Resolve<DataManagerBase>();

        private static WarnType? _maxShowingWarnType = null;
        public static WarnType MaxShowingWarnType
        {
            get
            {
                if (_maxShowingWarnType == null)
                {
                    if (DataManager.Has(nameof(MaxShowingWarnType)))
                        _maxShowingWarnType = DataManager.Get<WarnType>(nameof(MaxShowingWarnType));
                    else
                        MaxShowingWarnType = WarnType.Error;
                }
                return _maxShowingWarnType.Value;
            }
            set
            {
                _maxShowingWarnType = value;
                DataManager.Set(nameof(MaxShowingWarnType), _maxShowingWarnType);
            }
        }
        
        public static void Set(string message, WarnType type, Exception e = null, bool showAnyway = false)
        {
            JournalView.Set(message, type);
            if (type <= MaxShowingWarnType || showAnyway)
                JournalLightWindow.Show(message, type);
            if (type == WarnType.Fatal)
            {
                MessageBox.Show(message + "\r\n" + (e?.Message ?? string.Empty), "Критическая ошибка! Lazurite будет закрыт.", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Windows.Application.Current.Shutdown(1);
            }
        }
    }
}
