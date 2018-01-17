using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.CoreActions;
using Lazurite.CoreActions.ContextInitialization;
using Lazurite.CoreActions.CoreActions;
using Lazurite.IOC;
using Lazurite.Logging;
using Lazurite.MainDomain;
using Lazurite.Scenarios.ScenarioTypes;
using Lazurite.Shared;
using Lazurite.Utils;
using System;
using System.Linq;
using System.Threading;

namespace Lazurite.Scenarios.TriggerTypes
{
    [HumanFriendlyName("Триггер")]
    public class Trigger : TriggerBase
    {
        private readonly static ILogger Log = Singleton.Resolve<ILogger>();
        private readonly static ISystemUtils SystemUtils = Singleton.Resolve<ISystemUtils>();
        private readonly static int TriggerChangesListenInterval = GlobalSettings.Get(300);

        private EventsHandler<ScenarioBase> _lastSubscribe;

        public override IAction TargetAction
        {
            get;
            set;
        } = new ComplexAction();

        public override IAction[] GetAllActionsFlat()
        {
            if (TargetAction is IMultipleAction)
                return ((IMultipleAction)TargetAction).GetAllActionsFlat();
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
                SetScenario(repository.Scenarios.FirstOrDefault(x => x.Id.Equals(this.TargetScenarioId)));
                var instanceManager = Singleton.Resolve<IInstanceManager>();
                foreach (var action in ((ComplexAction)this.TargetAction).GetAllActionsFlat())
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
                Log.ErrorFormat(e, "Во время инициализации триггера [{0}] возникла ошибка", this.Name);
            }
        }

        public override void AfterInitialize()
        {
            if (Enabled)
                Run();
            else
                Stop();
        }

        protected override void RunInternal(CancellationToken cancellationToken)
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
            if (GetScenario() is SingleActionScenario && !((SingleActionScenario)GetScenario()).ActionHolder.Action.IsSupportsEvent)
                executeBySubscription = false;

            var contexCancellationTokenSource = new CancellationTokenSource();
            cancellationToken.Register(() => contexCancellationTokenSource.Cancel());
            if (executeBySubscription)
            {
                _lastSubscribe = (sender, args) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        //crutch; scenario can be changed before initializing, then we need to remove 
                        //current subscribe from previous scenario. CancellationToken.IsCancellationRequested
                        //can be setted in true only when trigger stopped;
                        args.Value.RemoveOnStateChanged(_lastSubscribe);
                    }
                    else
                    {
                        var action = TargetAction;
                        var outputChanged = new OutputChangedDelegates();
                        outputChanged.Add((value) => GetScenario().SetCurrentValueInternal(value));
                        contexCancellationTokenSource.Cancel();
                        contexCancellationTokenSource = new CancellationTokenSource();
                        var executionContext = new ExecutionContext(this, args.Value.GetCurrentValue(), outputChanged, contexCancellationTokenSource.Token);
                        TaskUtils.StartLongRunning(
                            () => action.SetValue(executionContext, string.Empty),
                            (exception) => Log.ErrorFormat(exception, "Error while trigger execute [{0}][{1}]", this.Name, this.Id));
                    }
                };
                GetScenario().SetOnStateChanged(_lastSubscribe);
            }
            else
            {
                var lastVal = string.Empty;
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var curVal = GetScenario().CalculateCurrentValue();
                        if (!lastVal.Equals(curVal))
                        {
                            lastVal = curVal;
                            contexCancellationTokenSource.Cancel();
                            contexCancellationTokenSource = new CancellationTokenSource();
                            var executionContext = new ExecutionContext(this, curVal, new OutputChangedDelegates(), contexCancellationTokenSource.Token);
                            TaskUtils.StartLongRunning(
                                () => TargetAction.SetValue(executionContext, string.Empty),
                                (exception) => Log.ErrorFormat(exception, "Error while executing trigger [{0}][{1}]", this.Name, this.Id));
                        }
                        SystemUtils.Sleep(TriggerChangesListenInterval, cancellationToken);
                    }
                    catch (Exception e)
                    {
                        Log.ErrorFormat(e, "Error while trigger execute: [{0}][{1}]", this.Name, this.Id);
                    }
                }
            }
        }
    }
}