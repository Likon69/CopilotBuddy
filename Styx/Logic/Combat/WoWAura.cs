using System;
using System.Runtime.InteropServices;
using Styx.WoWInternals;

namespace Styx.Logic.Combat
{
    /// <summary>
    /// Represents an aura (buff/debuff) on a unit.
    /// Based on HonorBuddy 3.3.5a structure.
    /// </summary>
    public class WoWAura : IEquatable<WoWAura>
    {
        #region Internal Backing Structure
        
        /// <summary>
        /// Memory layout for aura data in 3.3.5a (24 bytes total).
        /// Matches WoW's internal AURA_INFO structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
        internal struct AuraInfo
        {
            public ulong CreatorGuid;    // 0x00 - GUID of the caster
            public uint SpellId;         // 0x08 - Spell ID of the aura
            public byte Flags;           // 0x0C - AuraFlags
            public byte Level;           // 0x0D - Level at which aura was applied
            public ushort StackCount;    // 0x0E - Number of stacks (2 bytes)
            public uint Duration;        // 0x10 - Total duration in ms
            public uint EndTime;         // 0x14 - Performance counter when aura ends
            
            public override string ToString()
            {
                return $"CreatorGuid: 0x{CreatorGuid:X16}, SpellId={SpellId}, Flags={Flags}, Level={Level}, Duration={Duration}, StackCount={StackCount}, EndTime={EndTime}";
            }
        }
        
        #endregion
        
        #region AuraFlags Enum
        
        /// <summary>
        /// Flags indicating the state and behavior of an aura.
        /// </summary>
        [Flags]
        public enum AuraFlags : byte
        {
            /// <summary>No flags set.</summary>
            None = 0,
            /// <summary>First effect of the aura is active.</summary>
            FirstEffect = 1,
            /// <summary>Second effect of the aura is active.</summary>
            SecondEffect = 2,
            /// <summary>Third effect of the aura is active.</summary>
            ThirdEffect = 4,
            /// <summary>Any effect of the aura is active (FirstEffect | SecondEffect | ThirdEffect).</summary>
            AnyEffectActive = 7,
            /// <summary>The aura has no caster (NPC buff, item buff, etc.).</summary>
            NoCaster = 8,
            /// <summary>The aura can be cancelled by the player.</summary>
            Cancellable = 16,
            /// <summary>The aura has no duration (permanent until cancelled/removed).</summary>
            NoDuration = 32,
            /// <summary>Unknown flag (often indicates passive).</summary>
            Unknown = 64,
            /// <summary>The aura is harmful (debuff).</summary>
            Harmful = 128
        }
        
        #endregion
        
        #region Fields
        
        private readonly AuraInfo _data;
        private WoWSpell? _spell;
        
        #endregion
        
        #region Constructors
        
        /// <summary>
        /// Creates a new WoWAura from backing data.
        /// </summary>
        internal WoWAura(AuraInfo data)
        {
            _data = data;
        }
        
        /// <summary>
        /// Creates a new WoWAura from individual values.
        /// </summary>
        public WoWAura(int spellId, ulong creatorGuid, AuraFlags flags, ushort stackCount, byte level, uint duration, uint endTime)
        {
            _data = new AuraInfo
            {
                SpellId = (uint)spellId,
                CreatorGuid = creatorGuid,
                Flags = (byte)flags,
                Level = level,
                StackCount = stackCount,
                Duration = duration,
                EndTime = endTime
            };
        }
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Gets the spell ID of this aura.
        /// </summary>
        public int SpellId => (int)_data.SpellId;
        
        /// <summary>
        /// Gets the GUID of the unit that created this aura.
        /// </summary>
        public ulong CreatorGuid => _data.CreatorGuid;
        
        /// <summary>
        /// Gets the flags for this aura.
        /// </summary>
        public AuraFlags Flags => (AuraFlags)_data.Flags;
        
        /// <summary>
        /// Gets the current stack count of this aura.
        /// </summary>
        public uint StackCount => _data.StackCount;
        
        /// <summary>
        /// Gets the level at which this aura was applied.
        /// </summary>
        public int Level => _data.Level;
        
        /// <summary>
        /// Gets the total duration of this aura in milliseconds.
        /// </summary>
        public uint Duration => _data.Duration;
        
        /// <summary>
        /// Gets the performance counter value when this aura will end.
        /// </summary>
        public uint EndTime => _data.EndTime;
        
        /// <summary>
        /// Gets the time remaining on this aura.
        /// </summary>
        public TimeSpan TimeLeft
        {
            get
            {
                if (HasNoDuration)
                    return TimeSpan.MaxValue;
                
                // Use WoWClient.PerformanceCounter() for consistency with HB 4.3.4
                ulong currentTime = StyxWoW.WoWClient.PerformanceCounter();
                if (currentTime == 0 || EndTime <= currentTime)
                    return TimeSpan.Zero;
                
                uint remaining = EndTime - (uint)currentTime;
                return TimeSpan.FromMilliseconds(remaining);
            }
        }
        
        /// <summary>
        /// Gets the time remaining in milliseconds.
        /// </summary>
        public uint TimeLeftMs
        {
            get
            {
                if (HasNoDuration)
                    return uint.MaxValue;
                
                ulong currentTime = StyxWoW.WoWClient.PerformanceCounter();
                if (currentTime == 0 || EndTime <= currentTime)
                    return 0;
                
                return EndTime - (uint)currentTime;
            }
        }
        
