using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LazuriteUI.Windows.Main.Constructors.Decomposition
{
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class StringToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
                return Visibility.Visible;
            else return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
