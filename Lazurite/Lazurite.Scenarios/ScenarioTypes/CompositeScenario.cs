using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.CoreActions;
using Lazurite.CoreActions.ContextInitialization;
using Lazurite.CoreActions.CoreActions;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.Security;
using Lazurite.Shared;
using Lazurite.Utils;
using System;
using System.Linq;
using System.Threading;

namespace Lazurite.Scenarios.ScenarioTypes
{
    [HumanFriendlyName("Композитный сценарий")]
    public class CompositeScenario : ScenarioBase, IStandardValueAction
    {
        public ComplexAction TargetAction { get; set; } = new ComplexAction();

        public override ValueTypeBase ValueType
        {
            get;
            set;
        }

        public override void CalculateCurrentValueAsync(Action<string> callback)
        {
            //async is unneccesary
            callback(CalculateCurrentValue());
        }

        public override string CalculateCurrentValue()
        {
            //just return last "returned" state
            return GetCurrentValue();
        }
        
        protected override void ExecuteInternal(ExecutionContext context)
        {
            TargetAction.SetValue(context, string.Empty);
        }

        public override Type[] GetAllUsedActionTypes()
        {
            return TargetAction.GetAllActionsFlat().Select(x => x.GetType()).Distinct().ToArray();
        }
        
        private string _currentValue;
        public override void SetCurrentValueInternal(string value)
        {
            _currentValue = value;
            RaiseValueChangedEvents();
        }

        public override string GetCurrentValue()
        {
            return _currentValue;
        }

        public override void Initialize(Action<bool> callback)
        {
            try
            {
                var scensRepository = Singleton.Resolve<ScenariosRepositoryBase>();
                var usersRepository = Singleton.Resolve<UsersRepositoryBase>();

                if (InitializeWithValue == null)
                    InitializeWithValue = this.ValueType.DefaultValue;
                foreach (var action in this.TargetAction.GetAllActionsFlat())
                {
                    if (action != null)
                    {
                        var coreAction = action as IScenariosAccess;
                        coreAction?.SetTargetScenario(scensRepository.Scenarios.SingleOrDefault(x => x.Id.Equals(coreAction.TargetScenarioId)));
                        var initializable = action as IContextInitializable;
                        initializable?.Initialize(this);
                        var userAccess = action as IUsersDataAccess;
                        if (userAccess != null)
                            userAccess.NeedUsers = () => usersRepository.Users.ToArray();
                        action.Initialize();
                    }
                }
                this.IsAvailable = true;
            }
            catch (Exception e)
            {
                Log.ErrorFormat(e, "Во время инициализации сценария [{0}] возникла ошибка", this.Name);
                this.IsAvailable = false;
            }
            callback?.Invoke(this.IsAvailable);
        }

        public override void AfterInitilize()
        {
            if (this.ValueType != null && this.ValueType is ButtonValueType == false) //except buttonValueType because any input value starts scenario permanent
                ExecuteAsync(InitializeWithValue, out string executionId);
        }

        public override IAction[] GetAllActionsFlat()
        {
            return TargetAction.GetAllActionsFlat();
        }

        public string InitializeWithValue { get; set; }

        //crutch
        public string Value
        {
            get
            {
                return InitializeWithValue;
            }
            set
            {
                InitializeWithValue = value;
            }
        }

        public override SecuritySettingsBase SecuritySettings { get; set; } = new SecuritySettings();
    }
}
