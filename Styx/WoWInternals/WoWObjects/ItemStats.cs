using System;
using System.Collections.Generic;
using Styx.Helpers;
using Styx;
using GreenMagic;

namespace Styx.WoWInternals.WoWObjects
{
    public class ItemStats
    {
        #region Constructor
        public ItemStats()
        {
            Stats = new Dictionary<StatTypes, int>();
            DPS = 0f;
        }
        
        public ItemStats(string itemLink)
        {
            ItemStats itemStats = GetItemStatsFromLink(itemLink);
            this.Stats = itemStats.Stats;
            this.DPS = itemStats.DPS;
        }
        
        #endregion
        
        #region Internal Methods
        private const uint FillItemStatsAddress = 6424480U;
        private const int ItemStatFieldCount = 73;
        // CGItemStats_C struct: float DPS + 73 stat dwords + trailing flags dword = 300 bytes (HB Struct24)
        private const int ItemStatsStructSize = 4 + ItemStatFieldCount * 4 + 4;

        private static ItemStats GetItemStatsFromLink(string itemLink)
        {
            var stats = new ItemStats();

            ExecutorRand? executor = ObjectManager.Executor;
            if (executor == null || string.IsNullOrEmpty(itemLink))
                return stats;

            lock (executor.AssemblyLock)
            {
                using (AllocatedMemory linkBuffer = new AllocatedMemory(itemLink.Length + 2))
                using (AllocatedMemory buffer = new AllocatedMemory(ItemStatsStructSize))
                {
                    if (linkBuffer.Address == 0U || buffer.Address == 0U)
                        return stats;

                    Memory memory = executor.Memory;
                    if (memory == null)
                        return stats;

                    memory.Write(linkBuffer.Address, itemLink);

                    executor.Clear();
                    executor.AddLine("push {0}", linkBuffer.Address);
                    executor.AddLine("mov ecx, {0}", buffer.Address);
                    executor.AddLine("call {0}", FillItemStatsAddress);
                    executor.AddLine("retn");
                    executor.Execute();

                    using (StyxWoW.Memory.TemporaryCacheState(false))
                    {
                        stats.DPS = memory.Read<float>(buffer.Address);
                        if (float.IsNaN(stats.DPS))
                            stats.DPS = 0f;

                        for (int i = 0; i < ItemStatFieldCount; i++)
                        {
                            int value = memory.Read<int>(buffer.Address + 4U + (uint)(i * 4));
                            if (value != 0)
                                stats.Stats[(StatTypes)i] = value;
                        }
                    }
                }
            }

            return stats;
        }
        #endregion
        
        #region Properties
        public float DPS;
        public Dictionary<StatTypes, int> Stats;
        
        #endregion
        
        #region Helper Methods
        public int GetStat(StatTypes type)
        {
            if (Stats == null) return 0;
            return Stats.TryGetValue(type, out int value) ? value : 0;
        }
        public bool HasStat(StatTypes type)
        {
            return Stats != null && Stats.ContainsKey(type);
        }
        public int TotalStats
        {
            get
            {
                if (Stats == null) return 0;
                int total = 0;
                foreach (var kvp in Stats)
                {
                    total += kvp.Value;
                }
                return total;
            }
        }
        
        #endregion
        
        #region ToString
        public override string ToString()
        {
            if (Stats == null || Stats.Count == 0)
            {
                return $"ItemStats [DPS: {DPS:F1}, Stats: None]";
            }
            return $"ItemStats [DPS: {DPS:F1}, Stats: {Stats.Count}]";
        }
        
        #endregion
    }
    
    // Use Styx.StatTypes (canonical) instead of redeclaring here.
    
    #region WoWItemStatType Enum
    public enum WoWItemStatType
    {
        None = 0,
        Health = 1,
        Mana = 2,
        Agility = 3,
        Strength = 4,
        Intellect = 5,
        Spirit = 6,
        Stamina = 7,
        DefenseSkillRating = 12,
        DodgeRating = 13,
        ParryRating = 14,
        BlockRating = 15,
        HitMeleeRating = 16,
        HitRangedRating = 17,
        HitSpellRating = 18,
        CritMeleeRating = 19,
        CritRangedRating = 20,
        CritSpellRating = 21,
        HitTakenMeleeRating = 22,
        HitTakenRangedRating = 23,
        HitTakenSpellRating = 24,
        CritTakenMeleeRating = 25,
        CritTakenRangedRating = 26,
        CritTakenSpellRating = 27,
        HasteMeleeRating = 28,
        HasteRangedRating = 29,
        HasteSpellRating = 30,
        HitRating = 31,
        CritRating = 32,
        HitTakenRating = 33,
        CritTakenRating = 34,
        ResilienceRating = 35,
        HasteRating = 36,
        ExpertiseRating = 37,
        AttackPower = 38,
        RangedAttackPower = 39,
        FeralAttackPower = 40,
        SpellHealingDone = 41,
        SpellDamageDone = 42,
        ManaRegeneration = 43,
        ArmorPenetrationRating = 44,
        SpellPower = 45,
        HealthRegeneration = 46,
        SpellPenetration = 47,
        BlockValue = 48
    }
    
    #endregion
    
    #region WoWSocketColor Enum
    [Flags]
    public enum WoWSocketColor
    {
        None = 0,
        Meta = 1,
        Red = 2,
        Yellow = 4,
        Blue = 8,
        
        // Combinaisons
        Orange = Red | Yellow,      // 6
        Purple = Red | Blue,        // 10
        Green = Yellow | Blue,      // 12
        Prismatic = Red | Yellow | Blue  // 14
    }
    
    #endregion
}
