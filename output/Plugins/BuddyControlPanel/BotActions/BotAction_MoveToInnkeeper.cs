// Originally contributed by Chinajade.
//
// LICENSE:
// This work is licensed under the
//     Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
// also known as CC-BY-NC-SA.  To view a copy of this license, visit
//      http://creativecommons.org/licenses/by-nc-sa/3.0/
// or send a letter to
//      Creative Commons // 171 Second Street, Suite 300 // San Francisco, California, 94105, USA.

#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Buddy.Coroutines;
using BuddyControlPanel.Resources.Localization;
using CommonBehaviors.Actions;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.CommonBot.Coroutines;
using Styx.Database;
using Styx.Logic;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Pathing;
using TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

// ReSharper disable RedundantLambdaSignatureParentheses
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
#endregion


namespace BuddyControlPanel
{
    internal class BotAction_MoveToInnkeeper : IDisposable
    {
        public enum ActionOnArrival
        {
            BotStop,
            GameExit,
            Logout,
            NoPursue,
        };

        #region Creation & Destruction
        private BotAction_MoveToInnkeeper()
        {
            _behaviorTreeHook_VendorMain = CreateBehavior_VendorMain();
            TreeHooks.Instance.InsertHook(VendorHookName, 0, _behaviorTreeHook_VendorMain);
        }

        // Basic Dispose pattern (ref: https://msdn.microsoft.com/en-us/library/b1yfkh5e%28v=vs.110%29.aspx)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            // Reclaim unmanaged resources --
            TreeHooks.Instance.RemoveHook(VendorHookName, _behaviorTreeHook_VendorMain);

            s_instance = null;
        }

        public static BotAction_MoveToInnkeeper Instance
        {
            get { return s_instance ?? (s_instance = new BotAction_MoveToInnkeeper()); }
        }

        /// <summary>
        /// De-initializes the singleton instance.
        /// </summary>
        public static void ToJunkyard()
        {
            if (s_instance != null)
            {
                s_instance.Dispose();
                s_instance = null;
            }
        }
        #endregion


        #region Private data
        private const string VendorHookName = "Vendor_Main";

        // We want to force a dismount as we move final distance to Innkeeper.
        // We don't want to get stuck under a tent awning, or trying to cram a large mount through a small door.
        private const double Tunable_ForceDismountDistanceSqr = 30 * 30;

        // Delay we allow for Overlay notifications that may require additional read-time...
        private readonly TimeSpan _tunable_ImportantMessageDelay = TimeSpan.FromMilliseconds(3500);

        // If we get this close to an Innkeeper, and can't see it in the ObjectManager, then we are not in
        // the right 'phase' to use the Innkeeper.  So, we should blacklist this one, select a new one, and move on.
        private const double Tunable_InnkeeperPresenceValidateDistanceSqr = 40 * 40;

        // If we are this close to an Innkeeper, and still don't have rest bonus, then we declare 'done'.
        // Some Innkeepers actually stand in front of the tents or buildings that provide the rest bonus.
        // Examples are Westfall Brigade Encampment in Grizzly Hills, and Star's Rest in Dragonblight.
        private const double Tunable_InnkeeperMinDistanceSqr = 5 * 5;

        private readonly TimeSpan _tunable_ProgressNotifyInterval = TimeSpan.FromMilliseconds(1000);

        private readonly Composite _behaviorTreeHook_VendorMain;
        // N.B.: We must blacklist based on NpcResult vs NpcResult.Entry, since there may be multiple instances
        // of the same InnKeeper (e.g., Entry) in the database.  Due to phasing issues, the same Innkeeper may
        // appear in the database at different locations.
        private readonly List<NpcResult> _blacklistedInnkeepers = new List<NpcResult>();
        private NpcResult _innkeeperToPursue;
        private static BotAction_MoveToInnkeeper s_instance;
        private static readonly Stopwatch s_noFlyHysteresisTimer = new Stopwatch();
        private static readonly TimeSpan s_noFlyHysteresisTime = TimeSpan.FromSeconds(30);
        private Stopwatch _stopWatch_ProgressUpdate = null;
        #endregion


        #region Command & Control
        private ActionOnArrival _pursueInnkeeperViaAction = ActionOnArrival.NoPursue;
        public ActionOnArrival PursueInnkeeperViaAction
        {
            get { return _pursueInnkeeperViaAction; }
            set
            {
                _pursueInnkeeperViaAction = value;

                _blacklistedInnkeepers.Clear();
                _innkeeperToPursue = null;
                _stopWatch_ProgressUpdate = null;
            }
        }
        #endregion


        #region Main Behaviors
        private Composite CreateBehavior_VendorMain()
        {
            return new ActionRunCoroutine(ctx => Coroutine_VendorMain());
        }

