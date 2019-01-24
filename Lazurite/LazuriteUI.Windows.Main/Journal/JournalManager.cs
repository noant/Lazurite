using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.Windows.Logging;
using System;
using System.Windows;

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
