using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared.ActionCategory;

namespace Lazurite.CoreActions.ContextInitialization
{
    [OnlyExecute]
    [VisualInitialization]
    [HumanFriendlyName("Обновить значение сценария")]
    [SuitableValueTypes(true)]
    [Category(Category.Meta)]
    public class SetReturnValueAction : IAction, IMultipleAction, IContextInitializable
    {
        public bool IsSupportsEvent => false;
        
        public string Caption
        {
            get => string.Empty;
            set { }
        }
        
        public ActionHolder InputValue { get; set; } = new ActionHolder();

        public string TargetScenarioId { get; set; }
        
        public ValueTypeBase ValueType { get; set; }

        public void Initialize()
        {
            //do nothing
        }

        public IAction[] GetAllActionsFlat() => new[] { InputValue.Action };

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues) => true;

        public string GetValue(ExecutionContext context) => string.Empty;

        public bool IsSupportsModification => false;

        public void SetValue(ExecutionContext context, string value)
        {
            ValueType = context.AlgorithmContext.ValueType;
            var inputPrev = context.Input;
            context.Input = InputValue.Action.GetValue(context);
            context.PreviousValue = inputPrev;
            context.OutputChanged.Execute(context.Input);
        }

        public void Initialize(IAlgorithmContext algoContext)
        {
            ValueType = algoContext.ValueType;
        }

#pragma warning disable 67
        public event ValueChangedEventHandler ValueChanged;
#pragma warning restore 67
    }
}