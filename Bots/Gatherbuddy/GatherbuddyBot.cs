using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bots.Grind;
using CommonBehaviors.Actions;
using Styx;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Inventory;
using Styx.Logic.Inventory.Frames.Gossip;
using Styx.Logic.Inventory.Frames.LootFrame;
using Styx.Logic.Inventory.Frames.MailBox;
using Styx.Logic.Inventory.Frames.Merchant;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles;
using Styx.WoWInternals;
using Styx.WoWInternals.World;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Action = TreeSharp.Action;

namespace Bots.Gatherbuddy
{
    public class GatherbuddyBot : BotBase
    {
        #region Fields

        // Arrival threshold squared: (flySpeed + 10)^2.
        // Hotspot is considered "reached" when distance to it drops below this value.
        // WoD: Single_0
        private static float ArrivalThresholdSqr
        {
            get
            {
                float speed = StyxWoW.Me.MovementInfo.FlySpeed + 10f;
                return speed * speed;
            }
        }

        // Set to true when we are in combat while flying with no node nearby.
        // Prevents the combat subtree from firing while we are airborne and should keep moving.
        // WoD: bool_1
        private static bool _combatSuppressed;

        // True after we've done the initial mount+ascend during resurrection sickness.
        // WoD: bool_2
        private static bool _rezSicknessTakeoffDone;

        // Best landing position around the current node, computed by CalculateApproachPoint().
        // WoWPoint.Zero means "not yet computed this node cycle".
        // WoD: woWPoint_0
        private static WoWPoint _approachPoint;

        // Number of interact attempts on _currentNode this node cycle.
        // Blacklists the node after 3 failures.
        // WoD: int_2
        private static int _gatherAttemptCount;

        // The node object we are currently trying to gather.
        // Reset to null whenever the gather cycle completes or blacklists.
        // WoD: woWObject_1
        private static WoWObject _currentNode;

        // GUID of the last node we logged "Flying to" for — prevents per-tick log spam.
        private static ulong _lastLoggedNodeGuid;

        // Per-node timer. If it exceeds BlacklistTimer seconds the node is blacklisted.
        // WoD: stopwatch_0
        internal static readonly Stopwatch _gatherTimer = new Stopwatch();

        // Session start time — used by RunningTime and the Stop() summary log.
        // WoD: dateTime_0
        private static DateTime _sessionStart;

        // Per-node-name harvest counts, reported when the bot stops.
        public static readonly Dictionary<string, int> NodeCollectionCount = new Dictionary<string, int>();

        // Positions that have been manually or automatically blacklisted along with a reason string.
        public static readonly Dictionary<WoWPoint, string> BlacklistNodes = new Dictionary<WoWPoint, string>();

        // Node positions that have already passed the underwater / navmesh validation.
        // Avoids re-running the TraceLine checks on every filter tick.
        // WoD: hashSet_0
        private static readonly HashSet<WoWPoint> _validatedNodePositions = new HashSet<WoWPoint>();

        // GameObject entry IDs that are never gathered: traps, cages, misc objects.
        // WoD: list_6
        internal static readonly List<uint> _avoidList = new List<uint>
        {
            185881U, 190702U, 185877U, 1610U, 19903U,
            73940U, 73941U, 123310U, 123309U, 123848U, 177388U
        };

        // Cached root composite — built once on first access, then reused.
        // WoD: composite_0 / GatherbuddyBot.Root
        private Composite _root;

        // Circular waypoint queue driven from the loaded profile hotspots.
        private CircularQueue<WoWPoint> _waypointQueue;
        private readonly List<WoWPoint> _waypoints = new List<WoWPoint>();

        #endregion

        #region BotBase

        public override string Name          => "GatherBuddy";
        public override bool   IsPrimaryType => true;
        public override bool   RequiresProfile => true;
        public override PulseFlags PulseFlags => PulseFlags.All;
        public override object ConfigurationForm => new GatherBuddySettingsWindow();

        public override Composite Root
        {
            get
            {
                if (_root == null)
                    _root = CreateRootBehavior();
                return _root;
            }
        }

        /// <summary>Elapsed time since the last Start() call.</summary>
        public static TimeSpan RunningTime => DateTime.Now.Subtract(_sessionStart);

        public override void Pulse() { }

        #endregion

        #region Lifecycle

        public GatherbuddyBot()
        {
            // Load additional avoidList entries from XML, if present.
            LoadAvoidList();
        }

        public override void Initialize()
        {
            // Log every setting on first load so the user can verify configuration.
            // WoD: method_9()
            Log("--------------- Settings ---------------");
            foreach (var kvp in GatherBuddySettings.Instance.GetSettings())
                Log("{0}: {1}", kvp.Key, kvp.Value);
            Log("----------------------------------------");
        }

        public override void Start()
        {
            InitBlacklist();
            TreeRoot.TicksPerSecond = 15;

            NodeCollectionCount.Clear();
            _validatedNodePositions.Clear();
            _combatSuppressed         = false;
            _rezSicknessTakeoffDone   = false;
            _approachPoint            = WoWPoint.Zero;
            _gatherAttemptCount       = 0;
            _currentNode              = null;
            _lastLoggedNodeGuid       = 0;
            _gatherTimer.Reset();
            _sessionStart             = DateTime.Now;

            _waypoints.Clear();
            if (ProfileManager.CurrentProfile == null)
                throw new InvalidOperationException("[GatherBuddy] No profile loaded.");

            ReloadWaypoints();

            if (ProfileManager.CurrentProfile.Blackspots?.Count > 0)
                BlackspotManager.AddBlackspots(ProfileManager.CurrentProfile.Blackspots);

            // HB 6.2.3: invalidate cached PolyNav after loading blackspots so the flight engine
            // rebuilds its path around any new aerial obstacles from the profile.
            Flightor.Clear();

            Targeting.Instance.IncludeTargetsFilter      += IncludeTargetsFilter;
            LootTargeting.Instance.IncludeTargetsFilter  += IncludeLootFilter;
            LootTargeting.Instance.RemoveTargetsFilter   += RemoveLootFilter;
            LootTargeting.Instance.WeighTargetsFilter    += WeighLootTargetsFilter;
            BotEvents.Profile.OnNewProfileLoaded         += OnProfileLoaded;

            Log("Started. Loaded {0} waypoints.", _waypoints.Count);
        }

        public override void Stop()
        {
            TreeRoot.TicksPerSecond = CharacterSettings.Instance.TicksPerSecond;
            _approachPoint      = WoWPoint.Zero;
            _currentNode        = null;
            _gatherAttemptCount = 0;

            Log("Stopped. Harvested {0} nodes in {1}h {2}m {3}s.",
                NodeCollectionCount.Values.Sum(),
                RunningTime.Hours, RunningTime.Minutes, RunningTime.Seconds);

            foreach (var kvp in NodeCollectionCount)
                Log("  {0}: {1}", kvp.Key, kvp.Value);

            Vendors.ForceSell = false;

            if (ProfileManager.CurrentProfile?.Blackspots?.Count > 0)
                BlackspotManager.RemoveBlackspots(ProfileManager.CurrentProfile.Blackspots);

            Targeting.Instance.IncludeTargetsFilter      -= IncludeTargetsFilter;
            LootTargeting.Instance.IncludeTargetsFilter  -= IncludeLootFilter;
            LootTargeting.Instance.RemoveTargetsFilter   -= RemoveLootFilter;
            LootTargeting.Instance.WeighTargetsFilter    -= WeighLootTargetsFilter;
            BotEvents.Profile.OnNewProfileLoaded         -= OnProfileLoaded;
        }

