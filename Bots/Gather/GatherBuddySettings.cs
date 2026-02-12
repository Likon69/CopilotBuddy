using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Styx;
using Styx.Helpers;

namespace Bots.Gather
{
    /// <summary>
    /// Persistent settings for GatherBuddy.
    /// Saved to Settings/GatherBuddySettings_{Name}.xml
    /// Pattern from HB 3.3.5a.
    /// </summary>
    public class GatherBuddySettings : Settings
    {
        public static readonly GatherBuddySettings Instance = new GatherBuddySettings();

        public GatherBuddySettings()
            : base(Path.Combine(Logging.ApplicationPath,
                string.Format("Settings\\GatherBuddySettings_{0}.xml",
                (StyxWoW.Me != null) ? StyxWoW.Me.Name : "")))
        {
        }

        // ═══════════════════════════════════════════════════════════
        // GATHERING
        // ═══════════════════════════════════════════════════════════
        
        [Setting, DefaultValue(true)]
        public bool GatherHerbs { get; set; }

        [Setting, DefaultValue(true)]
        public bool GatherMinerals { get; set; }

        /// <summary>
        /// Gather treasure chests along the route.
        /// </summary>
        [Setting, DefaultValue(false)]
        public bool GatherChests { get; set; }

        /// <summary>
        /// Skin killed mobs (requires Skinning profession).
        /// </summary>
        [Setting, DefaultValue(false)]
        public bool SkinMobs { get; set; }

        // ═══════════════════════════════════════════════════════════
        // NAVIGATION
        // ═══════════════════════════════════════════════════════════
        
        [Setting, DefaultValue(PathType.Circle)]
        public PathType PathingType { get; set; }
        
        /// <summary>
        /// Maximum detection range for nodes (yards)
        /// </summary>
        [Setting, DefaultValue(70f)]
        public float NodeDetectionRange { get; set; }
        
        /// <summary>
        /// Height modifier for flying (yards above ground)
        /// </summary>
        [Setting, DefaultValue(0f)]
        public float HeightModifier { get; set; }

        /// <summary>
        /// Randomize hotspot visit order on start.
        /// </summary>
        [Setting, DefaultValue(false)]
        public bool RandomizeHotspots { get; set; }

        // ═══════════════════════════════════════════════════════════
        // COMBAT
        // ═══════════════════════════════════════════════════════════
        
        /// <summary>
        /// Loot killed mobs during gathering
        /// </summary>
        [Setting, DefaultValue(true)]
        public bool LootMobs { get; set; }
        
        /// <summary>
        /// Ignore Elite mobs (do not pull them)
        /// </summary>
        [Setting, DefaultValue(true)]
        public bool IgnoreElites { get; set; }

        /// <summary>
        /// Face nodes before interacting
        /// </summary>
        [Setting, DefaultValue(true)]
        public bool FaceNodes { get; set; }

        /// <summary>
        /// Loot radius in yards.
        /// </summary>
        [Setting, DefaultValue(30f)]
        public float LootRadius { get; set; }

        // ═══════════════════════════════════════════════════════════
        // ANTI-NINJA
        // ═══════════════════════════════════════════════════════════
        
        /// <summary>
        /// Do not steal nodes from other players
        /// </summary>
        [Setting, DefaultValue(true)]
        public bool NoNinja { get; set; }
        
        /// <summary>
        /// Blacklist duration for failed nodes (seconds)
        /// </summary>
        [Setting, DefaultValue(20)]
        public int BlacklistTimer { get; set; }

        // ═══════════════════════════════════════════════════════════
        // VENDOR/REPAIR
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Go to vendor when bags are full.
        /// Uses profile <Vendors> if available, otherwise scans nearby NPCs.
        /// </summary>
        [Setting, DefaultValue(true)]
        public bool VendorWhenFull { get; set; }

        /// <summary>
        /// Repair gear at vendor when durability is low.
        /// </summary>
        [Setting, DefaultValue(true)]
        public bool RepairAtVendor { get; set; }

        /// <summary>
        /// Durability percentage threshold to trigger repair.
        /// </summary>
        [Setting, DefaultValue(20)]
        public int RepairDurabilityPercent { get; set; }

        /// <summary>
        /// Number of free bag slots below which the bot goes to vendor.
        /// </summary>
        [Setting, DefaultValue(2)]
        public int MinFreeBagSlots { get; set; }

        /// <summary>
        /// Use FindVendorsAutomatically when profile has no vendors.
        /// Falls back to NpcDatabase queries.
        /// </summary>
        [Setting, DefaultValue(false)]
        public bool FindVendorsAutomatically { get; set; }

