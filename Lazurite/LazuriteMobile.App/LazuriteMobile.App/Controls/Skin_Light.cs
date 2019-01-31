using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public class Skin_Light : SkinBase
    {
        public override Color SwitchBorder { get; } = Color.FromHex("7d28a6");
        public override Color ScaleColor { get; } = Color.LightBlue;

        public override Color ItemBackground { get; } = Color.LightGray;
        public override Color ItemSelection { get; } = Color.LightSkyBlue;
        public override Color BrightItemBackground { get; } = Color.AntiqueWhite;
        public override Color Background { get; } = Color.LightGray;
        public override Color BackgroundAlter { get; } = Color.DimGray;
        public override Color CaptionForeground { get; } = Color.SteelBlue;
        public override Color CaptionForegroundAlter { get; } = Color.Orchid;
        public override Color Foreground { get; } = Color.SlateGray;
        public override int FontSize { get; } = 14;
        public override int BigFontSize { get; } = 17;

        public override string FontFamily { get; } = "sans-serif-light";

        public override Color SwitchBackground { get; } = Color.LightGray;
        public override Color SwitchBackgroundReadonly { get; } = Color.FloralWhite;

        public override Color SelectedSwitchBackground { get; } = Color.LightSkyBlue;
        public override Color SelectedSwitchBackgroundReadonly { get; } = Color.LightSteelBlue;

        public override Color MessageEditColor { get; } = Color.NavajoWhite;
        public override Color BackgroundSwitchesGrid { get; } = Color.White;
    }
}
