using LazuriteMobile.App.Controls;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches.Bases.Converters
{
    public class Foreground_StateToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (ScenarioState)value;

            switch (state)
            {
                case ScenarioState.NotSelected:
                    return Visual.Current.SwitchForeground;
                case ScenarioState.ReadonlyAndNotSelected:
                    return Visual.Current.SwitchForegroundReadonly;
                case ScenarioState.Selected:
                    return Visual.Current.SelectedSwitchForeground;
                case ScenarioState.ReadonlyAndSelected:
                    return Visual.Current.SelectedSwitchForegroundReadonly;
            }

            throw new Exception("Unknown scenario state");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
