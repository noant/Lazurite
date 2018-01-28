using Lazurite.MainDomain;
using System;
using System.Windows.Controls;

namespace LazuriteUI.Windows.Main.Switches
{
    public static class SwitchesCreator
    {
        public static UserControl CreateScenarioControl(ScenarioBase scenario, UserVisualSettings visualSetting)
        {
            if (scenario.ValueType is Lazurite.ActionsDomain.ValueTypes.ButtonValueType || scenario.ValueType == null)
                return new ButtonView(scenario, visualSetting);
            if (scenario.ValueType is Lazurite.ActionsDomain.ValueTypes.DateTimeValueType)
                return new DateTimeView(scenario, visualSetting);
            if (scenario.ValueType is Lazurite.ActionsDomain.ValueTypes.FloatValueType)
                return new FloatView(scenario, visualSetting);
            if (scenario.ValueType is Lazurite.ActionsDomain.ValueTypes.ImageValueType)
                return new ImageView(scenario, visualSetting);
            if (scenario.ValueType is Lazurite.ActionsDomain.ValueTypes.InfoValueType)
                return new InfoView(scenario, visualSetting);
            if (scenario.ValueType is Lazurite.ActionsDomain.ValueTypes.StateValueType)
                return new StatusView(scenario, visualSetting);
            if (scenario.ValueType is Lazurite.ActionsDomain.ValueTypes.ToggleValueType)
                return new ToggleView(scenario, visualSetting);
            if (scenario.ValueType is Lazurite.ActionsDomain.ValueTypes.GeolocationValueType)
                return new GeolocationView(scenario, visualSetting);
            throw new Exception("Not compatible value type");
        }
    }
}