        /// <summary>
        /// Rebuilds _waypoints and _waypointQueue from the current profile.
        /// Called on Start() and when the user loads a new profile at runtime.
        /// WoD: method_10 (OnNewProfileLoaded) + Start() hotspot block.
        /// </summary>
        private void ReloadWaypoints()
        {
            _waypoints.Clear();
            float heightMod = GatherBuddySettings.Instance.HeightModifier;

            if (ProfileManager.CurrentProfile?.GrindArea?.Hotspots != null &&
                ProfileManager.CurrentProfile.GrindArea.Hotspots.Count > 0)
            {
                foreach (var hs in ProfileManager.CurrentProfile.GrindArea.Hotspots)
                    _waypoints.Add(hs.ToWoWPoint().Add(0f, 0f, heightMod));
            }
            else if (ProfileManager.CurrentProfile?.HotspotManager?.Hotspots != null &&
                     ProfileManager.CurrentProfile.HotspotManager.Hotspots.Count > 0)
            {
                foreach (var pt in ProfileManager.CurrentProfile.HotspotManager.Hotspots)
                    _waypoints.Add(pt.Add(0f, 0f, heightMod));
            }
            else
            {
                Log("Profile has no hotspots — waypoints unchanged.");
                return;
            }

            if (GatherBuddySettings.Instance.RandomizeHotspots)
                ShuffleList(_waypoints);

            _waypointQueue = new CircularQueue<WoWPoint>();
            _waypointQueue.Mode = GatherBuddySettings.Instance.PathingType == PathType.Bounce
                ? Styx.Helpers.QueueMode.Bounce
                : Styx.Helpers.QueueMode.Circle;
            foreach (var wp in _waypoints)
                _waypointQueue.Enqueue(wp);

            // Start from the nearest hotspot.
            if (StyxWoW.Me != null && _waypoints.Count > 0)
            {
                var nearest = _waypoints.OrderBy(w => w.DistanceSqr(StyxWoW.Me.Location)).First();
                _waypointQueue.CycleTo(nearest);
            }
        }

        /// <summary>
        /// Called when the user loads a new profile while the bot is running.
        /// Reloads waypoints from the new profile without restarting the bot.
        /// WoD: method_10 (BotEvents.Profile.OnNewProfileLoaded handler).
        /// </summary>
        private void OnProfileLoaded(BotEvents.Profile.NewProfileLoadedEventArgs args)
        {
            Log("New profile loaded — reloading waypoints.");
            _approachPoint = WoWPoint.Zero;
            _currentNode   = null;
            ReloadWaypoints();
        }

        #endregion

        #region Root Behavior Tree

        /// <summary>
        /// Top-level 12-child PrioritySelector that drives the entire bot.
        /// WoD: method_2()
        /// </summary>
        private Composite CreateRootBehavior()
        {
            return new PrioritySelector(

                // [0] Dead / ghost — release, corpse walk, spirit healer, rez sickness.
                CreateDeathBehavior(),

                // [1] In combat while mounted (ANY mount, ground or flying) and no node nearby:
                //     flag _combatSuppressed so the combat tree is skipped — keep moving.
                //     HB 4.3.4: smethod_13 — no IsFlying check, distance gate is 50y.
                new Decorator(
                    ctx => StyxWoW.Me.Combat &&
                           Flightor.MountHelper.Mounted &&
                           (LootTargeting.Instance.FirstObject == null ||
                            LootTargeting.Instance.FirstObject.Distance > 50.0),
                    new Action(ctx =>
                    {
                        _combatSuppressed = true;
                        return RunStatus.Failure;
                    })
                ),

                // [2] Any ground combat (or eating/drinking): reset per-node gather counters.
                //     HB 6.2.3 Class670.method_2: Me.Combat || HasAura("Food") || HasAura("Drink")
                //     Returns Failure so the tree continues to the combat subtree below.
                new Decorator(
                    ctx => StyxWoW.Me.Combat ||
                           StyxWoW.Me.HasAura("Food") ||
                           StyxWoW.Me.HasAura("Drink"),
                    new Action(ctx =>
                    {
                        _gatherAttemptCount = 0;
                        _gatherTimer.Reset();
                        return RunStatus.Failure;
                    })
                ),

                // [3] No target but our pet is fighting — forward pet's target to the player.
                //     WoD: Class670.method_4/5
                new Decorator(
                    ctx => StyxWoW.Me.CurrentTarget == null &&
                           StyxWoW.Me.GotAlivePet &&
                           StyxWoW.Me.Pet != null &&
                           StyxWoW.Me.Pet.Combat &&
                           StyxWoW.Me.Pet.CurrentTarget != null,
                    new Action(ctx =>
                    {
                        StyxWoW.Me.Pet.CurrentTarget.Target();
                        return RunStatus.Success;
                    })
                ),

                // [4] Combat subtree — run when not mounted OR not flying.
                //     HB 6.2.3 Class670.method_6: !Flightor.MountHelper.Mounted || !Me.MovementInfo.IsFlying
                //     Plus inner gate method_7: !bool_1 (not combat-suppressed from [1]).
                //     When dismounted in combat, HB enters combat immediately; CB must match.
                new Decorator(
                    ctx => (!Flightor.MountHelper.Mounted || !StyxWoW.Me.MovementInfo.IsFlying) && !_combatSuppressed,
                    new PrioritySelector(
                        LevelBot.CreateCombatBehavior()
                    )
                ),

                // [5] Bags full / gear damaged — go to vendor.
                //     WoD: method_4()
                CreateVendorBehavior(),

                // [6] Clear _combatSuppressed every tick; always returns Failure.
                //     HB 6.2.3: Class670.method_10 — bool_1 = false; return Failure.
                new Action(ctx =>
                {
                    _combatSuppressed = false;
                    return RunStatus.Failure;
                }),

                // [7] BottingHours session cap.
                //     WoD: Class670.method_11/12
                new Decorator(
                    ctx => GatherBuddySettings.Instance.BottingHours > 0f &&
                           RunningTime.TotalHours >= GatherBuddySettings.Instance.BottingHours,
                    new Action(ctx =>
                    {
                        Log("Ran for {0:F1} hours, stopping.", GatherBuddySettings.Instance.BottingHours);
                        TreeRoot.Stop("GatherBuddy: BottingHours");
                        return RunStatus.Success;
                    })
                ),

                // [8] Resurrection Sickness: mount up and jump-ascend once, then hold position.
                //     WoD: Class670.method_13–18
                new Decorator(
                    ctx => GatherBuddySettings.Instance.WaitRezSickness &&
                           StyxWoW.Me.HasAura("Resurrection Sickness"),
                    new PrioritySelector(
                        // First tick with rez sickness: mount and ascend so we don't
                        // stand on the ground while waiting.
                        new Decorator(
                            ctx => !_rezSicknessTakeoffDone,
                            new Sequence(
                                new Action(ctx =>
                                {
                                    Flightor.MountHelper.MountUp();
                                    return RunStatus.Success;
                                }),
                                new WaitContinue(10,
                                    ctx => Flightor.MountHelper.Mounted,
                                    new ActionAlwaysSucceed()),
                                new Action(ctx =>
                                {
                                    WoWMovement.Move(WoWMovement.MovementDirection.JumpAscend,
                                        TimeSpan.FromSeconds(3.0));
                                    return RunStatus.Success;
                                }),
                                new Action(ctx =>
                                {
                                    _rezSicknessTakeoffDone = true;
                                    return RunStatus.Success;
                                })
                            )
                        ),
                        // Subsequent ticks: hold and display a status message.
                        new Sequence(
                            new ActionSetActivity("Waiting for Resurrection Sickness"),
                            new ActionAlwaysSucceed()
                        )
                    )
                ),

                // [9] Close enough to the current hotspot — advance the queue to the next one.
                //     ArrivalThresholdSqr = (flySpeed+10)^2 to account for inertia at speed.
                new Decorator(
                    ctx => _waypointQueue != null &&
                           _waypointQueue.Count > 0 &&
                           StyxWoW.Me.Location.Distance2DSqr(_waypointQueue.Peek()) < ArrivalThresholdSqr,
                    new Action(ctx =>
                    {
                        _waypointQueue.Dequeue();
                        return RunStatus.Success;
                    })
                ),

                // [10] A node is available in LootTargeting — run the gather sub-tree.
                new Decorator(
                    ctx => LootTargeting.Instance.FirstObject != null,
                    CreateGatherBehavior()
                ),

                // [11] No node — patrol to the next hotspot via Flightor.
                //      Stop any active descend key first to avoid fighting the flight engine.
                //      WoD: Class670.method_70–73
                new Decorator(
                    ctx => LootTargeting.Instance.FirstObject == null,
                    new PrioritySelector(
                        new Sequence(
                            new ActionSetActivity("Patrolling"),
                            new DecoratorContinue(
                                ctx => StyxWoW.Me.MovementInfo.IsDescending,
                                new Action(ctx =>
                                {
                                    WoWMovement.MoveStop(WoWMovement.MovementDirection.Descend);
                                    return RunStatus.Success;
                                })
                            ),
                            new Action(ctx =>
                            {
                                if (_waypointQueue == null || _waypointQueue.Count == 0)
                                    return RunStatus.Failure;

                                // Adapt minHeight to the next waypoint's ground height so we
                                // don't descend below it and have to climb back up immediately.
                                float minHeight = 40f;
                                if (_waypointQueue.Count > 1)
                                {
                                    WoWPoint nextWp = _waypointQueue.ElementAt(1);
                                    float nextWpRelZ = nextWp.Z - StyxWoW.Me.Location.Z;
                                    if (nextWpRelZ > minHeight)
                                        minHeight = nextWpRelZ + 5f;
                                }

                                Flightor.MoveTo(_waypointQueue.Peek(), minHeight);
                                return RunStatus.Success;
                            }),
                            new ActionIdle()
                        )
                    )
                )
            );
        }

