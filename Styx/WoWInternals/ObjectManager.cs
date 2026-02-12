using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using GreenMagic;
using Styx.Helpers;
using Styx.WoWInternals.WoWObjects;

namespace Styx.WoWInternals
{
    public static class ObjectManager
    {
        #region Constants - WoW 3.3.5a Build 12340
        internal const int SupportedBuild = 12340;
        
        // Offsets ObjectManager (WoW 3.3.5a Build 12340)
        private const uint CurMgrBase = 0xC79CE0;      // 13081824U - s_curMgr base pointer
        private const uint CurMgrOffset = 0x2ED0;      // 11984U - offset to actual manager
        private const uint LocalGuidOffset = 0xC0;     // 192U - offset to local player GUID
        private const uint FirstObjectOffset = 0xAC;   // 172U - offset to first object in list
        private const uint NextObjectOffset = 0x3C;    // 60U - offset to next object
        private const uint ObjectTypeOffset = 0x14;    // 20U - offset to object type
        private const uint ObjectGuidOffset = 0x30;    // 48U - offset to object GUID
        
        // D3D/EndScene offsets
        private const uint D3DDevicePtr = 0xC5DF88;    // 12967816U - pDevicePtr_1
        private const uint D3DDeviceOffset = 0x397C;   // 14716U - pDevicePtr_2 (NOT 0x398C!)
        private const uint EndSceneVtableOffset = 0xA8; // 168U (vtable index 42) - oEndScene
        
        // IsInGame offset
        private const uint IsInGameOffset = 0xBD0792;  // 12388242U
        
        // Performance counter for aura timing
        private const uint PerformanceCounterOffset = 0x0086AE20; // 8830240
        
        #endregion
        
        #region Private Fields
        private static readonly Dictionary<ulong, WoWObject> _objectList = new();
        private static readonly Dictionary<ulong, WoWObject> _objectsToRemove = new();
        private static ObjectListUpdateFinishedDelegate? _onObjectListUpdateFinished;
        private static readonly object _updateLock = new();
        
        #endregion
        
        #region Public Properties - API Honorbuddy
        public static Process? WoWProcess { get; private set; }
        public static Memory? Wow { get; private set; }
        public static ExecutorRand? Executor { get; set; }
        public static LocalPlayer? Me { get; set; }
        public static bool IsInitialized => Wow != null;
        public static bool IsInGame
        {
            get
            {
                if (Wow == null) return false;
                try
                {
                    return Wow.Read<byte>(IsInGameOffset) != 0;
                }
                catch
                {
                    return false;
                }
            }
        }
        
        /// <summary>
        /// Gets the current performance counter from WoW.
        /// Used for calculating aura time remaining.
        /// </summary>
        public static uint PerformanceCounter
        {
            get
            {
                if (Wow == null) return 0U;
                try
                {
                    return Wow.Read<uint>(PerformanceCounterOffset);
                }
                catch
                {
                    return 0U;
                }
            }
        }
        
        public static ulong LocalGuid
        {
            get
            {
                if (Wow == null) return 0UL;
                try
                {
                    uint curMgr = CurMgr;
                    if (curMgr == 0) return 0UL;
                    return Wow.Read<ulong>(curMgr + LocalGuidOffset);
                }
                catch
                {
                    return 0UL;
                }
            }
        }
        public static List<WoWObject> ObjectList
        {
            get
            {
                lock (_updateLock)
                {
                    return _objectList.Values.ToList();
                }
            }
        }
        private static uint CurMgr
        {
            get
            {
                if (Wow == null) return 0U;
                try
                {
                    return Wow.Read<uint>(CurMgrBase, CurMgrOffset);
                }
                catch
                {
                    return 0U;
                }
            }
        }
        
        #endregion
        
        #region Events - API Honorbuddy
        public static event ObjectListUpdateFinishedDelegate OnObjectListUpdateFinished
        {
            add
            {
                ObjectListUpdateFinishedDelegate? current = _onObjectListUpdateFinished;
                ObjectListUpdateFinishedDelegate? updated;
                do
                {
                    updated = current;
                    ObjectListUpdateFinishedDelegate? combined = (ObjectListUpdateFinishedDelegate?)Delegate.Combine(updated, value);
                    current = Interlocked.CompareExchange(ref _onObjectListUpdateFinished, combined, updated);
                }
                while (current != updated);
            }
            remove
            {
                ObjectListUpdateFinishedDelegate? current = _onObjectListUpdateFinished;
                ObjectListUpdateFinishedDelegate? updated;
                do
                {
                    updated = current;
                    ObjectListUpdateFinishedDelegate? removed = (ObjectListUpdateFinishedDelegate?)Delegate.Remove(updated, value);
                    current = Interlocked.CompareExchange(ref _onObjectListUpdateFinished, removed, updated);
                }
                while (current != updated);
            }
        }
        
