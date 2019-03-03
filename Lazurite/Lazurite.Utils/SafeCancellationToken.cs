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
        /// Exceptions throw mode
        /// </summary>
        public CancellationTokenExceptionThrowMode ExceptionThrowMode { get; set; } = CancellationTokenExceptionThrowMode.None;

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

            if ((ExceptionThrowMode == CancellationTokenExceptionThrowMode.OnCancelInvokeWhenAlreadyCancelled ||
                ExceptionThrowMode == CancellationTokenExceptionThrowMode.Both) &&
                _cancelled)
            {
                throw new InvalidOperationException("Already cancelled");
            }
            else if (!_cancelled)
            {
                _cancelled = true;
                foreach (var callback in _cancellationCallbacks)
                {
                    callback();
                }
                _cancellationCallbacks = null;

                if (ExceptionThrowMode == CancellationTokenExceptionThrowMode.OnCancelInvoke ||
                    ExceptionThrowMode == CancellationTokenExceptionThrowMode.Both)
                {
                    throw new InvalidOperationException("Cancelled");
                }
            }
        }
    }

    public enum CancellationTokenExceptionThrowMode : byte
    {
        None = 0,
        OnCancelInvoke = 1,
        OnCancelInvokeWhenAlreadyCancelled = 2,
        Both = 3
    }
}