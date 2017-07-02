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
        private IScenariosManager _manager = Singleton.Resolve<IScenariosManager>(); 

        public ScenarioModel(ScenarioInfo scenario)
        {
            _manager.ConnectionLost += _manager_ConnectionLost;
            _manager.ConnectionRestored += _manager_ConnectionRestored;

            RefreshWith(scenario);
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
        private bool _editMode;
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
            OnPropertyChanged(nameof(Icon1));
            OnPropertyChanged(nameof(Icon2));
            OnPropertyChanged(nameof(ScenarioName));
            OnPropertyChanged(nameof(ScenarioValue));
        }

        public string Icon1
        {
            get
            {
                if (VisualSettings.AddictionalData == null)
                    VisualSettings.AddictionalData = new string[0];
                if (VisualSettings.AddictionalData.Any() && !string.IsNullOrEmpty(VisualSettings.AddictionalData[0]))
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
                    return "New";
                }
            }
            set
            {
                if (VisualSettings.AddictionalData == null)
                    VisualSettings.AddictionalData = new string[0];
                if (VisualSettings.AddictionalData.Any())
                    VisualSettings.AddictionalData[0] = value;
                else VisualSettings.AddictionalData = new string[] { value, value };
                OnPropertyChanged(nameof(Icon1));
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
                OnPropertyChanged(nameof(Icon2));
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


        public bool EditMode
        {
            get
            {
                return _editMode;
            }
            set
            {
                _editMode = value;
                OnPropertyChanged(nameof(EditMode));
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
