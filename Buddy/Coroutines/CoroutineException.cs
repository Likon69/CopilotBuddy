using System;

namespace Buddy.Coroutines
{
    /// <summary>
    /// Port of HB 6.2.3 Buddy.Coroutines.CoroutineException.
    /// Base class for all coroutine-related exceptions.
    /// </summary>
    public abstract class CoroutineException : Exception
    {
        protected CoroutineException(string message) : base(message) { }
        protected CoroutineException(string message, Exception innerException) : base(message, innerException) { }
    }
}
