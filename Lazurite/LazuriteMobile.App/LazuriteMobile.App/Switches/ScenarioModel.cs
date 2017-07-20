using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using LazuriteMobile.App;
using LazuriteMobile.MainDomain;
using LazuriteUI.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteMobile.App.Switches
{
    public class ScenarioModel: ObservableObject, IDisposable
    {
        private static readonly string Icon1Key = "Icon1";
        private static readonly string Icon2Key = "Icon2";

        private IScenariosManager _manager = Singleton.Resolve<IScenariosManager>(); 

        public ScenarioModel(ScenarioInfo scenario)
        {
            _manager.ConnectionLost += _manager_ConnectionLost;
            _manager.ConnectionRestored += _manager_ConnectionRestored;
            RefreshWith(scenario);
        }

        private string GetStandardIcon1()
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
            return "New";
        }

        private string GetStandardIcon2()
        {
            if (Scenario.ValueType is ToggleValueType)
                return "Off";
            else return "None";
        }

        private void _manager_ConnectionRestored()
        {
            Available = true;
        }

        private void _manager_ConnectionLost()
        {
            Available = false;
        }

        private string _value;
        private bool _available;
        private bool _checked;

        public UserVisualSettings VisualSettings {
            get
            {
                return Scenario.VisualSettings;
            }
        }
        public ScenarioInfo Scenario { get; private set; }
        
        public void RefreshWith(ScenarioInfo scenario)
        {
            Scenario = scenario;
            Scenario.ValueChanged += ScenarioValueChanged;
            this._value = Scenario.CurrentValue;
            if (!VisualSettings.AddictionalData.ContainsKey(Icon1Key))
                VisualSettings.AddictionalData.Set(Icon1Key, GetStandardIcon1());
            if (!VisualSettings.AddictionalData.ContainsKey(Icon2Key))
                VisualSettings.AddictionalData.Set(Icon2Key, GetStandardIcon2());
            OnPropertyChanged(nameof(Icon1));
            OnPropertyChanged(nameof(Icon2));
            OnPropertyChanged(nameof(ScenarioName));
            OnPropertyChanged(nameof(ScenarioValue));
            OnPropertyChanged(nameof(AllowClick));
        }

        public string Icon1
        {
            get
            {
                return VisualSettings.AddictionalData[Icon1Key];
            }
            set
            {
                VisualSettings.AddictionalData.Set(Icon1Key, value);
                OnPropertyChanged(nameof(Icon1));
            }
        }

        public string Icon2
        {
            get
            {
                return VisualSettings.AddictionalData[Icon2Key];
            }
            set
            {
                VisualSettings.AddictionalData.Set(Icon2Key, value);
                OnPropertyChanged(nameof(Icon2));
            }
        }

        public bool AllowClick
        {
            get
            {
                return !Scenario.OnlyGetValue;
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
                _manager.ExecuteScenario(Scenario.ScenarioId, _value);
                OnPropertyChanged(nameof(ScenarioValue));
            }
        }
        
        public double Max {
            get
            {
                return double.Parse((Scenario.ValueType as FloatValueType)?.AcceptedValues.Last());
            }
        }
                
        public bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                _checked = value;
                OnPropertyChanged(nameof(Checked));
            }
        }
        public double Min
        {
            get
            {
                return double.Parse((Scenario.ValueType as FloatValueType)?.AcceptedValues.First());
            }
        }

        public bool Available
        {
            get
            {
                return _available;
            }
            set
            {
                _available = value;
                OnPropertyChanged(nameof(Available));
            }
        }

        private void ScenarioValueChanged(ScenarioInfo scenario)
        {
            _value = scenario.CurrentValue;
            OnPropertyChanged(nameof(ScenarioValue));
        }

        public void Dispose()
        {
            Scenario.ValueChanged -= ScenarioValueChanged;
            _manager.ConnectionLost -= _manager_ConnectionLost;
            _manager.ConnectionRestored -= _manager_ConnectionRestored;
        }
    }
}
