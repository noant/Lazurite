using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Visual;
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
            _visualSettings = visualSettings;
            Scenario.SetOnStateChanged(ScenarioValueChanged);
            Scenario.CalculateCurrentValueAsync((value) => {
                _value = value;
                OnPropertyChanged(nameof(ScenarioValue));
            });
        }

        private string _value;
        private bool _editMode;
        private bool _checked;

        public ScenarioBase Scenario { get; private set; }
        private UserVisualSettings _visualSettings;
        private VisualSettingsRepository _visualSettingsRepository = Singleton.Resolve<VisualSettingsRepository>();

        public string Icon1
        {
            get
            {
                if (_visualSettings.AddictionalData == null)
                    _visualSettings.AddictionalData = new string[0];
                if (_visualSettings.AddictionalData.Any())
                    return _visualSettings.AddictionalData[0];
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
                if (_visualSettings.AddictionalData == null)
                    _visualSettings.AddictionalData = new string[0];
                if (_visualSettings.AddictionalData.Any())
                    _visualSettings.AddictionalData[0] = value;
                else _visualSettings.AddictionalData = new string[] { value, value };
                _visualSettingsRepository.Update(_visualSettings);
                OnPropertyChanged(nameof(Icon1));
            }
        }

        public string Icon2
        {
            get
            {
                if (_visualSettings.AddictionalData == null)
                    _visualSettings.AddictionalData = new string[0];
                if (_visualSettings.AddictionalData.Length > 1)
                    return _visualSettings.AddictionalData[1];
                else
                {
                    if (Scenario.ValueType is ToggleValueType)
                        return "Off";
                    else return "None";
                }
            }
            set
            {
                if (_visualSettings.AddictionalData == null)
                    _visualSettings.AddictionalData = new string[0];
                if (_visualSettings.AddictionalData.Length > 1)
                    _visualSettings.AddictionalData[1] = value;
                else if (_visualSettings.AddictionalData.Length == 1)
                    _visualSettings.AddictionalData = new string[] { _visualSettings.AddictionalData[0], value };
                else _visualSettings.AddictionalData = new string[] { "ButtonOn", value };
                _visualSettingsRepository.Update(_visualSettings);
                OnPropertyChanged(nameof(Icon2));
            }
        }

        public int PositionX
        {
            get
            {
                return _visualSettings.PositionX;
            }
            set
            {
                _visualSettings.PositionX = value;
                _visualSettingsRepository.Update(_visualSettings);
                OnPropertyChanged(nameof(PositionX));
            }
        }

        public int PositionY
        {
            get
            {
                return _visualSettings.PositionY;
            }
            set
            {
                _visualSettings.PositionY = value;
                _visualSettingsRepository.Update(_visualSettings);
                OnPropertyChanged(nameof(PositionY));
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
