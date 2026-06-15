using System;
using GreenMagic;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.Patchables;
using Styx.WoWInternals;

namespace Styx.Logic
{
    /// <summary>
    /// Represents a landmark/POI in WoW 3.3.5a (AreaPOI)
    /// </summary>
    public class WoWLandMark
    {
        private readonly uint _baseAddress;
        private readonly WoWDb.Row? _dbRow;
        private LandmarkStruct _landmarkData;

        public WoWLandMark(uint ptr)
        {
            _baseAddress = ptr;
            
            if (IsValid)
            {
                _landmarkData = ObjectManager.Wow!.Read<LandmarkStruct>(_baseAddress);
                var db = StyxWoW.Db?[ClientDb.AreaPOI];
                _dbRow = db?.GetRow(_landmarkData.Entry);
            }
            else
            {
                _dbRow = null;
            }
        }

        // Ported from .hb 4.3.4 WoWLandMark.cs (lines 257-289 in the decompiled source).
        // HB exposes these as instance methods on WoWLandMark, using the private
        // 'uint_0' field. Our port uses '_baseAddress'. Each method simply re-wraps
        // this landmark in the BG-specific subclass.
        public ArathiBasinLandmark ToArathiBasinLandmark() => new ArathiBasinLandmark(_baseAddress);
        public EyeOfTheStormLandmark ToEyeOfTheStormLandmark() => new EyeOfTheStormLandmark(_baseAddress);
        public BattleForGilneasLandmark ToBattleForGilneasLandmark() => new BattleForGilneasLandmark(_baseAddress);
        public AlteracValleyLandmark ToAlteracValleyLandmark() => new AlteracValleyLandmark(_baseAddress);
        public IsleOfConquestLandmark ToIsleOfConquestLandmark() => new IsleOfConquestLandmark(_baseAddress);
        public StrandOfTheAncientsLandmark ToStrandOfTheAncientsLandmark() => new StrandOfTheAncientsLandmark(_baseAddress);

        public bool IsValid => _baseAddress != 0;
        
        public uint Entry => _landmarkData.Entry;

        public int NormalIcon
        {
            get => _dbRow?.GetField<int>(2) ?? 0;
            set => _dbRow?.SetField(2, value);
        }

        public WoWPoint Location => _dbRow?.GetField<WoWPoint>(13) ?? WoWPoint.Empty;

        public int MapId => _dbRow?.GetField<int>(16) ?? 0;

        public int AreaId => _dbRow?.GetField<int>(17) ?? 0;

        public Iconflags Flags => (Iconflags)(_dbRow?.GetField<uint>(16) ?? 0);

        public string Name => _dbRow?.GetField<string>(18) ?? string.Empty;

        public string Description => _dbRow?.GetField<string>(19) ?? string.Empty;

        /// <summary>
        /// Gets WorldState value for this landmark (requires native call to GetWorldState)
        /// </summary>
        public uint WorldState
        {
            get
            {
                uint stateId = _dbRow?.GetField<uint>(20) ?? 0;
                return GetWorldState(stateId);
            }
        }

        public bool ShowInBattleMap => _dbRow?.GetField<bool>(21) ?? false;

        /// <summary>
        /// Gets WorldState value by calling native function
        /// Offset: 5541136 (0x00548810) - UIParent::GetWorldState
        /// </summary>
        private static uint GetWorldState(uint stateId)
        {
            if (stateId == 0)
                return 0;

            var executor = ObjectManager.Executor;
            if (executor == null)
            {
                Logging.Write("Invalid executor in GetWorldState");
                return 0;
            }

            try
            {
                lock (executor.AssemblyLock)
                {
                    executor.Clear();
                    executor.AddLine("push {0}", stateId);
                    executor.AddLine("call {0}", 5541136); // GetWorldState offset
                    executor.AddLine("add esp, 4");
                    executor.AddLine("retn");
                    executor.Execute();
                    
                    return ObjectManager.Wow!.Read<uint>(executor.ReturnPointer);
                }
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
                return 0;
            }
        }

        public override string ToString()
        {
            return string.Format(
                "[{0} - {1}]: Entry: {2} NormalIcon:{3} Location:{4} MapId:{5} AreaId:{6} Flags:{7} WorldState:{8} ShowInBattleMap:{9}",
                Name, Description, Entry, NormalIcon, Location, MapId, AreaId, Flags, WorldState, ShowInBattleMap);
        }

        /// <summary>
        /// Internal landmark structure (20 bytes)
        /// </summary>
        private struct LandmarkStruct
        {
            public uint Entry;       // +0x00: AreaPOI entry ID
            private float _unk1;     // +0x04
            private float _unk2;     // +0x08
            private uint _unk3;      // +0x0C
            private uint _unk4;      // +0x10
        }
    }
}
