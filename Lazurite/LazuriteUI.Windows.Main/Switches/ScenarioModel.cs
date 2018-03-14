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
        private static readonly VisualSettingsRepository Repository = Singleton.Resolve<VisualSettingsRepository>();
        private static readonly UsersRepositoryBase UsersRepository = Singleton.Resolve<UsersRepositoryBase>();
        private static readonly string Icon1Key = "Icon1";
        private static readonly string Icon2Key = "Icon2";

        private static readonly ScenarioActionSource ExecuteScenarioActionSource = new ScenarioActionSource(UsersRepository.SystemUser, ScenarioStartupSource.SystemUI, ScenarioAction.Execute);
        private static readonly ScenarioActionSource ViewScenarioActionSourceCrutch = new ScenarioActionSource(UsersRepository.SystemUser, ScenarioStartupSource.System, ScenarioAction.ViewValue);

        public ScenarioModel(ScenarioBase scenario)
        {
            Scenario = scenario;
            VisualSettings = Repository.VisualSettings.FirstOrDefault(x => x.ScenarioId == scenario.Id);
            if (VisualSettings == null)
            {
                Repository.Add(VisualSettings = new UserVisualSettings()
                {
                    UserId = UsersRepository.SystemUser.Id,
                    ScenarioId = scenario.Id,
                    VisualIndex = (Repository.VisualSettings.Any() ? Repository.VisualSettings.Max(x=>x.VisualIndex) : 0) + 1
                });
                VisualSettings.AddictionalData.Set(Icon1Key, GetStandardIcon1());
                VisualSettings.AddictionalData.Set(Icon2Key, GetStandardIcon2());
                Repository.Save();
            }

            Scenario.SetOnStateChanged(ScenarioValueChanged);
            scenario.SetOnAvailabilityChanged(ScenarioAvailabilityChanged);
            Scenario.CalculateCurrentValueAsync(
                ViewScenarioActionSourceCrutch,
                (value) => {
                    _value = value;
                    _smoothValue = ScenarioValueDouble;
                    OnPropertyChanged(nameof(ScenarioValue));
                    OnPropertyChanged(nameof(SmoothChangeValue));
                }, 
            null);

            OnPropertyChanged(nameof(Icon1));
            OnPropertyChanged(nameof(Icon2));
            OnPropertyChanged(nameof(AllowClick));
        }

        public ScenarioModel() : this(null) { }

        private string _value;
        private double _smoothValue;
        private bool _editMode;
        private bool _checked;

        public ScenarioBase Scenario { get; private set; }
        public UserVisualSettings VisualSettings { get; private set; }
        
        public void RefreshAndReCalculate()
        {
            Scenario.CalculateCurrentValueAsync(
                ViewScenarioActionSourceCrutch,
                (value) => {
                    _value = value;
                    _smoothValue = ScenarioValueDouble;
                    Refresh();
                }, 
            null);
        }

        public void Refresh()
        {
            OnPropertyChanged(nameof(ScenarioName));
            OnPropertyChanged(nameof(ScenarioValue));
            OnPropertyChanged(nameof(ScenarioValueWithUnit));
            OnPropertyChanged(nameof(Max));
            OnPropertyChanged(nameof(Min));
            OnPropertyChanged(nameof(Icon1));
            OnPropertyChanged(nameof(Icon2));
            OnPropertyChanged(nameof(AllowClick));
            OnPropertyChanged(nameof(IsAvailable));
        }

        public bool AllowClick => EditMode || (!IsReadOnly && IsAvailable);

        public bool IsReadOnly => !Scenario.IsAccessAvailable(ExecuteScenarioActionSource);

        public bool IsAvailable => Scenario.GetIsAvailable();

        public string Icon1
        {
            get => VisualSettings.AddictionalData[Icon1Key].ToString();
            set
            {
                VisualSettings.AddictionalData[Icon1Key] = value;
                Repository.Update(VisualSettings);
                Repository.Save();
                OnPropertyChanged(nameof(Icon1));
            }
        }

        public string Icon2
        {
            get => VisualSettings.AddictionalData[Icon2Key].ToString();
            set
            {
                VisualSettings.AddictionalData[Icon2Key] = value;
                Repository.Update(VisualSettings);
                Repository.Save();
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
            else return "New";
        }

        public int VisualIndex
        {
            get => VisualSettings.VisualIndex;
            set
            {
                if (VisualSettings.VisualIndex != value)
                {
                    VisualSettings.VisualIndex = value;
                    Repository.Update(VisualSettings);
                    OnPropertyChanged(nameof(VisualIndex));
                }
            }
        }

        public string ScenarioName => Scenario.Name;

        public string Unit => (Scenario.ValueType as FloatValueType)?.Unit;

        public string[] AcceptedValues => Scenario.ValueType.AcceptedValues;

        public string ScenarioValue
        {
            get => _value;
            set
            {
                _value = value;
                Scenario.ExecuteAsync(ExecuteScenarioActionSource, _value, out string executionId);
            }
        }

        public string ScenarioValueWithUnit => ConvertersStatic.ValueTypeStringToDoubleRound.Convert(ScenarioValue, null, null, null) + Unit;

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

        public double SmoothChangeValue
        {
            get => _smoothValue;
            set
            {
                _smoothValue = value;
                OnPropertyChanged(nameof(SmoothChangeValue));
            }
        }

        public double Max => double.Parse((Scenario.ValueType as FloatValueType)?.AcceptedValues.Last());

        public double Min => double.Parse((Scenario.ValueType as FloatValueType)?.AcceptedValues.First());

        public bool EditMode
        {
            get => _editMode;
            set
            {
                _editMode = value;
                OnPropertyChanged(nameof(EditMode));
                OnPropertyChanged(nameof(AllowClick));
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

        private void ScenarioValueChanged(object sender, EventsArgs<ScenarioBase> args)
        {
            _value = args.Value.GetCurrentValue();
            _smoothValue = ScenarioValueDouble;
            OnPropertyChanged(nameof(ScenarioValue));
            OnPropertyChanged(nameof(ScenarioValueWithUnit));
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
