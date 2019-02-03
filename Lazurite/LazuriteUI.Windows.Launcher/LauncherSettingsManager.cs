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
            var fileDataManager = new FileDataManager();
            if (fileDataManager.Has(LauncherSettingsKey))
            {
                Settings = fileDataManager.Get<LauncherSettings>(LauncherSettingsKey);
                SaveSettings();
            }
        }

        public void SaveSettings()
        {
            var fileDataManager = new FileDataManager();
            fileDataManager.Set(LauncherSettingsKey, Settings);
        }
    }
}
