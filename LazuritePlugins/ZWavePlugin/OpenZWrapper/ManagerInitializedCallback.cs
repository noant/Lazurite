using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenZWrapper
{
    public class ManagerInitializedCallback
    {
        public bool RemoveAfterInvoke { get; set; }
        public Action<object, ManagerInitializedEventArgs> Callback { get;set; }
    }
}
