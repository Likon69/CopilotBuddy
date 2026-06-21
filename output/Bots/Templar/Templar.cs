// Botbase created by AknA and Wigglez
// Features to be added:
// MailBox Support - Beta
// Protected Items Support - Beta
// Skinning - Beta
// Disenchantrix
// Fix bags (stacking / making bolts etc.)

using System;
using System.Windows.Forms;
using Styx;
using Styx.WoWInternals;
using Templar.GUI;
using Templar.Helpers;
using TreeSharp;

namespace Templar {
    public class Templar : BotBase {
        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        private static Composite _root;
        private Form _form;

        // ===========================================================
        // Constructors
        // ===========================================================

        // ===========================================================
        // Getter & Setter
        // ===========================================================

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        public override string Name {
            get { return "Templar"; }
        }

        public override Composite Root {
            get { return _root ?? (_root = Composites.CreateRoot()); }
        }

        public override PulseFlags PulseFlags {
            get { return PulseFlags.All; }
        }

        public override Form ConfigurationForm {
            get {
                if(_form == null || !_form.Visible) {
                    _form = new TemplarGUI();
                }

                return _form;
            }
        }

        public override void Start() {
            try {
                CustomLog.Name = "Templar";

                CustomBlacklist.SweepTimer();

                Lua.Events.AttachEvent("UI_ERROR_MESSAGE", TaskManager.HandleErrorMessage);

                PriorityTreeState.TreeState = PriorityTreeState.State.ReadyForTask;
            } catch(Exception e) {
                CustomLog.Normal("Could not initialize.\nMessage = {0}\nStacktrace = {1}", e.Message, e.StackTrace);
            } finally {
                CustomLog.Normal("Initialization complete.");
            }
        }

        public override void Stop() {
            try {
                CustomBlacklist.RemoveAllNames();

                Lua.Events.DetachEvent("UI_ERROR_MESSAGE", TaskManager.HandleErrorMessage);

                Variables.SetStartLocation = false;
                Variables.AlteredSettings = false;
            } catch(Exception e) {
                CustomLog.Normal("Could not dispose.\nMessage = {0}\nStacktrace = {1}", e.Message, e.StackTrace);
            } finally {
                CustomLog.Normal("Shutdown complete.");
            }
        }

        public override bool RequiresProfile {
            get { return false; }
        }

        public override void Pulse() {
            PriorityTreeState.TreeStateHandler();
        }

        // ===========================================================
        // Methods
        // ===========================================================

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================

    }
}
