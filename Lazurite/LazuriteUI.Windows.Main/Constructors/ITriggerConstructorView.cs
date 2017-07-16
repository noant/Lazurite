using Lazurite.MainDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazuriteUI.Windows.Main.Constructors
{
    public interface ITriggerConstructorView
    {
        void Revert(TriggerBase scenario);
        event Action Modified;
        event Action Failed;
        event Action Succeed;
    }
}
