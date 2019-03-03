using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public class Skin_BlueGray : SkinBase
    {
        private static readonly Color SwitchForeColor = Color.FromHex("CFDAE7");
        private static readonly Color SwitchForeColorReadonly = Color.FromHex("7F8793");
        private static readonly Color SelectedSwitchForeColor = Color.FromHex("5294E2");
        private static readonly Color SelectedSwitchForeColorReadonly = Color.FromHex("406490");
        private static readonly Color ForeColor = Color.FromHex("CFDAE7");
        private static readonly Color Bg = Color.FromHex("2F343F");

        public override string SkinName { get; } = "Тускло-синий";

        public override Color Background { get; } = Bg;
        public override Color BackgroundAlter { get; } = Bg;
        public override Color BackgroundSwitchesGrid { get; } = Bg;
        public override Color BottomPanelColor { get; } = Color.FromHex("404552");
        public override Color BottomPanelIconColor { get; } = Color.FromHex("767B87");
        public override Color DialogViewBackground { get; } = Color.Black;
        public override Color DialogViewCloseIconColor { get; } = Color.Crimson;
        public override Color EntryBackground { get; } = Color.Transparent;
        public override Color Foreground { get; } = ForeColor;
        public override Color ItemBackground { get; } = Color.FromHex("404552");
        public override Color ItemBackgroundAlter { get; } = Color.FromHex("404552");
        public override Color ItemForeground { get; } = ForeColor;
        public override Color ItemIconColor { get; } = ForeColor;
        public override Color ItemSelection { get; } = Color.DarkSlateBlue;
        public override Color MessageEditColor { get; } = Bg;
        public override Color ScaleBackColor { get; } = Color.Transparent;
        public override Color ScaleColor { get; } = Color.FromHex("252A33");
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
        public override Color SwitchBorder { get; } = Color.FromHex("252A33");
        public override Color SwitchForeground { get; } = SwitchForeColor;
        public override Color SwitchForegroundReadonly { get; } = SwitchForeColorReadonly;
        public override Color SwitchIconColor { get; } = Color.FromHex("757B87");
        public override Color SwitchIconColorReadonly { get; } = Color.FromHex("525763");
        public override Color SwitchValueForeground { get; } = Color.FromHex("757B87");
        public override Color SwitchValueForegroundReadonly { get; } = Color.FromHex("525763");
        public override Color ValueForeground { get; } = ForeColor;
        public override Color ValueForegroundAlter { get; } = ForeColor;
        public override double BottomPanelIconOpacity { get; } = 1;
        public override double BottomPanelOpacity { get; } = 1;
        public override double DialogViewBackgroundOpacity { get; } = 0.7;
        public override double SwitchBottomBorderHeight { get; } = 1;
        public override double SwitchGridElementMargin { get; } = 2;
        public override double SwitchIconOpacity { get; } = 1;
        public override int BigFontSize { get; } = 17;
        public override int FontSize { get; } = 12;
        public override string FontFamily { get; } = "sans-serif-light";

        public override int VisualOrder { get; } = 4;
    }
}