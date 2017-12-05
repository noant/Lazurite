using Lazurite.ActionsDomain;

namespace Lazurite.CoreActions
{
    public class ActionHolder
    {
        public IAction Action { get; set; }

        public ActionHolder(IAction action)
        {
            Action = action;
        }

        public ActionHolder() : this(new EmptyAction()) { }
    }
}
