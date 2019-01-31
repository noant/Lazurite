using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public static class Visual
    {
        public static SkinBase Current { get; set; } = new Skin_Lazurite();

        public static Color ScaleColor => Current.ScaleColor;
        public static Color SwitchBorder => Current.SwitchBorder;
        public static Color MessageEditColor => Current.MessageEditColor;

        public static Color ItemBackground => Current.ItemBackground;
        public static Color ItemSelection => Current.ItemSelection;
        public static Color BrightItemBackground => Current.BrightItemBackground;
        public static Color Background => Current.Background;
        public static Color BackgroundAlter => Current.BackgroundAlter;
        public static Color BackgroundSwitchesGrid => Current.BackgroundSwitchesGrid;
        public static Color CaptionForeground => Current.CaptionForeground;
        public static Color CaptionForegroundAlter => Current.CaptionForegroundAlter;
        public static Color Foreground => Current.Foreground;
        public static int FontSize => Current.FontSize;
        public static int BigFontSize => Current.BigFontSize;

        public static string FontFamily => Current.FontFamily;

        public static Color SwitchBackground => Current.SwitchBackground;
        public static Color SwitchBackgroundReadonly => Current.SwitchBackgroundReadonly;

        public static Color SelectedSwitchBackground => Current.SelectedSwitchBackground;
        public static Color SelectedSwitchBackgroundReadonly => Current.SelectedSwitchBackgroundReadonly;
    }
}