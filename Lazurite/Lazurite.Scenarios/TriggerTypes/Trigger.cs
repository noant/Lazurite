using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lazurite.ActionsDomain;
using System.Threading;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.CoreActions;
using Lazurite.Scenarios.ScenarioTypes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.IOC;
using Lazurite.CoreActions.ContextInitialization;
using Lazurite.CoreActions.CoreActions;
using Lazurite.Logging;

namespace Lazurite.Scenarios.TriggerTypes
{
    [HumanFriendlyName("Триггер")]
    public class Trigger : TriggerBase
    {
        private Action<ScenarioBase> _lastSubscribe;
        private ISystemUtils _systemUtils = Singleton.Resolve<ISystemUtils>();
        private ILogger _log = Singleton.Resolve<ILogger>();

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

        public override void Initialize(ScenariosRepositoryBase scenariosRepository)
        {
            try
            {
                SetScenario(scenariosRepository.Scenarios.FirstOrDefault(x => x.Id.Equals(this.TargetScenarioId)));
                foreach (var action in ((ComplexAction)this.TargetAction).GetAllActionsFlat())
                {
                    if (action != null)
                    {
                        var coreAction = action as ICoreAction;
                        coreAction?.SetTargetScenario(scenariosRepository.Scenarios.SingleOrDefault(x => x.Id.Equals(coreAction.TargetScenarioId)));
                        var initializable = action as IContextInitializable;
                        initializable?.Initialize(this);
                        action.Initialize();
                    }
                }
            }
            catch (Exception e)
            {
                _log.ErrorFormat(e, "Во время инициализации триггера [{0}] возникла ошибка", this.Name);
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
            var scenario = GetScenario();

            if (scenario == null)
                return;

            //удаляем старую подписку, если имеется
            if (_lastSubscribe != null)
                scenario.RemoveOnStateChanged(_lastSubscribe);

            //выполнение по подписке на изменение значения
            var executeBySubscription = true;

            //если сценарий это одиночное действие и нельзя подписаться на изменение целевого действия
            //то не выполняем по подписке, а выполняем просто через цикл 
            if (scenario is SingleActionScenario && !((SingleActionScenario)scenario).ActionHolder.Action.IsSupportsEvent)
                executeBySubscription = false;

            var contexCancellationTokenSource = new CancellationTokenSource();
            if (executeBySubscription)
            {
                _lastSubscribe = (s) =>
                {
                    var action = TargetAction;
                    var outputChanged = new OutputChangedDelegates();
                    outputChanged.Add((value) => scenario.SetCurrentValueInternal(value));
                    contexCancellationTokenSource.Cancel();
                    contexCancellationTokenSource = new CancellationTokenSource();
                    var executionContext = new ExecutionContext(this, s.GetCurrentValue(), outputChanged, contexCancellationTokenSource.Token);
                    Task.Factory.StartNew(() => action.SetValue(executionContext, string.Empty));
                };
                scenario.SetOnStateChanged(_lastSubscribe);
            }
            else
            {
                var lastVal = string.Empty;
                while (!cancellationToken.IsCancellationRequested)
                {
                    var curVal = scenario.CalculateCurrentValue();
                    if (!lastVal.Equals(curVal))
                    {
                        lastVal = curVal;
                        contexCancellationTokenSource.Cancel();
                        contexCancellationTokenSource = new CancellationTokenSource();
                        var executionContext = new ExecutionContext(this, curVal, new OutputChangedDelegates(), contexCancellationTokenSource.Token);
                        Task.Factory.StartNew(() => TargetAction.SetValue(executionContext, string.Empty));
                    }
                    _systemUtils.Sleep(300, cancellationToken);
                }
            }
            cancellationToken.Register(() => contexCancellationTokenSource.Cancel());
        }
    }
}