// Ported from: .hb 4.3.4/Honorbuddy/Honorbuddy/Bots/BGBuddy/BGBuddy.cs
// Target path: Bots/BGBuddy/BGBuddy.cs
// Deobfuscated: smethod_0 → CalculatePathPrecision
//              method_0 → BuildBehaviorTree
//              method_1 → OnMapChanged
//              method_2 → OnMapChangedOrStartStop
//              method_3 → CreateOutsideBattlegroundLogic
//              method_4 → RecordWinLoss
//              method_5 → CreateInsideBattlegroundLogic
//              method_6 → LogBattlegroundEnded
//              method_7 → RecordWinLossAction
//              smethod_1 → IsQueuedStatus
//              smethod_2 → IsInsideBattleground
//              smethod_3 → IsNotInsideBattleground
//              smethod_4 → HasDeserterAura
//              smethod_5 → LogWaitingDeserter
//              smethod_6 → AlwaysFalse
//              smethod_7 → IsNotHotspotTimerFinished
//              smethod_8 → ResetHotspotTimer
//              smethod_9 → IsWaitingForConfirmation
//              smethod_10 → RandomWaitSeconds
//              smethod_11 → LogAcceptingJoin
//              smethod_12 → HasCurrentBattleground
//              smethod_13 → ClearCurrentStatuses
//              smethod_14 → SleepForSeconds
//              smethod_15 → ResetHotspotTimerAction
//              smethod_16 → AcceptConfirmation
//              smethod_17 → SleepAfterAccept
//              smethod_18 → IsInPartyNotLeader
//              smethod_19 → LogWaitingGroupLeader
//              smethod_20 → ShouldQueue1
//              smethod_21 → LogQueueingUp1
//              smethod_22 → JoinQueue1
//              smethod_23 → ShouldQueue2
//              smethod_24 → LogQueueingUp2
//              smethod_25 → JoinQueue2
//              smethod_26 → IsBattlegroundFinished
//              smethod_27 → AlwaysFalse2
//              smethod_28 → LogLeavingBattleground
//              smethod_29 → LeaveBattlefieldAction
//              smethod_30 → SleepAfterLeave
//              smethod_31 → ResetHotspotTimerAfterLeave
//              smethod_32 → IsDeadOrGhost
//              smethod_33 → FindSpiritGuide
//              smethod_34 → IsNoSpiritGuide
//              smethod_35 → RandomCorpseRunWait
//              smethod_36 → AlwaysFalse3
//              smethod_37 → ClearPoiDied
//              smethod_38 → StopMoving
//              smethod_39 → RepopMe
//              smethod_40 → HasSpiritGuide
//              smethod_41 → IsSpiritGuide
//              smethod_42 → HasFoodTable
//              smethod_43 → HasNoHealthstone
//              smethod_44 → FindFoodTable
//              smethod_45 → IsWithinInteractRange
//              smethod_46 → InteractWithObject
//              smethod_47 → GetObjectLocation
//              smethod_48 → IsFoodTableEntry
//              smethod_49 → ItemHasHealthstoneSpell
//              smethod_50 → IsHealthstoneSpell
//              smethod_51 → IsFoodTableEntry2
//              smethod_52 → ReturnSuccess
//              smethod_53 → IsMoving
//              smethod_54 → StopMovingAction

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Action = TreeSharp.Action;
using Bots.BGBuddy.Forms;
using Bots.BGBuddy.Helpers;
using Bots.BGBuddy.Resources;
using Bots.Grind;
using CommonBehaviors.Actions;
using Styx;
using Styx.Common;
using Styx.Helpers;
using Styx.Loaders;
using Styx.Logic;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.Logic.POI;
using Styx.Logic.Profiles;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using AreaType = Tripper.Navigation.AreaType;
using Tripper.Tools.Math;

namespace Bots.BGBuddy
{
    /// <summary>
    /// BGBuddy bot base — automatic battleground queuing, combat, and objective logic.
    /// </summary>
    public class BGBuddy : BotBase
    {
        #region Fields

        private Composite _compositeRoot;
        private readonly WaitTimer _worldStateTimer = new WaitTimer(TimeSpan.FromSeconds(2));
        private WorldStatesUpdateDelegate _worldStatesDelegate;
        private static readonly WaitTimer _internalTimer = new WaitTimer(TimeSpan.FromSeconds(2));

