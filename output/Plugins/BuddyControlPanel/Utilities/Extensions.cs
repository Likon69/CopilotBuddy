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

using System.Windows;

using Point = System.Windows.Point;

// ReSharper disable InconsistentNaming
#endregion


namespace BuddyControlPanel
{
    // By its nature, only static elements belong in the Utility class...
    public static class Extensions
    {
        /// <summary>
        /// Shows a WINDOW at an optional caller-specified LOCATION.
        /// The Window is configured to be the topmost in the display stack when first displayed;
        /// however, it may be pushed lower if the user desires.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="location"></param>
        public static void ShowAtFront(this Window window, Point? location = null)
        {
            if (location.HasValue)
            {
                window.Left = location.Value.X;
                window.Top = location.Value.Y;
                window.WindowStartupLocation = WindowStartupLocation.Manual;
            }

            if (window.WindowState == WindowState.Minimized)
                window.WindowState = WindowState.Normal;

            // Hacky fix to always force the window to the front...
            // Normally, this would be accomplished by setting the Window's 'owner'.
            // This is a general solution where the calling (Overlay) thread is may not be the same thread that owns
            // the window (Main thread).
            window.Activate();
            window.Topmost = true;
            window.Show();
            window.Topmost = false;
        }


        /// <summary>
        /// Shows a FORM at an optional caller-specified LOCATION.
        /// The Window is configured to be the topmost in the display stack when first displayed;
        /// however, it may be pushed lower if the user desires.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="location"></param>
        public static void ShowAtFront(this System.Windows.Forms.Form form, Point? location = null)
        {
            if (location.HasValue)
            {
                form.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                form.Location = new System.Drawing.Point((int)location.Value.X, (int)location.Value.Y);
            }

            form.BringToFront();
            form.Show();
        }
    }
}