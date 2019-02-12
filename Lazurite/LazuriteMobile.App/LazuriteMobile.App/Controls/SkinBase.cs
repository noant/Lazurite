using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public abstract class SkinBase
    {
        public abstract string SkinName { get; }

        public abstract int VisualOrder { get; }

        public abstract Color Background { get; }
        public abstract Color BackgroundAlter { get; }
        public abstract Color BackgroundSwitchesGrid { get; }
        public abstract Color BottomPanelColor { get; }
        public abstract Color BottomPanelIconColor { get; }
        public abstract Color DialogViewBackground { get; }
        public abstract Color DialogViewCloseIconColor { get; }
        public abstract Color EntryBackground { get; }
        public abstract Color Foreground { get; }
        public abstract Color ItemBackground { get; }
        public abstract Color ItemBackgroundAlter { get; }
        public abstract Color ItemIconColor { get; }
        public abstract Color ItemSelection { get; }
        public abstract Color MessageEditColor { get; }
        public abstract Color ScaleBackColor { get; }
        public abstract Color ScaleColor { get; }
        public abstract Color SelectedSwitchBackground { get; }
        public abstract Color SelectedSwitchBackgroundReadonly { get; }
        public abstract Color SelectedSwitchForeground { get; }
        public abstract Color SelectedSwitchForegroundReadonly { get; }
        public abstract Color SelectedSwitchIconColor { get; }
        public abstract Color SelectedSwitchIconColorReadonly { get; }
        public abstract Color SelectedSwitchValueForeground { get; }
        public abstract Color SelectedSwitchValueForegroundReadonly { get; }
        public abstract Color StandardIconColor { get; }
        public abstract Color SwitchBackground { get; }
        public abstract Color SwitchBackgroundReadonly { get; }
        public abstract Color SwitchBorder { get; }
        public abstract Color SwitchForeground { get; }
        public abstract Color SwitchForegroundReadonly { get; }
        public abstract Color SwitchIconColor { get; }
        public abstract Color SwitchIconColorReadonly { get; }
        public abstract Color SwitchValueForeground { get; }
        public abstract Color SwitchValueForegroundReadonly { get; }
        public abstract Color ValueForeground { get; }
        public abstract Color ValueForegroundAlter { get; }
        public abstract double BottomPanelIconOpacity { get; }
        public abstract double BottomPanelOpacity { get; }
        public abstract double DialogViewBackgroundOpacity { get; }
        public abstract double SwitchBottomBorderHeight { get; }
        public abstract double SwitchGridElementMargin { get; }
        public abstract double SwitchIconOpacity { get; }
        public abstract int BigFontSize { get; }
        public abstract int FontSize { get; }
        public abstract string FontFamily { get; }
    }
}
