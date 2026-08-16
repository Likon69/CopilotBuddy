using System;
using TreeSharp;

namespace Bots.DungeonBuddy.Helpers
{
    public class DecoratorContinue<T> : TreeSharp.DecoratorContinue
    {
        private readonly Predicate<T> _canRun;

        public DecoratorContinue(Predicate<T> canRun, Composite decorated)
            : base(decorated)
        {
            _canRun = canRun;
        }

        protected override bool CanRun(object context)
        {
            return context is T typedContext && _canRun(typedContext);
        }
    }
}
