using System;
using System.ComponentModel;
using Styx;
using Styx.Helpers;
using Styx.Logic;

// NOTE: [DefaultValue] must resolve to Styx.Helpers.DefaultValueAttribute,
// not System.ComponentModel.DefaultValueAttribute. Use full qualification.

namespace Bots.BGBuddy
{
    /// <summary>
    /// Persistent settings for BGBuddy. Stored per-character in XML.
    /// </summary>
    internal class BGBuddySettings : Settings
    {
        private static BGBuddySettings _instance;

        public BGBuddySettings()
            : base(string.Format("{0}\\Settings\\BGBuddySettings_{1}.xml", Logging.ApplicationPath, StyxWoW.Me.Name))
        {
        }

        public static BGBuddySettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BGBuddySettings();
                return _instance;
            }
        }

        #region Settings

        [Styx.Helpers.DefaultValue(true)]
        [Browsable(false)]
        [Setting]
        public bool FirstTime { get; set; }

        [Setting]
        [Category("Settings")]
        [Styx.Helpers.DefaultValue(false)]
        [Description("Enabling this will make the toon loot player corpses if there is nothing else to do")]
        [DisplayName("Loot Corpses")]
        public bool LootCorpses { get; set; }

        [Category("Settings")]
        [DisplayName("Pull Distance")]
        [Setting]
        [Styx.Helpers.DefaultValue(40.0)]
        [Description("That is the distance that CC's pull and pullbuff behavior will be called. Suggested to be lowered for melee chars, so they don't dismount far away")]
        public double PullDistance { get; set; }

        [Setting]
        [Styx.Helpers.DefaultValue(40.0)]
        [Description("This is the distance that BGBuddy will mount up to travel. Suggested to be lowered for melee chars. Druids and shamans will use Ghost Wolf/Travel Form for distances lower then 60 yards with Singular")]
        [Category("Settings")]
        [DisplayName("Mounting Distance")]
        public double MountUpDistance { get; set; }

        [Setting]
        [Description("First battleground to queue")]
        [DisplayName("Queue #1")]
        [Category("Queue")]
        public BattlegroundType Queue1 { get; set; }

        [DisplayName("Queue #2")]
        [Description("Second battleground to queue")]
        [Category("Queue")]
        [Setting]
        public BattlegroundType Queue2 { get; set; }

        // Per-BG logic type settings (Attack/Defend preference)
        [Browsable(false)]
        [Setting]
        public LogicType EotsLogicType { get; set; }

        [Setting]
        [Browsable(false)]
        public LogicType BfgLogicType { get; set; }

        [Browsable(false)]
        [Setting]
        public LogicType AVLogicType { get; set; }

        [Browsable(false)]
        [Setting]
        public LogicType WsgLogicType { get; set; }

        [Setting]
        [Browsable(false)]
        public LogicType TpLogicType { get; set; }

        [Browsable(false)]
        [Setting]
        public LogicType IocLogicType { get; set; }

        [Setting]
        [Browsable(false)]
        public LogicType SotaLogicType { get; set; }

        [Browsable(false)]
        [Setting]
        public LogicType AbLogicType { get; set; }

        #endregion
    }
}
