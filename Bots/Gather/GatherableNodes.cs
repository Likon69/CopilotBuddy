using System.Collections.Generic;

namespace Bots.Gather
{
    /// <summary>
    /// Represents a gatherable resource node (herb or mineral) in WoW 3.3.5a.
    /// </summary>
    public sealed record GatherableNode(uint Entry, string Name, uint RequiredSkill);

    /// <summary>
    /// Hardcoded lists of all WotLK-and-below herb and mineral game objects.
    /// Entry IDs, names, and required skill levels sourced from HB 4.3.4 reference.
    /// Filtered to WotLK max skill cap (450).
    /// </summary>
    public static class GatherableNodes
    {
        // ═══════════════════════════════════════════════════════════
        // HERBS — Classic through WotLK (Herbalism skill ≤ 450)
        // ═══════════════════════════════════════════════════════════

        public static readonly List<GatherableNode> Herbs = new()
        {
            // ── Classic (1–300) ──
            new(1618U,   "Peacebloom",           1U),
            new(1617U,   "Silverleaf",           1U),
            new(1619U,   "Earthroot",           15U),
            new(1620U,   "Mageroyal",           50U),
            new(1621U,   "Briarthorn",          70U),
            new(1622U,   "Bruiseweed",          85U),
            new(2045U,   "Stranglekelp",        85U),
            new(1628U,   "Grave Moss",         105U),
            new(1623U,   "Wild Steelbloom",    115U),
            new(1624U,   "Kingsblood",         125U),
            new(2042U,   "Fadeleaf",           150U),
            new(2046U,   "Goldthorn",          150U),
            new(2041U,   "Liferoot",           150U),
            new(2043U,   "Khadgar's Whisker",  160U),
            new(2044U,   "Wintersbite",        195U),
            new(2866U,   "Firebloom",          205U),
            new(142140U, "Purple Lotus",       210U),
            new(142141U, "Arthas' Tears",      220U),
            new(142142U, "Sungrass",           230U),
            new(142143U, "Blindweed",          235U),
            new(142144U, "Ghost Mushroom",     245U),
            new(142145U, "Gromsblood",         250U),
            new(176583U, "Golden Sansam",      260U),
            new(176584U, "Dreamfoil",          270U),
            new(176588U, "Icecap",             270U),
            new(176586U, "Mountain Silversage", 280U),
            new(176587U, "Plaguebloom",        285U),
            new(176589U, "Black Lotus",        300U),

            // ── Burning Crusade (300–375) ──
            new(181166U, "Bloodthistle",         1U),   // Blood Elf starting zone
            new(181270U, "Felweed",            300U),
            new(181271U, "Dreaming Glory",     315U),
            new(181275U, "Ragveil",            325U),
            new(181277U, "Terocone",           325U),
            new(181276U, "Flame Cap",          335U),
            new(181278U, "Ancient Lichen",     340U),
            new(181279U, "Netherbloom",        350U),
            new(185881U, "Netherdust Bush",    350U),
            new(181280U, "Nightmare Vine",     365U),
            new(181281U, "Mana Thistle",       375U),

            // ── Wrath of the Lich King (350–450) ──
            new(189973U, "Goldclover",         350U),
            new(191303U, "Firethorn",          360U),
            new(190169U, "Tiger Lily",         375U),
            new(190170U, "Talandra's Rose",    385U),
            new(191019U, "Adder's Tongue",     400U),
            new(190173U, "Frozen Herb",        400U),
            new(190171U, "Lichbloom",          425U),
            new(190172U, "Icethorn",           435U),
            new(190176U, "Frost Lotus",        450U),
        };

        // ═══════════════════════════════════════════════════════════
        // MINERALS — Classic through WotLK (Mining skill ≤ 450)
        // ═══════════════════════════════════════════════════════════

        public static readonly List<GatherableNode> Minerals = new()
        {
            // ── Classic (1–300) ──
            new(1731U,   "Copper Vein",              1U),
            new(1732U,   "Tin Vein",                50U),
            new(1733U,   "Silver Vein",             65U),
            new(1735U,   "Iron Deposit",           100U),
            new(1734U,   "Gold Vein",              115U),
            new(2040U,   "Mithril Deposit",        150U),
            new(2047U,   "Truesilver Deposit",     165U),
            new(165658U, "Dark Iron Deposit",      175U),
            new(324U,    "Small Thorium Vein",     200U),
            new(175404U, "Rich Thorium Vein",      215U),

            // ── Rare/Quest Classic nodes ──
            new(1610U,   "Incendicite Mineral Vein", 50U),
            new(2653U,   "Lesser Bloodstone Deposit", 65U),
            new(73940U,  "Ooze Covered Silver Vein",  65U),
            new(73941U,  "Ooze Covered Gold Vein",   115U),
            new(19903U,  "Indurium Mineral Vein",    150U),
            new(123310U, "Ooze Covered Mithril Deposit", 150U),
            new(123309U, "Ooze Covered Truesilver Deposit", 165U),
            new(123848U, "Ooze Covered Thorium Vein", 200U),
            new(177388U, "Ooze Covered Rich Thorium Vein", 215U),
            new(188699U, "Strange Ore",              1U),
            new(191844U, "Enchanted Earth",          1U),
            new(191845U, "Enchanted Earth",          1U),

            // ── Burning Crusade (275–375) ──
            new(181555U, "Fel Iron Deposit",       275U),
            new(181556U, "Adamantite Deposit",     325U),
            new(181569U, "Rich Adamantite Deposit", 350U),
            new(181557U, "Khorium Vein",           375U),
            new(181069U, "Large Obsidian Chunk",   305U),
            new(181068U, "Small Obsidian Chunk",   305U),
            new(185557U, "Ancient Gem Vein",       375U),

            // ── Wrath of the Lich King (350–450) ──
            new(189978U, "Cobalt Deposit",         350U),
            new(189979U, "Rich Cobalt Deposit",    375U),
            new(189980U, "Saronite Deposit",       400U),
            new(189981U, "Rich Saronite Deposit",  425U),
            new(195036U, "Pure Saronite Deposit",  450U),
            new(191133U, "Titanium Vein",          450U),
        };

        /// <summary>
        /// Entry IDs that are blacklisted by default (rare quest nodes, special objects).
        /// Same defaults as HB 4.3.4 GatherBuddy.
        /// </summary>
        public static readonly HashSet<uint> DefaultBlacklistedEntries = new()
        {
            185881U,  // Netherdust Bush
            185877U,  // Nethercite Deposit (excluded from IsMineral already)
            1610U,    // Incendicite Mineral Vein
            19903U,   // Indurium Mineral Vein
            73940U,   // Ooze Covered Silver Vein
            73941U,   // Ooze Covered Gold Vein
            123310U,  // Ooze Covered Mithril Deposit
            123309U,  // Ooze Covered Truesilver Deposit
            123848U,  // Ooze Covered Thorium Vein
            177388U,  // Ooze Covered Rich Thorium Vein
        };
    }
}
