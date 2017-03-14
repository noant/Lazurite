using Pyrite.ActionsDomain;
using Pyrite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Pyrite.ActionsDomain.ValueTypes;
using Pyrite.ActionsDomain.Attributes;

namespace Pyrite.CoreActions
{
    [VisualInitialization]
    [OnlyExecute]
    [SuitableValueTypes(typeof(ButtonValueType))]
    [HumanFriendlyName("Пока")]
    public class WhileAction : IAction, IMultipleAction
    {
        public ComplexAction Action { get; set; }
        public ComplexCheckerAction Checker { get; set; }

        public string Caption
        {
            get
            {
                return string.Empty;
            }
            set
            {
                //
            }
        }
        
        private ButtonValueType _valueType = new ButtonValueType();
        public ActionsDomain.ValueTypes.ValueTypeBase ValueType
        {
            get
            {
                return _valueType;
            }
            set
            {
                //
            }
        }

        public IAction[] GetAllActionsFlat()
        {
            return new[] { Action, Action }
            .Union(Action.GetAllActionsFlat())
            .Union(Checker.GetAllActionsFlat())
            .ToArray();
        }

        public void Initialize()
        {
            //
        }
        
        public void UserInitializeWith(ValueTypeBase valueType)
        {
            //
        }

        public string GetValue(ExecutionContext context)
        {
            return string.Empty;
        }

        public void SetValue(ExecutionContext context, string value)
        {
            while (Checker.Evaluate(context))
            {
                if (context.CancellationToken.IsCancellationRequested)
                    break;
                Action.SetValue(context, string.Empty);
            }
        }

        public ValueChangedDelegate ValueChanged { get; set; }
    }
}