        #endregion

        #region Singleton

        public static BGBuddy Instance { get; private set; }

        #endregion

        #region BotBase Overrides

        public override string Name => BGBuddyResources.BotName;

        public override Composite Root => _compositeRoot;

        public override PulseFlags PulseFlags => PulseFlags.All;

        public override System.Windows.Window ConfigurationWindow => new Forms.ConfigWindow();

        public override bool IsPrimaryType => false;

        public override bool RequirementsMet
        {
            get
            {
                // Already inside a BG — we're good
                if (Styx.Logic.Battlegrounds.IsInsideBattleground)
                    return true;

                // Deserter debuff — can't queue
                if (StyxWoW.Me.HasAura("Deserter"))
                    return false;

                // Check if we need to queue for more battlegrounds
                int desiredQueues = 0;
                if (BGBuddySettings.Instance.Queue1 != BattlegroundType.None)
                    desiredQueues++;
                if (BGBuddySettings.Instance.Queue2 != BattlegroundType.None)
                    desiredQueues++;

                int activeQueues = Styx.Logic.Battlegrounds.BattlegroundStatuses.Count(s => s == BattlegroundStatus.Queued);

                if (activeQueues < desiredQueues)
                    return true;

                return Styx.Logic.Battlegrounds.WaitingForConfirmation;
            }
        }

        #endregion

        #region Properties

        internal static Battleground CurrentBattleground { get; set; }

        public List<Battleground> Battlegrounds { get; set; }

        /// <summary>
        /// Tracks win/loss history per battleground name for session reporting.
        /// </summary>
        public static Dictionary<string, Dictionary<string, int>> BattlegroundHistory = new Dictionary<string, Dictionary<string, int>>();

        #endregion

        #region Events

        public event WorldStatesUpdateDelegate WorldStatesUpdated
        {
            add
            {
                WorldStatesUpdateDelegate current = _worldStatesDelegate;
                WorldStatesUpdateDelegate updated;
                do
                {
                    updated = current;
                    current = Interlocked.CompareExchange(ref _worldStatesDelegate,
                        (WorldStatesUpdateDelegate)Delegate.Combine(updated, value), updated);
                } while (current != updated);
            }
            remove
            {
                WorldStatesUpdateDelegate current = _worldStatesDelegate;
                WorldStatesUpdateDelegate updated;
                do
                {
                    updated = current;
                    current = Interlocked.CompareExchange(ref _worldStatesDelegate,
                        (WorldStatesUpdateDelegate)Delegate.Remove(updated, value), updated);
                } while (current != updated);
            }
        }

        #endregion

        #region Initialize / Start / Stop / Pulse

        public override void Initialize()
        {
            if (BGBuddySettings.Instance.FirstTime)
            {
                BGBuddySettings.Instance.Queue1 = BattlegroundType.RandomBattleground;
                BGBuddySettings.Instance.FirstTime = false;
                BGBuddySettings.Instance.Save();
            }
        }

        public override void Start()
        {
            if (StyxWoW.Me.Level < 10)
                throw new HonorbuddyUnableToStartException(BGBuddyResources.CanNotStartUnderLevel10);

            Battlegrounds = new DllLoader<Battleground>(Assembly.GetExecutingAssembly());
            Instance = this;
            ProfileManager.LoadEmpty();

            // Set up BG navigation provider
            Navigator.NavigationProvider = new BgMeshNavigator();
            Navigator.TripperNavigator.ChangeMap(new[] { StyxWoW.Me.MapId.ToString() });
            Navigator.TripperNavigator.QueryFilter.AreaCosts[AreaType.Water] = 7.33f;
            Navigator.TripperNavigator.QueryFilter.AreaCosts[AreaType.Misc8] = 7.33f;
            Navigator.TripperNavigator.QueryFilter.AreaCosts[AreaType.Fall] = 1f;

            BotEvents.Player.OnMapChanged += OnMapChanged;
            OnMapChangedOrStartStop(null);
        }

