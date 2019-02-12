using LazuriteMobile.App.Controls;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches.Bases.Converters
{
    public class IconColor_StateToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (ScenarioState)value;

            switch (state)
            {
                case ScenarioState.NotSelected:
                    return Visual.Current.SwitchIconColor;
                case ScenarioState.ReadonlyAndNotSelected:
                    return Visual.Current.SwitchIconColorReadonly;
                case ScenarioState.Selected:
                    return Visual.Current.SelectedSwitchIconColor;
                case ScenarioState.ReadonlyAndSelected:
                    return Visual.Current.SelectedSwitchIconColorReadonly;
            }

            throw new Exception("Unknown scenario state");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
