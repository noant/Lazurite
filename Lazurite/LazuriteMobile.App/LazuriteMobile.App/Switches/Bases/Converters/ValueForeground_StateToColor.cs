using LazuriteMobile.App.Controls;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches.Bases.Converters
{
    public class ValueForeground_StateToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (ScenarioState)value;
            switch (state)
            {
                case ScenarioState.NotSelected:
                    return Visual.Current.SwitchValueForeground;
                case ScenarioState.ReadonlyAndNotSelected:
                    return Visual.Current.SwitchValueForegroundReadonly;
                case ScenarioState.Selected:
                    return Visual.Current.SelectedSwitchValueForeground;
                case ScenarioState.ReadonlyAndSelected:
                    return Visual.Current.SelectedSwitchValueForegroundReadonly;
            }

            throw new Exception("Unknown scenario state");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
