using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenZWrapper
{
    public class CallbacksPool
    {
        private Dictionary<string, Action<bool>> _callbacks = new Dictionary<string, Action<bool>>();

        private bool CheckCurrent(string memberName)
        {
            return _callbacks.ContainsKey(memberName);
        }

        private void ThrowIfCalledEarlier(string memberName)
        {
            if (CheckCurrent(memberName))
                throw new Exception(string.Format("Method [{0}] was called earlier and has not yet been completed", memberName));
        }

        public void Add(Action<bool> callback, [CallerMemberName] string memberName = "")
        {
            ThrowIfCalledEarlier(memberName);
            _callbacks.Add(memberName, callback);
        }

        public void Dequeue(bool result, string memberName)
        {
            if (CheckCurrent(memberName))
            {
                var action = _callbacks[memberName];
                _callbacks.Remove(memberName);
                action(result);
            }
        }

        public void Dequeue(bool result, params string[] membersNames)
        {
            foreach (var memberName in membersNames)
                Dequeue(result, memberName);
        }

        public void ExecuteBool(Func<bool> action, Action<bool> callback, [CallerMemberName] string memberName="")
        {
            Add(callback, memberName);
            if (!action())
                Dequeue(false, memberName);
        }
    }
}
