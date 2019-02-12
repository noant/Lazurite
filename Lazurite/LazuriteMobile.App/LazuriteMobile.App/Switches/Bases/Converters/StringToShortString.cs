using Lazurite.Utils;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches.Bases.Converters
{
    public class StringToShortString : IValueConverter
    {
        public StringToShortString(int maxLen)
        {
            _maxLen = maxLen;
        }

        private int _maxLen = 12;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
                return "[пусто]";
            return StringUtils.TruncateStringNoWrap(value.ToString(), _maxLen);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
