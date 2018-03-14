using System;
using System.Globalization;
using System.Windows.Data;

namespace LazuriteUI.Windows.Main.Switches
{
    [ValueConversion(typeof(string), typeof(string))]
    public class StringToShortString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
                return "[пусто]";
            var val = value.ToString();
            if (val.Length <= 14)
                return val;
            else return val.Substring(0, 12)+"...";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
