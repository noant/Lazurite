using Lazurite.ActionsDomain.ValueTypes;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
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
