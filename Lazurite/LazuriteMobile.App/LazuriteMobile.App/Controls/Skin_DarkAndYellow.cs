using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public class Skin_DarkAndYellow : SkinBase
    {
        private static readonly Color ForeColor = Color.DarkGray;
        private static readonly Color SwitchForeColor = Color.FromHex("808080");
        private static readonly Color Bg = Color.FromHex("000505");
        private static readonly Color BgAlter = Color.FromHex("151515");
        private static readonly Color SelectedSwitchForeColor = Bg;
        private static readonly Color Panel = Color.FromHex("A46D0E");
        private static readonly Color Panel2 = Color.FromHex("843D0E");

        public override string SkinName { get; } = "Черный/желтый";

        public override Color Background { get; } = Bg;
        public override Color BackgroundAlter { get; } = Bg;
        public override Color BackgroundSwitchesGrid { get; } = Bg;
        public override Color BottomPanelColor { get; } = Panel;
        public override Color BottomPanelIconColor { get; } = SelectedSwitchForeColor;
        public override Color DialogViewBackground { get; } = Color.Black;
        public override Color DialogViewCloseIconColor { get; } = Color.Crimson;
        public override Color EntryBackground { get; } = Color.Transparent;
        public override Color Foreground { get; } = ForeColor;
        public override Color ItemBackground { get; } = BgAlter;
        public override Color ItemBackgroundAlter { get; } = BgAlter;
        public override Color ItemForeground { get; } = ForeColor;
        public override Color ItemIconColor { get; } = ForeColor;
        public override Color ItemSelection { get; } = Panel2;
        public override Color MessageEditColor { get; } = Bg;
        public override Color ScaleBackColor { get; } = Color.Transparent;
        public override Color ScaleColor { get; } = Panel;
        public override Color SelectedSwitchBackground { get; } = Panel;
        public override Color SelectedSwitchBackgroundReadonly { get; } = Panel2;
        public override Color SelectedSwitchForeground { get; } = Bg;
        public override Color SelectedSwitchForegroundReadonly { get; } = BgAlter;
        public override Color SelectedSwitchIconColor { get; } = Bg;
        public override Color SelectedSwitchIconColorReadonly { get; } = BgAlter;
        public override Color SelectedSwitchValueForeground { get; } = Bg;
        public override Color SelectedSwitchValueForegroundReadonly { get; } = BgAlter;
        public override Color StandardIconColor { get; } = ForeColor;
        public override Color SwitchBackground { get; } = Bg;
        public override Color SwitchBackgroundReadonly { get; } = Bg;
        public override Color SwitchBorder { get; } = Color.Transparent;
        public override Color SwitchForeground { get; } = SwitchForeColor;
        public override Color SwitchForegroundReadonly { get; } = Color.DarkSlateGray;
        public override Color SwitchIconColor { get; } = SwitchForeColor;
        public override Color SwitchIconColorReadonly { get; } = Color.DarkSlateGray;
        public override Color SwitchValueForeground { get; } = SwitchForeColor;
        public override Color SwitchValueForegroundReadonly { get; } = Color.DarkSlateGray;
        public override Color ValueForeground { get; } = ForeColor;
        public override Color ValueForegroundAlter { get; } = ForeColor;
        public override double BottomPanelIconOpacity { get; } = 1;
        public override double BottomPanelOpacity { get; } = 1;
        public override double DialogViewBackgroundOpacity { get; } = 0.85;
        public override double SwitchBottomBorderHeight { get; } = 0;
        public override double SwitchGridElementMargin { get; } = 2;
        public override double SwitchIconOpacity { get; } = 1;
        public override int BigFontSize { get; } = 17;
        public override int FontSize { get; } = 12;
        public override string FontFamily { get; } = "sans-serif-light";

        public override int VisualOrder { get; } = 5;
    }
}