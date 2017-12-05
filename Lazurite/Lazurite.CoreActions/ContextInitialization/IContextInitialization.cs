using Lazurite.ActionsDomain;

namespace Lazurite.CoreActions.ContextInitialization
{
    public interface IContextInitializable
    {
        void Initialize(IAlgorithmContext parameters);
    }
}
