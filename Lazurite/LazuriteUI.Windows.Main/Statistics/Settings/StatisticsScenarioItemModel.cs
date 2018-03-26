using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using Lazurite.Scenarios.ScenarioTypes;
using Lazurite.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main.Statistics.Settings
{
    public class StatisticsScenarioItemModel : ObservableObject
    {
        private static IStatisticsManager StatisticsManager = Singleton.Resolve<IStatisticsManager>();

        private ScenarioBase _scenario;

        public StatisticsScenarioItemModel(ScenarioBase scenario)
        {
            _scenario = scenario;
            OnPropertyChanged(nameof(IsStatisticsRegistered));
            OnPropertyChanged(nameof(ScenarioName));
            OnPropertyChanged(nameof(IsLocalScenario));
        }

        public string ScenarioName => _scenario.Name;

        public bool IsLocalScenario => _scenario is RemoteScenario == false;

        public bool IsStatisticsRegistered
        {
            get => _scenario.GetIsAvailable() && StatisticsManager.IsRegistered(_scenario);
            set {
                if (value)
                    StatisticsManager.Register(_scenario);
                else StatisticsManager.UnRegister(_scenario);
                OnPropertyChanged(nameof(IsStatisticsRegistered));
            }
        }
    }
}
