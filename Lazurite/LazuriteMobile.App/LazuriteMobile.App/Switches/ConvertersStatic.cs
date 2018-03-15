namespace LazuriteMobile.App.Switches
{
    public static class ConvertersStatic
    {
        public static readonly BoolToDouble BoolToDouble = new BoolToDouble();
        public static readonly BoolInvert BoolInvert = new BoolInvert();
        public static readonly DateTimeValueTypeToSplittedString DateTimeValueTypeToSplittedString = new DateTimeValueTypeToSplittedString();
        public static readonly GeolocationDateTimeValueTypeToSplittedString GeolocationDateTimeValueTypeToSplittedString = new GeolocationDateTimeValueTypeToSplittedString();
        public static readonly StringToIcon StringToIcon = new StringToIcon();
        public static readonly StringToSplittedString StringToSplittedString = new StringToSplittedString();
        public static readonly StringToSplittedStringLong StringToSplittedStringLong = new StringToSplittedStringLong();
        public static readonly StringToSplittedStringShort StringToSplittedStringShort = new StringToSplittedStringShort();
        public static readonly ValueTypeStringToBool ValueTypeStringToBool = new ValueTypeStringToBool();
        public static readonly ValueTypeStringToBoolInvert ValueTypeStringToBoolInvert = new ValueTypeStringToBoolInvert();
        public static readonly ValueTypeStringToDouble ValueTypeStringToDouble = new ValueTypeStringToDouble();
        public static readonly ValueTypeStringToDoubleRoundStr ValueTypeStringToDoubleRoundStr = new ValueTypeStringToDoubleRoundStr();
        public static readonly Selection_BoolToColor Selection_BoolToColor = new Selection_BoolToColor();
        public static readonly Background_BoolToColor Background_BoolToColor = new Background_BoolToColor();
    }
}
