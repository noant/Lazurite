using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main.Switches
{
    public class ScenarioModel: ObservableObject, IDisposable
    {
        public ScenarioModel(ScenarioBase scenario, UserVisualSettings visualSettings)
        {
            Scenario = scenario;
            VisualSettings = visualSettings;
            Scenario.SetOnStateChanged(ScenarioValueChanged);
            Scenario.CalculateCurrentValueAsync((value) => {
                _value = value;
                OnPropertyChanged(nameof(ScenarioValue));
            });
        }

        private string _value;

        public UserVisualSettings VisualSettings { get; private set; }
        public ScenarioBase Scenario { get; private set; }
        
        public string Icon1
        {
            get
            {
                if (VisualSettings.AddictionalData == null)
                    VisualSettings.AddictionalData = new string[0];
                if (VisualSettings.AddictionalData.Any())
                    return VisualSettings.AddictionalData[0];
                else
                {
                    if (Scenario.ValueType is ToggleValueType)
                        return "ButtonOn";
                    else if (Scenario.ValueType is StateValueType)
                        return "New";
                    else if (Scenario.ValueType is FloatValueType)
                        return "DimensionArrowLineWidth";
                    else if (Scenario.ValueType is DateTimeValueType)
                        return "Timer";
                    else if (Scenario.ValueType is InfoValueType)
                        return "PageText";
                    return "None";
                }
            }
            set
            {
                if (VisualSettings.AddictionalData == null)
                    VisualSettings.AddictionalData = new string[0];
                if (VisualSettings.AddictionalData.Any())
                    VisualSettings.AddictionalData[0] = value;
                else VisualSettings.AddictionalData = new string[] { value, value };
            }
        }
        public string Icon2
        {
            get
            {
                if (VisualSettings.AddictionalData == null)
                    VisualSettings.AddictionalData = new string[0];
                if (VisualSettings.AddictionalData.Length > 1)
                    return VisualSettings.AddictionalData[1];
                else
                {
                    if (Scenario.ValueType is ToggleValueType)
                        return "Off";
                    else return "None";
                }
            }
            set
            {
                if (VisualSettings.AddictionalData == null)
                    VisualSettings.AddictionalData = new string[0];
                if (VisualSettings.AddictionalData.Length > 1)
                    VisualSettings.AddictionalData[1] = value;
                else if (VisualSettings.AddictionalData.Length == 1)
                    VisualSettings.AddictionalData = new string[] { VisualSettings.AddictionalData[0], value };
                else VisualSettings.AddictionalData = new string[] { "ButtonOn", value };
            }
        }

        public string ScenarioName
        {
            get
            {
                return Scenario.Name;
            }
        }

        public string[] AcceptedValues
        {
            get
            {
                return Scenario.ValueType.AcceptedValues;
            }
        }

        public string ScenarioValue
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                Scenario.ExecuteAsync(_value);
                OnPropertyChanged(nameof(ScenarioValue));
            }
        }

        public double Max
        {
            get
            {
                return double.Parse((Scenario.ValueType as FloatValueType)?.AcceptedValues.Last());
            }
        }
        
        public double Min
        {
            get
            {
                return double.Parse((Scenario.ValueType as FloatValueType)?.AcceptedValues.First());
            }
        }

        private void ScenarioValueChanged(ScenarioBase scenario)
        {
            _value = scenario.GetCurrentValue();
            OnPropertyChanged(nameof(ScenarioValue));
        }

        public void Dispose()
        {
            Scenario.RemoveOnStateChanged(ScenarioValueChanged);
        }
    }
}