        #endregion

        #region Death Behavior

        /// <summary>
        /// Handles dead body, ghost state, spirit healer, Sea Legs corpse run.
        /// WoD: method_3()
        /// </summary>
        private Composite CreateDeathBehavior()
        {
            return new PrioritySelector(
                // Dead body on the ground — release spirit via LevelBot.
                new Decorator(
                    ctx => StyxWoW.Me.IsDead,
                    LevelBot.CreateDeathBehavior()
                ),

                // Ghost — find the fastest path back to the body.
                new Decorator(
                    ctx => StyxWoW.Me.IsGhost,
                    new PrioritySelector(
                        // Spirit healer path: explicitly enabled, or no flight and can't navigate to corpse.
                        new Decorator(
                            ctx => GatherBuddySettings.Instance.UseSpiritHealer ||
                                   (!Flightor.CanFly &&
                                    StyxWoW.Me.CorpsePoint != WoWPoint.Empty &&
                                    !Navigator.CanNavigateFully(StyxWoW.Me.Location, StyxWoW.Me.CorpsePoint)),
                            new Sequence(
                                new Action(ctx =>
                                {
                                    var spiritHealer = ObjectManager.GetObjectsOfType<WoWUnit>()
                                        .Where(u => u.IsValid && u.IsSpiritHealer)
                                        .OrderBy(u => u.DistanceSqr)
                                        .FirstOrDefault();

                                    if (spiritHealer == null)
                                        return RunStatus.Failure;

                                    if (!spiritHealer.WithinInteractRange)
                                    {
                                        if (Flightor.CanFly)
                                            Flightor.MoveTo(spiritHealer.Location, 40f);
                                        else
                                            Navigator.MoveTo(spiritHealer.Location);
                                        return RunStatus.Running;
                                    }

                                    WoWMovement.MoveStop();
                                    spiritHealer.Interact();
                                    return RunStatus.Success;
                                }),
                                new Action(ctx => { StyxWoW.Sleep(1000); return RunStatus.Success; }),
                                new Action(ctx =>
                                {
                                    Lua.DoString("StaticPopup1Button1:Click()");
                                    return RunStatus.Success;
                                }),
                                new Action(ctx => { StyxWoW.Sleep(500); return RunStatus.Success; }),
                                new Action(ctx =>
                                {
                                    Lua.DoString("StaticPopup1Button1:Click()");
                                    return RunStatus.Success;
                                }),
                                new Action(ctx => { StyxWoW.Sleep(2000); return RunStatus.Success; })
                            )
                        ),

                        // Sea Legs: can't run on water, fly directly to corpse instead.
                        new Decorator(
                            ctx => StyxWoW.Me.HasAura("Sea Legs") &&
                                   StyxWoW.Me.CorpsePoint != WoWPoint.Empty &&
                                   StyxWoW.Me.CorpsePoint.Distance(StyxWoW.Me.Location) > 5f,
                            new Action(ctx =>
                            {
                                Flightor.MoveTo(StyxWoW.Me.CorpsePoint, 40f);
                                return RunStatus.Running;
                            })
                        ),

                        // Default: standard HB corpse run.
                        LevelBot.CreateDeathBehavior()
                    )
                )
            );
        }

        #endregion

        #region Vendor Behavior

        /// <summary>
        /// Top-level vendor/repair selector.
        /// WoD: method_4()
        /// </summary>
        private Composite CreateVendorBehavior()
        {
            return new PrioritySelector(
                // MailToAlt enabled and there are items to mail — go to mailbox first.
                new Decorator(
                    new CanRunDecoratorDelegate(NeedsMailing),
                    CreateMailBehavior()
                ),
                // Bags full for active gather type(s) — go sell (and repair while there).
                new Decorator(
                    new CanRunDecoratorDelegate(NeedsBagsEmptied),
                    new PrioritySelector(
                        CreateSellBehavior()
                    )
                ),
                // Gear damaged below threshold — repair only.
                new Decorator(
                    ctx => GatherBuddySettings.Instance.RepairAtVendor &&
                           StyxWoW.Me.DurabilityPercent <= GatherBuddySettings.Instance.RepairDurabilityPercent,
                    CreateRepairBehavior()
                )
            );
        }

