using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TreeSharp
{
    public abstract class Composite : IEquatable<Composite>
    {
        protected static readonly object Locker = new object();
        
        public Guid Guid { get; private set; }
        public Composite? Parent { get; set; }
        public virtual IList<Composite> Children { get { return new List<Composite>(); } }
        public RunStatus? LastStatus { get; set; }
        protected ContextChangeHandler? ContextChanger { get; set; }
        protected Stack<CleanupHandler> CleanupHandlers { get; set; }

        private IEnumerator<RunStatus>? _enumerator;

        [DebuggerStepThrough]
        protected Composite()
        {
            Guid = Guid.NewGuid();
            CleanupHandlers = new Stack<CleanupHandler>();
        }

        public virtual RunStatus Tick(object context)
        {
            if (LastStatus.HasValue && LastStatus.Value != RunStatus.Running)
                return LastStatus.Value;

            if (_enumerator == null)
            {
                // Defensive: if Stop() was called between Start() and Tick(),
                // return Failure instead of throwing (prevents NRE spam on shutdown)
                LastStatus = RunStatus.Failure;
                return RunStatus.Failure;
            }

            try
            {
                if (!_enumerator.MoveNext())
                    throw new ApplicationException("Iterator completed unexpectedly - did Execute() yield all status values correctly?");

                LastStatus = _enumerator.Current;
            }
            catch (Exception ex)
            {
                Styx.Helpers.Logging.WriteException(ex);
                LastStatus = RunStatus.Failure;
                Stop(context);
                return LastStatus.Value;
            }

            if (LastStatus.Value != RunStatus.Running)
            {
                Stop(context);
            }

            return LastStatus ?? RunStatus.Failure;
        }

        public RunStatus Run(object context)
        {
            return Tick(context);
        }

        protected virtual IEnumerable<RunStatus> Execute(object context)
        {
            yield return RunStatus.Failure;
        }

        public virtual void Start(object context)
        {
            LastStatus = null;
            
            try
            {
                var executeResult = Execute(context);
                if (executeResult == null)
                    throw new ApplicationException($"Execute() returned null for {GetType().Name}");
                
                _enumerator = executeResult.GetEnumerator();
                
                if (_enumerator == null)
                    throw new ApplicationException($"GetEnumerator() returned null for {GetType().Name}");
            }
            catch (Exception ex)
            {
                Styx.Helpers.Logging.WriteException(ex);
                throw;
            }
        }

        public virtual void Stop(object context)
        {
            Cleanup();
            
            if (_enumerator != null)
            {
                _enumerator.Dispose();
                _enumerator = null;
            }
            
            if (LastStatus.HasValue && LastStatus.Value == RunStatus.Running)
            {
                LastStatus = RunStatus.Failure;
            }
        }

        [DebuggerStepThrough]
        protected void Cleanup()
        {
            if (CleanupHandlers.Count == 0)
                return;
                
            while (CleanupHandlers.Count != 0)
                CleanupHandlers.Pop().Dispose();
        }

        public bool Equals(Composite? other)
        {
            return other != null && Guid == other.Guid;
        }
        
        protected abstract class CleanupHandler : IDisposable
        {
            private bool _isDisposed;

            [DebuggerStepThrough]
            protected CleanupHandler(Composite owner, object context)
            {
                Owner = owner;
                Context = context;
            }

            public Composite Owner { get; private set; }
            public object Context { get; private set; }
            public bool IsDisposed => _isDisposed;

            [DebuggerStepThrough]
            public void Dispose()
            {
                if (IsDisposed)
                    return;
                    
                _isDisposed = true;
                DoCleanup(Context);
            }

            [DebuggerStepThrough]
            protected abstract void DoCleanup(object context);
        }
    }
}
