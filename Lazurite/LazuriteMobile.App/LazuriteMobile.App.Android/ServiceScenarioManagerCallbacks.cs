using System;
using System.Collections.Generic;
using System.Linq;

namespace LazuriteMobile.App.Droid
{
    public class ServiceScenarioManagerCallbacks
    {
        private List<Tuple<ServiceOperation, Action<object>>> _callbacks = new List<Tuple<ServiceOperation, Action<object>>>();

        public void Add(ServiceOperation operation, Action<object> action)
        {
            _callbacks.Add(new Tuple<ServiceOperation, Action<object>>(operation, action));
        }

        public void Dequeue(ServiceOperation operation, object parameter)
        {
            var targetCallbacks = _callbacks.Where(x => x.Item1.Equals(operation)).ToArray();
            targetCallbacks.All(x => _callbacks.Remove(x));
            foreach (var callback in targetCallbacks)
            {
                _callbacks.Remove(callback);
                callback.Item2.Invoke(parameter);
            }
        }
    }
}