        public override void Stop()
        {
            // Reset navigation to default
            Navigator.NavigationProvider = new MeshNavigator();
            Navigator.TripperNavigator.ChangeMap(new[] { StyxWoW.Me.MapId.ToString() }); // plain MeshNavigator on stop — BgMeshNavigator only active during BG run
            Navigator.TripperNavigator.ResetQueryFilter();

            BotEvents.Player.OnMapChanged -= OnMapChanged;
            OnMapChangedOrStartStop(null);

            // Restore previous profile
            if (CharacterSettings.Instance.LastUsedPath != null && File.Exists(CharacterSettings.Instance.LastUsedPath))
                ProfileManager.LoadNew(CharacterSettings.Instance.LastUsedPath, false);

            // Session report
            if (BattlegroundHistory.Count > 0)
            {
                Logger.Write(BGBuddyResources.BGBuddySessionReport);
                Logger.Write("-----------------------------------------");
                foreach (var kvp in BattlegroundHistory)
                {
                    int wins = kvp.Value.ContainsKey(BGBuddyResources.Won) ? kvp.Value[BGBuddyResources.Won] : 0;
                    int losses = kvp.Value.ContainsKey(BGBuddyResources.Lost) ? kvp.Value[BGBuddyResources.Lost] : 0;
                    Logger.Write(BGBuddyResources.WinsLosses, kvp.Key, wins, losses);
                }
            }
        }

        public override void Pulse()
        {
            // Scale path precision with movement speed to avoid nav stuttering
            Navigator.PathPrecision = CalculatePathPrecision();

            Battleground.HotspotTimer.Update();
            Battleground.SetHotspotTimer.Update();

            if (_worldStateTimer.IsFinished)
            {
                if (_worldStatesDelegate != null)
                {
                    foreach (Delegate del in _worldStatesDelegate.GetInvocationList())
                    {
                        try
                        {
                            del.DynamicInvoke(null);
                        }
                        catch (Exception ex)
                        {
                            Logging.WriteException(ex);
                        }
                    }
                }
                _worldStateTimer.Reset();
            }
        }

        #endregion

        #region Targeting

        public void SetTargetingFilters()
        {
            Targeting.Instance = new BgTargeting();
        }

        public void RemoveTargetingFilters()
        {
            Targeting.Instance = new Targeting();
        }

        #endregion

        #region Internal — Map Change / BG Loading

        private void OnMapChanged(BotEvents.Player.MapChangedEventArgs args)
        {
            OnMapChangedOrStartStop(null);
        }

        private void OnMapChangedOrStartStop(EventArgs args)
        {
            if (StyxWoW.Me.CurrentMap.IsBattleground)
            {
                uint mapId = StyxWoW.Me.MapId;

                // Already running the right BG — don't restart
                if (CurrentBattleground != null && CurrentBattleground.MapId == mapId)
                    return;

                CurrentBattleground = Battlegrounds.FirstOrDefault(bg => bg.MapId == mapId);
                if (CurrentBattleground == null)
                    throw new HonorbuddyUnableToStartException(BGBuddyResources.NotSupported);

                SetTargetingFilters();
                Logger.Write(BGBuddyResources.Starting + CurrentBattleground.Name);
                CurrentBattleground.Start();
            }
            else if (CurrentBattleground != null)
            {
                CurrentBattleground.Dispose();
                CurrentBattleground = null;
                RemoveTargetingFilters();
            }

            BuildBehaviorTree();
        }

        #endregion

        #region Behavior Tree Construction

        /// <summary>
        /// Recalculates path precision based on current movement speed.
        /// Faster movement → higher precision to avoid overshooting.
        /// Clamped between 1.5 and 10.
        /// </summary>
        private static float CalculatePathPrecision()
        {
            float speed = StyxWoW.Me.MovementInfo.CurrentSpeed;
            float precision = speed * 0.15f;
            return MathEx.Clamp(precision, 1.5f, 10f);
        }

        private void BuildBehaviorTree()
        {
            _compositeRoot = new PrioritySelector(
                // Outside battleground logic
                CreateOutsideBattlegroundLogic(),
                // Inside battleground logic
                new Decorator(
                    ctx => Styx.Logic.Battlegrounds.IsInsideBattleground,
                    CreateInsideBattlegroundLogic()
                )
            );
        }

