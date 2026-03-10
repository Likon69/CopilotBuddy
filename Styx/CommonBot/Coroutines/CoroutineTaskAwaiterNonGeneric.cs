using System;
using System.Runtime.CompilerServices;

namespace Styx.CommonBot.Coroutines
{
    /// <summary>
    /// Port of HB 6.2.3 Styx.CommonBot.Coroutines.CoroutineTaskAwaiter (non-generic).
    /// </summary>
    public struct CoroutineTaskAwaiter : INotifyCompletion
    {
        private readonly CoroutineTask _task;

        internal CoroutineTaskAwaiter(CoroutineTask task)
        {
            _task = task;
        }

        public bool IsCompleted => _task.InternalAwaiter.IsCompleted;

        public void GetResult() => _task.InternalAwaiter.GetResult();

        public void OnCompleted(Action continuation)
        {
            _task.InternalAwaiter.OnCompleted(continuation);
        }
    }
}
