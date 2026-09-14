using System;
using System.Collections.Generic;
using GreenMagic;
using Styx.Patchables;

namespace Styx.WoWInternals
{
    public class WoWDb
    {
        #region Fields

        private static readonly Dictionary<ClientDb, DbTable> _tables = new Dictionary<ClientDb, DbTable>();
        // Static flag: true once init succeeded. False allows retry if WoW wasn't attached on first call.
        private static bool _initialized = false;

        #endregion

        #region Constructor

        internal WoWDb()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_initialized) return;

            var wow = ObjectManager.Wow;
            if (wow == null) return;

            _initialized = true;

            try
            {
                // Base address for ClientDb_RegisterBase
                uint addr = (uint)GlobalOffsets.ClientDb_RegisterBase; // 0x6337D0
                
                // Walk through the registration table
                while (true)
                {
                    byte opcode = wow.Read<byte>(addr);
                    if (opcode == 0xC3) // RET instruction = end of list
                        break;

                    uint tableId = wow.Read<uint>(addr + 1);
                    int tablePtr = wow.Read<int>(addr + 11);
                    IntPtr pointer = new IntPtr(tablePtr + 24);
                    
                    if (!_tables.ContainsKey((ClientDb)tableId))
                    {
                        _tables.Add((ClientDb)tableId, new DbTable(pointer));
                    }
                    
                    addr += 17; // Each entry is 17 bytes
                }
                
                // Diagnostic: log what was loaded
                Helpers.Logging.WriteDebug($"[WoWDb] Loaded {_tables.Count} DBC tables");
                int logged = 0;
                foreach (var kvp in _tables)
                {
                    if (logged++ < 5)
                        Helpers.Logging.WriteDebug($"[WoWDb]   Key={(int)kvp.Key} (0x{(int)kvp.Key:X8}) Rows={kvp.Value.NumRows}");
                }
                
                // Check if Lock DBC is accessible
                var lockDb = this[ClientDb.Lock];
                Helpers.Logging.WriteDebug($"[WoWDb] Lock DBC lookup: {(lockDb != null ? $"FOUND (rows={lockDb.NumRows})" : "NOT FOUND")}");
                Helpers.Logging.WriteDebug($"[WoWDb] ClientDb.Lock enum value = {(int)ClientDb.Lock} (0x{(int)ClientDb.Lock:X8})");
            }
            catch (Exception)
            {
                // Table initialization failed - tables will be unavailable
            }
        }

        #endregion

        #region Indexer
        public DbTable? this[ClientDb db]
        {
            get
            {
                DbTable? table;
                return _tables.TryGetValue(db, out table) ? table : null;
            }
        }

        #endregion

        #region Nested Classes
        public class DbTable
        {
            private readonly IntPtr _tablePtr;
            private readonly DbTableHeader _header;

            internal DbTable(IntPtr tablePtr)
            {
                _tablePtr = tablePtr;
                var wow = ObjectManager.Wow;
                _header = wow != null ? wow.ReadStruct<DbTableHeader>((uint)tablePtr.ToInt32() - 24) : default;
            }
            public bool IsLoaded => _header.IsLoaded != 0;
            public int NumRows => _header.NumRows;
            public int MaxIndex => _header.MaxIndex;
            public int MinIndex => _header.MinIndex;
            public Row? GetRow(uint index)
            {
                var wow = ObjectManager.Wow;
                if (wow == null) return null;
                
                if (index < MinIndex || index > MaxIndex)
                    return null;

                uint offset = (uint)_header.RowArrayPtr.ToInt32();
                offset += (index - (uint)MinIndex) * 4;
                
                uint rowPtr = wow.Read<uint>(offset);
                if (rowPtr == 0)
                    return null;

                return new Row(new IntPtr(rowPtr));
            }
        }
        public class Row
        {
            private readonly IntPtr _address;
            private readonly bool _ownsMemory;

            internal Row(IntPtr address)
            {
                _address = address;
                _ownsMemory = false;
            }

            internal Row(IntPtr address, bool ownsMemory) : this(address)
            {
                _ownsMemory = ownsMemory;
            }
            public bool IsValid => _address != IntPtr.Zero;
            public T? GetField<T>(uint index)
            {
                try
                {
                    var wow = ObjectManager.Wow;
                    if (wow == null || _ownsMemory)
                        return default;

                    if (typeof(T) == typeof(string))
                    {
                        uint strPtr = wow.Read<uint>((uint)_address.ToInt32() + index * 4);
                        object result = wow.Read<string>(strPtr);
                        return (T)result;
                    }
                    
                    return wow.Read<T>((uint)_address.ToInt32() + index * 4);
                }
                catch
                {
                    return default;
                }
            }
            public void SetField<T>(uint index, T value)
            {
                var wow = ObjectManager.Wow;
                if (wow != null)
                    wow.Write((uint)_address.ToInt32() + index * 4, value);
            }
            public T GetStruct<T>() where T : struct
            {
                try
                {
                    var wow = ObjectManager.Wow;
                    if (wow == null)
                        return default;
                    return wow.ReadStruct<T>((uint)_address.ToInt32());
                }
                catch
                {
                    return default;
                }
            }
        }
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
        internal struct DbTableHeader
        {
            private readonly IntPtr _reserved0;
            public int IsLoaded;
            public int NumRows;
            public int MaxIndex;
            public int MinIndex;
            public int RecordSize;
            private readonly IntPtr _reserved1;
            public int FieldCount;
            public IntPtr RowArrayPtr;
        }

        #endregion
    }
}
