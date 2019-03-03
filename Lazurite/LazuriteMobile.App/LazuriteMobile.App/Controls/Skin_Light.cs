using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public class Skin_Light : SkinBase
    {
        private static readonly Color ForeColor = Color.DimGray;
        private static readonly Color ForeColorAlter = Color.FromHex("8A263E");
        private static readonly Color Bg = Color.FromHex("EFEFEF");
        private static readonly Color PanelColor = Color.FromHex("E1E1E1");

        public override string SkinName { get; } = "Светлый";

        public override Color Background { get; } = Bg;
        public override Color BackgroundAlter { get; } = Bg;
        public override Color BackgroundSwitchesGrid { get; } = Bg;
        public override Color BottomPanelColor { get; } = Color.FromHex("DDEFEFEF");
        public override Color BottomPanelIconColor { get; } = ForeColor;
        public override Color DialogViewBackground { get; } = PanelColor;
        public override Color DialogViewCloseIconColor { get; } = Color.Crimson;
        public override Color EntryBackground { get; } = PanelColor;
        public override Color Foreground { get; } = ForeColor;
        public override Color ItemBackground { get; } = PanelColor;
        public override Color ItemBackgroundAlter { get; } = Bg;
        public override Color ItemForeground { get; } = ForeColor;
        public override Color ItemIconColor { get; } = ForeColor;
        public override Color ItemSelection { get; } = Color.SkyBlue;
        public override Color MessageEditColor { get; } = PanelColor;
        public override Color ScaleBackColor { get; } = Color.Transparent;
        public override Color ScaleColor { get; } = ForeColor;
        public override Color SelectedSwitchBackground { get; } = Color.DimGray;
        public override Color SelectedSwitchBackgroundReadonly { get; } = Color.FromHex("888888");
        public override Color SelectedSwitchForeground { get; } = Bg;
        public override Color SelectedSwitchForegroundReadonly { get; } = Bg;
        public override Color SelectedSwitchIconColor { get; } = Bg;
        public override Color SelectedSwitchIconColorReadonly { get; } = Bg;
        public override Color SelectedSwitchValueForeground { get; } = Bg;
        public override Color SelectedSwitchValueForegroundReadonly { get; } = Bg;
        public override Color StandardIconColor { get; } = ForeColor;
        public override Color SwitchBackground { get; } = Color.FromHex("D5D5D5");
        public override Color SwitchBackgroundReadonly { get; } = PanelColor;
        public override Color SwitchBorder { get; } = ForeColor;
        public override Color SwitchForeground { get; } = ForeColor;
        public override Color SwitchForegroundReadonly { get; } = ForeColor;
        public override Color SwitchIconColor { get; } = ForeColor;
        public override Color SwitchIconColorReadonly { get; } = ForeColor;
        public override Color SwitchValueForeground { get; } = ForeColor;
        public override Color SwitchValueForegroundReadonly { get; } = ForeColor;
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

        public override int VisualOrder { get; } = 2;
    }
}