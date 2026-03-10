using System;
using System.Threading.Tasks;
using CommonBehaviors.Actions;
using Styx.CommonBot.Coroutines;

namespace Styx.Common
{
    /// <summary>
    /// Port of HB 6.2.3 ns47.Class678.
    /// CoroutineTask&lt;bool&gt; that delegates to a <see cref="HookExecutor"/>,
    /// allowing plugins to inject behavior via <see cref="TreeHooks"/>.
    /// </summary>
    internal class HookCoroutineTask : CoroutineTask<bool>
    {
        private readonly HookExecutor _hookExecutor;

        public HookCoroutineTask(string location, string? description = null, Func<Task<bool>>? defaultCoroutine = null)
        {
            _hookExecutor = new HookExecutor(
                location,
                description,
                defaultCoroutine != null
                    ? new ActionRunCoroutine(ctx => defaultCoroutine())
                    : null);
        }

        public override Task<bool> Run()
        {
            return _hookExecutor.ExecuteCoroutine(null);
        }
    }
}
