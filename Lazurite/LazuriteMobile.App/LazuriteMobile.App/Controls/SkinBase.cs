using Xamarin.Forms;

namespace LazuriteMobile.App.Controls
{
    public abstract class SkinBase
    {
        public abstract Color ScaleColor { get; }
        public abstract Color SwitchBorder { get; }
        public abstract Color MessageEditColor { get; }

        public abstract Color ItemBackground { get; }
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

        public abstract Color SelectedSwitchBackground { get; }
        public abstract Color SelectedSwitchBackgroundReadonly { get; }
    }
}
