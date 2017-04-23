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
    [HumanFriendlyName("Если")]
    public class IfAction : IAction, IMultipleAction
    {
        public ComplexAction ActionIf { get; set; }
        public ComplexAction ActionElse { get; set; }
        public ComplexCheckerAction Checker { get; set; }

        public string Caption
        {
            get
            {
                return "ЕСЛИ";
            }
            set
            {
                //
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
            return new IAction[] { ActionIf, ActionElse, Checker }
            .Union(ActionIf.GetAllActionsFlat())
            .Union(ActionElse.GetAllActionsFlat())
            .Union(Checker.GetAllActionsFlat())
            .ToArray();
        }

        public bool IsSupportsEvent
        {
            get
            {
                return ValueChanged != null;
            }
        }

        public void Initialize()
        {
            //
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return false;
        }

        public string GetValue(ExecutionContext context)
        {
            return string.Empty;
        }

        public void SetValue(ExecutionContext context, string value)
        {
            if (Checker.Evaluate(context))
                ActionIf.SetValue(context, string.Empty);
            else
                ActionElse.SetValue(context, string.Empty);
        }

        public event ValueChangedDelegate ValueChanged;
    }
}
