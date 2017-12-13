using Lazurite.ActionsDomain.ValueTypes;
using System;
using System.Globalization;
using System.Windows.Data;

namespace LazuriteUI.Windows.Main.Switches
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class ValueTypeStringToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ToggleValueType.ValueON.Equals((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? ToggleValueType.ValueON : ToggleValueType.ValueOFF;
        }
    }
}
