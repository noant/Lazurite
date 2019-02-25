using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public class Skin_Lazurite : SkinBase
    {
        public override string SkinName { get; } = "Классика";

        public override Color Background { get; } = Color.FromRgb(26, 26, 34);
        public override Color BackgroundAlter { get; } = Color.FromRgb(29, 25, 39);
        public override Color BackgroundSwitchesGrid { get; } = Color.FromRgb(26, 26, 34);
        public override Color BottomPanelColor { get; } = Color.Black;
        public override Color BottomPanelIconColor { get; } = Color.Transparent;
        public override Color DialogViewBackground { get; } = Color.Black;
        public override Color DialogViewCloseIconColor { get; } = Color.Red;
        public override Color EntryBackground { get; } = Color.FromRgb(26, 26, 34);
        public override Color Foreground { get; } = Color.LightGray;
        public override Color ItemBackground { get; } = Color.FromRgb(37, 37, 45);
        public override Color ItemBackgroundAlter { get; } = Color.FromRgb(60, 60, 68);
        public override Color ItemForeground { get; } = Color.LightGray;
        public override Color ItemIconColor { get; } = Color.LightGray;
        public override Color ItemSelection { get; } = Color.SlateBlue;
        public override Color MessageEditColor { get; } = Color.FromHex("302530");
        public override Color ScaleBackColor { get; } = Color.Gray;
        public override Color ScaleColor { get; } = Color.MediumOrchid;
        public override Color SelectedSwitchBackground { get; } = Color.SlateBlue;
        public override Color SelectedSwitchBackgroundReadonly { get; } = Color.FromRgb(0, 80, 130);
        public override Color SelectedSwitchForeground { get; } = Color.LightGray;
        public override Color SelectedSwitchForegroundReadonly { get; } = Color.LightGray;
        public override Color SelectedSwitchIconColor { get; } = Color.FromHex("dedede");
        public override Color SelectedSwitchIconColorReadonly { get; } = Color.FromHex("dedede");
        public override Color SelectedSwitchValueForeground { get; } = Color.SteelBlue;
        public override Color SelectedSwitchValueForegroundReadonly { get; } = Color.SteelBlue;
        public override Color StandardIconColor { get; } = Color.White;
        public override Color SwitchBackground { get; } = Color.FromRgb(37, 37, 45);
        public override Color SwitchBackgroundReadonly { get; } = Color.FromRgb(0, 49, 83);
        public override Color SwitchBorder { get; } = Color.FromHex("7d28a6");
        public override Color SwitchForeground { get; } = Color.LightGray;
        public override Color SwitchForegroundReadonly { get; } = Color.LightGray;
        public override Color SwitchIconColor { get; } = Color.FromHex("dedede");
        public override Color SwitchIconColorReadonly { get; } = Color.FromHex("dedede");
        public override Color SwitchValueForeground { get; } = Color.SteelBlue;
        public override Color SwitchValueForegroundReadonly { get; } = Color.SteelBlue;
        public override Color ValueForeground { get; } = Color.SteelBlue;
        public override Color ValueForegroundAlter { get; } = Color.Orchid;
        public override double BottomPanelIconOpacity { get; } = 0.9;
        public override double BottomPanelOpacity { get; } = 0.7;
        public override double DialogViewBackgroundOpacity { get; } = 0.85;
        public override double SwitchBottomBorderHeight { get; } = 1;
        public override double SwitchGridElementMargin { get; } = 2;
        public override double SwitchIconOpacity { get; } = 0.8;
        public override int BigFontSize { get; } = 17;
        public override int FontSize { get; } = 12;
        public override string FontFamily { get; } = "sans-serif-light";

        public override int VisualOrder { get; } = 0;
    }
}