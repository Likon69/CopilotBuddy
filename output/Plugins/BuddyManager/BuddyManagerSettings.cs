using System.IO;
using Styx;
using Styx.Helpers;

namespace BuddyManager
{
    public class BuddyManagerSettings : Settings
    {
        public static readonly BuddyManagerSettings Instance = new BuddyManagerSettings();

        public BuddyManagerSettings()
            : base(Path.Combine(Logging.ApplicationPath, string.Format(@"Settings/BuddyManager/BuddyManager-Settings-{0}-{1}.xml", StyxWoW.Me.Name, StyxWoW.Me.Race)))
        {
        }
        

        [Setting, DefaultValue("Grind Bot")]
        public string SelectedBB1 { get; set; }

        [Setting, DefaultValue("Grind Bot")]
        public string SelectedBB2 { get; set; }

        [Setting, DefaultValue("Grind Bot")]
        public string SelectedBB3 { get; set; }

        [Setting, DefaultValue("Grind Bot")]
        public string SelectedBB4 { get; set; }

        [Setting, DefaultValue("None")]
        public string SelectedProfile1 { get; set; }

        [Setting, DefaultValue("None")]
        public string SelectedProfile2 { get; set; }

        [Setting, DefaultValue("None")]
        public string SelectedProfile3 { get; set; }

        [Setting, DefaultValue("None")]
        public string SelectedProfile4 { get; set; }

        [Setting, DefaultValue(false)]
        public bool ThreeEnabled { get; set; }

        [Setting, DefaultValue(false)]
        public bool FourEnabled { get; set; }

        [Setting, DefaultValue(true)]
        public bool LogOutAfter { get; set; }

        [Setting, DefaultValue(false)]
        public bool LoopAll { get; set; }

        [Setting, DefaultValue(0)]
        public int LoopHours { get; set; }

        [Setting, DefaultValue(0)]
        public int LoopMins { get; set; }

        [Setting, DefaultValue(0)]
        public int P1Hours { get; set; }

        [Setting, DefaultValue(0)]
        public int P2Hours { get; set; }

        [Setting, DefaultValue(0)]
        public int P3Hours { get; set; }

        [Setting, DefaultValue(0)]
        public int P4Hours { get; set; }

        [Setting, DefaultValue(0)]
        public int P1Mins { get; set; }

        [Setting, DefaultValue(0)]
        public int P2Mins { get; set; }

        [Setting, DefaultValue(0)]
        public int P3Mins { get; set; }

        [Setting, DefaultValue(0)]
        public int P4Mins { get; set; }

        [Setting, DefaultValue(" ")]
        public string Zone1 { get; set; }

        [Setting, DefaultValue(" ")]
        public string Zone2 { get; set; }

        [Setting, DefaultValue(" ")]
        public string Zone3 { get; set; }

        [Setting, DefaultValue(" ")]
        public string Zone4 { get; set; }

        [Setting, DefaultValue(false)]
        public bool Repair { get; set; }

        [Setting, DefaultValue(false)]
        public bool Mail { get; set; }

        [Setting, DefaultValue(false)]
        public bool Sell { get; set; }

        [Setting, DefaultValue(false)]
        public bool HearthRMS { get; set; }


    }

}


