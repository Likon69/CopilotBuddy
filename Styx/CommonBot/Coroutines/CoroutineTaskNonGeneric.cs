using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Styx.CommonBot.Coroutines
{
    /// <summary>
    /// Port of HB 6.2.3 Styx.CommonBot.Coroutines.CoroutineTask (non-generic).
    /// </summary>
    public abstract class CoroutineTask
    {
        public abstract Task Run();

        internal Task? InternalTask { get; set; }
        internal TaskAwaiter InternalAwaiter { get; set; }

        public CoroutineTaskAwaiter GetAwaiter()
        {
            InternalTask = Run();
            InternalAwaiter = InternalTask.GetAwaiter();
            return new CoroutineTaskAwaiter(this);
        }
    }
}
