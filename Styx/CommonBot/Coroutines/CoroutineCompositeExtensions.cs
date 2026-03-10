using System;
using System.Threading.Tasks;
using Buddy.Coroutines;
using CommonBehaviors.Actions;
using TreeSharp;

namespace Styx.CommonBot.Coroutines
{
    /// <summary>
    /// Port of HB 6.2.3 Styx.CommonBot.Coroutines.CoroutineCompositeExtensions.
    /// Allows running a TreeSharp Composite as an async coroutine.
    /// </summary>
    public static class CoroutineCompositeExtensions
    {
        /// <summary>
        /// HB 6.2.3 smethod_0: Runs a composite to completion as a coroutine.
        /// Yields between each tick.
        /// </summary>
        private static async Task<bool> RunCompositeAsync(Composite composite, object? context)
        {
            composite.Start(context);
            while (composite.Tick(context) == RunStatus.Running)
            {
                await Coroutine.Yield();
            }
            composite.Stop(context);
            return composite.LastStatus == RunStatus.Success;
        }

        /// <summary>
        /// HB 6.2.3: Extension method to execute any Composite as a coroutine.
        /// If the composite is an ActionRunCoroutine with a task producer,
        /// invokes the producer directly (avoids extra wrapping).
        /// </summary>
        public static Task<bool> ExecuteCoroutine(this Composite composite, object? context = null)
        {
            if (composite == null)
                throw new ArgumentNullException(nameof(composite));

            if (composite is ActionRunCoroutine arc && arc._taskProducer != null)
            {
                return arc._taskProducer(context);
            }

            return RunCompositeAsync(composite, context);
        }
    }
}
