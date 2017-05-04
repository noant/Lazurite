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
        public ScenarioModel(ScenarioBase scenario)
        {
            Scenario = scenario;
            Scenario.SetOnStateChanged(ScenarioValueChanged);
            Scenario.CalculateCurrentValueAsync((value) => {
                _value = value;
                OnPropertyChanged(nameof(ScenarioValue));
            });
        }

        private string _value;

        public ScenarioBase Scenario { get; private set; }
        
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

        public double Max {
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
