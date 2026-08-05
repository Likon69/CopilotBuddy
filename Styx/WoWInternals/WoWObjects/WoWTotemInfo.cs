#nullable disable
using System;
using Styx.Helpers;
using Styx.Logic.Combat;

namespace Styx.WoWInternals.WoWObjects
{
    /// <summary>
    /// Information about an active totem.
    /// WotLK 3.3.5a implementation using Lua.
    /// </summary>
    public class WoWTotemInfo
    {
        #region Fields

        private readonly int _slot;

        private const uint TotemSlotTable = 0xBD0B00;
        private const uint SlotStride = 32;
        private const uint OffsetGuid = 8;
        private const uint OffsetName = 0x10;
        private const uint OffsetDuration = 0x14;
        private const uint OffsetStart = 0x18;

        private uint SlotBase => TotemSlotTable + SlotStride * (uint)_slot;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a totem info for a specific slot.
        /// </summary>
        /// <param name="slot">Slot index (0-3 for Fire, Earth, Water, Air)</param>
        public WoWTotemInfo(int slot)
        {
            _slot = slot;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the slot index (0=Fire, 1=Earth, 2=Water, 3=Air).
        /// </summary>
        public int Slot => _slot;

        /// <summary>
        /// Gets the totem type for this slot.
        /// </summary>
        public WoWTotemType Type
        {
            get
            {
                // Slots are 1-indexed in Lua: Fire=1, Earth=2, Water=3, Air=4
                // Our slots are 0-indexed: Fire=0, Earth=1, Water=2, Air=3
                return (WoWTotemType)(_slot + 1);
            }
        }

        /// <summary>
        /// Returns true if a totem is active in this slot.
        /// </summary>
        public bool Active => Guid != 0UL;

        /// <summary>
        /// Gets the name of the active totem.
        /// </summary>
        public string Name
        {
            get
            {
                try
                {
                    var memory = ObjectManager.Wow;
                    if (memory == null || Guid == 0UL)
                        return string.Empty;

                    uint namePtr = memory.Read<uint>(SlotBase + OffsetName);
                    if (namePtr == 0U)
                        return string.Empty;

                    return memory.ReadString(namePtr) ?? string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the spell ID that created this totem.
        /// </summary>
        public int SpellId
        {
            get
            {
                // WotLK GetTotemInfo doesn't return spellId directly
                // We need to find the totem unit and get CreatedBySpellId
                var unit = Unit;
                if (unit != null)
                    return (int)unit.CreatedBySpellId;
                
                // Fallback: try to match by name
                var name = Name;
                if (string.IsNullOrEmpty(name))
                    return 0;
                
                // GetSpellLink in WotLK 3.3.5a takes a spell name, not a spellbook slot index.
                // The Cata-style GetSpellLink(i, 'spell') doesn't exist here — use GetSpellLink(name) directly.
                var spellId = Lua.GetReturnVal<int>(
                    $"local spellName = GetSpellInfo('{name}'); if spellName then local link = GetSpellLink(spellName); if link then return tonumber(link:match('spell:(%d+)')) or 0; end end return 0", 0);
                
                return spellId;
            }
        }

        /// <summary>
        /// Gets the WoWTotem enum value.
        /// </summary>
        public WoWTotem Totem
        {
            get
            {
                var spellId = SpellId;
                if (Enum.IsDefined(typeof(WoWTotem), spellId))
                    return (WoWTotem)spellId;
                return WoWTotem.None;
            }
        }

        /// <summary>
        /// Gets the WoWTotem enum value (Singular compatibility).
        /// </summary>
        public WoWTotem WoWTotem => Totem;

        /// <summary>
        /// Gets the unit representing this totem.
        /// </summary>
        public WoWUnit Unit
        {
            get
            {
                var guid = Guid;
                if (guid == 0)
                    return null;
                return ObjectManager.GetObjectByGuid<WoWUnit>(guid);
            }
        }

        /// <summary>
        /// Gets the GUID of the totem unit.
        /// </summary>
        public ulong Guid
        {
            get
            {
                try
                {
                    var memory = ObjectManager.Wow;
                    if (memory == null)
                        return 0UL;

                    return memory.Read<ulong>(SlotBase + OffsetGuid);
                }
                catch
                {
                    return 0UL;
                }
            }
        }

        /// <summary>
        /// Gets the time when the totem was placed.
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                uint startMs = RawStartMs;
                if (startMs == 0U)
                    return DateTime.MinValue;

                long elapsed = (long)ObjectManager.PerformanceCounter - startMs;
                if (elapsed < 0L)
                    return DateTime.MinValue;

                return DateTime.Now.AddMilliseconds(-elapsed);
            }
        }

        /// <summary>
        /// Gets the duration of the totem in seconds.
        /// </summary>
        public double Duration => RawDurationMs / 1000.0;

        /// <summary>
        /// Gets the remaining time on the totem.
        /// </summary>
        public TimeSpan TimeLeft
        {
            get
            {
                uint startMs = RawStartMs;
                uint durationMs = RawDurationMs;
                if (startMs == 0U || durationMs == 0U || Guid == 0UL)
                    return TimeSpan.Zero;

                long remaining = (long)startMs + durationMs - ObjectManager.PerformanceCounter;
                return remaining > 0 ? TimeSpan.FromMilliseconds(remaining) : TimeSpan.Zero;
            }
        }

        private uint RawStartMs => ReadSlotDword(OffsetStart);
        private uint RawDurationMs => ReadSlotDword(OffsetDuration);

        private uint ReadSlotDword(uint offset)
        {
            try
            {
                var memory = ObjectManager.Wow;
                return memory == null ? 0U : memory.Read<uint>(SlotBase + offset);
            }
            catch
            {
                return 0U;
            }
        }

        /// <summary>
        /// Returns true if the totem has expired.
        /// </summary>
        public bool Expired => !Active || TimeLeft <= TimeSpan.Zero;

        /// <summary>
        /// Gets the spell that created this totem.
        /// </summary>
        public WoWSpell Spell => WoWSpell.FromId(SpellId);

        /// <summary>
        /// Gets the icon texture path for this totem's spell.
        /// </summary>
        public string IconPath
        {
            get
            {
                int id = SpellId;
                if (id <= 0)
                    return string.Empty;
                return Lua.GetReturnVal<string>($"local t = GetSpellTexture({id}); return t or ''", 0) ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets the expiry DateTime for this totem (StartTime + Duration).
        /// </summary>
        public DateTime Expires
        {
            get
            {
                var start = StartTime;
                if (start == DateTime.MinValue)
                    return DateTime.MinValue;
                return start.AddSeconds(Duration);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Destroys this totem.
        /// </summary>
        public void Destroy()
        {
            int luaSlot = _slot + 1;
            Lua.DoString($"DestroyTotem({luaSlot})");
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return $"[Totem Slot {_slot}] {Type}: {Name} - Active:{Active} TimeLeft:{TimeLeft}";
        }

        #endregion
    }
}