        private async Task<bool> Coroutine_VendorMain()
        {
            // If we're not on mission to find Innkeeper, we're done...
            if (PursueInnkeeperViaAction == ActionOnArrival.NoPursue)
                return false;

            if (!StyxWoW.IsInGame)
                return false;

            if (StyxWoW.Me.IsDead)
                return false;

            // If we're already on vendor run, take care of that first...
            if (Vendors.ForceBuy || Vendors.ForceMail || Vendors.ForceRepair || Vendors.ForceSell || Vendors.ForceTrainer)
                return false;

            // Find innkeeper, if we don't have one...
            if (!FindInnkeeperToPursue())
            {
                PursueInnkeeperViaAction = ActionOnArrival.NoPursue;
                return false;
            }

            ShowProgressUpdate();

            // If we can 'see' the Innkeeper in the ObjectManager, take finalizing actions...
            var innKeeper = ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(o => o.Entry == _innkeeperToPursue.Entry);
            if (innKeeper != null)
                return await ExecuteArrivalActions(innKeeper);

            // We do not 'see' the Innkeeper.  If he should be close enough to see, then find a new one...
            var innkeeperDistanceSqr = Vector3.DistanceSquared(StyxWoW.Me.Location, _innkeeperToPursue.Location);
            if (innkeeperDistanceSqr <= Tunable_InnkeeperPresenceValidateDistanceSqr)
            {
                var message = string.Format(BCPGlobalization.GeneralTextFormat_InnkeeperNotPresent, _innkeeperToPursue.Name);
                PluginLog.Warning("{0}", message);
                Utility.OverlayNotification(message, Assets.ColorWarning, _tunable_ImportantMessageDelay);

                _blacklistedInnkeepers.Add(_innkeeperToPursue);
                _innkeeperToPursue = null;  // forces find of a new Innkeeper
                return true;
            }

            // Otherwise, move closer to Innkeeper's presumed location...
            await ExecuteMovementActions(_innkeeperToPursue);
            return true;
        }
        #endregion


        #region Helpers
        private async Task<bool> ExecuteArrivalActions(WoWUnit innkeeper)
        {
            Contract.Requires(innkeeper != null, () => "innkeeper != null");

            // If we are at minimum distance to innkeeper, we're done whether or not we're getting rest bonus...
            if (innkeeper.DistanceSqr <= Tunable_InnkeeperMinDistanceSqr)
            {
                WoWMovement.MoveStop();
                await FinishingAction();
                PursueInnkeeperViaAction = ActionOnArrival.NoPursue;
                return false;
            }

            // If we are flying, force a landing (we will be using ground-travel from here on out)...
            if (StyxWoW.Me.IsFlying)
            {
                await CommonCoroutines.LandAndDismount(string.Format("{0} presence confirmed.", innkeeper.Name));
                return true;
            }

            // Dismount before we near Innkeeper...
            // This will prevent us from getting stuck in doorways, under tent awnings, etc.
            if (StyxWoW.Me.Mounted && (innkeeper.DistanceSqr < Tunable_ForceDismountDistanceSqr))
            {
                await CommonCoroutines.LandAndDismount(string.Format("Approaching {0}.", innkeeper.Name));
                return true;
            }

            Navigator.MoveTo(_innkeeperToPursue.Location);
            return true;
        }


        private async Task<bool> ExecuteMovementActions(NpcResult npcResult)
        {
            var destinationLocation = npcResult.Location;
            var isFlightorConfused = FlightorMonitor.IsFlightorConfused();

            // If we've arrived at destination, we're done...
            if (Navigator.AtLocation(destinationLocation))
                return false;

            // Do we need to quit flying for a bit?
            if (isFlightorConfused || IsFlyingOverDestination(destinationLocation))
                s_noFlyHysteresisTimer.Restart();

            // If we're not allowed to fly yet, continue on ground...
            if (s_noFlyHysteresisTimer.IsRunning && (s_noFlyHysteresisTimer.Elapsed < s_noFlyHysteresisTime))
            {
                if (StyxWoW.Me.IsFlying)
                {
                    // Try to recover from Flightor confusion by landing...
                    // We will continue a little way on the ground, before attempting to fly again.
                    await CommonCoroutines.LandAndDismount("Using ground-based navigation for a bit.");
                }

                // Continue on ground until for a bit, until we're out of confusion-causing situation...
                var runStatus = Navigator.MoveTo(destinationLocation);
                if (!runStatus.IsSuccessful())
                {
                    var message = string.Format(BCPGlobalization.GeneralTextFormat_InnkeeperNotNavigable, _innkeeperToPursue.Name);
                    PluginLog.Warning("{0}", message);
                    Utility.OverlayNotification(message, Assets.ColorWarning, _tunable_ImportantMessageDelay);

                    // Blacklist selected innkeeper, and find another...
                    _blacklistedInnkeepers.Add(npcResult);
                    _innkeeperToPursue = null;
                }
                return true;
            }

            // Flightor is not confused, continue normally...
            Flightor.MoveTo(destinationLocation);
            s_noFlyHysteresisTimer.Reset();
            return true;
        }


