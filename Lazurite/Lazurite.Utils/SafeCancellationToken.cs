using System;
using System.Collections.Generic;

namespace Lazurite.Utils
{
    /// <summary>
    /// Use SafeCancellationToken instead of CancellationTokenSource to prevent memory leaks
    /// </summary>
    public class SafeCancellationToken
    {
        public static readonly SafeCancellationToken None = new SafeCancellationToken() { IsStub = true };

        private volatile bool _cancelled;

        private List<Action> _cancellationCallbacks = new List<Action>();

        protected bool IsStub { get; set; }

        /// <summary>
        /// Cancel method was executed
        /// </summary>
        public bool IsCancellationRequested => _cancelled;

        /// <summary>
        /// Register action to run it after cancel operation
        /// </summary>
        /// <param name="action"></param>
        public void RegisterCallback(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (_cancelled)
            {
                throw new InvalidOperationException("Already cancelled");
            }

            _cancellationCallbacks.Add(action);
        }

        /// <summary>
        /// Cancel operation
        /// </summary>
        public void Cancel()
        {
            if (IsStub)
            {
                throw new InvalidOperationException("Token is SafeCancellationToken.None");
            }

            if (!_cancelled)
            {
                _cancelled = true;
                foreach (var callback in _cancellationCallbacks)
                {
                    callback();
                }
                _cancellationCallbacks = null;
            }
            else
            {
                throw new InvalidOperationException("Already cancelled");
            }
        }
    }
}