        /// <summary>
        /// Gets whether this aura is harmful (a debuff).
        /// </summary>
        public bool IsHarmful => (Flags & AuraFlags.Harmful) != 0;
        
        /// <summary>
        /// Gets whether this aura has any active effect.
        /// </summary>
        public bool IsActive => (Flags & AuraFlags.AnyEffectActive) != 0;
        
        /// <summary>
        /// Gets whether this aura is passive (Unknown flag, commonly used).
        /// </summary>
        public bool IsPassive => (Flags & AuraFlags.Unknown) != 0;
        
        /// <summary>
        /// Gets whether this aura can be cancelled by the player.
        /// </summary>
        public bool Cancellable => (Flags & AuraFlags.Cancellable) != 0;
        
        /// <summary>
        /// Gets whether this aura has no duration (permanent).
        /// </summary>
        public bool HasNoDuration => (Flags & AuraFlags.NoDuration) != 0;
        
        /// <summary>
        /// Gets whether this aura has a known caster.
        /// </summary>
        public bool HasCaster => (Flags & AuraFlags.NoCaster) == 0;
        
        /// <summary>
        /// Gets the name of the spell associated with this aura.
        /// </summary>
        public string Name => Spell?.Name ?? $"Spell_{SpellId}";
        
        /// <summary>
        /// Gets the rank of the spell associated with this aura.
        /// </summary>
        public string Rank => Spell?.Rank ?? string.Empty;
        
        /// <summary>
        /// Gets the spell associated with this aura.
        /// </summary>
        public WoWSpell? Spell
        {
            get
            {
                if (_spell == null && SpellId > 0)
                {
                    _spell = WoWSpell.FromId(SpellId);
                }
                return _spell;
            }
        }
        
        /// <summary>
        /// Gets the apply aura type from the first effect of the spell.
        /// </summary>
        public WoWApplyAuraType ApplyAuraType
        {
            get
            {
                var spell = Spell;
                if (spell == null)
                    return WoWApplyAuraType.None;
                // Get from first spell effect
                var effect = spell.SpellEffect1;
                return effect?.AuraType ?? WoWApplyAuraType.None;
            }
        }
        
        #endregion
        
        #region Methods
        
        /// <summary>
        /// Attempts to cancel this aura (if it's cancellable).
        /// Only works for auras on the player.
        /// </summary>
        /// <returns>True if the aura was cancelled, false otherwise.</returns>
        public bool TryCancel()
        {
            if (!Cancellable || IsHarmful)
                return false;
            
            // Verify this aura belongs to the player
            var me = ObjectManager.Me;
            if (me == null || CreatorGuid != me.Guid)
                return false;
            
            try
            {
                // Use index-based cancellation to avoid name escaping issues
                // Find the aura index by spell ID
                var auras = me.GetAllAuras();
                int index = 1; // Lua uses 1-based indexing
                foreach (var aura in auras)
                {
                    if (aura.SpellId == SpellId)
                    {
                        WoWInternals.Lua.DoString($"CancelUnitBuff(\"player\", {index})");
                        Helpers.Logging.WriteDebug("[WoWAura] Cancelled aura: {0} (index {1})", Name, index);
                        return true;
                    }
                    index++;
                }
                
                // Fallback to name-based cancellation (escape quotes)
                string safeName = Name.Replace("\"", "\\\"");
                WoWInternals.Lua.DoString($"CancelUnitBuff(\"player\", \"{safeName}\")");
                Helpers.Logging.WriteDebug("[WoWAura] Cancelled aura: {0}", Name);
                return true;
            }
            catch (Exception ex)
            {
                Helpers.Logging.WriteException(ex);
                return false;
            }
        }
        
        /// <summary>
        /// Returns a string representation of this aura.
        /// </summary>
        public override string ToString()
        {
            return $"{Name} (SpellId: {SpellId}, Stacks: {StackCount}, TimeLeft: {TimeLeft.TotalSeconds:F1}s)";
        }
        
        #endregion
        
        #region Equality
        
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(WoWAura)) return false;
            return Equals((WoWAura)obj);
        }
        
        public bool Equals(WoWAura? other)
        {
            if (other is null) return false;
            return SpellId == other.SpellId;
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = SpellId.GetHashCode();
                hash = (hash * 397) ^ (Name?.GetHashCode() ?? 0);
                hash = (hash * 397) ^ (Rank?.GetHashCode() ?? 0);
                return hash;
            }
        }
        
        public static bool operator ==(WoWAura? left, WoWAura? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;
            return left.SpellId == right.SpellId;
        }
        
        public static bool operator !=(WoWAura? left, WoWAura? right)
        {
            return !(left == right);
        }
        
        #endregion
        
        #region Factory Methods
        
        /// <summary>
        /// Creates a WoWAura from a memory address.
        /// </summary>
        internal static WoWAura? FromAddress(uint address)
        {
            var memory = ObjectManager.Wow;
            if (address == 0 || memory == null)
                return null;
            
            try
            {
                var data = memory.Read<AuraInfo>(address);
                if (data.SpellId == 0)
                    return null;
                
                return new WoWAura(data);
            }
            catch
            {
                return null;
            }
        }
        
        #endregion
    }
}
