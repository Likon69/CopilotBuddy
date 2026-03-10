namespace Buddy.Coroutines
{
    /// <summary>
    /// Port of HB 6.2.3 Buddy.Coroutines.CoroutineStatus.
    /// Tracks the state of a coroutine through its lifecycle.
    /// </summary>
    public enum CoroutineStatus
    {
        Runnable,
        RanToCompletion,
        Stopped,
        Faulted
    }
}
