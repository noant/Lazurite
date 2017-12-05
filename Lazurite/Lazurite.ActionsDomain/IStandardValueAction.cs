using Lazurite.ActionsDomain.ValueTypes;

namespace Lazurite.ActionsDomain
{
    public interface IStandardValueAction
    {
        string Value { get; set; }
        ValueTypeBase ValueType { get; set; }
    }
}
