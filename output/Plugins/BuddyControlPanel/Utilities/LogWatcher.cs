// Originally contributed by Chinajade
//
// LICENSE:
// This work is licensed under the
//     Creative Commons Attribution-NonCommercial-ShareAlike 4.0 Unported License.
// also known as CC-BY-NC-SA.  To view a copy of this license, visit
//      http://creativecommons.org/licenses/by-nc-sa/4.0/
// or send a letter to
//      Creative Commons // 171 Second Street, Suite 300 // San Francisco, California, 94105, USA.
//

#region Usings

using System;
using System.Collections.ObjectModel;
using Styx.Common;
using Styx.Helpers;
#endregion


namespace BuddyControlPanel
{
    public class LogWatcher : IDisposable
    {
        /// <summary>
        /// The LogWatcher examines the log messages produced by Honorbuddy, and reflects them to Overlay in the form
        /// of "Toast" messages.  Once an expected FINALPHRASE is seen, the LogWatcher terminates.
        /// The FINALPHRASE is included in the messages reflected to the Overlay.
        /// The LogWatcher was meant to be used in a "fire and forget" capacity.
        /// </summary>
        /// <param name="finalPhrase">The string from the log that determines 
        /// when the LogWatcher's job is complete.</param>
        /// <param name="toastDuration">The amount of time that the log messages should remain on the
        /// screen.  If omitted, this value defaults to 5000 milliseconds.</param>
        public LogWatcher(string finalPhrase, TimeSpan? toastDuration = null)
        {
            _finalPhrase = finalPhrase;
            _lastMessageTimeSeen = DateTime.MinValue;
            _toastDuration = toastDuration ?? TimeSpan.FromMilliseconds(5000);

            Logging.OnLogMessage += Logging_OnLogMessage;
        }

        private readonly string _finalPhrase;
        private DateTime _lastMessageTimeSeen;
        private readonly TimeSpan _toastDuration;


        // Basic Dispose pattern (ref: https://msdn.microsoft.com/en-us/library/b1yfkh5e%28v=vs.110%29.aspx)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Logging.OnLogMessage -= Logging_OnLogMessage;
            }
        }


        private void Logging_OnLogMessage(ReadOnlyCollection<Logging.LogMessage> messages)
        {
            foreach (var message in messages)
            {
                if (message.Timestamp < _lastMessageTimeSeen)
                    continue;

                if (message.Level > LogLevel.Normal)
                    continue;

                // Capture message that needs to be emitted, if an bot launch error occurs...
                Utility.OverlayNotification(message.Message, message.Color, _toastDuration);
                _lastMessageTimeSeen = message.Timestamp;

                // If the final phrase is seen, then unhook the event handler...
                if (message.Message == _finalPhrase)
                    Logging.OnLogMessage -= Logging_OnLogMessage;
            }
        }
    }
}