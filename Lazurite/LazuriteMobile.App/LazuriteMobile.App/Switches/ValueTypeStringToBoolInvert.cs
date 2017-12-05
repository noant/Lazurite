using Lazurite.ActionsDomain.ValueTypes;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public class ValueTypeStringToBoolInvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ToggleValueType.ValueOFF.Equals((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? ToggleValueType.ValueOFF : ToggleValueType.ValueON;
        }
    }
}
