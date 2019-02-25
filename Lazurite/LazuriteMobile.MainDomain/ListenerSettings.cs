namespace LazuriteMobile.MainDomain
{
    public class ListenerSettings
    {
        public static readonly ListenerSettings WithoutEnergySave = new ListenerSettings(45000, 10000, 30000, 180000);
        public static readonly ListenerSettings Normal = new ListenerSettings(60000, 16000, 45000, 180000);
        public static readonly ListenerSettings EnergySave1 = new ListenerSettings(90000, 25000, 70000, 360000);
        public static readonly ListenerSettings EnergySave2 = new ListenerSettings(120000, 35000, 90000, 360000);
        public static readonly ListenerSettings Ultra = new ListenerSettings(10 * 60 * 1000, 60000, 5 * 60 * 1000, 60 * 60 * 1000);

        public ListenerSettings(int screenOffInterval, int screenOnInterval, int onErrorInterval, int powerSavingModeInterval)
        {
            ScreenOffInterval = screenOffInterval;
            ScreenOnInterval = screenOnInterval;
            OnErrorInterval = onErrorInterval;
            PowerSavingModeInterval = powerSavingModeInterval;
        }

        public ListenerSettings()
        {
            // Empty
        }

        public int ScreenOffInterval { get; set; }
        public int ScreenOnInterval { get; set; }
        public int OnErrorInterval { get; set; }
        public int PowerSavingModeInterval { get; set; }
    }
}