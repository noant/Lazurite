using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public class Skin_GrayAndGreen : SkinBase
    {
        private static readonly Color SwitchForeColor = Color.FromHex("73757B");
        private static readonly Color SwitchForeColorReadonly = Color.FromHex("53555B");
        private static readonly Color SelectedSwitchForeColor = Color.FromHex("40BD5F");
        private static readonly Color SelectedSwitchForeColorReadonly = Color.FromHex("107D2F");
        private static readonly Color ForeColor = Color.FromHex("93959B");
        private static readonly Color Bg = Color.FromHex("252935");

        public override string SkinName { get; } = "Серый/зеленый";

        public override Color Background { get; } = Bg;
        public override Color BackgroundAlter { get; } = Bg;
        public override Color BackgroundSwitchesGrid { get; } = Bg;
        public override Color BottomPanelColor { get; } = Color.FromHex("42C662");
        public override Color BottomPanelIconColor { get; } = Bg;
        public override Color DialogViewBackground { get; } = Color.Black;
        public override Color DialogViewCloseIconColor { get; } = Color.Crimson;
        public override Color EntryBackground { get; } = Color.Transparent;
        public override Color Foreground { get; } = ForeColor;
        public override Color ItemBackground { get; } = Color.FromHex("353945");
        public override Color ItemBackgroundAlter { get; } = Color.FromHex("283238");
        public override Color ItemForeground { get; } = ForeColor;
        public override Color ItemIconColor { get; } = SelectedSwitchForeColor;
        public override Color ItemSelection { get; } = Color.FromHex("005599");
        public override Color MessageEditColor { get; } = Bg;
        public override Color ScaleBackColor { get; } = Color.Transparent;
        public override Color ScaleColor { get; } = SwitchForeColor;
        public override Color SelectedSwitchBackground { get; } = Bg;
        public override Color SelectedSwitchBackgroundReadonly { get; } = Bg;
        public override Color SelectedSwitchForeground { get; } = SelectedSwitchForeColor;
        public override Color SelectedSwitchForegroundReadonly { get; } = SelectedSwitchForeColorReadonly;
        public override Color SelectedSwitchIconColor { get; } = SelectedSwitchForeColor;
        public override Color SelectedSwitchIconColorReadonly { get; } = SelectedSwitchForeColorReadonly;
        public override Color SelectedSwitchValueForeground { get; } = SelectedSwitchForeColor;
        public override Color SelectedSwitchValueForegroundReadonly { get; } = SelectedSwitchForeColorReadonly;
        public override Color StandardIconColor { get; } = SwitchForeColorReadonly;
        public override Color SwitchBackground { get; } = Bg;
        public override Color SwitchBackgroundReadonly { get; } = Bg;
        public override Color SwitchBorder { get; } = Color.Transparent;
        public override Color SwitchForeground { get; } = SwitchForeColor;
        public override Color SwitchForegroundReadonly { get; } = SwitchForeColorReadonly;
        public override Color SwitchIconColor { get; } = SwitchForeColor;
        public override Color SwitchIconColorReadonly { get; } = SwitchForeColorReadonly;
        public override Color SwitchValueForeground { get; } = SwitchForeColor;
        public override Color SwitchValueForegroundReadonly { get; } = SwitchForeColorReadonly;
        public override Color ValueForeground { get; } = ForeColor;
        public override Color ValueForegroundAlter { get; } = ForeColor;
        public override double BottomPanelIconOpacity { get; } = 1;
        public override double BottomPanelOpacity { get; } = 1;
        public override double DialogViewBackgroundOpacity { get; } = 0.7;
        public override double SwitchBottomBorderHeight { get; } = 0;
        public override double SwitchGridElementMargin { get; } = 4;
        public override double SwitchIconOpacity { get; } = 1;
        public override int BigFontSize { get; } = 17;
        public override int FontSize { get; } = 12;
        public override string FontFamily { get; } = "sans-serif-light";

        public override int VisualOrder { get; } = 1;
    }
}