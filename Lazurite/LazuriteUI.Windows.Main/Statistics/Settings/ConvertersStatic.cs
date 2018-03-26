namespace LazuriteUI.Windows.Main.Statistics.Settings
{
    public static class ConvertersStatic
    {
        public static readonly BoolToIcon_IsRegistered_Converter BoolToIcon_IsRegistered_Converter = new BoolToIcon_IsRegistered_Converter();
        public static readonly BoolToString_IsRegistered_Converter BoolToString_IsRegistered_Converter = new BoolToString_IsRegistered_Converter();
        public static readonly BoolToString_IsLocal_Converter BoolToString_IsLocal_Converter = new BoolToString_IsLocal_Converter();
        public static readonly StringToShortString_Converter StringToShortString_Converter = new StringToShortString_Converter();
    }
}