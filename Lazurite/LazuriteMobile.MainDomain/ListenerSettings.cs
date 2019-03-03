namespace LazuriteMobile.MainDomain
{
    public class ListenerSettings
    {
        public static readonly ListenerSettings WithoutEnergySave = new ListenerSettings(45000, 10000, 30000, 180000, true);
        public static readonly ListenerSettings Normal1 = new ListenerSettings(60000, 17000, 45000, 180000, true);
        public static readonly ListenerSettings Normal2 = new ListenerSettings(60000, 17000, 45000, 180000, false);
        public static readonly ListenerSettings EnergySave1 = new ListenerSettings(90000, 25000, 70000, 360000, false);
        public static readonly ListenerSettings EnergySave2 = new ListenerSettings(120000, 35000, 90000, 360000, false);
        public static readonly ListenerSettings Ultra = new ListenerSettings(10 * 60 * 1000, 60000, 5 * 60 * 1000, 60 * 60 * 1000, false);

        public ListenerSettings(int screenOffInterval, int screenOnInterval, int onErrorInterval, int powerSavingModeInterval, bool useCPUInBackground)
        {
            UseCPUInBackground = useCPUInBackground;
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
        public bool UseCPUInBackground { get; set; } // WakeLock for Android

        public override bool Equals(object obj)
        {
            return obj is ListenerSettings settings &&
                   ScreenOffInterval == settings.ScreenOffInterval &&
                   ScreenOnInterval == settings.ScreenOnInterval &&
                   OnErrorInterval == settings.OnErrorInterval &&
                   PowerSavingModeInterval == settings.PowerSavingModeInterval &&
                   UseCPUInBackground == settings.UseCPUInBackground;
        }

        public override int GetHashCode()
        {
            var hashCode = 738121570;
            hashCode = hashCode * -1521134295 + ScreenOffInterval.GetHashCode();
            hashCode = hashCode * -1521134295 + ScreenOnInterval.GetHashCode();
            hashCode = hashCode * -1521134295 + OnErrorInterval.GetHashCode();
            hashCode = hashCode * -1521134295 + PowerSavingModeInterval.GetHashCode();
            hashCode = hashCode * -1521134295 + UseCPUInBackground.GetHashCode();
            return hashCode;
        }
    }
}