using Lazurite.ActionsDomain.ValueTypes;
using LazuriteMobile.App.Controls;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public class Foreground_StringToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value as string == ToggleValueType.ValueON)
                return Visual.Current.SelectedSwitchForeground;
            else
                return Visual.Current.SwitchForeground;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
