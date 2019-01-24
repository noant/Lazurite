using System;
using System.Globalization;
using System.Windows.Data;

namespace LazuriteUI.Windows.Main.Statistics.Settings
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class BoolToString_IsLocal_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Локальный сценарий" : "Удаленный сценарий";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
