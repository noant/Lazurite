using Pyrite.ActionsDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pyrite.ActionsDomain.ValueTypes;
using Pyrite.MainDomain;
using Pyrite.ActionsDomain.Attributes;

namespace Pyrite.CoreActions.CoreActions
{
    [OnlyGetValue]
    [SuitableValueTypes(true)]
    [HumanFriendlyName("ВходящееЗначение")]
    [VisualInitialization]
    public class GetInputValueAction : IAction, ICoreAction
    {
        public string Caption
        {
            get
            {
                return "ВходящееЗначение";
            }
            set
            {
                //
            }
        }

        public string TargetScenarioId
        {
            get; set;
        }

        public AbstractValueType ValueType
        {
            get
            {
                return _scenario.ValueType;
            }

            set
            {
                //
            }
        }

        public ScenarioBase GetTargetScenario()
        {
            return _scenario;
        }

        public string GetValue(ExecutionContext context)
        {
            return context.Input;
        }

        public void Initialize()
        {
            //
        }

        private ScenarioBase _scenario;
        public void SetTargetScenario(ScenarioBase scenario)
        {
            _scenario = scenario;
        }

        public void SetValue(ExecutionContext context, string value)
        {
            //
        }

        public void UserInitializeWith(AbstractValueType valueType)
        {
            //
        }

        public ValueChangedDelegate ValueChanged { get; set; }
    }
}
