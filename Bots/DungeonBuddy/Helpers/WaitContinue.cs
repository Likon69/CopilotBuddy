using System;
using TreeSharp;

namespace Bots.DungeonBuddy.Helpers
{
    public class WaitContinue<T> : TreeSharp.WaitContinue
    {
        private readonly Predicate<T> _canRun;

        public WaitContinue(int timeoutSeconds, Predicate<T> canRun, Composite child)
            : base(timeoutSeconds, child)
        {
            _canRun = canRun;
        }

        public WaitContinue(TimeSpan timeout, Predicate<T> canRun, Composite child)
            : base(timeout, child)
        {
            _canRun = canRun;
        }

        public WaitContinue(WaitGetTimeoutDelegate timeoutRetriever, Predicate<T> canRun, Composite child)
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
