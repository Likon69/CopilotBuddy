using System;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Styx.CommonBot.Coroutines;
using TreeSharp;

namespace CommonBehaviors.Actions
{
    /// <summary>
    /// Port of HB 6.2.3 CommonBehaviors.Actions.ActionRunCoroutine.
    /// Bridges async Task-based coroutines with TreeSharp's synchronous
    /// behavior tree.  Each Tick() calls Coroutine.Resume() which advances
    /// the async method by one step.
    /// </summary>
    public class ActionRunCoroutine : TreeSharp.Action
    {
        private readonly Func<object?, Coroutine> _coroutineProducer;
        internal readonly Func<object?, Task<bool>>? _taskProducer;
        private Coroutine? _coroutine;

        public ActionRunCoroutine(Func<object?, Coroutine> coroutineProducer)
        {
            _coroutineProducer = coroutineProducer ?? throw new ArgumentNullException(nameof(coroutineProducer));
        }

        public ActionRunCoroutine(Func<object?, Task<bool>> taskProducer)
            : this(WrapTaskProducer(taskProducer))
        {
            _taskProducer = taskProducer;
        }

        public ActionRunCoroutine(Func<object?, CoroutineTask<bool>> taskProducer)
            : this((Func<object?, Task<bool>>)(ctx => taskProducer(ctx).Run()))
        {
        }

        public ActionRunCoroutine(Func<object?, Task> taskProducer)
            : this(WrapVoidTaskProducer(taskProducer))
        {
        }

        public ActionRunCoroutine(Func<object?, CoroutineTask> taskProducer)
            : this((Func<object?, Task>)(ctx => taskProducer(ctx).Run()))
        {
        }

        private static Func<object?, Coroutine> WrapTaskProducer(Func<object?, Task<bool>> taskProducer)
        {
            return ctx => new Coroutine(async () =>
            {
                object result = await taskProducer(ctx);
                return result;
            });
        }

        private static Func<object?, Coroutine> WrapVoidTaskProducer(Func<object?, Task> taskProducer)
        {
            return ctx => new Coroutine(() => taskProducer(ctx));
        }

        private void DisposeCoroutine()
        {
            if (_coroutine == null)
                return;
            _coroutine.Dispose();
            _coroutine = null;
        }

        public override void Start(object? context)
        {
            base.Start(context);
            DisposeCoroutine();
            _coroutine = _coroutineProducer(context);
        }

        public override void Stop(object? context)
        {
            DisposeCoroutine();
            base.Stop(context);
        }

        protected override RunStatus Run(object? context)
        {
            _coroutine!.Resume();

            switch (_coroutine.Status)
            {
                case CoroutineStatus.Runnable:
                    return RunStatus.Running;

                case CoroutineStatus.RanToCompletion:
                    if (_coroutine.Result is bool b && !b)
                        return RunStatus.Failure;
                    return RunStatus.Success;

                case CoroutineStatus.Stopped:
                case CoroutineStatus.Faulted:
                    return RunStatus.Failure;

                default:
                    throw new Exception("Coroutine status was invalid");
            }
        }
    }
}
