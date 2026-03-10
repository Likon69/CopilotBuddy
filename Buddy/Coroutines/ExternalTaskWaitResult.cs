using System;

namespace Buddy.Coroutines
{
    /// <summary>
    /// Result of an external task wait operation that may have timed out.
    /// Port of HB 6.2.3 Buddy.Coroutines.ExternalTaskWaitResult&lt;T&gt;.
    /// </summary>
    public struct ExternalTaskWaitResult<T>
    {
        private ExternalTaskWaitResult(T result)
        {
            Completed = true;
            Result = result;
        }

        public bool Completed { get; }

        public T Result { get; }

        internal static ExternalTaskWaitResult<T> Create(T result)
        {
            return new ExternalTaskWaitResult<T>(result);
        }

        internal static readonly ExternalTaskWaitResult<T> TimedOut;
    }
}