        /// <summary>
        /// Logic when not inside a battleground: deserter wait, queueing, BG acceptance.
        /// Ported from HB 4.3.4 BGBuddy.method_3.
        /// </summary>
        private Composite CreateOutsideBattlegroundLogic()
        {
            return new Decorator(
                ctx => !Styx.Logic.Battlegrounds.IsInsideBattleground,
                new PrioritySelector(
                    // [0] Death behavior (HB 4.3.4 smethod_32)
                    LevelBot.CreateDeathBehavior(),
                    // [1] Vendor behavior
                    LevelBot.CreateVendorBehavior(),
                    // [2] Combat behavior
                    LevelBot.CreateCombatBehavior(),

                    // [3] Deserter debuff — wait it out (HB 4.3.4 smethod_4: HasDeserterAura)
                    new Decorator(
                        ctx => StyxWoW.Me.HasAura("Deserter"),
                        new Sequence(
                            new Action(ctx => Logger.Write(BGBuddyResources.WaitingForDeserter)),
                            new WaitContinue(5, ctx => false, new ActionAlwaysSucceed()),
                            new ActionIdle()
                        )),

                    // [4] Short cooldown between actions (HB 4.3.4 smethod_7: !IsFinished, smethod_15: Reset)
                    //    smethod_8 (reset) returns RunStatus.Failure so the PrioritySelector continues
                    //    to the queue actions. Returning Success here would stop the tree prematurely.
                    new Decorator(
                        ctx => !_internalTimer.IsFinished,
                        new ActionAlwaysSucceed()
                    ),
                    new Action(ctx => { _internalTimer.Reset(); return RunStatus.Failure; }),

                    // [5] Waiting for BG confirmation popup — accept it (HB 4.3.4 smethod_9: WaitingForConfirmation)
                    new Decorator(
                        ctx => Styx.Logic.Battlegrounds.WaitingForConfirmation,
                        new PrioritySelector(
                            ctx => new Random().Next(3, 10), // random delay to look human
                            new Sequence(
                                new Action(ctx => Logger.Write(BGBuddyResources.AcceptingJoin, ctx)),
                                new DecoratorContinue(ctx => CurrentBattleground != null,
                                    new Action(ctx => CurrentBattleground.Statuses.Clear())),
                                new Action(ctx => Thread.Sleep((int)ctx * 1000)),
                                new Action(ctx => _internalTimer.Reset()),
                                new Action(ctx => Styx.Logic.Battlegrounds.AcceptBattlegroundConfirmation()),
                                new Action(ctx => Thread.Sleep(10000))
                            )
                        )),

                    // [6] In party but not leader — wait for leader to queue (HB 4.3.4 smethod_18: InPartyNotLeader)
                    new Decorator(
                        ctx => StyxWoW.Me.IsInParty && !StyxWoW.Me.IsGroupLeader,
                        new Sequence(
                            new Action(ctx => Logger.Write(BGBuddyResources.WaitingGroupLeader)),
                            new ActionIdle()
                        )),

                    // [7] Queue #1 (HB 4.3.4 smethod_20: ShouldQueue1)
                    //     Queue1 maps to WoW BG slot 1
                    new Decorator(
                        ctx => BGBuddySettings.Instance.Queue1 != BattlegroundType.None
                            && !Styx.Logic.Battlegrounds.IsQueuedForBattleground(BGBuddySettings.Instance.Queue1),
                        new Sequence(
                            new Action(ctx => Logger.Write(BGBuddyResources.QueueingUpFor, BGBuddySettings.Instance.Queue1)),
                            new Action(ctx => Styx.Logic.Battlegrounds.JoinBattlefield(1, BGBuddySettings.Instance.Queue1, StyxWoW.Me.IsInParty))
                        )),

                    // [8] Queue #2 (HB 4.3.4 smethod_23: ShouldQueue2)
                    //     Queue2 maps to WoW BG slot 2
                    new Decorator(
                        ctx => BGBuddySettings.Instance.Queue2 != BattlegroundType.None
                            && !Styx.Logic.Battlegrounds.IsQueuedForBattleground(BGBuddySettings.Instance.Queue2),
                        new Sequence(
                            new Action(ctx => Logger.Write(BGBuddyResources.QueueingUpFor, BGBuddySettings.Instance.Queue2)),
                            new Action(ctx => Styx.Logic.Battlegrounds.JoinBattlefield(2, BGBuddySettings.Instance.Queue2, StyxWoW.Me.IsInParty))
                        )),

                    // [9] Idle — nothing to do (HB 4.3.4 array[10] = ActionIdle)
                    new ActionIdle()
                )
            );
        }

