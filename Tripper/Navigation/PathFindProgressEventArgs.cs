using System;

namespace Tripper.Navigation
{
    // HB 6.2.3 Tripper/Navigation/PathFindProgressEventArgs.cs
    public class PathFindProgressEventArgs : EventArgs
    {
        internal PathFindProgressEventArgs(TimeSpan runTime)
        {
            RunTime = runTime;
        }

        public DateTime PathFindStart => DateTime.Now - RunTime;

        public TimeSpan RunTime { get; private set; }

        public bool Cancel { get; set; }
    }
}
