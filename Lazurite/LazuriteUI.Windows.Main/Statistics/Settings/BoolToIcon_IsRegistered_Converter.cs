using LazuriteUI.Icons;
using System;
using System.Globalization;
using System.Windows.Data;

namespace LazuriteUI.Windows.Main.Statistics.Settings
{
    [ValueConversion(typeof(bool), typeof(Icon))]
    public class BoolToIcon_IsRegistered_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Icon.Check : Icon.Cancel;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