        /// <summary>
        /// Logic when inside a battleground: BG end handling, death/release, and BG-specific logic.
        /// </summary>
        private Composite CreateInsideBattlegroundLogic()
        {
            var prioritySelector = new PrioritySelector(
                // BG finished — record result and leave
                new Decorator(
                    ctx => Styx.Logic.Battlegrounds.Finished,
                    new Sequence(
                        new Action(ctx => Logger.Write(BGBuddyResources.BattlegroundEnded,
                            WonOrLost ? BGBuddyResources.Won2 : BGBuddyResources.Lost2)),
                        new Action(ctx => RecordWinLoss()),
                        new WaitContinue(3, ctx => false, new ActionAlwaysSucceed()),
                        new Action(ctx => Logger.Write(BGBuddyResources.LeavingBattleground)),
                        new Action(ctx => Styx.Logic.Battlegrounds.LeaveBattlefield()),
                        new Action(ctx => Thread.Sleep(10000)),
                        new Action(ctx => _internalTimer.Reset())
                    )),

                // Dead or ghost — release and find spirit guide
                new Decorator(
                    ctx => StyxWoW.Me.Dead || StyxWoW.Me.IsGhost,
                    new PrioritySelector(
                        ctx => ObjectManager.ObjectList.FirstOrDefault(o => o is WoWUnit unit && unit.IsSpiritGuide),
                        new Decorator(
                            ctx => ctx == null,
                            new PrioritySelector(
                                new Wait(BgRandomTimeout, ctx => false, new ActionAlwaysFail()),
                                new Sequence(
                                    new Action(ctx => BotPoi.Clear(BGBuddyResources.Died)),
                                    new Action(ctx => Navigator.PlayerMover.MoveStop()),
                                    new Action(ctx => Lua.DoString("RepopMe()"))
                                )
                            )),
                        new Decorator(
                            ctx => ctx != null,
                            new ActionAlwaysSucceed()
                        )
                    ))
            );

            // Add BG-specific logic if available
            if (CurrentBattleground != null)
            {
                prioritySelector.AddChild(CurrentBattleground.CreateCommonLogic);
                prioritySelector.AddChild(CurrentBattleground.Logic);
            }

            return prioritySelector;
        }

        #endregion

        #region Win/Loss Tracking

        private bool WonOrLost
        {
            get
            {
                if (Styx.Logic.Battlegrounds.Winner != BattlefieldWinner.Alliance)
                    return StyxWoW.Me.IsHorde;
                return StyxWoW.Me.IsAlliance;
            }
        }

        private void RecordWinLoss()
        {
            string name = CurrentBattleground.Name;
            string result = WonOrLost ? BGBuddyResources.Won : BGBuddyResources.Lost;

            if (WonOrLost)
                InfoPanel.BGWon();
            else
                InfoPanel.BGLost();

            if (BattlegroundHistory.ContainsKey(name))
            {
                var dict = BattlegroundHistory[name];
                if (dict.ContainsKey(result))
                    dict[result]++;
                else
                    dict.Add(result, 1);
            }
            else
            {
                BattlegroundHistory.Add(name, new Dictionary<string, int> { { result, 1 } });
            }
        }

        #endregion

        #region Consumables (Healthstone from food table)

        /// <summary>
        /// Finds and uses a mage food table (entry 181621) to obtain a Healthstone.
        /// </summary>
        public Composite CreateTakeConsumablesBehavior()
        {
            return new Decorator(
                ctx => ObjectManager.GetObjectsOfType<WoWGameObject>().Any(g => g.Entry == 181621),
                new Decorator(
                    ctx => !StyxWoW.Me.CarriedItems.Any(item =>
                        item.ItemSpells.Any(spell => spell.ActualSpell != null && spell.ActualSpell.Name == "Healthstone")),
                    new PrioritySelector(
                        ctx => ObjectManager.GetObjectsOfType<WoWGameObject>().FirstOrDefault(g => g.Entry == 181621),
                        new Decorator(
                            ctx => ((WoWObject)ctx).WithinInteractRange,
                            new Action(ctx => ((WoWObject)ctx).Interact())
                        ),
                        CreateMoveToLocationBehavior(ctx => ((WoWObject)ctx).Location, true, 2f)
                    )
                )
            );
        }

        #endregion

        #region Static Behavior Helpers

