using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.MainDomain.Statistics;
using Lazurite.Utils;
using Lazurite.Visual;
using SimpleRemoteMethods.Bases;
using SimpleRemoteMethods.ServerSide;
using System;
using System.Linq;
using System.Reflection;

namespace Lazurite.Service
{
    public class LazuriteService : IServer
    {
        private static readonly ScenariosRepositoryBase ScenariosRepository = Singleton.Resolve<ScenariosRepositoryBase>();
        private static readonly UsersRepositoryBase UsersRepository = Singleton.Resolve<UsersRepositoryBase>();
        private static readonly VisualSettingsRepository VisualSettings = Singleton.Resolve<VisualSettingsRepository>();
        private static readonly ILogger WarningHandler = Singleton.Resolve<ILogger>();
        private static readonly AddictionalDataManager AddictionalDataManager = Singleton.Resolve<AddictionalDataManager>();
        private static readonly IStatisticsManager StatisticsManager = Singleton.Resolve<IStatisticsManager>();
        private static readonly ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();

        private UserBase GetCurrentUser()
        {
            var login = Server<IServer>.CurrentRequestContext.UserName;
            var user = UsersRepository.Users.SingleOrDefault(x => x.Login.Equals(login));
            if (user == null)
                ThrowUnauthorizedAccessException();

            return user;
        }

        private ScenarioBase GetScenarioWithPrivileges(string scenarioId, ScenarioActionSource actionSource)
        {
            var scenario = GetScenario(scenarioId);
                        
            if (!scenario.IsAccessAvailable(actionSource))
                ThrowUnauthorizedAccessException();

            return scenario;
        }

        private ScenarioInfoLW[] GetChangedScenariosInternal(DateTime since, UserBase user)
        {
            var scenarioActionSource = new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.ViewValue);
            return ScenariosRepository
                .Scenarios
                .Where(x => x.LastChange >= since)
                .Select(x =>
                {
                    bool isAvailable = true;
                    var curVal = string.Empty;
                    try
                    {
                        curVal = x.CalculateCurrentValue(scenarioActionSource, null);
                    }
                    catch
                    {
                        isAvailable = false;
                        curVal = x.ValueType.DefaultValue;
                    }
                    return new ScenarioInfoLW()
                    {
                        IsAvailable = isAvailable && x.GetIsAvailable(),
                        CurrentValue = curVal,
                        ScenarioId = x.Id
                    };
                }).ToArray();
        }

        private ScenarioBase GetScenario(string scenarioId)
        {
            var scenario = ScenariosRepository
                .Scenarios
                .SingleOrDefault(x => x.Id.Equals(scenarioId));

            if (scenario == null)
                ThrowScenarioNotExistException(scenarioId);

            return scenario;
        }

        private void ThrowUnauthorizedAccessException()
        {
            throw RemoteException.Get(RemoteExceptionData.AccessDenied, "Object access denied");
        }

        private void ThrowScenarioNotExistException(string scenarioId)
        {
            throw RemoteException.Get(RemoteExceptionData.InternalServerError, "Scenario not exist: " + scenarioId);
        }

        private UserVisualSettings GetVisualSettings(UserBase user, string scenarioId)
        {
            var visualSettings = VisualSettings.VisualSettings
                .SingleOrDefault(x => x is UserVisualSettings &&
                x.UserId.Equals(user.Id) && x.ScenarioId.Equals(scenarioId));

            //if we can not found visualSettings then get visualSetting of SystemUser
            if (visualSettings == null)
                visualSettings = VisualSettings.VisualSettings
                    .SingleOrDefault(x => x is UserVisualSettings &&
                    x.UserId.Equals(UsersRepository.SystemUser.Id) &&
                    x.ScenarioId.Equals(scenarioId));
            
            return visualSettings;
        }