        /// <summary>
        /// Move to the nearest repair vendor, interact, repair all.
        /// WoD: method_5()
        /// </summary>
        private Composite CreateRepairBehavior()
        {
            return new PrioritySelector(
                new Action(ctx =>
                {
                    var vendor = ProfileManager.CurrentProfile?.VendorManager?
                        .GetClosestVendor(Vendor.VendorType.Repair);
                    if (vendor == null) return RunStatus.Failure;

                    var vendorUnit = ObjectManager.GetObjectsOfType<WoWUnit>()
                        .Where(u => u.IsValid && u.Entry == (uint)vendor.Entry && u.IsAlive)
                        .OrderBy(u => u.DistanceSqr)
                        .FirstOrDefault();

                    if (vendorUnit == null)
                    {
                        TreeRoot.StatusText = "Moving to repair vendor";
                        if (Flightor.CanFly)
                            Flightor.MoveTo(vendor.Location, 40f);
                        else
                            Navigator.MoveTo(vendor.Location);
                        return RunStatus.Running;
                    }

                    if (!vendorUnit.WithinInteractRange)
                    {
                        Navigator.MoveTo(vendorUnit.Location);
                        return RunStatus.Running;
                    }

                    WoWMovement.MoveStop();
                    Log("Repairing at {0}.", vendorUnit.Name);
                    vendorUnit.Interact();
                    StyxWoW.SleepForLagDuration();

                    // Some NPCs show a gossip frame before the merchant frame.
                    // Select the first option ("Browse Wares" / "Repair") to open it.
                    // WoD: method_95/96 — DecoratorContinue(!MerchantFrame.IsVisible && GossipFrame.IsVisible)
                    if (!MerchantFrame.Instance.IsVisible && GossipFrame.Instance.IsVisible)
                    {
                        GossipFrame.Instance.SelectGossipOption(0);
                        StyxWoW.SleepForLagDuration();
                    }

                    if (!MerchantFrame.Instance.IsVisible)
                        return RunStatus.Running;

                    Lua.DoString("RepairAllItems()");
                    StyxWoW.SleepForLagDuration();
                    MerchantFrame.Instance.Close();
                    return RunStatus.Success;
                })
            );
        }

        /// <summary>
        /// Move to the nearest sell vendor, sell all, optionally repair.
        /// WoD: method_6()
        /// </summary>
        private Composite CreateSellBehavior()
        {
            return new PrioritySelector(
                new Action(ctx =>
                {
                    var vendor = ProfileManager.CurrentProfile?.VendorManager?
                        .GetClosestVendor(Vendor.VendorType.Sell);
                    if (vendor == null) return RunStatus.Failure;

                    var vendorUnit = ObjectManager.GetObjectsOfType<WoWUnit>()
                        .Where(u => u.IsValid && u.Entry == (uint)vendor.Entry && u.IsAlive)
                        .OrderBy(u => u.DistanceSqr)
                        .FirstOrDefault();

                    if (vendorUnit == null)
                    {
                        TreeRoot.StatusText = "Moving to vendor";
                        if (Flightor.CanFly)
                            Flightor.MoveTo(vendor.Location, 40f);
                        else
                            Navigator.MoveTo(vendor.Location);
                        return RunStatus.Running;
                    }

                    if (!vendorUnit.WithinInteractRange)
                    {
                        Navigator.MoveTo(vendorUnit.Location);
                        return RunStatus.Running;
                    }

                    WoWMovement.MoveStop();
                    Log("Selling at {0}.", vendorUnit.Name);
                    vendorUnit.Interact();
                    StyxWoW.SleepForLagDuration();

                    // Some NPCs show a gossip frame before the merchant frame.
                    // Select the first option ("Browse Wares") to open the merchant frame.
                    // WoD: method_95/96 — DecoratorContinue(!MerchantFrame.IsVisible && GossipFrame.IsVisible)
                    if (!MerchantFrame.Instance.IsVisible && GossipFrame.Instance.IsVisible)
                    {
                        GossipFrame.Instance.SelectGossipOption(0);
                        StyxWoW.SleepForLagDuration();
                    }

                    if (!MerchantFrame.Instance.IsVisible)
                        return RunStatus.Running;

                    Vendors.ForceSell = true;
                    if (GatherBuddySettings.Instance.RepairAtVendor)
                    {
                        Lua.DoString("RepairAllItems()");
                        StyxWoW.SleepForLagDuration();
                    }
                    MerchantFrame.Instance.Close();
                    Vendors.ForceSell = false;
                    return RunStatus.Success;
                })
            );
        }

        /// <summary>
        /// Returns true when the bags are considered full for whichever gather types are active.
        /// WoD: method_17() — used BagHelper.EmptyHerbSlots / EmptyMineSlots.
        /// CopilotBuddy: StyxWoW.Me.FreeBagSlots vs. GatherBuddySettings.MinFreeBagSlots.
        /// </summary>
        private bool NeedsBagsEmptied(object ctx)
        {
            if (StyxWoW.Me == null || StyxWoW.Me.Combat || StyxWoW.Me.IsDead || StyxWoW.Me.IsGhost)
                return false;
            if (!GatherBuddySettings.Instance.VendorWhenFull)
                return false;

            uint minFree      = (uint)GatherBuddySettings.Instance.MinFreeBagSlots;
            bool herbsFull    = !GatherBuddySettings.Instance.GatherHerbs    || BagHelper.EmptyHerbSlots <= minFree;
            bool mineralsFull = !GatherBuddySettings.Instance.GatherMinerals || BagHelper.EmptyMineSlots <= minFree;
            return herbsFull && mineralsFull;
        }

        /// <summary>
        /// Returns true when MailToAlt is on, a recipient is set, a mailbox exists in the profile,
        /// and there are non-soulbound items that qualify under the mail quality filters.
        /// WoD: method_17 variant — smethod_160 (canRunDecoratorDelegate_65 gate).
        /// </summary>
        private bool NeedsMailing(object ctx)
        {
            var s = GatherBuddySettings.Instance;
            if (!s.MailToAlt) return false;
            if (string.IsNullOrEmpty(s.MailRecipient)) return false;
            if (ProfileManager.CurrentProfile?.MailboxManager == null) return false;
            if (StyxWoW.Me == null || StyxWoW.Me.Combat || StyxWoW.Me.IsDead || StyxWoW.Me.IsGhost) return false;
            return GetItemsToMail().Length > 0;
        }

        /// <summary>
        /// Collects carried items that should be mailed based on GatherBuddySettings quality flags.
        /// Non-soulbound, non-conjured, not protected, quality matches enabled flags.
        /// WoD: smethod_12 / list_3 population logic in GatherbuddyBot.
        /// </summary>
        private WoWItem[] GetItemsToMail()
        {
            var s = GatherBuddySettings.Instance;
            return StyxWoW.Me?.CarriedItems
                .Where(item => item.IsValid
                    && !item.IsSoulbound
                    && !item.IsConjured
                    && !Styx.Logic.Profiles.ProtectedItemsManager.Contains(item.Entry)
                    && IsMailQuality(item.Quality, s))
                .ToArray() ?? Array.Empty<WoWItem>();
        }

        private static bool IsMailQuality(WoWItemQuality quality, GatherBuddySettings s)
        {
            switch (quality)
            {
                case WoWItemQuality.Poor:     return s.MailGrey;
                case WoWItemQuality.Common:   return s.MailWhite;
                case WoWItemQuality.Uncommon: return s.MailGreen;
                case WoWItemQuality.Rare:     return s.MailBlue;
                case WoWItemQuality.Epic:     return s.MailPurple;
                default:                      return false;
            }
        }

