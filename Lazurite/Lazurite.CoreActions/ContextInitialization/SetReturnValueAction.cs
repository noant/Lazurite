using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;

namespace Lazurite.CoreActions.ContextInitialization
{
    [OnlyExecute]
    [VisualInitialization]
    [HumanFriendlyName("Обновить значение сценария")]
    [SuitableValueTypes(true)]
    public class SetReturnValueAction : IAction, IMultipleAction, IContextInitializable
    {
        public bool IsSupportsEvent
        {
            get
            {
                return false;
            }
        }
        
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
        
        public ActionHolder InputValue { get; set; } = new ActionHolder();

        public string TargetScenarioId
        {
            get; set;
        }
        
        public ValueTypeBase ValueType
        {
            get;
            set;
        }

        public void Initialize()
        {
            //do nothing
        }

        public IAction[] GetAllActionsFlat()
        {
            return new[] { InputValue.Action };
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return true;
        }

        public string GetValue(ExecutionContext context)
        {
            return string.Empty;
        }

        public bool IsSupportsModification
        {
            get
            {
                return false;
            }
        }

        public void SetValue(ExecutionContext context, string value)
        {
            this.ValueType = context.AlgorithmContext.ValueType;
            context.Input = InputValue.Action.GetValue(context);
            context.OutputChanged.Execute(context.Input);
        }

        public void Initialize(IAlgorithmContext algoContext)
        {
            this.ValueType = algoContext.ValueType;
        }

        public event ValueChangedEventHandler ValueChanged;
    }
}