using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GreenMagic;
using Styx.Helpers;
using Styx.Patchables;
using Styx.WoWInternals.WoWObjects;

namespace Styx.WoWInternals
{
    public static class Lua
    {
        #region Private Fields

        private static AllocatedMemory? _returnBuffer;
        private static readonly Dictionary<string, string> EscapeSequences = new Dictionary<string, string>
        {
            { "\\", "\\\\" },
            { "\"", "\\\"" },
            { "'", "\\'" },
            { "\n", "\\n" },
            { "\r", "\\r" },
            { "\t", "\\t" }
        };

        #endregion

        #region Public Methods

        public static string Escape(string unescaped)
        {
            if (string.IsNullOrEmpty(unescaped))
                return unescaped;

            foreach (var kvp in EscapeSequences)
            {
                unescaped = unescaped.Replace(kvp.Key, kvp.Value);
            }
            return unescaped;
        }

        // Shared buffer for Lua return values (reused across calls like HB 3.3.5a)
        private static readonly byte[] _luaBuffer = new byte[4000];

        public static List<string> GetReturnValues(string lua)
        {
            return GetReturnValues(lua, "CopilotBuddy.lua");
        }

        /// <summary>
        /// Executes Lua and returns multiple values with script name.
        /// HB 3.3.5a exact implementation.
        /// </summary>
        public static List<string> GetReturnValues(string lua, string scriptName)
        {
            var executor = ObjectManager.Executor;
            if (executor == null)
                return new List<string>();

            var wow = ObjectManager.Wow;
            if (wow == null)
                return new List<string>();

            try
            {
                // Read Lua full state (same offset as HB 3.3.5a)
                uint fullState = wow.Read<uint>((uint)GlobalOffsets.LuaState);
                if (fullState == 0)
                    return new List<string>();

                byte[] bytes = Encoding.UTF8.GetBytes(lua);
                byte[] bytes2 = Encoding.UTF8.GetBytes(scriptName);
                List<byte> list = new List<byte>(bytes.Length + 1 + bytes2.Length + 1);
                list.AddRange(bytes);
                list.Add(0);
                list.AddRange(bytes2);
                list.Add(0);

                using (var allocatedMemory = new AllocatedMemory(list.Count))
                {
                    allocatedMemory.WriteBytes(0, list.ToArray());
                    uint address = allocatedMemory.Address;
                    uint fileNameOffset = (uint)(allocatedMemory.Address + bytes.Length + 1);

                    if (_returnBuffer == null)
                        _returnBuffer = new AllocatedMemory(4000);

                    // Clear buffer (HB 3.3.5a pattern)
                    _returnBuffer.WriteBytes(0, _luaBuffer);

                    lock (executor.AssemblyLock)
                    {
                        executor.Clear();

                        // HB 3.3.5a exact ASM sequence:
                        // 1. lua_gettop to get current stack position
                        executor.AddLine("push {0}", fullState);
                        executor.AddLine("call {0}", (uint)GlobalOffsets.FrameScript_GetTop);
                        executor.AddLine("add esp, 0x4");
                        executor.AddLine("mov ebx, eax");   // ebx = old top
                        executor.AddLine("push ebx");       // save ebx on stack

                        // 2. luaL_loadbuffer
                        executor.AddLine("push {0}", fileNameOffset);
                        executor.AddLine("push {0}", lua.Length);
                        executor.AddLine("push {0}", address);
                        executor.AddLine("push {0}", fullState);
                        executor.AddLine("call {0}", (uint)GlobalOffsets.FrameScript_Load);
                        executor.AddLine("add esp, 0x10");
                        executor.AddLine("test eax, eax");
                        executor.AddLine("jnz @Finally");

                        // 3. lua_pcall with error handler at -2
                        executor.AddLine("push {0}", -2);
                        executor.AddLine("push {0}", -1);
                        executor.AddLine("push {0}", 0);
                        executor.AddLine("push {0}", fullState);
                        executor.AddLine("call {0}", (uint)GlobalOffsets.FrameScript_PCall);
                        executor.AddLine("add esp, 0x10");
                        executor.AddLine("test eax, eax");
                        executor.AddLine("jnz @Finally");

                        // 4. Get new top, calculate return count
                        executor.AddLine("push {0}", fullState);
                        executor.AddLine("call {0}", (uint)GlobalOffsets.FrameScript_GetTop);
                        executor.AddLine("add esp, 0x4");
                        executor.AddLine("cmp eax, ebx");
                        executor.AddLine("jle @FailNoRetValues");
                        executor.AddLine("sub eax, ebx");

                        // 5. Store count in return buffer
                        executor.AddLine("mov ecx, {0}", _returnBuffer.Address);
                        executor.AddLine("mov [ecx], eax");
                        executor.AddLine("add eax, ebx");   // eax = new top (for loop comparison)

                        // 6. Loop to read each return value
                        executor.AddLine("@LoopStart:");
                        executor.AddLine("add ecx, 0x4");
                        executor.AddLine("inc ebx");
                        executor.AddLine("push eax");       // save eax
                        executor.AddLine("push ecx");       // save ecx

                        // lua_tolstring(pState, index, NULL)
                        executor.AddLine("push {0}", 0);
                        executor.AddLine("push ebx");
                        executor.AddLine("push {0}", fullState);
                        executor.AddLine("call {0}", (uint)GlobalOffsets.FrameScript_ToLString);
                        executor.AddLine("add esp, 0xC");

                        executor.AddLine("pop ecx");        // restore ecx
                        executor.AddLine("mov [ecx], eax"); // store string pointer
                        executor.AddLine("pop eax");        // restore eax
                        executor.AddLine("cmp ebx, eax");
                        executor.AddLine("jl @LoopStart");

                        // Success - return 0
                        executor.AddLine("mov eax, 0");
                        executor.AddLine("jmp @Finally");

                        // No return values
                        executor.AddLine("@FailNoRetValues:");
                        executor.AddLine("mov eax, -1");
                        executor.AddLine("jmp @Finally");

                        // Cleanup: restore Lua stack with lua_settop
                        executor.AddLine("@Finally:");
                        executor.AddLine("pop ebx");        // restore ebx (old top)
                        executor.AddLine("push eax");       // save result
                        executor.AddLine("push ebx");
                        executor.AddLine("push {0}", fullState);
                        executor.AddLine("call {0}", (uint)GlobalOffsets.FrameScript__SetTop);
                        executor.AddLine("add esp, 0x8");
                        executor.AddLine("pop eax");        // restore result
                        executor.AddLine("retn");

                        executor.Execute();
                    }

                    // Read result from executor
                    int luaStatus = executor.Memory.Read<int>(executor.ReturnPointer);
                    if (luaStatus == 0)
                    {
                        // Success - read return values
                        int resultCount = _returnBuffer.Read<int>(0);
                        var results = new List<string>(resultCount);
                        for (int i = 0; i < resultCount; i++)
                        {
                            uint strPtr = _returnBuffer.Read<uint>((i + 1) * 4);
                            results.Add(executor.Memory.ReadString(strPtr));
                        }
                        return results;
                    }
                    else if (luaStatus < 0)
                    {
                        // No return values
                        return new List<string>();
                    }
                    else
                    {
                        Logging.WriteDebug("Lua failed! Status: {0}", luaStatus);
                        return new List<string>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.WriteDebug("Exception in GetReturnValues: {0}", ex.Message);
                return new List<string>();
            }
        }

        [Obsolete("Use GetReturnValues instead. They do the same.")]
        public static List<string> LuaGetReturnValue(string lua, string scriptName)
        {
            return GetReturnValues(lua, scriptName);
        }

        public static T GetReturnVal<T>(string lua, int retVal)
        {
            return GetReturnVal<T>(lua, (uint)retVal);
        }

        public static T GetReturnVal<T>(string lua, uint retVal)
        {
            try
            {
                var returnValues = GetReturnValues(lua);

                if (retVal >= returnValues.Count)
                    return default(T)!;

                string value = returnValues[(int)retVal];

                // Handle special types
                if (typeof(T) == typeof(bool))
                {
                    // In Lua, nil and false are false, everything else is true
                    bool result = !string.IsNullOrEmpty(value) &&
                                  !value.Equals("nil", StringComparison.OrdinalIgnoreCase) &&
                                  !value.Equals("false", StringComparison.OrdinalIgnoreCase) &&
                                  !value.Equals("0", StringComparison.OrdinalIgnoreCase);
                    return (T)(object)result;
                }

                if (string.IsNullOrEmpty(value) || value.Equals("nil", StringComparison.OrdinalIgnoreCase))
                    return default(T)!;

                // Convert to target type
                return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Logging.WriteDebug("Exception in GetReturnVal<{0}>: {1}", typeof(T).Name, ex.Message);
                return default(T)!;
            }
        }

        public static void DoString(string lua)
        {
            DoString(lua, "CopilotBuddy", 0);
        }

        public static void DoString(string lua, string luaFile, uint pState)
        {
            var executor = ObjectManager.Executor;
            if (executor == null)
                return;

            var wow = ObjectManager.Wow;
            if (wow == null)
                return;

            try
            {
                // Get Lua state if not provided
                if (pState == 0)
                    pState = wow.Read<uint>((uint)GlobalOffsets.LuaState);

                if (pState == 0)
                    return;

                // Encode script and name
                byte[] luaBytes = Encoding.UTF8.GetBytes(lua);
                byte[] nameBytes = Encoding.UTF8.GetBytes(luaFile);

                var scriptMemory = new AllocatedMemory(luaBytes.Length + nameBytes.Length + 2);
                try
                {
                    scriptMemory.WriteBytes(0, luaBytes);
                    scriptMemory.WriteByte(luaBytes.Length, 0);
                    scriptMemory.WriteBytes(luaBytes.Length + 1, nameBytes);
                    scriptMemory.WriteByte(luaBytes.Length + 1 + nameBytes.Length, 0);

                    uint luaPtr = scriptMemory.Address;
                    uint namePtr = (uint)(scriptMemory.Address + luaBytes.Length + 1);

                    lock (executor.AssemblyLock)
                    {
                        executor.Clear();

                        // luaL_loadbuffer(pState, lua, len, name)
                        executor.AddLine("push {0}", namePtr);
                        executor.AddLine("push {0}", luaBytes.Length);
                        executor.AddLine("push {0}", luaPtr);
                        executor.AddLine("push {0}", pState);
                        executor.AddLine("call {0}", (uint)GlobalOffsets.FrameScript_Load);
                        executor.AddLine("add esp, 16");

                        // lua_pcall(pState, 0, 0, 0)
                        executor.AddLine("push 0");
                        executor.AddLine("push 0");
                        executor.AddLine("push 0");
                        executor.AddLine("push {0}", pState);
                        executor.AddLine("call {0}", (uint)GlobalOffsets.FrameScript_PCall);
                        executor.AddLine("add esp, 16");

                        executor.AddLine("retn");
                        executor.Execute();
                    }
                }
                finally
                {
                    scriptMemory?.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logging.WriteDebug("Exception in DoString: {0} StackTrace:{1}", ex.Message, ex.StackTrace);
            }
        }

        public static void DoString(string szLua, string szLuaFile)
        {
            DoString(szLua, szLuaFile, 0);
        }

        public static void DoString(string format, params object[] args)
        {
            DoString(string.Format(format, args), "CopilotBuddy");
        }

        public static int GetTop(uint pState)
        {
            var executor = ObjectManager.Executor;
            if (executor == null)
                return 0;

            lock (executor.AssemblyLock)
            {
                executor.Clear();
                executor.AddLine("push {0}", pState);
                executor.AddLine("call {0}", (uint)GlobalOffsets.FrameScript_GetTop);
                executor.AddLine("add esp, 4");
                executor.AddLine("retn");
                executor.Execute();

                return executor.Memory.Read<int>(executor.ReturnPointer);
            }
        }

        public static void ShowLuaStack(uint pState)
        {
            int top = GetTop(pState);
            for (int i = 1; i <= top; i++)
            {
                string value = ToLString(pState, i, 0);
                Logging.WriteDebug("Stack[{0}]: {1}", i, value);
            }
        }

        public static string ToLString(uint pState, int index, int len)
        {
            var executor = ObjectManager.Executor;
            if (executor == null)
                return string.Empty;

            lock (executor.AssemblyLock)
            {
                executor.Clear();
                executor.AddLine("push {0}", len);
                executor.AddLine("push {0}", index);
                executor.AddLine("push {0}", pState);
                executor.AddLine("call {0}", (uint)GlobalOffsets.FrameScript_ToLString);
                executor.AddLine("add esp, 12");
                executor.AddLine("retn");
                executor.Execute();

                uint strPtr = executor.Memory.Read<uint>(executor.ReturnPointer);
                if (strPtr == 0)
                    return string.Empty;

                return ObjectManager.Wow?.Read<string>(strPtr) ?? string.Empty;
            }
        }

        public static T GetLocalizedText<T>(string szLuaVariable)
        {
            string text = GetLocalizedText(szLuaVariable, StyxWoW.Me?.BaseAddress ?? 0);
            return (T)Convert.ChangeType(text, typeof(T), CultureInfo.InvariantCulture);
        }

        public static string GetLocalizedText(string szLuaVariable)
        {
            return GetLocalizedText(szLuaVariable, StyxWoW.Me?.BaseAddress ?? 0);
        }

        public static string GetLocalizedText(string szLuaVariable, uint lpLocalPlayer)
        {
            var executor = ObjectManager.Executor;
            if (executor == null)
                return string.Empty;

            if (string.IsNullOrEmpty(szLuaVariable))
                return string.Empty;

            if (lpLocalPlayer == 0)
                return string.Empty;

            uint varPtr = 0;
            try
            {
                lock (executor.AssemblyLock)
                {
                    byte[] varBytes = Encoding.UTF8.GetBytes(szLuaVariable + "\0");
                    varPtr = executor.Memory.AllocateMemory(varBytes.Length);
                    executor.Memory.Write(varPtr, varBytes);

                    executor.Clear();
                    executor.AddLine("push -1");
                    executor.AddLine("push {0}", varPtr);
                    executor.AddLine("mov ecx, {0}", lpLocalPlayer);
                    executor.AddLine("call {0}", (uint)GlobalOffsets.FrameScript__GetLocalizedText);
                    executor.AddLine("retn");
                    executor.Execute();

                    uint resultPtr = executor.Memory.Read<uint>(executor.ReturnPointer);
                    if (resultPtr != 0)
                        return executor.Memory.Read<string>(resultPtr) ?? string.Empty;

                    return string.Empty;
                }
            }
            finally
            {
                if (varPtr != 0)
                    executor.Memory.FreeMemory(varPtr);
            }
        }

        public static int GetLocalizedInt32(string szLuaVariable, uint lpLocalPlayer)
        {
            string text = GetLocalizedText(szLuaVariable, lpLocalPlayer);
            if (string.IsNullOrEmpty(text) || text == "nil")
                return 0;
            return int.TryParse(text, out int result) ? result : 0;
        }

        public static uint GetLocalizedUInt32(string szLuaVariable, uint lpLocalPlayer)
        {
            string text = GetLocalizedText(szLuaVariable, lpLocalPlayer);
            if (string.IsNullOrEmpty(text) || text == "nil")
                return 0;
            return uint.TryParse(text, out uint result) ? result : 0;
        }

        public static long GetLocalizedInt64(string szLuaVariable, uint lpLocalPlayer)
        {
            string text = GetLocalizedText(szLuaVariable, lpLocalPlayer);
            if (string.IsNullOrEmpty(text) || text == "nil")
                return 0;
            return long.TryParse(text, out long result) ? result : 0;
        }

        public static ulong GetLocalizedUInt64(string szLuaVariable, uint lpLocalPlayer)
        {
            string text = GetLocalizedText(szLuaVariable, lpLocalPlayer);
            if (string.IsNullOrEmpty(text) || text == "nil")
                return 0;
            return ulong.TryParse(text, out ulong result) ? result : 0;
        }

        public static bool GetLocalizedBool(string szLuaVariable, uint lpLocalPlayer)
        {
            string text = GetLocalizedText(szLuaVariable, lpLocalPlayer);
            if (string.IsNullOrEmpty(text) || text == "nil")
                return false;
            return bool.TryParse(text, out bool result) && result;
        }

        public static LuaState State
        {
            get
            {
                var wow = ObjectManager.Wow;
                if (wow == null)
                    return new LuaState(0);
                return new LuaState(wow.Read<uint>((uint)GlobalOffsets.LuaState));
            }
        }

        private static LuaEvents _events;

        public static LuaEvents Events
        {
            get { return _events ??= new LuaEvents(); }
        }

        internal static void ProcessEvents()
        {
            try
            {
                Events.ProcessEvents();
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents the Lua interpreter state in WoW memory.
    /// </summary>
    public class LuaState
    {
        // Offset to globals table in LuaState structure for WotLK 3.3.5a (build 12340)
        private const uint GlobalsOffset = 72; // 0x48

        private LuaTable _cachedGlobals;

        public uint Address { get; }

        public LuaState(uint address)
        {
            Address = address;
        }

        public bool IsValid => Address != 0;

        /// <summary>
        /// Gets the global variables table from the Lua state.
        /// This allows direct memory reading of Lua tables without executing Lua code.
        /// </summary>
        public LuaTable Globals
        {
            get
            {
                if (_cachedGlobals == null && Address != 0)
                {
                    // Read the globals table from LuaState + offset
                    // The globals table is stored as a LuaTValue at this offset
                    var tvalue = new LuaTValue(Address + GlobalsOffset);
                    if (tvalue.Type == LuaType.Table)
                    {
                        _cachedGlobals = tvalue.Value.Table;
                    }
                }
                return _cachedGlobals;
            }
        }
    }
}
