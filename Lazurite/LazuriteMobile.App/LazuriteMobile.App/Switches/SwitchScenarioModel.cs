using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Shared;
using LazuriteMobile.MainDomain;
using System;
using System.Linq;

namespace LazuriteMobile.App.Switches
{
    public class SwitchScenarioModel: ObservableObject, IDisposable
    {
        private static readonly string Icon1Key = "Icon1";
        private static readonly string Icon2Key = "Icon2";

        private IScenariosManager _manager = Singleton.Resolve<LazuriteContext>().Manager; 

        public SwitchScenarioModel(ScenarioInfo scenario)
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
            else return "_None";
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

        public UserVisualSettings VisualSettings
        {
            get
            {
                return Scenario?.VisualSettings;
            }
        }

        public ScenarioInfo Scenario { get; private set; }
        
        public void RefreshWith(ScenarioInfo scenario)
        {
            var prevIcon1 = Icon1;
            var prevIcon2 = Icon2;

            if (Scenario != null)
                Scenario.ValueChanged -= ScenarioValueChanged;

            Scenario = scenario;
            Scenario.ValueChanged += ScenarioValueChanged;
            _value = Scenario.CurrentValue;
            if (!VisualSettings.AddictionalData.ContainsKey(Icon1Key))
                VisualSettings.AddictionalData.Set(Icon1Key, GetStandardIcon1());
            if (!VisualSettings.AddictionalData.ContainsKey(Icon2Key))
                VisualSettings.AddictionalData.Set(Icon2Key, GetStandardIcon2());

            if (prevIcon1 != Icon1)
                OnPropertyChanged(nameof(Icon1));
            if (prevIcon2 != Icon2)
                OnPropertyChanged(nameof(Icon2));
            OnPropertyChanged(nameof(ScenarioName));
            OnPropertyChanged(nameof(ScenarioValue));
            OnPropertyChanged(nameof(AllowClick));
            OnPropertyChanged(nameof(IsAvailable));
        }

        public string Icon1
        {
            get
            {
                if (VisualSettings == null || !VisualSettings.AddictionalData.ContainsKey(Icon1Key))
                    return "_None";
                return VisualSettings.AddictionalData[Icon1Key].ToString();
            }
            set
            {
                VisualSettings.AddictionalData[Icon1Key] = value;
                OnPropertyChanged(nameof(Icon1));
            }
        }

        public string Icon2
        {
            get
            {
                if (VisualSettings == null || !VisualSettings.AddictionalData.ContainsKey(Icon2Key))
                    return "_None";
                return VisualSettings.AddictionalData[Icon2Key].ToString();
            }
            set
            {
                VisualSettings.AddictionalData[Icon2Key] = value;
                OnPropertyChanged(nameof(Icon2));
            }
        }

        public bool AllowClick
        {
            get
            {
                return !Scenario.OnlyGetValue && Scenario.IsAvailable;
            }
        }

        public bool IsAvailable
        {
            get
            {
                return Scenario.IsAvailable;
            }
        }

        public string ScenarioName
        {
            get
            {
                return Scenario?.Name;
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
                _manager.ExecuteScenario(new ExecuteScenarioArgs(Scenario.ScenarioId, _value));
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
                OnPropertyChanged(nameof(AllowClick));
            }
        }

        private void ScenarioValueChanged(object sender, EventsArgs<ScenarioInfo> args)
        {
            _value = args.Value.CurrentValue;
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