        #endregion
        
        #region Initialization - API Honorbuddy
        public static void Initialize(Memory memory)
        {
            if (memory == null)
                throw new ArgumentNullException(nameof(memory));
            
            Wow = memory;
            
            // Trouver le processus WoW
            WoWProcess = Process.GetProcesses()
                .FirstOrDefault(p => p.Id == memory.ProcessId);
            
            if (WoWProcess != null)
            {
                WoWProcess.EnableRaisingEvents = true;
                
                // Check the build
                try
                {
                    int build = WoWProcess.MainModule?.FileVersionInfo.FilePrivatePart ?? 0;
                    if (build != SupportedBuild)
                    {
                        Logging.Write($"[ObjectManager] Build {build} not supported (expected: {SupportedBuild})");
                        throw new Exception($"WoW build {build} not supported. Required build: {SupportedBuild}");
                    }
                    Logging.WriteDebug($"[ObjectManager] WoW build {build} detected - OK");
                }
                catch (Exception ex) when (ex is not InvalidOperationException)
                {
                    Logging.WriteDebug($"[ObjectManager] Build verification failed: {ex.Message}");
                }
            }
            
            // Hook EndScene for code execution
            HookEndscene();
            
            // First Update to populate the object list
            Update();
            
            Logging.WriteDebug("[ObjectManager] Initialized successfully");
        }
        public static bool HookEndscene(Action<string>? logger = null)
        {
            void Log(string msg)
            {
                Logging.WriteDebug(msg);
                logger?.Invoke(msg);
            }
            
            if (Wow == null)
            {
                Log("[ObjectManager] HookEndscene: Memory not initialized");
                return false;
            }
            
            // Protection: do not re-hook if already done
            if (Executor != null && Executor.IsInitialized)
            {
                Log("[ObjectManager] HookEndscene: Already hooked, skipping");
                return true;
            }
            
            try
            {
                Log($"[ObjectManager] D3DDevicePtr = 0x{D3DDevicePtr:X8}");
                
                // Step 1: Read the D3D device pointer
                uint devicePtrAddr = Wow.Read<uint>(D3DDevicePtr);
                Log($"[ObjectManager] devicePtrAddr = 0x{devicePtrAddr:X8}");
                if (devicePtrAddr == 0U)
                {
                    throw new InvalidOperationException("DevicePointer not found (D3DDevicePtr = 0)");
                }
                
                // Step 2: Read the actual device
                uint device = Wow.Read<uint>(devicePtrAddr + D3DDeviceOffset);
                Log($"[ObjectManager] device = 0x{device:X8} (at 0x{devicePtrAddr + D3DDeviceOffset:X8})");
                if (device == 0U)
                {
                    throw new InvalidOperationException("Device not found (device = 0)");
                }
                
                // Step 3: Read the vtable
                uint vtablePtr = Wow.Read<uint>(device);
                Log($"[ObjectManager] vtablePtr = 0x{vtablePtr:X8}");
                if (vtablePtr == 0U)
                {
                    throw new InvalidOperationException("VTable not found (vtable = 0)");
                }
                
                // Step 4: Read the EndScene address (vtable[42])
                uint endSceneAddr = Wow.Read<uint>(vtablePtr + EndSceneVtableOffset);
                Log($"[ObjectManager] endSceneAddr = 0x{endSceneAddr:X8} (at 0x{vtablePtr + EndSceneVtableOffset:X8})");
                if (endSceneAddr == 0U)
                {
                    throw new InvalidOperationException("EndScene not found (endScene = 0)");
                }
                
                Log($"[ObjectManager] D3D: Device=0x{device:X8}, VTable=0x{vtablePtr:X8}, EndScene=0x{endSceneAddr:X8}");
                
                // Create the Executor
                Log("[ObjectManager] Creating ExecutorRand...");
                Executor = new ExecutorRand(Wow, endSceneAddr);
                Log($"[ObjectManager] ExecutorRand created, IsInitialized = {Executor?.IsInitialized}");
                
                Log("[ObjectManager] EndScene hook succeeded");
                return true;
            }
            catch (Exception ex)
            {
                Log($"[ObjectManager] EndScene hook failed: {ex.Message}");
                Log($"[ObjectManager] Stack: {ex.StackTrace}");
                return false;
            }
        }
        
