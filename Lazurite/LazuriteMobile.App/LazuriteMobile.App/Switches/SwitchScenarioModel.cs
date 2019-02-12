using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Shared;
using LazuriteMobile.MainDomain;
using System;

namespace LazuriteMobile.App.Switches
{
    public class SwitchScenarioModel: ObservableObject, IDisposable
    {
        private static readonly string Icon1Key = "Icon1";
        private static readonly string Icon2Key = "Icon2";

        private string _value;
        private bool _available;
        private bool _checked;

        private IScenariosManager _manager = Singleton.Resolve<LazuriteContext>().Manager;

        public UserVisualSettings VisualSettings => Scenario?.VisualSettings;

        public ScenarioInfo Scenario { get; private set; }

        public string ScenarioValueWithUnit => ConvertersStatic.ValueTypeStringToDoubleRoundStr.Convert(ScenarioValue, null, null, null) + Unit;

        public string Unit => (Scenario.ValueType as FloatValueType)?.Unit;
        
        public bool AllowClick => !Scenario.OnlyGetValue && Scenario.IsAvailable;

        public ScenarioState State
        {
            get
            {
                if (Scenario.ValueType is ToggleValueType)
                {
                    if (AllowClick && ScenarioValue == ToggleValueType.ValueON)
                        return ScenarioState.Selected;
                    else if (!AllowClick && ScenarioValue == ToggleValueType.ValueON)
                        return ScenarioState.ReadonlyAndSelected;
                    else if (AllowClick && ScenarioValue != ToggleValueType.ValueON)
                        return ScenarioState.NotSelected;
                    else
                        return ScenarioState.ReadonlyAndNotSelected;
                }
                else
                {
                    if (AllowClick)
                        return ScenarioState.NotSelected;
                    else
                        return ScenarioState.ReadonlyAndNotSelected;
                }
            }
        }

        public bool IsAvailable => Scenario.IsAvailable;

        public string ScenarioName => Scenario?.Name;

        public string[] AcceptedValues => Scenario.ValueType.AcceptedValues;

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
            OnPropertyChanged(nameof(ScenarioValueWithUnit));
            OnPropertyChanged(nameof(State));
            OnPropertyChanged(nameof(CurrentIcon));
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
                OnPropertyChanged(nameof(CurrentIcon));
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
                OnPropertyChanged(nameof(CurrentIcon));
            }
        }

        public string CurrentIcon
        {
            get
            {
                if (Scenario.ValueType is ToggleValueType == false)
                    return Icon1;

                if (ScenarioValue == ToggleValueType.ValueON)
                    return Icon2;
                else return Icon1;
            }
        }

        public string ScenarioValue
        {
            get => _value;
            set
            {
                _value = value;
                _manager.ExecuteScenario(new ExecuteScenarioArgs(Scenario.ScenarioId, _value));
                OnPropertyChanged(nameof(ScenarioValue));
                OnPropertyChanged(nameof(ScenarioValueWithUnit));
                OnPropertyChanged(nameof(State));

                if (Scenario.ValueType is ToggleValueType)
                    OnPropertyChanged(nameof(CurrentIcon));
            }
        } 
        
        public bool Checked
        {
            get => _checked;
            set
            {
                _checked = value;
                OnPropertyChanged(nameof(Checked));
            }
        }

        public double Max => (Scenario.ValueType as FloatValueType)?.Max ?? 100;

        public double Min => (Scenario.ValueType as FloatValueType)?.Min ?? 0;

        public bool Available
        {
            get => _available;
            set
            {
                _available = value;
                OnPropertyChanged(nameof(Available));
                OnPropertyChanged(nameof(AllowClick));
                OnPropertyChanged(nameof(State));

                if (Scenario.ValueType is ToggleValueType)
                    OnPropertyChanged(nameof(CurrentIcon));
            }
        }

        private void ScenarioValueChanged(object sender, EventsArgs<ScenarioInfo> args)
        {
            _value = args.Value.CurrentValue;
            OnPropertyChanged(nameof(ScenarioValue));
            OnPropertyChanged(nameof(ScenarioValueWithUnit));
            OnPropertyChanged(nameof(State));

            if (Scenario.ValueType is ToggleValueType)
                OnPropertyChanged(nameof(CurrentIcon));
        }

        public void Dispose()
        {
            Scenario.ValueChanged -= ScenarioValueChanged;
            _manager.ConnectionLost -= _manager_ConnectionLost;
            _manager.ConnectionRestored -= _manager_ConnectionRestored;
        }
    }

    public enum ScenarioState {
        ReadonlyAndNotSelected,
        ReadonlyAndSelected,
        Selected,
        NotSelected
    }
}
