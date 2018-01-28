using Lazurite.ActionsDomain.ValueTypes;
using System;
using System.Globalization;
using System.Windows.Data;

namespace LazuriteUI.Windows.Main.Switches
{
    [ValueConversion(typeof(string), typeof(string))]
    public class GeolocationDateTimeValueTypeToSplittedString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var dateTime = GeolocationData.FromString(value.ToString()).DateTime;
                return dateTime.ToString().Replace(" ", "\r\n");
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
