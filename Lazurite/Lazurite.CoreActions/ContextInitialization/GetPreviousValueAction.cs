using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared.ActionCategory;

namespace Lazurite.CoreActions.ContextInitialization
{
    [OnlyGetValue]
    [SuitableValueTypes(true)]
    [HumanFriendlyName("Предыдущее значение")]
    [VisualInitialization]
    [Category(Category.Meta)]
    public class GetPreviousValueAction : IAction, IContextInitializable
    {
        public string Caption
        {
            get => string.Empty;
            set { }
        }

        public bool IsSupportsEvent => false;

        public bool IsSupportsModification => false;

        public ValueTypeBase ValueType { get; set; }

        public string GetValue(ExecutionContext context)
        {
            ValueType = context.AlgorithmContext.ValueType;
            return context.PreviousValue;
        }

        public void SetValue(ExecutionContext context, string value)
        {
            //
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues) => true;

        public void Initialize()
        {
            //
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
