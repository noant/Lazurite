using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public class Skin_Light : SkinBase
    {
        public override Color SwitchBorder { get; } = Color.FloralWhite;
        public override Color ScaleColor { get; } = Color.LightBlue;

        public override Color ItemBackground { get; } = Color.LightGray;
        public override Color ItemSelection { get; } = Color.LightSkyBlue;
        public override Color BrightItemBackground { get; } = Color.AntiqueWhite;
        public override Color Background { get; } = Color.FromHex("e8faff");
        public override Color BackgroundAlter { get; } = Color.FromHex("ffe8fd");
        public override Color CaptionForeground { get; } = Color.SteelBlue;
        public override Color CaptionForegroundAlter { get; } = Color.Orchid;
        public override Color Foreground { get; } = Color.Gray;
        public override int FontSize { get; } = 14;
        public override int BigFontSize { get; } = 17;

        public override string FontFamily { get; } = "sans-serif-light";

        public override Color SwitchBackground { get; } = Color.FloralWhite;
        public override Color SwitchBackgroundReadonly { get; } = Color.FloralWhite;

        public override Color SelectedSwitchBackground { get; } = Color.LightSkyBlue;
        public override Color SelectedSwitchBackgroundReadonly { get; } = Color.LightSteelBlue;
        
        public override Color MessageEditColor { get; } = Color.FloralWhite;
        public override Color BackgroundSwitchesGrid { get; } = Color.FloralWhite;

        public override Color ItemIconColor { get; } = Color.White;
        public override Color SwitchIconColor { get; } = Color.Black;
        public override Color BottomPanelColor { get; } = Color.FloralWhite;
        public override Color BottomPanelIconColor { get; } = Color.Black;

        public override Color SwitchForeground { get; } = Color.SteelBlue;
        public override Color SwitchCaptionForeground { get; } = Color.SlateGray;

        public override double SwitchGridElementMargin { get; } = 0;

        public override Color ScaleBackColor { get; } = Color.Transparent;

        public override double BottomPanelIconOpacity { get; } = 1;

        public override double BottomPanelOpacity { get; } = 1;
    }
}
