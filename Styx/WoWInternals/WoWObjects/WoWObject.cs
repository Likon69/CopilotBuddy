using System;
using System.Threading;
using GreenMagic;
using Styx.Helpers;
using Styx.Logic.Pathing;

namespace Styx.WoWInternals.WoWObjects
{
    public class WoWObject : IComparable<WoWObject>, IEquatable<WoWObject>
    {
        #region Constants - Offsets 3.3.5a
        
        // Offsets depuis BaseAddress
        private const uint TypeOffset = 0x14;           // 20 - WoWObjectType
        private const uint GuidOffset = 0x30;           // 48 - Object GUID
        private const uint ObjectFlagsOffset = 0xBC;    // 188 - Object flags
        private const uint DescriptorOffset = 0x08;     // 8 - Pointer to descriptors
        
        // Descriptor field indices (multiply by 4 for offset)
        private const uint DescGuid = 0x00;             // Descriptor GUID
        private const uint DescType = 0x02;             // Descriptor Type flags
        private const uint DescEntry = 0x03;            // Descriptor Entry ID
        
        #endregion
        
        #region Private Fields
        
        private ObjectInvalidateDelegate? _onInvalidate;
        private ulong _cachedGuid;
        private uint _cachedEntry;
        private string? _cachedName;
        
        #endregion
        
        #region Constructor
        public WoWObject(uint baseAddress)
        {
            BaseAddress = baseAddress;
        }
        
        #endregion
        
        #region Events
        public event ObjectInvalidateDelegate OnInvalidate
        {
            add
            {
                ObjectInvalidateDelegate? current = _onInvalidate;
                ObjectInvalidateDelegate? updated;
                do
                {
                    updated = current;
                    ObjectInvalidateDelegate? combined = (ObjectInvalidateDelegate?)Delegate.Combine(updated, value);
                    current = Interlocked.CompareExchange(ref _onInvalidate, combined, updated);
                }
                while (current != updated);
            }
            remove
            {
                ObjectInvalidateDelegate? current = _onInvalidate;
                ObjectInvalidateDelegate? updated;
                do
                {
                    updated = current;
                    ObjectInvalidateDelegate? removed = (ObjectInvalidateDelegate?)Delegate.Remove(updated, value);
                    current = Interlocked.CompareExchange(ref _onInvalidate, removed, updated);
                }
                while (current != updated);
            }
        }
        
        #endregion
        
        #region Core Properties
        public uint BaseAddress { get; private set; }
        public virtual ulong Guid
        {
            get
            {
                if (_cachedGuid == 0UL && BaseAddress != 0U)
                {
                    _cachedGuid = Memory?.Read<ulong>(BaseAddress + GuidOffset) ?? 0UL;
                }
                return _cachedGuid;
            }
        }
        public uint Entry
        {
            get
            {
                if (_cachedEntry == 0U && BaseAddress != 0U)
                {
                    _cachedEntry = ReadDescriptor<uint>(DescEntry);
                }
                return _cachedEntry;
            }
        }
        public virtual WoWObjectType Type
        {
            get
            {
                if (BaseAddress == 0U) return WoWObjectType.None;
                return (WoWObjectType)(Memory?.Read<uint>(BaseAddress + TypeOffset) ?? 0U);
            }
        }
        public WoWObjectTypeFlag TypeFlags
        {
            get
            {
                return (WoWObjectTypeFlag)ReadDescriptor<uint>(DescType);
            }
        }
        public uint ObjectFlags
        {
            get
            {
                if (BaseAddress == 0U) return 0U;
                return Memory?.Read<uint>(BaseAddress + ObjectFlagsOffset) ?? 0U;
            }
        }
        public virtual ulong DescriptorGuid
        {
            get
            {
                return ReadDescriptor<ulong>(DescGuid);
            }
        }
        public QuestGiverStatus QuestGiverStatus
        {
            get
            {
                if (BaseAddress == 0U) return QuestGiverStatus.None;
                return (QuestGiverStatus)(Memory?.Read<uint>(BaseAddress + 144U) ?? 0U);
            }
        }
        public WoWInteractType InteractType
        {
            get
            {
                if (BaseAddress == 0U) return WoWInteractType.None;
                return (WoWInteractType)(Memory?.Read<uint>(BaseAddress + 148U) ?? 0U);
            }
        }
        
        #endregion
        
