using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public class Skin_Lazurite: SkinBase
    {
        public override Color SwitchBorder { get; } = Color.FromHex("7d28a6");
        public override Color ScaleColor { get; } = Color.MediumOrchid;

        public override Color ItemBackground { get; } = Color.FromRgb(37, 37, 45);
        public override Color ItemSelection { get; } = Color.SlateBlue;
        public override Color BrightItemBackground { get; } = Color.FromRgb(60, 60, 68);
        public override Color Background { get; } = Color.FromRgb(26, 26, 34);
        public override Color BackgroundAlter { get; } = Color.FromRgb(29, 25, 39);
        public override Color CaptionForeground { get; } = Color.SteelBlue;
        public override Color CaptionForegroundAlter { get; } = Color.Orchid;
        public override Color Foreground { get; } = Color.LightGray;
        public override int FontSize { get; } = 14;
        public override int BigFontSize { get; } = 17;

        public override string FontFamily { get; } = "sans-serif-light";

        public override Color SwitchBackground { get; } = Color.FromRgb(37, 37, 45);
        public override Color SwitchBackgroundReadonly { get; } = Color.FromRgb(0, 49, 83);

        public override Color SelectedSwitchBackground { get; } = Color.SlateBlue;
        public override Color SelectedSwitchBackgroundReadonly { get; } = Color.FromRgb(0, 80, 130);

        public override Color MessageEditColor { get; } = Color.FromHex("302530");

        public override Color BackgroundSwitchesGrid { get; } = Color.FromRgb(26, 26, 34);
    }
}
