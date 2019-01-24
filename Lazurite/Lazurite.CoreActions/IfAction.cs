using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using System.Linq;

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

#pragma warning disable 67
        public event ValueChangedEventHandler ValueChanged;
#pragma warning restore 67
    }
}