        #region Validity Properties
        public bool IsValid
        {
            get
            {
                if (BaseAddress == 0U) return false;
                if (IsDisabled) return false;
                
                // Basic check - the address must be readable
                try
                {
                    if (Memory == null) return false;
                    // Try to read the type to verify the address is valid
                    uint type = Memory.Read<uint>(BaseAddress + TypeOffset);
                    return type > 0 && type <= 10; // Types valides: 1-10
                }
                catch
                {
                    return false;
                }
            }
        }
        public bool IsDisabled
        {
            get
            {
                return (ObjectFlags & 0x10000U) != 0U; // 65536
            }
        }
        public bool IsMe
        {
            get
            {
                return Guid == ObjectManager.LocalGuid;
            }
        }
        
        #endregion
        
        #region Position Properties (Virtual - Override in derived classes)
        public virtual float X => 0f;
        public virtual float Y => 0f;
        public virtual float Z => 0f;
        public virtual float Rotation => 0f;
        public float RotationDegrees => Rotation * (180f / (float)Math.PI);
        public virtual WoWPoint Location => new(X, Y, Z);
        
        /// <summary>
        /// Alias for Location. Matches HB 4.3.4 API surface used by external plugins.
        /// </summary>
        public WoWPoint WorldLocation => Location;
        
        #endregion
        
        #region Distance Properties
        public virtual double Distance
        {
            get
            {
                if (ObjectManager.Me == null) return double.MaxValue;
                return ObjectManager.Me.Location.Distance(Location);
            }
        }
        public virtual double DistanceSqr
        {
            get
            {
                if (ObjectManager.Me == null) return double.MaxValue;
                return ObjectManager.Me.Location.DistanceSqr(Location);
            }
        }
        public virtual double Distance2D
        {
            get
            {
                if (ObjectManager.Me == null) return double.MaxValue;
                return ObjectManager.Me.Location.Distance2D(Location);
            }
        }
        public virtual double Distance2DSqr
        {
            get
            {
                if (ObjectManager.Me == null) return double.MaxValue;
                return ObjectManager.Me.Location.Distance2DSqr(Location);
            }
        }
        
        #endregion
        
        #region Interaction Properties
        public virtual float InteractRange => 4f;

        /// <summary>FEAT-28: Squared interact range for faster distance checks (avoids sqrt).</summary>
        public float InteractRangeSqr
        {
            get
            {
                float range = InteractRange;
                return range * range;
            }
        }

        public bool WithinInteractRange
        {
            get
            {
                float range = InteractRange;
                return DistanceSqr < (range * range);
            }
        }
        public virtual string Name => GetObjectName();
        
        #endregion
        