        #endregion
        
        #region Update - Core of the system
        public static void Update()
        {
            if (Wow == null)
            {
                throw new InvalidOperationException("Memory not initialized. Call Initialize() first.");
            }
            
            lock (_updateLock)
            {
                try
                {
                    // Mark all objects as potentially invalid
                    foreach (var kvp in _objectList)
                    {
                        kvp.Value.UpdateBaseAddress(0U);
                    }
                    
                    // Offsets for optimized reading (read multiple values at once)
                    // Same as HB: num=48 (guid), num2=20 (type), num3=60 (next)
                    int guidOffset = (int)ObjectGuidOffset;      // 48
                    int typeOffset = (int)ObjectTypeOffset;      // 20
                    int nextOffset = (int)NextObjectOffset;      // 60
                    
                    // Calculate the buffer size for batch reading
                    int minOffset = Math.Min(Math.Min(guidOffset, typeOffset), nextOffset);
                    int maxOffset = Math.Max(Math.Max(guidOffset + 8, typeOffset + 4), nextOffset + 4);
                    byte[] buffer = new byte[maxOffset - minOffset];
                    
                    // Ajuster les offsets relatifs au buffer
                    guidOffset -= minOffset;
                    typeOffset -= minOffset;
                    nextOffset -= minOffset;
                    
                    // Retrieve the local GUID
                    ulong localGuid = LocalGuid;
                    
                    // Retrieve the first object from the list
                    uint curMgr = CurMgr;
                    if (curMgr == 0U) return;
                    
                    uint currentObject = Wow.Read<uint>(curMgr + FirstObjectOffset);
                    
                    // Traverse the linked list
                    while (currentObject != 0U && (currentObject & 1U) == 0U)
                    {
                        // Batch reading for performance
                        Wow.ReadBytes(currentObject + (uint)minOffset, buffer);
                        
                        // Extract GUID
                        ulong objGuid = BitConverter.ToUInt64(buffer, guidOffset);
                        
                        // If it's the local player, update Me
                        if (objGuid == localGuid && Me != null)
                        {
                            Me.UpdateBaseAddress(currentObject);
                        }
                        
                        // Update or create the object
                        if (_objectList.TryGetValue(objGuid, out WoWObject? existingObj))
                        {
                            existingObj.UpdateBaseAddress(currentObject);
                        }
                        else
                        {
                            // Extract the type
                            WoWObjectType objType = (WoWObjectType)BitConverter.ToUInt32(buffer, typeOffset);
                            
                            // Create the correct object type
                            WoWObject newObj = CreateWoWObject(currentObject, objType, objGuid, localGuid);
                            _objectList.Add(objGuid, newObj);
                        }
                        
                        // Move to the next object
                        uint nextObject = BitConverter.ToUInt32(buffer, nextOffset);
                        if (nextObject == currentObject) break; // End of list
                        currentObject = nextObject;
                    }
                    
                    // Clean up invalid objects (BaseAddress = 0)
                    _objectsToRemove.Clear();
                    foreach (var kvp in _objectList)
                    {
                        if (kvp.Value.BaseAddress == 0U)
                        {
                            _objectsToRemove.Add(kvp.Key, kvp.Value);
                        }
                    }
                    
                    // Remove and notify
                    foreach (var kvp in _objectsToRemove)
                    {
                        _objectList.Remove(kvp.Key);
                        kvp.Value.OnInvalidated();
                    }
                    
                    // Trigger the end-of-update event
                    _onObjectListUpdateFinished?.Invoke(Me);
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    throw;
                }
            }
        }
        private static WoWObject CreateWoWObject(uint baseAddress, WoWObjectType? objType, ulong objGuid, ulong myGuid)
        {
            // If type not provided, read it from memory
            if (objType == null && Wow != null)
            {
                objType = (WoWObjectType)Wow.Read<uint>(baseAddress + ObjectTypeOffset);
            }
            
            switch (objType)
            {
                case WoWObjectType.Item:
                    return new WoWItem(baseAddress);
                    
                case WoWObjectType.Container:
                    return new WoWContainer(baseAddress);
                    
                case WoWObjectType.Unit:
                    return new WoWUnit(baseAddress);
                    
                case WoWObjectType.Player:
                    // If it's us, create/return LocalPlayer
                    if (objGuid == myGuid)
                    {
                        if (Me == null)
                        {
                            Me = new LocalPlayer(baseAddress);
                        }
                        else
                        {
                            Me.UpdateBaseAddress(baseAddress);
                        }
                        return Me;
                    }
                    return new WoWPlayer(baseAddress);
                    
                case WoWObjectType.GameObject:
                    return new WoWGameObject(baseAddress);
                    
                case WoWObjectType.DynamicObject:
                    return new WoWDynamicObject(baseAddress);
                    
                case WoWObjectType.Corpse:
                    return new WoWCorpse(baseAddress);
                    
                default:
                    return new WoWObject(baseAddress);
            }
        }
        
