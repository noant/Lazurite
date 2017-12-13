using Lazurite.MainDomain;
using System;

namespace LazuriteUI.Windows.Main.Constructors
{
    public interface IScenarioConstructorView
    {
        void Revert(ScenarioBase scenario);
        event Action Modified;
        event Action Failed;
        event Action Succeed;
    }
}
