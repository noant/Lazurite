using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenZWrapper
{
    public class CallbacksPool
    {
        private const int OperationTimeout = 60;

        private Dictionary<string, BoolAction> _callbacks = new Dictionary<string, BoolAction>();

        bool _timeoutCountingIsStopped = true;
        private void BeginTimeoutCountingIfStopped()
        {
            if (_timeoutCountingIsStopped)
                Task.Factory.StartNew(() => {
                    while (_callbacks.Any())
                    {
                        foreach (var boolAction in _callbacks.ToArray())
                        {
                            if (DateTime.Now > boolAction.Value.Started + boolAction.Value.Timeout)
                                Dequeue(false, boolAction.Key);
                        }
                        Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                    }
                });
        }

        private bool CheckCurrent(string memberName)
        {
            return _callbacks.ContainsKey(memberName);
        }

        private void ThrowIfCalledEarlier(string memberName)
        {
            if (CheckCurrent(memberName))
                throw new Exception(string.Format("Method [{0}] was called earlier and has not yet been completed", memberName));
        }

        public void Add(Action<bool> callback, int timeout = OperationTimeout, [CallerMemberName] string memberName = "")
        {
            ThrowIfCalledEarlier(memberName);
            _callbacks.Add(memberName, new BoolAction(callback, TimeSpan.FromSeconds(timeout)));
            BeginTimeoutCountingIfStopped();
        }

        public void Dequeue(bool result, string memberName)
        {
            if (CheckCurrent(memberName))
            {
                var value = _callbacks[memberName];
                _callbacks.Remove(memberName);
                value.Action(result);
            }
        }

        public void Dequeue(bool result, params string[] membersNames)
        {
            foreach (var memberName in membersNames)
                Dequeue(result, memberName);
        }

        public void ExecuteBool(Func<bool> action, Action<bool> callback, int timeout = OperationTimeout, [CallerMemberName] string memberName="")
        {
            Add(callback, timeout, memberName);
            if (!action())
                Dequeue(false, memberName);
        }
    }

    public struct BoolAction
    {
        public BoolAction(Action<bool> action, TimeSpan timeout)
        {
            Action = action;
            Started = DateTime.Now;
            Timeout = timeout;
        }

        public Action<bool> Action;
        public DateTime Started;
        public TimeSpan Timeout;
    }
}