        /// <summary>
        /// Creates a movement behavior that navigates to the given location, optionally stopping in range.
        /// Handles mounting for long distances.
        /// </summary>
        public static Composite CreateMoveToLocationBehavior(LocationRetriever location, bool stopInRange, float range)
        {
            return new PrioritySelector(
                // Already at destination — stop and succeed
                new Decorator(
                    ctx => stopInRange && StyxWoW.Me.Location.Distance(location(ctx)) + 0.6f < range,
                    new PrioritySelector(
                        CreateEnsureMovementStoppedBehavior(),
                        new Action(ctx => RunStatus.Success)
                    )),
                // Need to mount up for long distance (HB: mount attempt is the Decorator condition, not a child Action)
                new Decorator(
                    ctx => !StyxWoW.Me.Mounted
                        && location(ctx).Distance(StyxWoW.Me.Location) > BGBuddySettings.Instance.MountUpDistance
                        && Mount.MountUp(() => true, () => location(ctx)),
                    new ActionAlwaysSucceed()
                ),
                // Move to location
                new Action(ctx => Navigator.MoveTo(location(ctx)))
            );
        }

        /// <summary>
        /// Stops the player if currently moving.
        /// </summary>
        public static Composite CreateEnsureMovementStoppedBehavior()
        {
            return new Decorator(
                ctx => StyxWoW.Me.IsMoving,
                new Action(ctx => Navigator.PlayerMover.MoveStop())
            );
        }

        #endregion

        #region BgTargeting — Custom Targeting for Battlegrounds

        /// <summary>
        /// Custom targeting class for battlegrounds.
        /// Filters out irrelevant targets (pets, totems, mounted players, CC'd players)
        /// and weights targets by priority (flag carriers, healers, closeness).
        /// </summary>
        public class BgTargeting : Targeting
        {
            // Entries of important BG NPCs that should be targeted
            private static readonly HashSet<uint> _priorityNpcEntries = new HashSet<uint>
            {
                28781,  // Alterac Valley bunker/tower flag
                34802, 35273, 35069, 34776, 34775 // IoC boss NPCs and similar
            };

            // Transport GUIDs of mounted enemy players (used to exclude them)
            private static readonly HashSet<ulong> _transportGuids = new HashSet<ulong>();

            public BgTargeting()
            {
                AllowEvents = false;
            }

            protected sealed override List<WoWObject> GetInitialObjectList()
            {
                return ObjectManager.GetObjectsOfType<WoWUnit>(true, false)
                    .Where(u => !u.IsFriendly)
                    .Cast<WoWObject>()
                    .ToList();
            }

            protected sealed override void DefaultRemoveTargetsFilter(List<WoWObject> units)
            {
                bool isMounted = StyxWoW.Me.Mounted;
                ulong myGuid = StyxWoW.Me.Guid;
                ulong[] raidGuids = StyxWoW.Me.RaidMemberGuids;

                _transportGuids.Clear();

                units.RemoveAll(obj =>
                {
                    var unit = obj as WoWUnit;
                    if (unit == null)
                        return true;

                    // NPC targets
                    if (!unit.IsPlayer)
                    {
                        if (!unit.IsAlive) return true;
                        double dist = unit.DistanceSqr;

                        // AV flag NPC — always keep if close
                        if (unit.Entry == 28781 && dist < 10000)
                            return false;

                        if (isMounted) return true;
                        if (dist > 1600) return true;
                        if (Blacklist.Contains(unit.Guid)) return true;
                        if (unit.CurrentTargetGuid == 0) return true;
                        if (unit.CurrentTargetGuid != myGuid && !raidGuids.Contains(unit.CurrentTargetGuid)) return true;
                        if (unit.IsTotem) return true;
                        if (unit.IsPet) return true;
                        if (unit.IsNonCombatPet) return true;

                        return false;
                    }

                    // Player targets
                    if (unit.Dead) return true;
                    if (unit.Shapeshift == ShapeshiftForm.SpiritOfRedemption) return true;

                    ulong transportGuid = unit.WoWMovementInfo.TransportGuid;
                    if (transportGuid != 0)
                        _transportGuids.Add(transportGuid);

                    if (unit.DistanceSqr > 3600) return true;
                    if (unit.Mounted) return true;

                    // Skip CC'd players unless they're carrying a flag
                    if (IsCrowdControlled(unit))
                    {
                        if (!unit.ActiveAuras.Keys.Any(k => k.ToLowerInvariant().Contains("flag")))
                            return true;
                    }

                    if (Blacklist.Contains(unit.Guid) && unit.CurrentTargetGuid != myGuid) return true;
                    if (unit.IsPet) return true;
                    if (unit.IsNonCombatPet) return true;
                    if (unit.IsOnTransport) return true;

                    return false;
                });
            }

