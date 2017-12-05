using Lazurite.ActionsDomain;

namespace Lazurite.CoreActions
{
    public interface IChecker
    {
        bool Evaluate(ExecutionContext context);
    }
}
