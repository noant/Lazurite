using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Data;
using Lazurite.IOC;
using Lazurite.MainDomain;
using Lazurite.Scenarios.RemoteScenarioCode;
using Lazurite.Security;
using Lazurite.Utils;
using SimpleRemoteMethods.Bases;
using System;
using System.Threading.Tasks;
using ExecutionContext = Lazurite.ActionsDomain.ExecutionContext;

namespace Lazurite.Scenarios.ScenarioTypes
{
    [HumanFriendlyName("Сценарий другой машины")]
    [EncryptFile]
    public class RemoteScenario : ScenarioBase
    {
        private ValueTypeBase _valueType;
        private ScenarioInfo _scenarioInfo;
        private RemoteScenarioInfo _listenerInfo;

        /// <summary>
        /// Target server credentials
        /// </summary>
        public ConnectionCredentials Credentials { get; set; } = ConnectionCredentials.Default;
        
        /// <summary>
        /// Remote scenario guid
        /// </summary>
        public string RemoteScenarioId { get; set; }

        /// <summary>
        /// Remote scenario name
        /// </summary>
        public string RemoteScenarioName { get; set; }

        /// <summary>
        /// Value type of remote scenario
        /// </summary>
        public override ValueTypeBase ValueType
        {
            get => _valueType;
            set => _valueType = value;
        }

        public override string CalculateCurrentValue(ScenarioActionSource source, ExecutionContext parentContext)
        {
            CheckRights(source, parentContext);
            return CalculateCurrentValueInternal();
        }

        public override void CalculateCurrentValueAsync(ScenarioActionSource source, Action<string> callback, ExecutionContext parentContext)
        {
            CheckRights(source, parentContext);
            callback(CalculateCurrentValueInternal());
        }
        
        protected override string CalculateCurrentValueInternal() => GetCurrentValue();

        protected override void ExecuteInternal(ExecutionContext context)
        {
            //
        }
        
        public override void Execute(ScenarioActionSource source, string param, out string executionId, ExecutionContext parentContext)
        {
            CheckRights(source, parentContext);
            CheckValue(param, parentContext);
            executionId = PrepareExecutionId();
            TaskUtils.Start(() => {
                Log.Debug($"Scenario execution begin: [{Name}][{Id}]");
                try
                {
                    GetClient().ExecuteScenario(RemoteScenarioId, param).Wait();
                    SetCurrentValue(param, source);
                }
                catch (RemoteException e)
                {
                    Log.Info($"Ошибка соединения с удаленным сценарием [{Name}][{Id}][{Credentials.GetAddress()}]: {e.Message}");
                }
                catch (Exception e)
                {
                    Log.Info($"Непредвиденная ошибка соединения с удаленным сценарием [{Name}][{Id}][{Credentials.GetAddress()}]", e);
                }
                Log.Debug($"Scenario execution end: [{Name}][{Id}]");
            });
        }

        public override void ExecuteAsync(ScenarioActionSource source, string param, out string executionId, ExecutionContext parentContext)
        {
            CheckRights(source, parentContext);
            CheckValue(param, parentContext);
            executionId = PrepareExecutionId();
            Log.DebugFormat($"Scenario execution begin: [{Name}][{Id}]");
            TaskUtils.Start(() =>
            {
                try
                {
                    GetClient().AsyncExecuteScenario(RemoteScenarioId, param).Wait();
                    SetCurrentValue(param, source);
                }
                catch (Exception e) when (e is RemoteException == false)
                {
                    Log.Info($"Непредвиденная ошибка соединения с удаленным сценарием [{Name}][{Id}][{Credentials.GetAddress()}]", e);
                }
                catch (RemoteException e)
                {
                    Log.Info($"Ошибка соединения с удаленным сценарием [{Name}][{Id}][{Credentials.GetAddress()}]: {e.Message}");
                }
                Log.DebugFormat($"Scenario execution end: [{Name}][{Id}]");
            });
        }

        public override void ExecuteAsyncParallel(ScenarioActionSource source, string param, ExecutionContext parentContext)
        {
            CheckRights(source, parentContext);
            CheckValue(param, parentContext);
            Log.DebugFormat($"Scenario execution begin: [{Name}][{Id}]");

            TaskUtils.Start(() =>
            {
                try
                {
                    GetClient().AsyncExecuteScenarioParallel(RemoteScenarioId, param).Wait();
                }
                catch (Exception e) when (e is RemoteException == false)
                {
                    Log.Info($"Непредвиденная ошибка соединения с удаленным сценарием [{Name}][{Id}][{Credentials.GetAddress()}]", e);
                }
                catch (RemoteException e)
                {
                    Log.Info($"Ошибка соединения с удаленным сценарием [{Name}][{Id}][{Credentials.GetAddress()}]: {e.Message}");
                }
                Log.DebugFormat($"Scenario execution end: [{Name}][{Id}]");
            });
        }

