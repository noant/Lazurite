using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xamarin.Forms;

namespace LazuriteMobile.App.Switches
{
    public static class SwitchesCreator
    {
        public static View CreateScenarioControl(ScenarioInfo scenario)
        {
            if (scenario.ValueType is Lazurite.ActionsDomain.ValueTypes.ButtonValueType)
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
            throw new Exception("Not compatible value type");
        }
    }
}