        // ═══════════════════════════════════════════════════════════
        // SELL QUALITY FILTERS
        // ═══════════════════════════════════════════════════════════

        [Setting, DefaultValue(true)]
        public bool SellGrey { get; set; }

        [Setting, DefaultValue(true)]
        public bool SellWhite { get; set; }

        [Setting, DefaultValue(false)]
        public bool SellGreen { get; set; }

        [Setting, DefaultValue(false)]
        public bool SellBlue { get; set; }

        [Setting, DefaultValue(false)]
        public bool SellPurple { get; set; }

        // ═══════════════════════════════════════════════════════════
        // MAIL
        // ═══════════════════════════════════════════════════════════

        [Setting, DefaultValue(false)]
        public bool MailToAlt { get; set; }
        
        /// <summary>
        /// Mail recipient character name
        /// </summary>
        [Setting, DefaultValue("")]
        public string MailRecipient { get; set; } = string.Empty;

        [Setting, DefaultValue(false)]
        public bool MailGrey { get; set; }

        [Setting, DefaultValue(true)]
        public bool MailWhite { get; set; }

        [Setting, DefaultValue(true)]
        public bool MailGreen { get; set; }

        [Setting, DefaultValue(true)]
        public bool MailBlue { get; set; }

        [Setting, DefaultValue(true)]
        public bool MailPurple { get; set; }

        // ═══════════════════════════════════════════════════════════
        // FLYING
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Use flying mount + Flightor for navigation when possible.
        /// Requires Cold Weather Flying in Northrend zones.
        /// </summary>
        [Setting, DefaultValue(false)]
        public bool UseFlying { get; set; }

        /// <summary>
        /// Flying altitude above ground (yards). Used as HeightModifier for Flightor.
        /// </summary>
        [Setting, DefaultValue(20f)]
        public float FlyingAltitude { get; set; }

        /// <summary>
        /// Minimum distance to node before descending to gather (yards).
        /// </summary>
        [Setting, DefaultValue(15f)]
        public float FlyingDescentRange { get; set; }

        // ═══════════════════════════════════════════════════════════
        // DEATH / SAFETY
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Use spirit healer instead of corpse running (accepts rez sickness).
        /// </summary>
        [Setting, DefaultValue(false)]
        public bool UseSpiritHealer { get; set; }

        /// <summary>
        /// Wait out resurrection sickness debuff before continuing.
        /// </summary>
        [Setting, DefaultValue(true)]
        public bool WaitRezSickness { get; set; }

        // ═══════════════════════════════════════════════════════════
        // SESSION TIMER
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Auto-stop after X hours (0 = infinite/no limit).
        /// </summary>
        [Setting, DefaultValue(0f)]
        public float BottingHours { get; set; }

        /// <summary>
        /// Use Hearthstone and exit when BottingHours expires.
        /// </summary>
        [Setting, DefaultValue(true)]
        public bool HearthAndExit { get; set; }

        // ═══════════════════════════════════════════════════════════
        // NODE SELECTION — BLACKLIST
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Comma-separated list of Entry IDs that should NOT be gathered.
        /// Unchecked nodes in the Node Selection tab are stored here.
        /// </summary>
        [Setting, DefaultValue("")]
        public string BlacklistedEntriesRaw { get; set; } = string.Empty;

        /// <summary>
        /// Parsed set of blacklisted Entry IDs. Not persisted directly —
        /// read/write through <see cref="BlacklistedEntriesRaw"/>.
        /// </summary>
        public HashSet<uint> BlacklistedEntries
        {
            get
            {
                if (_blacklistedEntries == null)
                {
                    _blacklistedEntries = new HashSet<uint>();
                    if (!string.IsNullOrWhiteSpace(BlacklistedEntriesRaw))
                    {
                        foreach (var token in BlacklistedEntriesRaw.Split(','))
                        {
                            if (uint.TryParse(token.Trim(), out uint entry))
                                _blacklistedEntries.Add(entry);
                        }
                    }
                }
                return _blacklistedEntries;
            }
        }

        private HashSet<uint>? _blacklistedEntries;

        /// <summary>
        /// Update the blacklist from a set of Entry IDs.
        /// Persists to the raw string setting.
        /// </summary>
        public void SetBlacklistedEntries(HashSet<uint> entries)
        {
            _blacklistedEntries = entries;
            BlacklistedEntriesRaw = string.Join(",", entries.OrderBy(e => e));
        }
    }
}
