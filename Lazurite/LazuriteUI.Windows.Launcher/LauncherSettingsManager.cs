using Lazurite.Data;

namespace LazuriteUI.Windows.Launcher
{
    public class LauncherSettingsManager
    {
        private static readonly string LauncherSettingsKey = "launcherSettings";

        public LauncherSettingsManager()
        {
            LoadSettings();
        }

        public LauncherSettings Settings { get; private set; } = new LauncherSettings();

        public void LoadSettings()
        {
            var fileSavior = new FileSavior();
            if (fileSavior.Has(LauncherSettingsKey))
            {
                Settings = fileSavior.Get<LauncherSettings>(LauncherSettingsKey);
                SaveSettings();
            }
        }

        public void SaveSettings()
        {
            var fileSavior = new FileSavior();
            fileSavior.Set(LauncherSettingsKey, Settings);
        }
    }
}
