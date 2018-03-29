using LazuriteMobile.App.Controls;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public class Selection_BoolToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return Visual.SelectedSwitchBackground;
            else
                return Visual.SelectedSwitchBackgroundReadonly;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
