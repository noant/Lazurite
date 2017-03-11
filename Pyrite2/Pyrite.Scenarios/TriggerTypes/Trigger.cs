using Pyrite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pyrite.ActionsDomain;
using System.Threading;
using Pyrite.ActionsDomain.Attributes;
using Pyrite.CoreActions;
using Pyrite.Scenarios.ScenarioTypes;

namespace Pyrite.Scenarios.TriggerTypes
{
    [HumanFriendlyName("Триггер")]
    public class Trigger : TriggerBase
    {
        private Action<ScenarioBase> _lastSubscribe;

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

        public override void Initialize()
        {

        }

        protected override void RunInternal(CancellationToken cancellationToken)
        {
            var scenario = GetScenario();

            //удаляем старую подписку, если имеется
            if (_lastSubscribe != null)
                scenario.RemoveOnStateChanged(_lastSubscribe);

            //выполнение по подписке на изменение значения
            var executeBySubscription = true;

            //если сценарий это одиночное действие и нельзя подписаться на изменение целевого действия
            //то не выполняем по подписке, а выполняем просто черех цикл 
            if (scenario is SingleActionScenario && ((SingleActionScenario)scenario).TargetAction.ValueChanged == null)
                executeBySubscription = false;

            if (executeBySubscription)
            {
                _lastSubscribe = (s) =>
                {
                    var action = TargetAction;
                    var executionContext = new ExecutionContext(s.GetCurrentValue(), new OutputChangedDelegates(), cancellationToken);
                    Task.Factory.StartNew(() => action.SetValue(executionContext, string.Empty));
                };
                scenario.SetOnStateChanged(_lastSubscribe);
            }
        }
    }
}