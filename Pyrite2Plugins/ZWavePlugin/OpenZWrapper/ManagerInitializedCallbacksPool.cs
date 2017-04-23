using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenZWrapper
{
    public class ManagerInitializedCallbacksPool
    {
        private List<ManagerInitializedCallback> _callbacks = new List<ManagerInitializedCallback>();

        public void Add(ManagerInitializedCallback callback)
        {
            _callbacks.Add(callback);
        }

        public void ExecuteAll(ZWaveManager manager)
        {
            for (int i=0; i<_callbacks.Count; i++)
            {
                var handler = _callbacks[i];
                handler.Callback(manager, new ManagerInitializedEventArgs() { Manager = manager });
                if (handler.RemoveAfterInvoke)
                    _callbacks.Remove(handler);
            }
        }
    }
}
