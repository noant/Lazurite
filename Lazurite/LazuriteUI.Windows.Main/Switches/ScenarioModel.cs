using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Shared;
using Lazurite.Visual;
using System;
using System.Linq;

namespace LazuriteUI.Windows.Main.Switches
{
    public class ScenarioModel: ObservableObject, IDisposable
    {
        private static readonly string Icon1Key = "Icon1";
        private static readonly string Icon2Key = "Icon2";

        public ScenarioModel(ScenarioBase scenario, UserVisualSettings visualSettings)
        {
            Scenario = scenario;
            VisualSettings = visualSettings;
            Scenario.SetOnStateChanged(ScenarioValueChanged);
            scenario.SetOnAvailabilityChanged(ScenarioAvailabilityChanged);
            Scenario.CalculateCurrentValueAsync((value) => {
                _value = value;
                _smoothValue = ScenarioValueDouble;
                OnPropertyChanged(nameof(ScenarioValue));
                OnPropertyChanged(nameof(SmoothChangeValue));
            });

            if (!VisualSettings.AddictionalData.ContainsKey(Icon1Key))
                VisualSettings.AddictionalData.Set(Icon1Key, GetStandardIcon1());

            if (!VisualSettings.AddictionalData.ContainsKey(Icon2Key))
                VisualSettings.AddictionalData.Set(Icon2Key, GetStandardIcon2());

            OnPropertyChanged(nameof(Icon1));
            OnPropertyChanged(nameof(Icon2));
            OnPropertyChanged(nameof(AllowClick));
        }

        public ScenarioModel() : this(null, null) { }

        private string _value;
        private double _smoothValue;
        private bool _editMode;
        private bool _checked;

        public ScenarioBase Scenario { get; private set; }
        public UserVisualSettings VisualSettings { get; private set; }
        private VisualSettingsRepository _visualSettingsRepository = Singleton.Resolve<VisualSettingsRepository>();
        
        public void RefreshAndReCalculate()
        {
            Scenario.CalculateCurrentValueAsync((value) => {
                _value = value;
                _smoothValue = ScenarioValueDouble;
                Refresh();
            });
        }

        public void Refresh()
        {
            OnPropertyChanged(nameof(ScenarioName));
            OnPropertyChanged(nameof(ScenarioValue));
            OnPropertyChanged(nameof(Max));
            OnPropertyChanged(nameof(Min));
            OnPropertyChanged(nameof(Icon1));
            OnPropertyChanged(nameof(Icon2));
            OnPropertyChanged(nameof(AllowClick));
            OnPropertyChanged(nameof(IsAvailable));
        }
        
        public bool AllowClick
        {
            get
            {
                if (EditMode)
                    return true;
                else return !Scenario.OnlyGetValue && IsAvailable;
            }
        }

        public bool IsAvailable
        {
            get
            {
                return Scenario.IsAvailable;
            }
        }

        public string Icon1
        {
            get
            {
                return VisualSettings.AddictionalData[Icon1Key].ToString();
            }
            set
            {
                VisualSettings.AddictionalData[Icon1Key] = value;
                _visualSettingsRepository.Update(VisualSettings);
                OnPropertyChanged(nameof(Icon1));
            }
        }

        public string Icon2
        {
            get
            {
                return VisualSettings.AddictionalData[Icon2Key].ToString();
            }
            set
            {
                VisualSettings.AddictionalData[Icon2Key] = value;
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
            else return "_None";
        }

        public int VisualIndex
        {
            get
            {
                return VisualSettings.VisualIndex;
            }
            set
            {
                if (VisualSettings.VisualIndex != value)
                {
                    VisualSettings.VisualIndex = value;
                    _visualSettingsRepository.Update(VisualSettings);
                    OnPropertyChanged(nameof(VisualIndex));
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
                Scenario.ExecuteAsync(_value, out string executionId);
            }
        }

        public double ScenarioValueDouble
        {
            get
            {
                try
                {
                    return double.Parse((!string.IsNullOrEmpty(ScenarioValue) ? ScenarioValue : "0"));
                }
                catch
                {
                    return -1;
                }
            }
        }

        public double SmoothChangeValue { 
            get
            {
                return _smoothValue;
            }
            set
            {
                _smoothValue = value;
                OnPropertyChanged(nameof(SmoothChangeValue));
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
                OnPropertyChanged(nameof(AllowClick));
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

        private void ScenarioValueChanged(object sender, EventsArgs<ScenarioBase> args)
        {
            _value = args.Value.GetCurrentValue();
            _smoothValue = ScenarioValueDouble;
            OnPropertyChanged(nameof(ScenarioValue));
            OnPropertyChanged(nameof(SmoothChangeValue));
        }

        private void ScenarioAvailabilityChanged(object sender, EventsArgs<ScenarioBase> args)
        {
            OnPropertyChanged(nameof(IsAvailable));
            OnPropertyChanged(nameof(AllowClick));
        }

        public void Dispose()
        {
            Scenario.RemoveOnStateChanged(ScenarioValueChanged);
            Scenario.RemoveOnAvailabilityChanged(ScenarioAvailabilityChanged);
        }
    }
}