        public override void TryCancelAll()
        {
            if (_listenerInfo != null)
                RemoteScenarioChangesListener.Unregister(_listenerInfo);
            base.TryCancelAll();
        }

        public override Type[] GetAllUsedActionTypes() => new Type[0];
        
        protected override async Task<bool> InitializeInternal()
        {
            await base.InitializeInternal();
            SetInitializationState(ScenarioInitializationValue.Initializing);
            UpdateValueAsDefault();
            Log.DebugFormat($"Scenario initialize begin: [{Name}][{Id}]");
            SetIsAvailable(false);
            var remoteScenarioAvailable = false;
            var initialized = false;
            try
            {
                if (string.IsNullOrEmpty(Credentials.SecretKey))
                    throw new RemoteException(ErrorCode.UnknownData, "Необходим ввод секретного ключа");
                _scenarioInfo = await GetClient().GetScenarioInfo(RemoteScenarioId);
                remoteScenarioAvailable = _scenarioInfo.IsAvailable;
                _valueType = _scenarioInfo.ValueType;
                RemoteScenarioName = _scenarioInfo.Name;
                initialized = true;
            }
            catch (Exception e) when (e is RemoteException == false)
            {
                initialized = false;
                SetDefaultValue();
                Log.Info($"Непредвиденная ошибка соединения с удаленным сценарием [{Name}][{Id}][{Credentials.GetAddress()}]", e);
            }
            catch (RemoteException e)
            {
                initialized = false;
                SetDefaultValue();
                Log.Info($"Ошибка соединения с удаленным сценарием [{Name}][{Id}][{Credentials.GetAddress()}]: {e.Message}");
            }
            Log.Debug($"Scenario initialize end: [{Name}][{Id}]");
            SetIsAvailable(initialized && remoteScenarioAvailable);
            SetInitializationState(ScenarioInitializationValue.Initialized);
            return initialized;
        }
        
        public override void AfterInitilize()
        {
            //changes listener
            if (_listenerInfo != null)
                RemoteScenarioChangesListener.Unregister(_listenerInfo);
            _listenerInfo = new RemoteScenarioInfo(Name, Credentials, RemoteScenarioId, ValueChanged, AvailabilityChanged);
            RemoteScenarioChangesListener.Register(_listenerInfo);
        }

        public override async Task<bool> FullInitialize()
        {
            var result = await InitializeInternal();
            AfterInitilize(); // Start anyway - it starts listen remote scenario availability
            return result;
        }

        private void ValueChanged(RemoteScenarioValueChangedArgs args)
        {
            _scenarioInfo = args.ScenarioInfo;
            ValueType = _scenarioInfo.ValueType;
            var value = _scenarioInfo.CurrentValue ?? ValueType.DefaultValue;
            if (!value.Equals(GetCurrentValue()))
                SetCurrentValue(value, SystemActionSource);
            if (_scenarioInfo.IsAvailable != GetIsAvailable())
                SetIsAvailable(_scenarioInfo.IsAvailable);
        }

        private void AvailabilityChanged(RemoteScenarioAvailabilityChangedArgs args)
        {
            if (args.IsAvailable != GetIsAvailable())
            {
                SetIsAvailable(args.IsAvailable);
                if (_scenarioInfo != null)
                    _scenarioInfo.IsAvailable = args.IsAvailable;
            }
        }
        
        private void SetDefaultValue()
        {
            if (ValueType == null)
                ValueType = new ButtonValueType();
            SetCurrentValue(ValueType.DefaultValue, SystemActionSource);
        }

        private void UpdateValueAsDefault()
        {
            if (ValueType == null)
                ValueType = new ButtonValueType();
            SetCurrentValueNoEvents(ValueType.DefaultValue);
        }

        public async Task ReInitialize() => await InitializeInternal();
        
        public override IAction[] GetAllActionsFlat() => new IAction[0];

        public LazuriteClient GetClient() => ServiceClientFactory.Current.GetClient(Credentials);

        public override SecuritySettingsBase SecuritySettings { get; set; } = new SecuritySettings();        
    }
}