        /// <summary>
        /// Navigate to the closest profiled mailbox, interact with the in-world mailbox game object,
        /// then call SendMailWithManyAttachments. Mirrors HB 4.3.4 smethod_160–169 flow.
        /// </summary>
        private Composite CreateMailBehavior()
        {
            return new PrioritySelector(
                new Action(ctx =>
                {
                    Mailbox profileMailbox = ProfileManager.CurrentProfile?.MailboxManager?.GetClosestMailbox();
                    if (profileMailbox == null) return RunStatus.Failure;

                    // Find the actual in-world mailbox game object near the profile location.
                    WoWGameObject mailboxGo = ObjectManager.GetObjectsOfType<WoWGameObject>()
                        .Where(go => go.IsValid && go.IsMailbox &&
                                     go.Location.DistanceSqr(profileMailbox.Location) < 225f) // 15y
                        .OrderBy(go => go.DistanceSqr)
                        .FirstOrDefault();

                    // Still far from profile point — fly/walk there.
                    if (mailboxGo == null &&
                        StyxWoW.Me.Location.DistanceSqr(profileMailbox.Location) > 400f) // 20y
                    {
                        TreeRoot.StatusText = "Moving to mailbox";
                        if (Flightor.CanFly)
                            Flightor.MoveTo(profileMailbox.Location, 40f);
                        else
                            Navigator.MoveTo(profileMailbox.Location);
                        return RunStatus.Running;
                    }

                    // Close to profile point: also accept any mailbox in expanded radius.
                    if (mailboxGo == null)
                    {
                        mailboxGo = ObjectManager.GetObjectsOfType<WoWGameObject>()
                            .Where(go => go.IsValid && go.IsMailbox)
                            .OrderBy(go => go.DistanceSqr)
                            .FirstOrDefault();
                    }

                    if (mailboxGo == null) return RunStatus.Failure;

                    if (!mailboxGo.WithinInteractRange)
                    {
                        TreeRoot.StatusText = "Approaching mailbox";
                        Navigator.MoveTo(mailboxGo.Location);
                        return RunStatus.Running;
                    }

                    // At mailbox: stop, interact, wait for mail frame.
                    WoWMovement.MoveStop();
                    if (!MailFrame.Instance.IsVisible)
                    {
                        mailboxGo.Interact();
                        StyxWoW.SleepForLagDuration();
                        return RunStatus.Running;
                    }

                    WoWItem[] items = GetItemsToMail();
                    if (items.Length > 0)
                    {
                        Log("Mailing {0} item(s) to {1}.", items.Length, GatherBuddySettings.Instance.MailRecipient);
                        MailFrame.Instance.SendMailWithManyAttachments(
                            GatherBuddySettings.Instance.MailRecipient, 0, items);
                        StyxWoW.SleepForLagDuration();
                    }

                    MailFrame.Instance.Close();
                    return RunStatus.Success;
                })
            );
        }

        #endregion

        #region Gather Behavior

