using System.Linq;
using Pyrite.MainDomain;
using Pyrite.IOC;
using Pyrite.ActionsDomain;
using Pyrite.CoreActions.CoreActions;
using System;
using System.Threading;
using Pyrite.ActionsDomain.Attributes;
using Pyrite.ActionsDomain.ValueTypes;

namespace Pyrite.CoreActions.CoreActions
{
    [HumanFriendlyName("ПолучитьЗначениеСценария")]
    [VisualInitialization]
    [OnlyGetValue]
    [SuitableValueTypes(true)]
    public class GetExistingScenarioValueAction : ICoreAction, IAction
    {
        public string TargetScenarioId
        {
            get; set;
        }

        private ScenarioBase _scenario;

        public ValueChangedDelegate ValueChanged { get; set; }

        public void SetTargetScenario(ScenarioBase scenario)
        {
            _scenario = scenario;
        }

        public ScenarioBase GetTargetScenario()
        {
            return _scenario;
        }
        
        public string Caption
        {
            get
            {
                return _scenario.Name;
            }
            set
            {
                //
            }
        }

        public ValueTypeBase ValueType
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
        
        public void Initialize()
        {
            //do nothing
        }
        
        public void UserInitializeWith(ValueTypeBase valueType)
        {
            //do nothing
        }

        public string GetValue(ExecutionContext context)
        {
            return _scenario.CalculateCurrentValue();
        }

        public void SetValue(ExecutionContext context, string value)
        {
            //
        }
    }
}