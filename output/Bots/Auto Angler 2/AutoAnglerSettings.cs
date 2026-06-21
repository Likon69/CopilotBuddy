using Styx.Helpers;
using System.ComponentModel;
using DefaultValueAttribute = Styx.Helpers.DefaultValueAttribute;

namespace HighVoltz
{
    public class AutoAnglerSettings : Settings
    {
        public static AutoAnglerSettings Instance { get; private set; }
        public AutoAnglerSettings(string path) : base(path) { Instance = this; Load(); }

        [Setting, DefaultValue(0u),
        Description("Wowhead Id of the mainhand weapon to switch to when in combat"), Category("Weapon")]
        public uint MainHand { get; set; }

        [Setting, DefaultValue(0u), Description("Wowhead Id of the offhand weapon to switch to when in combat"),
        Category("Weapon")]
        public uint OffHand { get; set; }

        [Setting, DefaultValue(true),
        Description("Set this to true if you want to fish from pools, otherwise set to false."), Category("Fishing")]
        public bool Poolfishing { get; set; }

        [Setting, DefaultValue(PathingType.Circle),
        Description("In Circle mode, bot goes from 1st waypoint to last then jump back to 1st . In Bounce mode bot goes from 1st waypoint to last then it goes in reverse from bottom to top"),
        Category("Fishing")]
        public PathingType PathingType { get; set; }

        [Setting, DefaultValue(true), Description("Set to true to enable flying,false to use ground based navigation")
        , Category("Fishing")]
        public bool Fly { get; set; }

        [Setting, DefaultValue(true), Description("If set to true bot will use water walking, either use class abilities or pots")
        , Category("Fishing")]
        public bool UseWaterWalking { get; set; }

        [Setting, DefaultValue(false), Description("If set to true, bot will try to avoid landing in lava. Some pools by floating objects such as ice floes will get blacklisted if this is set to true")
        , Category("Fishing")]
        public bool AvoidLava { get; set; }

        [Setting, DefaultValue(5), Description("The maximum time in minutes to spend at a pool before it gets blacklisted")
        , Category("Advanced")]
        public int MaxTimeAtPool { get; set; }

        [Setting, DefaultValue(15), Description("The maximum number of failed casts at a pool before moving to a new location at pool")
        , Category("Advanced")]
        public int MaxFailedCasts { get; set; }

        [Setting, DefaultValue(false), Description("If set to true bot will 'ninja' nodes from other players.")
        , Category("Fishing")]
        public bool NinjaNodes { get; set; }

        [Setting, DefaultValue(40), Description("Number of tracelines to do in a 360 deg area. the higher the more likely to find a landing spot.recomended to set at a multiple of 20")
            , Category("Advanced")]
        public int TraceStep { get; set; }

        [Setting, DefaultValue(10), Description("Min range to stand at a pool, between 10 and 14 recomended")
            , Category("Advanced")]
        public int MinPoolRange { get; set; }

        [Setting, DefaultValue(20), Description("Max range to stand at a pool, 22 is maximum you should go. Recommended range is 18-22 (20 is probably best)")
            , Category("Advanced")]
        public int MaxPoolRange { get; set; }

        [Setting, DefaultValue(0.5f), Description("Each time bot fails to find a landing spot it adds this number to the range and tries again until it hits MaxPoolRange. Can use decimals.")
            , Category("Advanced")]
        public float PoolRangeStep { get; set; }

        [Setting, DefaultValue("")]
        public string LastLoadedProfile { get; set; }

        [Setting, DefaultValue(false),Description("If set to true bot will attempt to loot any dead lootable NPCs")]
        public bool LootNPCs { get; set; }
    }
}
