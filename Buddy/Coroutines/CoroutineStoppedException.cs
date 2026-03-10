using System;

namespace Buddy.Coroutines
{
    /// <summary>
    /// Port of HB 6.2.3 Buddy.Coroutines.CoroutineStoppedException.
    /// Thrown when a coroutine is stopped via Dispose().
    /// </summary>
    public class CoroutineStoppedException : Exception
    {
        internal CoroutineStoppedException(string message) : base(message) { }
    }
}
