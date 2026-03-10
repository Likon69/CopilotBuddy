using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Styx.CommonBot.Coroutines
{
    /// <summary>
    /// Port of HB 6.2.3 Styx.CommonBot.Coroutines.CoroutineTask{T}.
    /// Abstract base for hookable coroutine tasks.  The GetAwaiter()
    /// method returns a custom awaiter that wraps the Run() task,
    /// allowing these to be used with await from within ActionRunCoroutine.
    /// </summary>
    public abstract class CoroutineTask<T>
    {
        public abstract Task<T> Run();

        internal Task<T>? InternalTask { get; set; }
        internal TaskAwaiter<T> InternalAwaiter { get; set; }

        public CoroutineTaskAwaiter<T> GetAwaiter()
        {
            InternalTask = Run();
            InternalAwaiter = InternalTask.GetAwaiter();
            return new CoroutineTaskAwaiter<T>(this);
        }
    }
}
