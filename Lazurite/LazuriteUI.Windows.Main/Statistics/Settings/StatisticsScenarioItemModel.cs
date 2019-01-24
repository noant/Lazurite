using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using Lazurite.Scenarios.ScenarioTypes;

namespace LazuriteUI.Windows.Main.Statistics.Settings
{
    public class StatisticsScenarioItemModel : ObservableObject
    {
        private static IStatisticsManager StatisticsManager = Singleton.Resolve<IStatisticsManager>();

        private ScenarioBase _scenario;

        private bool _registered;

        public StatisticsScenarioItemModel(ScenarioBase scenario, bool registered)
        {
            _scenario = scenario;
            _registered = registered;
            OnPropertyChanged(nameof(IsStatisticsRegistered));
            OnPropertyChanged(nameof(ScenarioName));
            OnPropertyChanged(nameof(IsLocalScenario));
        }

        public string ScenarioName => _scenario.Name;

        public bool IsLocalScenario => _scenario is RemoteScenario == false;

        public bool IsStatisticsRegistered
        {
            get => _scenario.GetIsAvailable() && _registered;
            set {
                if (value)
                    StatisticsManager.Register(_scenario);
                else StatisticsManager.UnRegister(_scenario);
                _registered = value;
                OnPropertyChanged(nameof(IsStatisticsRegistered));
            }
        }
    }
}