        #region Location Checks
        public bool IsUnderground => Z < -500f;
        /// <summary>
        /// Whether this object is indoors. Override on LocalPlayer uses Lua for accuracy.
        /// FEAT-45: Made virtual so LocalPlayer can override.
        /// </summary>
        public virtual bool IsIndoors
        {
            get
            {
                try
                {
                    string result = Lua.GetReturnVal<string>("return IsIndoors() and '1' or '0'", 0);
                    return result == "1";
                }
                catch
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// Whether this object is outdoors. Override on LocalPlayer uses Lua for accuracy.
        /// FEAT-45: Made virtual so LocalPlayer can override.
        /// </summary>
        public virtual bool IsOutdoors => !IsIndoors;
        /// <summary>
        /// Whether this object is in line of sight from the player.
        /// Uses native CGWorldFrame::Intersect raycasting.
        /// Ported from HB 4.3.4 WoWObject — WoWUnit overrides with BoundingHeight-aware version.
        /// </summary>
        public virtual bool InLineOfSight
        {
            get
            {
                if (ObjectManager.Me == null)
                    return false;
                return World.GameWorld.IsInLineOfSight(
                    ObjectManager.Me.GetTraceLinePos(),
                    Location.Add(0f, 0f, 2.132f));
            }
        }

        /// <summary>
        /// HB 4.3.4 compat — was marked [Obsolete], just delegates to InLineOfSight.
        /// External bots (LazyRaider etc.) reference this.
        /// </summary>
        public virtual bool InLineOfSightOCD => InLineOfSight;
        
        #endregion
        
        #region Facing Methods
        public bool IsFacing(WoWObject obj)
        {
            if (obj == null) return false;
            return IsFacing(obj.Location);
        }
        public bool IsFacing(WoWPoint location)
        {
            return WoWMathHelper.IsFacing(this.Location, this.Rotation, location);
        }
        public bool IsSafelyFacing(WoWObject obj)
        {
            if (obj == null) return false;
            return WoWMathHelper.IsSafelyFacing(this.Location, this.Rotation, obj.Location);
        }
        
        public bool IsSafelyFacing(WoWObject obj, float viewDegrees)
        {
            if (obj == null) return false;
            return WoWMathHelper.IsFacing(this.Location, this.Rotation, obj.Location, WoWMathHelper.DegreesToRadians(viewDegrees));
        }
        
        public bool IsSafelyFacing(WoWPoint location)
        {
            return WoWMathHelper.IsSafelyFacing(this.Location, this.Rotation, location);
        }
        public bool IsBehind(WoWObject obj)
        {
            if (obj == null) return false;
            return WoWMathHelper.IsBehind(this.Location, obj.Location, obj.Rotation);
        }
        public bool IsSafelyBehind(WoWObject obj)
        {
            if (obj == null) return false;
            return WoWMathHelper.IsSafelyBehind(this.Location, obj.Location, obj.Rotation);
        }
        public bool MeIsBehind => ObjectManager.Me != null && ObjectManager.Me.IsBehind(this);
        public bool MeIsSafelyBehind => ObjectManager.Me != null && ObjectManager.Me.IsSafelyBehind(this);
        
        #endregion
        
        #region Interaction Methods
        
        // Interaction offset in vtable (44 * 4 = 176)
        private const uint InteractVtableOffset = 176U;
        
        // Timer to prevent spamming interactions
        private WaitTimer _interactTimer = new WaitTimer(TimeSpan.FromMilliseconds(500));
        
        public virtual void Interact()
        {
            Interact(ignoreTimer: false);
        }
        
        public virtual void Interact(bool ignoreTimer)
        {
            if (BaseAddress == 0U)
                return;
                
            if (!ignoreTimer && !_interactTimer.IsFinished)
                return;
                
            _interactTimer.Reset();
            StyxWoW.ResetAfk();
            
            Logging.WriteDebug("[Interact] Interacting with object at 0x{0:X}", BaseAddress);
            
            ExecutorRand? executor = ObjectManager.Executor;
            if (executor == null)
            {
                Logging.WriteDebug("[Interact] Invalid executor - cannot interact");
                return;
            }
            
            try
            {
                lock (executor.AssemblyLock)
                {
                    executor.Clear();
                    // mov ecx, BaseAddress (this pointer)
                    executor.AddLine("mov ecx, {0}", BaseAddress);
                    // mov eax, [ecx] (get vtable pointer)
                    executor.AddLine("mov eax, [ecx]");
                    // add eax, 176 (offset to Interact in vtable - index 44)
                    executor.AddLine("add eax, {0}", InteractVtableOffset);
                    // mov eax, [eax] (get function pointer)
                    executor.AddLine("mov eax, [eax]");
                    // call eax (call Interact)
                    executor.AddLine("call eax");
                    executor.AddLine("retn");
                    executor.Execute();
                }
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
            }
            
            Logging.WriteDebug("[Interact] Done interacting with object at 0x{0:X}", BaseAddress);
        }
        
        #endregion
        
        #region Type Conversion Methods
        public WoWUnit? ToUnit()
        {
            if (this is WoWUnit unit) return unit;
            if (Type == WoWObjectType.Unit || Type == WoWObjectType.Player)
                return new WoWUnit(BaseAddress);
            return null;
        }
        public WoWPlayer? ToPlayer()
        {
            if (this is WoWPlayer player) return player;
            if (Type == WoWObjectType.Player)
                return new WoWPlayer(BaseAddress);
            return null;
        }
        public WoWItem? ToItem()
        {
            if (this is WoWItem item) return item;
            if (Type == WoWObjectType.Item)
                return new WoWItem(BaseAddress);
            return null;
        }
        public WoWContainer? ToContainer()
        {
            if (this is WoWContainer container) return container;
            if (Type == WoWObjectType.Container)
                return new WoWContainer(BaseAddress);
            return null;
        }
        public WoWGameObject? ToGameObject()
        {
            if (this is WoWGameObject gameObject) return gameObject;
            if (Type == WoWObjectType.GameObject)
                return new WoWGameObject(BaseAddress);
            return null;
        }

        /// <summary>FEAT-28: Cast this object to WoWDynamicObject (e.g., Blizzard, Rain of Fire ground effects).</summary>
        public WoWDynamicObject? ToDynamicObject()
        {
            if (this is WoWDynamicObject dynObj) return dynObj;
            if (Type == WoWObjectType.DynamicObject)
                return new WoWDynamicObject(BaseAddress);
            return null;
        }
        
        #endregion
        
        #region Helper Methods
        public WoWPoint GetPosition() => Location;
        
        /// <summary>
        /// Gets the name of this WoW object from game memory.
        /// Uses Executor to call WoW's internal name retrieval function.
        /// Based on HB 3.3.5a WoWObject.GetObjectName() implementation.
        /// </summary>
        public string GetObjectName()
        {
            try
            {
                // Return cached name if valid
                if (_cachedName != null && _cachedName != "Unknown" && !_cachedName.StartsWith("Object_"))
                    return _cachedName;

                var executor = ObjectManager.Executor;
                if (executor == null)
                {
                    return _cachedName ?? $"Object_{Entry}";
                }

                string result;
                try
                {
                    lock (executor.AssemblyLock)
                    {
                        executor.Clear();
                        
                        // HB 3.3.5a assembly code to call WoW's name function
                        // vtable offset 216 (0xD8) for GetName in WotLK 3.3.5a
                        executor.AddLine("mov ecx, {0}", BaseAddress);
                        executor.AddLine("mov eax, [ecx]");
                        executor.AddLine("add eax, {0}", 216U);
                        executor.AddLine("mov eax, [eax]");
                        executor.AddLine("call eax");
                        executor.AddLine("retn");
                        
                        executor.Execute();
                        
                        // Disable cache while reading data produced by remote code (HB uses TemporaryCacheState here)
                        using (StyxWoW.Memory.TemporaryCacheState(false))
                        {
                            uint namePtr = executor.Memory.Read<uint>(executor.ReturnPointer);
                            result = (namePtr == 0U) ? $"Object_{Entry}" : executor.Memory.Read<string>(namePtr);
                        }
                    }
                }
                catch
                {
                    return $"Object_{Entry}";
                }

                _cachedName = result;
                return _cachedName;
            }
            catch
            {
                return $"Object_{Entry}";
            }
        }
        
        #endregion
        
        #region Internal Methods
        protected static Memory? Memory => ObjectManager.Wow;
        internal void UpdateBaseAddress(uint ptr)
        {
            BaseAddress = ptr;
            // Reset caches if the address changes
            if (ptr == 0U)
            {
                _cachedGuid = 0UL;
                _cachedEntry = 0U;
            }
        }
        internal void OnInvalidated()
        {
            _onInvalidate?.Invoke();
        }

        internal T ReadDescriptor<T>(uint field) where T : struct
        {
            if (BaseAddress == 0U)
                throw new InvalidOperationException("Cannot read descriptor on invalid object");

            field *= 4U;
            uint descriptorPtr = ObjectManager.Wow.Read<uint>(BaseAddress + 8U);
            return ObjectManager.Wow.Read<T>(descriptorPtr + field);
        }

        protected T GetDescriptorField<T>(int offset) where T : struct
        {
            if (BaseAddress == 0U || Memory == null) return default;
            
            try
            {
                uint descriptorPtr = Memory.Read<uint>(BaseAddress + DescriptorOffset);
                if (descriptorPtr == 0U) return default;
                
                return Memory.Read<T>(descriptorPtr + (uint)offset);
            }
            catch
            {
                return default;
            }
        }
        protected void WriteDescriptor<T>(uint field, T value) where T : struct
        {
            if (BaseAddress == 0U || Memory == null) return;
            
            try
            {
                uint descriptorPtr = Memory.Read<uint>(BaseAddress + DescriptorOffset);
                if (descriptorPtr == 0U) return;
                
                Memory.Write(descriptorPtr + (field * 4), value);
            }
            catch
            {
                // Ignorer les erreurs d'écriture
            }
        }
        
        #endregion
        
        #region Object Overrides
        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }
        public override string ToString()
        {
            return $"[{Type}] {Name} (GUID: {Guid:X16})";
        }
        public override bool Equals(object? obj)
        {
            if (obj is WoWObject other)
                return Equals(other);
            return false;
        }
        public bool Equals(WoWObject? other)
        {
            if (other is null) return false;
            return Guid == other.Guid;
        }
        public static bool operator ==(WoWObject? left, WoWObject? right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }
        public static bool operator !=(WoWObject? left, WoWObject? right)
        {
            return !(left == right);
        }
        public int CompareTo(WoWObject? other)
        {
            if (other is null) return 1;
            return Distance.CompareTo(other.Distance);
        }

        #endregion
    }
}
