using System;
using System.Windows.Forms;
using ObjectGatherer.GUI;
using ObjectGatherer.Helpers;
using Styx;
using TreeSharp;

namespace ObjectGatherer {
    public class ObjectGatherer : BotBase {
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

        public ObjectGatherer() {
            BotEvents.Profile.OnNewOuterProfileLoaded += ProfileHandler.OnNewOuterProfileLoaded;
            BotEvents.Profile.OnNewProfileLoaded += ProfileHandler.OnNewProfileLoaded;
        }

        // ===========================================================
        // Getter & Setter
        // ===========================================================

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================


        public override string Name {
            get { return "ObjectGatherer"; }
        }

        public override Composite Root {
            get { return _root ?? (_root = Composites.CreateRoot()); }
        }

        public override PulseFlags PulseFlags {
            get { return PulseFlags.All; }
        }

        public override bool RequiresProfile {
            get { return false; }
        }

        public override Form ConfigurationForm {
            get {
                if(_form == null || !_form.Visible) {
                    _form = new ObjectGathererGUI();
                }

                return _form;
            }
        }

        public override void Start() {
            try {
                CustomLog.Name = "ObjectGatherer";
                EntryList.Populate();
                CustomBlacklist.SweepTimer();
                PriorityTreeState.TreeState = PriorityTreeState.State.ReadyForTask;
            } catch(Exception e) {
                CustomLog.Normal("Could not initialize.\nMessage = {0}\nStacktrace = {1}", e.Message, e.StackTrace);
            } finally {
                CustomLog.Normal("{0} initialized.", CustomLog.Name);
            }
        }

        public override void Stop() {
            CustomBlacklist.RemoveAllGuid();
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
