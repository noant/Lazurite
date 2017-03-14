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
    [OnlyExecute]
    [VisualInitialization]
    [SuitableValueTypes(typeof(ButtonValueType))]
    [HumanFriendlyName("Составное действие")]
    public class ComplexAction : IAction, IMultipleAction
    {
        public ComplexAction()
        {
            Actions = new List<IAction>();
        }

        public List<IAction> Actions { get; set; }

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

        public string Value
        {
            get
            {
                return string.Empty;
            }
            set
            {
                
            }
        }

        private ButtonValueType _valueType = new ButtonValueType();
        public ValueTypeBase ValueType
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
            return Actions
                .Union(
                Actions
                .Where(x => x is IMultipleAction)
                .Select(x => ((IMultipleAction)x).GetAllActionsFlat()).SelectMany(x => x)).ToArray();
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
            return string.Empty;
        }

        public void SetValue(ExecutionContext context, string value)
        {
            foreach (var action in Actions)
            {
                if (context.CancellationToken.IsCancellationRequested)
                    break;
                action.SetValue(context, string.Empty);
            }
        }

        public ValueChangedDelegate ValueChanged { get; set; }
    }
}
