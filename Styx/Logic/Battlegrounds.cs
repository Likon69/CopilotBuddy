using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Styx.Logic
{
    /// <summary>
    /// FEAT-35: Battlegrounds management for 3.3.5a WotLK.
    /// Expanded from stub — uses Lua API for queue management.
    /// Ported from HB 4.3.4 Battlegrounds.cs.
    /// </summary>
    public static class Battlegrounds
    {
        private static readonly Landmarks _landmarks = new Landmarks();
        private static DateTime _battlefieldStartTime = DateTime.MinValue;

        // Per-slot tracking of the BG type we initiated the queue for.
        // HB 4.3.4 reads the type from memory; our Lua port has to track it
        // ourselves because GetBattlefieldStatus() does not return the BG type.
        // Cleared automatically when the slot's status returns to None.
        private static readonly BattlegroundType[] _queuedSlotType = new BattlegroundType[3]; // index 1..2

        #region Properties

        /// <summary>
        /// Landmarks manager for battlegrounds.
        /// </summary>
        public static Landmarks LandMarks => _landmarks;

        /// <summary>
        /// Current battleground type based on MapId.
        /// </summary>
        public static BattlegroundType Current => GetCurrentBattleground();

        /// <summary>
        /// Whether player is inside an active battleground.
        /// Computed from queue status — not a manual setter.
        /// </summary>
        public static bool IsInsideBattleground => GetBGIndexWithStatus(BattlegroundStatus.Active) != -1;

        /// <summary>
        /// Whether bot is active in battleground mode.
        /// </summary>
        public static bool IsActive { get; set; }

        /// <summary>
        /// Whether waiting for a BG confirmation popup.
        /// </summary>
        public static bool WaitingForConfirmation => GetBGIndexWithStatus(BattlegroundStatus.Confirm) != -1;

        /// <summary>
        /// Whether the current battleground has finished.
        /// Uses Lua GetBattlefieldWinner.
        /// </summary>
        public static bool Finished
        {
            get
            {
                try
                {
                    var results = Lua.GetReturnValues("return GetBattlefieldWinner()");
                    return results != null && results.Count > 0 && !string.IsNullOrEmpty(results[0]) && results[0] != "nil";
                }
                catch { return false; }
            }
        }

        /// <summary>
        /// The winner of the current/last battleground.
        /// </summary>
        public static BattlefieldWinner Winner => GetBattlefieldWinner();

        /// <summary>
        /// The start time of the current battleground instance.
        /// </summary>
        public static DateTime BattlefieldStartTime
        {
            get
            {
                if (_battlefieldStartTime == DateTime.MinValue && IsInsideBattleground)
                    _battlefieldStartTime = DateTime.Now;
                return _battlefieldStartTime;
            }
        }

        /// <summary>
        /// How long the current BG has been running.
        /// </summary>
        public static TimeSpan BattlefieldInstanceRunTime
        {
            get
            {
                if (!IsInsideBattleground) return TimeSpan.Zero;
                try
                {
                    int ms = Lua.GetReturnVal<int>("return GetBattlefieldInstanceRunTime()", 0);
                    if (ms > 0) return TimeSpan.FromMilliseconds(ms);
                }
                catch { }
                return DateTime.Now - BattlefieldStartTime;
            }
        }

        /// <summary>
        /// Gets all BG queue statuses.
        /// </summary>
        public static List<BattlegroundStatus> BattlegroundStatuses
        {
            get
            {
                var list = new List<BattlegroundStatus>();
                foreach (var info in QueuedInfos)
                    list.Add(info.Status);
                return list;
            }
        }

        #endregion

        #region Queue Management

        /// <summary>
        /// Gets queued battleground info for all queue slots (max 2 in WotLK).
        /// Uses Lua GetBattlefieldStatus.
        /// </summary>
        public static IEnumerable<QueuedBattlegroundInfo> QueuedInfos
        {
            get
            {
                for (uint i = 1; i <= 2; i++)
                {
                    var info = GetQueuedBattlegroundInfo(i);
                    yield return info;
                }
            }
        }

        /// <summary>
        /// Gets queue info for a specific slot via Lua.
        /// Index is 1-based (WoW Lua convention).
        /// </summary>
        public static QueuedBattlegroundInfo GetQueuedBattlegroundInfo(uint index)
        {
            var info = new QueuedBattlegroundInfo();
            try
            {
                var results = Lua.GetReturnValues(
                    $"local s,m,id,lo,hi = GetBattlefieldStatus({index}); return s or 'none',id or 0,lo or 0,hi or 0");
                if (results != null && results.Count >= 4)
                {
                    string statusStr = results[0].ToLower();
                    info.Status = statusStr switch
                    {
                        "queued" => BattlegroundStatus.Queued,
                        "confirm" => BattlegroundStatus.Confirm,
                        "active" => BattlegroundStatus.Active,
                        "error" => BattlegroundStatus.Error,
                        _ => BattlegroundStatus.None
                    };
                    info.InstanceId = (uint)Lua.ParseLuaValue<int>(results[1]);
                    info.LowestLevel = (uint)Lua.ParseLuaValue<int>(results[2]);
                    info.HighestLevel = (uint)Lua.ParseLuaValue<int>(results[3]);
                }
            }
            catch { }
            return info;
        }

        /// <summary>
        /// Gets the queue status for a specific slot.
        /// </summary>
        public static BattlegroundStatus GetStatus(uint index)
        {
            if (index < 1 || index > 2)
                throw new ArgumentOutOfRangeException(nameof(index));
            return GetQueuedBattlegroundInfo(index).Status;
        }

        /// <summary>
        /// Gets the wait time for a queued battleground.
        /// </summary>
        public static TimeSpan GetQueuedBattlegroundWaitTime(uint index)
        {
            try
            {
                var info = GetQueuedBattlegroundInfo(index);
                if (info.Status != BattlegroundStatus.Queued)
                    return TimeSpan.Zero;

                int ms = Lua.GetReturnVal<int>(
                    $"local s,m,i,l,h,rm,eq,tw = GetBattlefieldStatus({index}); return tw or 0", 0);
                return TimeSpan.FromMilliseconds(ms);
            }
            catch { return TimeSpan.Zero; }
        }

        /// <summary>
        /// Finds the first queue slot with the given status.
        /// Returns -1 if not found, or the 1-based index.
        /// </summary>
        public static int GetBGIndexWithStatus(BattlegroundStatus status)
        {
            uint idx = 1;
            foreach (var info in QueuedInfos)
            {
                if (info.Status == status)
                    return (int)idx;
                idx++;
            }
            return -1;
        }

        /// <summary>
        /// Whether the player is queued for a specific BG type.
        /// HB 4.3.4 reads the type from QueuedBattlegroundInfo (memory reader).
        /// Our Lua port does not expose the BG type per slot, so we match the
        /// tracked type we initiated the queue with against the live slot status.
        /// Slot returns to None ⇒ tracked type is cleared so we don't match
        /// stale data after the queue pops or the player leaves the BG.
        /// </summary>
        public static bool IsQueuedForBattleground(BattlegroundType type)
        {
            if (type == BattlegroundType.None) return false;

            for (int slot = 1; slot <= 2; slot++)
            {
                var status = GetQueuedBattlegroundInfo((uint)slot).Status;
                if (status == BattlegroundStatus.None)
                {
                    _queuedSlotType[slot] = BattlegroundType.None;
                    continue;
                }

                if (_queuedSlotType[slot] == type)
                    return true;
            }
            return false;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Joins a battleground queue via Lua.
        /// Backward-compatible overload: defaults to slot 1.
        /// Ported from HB 4.3.4 Battlegrounds.JoinBattlefield.
        /// </summary>
        public static void JoinBattlefield(BattlegroundType type, bool asGroup = false)
        {
            JoinBattlefield(1, type, asGroup);
        }

        /// <summary>
        /// Joins a battleground queue in a specific WoW BG slot (1 or 2).
        /// Also records the BG type for that slot so IsQueuedForBattleground()
        /// can tell which BG each queued slot belongs to.
        /// </summary>
        public static void JoinBattlefield(int slot, BattlegroundType type, bool asGroup = false)
        {
            if (type == BattlegroundType.None) return;
            if (slot < 1 || slot > 2) return;

            // Update WoW's BG UI to show the requested type, then click Join.
            // GetBattlegroundInfo returns the BG id at the 5th return position;
            // when it matches our type, refresh that slot's instance info so
            // the queue button is bound to the correct BG.
            Lua.DoString(
                $"for i=1,GetNumBattlegroundTypes() do " +
                $"local _,_,_,_,id = GetBattlegroundInfo(i); " +
                $"if id == {(uint)type} then RequestBattlegroundInstanceInfo(i); end end");
            Lua.DoString($"JoinBattlefield({slot}, {(asGroup ? "true" : "false")})");
            StyxWoW.Sleep(500);

            // Record the queued type for this slot so IsQueuedForBattleground()
            // can map a queue status back to its BG type.
            _queuedSlotType[slot] = type;
        }

        /// <summary>
        /// Leaves the current battleground.
        /// </summary>
        public static void LeaveBattlefield()
        {
            Lua.DoString("LeaveBattlefield()");
            _battlefieldStartTime = DateTime.MinValue;
        }

        /// <summary>
        /// Accepts a pending BG confirmation popup.
        /// </summary>
        public static void AcceptBattlegroundConfirmation()
        {
            int idx = GetBGIndexWithStatus(BattlegroundStatus.Confirm);
            if (idx != -1)
                AcceptBattlefieldPort(idx, true);
        }

        /// <summary>
        /// Accepts or declines a battlefield port at the given index.
        /// </summary>
        public static void AcceptBattlefieldPort(int index, bool accept)
        {
            Lua.DoString($"AcceptBattlefieldPort({index},{(accept ? 1 : 0)})");
        }

        #endregion

        #region Detection

        /// <summary>
        /// Determines the current battleground type from the player's MapId.
        /// WotLK BG map IDs: AV=30, WSG=489, AB=529, EotS=566, SotA=607, IoC=628.
        /// </summary>
        public static BattlegroundType GetCurrentBattleground()
        {
            if (ObjectManager.Me == null) return BattlegroundType.None;

            return ObjectManager.Me.MapId switch
            {
                30 => BattlegroundType.AV,
                489 => BattlegroundType.WSG,
                529 => BattlegroundType.AB,
                566 => BattlegroundType.EotS,
                607 => BattlegroundType.SotA,
                628 => BattlegroundType.IoC,
                _ => BattlegroundType.None
            };
        }

        /// <summary>
        /// Gets the BG winner (Horde=0, Alliance=1).
        /// </summary>
        public static BattlefieldWinner GetBattlefieldWinner()
        {
            try
            {
                var results = Lua.GetReturnValues("return GetBattlefieldWinner()");
                if (results != null && results.Count > 0 && !string.IsNullOrEmpty(results[0]) && results[0] != "nil")
                {
                    int winner = Lua.ParseLuaValue<int>(results[0]);
                    return winner switch
                    {
                        0 => BattlefieldWinner.Horde,
                        1 => BattlefieldWinner.Alliance,
                        _ => BattlefieldWinner.None
                    };
                }
            }
            catch { }
            return BattlefieldWinner.NotFinished;
        }

        /// <summary>
        /// Gets a profile name for a BG type.
        /// </summary>
        public static string? GetProfileName(BattlegroundType type)
        {
            return type switch
            {
                BattlegroundType.AV => "AV.xml",
                BattlegroundType.WSG => "WSG.xml",
                BattlegroundType.AB => "AB.xml",
                BattlegroundType.EotS => "EotS.xml",
                BattlegroundType.SotA => "SotA.xml",
                BattlegroundType.IoC => "IoC.xml",
                _ => null
            };
        }

        #endregion
    }
}
