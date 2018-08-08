using System;
using System.Globalization;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public class ValueTypeStringToDoubleRoundStr : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Round(double.Parse((value??0).ToString()), 2).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }
}