        /// <summary>
        /// Core 7-child PrioritySelector: detect node change → blacklist on failure →
        /// timeout blacklist → fly to approach point → descend → ground approach → interact/loot.
        /// WoD: the inner PrioritySelector on array7 inside method_2().
        /// </summary>
        private Composite CreateGatherBehavior()
        {
            return new PrioritySelector(

                // [0] Node changed or _currentNode invalidated — reset all per-node state.
                //     Also opportunistically advances the waypoint queue if the node is
                //     closer to the next waypoint than the current one.
                //     WoD: Class670.method_26–33
                new Decorator(
                    ctx => _currentNode == null ||
                           !_currentNode.IsValid ||
                           _currentNode != LootTargeting.Instance.FirstObject,
                    new Sequence(
                        new DecoratorContinue(
                            ctx =>
                            {
                                if (_waypointQueue == null || _waypointQueue.Count < 3) return false;
                                var firstObj = LootTargeting.Instance.FirstObject;
                                if (firstObj == null) return false;
                                // If the next waypoint is farther from the node than the current
                                // waypoint, skip forward so we approach more directly.
                                return _waypointQueue.ElementAt(1).Distance2D(_waypointQueue.Peek()) >
                                       _waypointQueue.ElementAt(1).Distance2D(firstObj.Location);
                            },
                            new Action(ctx =>
                            {
                                _waypointQueue.Dequeue();
                                return RunStatus.Success;
                            })
                        ),
                        new Action(ctx => { _gatherAttemptCount = 0;           return RunStatus.Success; }),
                        new Action(ctx => { _gatherTimer.Reset();               return RunStatus.Success; }),
                        new Action(ctx => { _gatherTimer.Start();               return RunStatus.Success; }),
                        new Action(ctx => { _approachPoint = WoWPoint.Zero;     return RunStatus.Success; }),
                        new Action(ctx =>
                        {
                            _currentNode = LootTargeting.Instance.FirstObject;
                            return RunStatus.Success;
                        })
                    )
                ),

                // [1] Three failed interact attempts — blacklist for 15 minutes and release.
                //     WoD: Class670.method_34/35
                new Decorator(
                    ctx => _currentNode != null &&
                           _gatherAttemptCount >= 3 &&
                           LootTargeting.Instance.FirstObject == _currentNode,
                    new Action(ctx =>
                    {
                        BlacklistNodes[_currentNode.Location] = "Failed 3 times";
                        Blacklist.Add(_currentNode.Guid, TimeSpan.FromMinutes(15));
                        Log("Blacklisted {0} after 3 failed attempts.", _currentNode.Name);
                        _gatherAttemptCount = 0;
                        _approachPoint      = WoWPoint.Zero;
                        _gatherTimer.Reset();
                        _currentNode        = null;
                        return RunStatus.Success;
                    })
                ),

                // [2] Gather timer exceeded BlacklistTimer seconds — timeout blacklist.
                //     WoD: Class670.method_36/37
                new Decorator(
                    ctx => _gatherTimer.IsRunning &&
                           _gatherTimer.Elapsed.TotalSeconds > GatherBuddySettings.Instance.BlacklistTimer,
                    new Action(ctx =>
                    {
                        if (_currentNode != null)
                        {
                            BlacklistNodes[_currentNode.Location] = "Timeout";
                            Blacklist.Add(_currentNode.Guid, TimeSpan.FromMinutes(15));
                            Log("Blacklisted {0} (gather timeout).", _currentNode.Name);
                        }
                        _approachPoint = WoWPoint.Zero;
                        _gatherTimer.Reset();
                        _currentNode   = null;
                        return RunStatus.Success;
                    })
                ),

                // [3] Fly to the calculated approach point.
                //     ContextChangeHandler calls CalculateApproachPoint() every tick and stores
                //     the result in _approachPoint. The Decorator condition checks whether we
                //     still need to travel (height diff > 15y, or 2D dist > 2.5y).
                //     Guard: after [4] dismounts and resets _approachPoint=Zero, the next tick
                //     recalculates a point 2.5-3.5y from the node. Without the ground check,
                //     [3] would immediately re-trigger Flightor.MoveTo() even though [5] can
                //     walk the remaining distance — causing the infinite mount loop.
                //     WoD: PrioritySelector(CTX=method_38, [Decorator(method_41, Seq(42,43))])
                new PrioritySelector(new ContextChangeHandler(CalculateApproachPoint),
                    new Decorator(
                        ctx =>
                        {
                            // Already on the ground within walking distance — let [5] handle it.
                            if (!StyxWoW.Me.MovementInfo.IsFlying
                                && _currentNode != null
                                && _currentNode.Location.Distance2D(StyxWoW.Me.Location) <= 18f)
                                return false;

                            return Math.Abs(StyxWoW.Me.Location.Z - _approachPoint.Z) > 15f ||
                                   _approachPoint.Distance2DSqr(StyxWoW.Me.Location) >= 6.25f;
                        },
                        new Sequence(
                            new Action(ctx =>
                            {
                                ulong guid = _currentNode?.Guid ?? 0;
                                if (guid != _lastLoggedNodeGuid)
                                {
                                    _lastLoggedNodeGuid = guid;
                                    Log("Flying to {0} ({1:F0}y).",
                                        _currentNode != null ? _currentNode.Name : "node",
                                        _approachPoint.Distance(StyxWoW.Me.Location));
                                }
                                return RunStatus.Success;
                            }),
                            new ActionSetActivity("Flying to node"),
                            new Action(ctx =>
                            {
                                Flightor.MoveTo(_approachPoint, 40f);
                                return RunStatus.Success;
                            })
                        )
                    )
                ),

                // [4] Still airborne after reaching approach point — descend and dismount.
                //     Clear _approachPoint so CalculateApproachPoint recomputes after landing.
                //     WoD: Decorator(method_44, Seq(45,46,WC(47),DC(48,49),50))
                new Decorator(
                    ctx => StyxWoW.Me.MovementInfo.IsFlying,
                    new Sequence(
                        new Action(ctx => { _approachPoint = WoWPoint.Zero; return RunStatus.Success; }),
                        new Action(ctx =>
                        {
                            WoWMovement.Move(WoWMovement.MovementDirection.Descend);
                            return RunStatus.Success;
                        }),
                        new WaitContinue(1,
                            ctx => !StyxWoW.Me.MovementInfo.IsFlying,
                            new ActionAlwaysSucceed()),
                        // Dismount if still airborne or if we overshot and are out of range.
                        new DecoratorContinue(
                            ctx => StyxWoW.Me.MovementInfo.IsFlying ||
                                   (_currentNode != null && !_currentNode.WithinInteractRange),
                            new Action(ctx =>
                            {
                                Flightor.MountHelper.Dismount();
                                return RunStatus.Success;
                            })
                        ),
                        new Action(ctx => { WoWMovement.MoveStop(); return RunStatus.Success; })
                    )
                ),

                // [5] On the ground but not yet in interact range — navigate or ClickToMove.
                //     ContextChangeHandler provides the target WoWPoint as ctx.
                //     WoD: Decorator(method_51, PS(CTX=method_52, [DC(53,54), Action(55)]))
                new Decorator(
                    ctx => _currentNode != null && !_currentNode.WithinInteractRange,
                    new PrioritySelector(new ContextChangeHandler(
                            ctx => _currentNode != null ? (object)_currentNode.Location : WoWPoint.Zero),
                        new Decorator(
                            ctx => ctx is WoWPoint pt &&
                                   Navigator.CanNavigateFully(StyxWoW.Me.Location, pt),
                            new Action(ctx =>
                            {
                                Navigator.MoveTo((WoWPoint)ctx);
                                return RunStatus.Success;
                            })
                        ),
                        new Action(ctx =>
                        {
                            if (ctx is WoWPoint pt)
                                WoWMovement.ClickToMove(pt);
                            return RunStatus.Success;
                        })
                    )
                ),

                // [6] Within interact range — stop, face, interact, loot.
                //     ContextChangeHandler provides FirstObject as ctx throughout the sequence.
                //     WoD: Sequence(CTX=method_56, [57,WC(58,59),WC(60,61),SetActivity,
                //                                   62,DC(63,64),Lag,65,Wait(5,66,Seq(67,WC(68))),
                //                                   DC(69,method_15)])
                new Sequence(new ContextChangeHandler(ctx => LootTargeting.Instance.FirstObject),
                    new Action(ctx => { WoWMovement.MoveStop(); return RunStatus.Success; }),
                    // Wait until we have fully stopped falling before interacting.
                    new WaitContinue(5,
                        ctx => !StyxWoW.Me.IsFalling,
                        new Action(ctx => { WoWMovement.MoveStop(); return RunStatus.Success; })
                    ),
                    // Wait until movement has fully stopped.
                    new WaitContinue(1,
                        ctx => !StyxWoW.Me.IsMoving,
                        new Action(ctx => { WoWMovement.MoveStop(); return RunStatus.Success; })
                    ),
                    new ActionSetActivity("Gathering"),
                    // Count this interact attempt for the failure-blacklist check above.
                    new Action(ctx => { _gatherAttemptCount++; return RunStatus.Success; }),
                    // Face the node if FaceNodes is on — reduces "out of range" failures.
                    new DecoratorContinue(
                        ctx => ctx is WoWGameObject && GatherBuddySettings.Instance.FaceNodes,
                        new Action(ctx =>
                        {
                            if (ctx is WoWGameObject go)
                                StyxWoW.Me.SetFacing(go);
                            return RunStatus.Success;
                        })
                    ),
                    // One lag tick before interact to let the server acknowledge our position.
                    new Action(ctx => { StyxWoW.SleepForLagDuration(); return RunStatus.Success; }),
                    new Action(ctx =>
                    {
                        if (ctx is WoWObject obj)
                            obj.Interact();
                        return RunStatus.Success;
                    }),
                    // Wait up to 5 seconds for loot frame, combat, flight, or node disappears.
                    new Wait(5,
                        ctx =>
                        {
                            if (StyxWoW.Me.Combat) return true;
                            if (StyxWoW.Me.MovementInfo.IsFlying) return true;
                            if (LootFrame.Instance.IsVisible && LootFrame.Instance.LootItems > 0) return true;
                            if (ctx == null || (WoWObject)ctx != _currentNode) return true;
                            return false;
                        },
                        new Sequence(
                            // Loot all items from the frame. WoD: method_67
                            new Action(ctx =>
                            {
                                if (StyxWoW.Me.Combat)
                                    return RunStatus.Success;

                                WoWObject obj = ObjectManager.GetAnyObjectByGuid<WoWObject>(
                                    LootFrame.Instance.LootingObjectGuid);
                                if (obj != null)
                                {
                                    string name = obj.Name;
                                    if (obj is WoWGameObject)
                                    {
                                        if (!NodeCollectionCount.ContainsKey(name))
                                            NodeCollectionCount.Add(name, 0);
                                        NodeCollectionCount[name]++;
                                    }
                                    LootFrame.Instance.LootAll();
                                    _approachPoint = WoWPoint.Zero;
                                    Log("{0} {1}.", obj is WoWGameObject ? "Harvested" : "Looted", name);
                                }
                                return RunStatus.Success;
                            }),
                            // Abort wait early if combat fires, node is gone, or node becomes unusable.
                            // WoD: method_68
                            new WaitContinue(2,
                                ctx => StyxWoW.Me.Combat ||
                                       ctx == null ||
                                       (ctx is WoWObject wo && !wo.IsValid) ||
                                       (ctx is WoWGameObject wgo && !wgo.CanUse()),
                                new ActionAlwaysSucceed())
                        )
                    ),
                    // Node depleted after loot — advance the waypoint queue.
                    // WoD: Decorator(method_69, method_15)
                    new Decorator(
                        ctx => _currentNode == null ||
                               !_currentNode.IsValid ||
                               (_currentNode is WoWGameObject g && !g.CanLoot),
                        new Action(ctx =>
                        {
                            if (_waypointQueue != null && _waypointQueue.Count > 0)
                                _waypointQueue.Dequeue();
                            return RunStatus.Success;
                        })
                    )
                )
            );
        }

        #endregion

        #region Calculate Approach Point

