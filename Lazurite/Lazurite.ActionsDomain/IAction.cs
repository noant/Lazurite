using Lazurite.ActionsDomain.ValueTypes;

namespace Lazurite.ActionsDomain
{
    public interface IAction
    {
        string GetValue(ExecutionContext context);
        void SetValue(ExecutionContext context, string value);
        string Caption { get; set; }
        ValueTypeBase ValueType { get; set; }
        void Initialize();
        bool UserInitializeWith(ValueTypeBase valueType, bool inheritsSupportedValues);
        event ValueChangedEventHandler ValueChanged;
        bool IsSupportsEvent { get; }
        bool IsSupportsModification { get; }
    }
}