        #endregion
        
        #region Query Methods - API Honorbuddy
        public static T? GetObjectByGuid<T>(ulong guid) where T : WoWObject
        {
            if (guid == 0UL) return null;
            
            lock (_updateLock)
            {
                if (!_objectList.TryGetValue(guid, out WoWObject? obj))
                    return null;
                
                if (obj == null || obj.BaseAddress == 0U || !obj.IsValid)
                    return null;
                
                // Cast selon le type demandé
                if (typeof(T) == typeof(WoWObject))
                    return (T)obj;
                    
                if (typeof(T) == typeof(WoWUnit))
                    return obj.ToUnit() as T;
                    
                if (typeof(T) == typeof(WoWPlayer))
                    return obj.ToPlayer() as T;
                    
                if (typeof(T) == typeof(WoWGameObject))
                    return obj.ToGameObject() as T;
                    
                if (typeof(T) == typeof(WoWItem))
                    return obj.ToItem() as T;
                    
                if (typeof(T) == typeof(WoWContainer))
                    return obj.ToContainer() as T;
                
                return obj as T;
            }
        }

        /// <summary>
        /// FEAT-29: Returns an object by GUID even if not fully valid.
        /// Skips IsValid check — useful for cache lookups on recently despawned objects.
        /// </summary>
        public static T? GetAnyObjectByGuid<T>(ulong guid) where T : WoWObject
        {
            if (guid == 0UL) return null;

            lock (_updateLock)
            {
                if (!_objectList.TryGetValue(guid, out WoWObject? obj))
                    return null;

                if (obj == null || obj.BaseAddress == 0U)
                    return null;

                if (typeof(T) == typeof(WoWObject))
                    return (T)obj;
                if (typeof(T) == typeof(WoWUnit))
                    return obj.ToUnit() as T;
                if (typeof(T) == typeof(WoWPlayer))
                    return obj.ToPlayer() as T;
                if (typeof(T) == typeof(WoWGameObject))
                    return obj.ToGameObject() as T;
                if (typeof(T) == typeof(WoWItem))
                    return obj.ToItem() as T;
                if (typeof(T) == typeof(WoWContainer))
                    return obj.ToContainer() as T;
                if (typeof(T) == typeof(WoWDynamicObject))
                    return obj.ToDynamicObject() as T;

                return obj as T;
            }
        }
        public static List<T> GetObjectsOfType<T>() where T : WoWObject
        {
            return GetObjectsOfType<T>(allowInheritance: false, includeMeIfFound: false);
        }
        public static List<T> GetObjectsOfType<T>(bool allowInheritance) where T : WoWObject
        {
            return GetObjectsOfType<T>(allowInheritance, includeMeIfFound: false);
        }
        public static List<T> GetObjectsOfType<T>(bool allowInheritance, bool includeMeIfFound) where T : WoWObject
        {
            Type targetType = typeof(T);
            List<T> result = new();
            
            lock (_updateLock)
            {
                List<WoWObject> objects = _objectList.Values.ToList();
                
                foreach (WoWObject obj in objects)
                {
                    Type objType = obj.GetType();
                    
                    // Check the type
                    bool typeMatch = objType == targetType || 
                                    (allowInheritance && targetType.IsAssignableFrom(objType));
                    
                    if (!typeMatch) continue;
                    
                    // Exclure Me si demandé
                    if (!includeMeIfFound && obj == Me) continue;
                    
                    // Cast et ajouter
                    if (obj is T typed)
                    {
                        result.Add(typed);
                    }
                }
            }
            
            return result;
        }
        
        #endregion
        
        #region Internal Helpers
        internal static WoWObject? GetObjectInternal(ulong guid)
        {
            if (guid == 0UL) return null;
            
            lock (_updateLock)
            {
                _objectList.TryGetValue(guid, out WoWObject? obj);
                
                if (obj == null || obj.BaseAddress == 0U || !obj.IsValid)
                    return null;
                    
                return obj;
            }
        }
        internal static uint GetBaseAddressForGuid(ulong guid)
        {
            WoWObject? obj = GetObjectInternal(guid);
            return obj?.BaseAddress ?? 0U;
        }
        
        #endregion
    }
}
