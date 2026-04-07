using System;

namespace Styx
{
    /// <summary>
    /// Thrown when the selected WoW process cannot be attached to.
    /// Ported from HB 4.3.4 InvalidProcessException.
    /// </summary>
    public class InvalidProcessException : Exception
    {
        public InvalidProcessException()
        {
        }

        public InvalidProcessException(string message)
            : base(message)
        {
        }

        public InvalidProcessException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
