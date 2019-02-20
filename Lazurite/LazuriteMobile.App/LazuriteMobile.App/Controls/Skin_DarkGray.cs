using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public class Skin_DarkGray : SkinBase
    {
        private static readonly Color SwitchForeColor = Color.FromHex("B8B8B8");
        private static readonly Color ForeColor = SwitchForeColor;
        private static readonly Color Bg = Color.FromHex("505050");

        public override string SkinName { get; } = "Темно-серый";

        public override Color Background { get; } = Bg;
        public override Color BackgroundAlter { get; } = Bg;
        public override Color BackgroundSwitchesGrid { get; } = Bg;
        public override Color BottomPanelColor { get; } = Bg;
        public override Color BottomPanelIconColor { get; } = SwitchForeColor;
        public override Color DialogViewBackground { get; } = Color.DimGray;
        public override Color DialogViewCloseIconColor { get; } = Color.Crimson;
        public override Color EntryBackground { get; } = Color.Transparent;
        public override Color Foreground { get; } = ForeColor;
        public override Color ItemBackground { get; } = Bg;
        public override Color ItemBackgroundAlter { get; } = Bg;
        public override Color ItemIconColor { get; } = SwitchForeColor;
        public override Color ItemSelection { get; } = Color.FromHex("AB1874CD");
        public override Color MessageEditColor { get; } = Bg;
        public override Color ScaleBackColor { get; } = Color.Transparent;
        public override Color ScaleColor { get; } = SwitchForeColor;
        public override Color SelectedSwitchBackground { get; } = Bg;
        public override Color SelectedSwitchBackgroundReadonly { get; } = Bg;
        public override Color SelectedSwitchForeground { get; } = Color.SkyBlue;
        public override Color SelectedSwitchForegroundReadonly { get; } = Color.DeepSkyBlue;
        public override Color SelectedSwitchIconColor { get; } = Color.SkyBlue;
        public override Color SelectedSwitchIconColorReadonly { get; } = Color.DeepSkyBlue;
        public override Color SelectedSwitchValueForeground { get; } = Color.SkyBlue;
        public override Color SelectedSwitchValueForegroundReadonly { get; } = Color.DeepSkyBlue;
        public override Color StandardIconColor { get; } = ForeColor;
        public override Color SwitchBackground { get; } = Bg;
        public override Color SwitchBackgroundReadonly { get; } = Bg;
        public override Color SwitchBorder { get; } = Color.Transparent;
        public override Color SwitchForeground { get; } = SwitchForeColor;
        public override Color SwitchForegroundReadonly { get; } = Color.SlateGray;
        public override Color SwitchIconColor { get; } = SwitchForeColor;
        public override Color SwitchIconColorReadonly { get; } = Color.SlateGray;
        public override Color SwitchValueForeground { get; } = SwitchForeColor;
        public override Color SwitchValueForegroundReadonly { get; } = Color.SlateGray;
        public override Color ValueForeground { get; } = ForeColor;
        public override Color ValueForegroundAlter { get; } = ForeColor;
        public override double BottomPanelIconOpacity { get; } = 1;
        public override double BottomPanelOpacity { get; } = 1;
        public override double DialogViewBackgroundOpacity { get; } = 0.85;
        public override double SwitchBottomBorderHeight { get; } = 0;
        public override double SwitchGridElementMargin { get; } = 4;
        public override double SwitchIconOpacity { get; } = 1;
        public override int BigFontSize { get; } = 17;
        public override int FontSize { get; } = 12;
        public override string FontFamily { get; } = "sans-serif-light";

        public override int VisualOrder { get; } = 3;
    }
}