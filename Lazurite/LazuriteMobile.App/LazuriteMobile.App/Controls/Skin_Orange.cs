using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public class Skin_Orange : SkinBase
    {
        private static readonly Color ForeColor = Color.White;
        private static readonly Color ForeColorAlter = Color.White;
        private static readonly Color Backround = Color.DarkOrange;
        private static readonly Color PanelColor = Color.DarkOrange;

        public override string SkinName { get; } = "Апельсин";

        public override Color Background { get; } = Backround;
        public override Color BackgroundAlter { get; } = Backround;
        public override Color BackgroundSwitchesGrid { get; } = Backround;
        public override Color BottomPanelColor { get; } = Color.FromHex("DDFF8C00");
        public override Color BottomPanelIconColor { get; } = ForeColor;
        public override Color DialogViewBackground { get; } = PanelColor;
        public override Color DialogViewCloseIconColor { get; } = Color.Crimson;
        public override Color EntryBackground { get; } = PanelColor;
        public override Color Foreground { get; } = ForeColor;
        public override Color ItemBackground { get; } = Color.Orange;
        public override Color ItemBackgroundAlter { get; } = Backround;
        public override Color ItemIconColor { get; } = ForeColor;
        public override Color ItemSelection { get; } = Color.LightSkyBlue;
        public override Color MessageEditColor { get; } = PanelColor;
        public override Color ScaleBackColor { get; } = Color.Transparent;
        public override Color ScaleColor { get; } = Color.Orange;
        public override Color SelectedSwitchBackground { get; } = Color.Peru;
        public override Color SelectedSwitchBackgroundReadonly { get; } = Color.OrangeRed;
        public override Color StandardIconColor { get; } = ForeColor;
        public override Color SwitchBackground { get; } = PanelColor;
        public override Color SwitchBackgroundReadonly { get; } = Color.Orange;
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
        public override double SwitchGridElementMargin { get; } = 0;
        public override double SwitchIconOpacity { get; } = 1;
        public override int BigFontSize { get; } = 17;
        public override int FontSize { get; } = 12;
        public override string FontFamily { get; } = "sans-serif-light";

        public override Color SelectedSwitchIconColor => throw new System.NotImplementedException();

        public override Color SelectedSwitchForeground => throw new System.NotImplementedException();
    }
}
