using Lazurite.ActionsDomain;
using Lazurite.ActionsDomain.Attributes;
using Lazurite.ActionsDomain.ValueTypes;

namespace Lazurite.CoreActions
{
    [OnlyExecute]
    [SuitableValueTypes(typeof(ButtonValueType))]
    [HumanFriendlyName("Остановить выполнение")]
    public class CancelExecutionAction : IAction
    {
        public string Caption { get => string.Empty; set { } }

        public ValueTypeBase ValueType { get; set; } = new ButtonValueType();

        public bool IsSupportsEvent => false;

        public bool IsSupportsModification => false;
#pragma warning disable 67
        public event ValueChangedEventHandler ValueChanged;
#pragma warning restore 67
        public string GetValue(ExecutionContext context) => string.Empty;

        public void Initialize()
        {
            //do nothing
        }

        public void SetValue(ExecutionContext context, string value)
        {
            context.CancellationTokenSource.Cancel();
        }

        public bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues) => true;
    }
}
