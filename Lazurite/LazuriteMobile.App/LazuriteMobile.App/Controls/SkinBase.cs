using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public abstract class SkinBase
    {
        public abstract Color ScaleColor { get; }
        public abstract Color ScaleBackColor { get; }

        public abstract Color SwitchBorder { get; }
        public abstract Color MessageEditColor { get; }

        public abstract Color ItemBackground { get; }
        public abstract Color ItemIconColor { get; }
        public abstract Color ItemSelection { get; }

        public abstract Color BrightItemBackground { get; }
        public abstract Color Background { get; }
        public abstract Color BackgroundAlter { get; }
        public abstract Color BackgroundSwitchesGrid { get; }
        public abstract Color CaptionForeground { get; }
        public abstract Color CaptionForegroundAlter { get; }
        public abstract Color Foreground { get; }
        public abstract int FontSize { get; }
        public abstract int BigFontSize { get; }

        public abstract string FontFamily { get; }

        public abstract Color SwitchBackground { get; }
        public abstract Color SwitchBackgroundReadonly { get; }

        public abstract Color SwitchForeground { get; }
        public abstract Color SwitchCaptionForeground { get; }

        public abstract Color SelectedSwitchBackground { get; }
        public abstract Color SelectedSwitchBackgroundReadonly { get; }

        public abstract Color SwitchIconColor { get; }

        public abstract double SwitchGridElementMargin { get; }

        public abstract Color BottomPanelColor { get; }
        public abstract Color BottomPanelIconColor { get; }
        public abstract double BottomPanelIconOpacity { get; }
        public abstract double BottomPanelOpacity { get; }
    }
}
