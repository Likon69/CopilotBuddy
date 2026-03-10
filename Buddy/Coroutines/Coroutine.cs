using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Buddy.Coroutines
{
    /// <summary>
    /// Port of HB 6.2.3 Buddy.Coroutines.Coroutine.
    /// Cooperative multitasking engine that bridges async/await with
    /// TreeSharp's synchronous tick loop.  Each call to Resume() advances
    /// the coroutine by one step; the coroutine yields back by awaiting
    /// Coroutine.Yield() or Coroutine.Sleep().
    /// </summary>
    public sealed class Coroutine : IDisposable
    {
        [ThreadStatic] private static Coroutine? _current;

        private readonly CoroutineYieldController _yieldController;
        private Action? _continuation;
        private Task<object>? _task;

        public static Coroutine? Current => _current;
        public object? Result { get; private set; }
        public CoroutineStatus Status { get; private set; }
        public bool IsDisposed { get; private set; }
        public CoroutineException? FaultingException { get; private set; }
        public int Ticks { get; private set; }

        public bool IsFinished =>
            Status == CoroutineStatus.RanToCompletion ||
            Status == CoroutineStatus.Faulted ||
            Status == CoroutineStatus.Stopped;

        public Coroutine(Func<Task> taskProducer)
            : this(WrapVoidTask(taskProducer))
        {
        }

        public Coroutine(Func<Task<object>> taskProducer)
        {
            if (taskProducer == null)
                throw new ArgumentNullException(nameof(taskProducer));

            _yieldController = new CoroutineYieldController();
            _continuation = () =>
            {
                try
                {
                    _task = taskProducer();
                }
                catch (Exception ex)
                {
                    throw SetFaulted(new CoroutineUnhandledException("Exception was thrown by root coroutine task producer", ex));
                }
                if (_task == null)
                {
                    throw SetFaulted(new CoroutineBehaviorException("The root coroutine task producer returned null"));
                }
            };
            Status = CoroutineStatus.Runnable;
        }

        private static Func<Task<object>> WrapVoidTask(Func<Task> producer)
        {
            return async () =>
            {
                await producer();
                return null!;
            };
        }

        /// <summary>
        /// Advance the coroutine by one tick.  The coroutine runs until it
        /// awaits a yield point, then returns control to the caller.
        /// </summary>
        public void Resume()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);
            if (IsFinished)
                throw new InvalidOperationException("This coroutine has finished execution and cannot be resumed.");
            if (_current == this)
                throw new InvalidOperationException("A coroutine cannot resume itself");

            RunOneTick(false);
        }

        private void RunOneTick(bool disposing)
        {
            var saved = SynchronizationContext.Current;
            var savedCoroutine = _current;
            try
            {
                _current = this;
                SynchronizationContext.SetSynchronizationContext(null);

                var action = _continuation;
                _continuation = null;
                action();

                if (!disposing)
                    Ticks++;

                CheckTaskStatus(disposing);
            }
            finally
            {
                _current = savedCoroutine;
                SynchronizationContext.SetSynchronizationContext(saved);
            }
        }

        private Exception SetFaulted(CoroutineException ex)
        {
            Status = CoroutineStatus.Faulted;
            FaultingException = ex;
            return ex;
        }

        private void CheckTaskStatus(bool disposing)
        {
            switch (_task!.Status)
            {
                case TaskStatus.WaitingForActivation:
                    if (disposing)
                        throw SetFaulted(new CoroutineBehaviorException(
                            "The coroutine could not successfully be disposed of. " +
                            "This is usually an indication that the CoroutineStoppedException was caught."));
                    if (_continuation == null)
                        throw SetFaulted(new CoroutineBehaviorException(
                            "No continuation was queued and coroutine didn't finish. " +
                            "This is usually an indication that an external task was awaited, " +
                            "which is not supported by coroutines."));
                    break;

                case TaskStatus.RanToCompletion:
                    if (_continuation != null)
                        throw SetFaulted(new CoroutineBehaviorException(
                            "The coroutine finished with a continuation queued. " +
                            "This is usually an indication that a task was created but not awaited."));
                    Status = CoroutineStatus.RanToCompletion;
                    Result = _task.Result;
                    break;

                case TaskStatus.Canceled:
                    try
                    {
                        _task.Wait(0);
                        throw SetFaulted(new CoroutineBehaviorException("Coroutine was canceled without any exceptions"));
                    }
                    catch (Exception ex)
                    {
                        throw SetFaulted(new CoroutineUnhandledException("Exception was thrown by coroutine", ex));
                    }

                case TaskStatus.Faulted:
                    var inner = _task.Exception!.InnerExceptions.FirstOrDefault();
                    if (inner is CoroutineStoppedException)
                    {
                        Status = CoroutineStatus.Stopped;
                    }
                    else
                    {
                        throw SetFaulted(new CoroutineUnhandledException("Exception was thrown by coroutine", inner!));
                    }
                    break;

                default:
                    throw SetFaulted(new CoroutineBehaviorException("Unexpected task status " + _task.Status));
            }
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            if (_current == this)
            {
                IsDisposed = true;
                throw CreateStoppedException();
            }

            if (Status == CoroutineStatus.Runnable)
            {
                if (_task != null)
                {
                    _yieldController.RequestStop = true;
                    RunOneTick(true);
                }
                else
                {
                    _continuation = null;
                }
            }

            IsDisposed = true;
        }

        public override string ToString()
        {
            switch (Status)
            {
                case CoroutineStatus.Runnable:
                    return "Runnable";
                case CoroutineStatus.RanToCompletion:
                    return Result != null ? $"Ran to completion. Result: {Result}" : "Ran to completion";
                case CoroutineStatus.Stopped:
                    return "Stopped";
                case CoroutineStatus.Faulted:
                    return FaultingException != null ? $"Faulted with exception {FaultingException}" : "Faulted";
                default:
                    return "Invalid state";
            }
        }

        internal static Exception CreateStoppedException()
        {
            return new CoroutineStoppedException("Coroutine was stopped");
        }

        // ── yield infrastructure ──

        private static void ValidateYieldContext()
        {
            var co = _current;
            if (co == null)
                throw new InvalidOperationException("This function can only be used from within a coroutine");
            if (co._continuation != null)
                throw new InvalidOperationException(
                    "Cannot obtain multiple coroutine tasks in a single coroutine tick. " +
                    "This is usually an indication that a coroutine task was created but not awaited.");
        }

        public static Task Yield()
        {
            ValidateYieldContext();
            return YieldInternal();
        }

        private static async Task YieldInternal()
        {
            var yc = Current!._yieldController;
            await yc;
            yc.ThrowIfStopping();
        }

        public static Task Sleep(TimeSpan timeout)
        {
            if (timeout < TimeSpan.Zero && timeout != Timeout.InfiniteTimeSpan)
                throw new ArgumentOutOfRangeException(nameof(timeout));
            ValidateYieldContext();
            return SleepInternal(timeout);
        }

        public static Task Sleep(int milliseconds)
        {
            if (milliseconds < 0 && milliseconds != -1)
                throw new ArgumentOutOfRangeException(nameof(milliseconds));
            return Sleep(new TimeSpan(0, 0, 0, 0, milliseconds));
        }

        private static async Task SleepInternal(TimeSpan timeout)
        {
            if (timeout == Timeout.InfiniteTimeSpan)
            {
                while (true)
                {
                    var yc = Current!._yieldController;
                    await yc;
                    yc.ThrowIfStopping();
                }
            }
            else
            {
                var sw = Stopwatch.StartNew();
                do
                {
                    var yc = Current!._yieldController;
                    await yc;
                    yc.ThrowIfStopping();
                }
                while (sw.Elapsed < timeout);
            }
        }

        public static Task<bool> Wait(TimeSpan maxWaitTimeout, Func<bool> condition)
        {
            if (maxWaitTimeout < TimeSpan.Zero && maxWaitTimeout != Timeout.InfiniteTimeSpan)
                throw new ArgumentOutOfRangeException(nameof(maxWaitTimeout), "Cannot wait for a negative timespan");
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            ValidateYieldContext();
            return WaitInternal(maxWaitTimeout, condition);
        }

        public static Task<bool> Wait(int maxWaitMs, Func<bool> condition)
        {
            if (maxWaitMs < 0 && maxWaitMs != -1)
                throw new ArgumentOutOfRangeException(nameof(maxWaitMs), "Cannot wait for a negative amount of time");
            return Wait(new TimeSpan(0, 0, 0, 0, maxWaitMs), condition);
        }

        private static async Task<bool> WaitInternal(TimeSpan timeout, Func<bool> condition)
        {
            if (timeout == TimeSpan.Zero)
                return condition();

            if (timeout == Timeout.InfiniteTimeSpan)
            {
                while (!condition())
                {
                    var yc = Current!._yieldController;
                    await yc;
                    yc.ThrowIfStopping();
                }
                return true;
            }

            var sw = Stopwatch.StartNew();
            while (!condition())
            {
                var yc = Current!._yieldController;
                await yc;
                yc.ThrowIfStopping();
                if (sw.Elapsed >= timeout)
                    return false;
            }
            return true;
        }

        // ── ExternalTask infrastructure ──

        public static Task ExternalTask(Func<Task> taskProducer)
        {
            if (taskProducer == null)
                throw new ArgumentNullException(nameof(taskProducer));
            Task task = taskProducer();
            if (task == null)
                throw new ArgumentException("The task producer returned null", nameof(taskProducer));
            return ExternalTask(task);
        }

        public static Task ExternalTask(Task externalTask)
        {
            return ExternalTaskInternal(externalTask, Timeout.InfiniteTimeSpan);
        }

        [Obsolete("Timeouts should be handled in the external task, not by this method. Use the overloads with infinite timeouts instead.")]
        public static Task<bool> ExternalTask(Task externalTask, TimeSpan timeout)
        {
            return ExternalTaskInternal(externalTask, timeout);
        }

        [Obsolete("Timeouts should be handled in the external task, not by this method. Use the overloads with infinite timeouts instead.")]
        public static Task<bool> ExternalTask(Task externalTask, int millisecondsTimeout)
        {
            if (millisecondsTimeout != -1 && millisecondsTimeout < 0)
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout), "Timeout cannot be negative");
            return ExternalTaskInternal(externalTask, new TimeSpan(0, 0, 0, 0, millisecondsTimeout));
        }

        private static Task<bool> ExternalTaskInternal(Task externalTask, TimeSpan timeout)
        {
            if (externalTask == null)
                throw new ArgumentNullException(nameof(externalTask));
            if (timeout < TimeSpan.Zero && timeout != Timeout.InfiniteTimeSpan)
                throw new ArgumentOutOfRangeException(nameof(timeout));
            ValidateYieldContext();
            return ExternalTaskInternalAsync(externalTask, timeout);
        }

        private static async Task<bool> ExternalTaskInternalAsync(Task externalTask, TimeSpan timeout)
        {
            if (timeout == TimeSpan.Zero)
            {
                return externalTask.Wait(0);
            }

            if (timeout == Timeout.InfiniteTimeSpan)
            {
                while (!externalTask.Wait(0))
                {
                    var yc = Current!._yieldController;
                    await yc;
                    yc.ThrowIfStopping();
                }
                return true;
            }

            var sw = Stopwatch.StartNew();
            while (!externalTask.Wait(0))
            {
                var yc = Current!._yieldController;
                await yc;
                yc.ThrowIfStopping();
                if (sw.Elapsed >= timeout)
                    return false;
            }
            return true;
        }

        public static Task<T> ExternalTask<T>(Func<Task<T>> taskProducer)
        {
            if (taskProducer == null)
                throw new ArgumentNullException(nameof(taskProducer));
            Task<T> task = taskProducer();
            if (task == null)
                throw new ArgumentException("The task producer returned null", nameof(taskProducer));
            return ExternalTask(task);
        }

        public static Task<T> ExternalTask<T>(Task<T> externalTask)
        {
            return UnwrapExternalTask(ExternalTaskInternal(externalTask, Timeout.InfiniteTimeSpan));
        }

        [Obsolete("Timeouts should be handled in the external task, not by this method. Use the overloads with infinite timeouts instead.")]
        public static Task<ExternalTaskWaitResult<T>> ExternalTask<T>(Task<T> externalTask, int millisecondsTimeout)
        {
            if (millisecondsTimeout < 0 && millisecondsTimeout != -1)
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout), "Timeout cannot be negative");
            return ExternalTaskInternal(externalTask, new TimeSpan(0, 0, 0, 0, millisecondsTimeout));
        }

        [Obsolete("Timeouts should be handled in the external task, not by this method. Use the overloads with infinite timeouts instead.")]
        public static Task<ExternalTaskWaitResult<T>> ExternalTask<T>(Task<T> externalTask, TimeSpan timeout)
        {
            return ExternalTaskInternal(externalTask, timeout);
        }

        private static Task<ExternalTaskWaitResult<T>> ExternalTaskInternal<T>(Task<T> externalTask, TimeSpan timeout)
        {
            if (externalTask == null)
                throw new ArgumentNullException(nameof(externalTask));
            if (timeout < TimeSpan.Zero && timeout != Timeout.InfiniteTimeSpan)
                throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout cannot be negative");
            ValidateYieldContext();
            return ExternalTaskInternalAsync(externalTask, timeout);
        }

        private static async Task<ExternalTaskWaitResult<T>> ExternalTaskInternalAsync<T>(Task<T> externalTask, TimeSpan timeout)
        {
            if (await ExternalTaskInternalAsync((Task)externalTask, timeout))
            {
                return ExternalTaskWaitResult<T>.Create(externalTask.Result);
            }
            return ExternalTaskWaitResult<T>.TimedOut;
        }

        private static async Task<T> UnwrapExternalTask<T>(Task<ExternalTaskWaitResult<T>> task)
        {
            return (await task).Result;
        }

        // ── Inner class: merged yield controller + awaiter (HB 6.2.3 Class224) ──

        internal sealed class CoroutineYieldController : INotifyCompletion
        {
            internal bool RequestStop;

            public bool IsCompleted => false;

            public CoroutineYieldController GetAwaiter()
            {
                return this;
            }

            public void GetResult() { }

            public void OnCompleted(Action continuation)
            {
                Coroutine._current!._continuation = continuation;
            }

            internal void ThrowIfStopping()
            {
                if (RequestStop)
                    throw CreateStoppedException();
            }
        }
    }
}