        private class QueryFilter_FindBestInnkeeper
        {
            // This filter will bias towards Innkeepers with a shorter surface path distance
            // (vs geometric distance).  This will make a significant difference in areas where
            // we have access to ground-travel only.  If we choose a slightly sub-optimal Innkeep
            // in areas where we can fly, the mount speed difference will make it unnoticable.
            public QueryFilter_FindBestInnkeeper(List<NpcResult> blacklistedInnkeepers)
            {
                Contract.Requires(blacklistedInnkeepers != null, () => "blacklistedInnkeepers may not be null.");

                BlacklistedInnkeepers = blacklistedInnkeepers;
                BestInnkeeper = null;
                BestInnkeeperDistanceSqr = float.MaxValue;
            }

            public NpcResult BestInnkeeper { get; private set; }
            private float BestInnkeeperDistanceSqr { get; set; }
            private List<NpcResult> BlacklistedInnkeepers { get; set; }

            public bool Filter(NpcResult npcResult)
            {
                Contract.Requires(npcResult != null, () => "npcResult != null");

                // N.B.: This method always returns 'false', since we want to evaluate all the
                // innkeepers in the database.
                // N.B.: There may be multiple instances of the same Innkeep in the database, based on phasing.
                var myLocation = StyxWoW.Me.Location;

                // If Innkeep is blacklisted, it is not a candidate...
                if (BlacklistedInnkeepers.Contains(npcResult))
                    return false;

                if (Databases.ConditionallyBlacklistedInnkeepers.Any(conditionalBlacklistFunc => conditionalBlacklistFunc(npcResult)))
                    return false;

                // If Innkeep is not faction compatible, we're done...
                if (npcResult.IsHostile)
                    return false;

                // If we do yet have a candidate, we do now...
                if (BestInnkeeper == null)
                {
                    BestInnkeeper = npcResult;
                    BestInnkeeperDistanceSqr = npcResult.Location.DistanceSquared(myLocation);
                    return false;
                }

                // If candidate is geometrically further than our current surface path distance, its no good...
                // N.B.: The geometric distance test is cheap to make to rule out non-viable candidates.
                var distSqr = Vector3.DistanceSquared(myLocation, npcResult.Location);
                if (distSqr > BestInnkeeperDistanceSqr)
                    return false;

                BestInnkeeper = npcResult;
                BestInnkeeperDistanceSqr = distSqr;
                return false;
            }
        }


        // Returns true, if we've a viable innkeeper to pursue; otherwise, false.
        private bool FindInnkeeperToPursue()
        {
            if (_innkeeperToPursue != null)
                return true;

            PluginLog.DeveloperInfo("{0}", BCPGlobalization.GeneralText_LocatingInnkeeper);
            Utility.OverlayNotification(BCPGlobalization.GeneralText_LocatingInnkeeper,
                Assets.ColorInformation, _tunable_ImportantMessageDelay);

            var filter = new QueryFilter_FindBestInnkeeper(_blacklistedInnkeepers);
            var myLocation = StyxWoW.Me.Location;
            foreach (var mapId in Utility.ActiveMapIds)
            {
                Query.GetNearestNpc(mapId,
                    myLocation,
                    UnitNPCFlags.Innkeeper,
                    filter.Filter
                    );
            }

            _innkeeperToPursue = filter.BestInnkeeper;

            string message;
            if (_innkeeperToPursue == null)
            {
                message = string.Format(BCPGlobalization.GeneralTextFormat_UnableToLocateInnkeeperOnMap,
                                        StyxWoW.Me.MapName, StyxWoW.Me.MapId);
                PluginLog.DeveloperInfo("{0}", message);
                Utility.OverlayNotification(message, Assets.ColorProblem, _tunable_ImportantMessageDelay);
                return false;
            }

            message = string.Format(BCPGlobalization.GeneralTextFormat_MovingToPlace,
                                    _innkeeperToPursue.Name,
                                    Vector3.Distance(StyxWoW.Me.Location, _innkeeperToPursue.Location),
                                    _innkeeperToPursue.Location);
            PluginLog.DeveloperInfo("{0}", message);
            Utility.OverlayNotification(message, Assets.ColorInformation, _tunable_ImportantMessageDelay);
            return true;
        }


