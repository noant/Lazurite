using Lazurite.ActionsDomain.ValueTypes;

namespace Lazurite.ActionsDomain
{
    public interface IAlgorithmContext
    {
        ValueTypeBase ValueType { get; }
    }
}
