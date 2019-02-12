using LazuriteUI.Icons;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches.Bases.Converters
{
    public class StateToToggleIcon : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (ScenarioState)value;

            switch (state)
            {
                case ScenarioState.NotSelected:
                case ScenarioState.ReadonlyAndNotSelected:
                    return Icon._None;
                case ScenarioState.ReadonlyAndSelected:
                case ScenarioState.Selected:
                    return Icon.Check;
            }

            throw new Exception("Unknown scenario state");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.GetName(typeof(Icon), value);
        }
    }
}
