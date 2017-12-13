namespace LazuriteUI.Windows.Main.Switches
{
    public static class ConvertersStatic
    {
        public static readonly BoolInvert BoolInvert = new BoolInvert();
        public static readonly BoolToDouble BoolToDouble = new BoolToDouble();
        public static readonly BoolToVisibility BoolToVisibility = new BoolToVisibility();
        public static readonly BoolToVisibilityInvert BoolToVisibilityInvert = new BoolToVisibilityInvert();
        public static readonly DateTimeValueTypeToSplittedString DateTimeValueTypeToSplittedString = new DateTimeValueTypeToSplittedString();
        public static readonly StringToIcon StringToIcon = new StringToIcon();
        public static readonly StringToShortString StringToShortString = new StringToShortString();
        public static readonly StringToSplittedString StringToSplittedString = new StringToSplittedString();
        public static readonly StringToSplittedStringLong StringToSplittedStringLong = new StringToSplittedStringLong();
        public static readonly ValueTypeStringToBool ValueTypeStringToBool = new ValueTypeStringToBool();
        public static readonly ValueTypeStringToDouble ValueTypeStringToDouble = new ValueTypeStringToDouble();
        public static readonly ValueTypeStringToDoubleRound ValueTypeStringToDoubleRound = new ValueTypeStringToDoubleRound();
    }
}
