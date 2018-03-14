using Lazurite.Utils;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public class StringToSplittedString : IValueConverter
    {
        const int MaxLineWidth = 12;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
                return "[пусто]";
            return StringUtils.TruncateString(value.ToString(), MaxLineWidth);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
