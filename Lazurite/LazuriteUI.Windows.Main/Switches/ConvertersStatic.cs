namespace LazuriteUI.Windows.Main.Switches
{
    public static class ConvertersStatic
    {
        public static readonly BoolInvert BoolInvert = new BoolInvert();
        public static readonly BoolToDouble BoolToDouble = new BoolToDouble();
        public static readonly BoolToVisibility BoolToVisibility = new BoolToVisibility();
        public static readonly BoolToVisibilityInvert BoolToVisibilityInvert = new BoolToVisibilityInvert();
        public static readonly GeolocationDateTimeValueTypeToSplittedString GeolocationDateTimeValueTypeToSplittedString = new GeolocationDateTimeValueTypeToSplittedString();
        public static readonly StringToIcon StringToIcon = new StringToIcon();
        public static readonly StringToShortString StringToShortString = new StringToShortString(16);
        public static readonly StringToShortString StringToShortStringSmall = new StringToShortString(7);
        public static readonly StringToShortString StringToShortStringBig = new StringToShortString(40);
        public static readonly ValueTypeStringToBool ValueTypeStringToBool = new ValueTypeStringToBool();
        public static readonly ValueTypeStringToDouble ValueTypeStringToDouble = new ValueTypeStringToDouble();
        public static readonly ValueTypeStringToDoubleRound ValueTypeStringToDoubleRound = new ValueTypeStringToDoubleRound();
        public static readonly Selection_BoolToBrush Selection_BoolToBrush = new Selection_BoolToBrush();
        public static readonly Background_BoolToBrush Background_BoolToBrush = new Background_BoolToBrush();
    }
}
