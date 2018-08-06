using System;
using System.Globalization;
using System.Windows.Data;

namespace LazuriteUI.Windows.Main.Switches
{
    [ValueConversion(typeof(string), typeof(string))]
    public class StringToShortString : IValueConverter
    {
        private short _maxLen = 14;

        public StringToShortString(short maxLen) =>
            _maxLen = maxLen;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
                return "[пусто]";
            var val = value.ToString();
            if (val.Length <= _maxLen)
                return val;
            else return val.Substring(0, _maxLen - 2) + "...";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
