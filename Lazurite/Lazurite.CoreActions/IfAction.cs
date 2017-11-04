using Lazurite.ActionsDomain;
using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.CoreActions.CoreActions;

namespace Lazurite.CoreActions
{
    [VisualInitialization]
    [OnlyExecute]
    [SuitableValueTypes(typeof(ButtonValueType))]
    [HumanFriendlyName("Если")]
    public class IfAction : IAction, IMultipleAction
    {
        public ComplexAction ActionIf { get; set; } = new ComplexAction();
        public ComplexAction ActionElse { get; set; } = new ComplexAction();
        public ComplexCheckerAction Checker { get; set; } = new ComplexCheckerAction();

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
        
        public ValueTypeBase ValueType
        {
            get;
            set;
        } = new ButtonValueType();

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
                return false;
            }
        }

        public bool IsSupportsModification
        {
            get
            {
                return true;
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
