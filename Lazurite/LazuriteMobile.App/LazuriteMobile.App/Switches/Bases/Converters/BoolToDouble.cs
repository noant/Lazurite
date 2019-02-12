using System;
using System.Globalization;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches.Bases.Converters
{
    public class BoolToDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return 1.0d;
            else
                return 0.35d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
