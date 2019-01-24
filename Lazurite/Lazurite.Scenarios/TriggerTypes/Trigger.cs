using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.CoreActions;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.Scenarios.ScenarioTypes;
using Lazurite.Shared;
using Lazurite.Utils;
using System;
using System.Linq;
using System.Threading;
using ExecutionContext = Lazurite.ActionsDomain.ExecutionContext;

namespace Lazurite.Scenarios.TriggerTypes
{
    [HumanFriendlyName("Триггер")]
    public class Trigger : TriggerBase
    {
        private readonly static ILogger Log = Singleton.Resolve<ILogger>();
        private readonly static ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();
        private readonly static int TriggerChangesListenInterval = GlobalSettings.Get(300);
        private readonly static UsersRepositoryBase UsersRepository = Singleton.Resolve<UsersRepositoryBase>();
        private readonly static ScenarioActionSource ViewActionSource = new ScenarioActionSource(UsersRepository.SystemUser, ScenarioStartupSource.OtherScenario, ScenarioAction.ViewValue);
        private readonly static ScenarioActionSource ExecuteActionSource = new ScenarioActionSource(UsersRepository.SystemUser, ScenarioStartupSource.System, ScenarioAction.Execute);

        private EventsHandler<ScenarioValueChangedEventArgs> _lastSubscribe;

        public override IAction TargetAction
        {
            get;
            set;
        } = new ComplexAction();

        public override IAction[] GetAllActionsFlat()
        {
            if (TargetAction is IMultipleAction multipleAction)
                return multipleAction.GetAllActionsFlat();
            else return new IAction[0];
        }

        public override Type[] GetAllUsedActionTypes()
        {
            return GetAllActionsFlat().Select(x => x.GetType()).Distinct().ToArray();
        }

        public override void Stop()
        {
            base.Stop();
            if (_lastSubscribe != null && GetScenario() != null)
                GetScenario().RemoveOnStateChanged(_lastSubscribe);
        }

        public override void Initialize()
        {
            try
            {
                var repository = Singleton.Resolve<ScenariosRepositoryBase>();
                SetScenario(repository.Scenarios.FirstOrDefault(x => x.Id.Equals(TargetScenarioId)));
                var instanceManager = Singleton.Resolve<IInstanceManager>();
                foreach (var action in ((ComplexAction)TargetAction).GetAllActionsFlat())
                {
                    if (action != null)
                    {
                        instanceManager.PrepareInstance(action, this);
                        action.Initialize();
                    }
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat(e, "Во время инициализации триггера [{0}] возникла ошибка", Name);
            }
        }

        public override void AfterInitialize()
        {
            if (Enabled)
                Run();
            else
                Stop();
        }

        protected override void RunInternal()
        {
            if (GetScenario() == null)
                return;

            //удаляем старую подписку, если имеется
            if (_lastSubscribe != null)
                GetScenario().RemoveOnStateChanged(_lastSubscribe);

            //выполнение по подписке на изменение значения
            var executeBySubscription = true;

            //если сценарий это одиночное действие и нельзя подписаться на изменение целевого действия
            //то не выполняем по подписке, а выполняем просто через цикл 
            if (GetScenario() is SingleActionScenario singleActionScen && !singleActionScen.ActionHolder.Action.IsSupportsEvent)
                executeBySubscription = false;

            var contexCancellationTokenSource = new CancellationTokenSource();
            CancellationToken.Value.Register(() => contexCancellationTokenSource.Cancel());
            if (executeBySubscription)
            {
                _lastSubscribe = (sender, args) =>
                {
                    if (CancellationToken.Value.IsCancellationRequested)
                    {
                        //crutch; scenario can be changed before initializing, then we need to remove 
                        //current subscribe from previous scenario. CancellationToken.IsCancellationRequested
                        //can be setted in true only when trigger stopped;
                        args.Value.Scenario.RemoveOnStateChanged(_lastSubscribe);
                    }
                    else
                    {
                        if (!args.Value.OnlyIntent)
                        {
                            var action = TargetAction;
                            var outputChanged = new OutputChangedDelegates();
                            outputChanged.Add((value) => GetScenario().SetCurrentValue(value, ExecuteActionSource));
                            contexCancellationTokenSource.Cancel();
                            contexCancellationTokenSource = new CancellationTokenSource();
                            var executionContext = new ExecutionContext(this, args.Value.Value, args.Value.PreviousValue, outputChanged, contexCancellationTokenSource);
                            TaskUtils.StartLongRunning(
                                () => action.SetValue(executionContext, string.Empty),
                                (exception) => Log.ErrorFormat(exception, "Ошибка выполнения триггера [{0}][{1}]", Name, Id));
                        }
                    }
                };
                GetScenario().SetOnStateChanged(_lastSubscribe);
            }
            else
            {
                var lastVal = string.Empty;
                var timerCancellationToken = SystemUtils.StartTimer(
                    (token) => {
                        try
                        {
                            var curVal = GetScenario().CalculateCurrentValue(ViewActionSource, null);
                            if (!lastVal.Equals(curVal))
                            {
                                var prevVal = GetScenario().GetPreviousValue();
                                lastVal = curVal;
                                contexCancellationTokenSource.Cancel();
                                contexCancellationTokenSource = new CancellationTokenSource();
                                var executionContext = new ExecutionContext(this, curVal, prevVal, new OutputChangedDelegates(), contexCancellationTokenSource);
                                try
                                {
                                    TargetAction.SetValue(executionContext, string.Empty);
                                }
                                catch (Exception exception)
                                {
                                    Log.ErrorFormat(exception, "Ошибка выполнения триггера [{0}][{1}]", Name, Id);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Log.ErrorFormat(e, "Ошибка выполнения триггера [{0}][{1}]", Name, Id);
                        }
                    },
                    () => TriggerChangesListenInterval,
                    true,
                    ticksSuperposition: true /*наложение тиков*/);
                CancellationToken.Value.Register(() => timerCancellationToken.Cancel());
            }
        }
    }
}