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
            VisualSettings = visualSettings;
            Scenario.SetOnStateChanged(ScenarioValueChanged);
            Scenario.CalculateCurrentValueAsync((value) => {
                _value = value;
                OnPropertyChanged(nameof(ScenarioValue));
            });
            OnPropertyChanged(nameof(Icon1));
            OnPropertyChanged(nameof(Icon2));
        }

        public ScenarioModel() : this(null, null) { }

        private string _value;
        private bool _editMode;
        private bool _checked;

        public ScenarioBase Scenario { get; private set; }
        public UserVisualSettings VisualSettings { get; private set; }
        private VisualSettingsRepository _visualSettingsRepository = Singleton.Resolve<VisualSettingsRepository>();

        private string[] GetAddictionalOrCalculatedData()
        {
            if (VisualSettings.AddictionalData == null || !VisualSettings.AddictionalData.Any())
                return new[] { GetStandardIcon1(), GetStandardIcon2() };
            else
                return VisualSettings.AddictionalData;
        }

        public void Refresh()
        {
            OnPropertyChanged(nameof(ScenarioName));
            OnPropertyChanged(nameof(ScenarioValue));
            OnPropertyChanged(nameof(Max));
            OnPropertyChanged(nameof(Min));
            OnPropertyChanged(nameof(Icon1));
            OnPropertyChanged(nameof(Icon2));
        }

        public string Icon1
        {
            get
            {
                return GetAddictionalOrCalculatedData()[0];
            }
            set
            {
                VisualSettings.AddictionalData[0] = value;
                _visualSettingsRepository.Update(VisualSettings);
                OnPropertyChanged(nameof(Icon1));
            }
        }

        public string Icon2
        {
            get
            {
                return GetAddictionalOrCalculatedData()[1];
            }
            set
            {
                VisualSettings.AddictionalData[1] = value;
                _visualSettingsRepository.Update(VisualSettings);
                OnPropertyChanged(nameof(Icon2));
            }
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

        public int PositionX
        {
            get
            {
                return VisualSettings.PositionX;
            }
            set
            {
                if (VisualSettings.PositionX != value)
                {
                    VisualSettings.PositionX = value;
                    _visualSettingsRepository.Update(VisualSettings);
                    OnPropertyChanged(nameof(PositionX));
                }
            }
        }

        public int PositionY
        {
            get
            {
                return VisualSettings.PositionY;
            }
            set
            {
                if (VisualSettings.PositionY != value)
                {
                    VisualSettings.PositionY = value;
                    _visualSettingsRepository.Update(VisualSettings);
                    OnPropertyChanged(nameof(PositionY));
                }
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