        public string CalculateScenarioValue(string scenarioId)
        {
            var user = GetCurrentUser();
            var scenario = GetScenario(scenarioId);
            var actionSource = new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.ViewValue);
            return scenario.CalculateCurrentValue(actionSource, null);
        }
                
        public void ExecuteScenario(string scenarioId, string value)
        {
            var user = GetCurrentUser();
            var scenario = GetScenario(scenarioId);
            var actionSource = new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.Execute);
            scenario.Execute(actionSource, value, out string executionId);
        }

        public void AsyncExecuteScenario(string scenarioId, string value)
        {
            var user = GetCurrentUser();
            var scenario = GetScenario(scenarioId);
            var actionSource = new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.Execute);
            scenario.ExecuteAsync(actionSource, value, out string executionId);
        }

        public void AsyncExecuteScenarioParallel(string scenarioId, string value)
        {
            var user = GetCurrentUser();
            var scenario = GetScenario(scenarioId);
            var actionSource = new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.Execute);
            scenario.ExecuteAsyncParallel(actionSource, value, null);
        }

        public ScenarioInfoLW[] GetChangedScenarios(DateTime since)
        {
            return GetChangedScenariosInternal(since, GetCurrentUser());
        }
        
        public ScenarioInfo GetScenarioInfo(string scenarioId)
        {
            var user = GetCurrentUser();
            var scenario = GetScenario(scenarioId);
            var executeScenarioAction = new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.Execute);
            var viewScenarioAction = new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.ViewValue);

            var currentValue = string.Empty;
            var canSetValue = true;
            var isAvailable = true;

            try
            {
                currentValue = scenario.CalculateCurrentValue(viewScenarioAction, null);
                canSetValue = scenario.IsAccessAvailable(executeScenarioAction);
            }
            catch
            {
                isAvailable = false;
                canSetValue = false;
                currentValue = scenario.ValueType.DefaultValue;
            }

            return new ScenarioInfo()
            {
                CurrentValue = currentValue,
                ScenarioId = scenario.Id,
                ValueType = scenario.ValueType,
                VisualSettings = GetVisualSettings(user, scenario.Id),
                Name = scenario.Name,
                OnlyGetValue = !canSetValue,
                IsAvailable = isAvailable && scenario.GetIsAvailable()
            };
        }

        public ScenarioInfo[] GetScenariosInfo()
        {
            var user = GetCurrentUser();
            var executeScenarioAction = new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.Execute);
            var viewScenarioAction = new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.ViewValue);
            var result = ScenariosRepository
                .Scenarios
                .Where(x => x.IsAccessAvailable(viewScenarioAction))
                .Select(x => new ScenarioInfo()
                {
                    CurrentValue = x.CalculateCurrentValue(viewScenarioAction, null),
                    ScenarioId = x.Id,
                    ValueType = x.ValueType,
                    Name = x.Name,
                    VisualSettings = GetVisualSettings(user, x.Id),
                    OnlyGetValue = !x.IsAccessAvailable(executeScenarioAction),
                    IsAvailable = x.GetIsAvailable()
                }).ToArray();

            return result;
        }

        public string GetScenarioValue(string scenarioId)
        {
            var user = GetCurrentUser();
            var scenarioActionSource = new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.ViewValue);
            return GetScenario(scenarioId).CalculateCurrentValue(scenarioActionSource, null);
        }

        public bool IsScenarioValueChanged(string scenarioId, string lastKnownValue)
        {
            var user = GetCurrentUser();
            var decryptedLastKnown = lastKnownValue;
            var scenarioActionSource = new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.ViewValue);
            var scenario = GetScenario(scenarioId);

            try
            {
                return scenario
                    .CalculateCurrentValue(scenarioActionSource, null)
                    .Equals(decryptedLastKnown);
            }
            catch
            {
                return false;
            }
        }

        public void SaveVisualSettings(UserVisualSettings visualSettings)
        {
            var user = GetCurrentUser();
            visualSettings.UserId = user.Id;
            VisualSettings.Add(visualSettings);
            VisualSettings.Save();
        }

        public AddictionalData SyncAddictionalData(AddictionalData data)
        {
            var user = GetCurrentUser();
            WarningHandler.Info($"User AddictionalData sync: [{user.Name}];");
            AddictionalDataManager.Handle(data, user);
            var preparedData = AddictionalDataManager.Prepare(user);
            return preparedData;
        }

        public StatisticsScenarioInfo GetStatisticsInfoForScenario(ScenarioInfo info)
        {
            var user = GetCurrentUser();
            var scenarioId = info.ScenarioId;
            var scenario = ScenariosRepository.Scenarios.FirstOrDefault(x => x.Id == scenarioId);
            if (scenario == null)
                return null;
            return TaskUtils.Wait(StatisticsManager.GetStatisticsInfoForScenario(scenario, new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.ViewValue)));
        }

        public StatisticsItem[] GetStatistics(DateTime since, DateTime to, StatisticsScenarioInfo info)
        {
            var user = GetCurrentUser();
            return TaskUtils.Wait(StatisticsManager.GetItems(info, since, to, new ScenarioActionSource(user, ScenarioStartupSource.Network, ScenarioAction.ViewValue)));
        }

        public ScenarioStatisticsRegistration GetStatisticsRegistration(string[] scenariosIds)
        {
            var scenarios = ScenariosRepository.Scenarios.Where(x => scenariosIds.Contains(x.Id)).ToArray();
            return TaskUtils.Wait(StatisticsManager.GetRegistrationInfo(scenarios));
        }

        public string GetLazuriteVersion() => SystemUtils.CurrentLazuriteVersion;
    }
}