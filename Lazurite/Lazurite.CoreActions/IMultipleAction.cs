using Lazurite.ActionsDomain;

namespace Lazurite.CoreActions
{
    public interface IMultipleAction
    {
        IAction[] GetAllActionsFlat();
    }
}
