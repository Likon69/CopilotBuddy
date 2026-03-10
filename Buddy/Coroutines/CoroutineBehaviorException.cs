using System;

namespace Buddy.Coroutines
{
    /// <summary>
    /// Port of HB 6.2.3 Buddy.Coroutines.CoroutineBehaviorException.
    /// Thrown when a coroutine violates expected behavior patterns.
    /// </summary>
    public class CoroutineBehaviorException : CoroutineException
    {
        public CoroutineBehaviorException(string message) : base(message) { }
        public CoroutineBehaviorException(string message, Exception innerException) : base(message, innerException) { }
    }
}
