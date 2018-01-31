using Lazurite.Utils;
using System;
using System.Globalization;
using System.Windows.Data;

namespace LazuriteUI.Windows.Main.Switches
{
    [ValueConversion(typeof(string), typeof(string))]
    public class StringToSplittedString : IValueConverter
    {
        const int MaxLineWidth = 14;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = (value ?? string.Empty).ToString();
            return StringUtils.TruncateString(str, MaxLineWidth);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
