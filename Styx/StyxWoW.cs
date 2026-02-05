using System;
using System.Diagnostics;
using Styx.Logic.AreaManagement;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Combat;
using Styx.WoWInternals;
using Styx.WoWInternals.Misc;
using Styx.WoWInternals.WoWCache;
using Styx.WoWInternals.WoWObjects;

namespace Styx
{
    public static class StyxWoW
    {
        #region Private Fields

        private static WoWCache? _cache;
        private static WoWDb? _db;
        private static AreaManager? _areaManager;
        private static WoWClient? _woWClient;

        #endregion

        #region Properties

        public static WoWDb Db
        {
            get
            {
                if (_db == null)
                    _db = new WoWDb();
                return _db;
            }
        }

        public static WoWCache Cache
        {
            get
            {
                if (_cache == null)
                    _cache = new WoWCache();
                return _cache;
            }
        }

        public static AreaManager AreaManager
        {
            get
            {
                if (_areaManager == null)
                    _areaManager = new AreaManager();
                return _areaManager;
            }
        }

        public static LocalPlayer? Me => ObjectManager.Me;

        public static WoWClient WoWClient
        {
            get
            {
                if (_woWClient == null)
                    _woWClient = new WoWClient();
                return _woWClient;
            }
        }

        public static bool IsInGame => ObjectManager.IsInGame;

        [Obsolete("Use TreeRoot.StatusText instead. This property has been deprecated, and will be removed in a future release.")]
        public static string StatusText
        {
            get => TreeRoot.StatusText;
            set => TreeRoot.StatusText = value;
        }

        public static Version GameVersion
        {
            get
            {
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(ObjectManager.WoWProcess.MainModule.FileName);
                return new Version(fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart, fileVersionInfo.FileBuildPart, fileVersionInfo.FilePrivatePart);
            }
        }

        public static uint LastHardwareAction
        {
            get => ObjectManager.Wow.Read<uint>(0x00B499A4); // 3.3.5a offset: 11835812 = 0x00B499A4
            private set => ObjectManager.Wow.Write<uint>(0x00B499A4, value);
        }

        public static bool GlobalCooldown => SpellManager.GlobalCooldown;

        #endregion

        #region Methods

        public static void SleepForLagDuration()
        {
            System.Threading.Thread.Sleep((int)(WoWClient.Latency * 2 + 50));
        }

        public static void ResetAfk()
        {
            uint currentCounter = (uint)WoWClient.PerformanceCounter();
            uint lastHardwareAction = LastHardwareAction;
            if (currentCounter >= lastHardwareAction)
            {
                LastHardwareAction = currentCounter;
            }
        }

        public static void Reset()
        {
            _cache = null;
            _db = null;
            _areaManager = null;
            _woWClient = null;
        }

        #endregion

        /// <summary>
        /// Offsets class - mimics HB 4.3.4 Class493 for compatibility
        /// </summary>
        public static class Offsets
        {
            /// <summary>
            /// Maps offset index to actual WotLK 3.3.5a offset value
            /// </summary>
            public static uint GetOffsetByIndex(int index)
            {
                // For WotLK 3.3.5a, most offsets are direct struct member offsets
                switch (index)
                {
                    case 5057: return 0x0BD0; // Pet spells array offset in LocalPlayer struct
                    case 5005: return 0x00ACFDF4; // COMPLETED_QUEST_LIST_HEAD (11337204U from HB 3.3.5a)
                    case 2833: return Styx.Offsets.GlobalOffsets.CGWorldFrame_Intersect; // CGWorldFrame::Intersect function
                    default: return 0; // Unknown offset
                }
            }
        }
    }
}
