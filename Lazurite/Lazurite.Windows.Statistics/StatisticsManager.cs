using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using Lazurite.Shared;
using Lazurite.Windows.Statistics.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lazurite.Windows.Statistics
{
    public class StatisticsManager : IStatisticsManager
    {
        private static readonly SaviorBase Savior = Singleton.Resolve<SaviorBase>();
        private static readonly ScenariosRepositoryBase ScenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
        private static readonly ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();

        private List<ScenarioBase> _statisticsScenarios = new List<ScenarioBase>();
        private List<StatisticsScenarioInfoInternal> _statisticsScenariosInfos = new List<StatisticsScenarioInfoInternal>();
        private CancellationTokenSource _timerCancellationTokenSource;

        public StatisticsManager()
        {
            LoadData();
            Initialize();
        }

        private void SaveData()
        {
            Savior.Set(nameof(_statisticsScenariosInfos), _statisticsScenariosInfos);
        }

        private void LoadData()
        {
            if (Savior.Has(nameof(_statisticsScenariosInfos)))
                _statisticsScenariosInfos = Savior.Get<List<StatisticsScenarioInfoInternal>>(nameof(_statisticsScenariosInfos));
            else _statisticsScenariosInfos = new List<StatisticsScenarioInfoInternal>();
        }

        private void Initialize()
        {
            InitializeScenarios();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            _timerCancellationTokenSource?.Cancel();
            _timerCancellationTokenSource = SystemUtils.StartTimer((c) => TimerTick(), () => 30000);
        }

        private void TimerTick()
        {
            foreach (var scenario in _statisticsScenarios.ToArray())
            {
                if (scenario.ValueType is ButtonValueType == false)
                {

                }
            }
        }

        private void InitializeScenarios()
        {
            _statisticsScenarios = ScenariosRepository
                .Scenarios
                .Where(x => _statisticsScenariosInfos.Any(z => z.ScenarioId == x.Id))
                .ToList();
        }

        private void RegisterInternal(ScenarioBase scenario)
        {
            scenario.SetOnStateChanged(EventTriggered);
            InitializeScenarios();
        }

        private void UnregisterInternal(ScenarioBase scenario)
        {
            scenario.RemoveOnStateChanged(EventTriggered);
            InitializeScenarios();
        }

        private void EventTriggered(object sender, EventsArgs<ScenarioValueChangedEventArgs> args)
        {

        }

        public StatisticsItem[] GetItems(StatisticsScenarioInfo info, DateTime since, DateTime to, ScenarioActionSource source)
        {
            var scenario = _statisticsScenarios.FirstOrDefault(x => x.Id == info.ID && ActionsDomain.Utils.GetValueTypeClassName(x.ValueType.GetType()) == info.ValueTypeName);
            if (scenario?.SecuritySettings.IsAvailableForUser(source.User, source.Source, source.Action) ?? false)
            {

            }
            throw new NotImplementedException();
        }

        public StatisticsScenarioInfo GetStatisticsInfoForScenario(ScenarioBase scenario, ScenarioActionSource source)
        {
            if (scenario.SecuritySettings.IsAvailableForUser(source.User, source.Source, source.Action))
            {

            }
            throw new ScenarioExecutionException(ScenarioExecutionError.AccessDenied);
        }

        public bool IsRegistered(ScenarioBase scenario) => _statisticsScenariosInfos.Any(x => x.ScenarioId == scenario.Id);

        public void Register(ScenarioBase scenario)
        {
            if (!IsRegistered(scenario))
            {
                _statisticsScenariosInfos.Add(
                    new StatisticsScenarioInfoInternal()
                    {
                        ScenarioId = scenario.Id,
                        ValueTypeName = ActionsDomain.Utils.GetValueTypeClassName(scenario.ValueType.GetType())
                    }
                );
                RegisterInternal(scenario);
                InitializeTimer();
                SaveData();
            }
        }

        public void UnRegister(ScenarioBase scenario)
        {
            if (IsRegistered(scenario))
            {
                _statisticsScenariosInfos.RemoveAll(x=> x.ScenarioId == scenario.Id);
                UnregisterInternal(scenario);
                InitializeTimer();
                SaveData();
            }
        }
    }
}
