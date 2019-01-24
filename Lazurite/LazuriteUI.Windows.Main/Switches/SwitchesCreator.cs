using Lazurite.MainDomain;
using System;
using System.Windows;

namespace LazuriteUI.Windows.Main.Switches
{
    public static class SwitchesCreator
    {
        public static FrameworkElement CreateScenarioControl(ScenarioBase scenario)
        {
            if (scenario.ValueType is Lazurite.ActionsDomain.ValueTypes.ButtonValueType || scenario.ValueType == null)
                return new ButtonView(scenario);
            if (scenario.ValueType is Lazurite.ActionsDomain.ValueTypes.DateTimeValueType)
                return new DateTimeView(scenario);
            if (scenario.ValueType is Lazurite.ActionsDomain.ValueTypes.FloatValueType)
                return new FloatView(scenario);
            if (scenario.ValueType is Lazurite.ActionsDomain.ValueTypes.ImageValueType)
                return new ImageView(scenario);
            if (scenario.ValueType is Lazurite.ActionsDomain.ValueTypes.InfoValueType)
                return new InfoView(scenario);
            if (scenario.ValueType is Lazurite.ActionsDomain.ValueTypes.StateValueType)
                return new StatusView(scenario);
            if (scenario.ValueType is Lazurite.ActionsDomain.ValueTypes.ToggleValueType)
                return new ToggleView(scenario);
            if (scenario.ValueType is Lazurite.ActionsDomain.ValueTypes.GeolocationValueType)
                return new GeolocationView(scenario);
            throw new Exception("Not compatible value type");
        }
    }
}
