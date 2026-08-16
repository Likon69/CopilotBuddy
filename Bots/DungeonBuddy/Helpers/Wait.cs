using System;
using TreeSharp;

namespace Bots.DungeonBuddy.Helpers
{
    public class Wait<T> : TreeSharp.Wait
    {
        private readonly Predicate<T> _canRun;

        public Wait(int timeoutSeconds, Predicate<T> canRun, Composite child)
            : base(timeoutSeconds, child)
        {
            _canRun = canRun;
        }

        public Wait(TimeSpan timeout, Predicate<T> canRun, Composite child)
            : base(timeout, child)
        {
            _canRun = canRun;
        }

        public Wait(WaitGetTimeoutDelegate timeoutRetriever, Predicate<T> canRun, Composite child)
            : base(timeoutRetriever, child)
        {
            _canRun = canRun;
        }

        protected override bool CanRun(object context)
        {
            return context is T typedContext && _canRun(typedContext);
        }
    }
}
