using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public class Skin_Light : SkinBase
    {
        private static readonly Color ForeColor = Color.FromHex("456b69");
        private static readonly Color ForeColorAlter = Color.FromHex("8A263E");
        private static readonly Color Backround = Color.FromHex("EFEFEF");
        private static readonly Color PanelColor = Color.FromHex("EAEAEA");

        public override Color Background { get; } = Backround;
        public override Color BackgroundAlter { get; } = Backround;
        public override Color BackgroundSwitchesGrid { get; } = Backround;
        public override Color BottomPanelColor { get; } = Color.FromHex("DDEFEFEF");
        public override Color BottomPanelIconColor { get; } = ForeColor;
        public override Color DialogViewBackground { get; } = PanelColor;
        public override Color DialogViewCloseIconColor { get; } = Color.Crimson;
        public override Color EntryBackground { get; } = PanelColor;
        public override Color Foreground { get; } = ForeColor;
        public override Color ItemBackground { get; } = PanelColor;
        public override Color ItemBackgroundAlter { get; } = Backround;
        public override Color ItemIconColor { get; } = ForeColor;
        public override Color ItemSelection { get; } = Color.LightSkyBlue;
        public override Color MessageEditColor { get; } = PanelColor;
        public override Color ScaleBackColor { get; } = Color.Transparent;
        public override Color ScaleColor { get; } = ForeColor;
        public override Color SelectedSwitchBackground { get; } = Color.FromHex("b9d8e5");
        public override Color SelectedSwitchBackgroundReadonly { get; } = Color.FromHex("a3c3d1");
        public override Color StandardIconColor { get; } = ForeColor;
        public override Color SwitchBackground { get; } = PanelColor;
        public override Color SwitchBackgroundReadonly { get; } = Color.FromHex("E5E5E5");
        public override Color SwitchBorder { get; } = ForeColor;
        public override Color SwitchForeground { get; } = ForeColor;
        public override Color SwitchIconColor { get; } = ForeColor;
        public override Color SwitchValueForeground { get; } = ForeColor;
        public override Color ValueForeground { get; } = ForeColor;
        public override Color ValueForegroundAlter { get; } = ForeColorAlter;
        public override double BottomPanelIconOpacity { get; } = 1;
        public override double BottomPanelOpacity { get; } = 1;
        public override double DialogViewBackgroundOpacity { get; } = 0.85;
        public override double SwitchBottomBorderHeight { get; } = 0;
        public override double SwitchGridElementMargin { get; } = 2;
        public override double SwitchIconOpacity { get; } = 1;
        public override int BigFontSize { get; } = 17;
        public override int FontSize { get; } = 12;
        public override string FontFamily { get; } = "sans-serif-light";
    }
}
