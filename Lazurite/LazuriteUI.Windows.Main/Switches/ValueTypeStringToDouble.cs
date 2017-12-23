using System;
using System.Globalization;
using System.Windows.Data;

namespace LazuriteUI.Windows.Main.Switches
{
    [ValueConversion(typeof(string), typeof(double))]
    public class ValueTypeStringToDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return double.Parse((!string.IsNullOrEmpty(value.ToString()) ? value.ToString() : "0"));
            }
            catch
            {
                return -1.0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }
}
