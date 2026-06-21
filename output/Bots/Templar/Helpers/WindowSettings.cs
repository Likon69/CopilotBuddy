using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Templar.Helpers {
    /* Author: Don Kirkby http://donkirkby.googlecode.com/
     * Released under the MIT licence http://www.opensource.org/licenses/mit-license.php
     * Installation:
     * - Copy this file into your project.
     * - Change the namespace above to match your project's namespace.
     * - Compile your project.
     * - Edit the project properties using the Project Properties... item in 
     * the project menu.
     * - Go to the settings tab.
     * - Add a new setting for each form whose position you want to save, and 
     * type a name for it like MainFormPosition.
     * - In the type column, select Browse... from the bottom of the list.
     * - You won't see WindowSettings in the list, but you can just type the
     * namespace and class name, and click OK. For example, if you changed this
     * class's namespace to UltimateApp, then you would type 
     * UltimateApp.WindowSettings and click OK.
     * - Add Load and FormClosing event handlers to any forms you want to save.
     * See the forms in this project for example code.
     * - Add a call to Settings.Default.Save() somewhere in your shutdown code.
     * The FormClosed event of your main form is a good spot. If you have 
     * subforms open, you may have to explicitly call their FormClosing events 
     * when shutting down the app, because they're not called by default.
     */
    /// <summary>
    /// Serializes a window's location, size, state, and any splitter 
    /// positions to a single application setting. Then you can just create a 
    /// setting of this type for each form in the application, save on close, 
    /// and restore on load.
    /// </summary>
    [Serializable]
    public class WindowSettings {
        public Point Location { get; set; }
        public Size Size { get; set; }
        public FormWindowState WindowState { get; set; }
        public int[] SplitterDistances { get; set; }
        public int[] SplitterSizes { get; set; }

        public WindowSettings() {
            // default to an invalid location
            Location = new Point(Int32.MinValue, Int32.MinValue);
        }

        /// <summary>
        /// Record a form's position and that of several splitters.
        /// </summary>
        /// <param name="windowSettings">Where the settings should be recorded,
        /// or null.</param>
        /// <param name="form">The form to record. May be null if you just want 
        /// to record splitter positions.</param>
        /// <param name="splitters">The splitters to record. You can change 
        /// some entries to null if you no longer use that position in the 
        /// list.</param>
        /// <returns>The windowSettings parameter, or a new WindowSettings 
        /// object if that was null.</returns>
        public static WindowSettings Record(
            WindowSettings windowSettings,
            Form form,
            params SplitContainer[] splitters) {
            if(windowSettings == null) {
                windowSettings = new WindowSettings();
            }
            windowSettings.Record(form, splitters);
            return windowSettings;
        }

        /// <summary>
        /// Record a form's position and that of several splitters.
        /// </summary>
        /// <param name="form">The form to record. May be null if you just want 
        /// to record splitter positions.</param>
        /// <param name="splitters">The splitters to record. You can change 
        /// some entries to null if you no longer use that position in the 
        /// list.</param>
        public void Record(Form form, params SplitContainer[] splitters) {
            bool shouldRecordSplitters;
            if(form == null) {
                shouldRecordSplitters = (from splitter in splitters where splitter != null let findForm = splitter.FindForm() where findForm != null select findForm.WindowState into formState select formState != FormWindowState.Minimized).FirstOrDefault();
            } else {
                switch(form.WindowState) {
                    case FormWindowState.Maximized:
                        RecordWindowPosition(form.RestoreBounds);
                        shouldRecordSplitters = true;
                        break;
                    case FormWindowState.Normal:
                        shouldRecordSplitters =
                            RecordWindowPosition(form.Bounds);
                        break;
                    default:
                        // Don't record anything when closing while minimized.
                        return;
                }
                WindowState = form.WindowState;
            }

            if(shouldRecordSplitters) {
                RecordSplitters(splitters);
            }
        }

        /// <summary>
        /// Restore a form's position and that of several splitters.
        /// </summary>
        /// <param name="windowSettings">Holds the settings to restore.</param>
        /// <param name="form">The form to restore. May be null if you just want 
        /// to record splitter positions.</param>
        /// <param name="splitters">The splitters to restore. You can change 
        /// some entries to null if you no longer use that position in the 
        /// list.</param>
        public static void Restore(
            WindowSettings windowSettings,
            Form form,
            params SplitContainer[] splitters) {
            if(windowSettings != null) {
                windowSettings.Restore(form, splitters);
            }
        }

        /// <summary>
        /// Restore a form's position and that of several splitters.
        /// </summary>
        /// <param name="form">The form to restore. May be null if you just want 
        /// to record splitter positions.</param>
        /// <param name="splitters">The splitters to restore. You can change 
        /// some entries to null if you no longer use that position in the 
        /// list.</param>
        public void Restore(Form form, params SplitContainer[] splitters) {
            if(form == null) {
                RestoreSplitters(splitters);
            } else if(IsOnScreen(Location, Size)) {
                form.Location = Location;
                form.Size = Size;
                form.WindowState = WindowState;
                RestoreSplitters(splitters);
            } else {
                form.WindowState = WindowState;
            }
        }

        private void RestoreSplitters(SplitContainer[] splitters) {
            for(
                var i = 0;
                i < splitters.Length &&
                SplitterDistances != null &&
                i < SplitterDistances.Length;
                i++) {
                var splitter = splitters[i];
                if(splitter == null) {
                    continue;
                }
                var splitterDistance = SplitterDistances[i];
                if(SplitterSizes != null) {
                    splitterDistance =
                        splitterDistance * GetSplitterSize(splitter) / SplitterSizes[i];
                }
                var splitterSize = GetSplitterSize(splitter);
                var isDistanceLegal =
                    splitter.Panel1MinSize <= splitterDistance
                    && splitterDistance <= splitterSize - splitter.Panel2MinSize;
                if(isDistanceLegal) {
                    splitter.SplitterDistance = splitterDistance;
                }
            }
        }

        private static int GetSplitterSize(SplitContainer splitter) {
            var splitterSize =
                splitter.Orientation == Orientation.Vertical
                ? splitter.Width
                : splitter.Height;
            return splitterSize;
        }

        private bool RecordWindowPosition(Rectangle bounds) {
            if(!IsOnScreen(bounds.Location, bounds.Size)) {
                return false;
            }

            Location = bounds.Location;
            Size = bounds.Size;
            return true;
        }

        private void RecordSplitters(IList<SplitContainer> splitters) {
            SplitterDistances = new int[splitters.Count];
            SplitterSizes = new int[splitters.Count];
            for(var i = 0; i < splitters.Count; i++) {
                if(splitters[i] == null) {
                    continue;
                }
                SplitterDistances[i] = splitters[i].SplitterDistance;
                SplitterSizes[i] = GetSplitterSize(splitters[i]);
            }
        }

        private static bool IsOnScreen(Point location, Size size) {
            return IsOnScreen(location) && IsOnScreen(location + size);
        }

        private static bool IsOnScreen(Point location) {
            foreach(var workingArea in Screen.AllScreens.Select(screen => new Rectangle(
                screen.WorkingArea.Location,
                screen.WorkingArea.Size))) {
                workingArea.Inflate(1, 1);
                if(workingArea.Contains(location)) {
                    return true;
                }
            }
            return false;
        }
    }
}