        private async Task<bool> FinishingAction()
        {
            switch (PursueInnkeeperViaAction)
            {
                case ActionOnArrival.BotStop:
                    {
                        var message = string.Format(BCPGlobalization.GeneralTextFormat_ArrivedAtLocation_BotStopped,
                                                _innkeeperToPursue.Name);
                        PluginLog.Info(message);
                        Utility.OverlayNotification(message, Assets.ColorInformation, _tunable_ImportantMessageDelay);
                        Utility.BuddyBotStop();
                        return false;
                    }

                case ActionOnArrival.GameExit:
                    {
                        var message = string.Format(BCPGlobalization.GeneralTextFormat_ArrivedAtLocation_GameExiting,
                                                    Environment.NewLine,
                                                    _innkeeperToPursue.Name);
                        PluginLog.Info(message);
                        await FinishingActionCountDown(message, ActionOnArrival.GameExit, 20);

                        // If user cancelled the action, we're done...
                        if (PursueInnkeeperViaAction != ActionOnArrival.GameExit)
                        {
                            Utility.OverlayNotification(BCPGlobalization.GeneralText_UserCanceledInnkeeperGameExit,
                                Assets.ColorInformation);
                            return false;
                        }

                        // Otherwise, schedule a game-client shutdown as a background thread...
                        Utility.GameClientExit();
                        return false;
                    }

                case ActionOnArrival.Logout:
                    {
                        var message = string.Format(BCPGlobalization.GeneralTextFormat_ArrivedAtLocation_LoggingOut,
                                                    Environment.NewLine,
                                                    _innkeeperToPursue.Name);
                        PluginLog.Info(message);
                        await FinishingActionCountDown(message, ActionOnArrival.Logout, 20);

                        // If user cancelled the action, we're done...
                        if (PursueInnkeeperViaAction != ActionOnArrival.Logout)
                        {
                            Utility.OverlayNotification(BCPGlobalization.GeneralText_UserCanceledInnkeeperLogout,
                                Assets.ColorInformation);
                            return false;
                        }

                        // Otherwise, schedule a game-client shutdown as a background thread...
                        Utility.GameClientLogout();
                        return false;
                    }

                case ActionOnArrival.NoPursue:
                    // empty
                    return false;

                default:
                    {
                        PluginLog.MaintenanceError("Unhandled ActionOnArrival disposition ({0})", PursueInnkeeperViaAction);
                        return false;
                    }
            }
        }


        private async Task<bool> FinishingActionCountDown(string message,
            ActionOnArrival expectedFinishingAction,
            int countDownDurationInSeconds)
        {
            var timeSpanOneSecond = TimeSpan.FromSeconds(1);
            var timeSpanPartialSecond = TimeSpan.FromMilliseconds(950);
            do
            {
                var formattedMessage = string.Format("{1}{0}({2}s)", Environment.NewLine, message, countDownDurationInSeconds);
                Utility.OverlayNotification(formattedMessage, Assets.ColorWarning, timeSpanPartialSecond);
                System.Media.SystemSounds.Asterisk.Play();

                // Bail, if user cancels Innkeeper pursuit while we're waiting for timeout...
                await Coroutine.Wait(timeSpanOneSecond, () => PursueInnkeeperViaAction != expectedFinishingAction);
            } while ((PursueInnkeeperViaAction == expectedFinishingAction) && (--countDownDurationInSeconds > 0));

            return false;
        }


        private bool IsFlyingOverDestination(Vector3 location)
        {
            return StyxWoW.Me.IsFlying
                   && (StyxWoW.Me.Location.Distance2DSquared(location) < Tunable_ForceDismountDistanceSqr);
        }


        private void ShowProgressUpdate()
        {
            var isUpdatedNeeded = false;

            if (_stopWatch_ProgressUpdate == null)
            {
                _stopWatch_ProgressUpdate = new Stopwatch();
                _stopWatch_ProgressUpdate.Restart();
                isUpdatedNeeded = true;
            }

            if (_stopWatch_ProgressUpdate.Elapsed > _tunable_ProgressNotifyInterval)
            {
                _stopWatch_ProgressUpdate.Restart();
                isUpdatedNeeded = true;
            }

            if (isUpdatedNeeded)
            {
                TreeRoot.StatusText =
                    string.Format(BCPGlobalization.GeneralTextFormat_MovingToPlace,
                        _innkeeperToPursue.Name,
                        Vector3.Distance(StyxWoW.Me.Location, _innkeeperToPursue.Location),
                        _innkeeperToPursue.Location);
            }
        }
        #endregion
    }
}
