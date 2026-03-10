using System;
using System.Runtime.CompilerServices;

namespace Styx.CommonBot.Coroutines
{
    /// <summary>
    /// Port of HB 6.2.3 Styx.CommonBot.Coroutines.CoroutineTaskAwaiter{T}.
    /// Custom awaiter that wraps a CoroutineTask's internal TaskAwaiter.
    /// </summary>
    public struct CoroutineTaskAwaiter<T> : INotifyCompletion
    {
        private readonly CoroutineTask<T> _task;

        internal CoroutineTaskAwaiter(CoroutineTask<T> task)
        {
            _task = task;
        }

        public bool IsCompleted => _task.InternalAwaiter.IsCompleted;

        public T GetResult() => _task.InternalAwaiter.GetResult();

        public void OnCompleted(Action continuation)
        {
            _task.InternalAwaiter.OnCompleted(continuation);
        }
    }
}