            protected sealed override void DefaultIncludeTargetsFilter(List<WoWObject> incomingUnits, HashSet<WoWObject> outgoingUnits)
            {
                foreach (var obj in incomingUnits)
                    outgoingUnits.Add(obj);
            }

            protected override void DefaultTargetWeight(List<Targeting.TargetPriority> units)
            {
                var me = StyxWoW.Me;
                bool anyoneTargetingMe = ObjectManager.GetObjectsOfType<WoWPlayer>().Any(p => p.IsTargetingMeOrPet);

                foreach (var priority in units)
                {
                    var unit = priority.Object.ToUnit();

                    if (!unit.IsPlayer)
                    {
                        // Priority NPCs (AV flags, IoC bosses) get high score if relevant
                        if (_priorityNpcEntries.Contains(unit.Entry) && _transportGuids.Contains(unit.Guid)
                            && !anyoneTargetingMe && me.SpecType != SpecType.Healer && unit.Distance < 70)
                        {
                            priority.Score = 5000;
                        }
                        else
                        {
                            priority.Score -= unit.Distance;
                        }
                        continue;
                    }

                    // Player targets: base score 1000
                    priority.Score = 1000;

                    // Flag carrier bonus
                    if (unit.ActiveAuras.Values.Any(a => a.Name.ToLowerInvariant().Contains("flag")))
                        priority.Score += 200;

                    // Current target bonus
                    if (unit.Guid == me.CurrentTargetGuid)
                        priority.Score += 150;

                    // Attackers bonus — more players attacking this target = higher priority
                    int attackers = ObjectManager.GetObjectsOfType<WoWUnit>(false, false)
                        .Count(u => u.CurrentTarget == unit && u.IsFriendly != unit.IsFriendly);
                    priority.Score += attackers * 50;

                    // Distance penalty based on spec
                    double dist = unit.Distance;
                    if (me.SpecType == SpecType.RangedDps)
                    {
                        if (dist > 35)
                            priority.Score -= dist * 3;
                    }
                    else
                    {
                        if (dist > 20)
                            priority.Score -= dist * 3 * 3;
                        else if (dist > 10)
                            priority.Score -= dist * 3;
                    }

                    // Lower health = higher priority
                    priority.Score -= unit.HealthPercent;

                    // Healer bonus (mana users)
                    if (unit.PowerType == WoWPowerType.Mana)
                        priority.Score += 50;
                }
            }

            /// <summary>
            /// Checks if a unit is under a loss-of-control CC effect.
            /// </summary>
            private static bool IsCrowdControlled(WoWUnit unit)
            {
                return unit.Auras.Values
                    .Select(a => a.Spell.Mechanic)
                    .Any(m => m == WoWSpellMechanic.Banished
                           || m == WoWSpellMechanic.Charmed
                           || m == WoWSpellMechanic.Horrified
                           || m == WoWSpellMechanic.Incapacitated
                           || m == WoWSpellMechanic.Polymorphed
                           || m == WoWSpellMechanic.Sapped
                           || m == WoWSpellMechanic.Shackled
                           || m == WoWSpellMechanic.Asleep
                           || m == WoWSpellMechanic.Frozen
                           || m == WoWSpellMechanic.Invulnerable
                           || m == WoWSpellMechanic.Invulnerable2
                           || m == WoWSpellMechanic.Turned);
            }
        }

        #endregion

        #region Delegates

        /// <summary>
        /// Retrieves a WoWPoint location from a behavior tree context object.
        /// </summary>
        public delegate WoWPoint LocationRetriever(object context);

        /// <summary>
        /// Retrieves a dynamic range value from a behavior tree context object.
        /// </summary>
        public delegate float DynamicRangeRetriever(object context);

        /// <summary>
        /// Returns a random timeout in seconds for corpse-run waits.
        /// </summary>
        private static WaitGetTimeoutDelegate BgRandomTimeout => () => new Random().Next(1, 3);

        #endregion
    }
}