        /// <summary>
        /// Computes the best landing point around the current node using 8-direction raycasting.
        /// Called every tick as a ContextChangeHandler; result cached in _approachPoint.
        /// WoD: method_38
        /// </summary>
        private object CalculateApproachPoint(object ctx)
        {
            using (StyxWoW.Memory.AcquireFrame(true))
            {
                WoWObject firstObject = LootTargeting.Instance.FirstObject;
                if (firstObject == null)
                    return _approachPoint;

                WoWPoint nodeLocation = firstObject.Location;
                WoWPoint myLocation   = StyxWoW.Me.Location;

                // No flight: just use the node's position directly.
                if (!Flightor.CanFly)
                    return _approachPoint = nodeLocation;

                // Very close and been trying for 10 s: land on the node itself.
                if (nodeLocation.Distance2DSqr(myLocation) <= 25f &&
                    _gatherTimer.Elapsed.TotalSeconds >= 10.0)
                    return _approachPoint = nodeLocation;

                // Already have a valid approach point: keep using it.
                if (_approachPoint != WoWPoint.Zero)
                    return _approachPoint;

                Logging.WriteDebug("[ApproachPt] Recalculating approach point (was Zero). Player Z={0:F1}, CanFly={1}, IsFlying={2}",
                    myLocation.Z, Flightor.CanFly, StyxWoW.Me.MovementInfo.IsFlying);
                // Already in interact range: no movement needed.
                if (firstObject.WithinInteractRange)
                    return _approachPoint = myLocation;

                // Cast 8 rays at 45° intervals, 2.5 y out from the node centre.
                // Pick the highest-Z hit within 3.5 y as the ideal landing spot.
                float facing = WoWMathHelper.CalculateNeededFacing(myLocation, nodeLocation);
                var candidatePoints = new[]
                {
                    nodeLocation.RayCast(facing - 3.14159274f, 2.5f),
                    nodeLocation.RayCast(facing - 2.3561945f,  2.5f),
                    nodeLocation.RayCast(facing - 1.57079637f, 2.5f),
                    nodeLocation.RayCast(facing - 0.7853982f,  2.5f),
                    nodeLocation.RayCast(facing,               2.5f),
                    nodeLocation.RayCast(facing + 0.7853982f,  2.5f),
                    nodeLocation.RayCast(facing + 1.57079637f, 2.5f),
                    nodeLocation.RayCast(facing + 2.3561945f,  2.5f)
                };

                var lines = candidatePoints
                    .Select(p => new WorldLine(p.Add(0f, 0f, 20f), p.Add(0f, 0f, -20f)))
                    .ToArray();

                bool[]     hitArr;
                WoWPoint[] hitPoints;
                GameWorld.MassTraceLine(lines, GameWorld.TraceLineHitFlags.Collision, out hitArr, out hitPoints);

                WoWPoint best = hitPoints
                    .Where(p => nodeLocation.DistanceSqr(p) <= 12.25f  // 3.5 y radius
                             && p.Z <= nodeLocation.Z + 2f)             // HB used +6f but WotLK rocks pass that → tighten to +2f
                    .OrderByDescending(p => p.Z)                         // HB 6.2.3 method_40: highest Z first (correct HB behavior)
                    .FirstOrDefault();

                WoWPoint newApproach = (best != WoWPoint.Zero)
                    ? best
                    : nodeLocation.Add(0f, 0f, 2f); // fallback: 2 y above node

                return _approachPoint = newApproach;
            }
        }

        #endregion

        #region Targeting Filters

        /// <summary>
        /// On ground only: score nodes by actual path distance rather than straight-line distance.
        /// WoD: method_11
        /// </summary>
        private void WeighLootTargetsFilter(List<Targeting.TargetPriority> targets)
        {
            if (Flightor.CanFly)
                return; // Flying — straight-line order is fine.

            foreach (Targeting.TargetPriority tp in targets)
            {
                tp.Score = 0.0;
                float? dist = Navigator.PathDistance(StyxWoW.Me.Location, tp.Object.Location, float.MaxValue);
                tp.Score = dist.HasValue ? -(double)dist.Value : -(double)tp.Object.Distance;
            }
        }

        /// <summary>
        /// Adds units that have aggro on us so the combat subtree can react.
        /// Not active while flying — no point engaging while airborne.
        /// WoD: method_12
        /// </summary>
        private void IncludeTargetsFilter(List<WoWObject> incoming, HashSet<WoWObject> outgoing)
        {
            if (StyxWoW.Me.IsFlying)
                return;

            foreach (WoWUnit unit in incoming.OfType<WoWUnit>())
            {
                if (unit.ThreatInfo.ThreatStatus > ThreatStatus.UnitNotInThreatTable)
                    outgoing.Add(unit);
            }
        }

        /// <summary>
        /// Pass-through: all loot candidates enter the RemoveLootFilter stage.
        /// WoD: method_13
        /// </summary>
        private void IncludeLootFilter(List<WoWObject> incoming, HashSet<WoWObject> outgoing)
        {
            foreach (WoWObject obj in incoming)
                outgoing.Add(obj);
        }

