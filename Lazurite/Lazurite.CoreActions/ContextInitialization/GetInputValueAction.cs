using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;
using Lazurite.Shared.ActionCategory;

namespace Lazurite.CoreActions.ContextInitialization
{
    [OnlyGetValue]
    [SuitableValueTypes(true)]
    [HumanFriendlyName("Входящее значение")]
    [VisualInitialization]
    [Category(Category.Meta)]
    public class GetInputValueAction : IAction, IContextInitializable
    {
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
                return false;
            }
        }

        public ValueTypeBase ValueType
        {
            get;
            set;
        }
        
        public string GetValue(ExecutionContext context)
        {
            ValueType = context.AlgorithmContext.ValueType;
            return context.Input;
        }
        
        public void SetValue(ExecutionContext context, string value)
        {
            //
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues)
        {
            return true;
        }

        public void Initialize()
        {
            //
        }

        public void Initialize(IAlgorithmContext algoContext)
        {
            ValueType = algoContext.ValueType;
        }

        public event ValueChangedEventHandler ValueChanged;
    }
}
