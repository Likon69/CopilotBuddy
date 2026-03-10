using System;

namespace Buddy.Coroutines
{
    /// <summary>
    /// Port of HB 6.2.3 Buddy.Coroutines.CoroutineUnhandledException.
    /// Wraps unhandled exceptions thrown inside a coroutine.
    /// </summary>
    public class CoroutineUnhandledException : CoroutineException
    {
        public CoroutineUnhandledException(string message) : base(message) { }
        public CoroutineUnhandledException(string message, Exception innerException) : base(message, innerException) { }
    }
}