        /// <summary>
        /// Prunes the loot candidate list: blacklisted, wrong type, bags full,
        /// ninja protection, elite guard, underwater, unnavigable, underground.
        /// WoD: method_14
        /// </summary>
        private void RemoveLootFilter(List<WoWObject> list)
        {
            bool canFly       = Flightor.CanFly;
            WoWPoint myLoc    = StyxWoW.Me.Location;
            uint minFree      = (uint)GatherBuddySettings.Instance.MinFreeBagSlots;
            bool herbsFull    = !GatherBuddySettings.Instance.GatherHerbs    || BagHelper.EmptyHerbSlots <= minFree;
            bool mineralsFull = !GatherBuddySettings.Instance.GatherMinerals || BagHelper.EmptyMineSlots <= minFree;

            var nearbyHostiles = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(u => u.IsValid && u.IsAlive && u.IsHostile)
                .ToList();

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (!list[i].IsValid || list[i].BaseAddress == IntPtr.Zero)
                    { list.RemoveAt(i); continue; }

                if (Blacklist.Contains(list[i].Guid))
                    { list.RemoveAt(i); continue; }

                // Units: keep if LootMobs+CanLoot, or SkinMobs+CanSkin.
                if (list[i] is WoWUnit unit)
                {
                    bool wantLoot = GatherBuddySettings.Instance.LootMobs && unit.CanLoot;
                    bool wantSkin = GatherBuddySettings.Instance.SkinMobs && unit.CanSkin;
                    if (!wantLoot && !wantSkin)
                        list.RemoveAt(i);
                    continue;
                }

                // Anything that isn't a WoWGameObject is irrelevant here.
                if (!(list[i] is WoWGameObject go))
                    { list.RemoveAt(i); continue; }

                if (_avoidList.Contains(go.Entry))                                                                                    { list.RemoveAt(i); continue; }
                if (GatherBuddySettings.Instance.BlacklistedEntries.Contains(go.Entry))                                                { list.RemoveAt(i); continue; }
                if (!go.IsHerb && !go.IsMineral && !(GatherBuddySettings.Instance.GatherChests && go.IsChest && go.CanLoot))            { list.RemoveAt(i); continue; }
                if (go.IsHerb     && herbsFull)        { list.RemoveAt(i); continue; }
                if (go.IsMineral  && mineralsFull)     { list.RemoveAt(i); continue; }
                if (!go.CanLoot)                       { list.RemoveAt(i); continue; }

                WoWPoint nodePos = go.Location;

                if (IsNodeBlacklisted(nodePos))
                    { list.RemoveAt(i); continue; }

                if (BlackspotManager.IsBlackspotted(nodePos))
                    { list.RemoveAt(i); continue; }

                // HB 6.2.3 method_14: also filter nodes inside aerial (Flightor) blackspots.
                if (Styx.Logic.Pathing.FlightorNavigation.BlackspotManager.IsInBlackspot(nodePos))
                {
                    Logging.WriteDebug("[GB Filter] Skipping aerial-blackspot node: {0} at {1}", go.Name, nodePos);
                    list.RemoveAt(i);
                    continue;
                }

                // NoNinja: skip any node another mounted player is clearly heading toward.
                if (GatherBuddySettings.Instance.NoNinja && Flightor.MountHelper.Mounted)
                {
                    bool playerNearby = ObjectManager.GetObjectsOfType<WoWPlayer>()
                        .Any(p => !p.IsMe && p.IsAlive && p.Location.DistanceSqr(nodePos) < 15f * 15f);
                    if (playerNearby)
                    {
                        Blacklist.Add(go.Guid, TimeSpan.FromSeconds(5));
                        list.RemoveAt(i);
                        continue;
                    }
                }

                // IgnoreElites: skip nodes guarded by an elite within 30 y.
                if (GatherBuddySettings.Instance.IgnoreElites)
                {
                    var elite = nearbyHostiles.FirstOrDefault(
                        u => u.Elite && u.Location.DistanceSqr(nodePos) < 30f * 30f);
                    if (elite != null)
                    {
                        Blacklist.Add(go.Guid, TimeSpan.FromSeconds(5));
                        list.RemoveAt(i);
                        continue;
                    }
                }

                // Position validation — run once per unique node position then cache.
                if (!_validatedNodePositions.Contains(nodePos))
                {
                    // Underwater: a traceline from sky down to node hits liquid surface.
                    // MUST use CGWorldFrameHitFlags (native WoW Intersect) — the TraceLineHitFlags
                    // overload only handles navmesh and returns false for Liquid flag.
                    if (GameWorld.TraceLine(nodePos.Add(0f, 0f, 1000f), nodePos,
                        GameWorld.CGWorldFrameHitFlags.HitTestLiquid))
                    {
                        BlacklistNodes[nodePos] = "Underwater node";
                        Logging.WriteDebug("[GB Filter] Blacklisted underwater node: {0} at {1}", go.Name, nodePos);
                        list.RemoveAt(i);
                        continue;
                    }

                    // Unnavigable from current position (ground-only check).
                    if (!canFly && !Navigator.CanNavigateWithin(myLoc, nodePos, 2.5f))
                    {
                        BlacklistNodes[nodePos] = "Unnavigable node";
                        list.RemoveAt(i);
                        continue;
                    }

                    // Underground/indoor: HB 6.2.3 uses the game's own IsOutdoors check
                    // (client routine 0x71B7F0) rather than a TraceLine from the sky. TraceLine
                    // from 200y above falsely blacklists nodes under cliff overhangs and arches
                    // that are outdoors and perfectly gatherable — exactly the root cause of
                    // the "oscillation at 20y from node" flight bug. IsOutdoors is reliable
                    // on WoWGameObjects in CB (WoWObject.cs calls 0x71B7F0 with object ECX).
                    if (canFly && !go.IsOutdoors)
                    {
                        BlacklistNodes[nodePos] = "Underground node";
                        Logging.WriteDebug("[GB Filter] Blacklisted indoor node: {0} at {1}", go.Name, nodePos);
                        list.RemoveAt(i);
                        continue;
                    }

                    _validatedNodePositions.Add(nodePos);
                }
            }
        }

        #endregion

        #region Blacklist Helpers

        /// <summary>
        /// Pre-populates BlacklistNodes with known-bad positions per continent.
        /// Called on Start(). WoD: smethod_6
        /// </summary>
        private static void InitBlacklist()
        {
            BlacklistNodes.Clear();
            uint mapId = StyxWoW.Me.MapId;

            if (mapId == 0U) // Eastern Kingdoms
            {
                BlacklistNodes[new WoWPoint(-4515.803, -4921.126, 158.473)] = "Default blacklist";
                BlacklistNodes[new WoWPoint(-4515.803, -4919.83,  158.473)] = "Default blacklist";
                BlacklistNodes[new WoWPoint(-4592.35,  -4893.94,  163.83)]  = "Default blacklist";
                BlacklistNodes[new WoWPoint(-4499.49,  -4460.0,   174.916)] = "Default blacklist";
                BlacklistNodes[new WoWPoint(-4160.62,  -4403.73,  195.035)] = "Default blacklist";
                return;
            }
            if (mapId == 1U) // Kalimdor
            {
                BlacklistNodes[new WoWPoint(-10298.63, -306.4288, 253.4157)] = "Default blacklist";
                return;
            }
            if (mapId == 530U) // Outland
            {
                BlacklistNodes[new WoWPoint(2612.169, 5853.195, 12.9664)] = "Default blacklist";
            }
        }

        /// <summary>
        /// Returns true if pos is within 2 y of any entry in BlacklistNodes.
        /// WoD: smethod_7 (2-arg overload with out params)
        /// </summary>
        private static bool IsNodeBlacklisted(WoWPoint pos, out WoWPoint blacklistKey, out string reason)
        {
            blacklistKey = WoWPoint.Empty;
            reason       = string.Empty;
            foreach (var kvp in BlacklistNodes)
            {
                if (kvp.Key.DistanceSqr(pos) < 4f) // 2 y radius
                {
                    blacklistKey = kvp.Key;
                    reason       = kvp.Value;
                    return true;
                }
            }
            return false;
        }

        /// <summary>WoD: smethod_7 (1-arg overload)</summary>
        private static bool IsNodeBlacklisted(WoWPoint pos)
        {
            WoWPoint k;
            string r;
            return IsNodeBlacklisted(pos, out k, out r);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Loads additional _avoidList entries from an external XML file.
        /// In WotLK the hardcoded _avoidList covers all known avoid entries; this is a no-op.
        /// WoD: smethod_9
        /// </summary>
        private static void LoadAvoidList() { }

        private static void Log(string format, params object[] args)
        {
            Logging.Write("[GatherBuddy] " + (args.Length > 0 ? string.Format(format, args) : format));
        }

        private static void ShuffleList<T>(List<T> list)
        {
            var rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T val  = list[k];
                list[k] = list[n];
                list[n] = val;
            }
        }

        /// <summary>
        /// Logs herb and mineral counts visible in the ObjectManager.
        /// Useful for diagnosing why no nodes are detected.
        /// </summary>
        public static void DiagnoseNodeDetection()
        {
            int herbs    = ObjectManager.GetObjectsOfType<WoWGameObject>().Count(o => o.IsValid && o.IsHerb);
            int minerals = ObjectManager.GetObjectsOfType<WoWGameObject>().Count(o => o.IsValid && o.IsMineral);
            Log("Diagnosis: {0} herbs, {1} minerals visible.", herbs, minerals);
        }

        /// <summary>
        /// Prints a harvest summary to the log. Can be called at any time.
        /// </summary>
        public static void StatusReport()
        {
            Log("Running for {0}h {1}m {2}s. Harvested {3} nodes total.",
                RunningTime.Hours, RunningTime.Minutes, RunningTime.Seconds,
                NodeCollectionCount.Values.Sum());
            foreach (var kvp in NodeCollectionCount)
                Log("  {0}: {1}", kvp.Key, kvp.Value);
        }

        #endregion
    }
}
