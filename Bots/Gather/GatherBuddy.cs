using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using CommonBehaviors.Actions;
using Styx;
using Styx.Combat.CombatRoutine;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Combat;
using Styx.Logic.Inventory;
using Styx.Logic.Inventory.Frames;
using Styx.Logic.Inventory.Frames.Gossip;
using Styx.Logic.Inventory.Frames.MailBox;
using Styx.Logic.Inventory.Frames.Merchant;
using Styx.Logic.Pathing;
using Styx.Logic.POI;
using Styx.Logic.Profiles;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Action = TreeSharp.Action;

namespace Bots.Gather
{
    /// <summary>
    /// GatherBuddy - Full-featured gathering bot for WoW 3.3.5a (WotLK).
    /// Harvests herbs, minerals, chests, skins mobs along a waypoint route.
    /// Supports profile vendors, mailboxes, blackspots, sell/mail quality filters,
    /// full combat behaviors, death handling with spirit healer, and session timers.
    /// </summary>
    public class GatherBuddy : BotBase
    {
        // ═══════════════════════════════════════════════════════════
        // BOTBASE IMPLEMENTATION
        // ═══════════════════════════════════════════════════════════

        public override string Name => "GatherBuddy";
        public override bool IsPrimaryType => true;
        public override bool RequiresProfile => true;
        public override bool RequirementsMet => true;
        public override PulseFlags PulseFlags => PulseFlags.All;

        /// <summary>
        /// Returns the settings window shown when "Bot Config" is clicked.
        /// </summary>
        public override object ConfigurationForm => new GatherBuddySettingsWindow();

        private PrioritySelector? _root;
        private CircularQueue<WoWPoint>? _waypointQueue;
        private List<WoWPoint> _waypoints = new();
        private WoWGameObject? _currentNode;
        private readonly Stopwatch _cleanupTimer = new();
        private static CombatRoutine? Routine => RoutineManager.Current;

        // Stats tracking
        private int _nodesGathered;
        private int _herbsGathered;
        private int _mineralsGathered;
        private readonly Stopwatch _sessionTimer = new();

        // Death tracking (corpse camp protection)
        private int _deathCount;
        private readonly Stopwatch _deathTimer = new();
        private bool _shouldUseSpiritHealer;

        // Loot tracking
        private int _lootAttemptCount;
        private int _lootFailCount;
        private ulong _lastLootGuid;

        // Session timer (BottingHours)
        private readonly Stopwatch _bottingTimer = new();

        // Random for hotspot shuffling and jiggle
        private static readonly Random _random = new();

        public override Composite Root => _root ??= CreateRootBehavior();

        /// <summary>
        /// Session statistics.
        /// </summary>
        public int NodesGathered => _nodesGathered;
        public int HerbsGathered => _herbsGathered;
        public int MineralsGathered => _mineralsGathered;
        public TimeSpan SessionTime => _sessionTimer.Elapsed;

        // ═══════════════════════════════════════════════════════════
        // LIFECYCLE
        // ═══════════════════════════════════════════════════════════

        public override void Initialize()
        {
            NodeTracker.LoadBlacklist();
            Logging.Write("[GatherBuddy] Initialized (blacklist loaded)");
        }

        public override void Start()
        {
            Logging.Write("[GatherBuddy] Starting...");
            var settings = GatherBuddySettings.Instance;

            // Reset stats
            _nodesGathered = 0;
            _herbsGathered = 0;
            _mineralsGathered = 0;
            _deathCount = 0;
            _shouldUseSpiritHealer = false;
            _lootAttemptCount = 0;
            _lootFailCount = 0;
            _lastLootGuid = 0;
            _sessionTimer.Restart();
            _bottingTimer.Restart();

            // Load waypoints from ProfileManager
            if (ProfileManager.CurrentProfile == null)
                throw new Exception("[GatherBuddy] No profile loaded. Use 'Load Profile' button first.");

            _waypoints.Clear();
            float heightMod = settings.HeightModifier;

            if (ProfileManager.CurrentProfile.GrindArea?.Hotspots != null &&
                ProfileManager.CurrentProfile.GrindArea.Hotspots.Count > 0)
            {
                Logging.Write("[GatherBuddy] Loading waypoints from GrindArea");
                foreach (var hotspot in ProfileManager.CurrentProfile.GrindArea.Hotspots)
                    _waypoints.Add(hotspot.ToWoWPoint().Add(0f, 0f, heightMod));
            }
            else if (ProfileManager.CurrentProfile.HotspotManager?.Hotspots != null &&
                     ProfileManager.CurrentProfile.HotspotManager.Hotspots.Count > 0)
            {
                Logging.Write("[GatherBuddy] Loading waypoints from HotspotManager");
                foreach (var point in ProfileManager.CurrentProfile.HotspotManager.Hotspots)
                    _waypoints.Add(point.Add(0f, 0f, heightMod));
            }
            else
            {
                throw new Exception("[GatherBuddy] Profile has no hotspots.");
            }

            // Randomize hotspots if enabled
            if (settings.RandomizeHotspots)
            {
                ShuffleList(_waypoints);
                Logging.Write("[GatherBuddy] Hotspots randomized");
            }

            // Build circular queue
            _waypointQueue = new CircularQueue<WoWPoint>();
            foreach (var wp in _waypoints)
                _waypointQueue.Enqueue(wp);

            // Start at nearest waypoint
            if (StyxWoW.Me != null)
            {
                var nearest = _waypoints.OrderBy(w => w.DistanceSqr(StyxWoW.Me.Location)).First();
                _waypointQueue.CycleTo(nearest);
            }

            // Bounce mode
            if (settings.PathingType == PathType.Bounce)
                _waypointQueue.OnEndOfQueue += OnQueueCycle;

            Logging.Write($"[GatherBuddy] Loaded {_waypoints.Count} waypoints");

            // Load blackspots from profile
            if (ProfileManager.CurrentProfile.Blackspots != null &&
                ProfileManager.CurrentProfile.Blackspots.Count > 0)
            {
                BlackspotManager.AddBlackspots(ProfileManager.CurrentProfile.Blackspots);
                Logging.Write($"[GatherBuddy] Loaded {ProfileManager.CurrentProfile.Blackspots.Count} blackspots from profile");
            }
            BlackspotManager.EnsureBlackspotsMarked();

            // Targeting filters
            Targeting.Instance.IncludeTargetsFilter += IncludeTargetsFilter;
            LootTargeting.Instance.IncludeTargetsFilter += IncludeLootFilter;

            // GameStats
            GameStats.Reset();
            GameStats.StartMeasuring();

            _cleanupTimer.Start();
            Logging.Write("[GatherBuddy] Started successfully!");
        }

        public override void Stop()
        {
            Logging.Write("[GatherBuddy] Stopping...");

            Targeting.Instance.IncludeTargetsFilter -= IncludeTargetsFilter;
            LootTargeting.Instance.IncludeTargetsFilter -= IncludeLootFilter;

            if (_waypointQueue != null)
                _waypointQueue.OnEndOfQueue -= OnQueueCycle;

            // Clear profile blackspots
            if (ProfileManager.CurrentProfile?.Blackspots != null &&
                ProfileManager.CurrentProfile.Blackspots.Count > 0)
            {
                BlackspotManager.RemoveBlackspots(ProfileManager.CurrentProfile.Blackspots);
            }

            NodeTracker.SaveBlacklist();
            GameStats.StopMeasuring();
            _sessionTimer.Stop();
            _bottingTimer.Stop();
            Logging.Write($"[GatherBuddy] Session: {_nodesGathered} nodes ({_herbsGathered} herbs, {_mineralsGathered} minerals) in {SessionTime:hh\\:mm\\:ss}");

            NodeTracker.Reset();
            _cleanupTimer.Stop();
        }

        public override void Pulse()
        {
            // PathPrecision scaling by speed (from LevelBot)
            float speed = StyxWoW.Me?.MovementInfo?.CurrentSpeed ?? 7f;
            Navigator.PathPrecision = Math.Clamp(speed * 0.15f, 1.5f, 10f);

            // Ensure blackspots are marked (tiles may load dynamically)
            BlackspotManager.EnsureBlackspotsMarked();

            // Periodic cleanup of expired node tracking
            if (_cleanupTimer.ElapsedMilliseconds > 30000)
            {
                NodeTracker.CleanupExpired();
                _cleanupTimer.Restart();
            }
        }

        private void OnQueueCycle(object? sender, EventArgs e)
        {
            _waypoints.Reverse();
            _waypointQueue = new CircularQueue<WoWPoint>();
            foreach (var wp in _waypoints)
                _waypointQueue.Enqueue(wp);
            _waypointQueue.OnEndOfQueue += OnQueueCycle;
        }

        // ═══════════════════════════════════════════════════════════
        // BEHAVIOR TREE
        // ═══════════════════════════════════════════════════════════

        private PrioritySelector CreateRootBehavior()
        {
            return new PrioritySelector(
                // 0. Session timer check
                CreateSessionTimerBehavior(),

                // 1. Death management (full: release, spirit healer, corpse walk, rez sickness)
                CreateDeathBehavior(),

                // 2. Combat (full: Rest, PreCombatBuff, Heal, CombatBuff, Pull, Combat)
                CreateCombatBehavior(),

                // 3. Loot killed mobs (with LootFrame, retry, skinning)
                new Decorator(
                    ctx => GatherBuddySettings.Instance.LootMobs,
                    CreateLootBehavior()
                ),

                // 4. Vendor/Repair/Mail
                new Decorator(
                    ctx => NeedsVendorOrRepairOrMail(),
                    CreateVendorMailBehavior()
                ),

                // 5. Node gathering
                CreateGatherBehavior(),

                // 6. Movement to next waypoint
                CreateMovementBehavior(),

                // 7. Idle
                new ActionIdle()
            );
        }

        // ═══════════════════════════════════════════════════════════
        // SESSION TIMER (BottingHours)
        // ═══════════════════════════════════════════════════════════

        private Composite CreateSessionTimerBehavior()
        {
            return new Decorator(
                ctx =>
                {
                    float hours = GatherBuddySettings.Instance.BottingHours;
                    return hours > 0f && _bottingTimer.Elapsed.TotalHours >= hours;
                },
                new Action(ctx =>
                {
                    Logging.Write("[GatherBuddy] BottingHours limit reached, stopping!");

                    if (GatherBuddySettings.Instance.HearthAndExit)
                    {
                        Logging.Write("[GatherBuddy] Using Hearthstone...");
                        Lua.DoString("UseItemByName(GetItemInfo(6948))");
                        Thread.Sleep(12000); // Wait for hearth cast
                    }

                    TreeRoot.Stop("GatherBuddy: BottingHours limit reached");
                    return RunStatus.Success;
                })
            );
        }

        // ═══════════════════════════════════════════════════════════
        // DEATH BEHAVIOR (Full — spirit healer, corpse camp, jiggle)
        // ═══════════════════════════════════════════════════════════

        private Composite CreateDeathBehavior()
        {
            return new PrioritySelector(
                // Dead - release spirit
                new Decorator(
                    ctx => StyxWoW.Me != null && StyxWoW.Me.IsDead,
                    new Sequence(
                        new Action(ctx =>
                        {
                            Logging.Write("[GatherBuddy] Died! Releasing...");
                            GameStats.Died();
                            TrackDeath();
                            Lua.DoString("RepopMe()");
                            SleepForLag();
                            return RunStatus.Success;
                        }),
                        new WaitContinue(5, ctx => StyxWoW.Me != null && StyxWoW.Me.IsGhost, new ActionAlwaysSucceed())
                    )
                ),

                // Spirit healer path (if setting enabled or 3+ deaths in 3 mins)
                new Decorator(
                    ctx => StyxWoW.Me != null && StyxWoW.Me.IsGhost &&
                           (_shouldUseSpiritHealer || GatherBuddySettings.Instance.UseSpiritHealer),
                    CreateSpiritHealerBehavior()
                ),

                // Can't navigate to corpse → use spirit healer
                new Decorator(
                    ctx => StyxWoW.Me != null && StyxWoW.Me.IsGhost &&
                           StyxWoW.Me.CorpsePoint != WoWPoint.Empty &&
                           StyxWoW.Me.Location.DistanceSqr(StyxWoW.Me.CorpsePoint) > 40 * 40 &&
                           !Navigator.CanNavigateFully(StyxWoW.Me.Location, StyxWoW.Me.CorpsePoint),
                    new Action(ctx =>
                    {
                        Logging.Write("[GatherBuddy] Can't navigate to corpse, using spirit healer");
                        _shouldUseSpiritHealer = true;
                        return RunStatus.Success;
                    })
                ),

                // Ghost - almost at corpse: jiggle + retrieve
                new Decorator(
                    ctx => StyxWoW.Me != null && StyxWoW.Me.IsGhost &&
                           StyxWoW.Me.CorpsePoint != WoWPoint.Empty &&
                           StyxWoW.Me.Location.DistanceSqr(StyxWoW.Me.CorpsePoint) < 40 * 40,
                    new Sequence(
                        // Jiggle 2-3y randomly before RetrieveCorpse (3.3.5a server bug fix)
                        new Action(ctx =>
                        {
                            float jx = (float)(_random.NextDouble() * 4 - 2);
                            float jy = (float)(_random.NextDouble() * 4 - 2);
                            var jigglePoint = StyxWoW.Me!.Location.Add(jx, jy, 0);
                            Navigator.MoveTo(jigglePoint);
                            Thread.Sleep(500);
                            WoWMovement.MoveStop();
                            return RunStatus.Success;
                        }),
                        new Action(ctx =>
                        {
                            Lua.DoString("RetrieveCorpse()");
                            SleepForLag();
                            return RunStatus.Success;
                        })
                    )
                ),

                // Ghost - move to corpse
                new Decorator(
                    ctx => StyxWoW.Me != null && StyxWoW.Me.IsGhost &&
                           StyxWoW.Me.CorpsePoint != WoWPoint.Empty,
                    new Action(ctx =>
                    {
                        TreeRoot.StatusText = "Moving to corpse";
                        Navigator.MoveTo(StyxWoW.Me!.CorpsePoint);
                        return RunStatus.Running;
                    })
                ),

                // Rez sickness: wait it out if enabled
                new Decorator(
                    ctx => StyxWoW.Me != null && !StyxWoW.Me.IsDead && !StyxWoW.Me.IsGhost &&
                           GatherBuddySettings.Instance.WaitRezSickness &&
                           HasRezSickness(),
                    new Action(ctx =>
                    {
                        TreeRoot.StatusText = "Waiting out Resurrection Sickness";
                        Logging.WriteDiagnostic("[GatherBuddy] Waiting for Resurrection Sickness to expire...");
                        return RunStatus.Running;
                    })
                )
            );
        }

        private Composite CreateSpiritHealerBehavior()
        {
            return new PrioritySelector(
                // Find and move to spirit healer
                new Action(ctx =>
                {
                    var spiritHealer = ObjectManager.GetObjectsOfType<WoWUnit>()
                        .Where(u => u.IsSpiritHealer)
                        .OrderBy(u => u.DistanceSqr)
                        .FirstOrDefault();

                    if (spiritHealer == null)
                    {
                        Logging.WriteDebug("[GatherBuddy] No spirit healer nearby");
                        return RunStatus.Failure;
                    }

                    if (spiritHealer.DistanceSqr > 10 * 10)
                    {
                        TreeRoot.StatusText = "Moving to Spirit Healer";
                        Navigator.MoveTo(spiritHealer.Location);
                        return RunStatus.Running;
                    }

                    // Interact — accept rez
                    WoWMovement.MoveStop();
                    spiritHealer.Interact();
                    Thread.Sleep(1000);
                    Lua.DoString("StaticPopup1Button1:Click()"); // Accept rez sickness
                    _shouldUseSpiritHealer = false;
                    _deathCount = 0;
                    Logging.Write("[GatherBuddy] Accepted Spirit Healer resurrection");
                    SleepForLag();
                    return RunStatus.Success;
                })
            );
        }

        /// <summary>
        /// Track deaths for corpse camp protection (3 deaths in 3 mins → spirit healer).
        /// </summary>
        private void TrackDeath()
        {
            if (_deathTimer.IsRunning && _deathTimer.Elapsed.TotalMinutes > 3)
            {
                _deathCount = 0;
                _deathTimer.Restart();
            }
            else if (!_deathTimer.IsRunning)
            {
                _deathTimer.Start();
            }

            _deathCount++;
            if (_deathCount >= 3)
            {
                Logging.Write("[GatherBuddy] 3 deaths in 3 minutes — switching to spirit healer");
                _shouldUseSpiritHealer = true;
            }
        }

        private static bool HasRezSickness()
        {
            var results = Lua.GetReturnValues("local name = UnitDebuff('player', 'Resurrection Sickness'); return name or ''");
            return results != null && results.Count > 0 && !string.IsNullOrEmpty(results[0]);
        }

        // ═══════════════════════════════════════════════════════════
        // COMBAT BEHAVIOR (Full — Rest, PreCombatBuff, Heal, CombatBuff, Pull, Combat)
        // ═══════════════════════════════════════════════════════════

        private Composite CreateCombatBehavior()
        {
            return new PrioritySelector(
                // Not in combat: Rest + PreCombatBuff
                new Decorator(
                    ctx => StyxWoW.Me != null && !StyxWoW.Me.Combat,
                    new PrioritySelector(
                        Routine?.RestBehavior ?? new ActionAlwaysFail(),
                        Routine?.PreCombatBuffBehavior ?? new ActionAlwaysFail()
                    )
                ),

                // In combat (or pet in combat): Dismount + Heal + CombatBuff + Combat
                new Decorator(
                    ctx => StyxWoW.Me != null &&
                           (StyxWoW.Me.Combat || (StyxWoW.Me.GotAlivePet && StyxWoW.Me.Pet.Combat)) &&
                           Targeting.Instance.FirstUnit != null,
                    new PrioritySelector(
                        // Dismount for combat
                        new Decorator(
                            ctx => StyxWoW.Me!.Mounted,
                            new Action(ctx =>
                            {
                                Mount.Dismount("Combat");
                                return RunStatus.Success;
                            })
                        ),
                        Routine?.HealBehavior ?? new ActionAlwaysFail(),
                        Routine?.CombatBuffBehavior ?? new ActionAlwaysFail(),
                        Routine?.CombatBehavior ?? new ActionAlwaysFail(),
                        new ActionAlwaysSucceed()
                    )
                )
            );
        }

        // ═══════════════════════════════════════════════════════════
        // LOOT BEHAVIOR (Full — LootFrame, LOOT_OPENED, retry, skinning)
        // ═══════════════════════════════════════════════════════════

        private Composite CreateLootBehavior()
        {
            return new Decorator(
                ctx => StyxWoW.Me != null && !StyxWoW.Me.Combat,
                new PrioritySelector(
                    // Loot a target mob
                    new Decorator(
                        ctx =>
                        {
                            var target = GetLootTarget();
                            return target != null;
                        },
                        new PrioritySelector(
                            // Move to lootable
                            new Decorator(
                                ctx =>
                                {
                                    var target = GetLootTarget();
                                    return target != null && target.DistanceSqr > 5 * 5;
                                },
                                new Action(ctx =>
                                {
                                    var target = GetLootTarget()!;
                                    TreeRoot.StatusText = $"Moving to loot {target.Name}";
                                    Navigator.MoveTo(target.Location);
                                    return RunStatus.Running;
                                })
                            ),
                            // Close enough — loot
                            new Action(ctx =>
                            {
                                var target = GetLootTarget();
                                if (target == null) return RunStatus.Failure;

                                // Track attempts for retry/blacklist
                                if (target.Guid != _lastLootGuid)
                                {
                                    _lastLootGuid = target.Guid;
                                    _lootAttemptCount = 0;
                                    _lootFailCount = 0;
                                }

                                _lootAttemptCount++;

                                // Too many failures → blacklist
                                if (_lootAttemptCount >= 5 || _lootFailCount >= 2)
                                {
                                    Logging.Write($"[GatherBuddy] Blacklisting loot target {target.Name} (too many attempts)");
                                    Blacklist.Add(target.Guid, TimeSpan.FromMinutes(5));
                                    _lastLootGuid = 0;
                                    return RunStatus.Success;
                                }

                                WoWMovement.MoveStop();
                                target.Interact();
                                SleepForLag();

                                // Wait for LOOT_OPENED
                                Thread.Sleep(500);

                                // Auto-loot all via Lua
                                Lua.DoString(
                                    "for i=GetNumLootItems(),1,-1 do " +
                                    "  LootSlot(i); ConfirmBindOnUse(); " +
                                    "end; " +
                                    "CloseLoot()");

                                GameStats.LootedMob();
                                return RunStatus.Success;
                            })
                        )
                    ),

                    // Skinning (if enabled)
                    new Decorator(
                        ctx => GatherBuddySettings.Instance.SkinMobs,
                        new Decorator(
                            ctx =>
                            {
                                var skinTarget = GetSkinTarget();
                                return skinTarget != null;
                            },
                            new PrioritySelector(
                                new Decorator(
                                    ctx => GetSkinTarget()!.DistanceSqr > 5 * 5,
                                    new Action(ctx =>
                                    {
                                        Navigator.MoveTo(GetSkinTarget()!.Location);
                                        return RunStatus.Running;
                                    })
                                ),
                                new Action(ctx =>
                                {
                                    var skinTarget = GetSkinTarget()!;
                                    WoWMovement.MoveStop();
                                    skinTarget.Interact();
                                    Logging.Write($"[GatherBuddy] Skinning {skinTarget.Name}");
                                    SleepForLag();
                                    Thread.Sleep(2000); // Wait for skinning cast
                                    return RunStatus.Success;
                                })
                            )
                        )
                    )
                )
            );
        }

        private WoWUnit? GetLootTarget()
        {
            float radius = GatherBuddySettings.Instance.LootRadius;
            float radiusSqr = radius * radius;
            return ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(u => u.IsDead && u.CanLoot && u.DistanceSqr < radiusSqr &&
                            !Blacklist.Contains(u.Guid))
                .OrderBy(u => u.DistanceSqr)
                .FirstOrDefault();
        }

        private WoWUnit? GetSkinTarget()
        {
            float radius = GatherBuddySettings.Instance.LootRadius;
            float radiusSqr = radius * radius;
            return ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(u => u.IsDead && u.CanSkin && !u.CanLoot && u.DistanceSqr < radiusSqr &&
                            u.KilledByMe && !Blacklist.Contains(u.Guid))
                .OrderBy(u => u.DistanceSqr)
                .FirstOrDefault();
        }

        // ═══════════════════════════════════════════════════════════
        // GATHER BEHAVIOR (core — with blackspot check)
        // ═══════════════════════════════════════════════════════════

        private Composite CreateGatherBehavior()
        {
            return new PrioritySelector(
                // ContextChanger: find best node each tick
                ctx =>
                {
                    _currentNode = FindBestNode();
                    return ctx;
                },

                // No node found - fall through to movement
                new Decorator(
                    ctx => _currentNode == null,
                    new ActionAlwaysFail()
                ),

                // Node found - move to it and harvest
                new Sequence(
                    new Action(ctx =>
                    {
                        TreeRoot.StatusText = $"Gathering {_currentNode!.Name} ({_currentNode.Distance:F0}y)";
                        Logging.WriteDiagnostic($"[GatherBuddy] Found {_currentNode.Name} at {_currentNode.Distance:F0}y");
                        return RunStatus.Success;
                    }),

                    new PrioritySelector(
                        // Too far - move closer
                        new Decorator(
                            ctx => _currentNode != null && _currentNode.DistanceSqr > 5 * 5,
                            new Sequence(
                                // Dismount if getting close (ground or descend if flying)
                                new DecoratorContinue(
                                    ctx => StyxWoW.Me != null && StyxWoW.Me.Mounted && _currentNode!.DistanceSqr < 15 * 15,
                                    new Action(ctx =>
                                    {
                                        Mount.Dismount("Gathering");
                                        return RunStatus.Success;
                                    })
                                ),
                                new Action(ctx =>
                                {
                                    if (GatherBuddySettings.Instance.UseFlying && Flightor.MountHelper.CanMount)
                                        Flightor.MoveTo(_currentNode!.Location);
                                    else
                                        Navigator.MoveTo(_currentNode!.Location);
                                    return RunStatus.Running;
                                })
                            )
                        ),

                        // Close enough - interact
                        new Decorator(
                            ctx => _currentNode != null && _currentNode.DistanceSqr <= 5 * 5 &&
                                   StyxWoW.Me != null && !StyxWoW.Me.IsCasting,
                            new Sequence(
                                new Action(ctx =>
                                {
                                    WoWMovement.MoveStop();

                                    if (StyxWoW.Me!.Mounted)
                                        Mount.Dismount("Gathering");

                                    if (GatherBuddySettings.Instance.FaceNodes)
                                        WoWMovement.Face(_currentNode!.Location);

                                    _currentNode!.Interact();
                                    Logging.Write($"[GatherBuddy] Gathering {_currentNode.Name}");
                                    return RunStatus.Success;
                                }),

                                // Wait for cast to finish
                                new WaitContinue(
                                    5,
                                    ctx => StyxWoW.Me != null && !StyxWoW.Me.IsCasting,
                                    new Action(ctx =>
                                    {
                                        if (_currentNode != null)
                                        {
                                            _nodesGathered++;
                                            if (_currentNode.IsHerb) _herbsGathered++;
                                            if (_currentNode.IsMineral) _mineralsGathered++;
                                            NodeTracker.MarkHarvested(_currentNode);

                                            // Auto-loot the gather (LOOT_OPENED)
                                            SleepForLag();
                                            Lua.DoString(
                                                "for i=GetNumLootItems(),1,-1 do " +
                                                "  LootSlot(i); ConfirmBindOnUse(); " +
                                                "end; " +
                                                "CloseLoot()");
                                        }
                                        _currentNode = null;
                                        return RunStatus.Success;
                                    })
                                )
                            )
                        )
                    )
                )
            );
        }

        // ═══════════════════════════════════════════════════════════
        // MOVEMENT BEHAVIOR (with mount logic)
        // ═══════════════════════════════════════════════════════════

        private Composite CreateMovementBehavior()
        {
            return new PrioritySelector(
                new Decorator(
                    ctx => _waypointQueue == null || _waypointQueue.Count == 0,
                    new Action(ctx =>
                    {
                        Logging.Write("[GatherBuddy] No waypoints! Load a profile with hotspots.");
                        return RunStatus.Failure;
                    })
                ),

                // Arrived at waypoint - advance
                new Decorator(
                    ctx => StyxWoW.Me != null &&
                           StyxWoW.Me.Location.DistanceSqr(_waypointQueue!.Peek()) < 15 * 15,
                    new Action(ctx =>
                    {
                        _waypointQueue!.Dequeue();
                        return RunStatus.Success;
                    })
                ),

                // Move to current waypoint
                new Action(ctx =>
                {
                    var targetWaypoint = _waypointQueue!.Peek();
                    TreeRoot.StatusText = $"Moving to waypoint ({_waypointQueue.Count} remaining)";

                    if (GatherBuddySettings.Instance.UseFlying && Flightor.MountHelper.CanMount)
                    {
                        float alt = GatherBuddySettings.Instance.FlyingAltitude;
                        var flyDest = new WoWPoint(targetWaypoint.X, targetWaypoint.Y, targetWaypoint.Z + alt);
                        Flightor.MoveTo(flyDest);
                        return RunStatus.Running;
                    }

                    // Ground mount if far
                    if (StyxWoW.Me != null && !StyxWoW.Me.Mounted &&
                        Mount.CanMount() &&
                        Mount.ShouldMount(targetWaypoint))
                    {
                        Mount.MountUp();
                        return RunStatus.Running;
                    }

                    Navigator.MoveTo(targetWaypoint);
                    return RunStatus.Running;
                })
            );
        }

        // ═══════════════════════════════════════════════════════════
        // VENDOR / MAIL BEHAVIOR (Full — Profile + Engine APIs)
        // ═══════════════════════════════════════════════════════════

        private bool NeedsVendorOrRepairOrMail()
        {
            if (StyxWoW.Me == null || StyxWoW.Me.Combat || StyxWoW.Me.IsDead || StyxWoW.Me.IsGhost)
                return false;

            return NeedsToSell() || NeedsToRepair() || NeedsToMail();
        }

        private bool NeedsToSell()
        {
            var settings = GatherBuddySettings.Instance;
            if (!settings.VendorWhenFull) return false;

            int freeSlots = GetFreeBagSlots();
            if (freeSlots > settings.MinFreeBagSlots) return false;

            // Check that a vendor exists (profile or nearby)
            var vendor = GetBestVendor(Vendor.VendorType.Sell);
            return vendor != null || HasNearbyVendorNpc();
        }

        private bool NeedsToRepair()
        {
            var settings = GatherBuddySettings.Instance;
            if (!settings.RepairAtVendor) return false;

            float durability = GetDurabilityPercent();
            if (durability <= 0f || durability >= settings.RepairDurabilityPercent) return false;

            var vendor = GetBestVendor(Vendor.VendorType.Repair);
            return vendor != null || HasNearbyRepairNpc();
        }

        private bool NeedsToMail()
        {
            var settings = GatherBuddySettings.Instance;
            if (!settings.MailToAlt || string.IsNullOrEmpty(settings.MailRecipient))
                return false;

            // Need mailbox from profile
            var mailbox = ProfileManager.CurrentProfile?.MailboxManager?.GetClosestMailbox();
            if (mailbox == null) return false;

            // Only mail if bags getting full
            int freeSlots = GetFreeBagSlots();
            return freeSlots <= settings.MinFreeBagSlots + 2;
        }

        /// <summary>
        /// Full vendor/mail behavior. Priority: Sell/Repair → Mail → Return to route.
        /// Uses profile VendorManager/MailboxManager when available, falls back to ObjectManager scan.
        /// </summary>
        private Composite CreateVendorMailBehavior()
        {
            return new PrioritySelector(
                // === SELL / REPAIR ===
                new Decorator(
                    ctx => NeedsToSell() || NeedsToRepair(),
                    new PrioritySelector(
                        // Try profile vendor first
                        new Decorator(
                            ctx => GetBestVendor(NeedsToRepair() ? Vendor.VendorType.Repair : Vendor.VendorType.Sell) != null,
                            CreateProfileVendorBehavior()
                        ),
                        // Fallback: nearby NPC vendor
                        CreateNearbyVendorBehavior()
                    )
                ),

                // === MAIL ===
                new Decorator(
                    ctx => NeedsToMail(),
                    CreateMailBehavior()
                )
            );
        }

        /// <summary>
        /// Navigate to profile vendor, handle Gossip, Sell, Repair.
        /// Uses Vendors.SellAllItems() with profile quality filters + ProtectedItems.
        /// </summary>
        private Composite CreateProfileVendorBehavior()
        {
            return new Action(ctx =>
            {
                var vendorType = NeedsToRepair() ? Vendor.VendorType.Repair : Vendor.VendorType.Sell;
                var vendor = GetBestVendor(vendorType);
                if (vendor == null) return RunStatus.Failure;

                // Move to vendor
                if (StyxWoW.Me!.Location.DistanceSqr(vendor.Location) > 5 * 5)
                {
                    TreeRoot.StatusText = $"Moving to vendor {vendor.Name}";
                    Navigator.MoveTo(vendor.Location);
                    return RunStatus.Running;
                }

                // Find the NPC unit
                var vendorUnit = ObjectManager.GetObjectsOfType<WoWUnit>()
                    .Where(u => u.Entry == vendor.Entry && u.IsAlive)
                    .OrderBy(u => u.DistanceSqr)
                    .FirstOrDefault();

                if (vendorUnit == null)
                {
                    Logging.Write($"[GatherBuddy] Vendor {vendor.Name} not found at location, blacklisting");
                    ProfileManager.CurrentProfile?.VendorManager?.Blacklist.Add(vendor);
                    return RunStatus.Failure;
                }

                WoWMovement.MoveStop();
                vendorUnit.Interact();
                SleepForLag();

                // Handle GossipFrame if visible
                HandleGossipFrame();

                // Sell using proper API (respects profile quality filters + ProtectedItems)
                if (NeedsToSell())
                {
                    SellItemsWithQualityFilter();
                    Logging.Write("[GatherBuddy] Sold items at vendor");
                }

                // Repair
                if (NeedsToRepair() && vendorUnit.IsRepairMerchant)
                {
                    MerchantFrame.Instance.RepairAllItems();
                    Logging.Write("[GatherBuddy] Repaired equipment");
                }

                SleepForLag();
                MerchantFrame.Instance.Close();
                return RunStatus.Success;
            });
        }

        /// <summary>
        /// Fallback: scan ObjectManager for nearby vendor NPCs.
        /// </summary>
        private Composite CreateNearbyVendorBehavior()
        {
            return new Action(ctx =>
            {
                var vendor = ObjectManager.GetObjectsOfType<WoWUnit>()
                    .Where(u => u.IsVendor && u.IsAlive && !u.IsHostile)
                    .OrderBy(u => u.DistanceSqr)
                    .FirstOrDefault();

                if (vendor == null)
                {
                    Logging.WriteDebug("[GatherBuddy] Bags full/low durability but no vendor nearby");
                    return RunStatus.Failure;
                }

                if (vendor.DistanceSqr > 5 * 5)
                {
                    TreeRoot.StatusText = $"Moving to vendor {vendor.Name}";
                    Navigator.MoveTo(vendor.Location);
                    return RunStatus.Running;
                }

                WoWMovement.MoveStop();
                vendor.Interact();
                SleepForLag();

                HandleGossipFrame();

                if (NeedsToSell())
                {
                    SellItemsWithQualityFilter();
                    Logging.Write("[GatherBuddy] Sold items at vendor");
                }

                if (NeedsToRepair() && vendor.IsRepairMerchant)
                {
                    MerchantFrame.Instance.RepairAllItems();
                    Logging.Write("[GatherBuddy] Repaired equipment");
                }

                SleepForLag();
                MerchantFrame.Instance.Close();
                return RunStatus.Success;
            });
        }

        /// <summary>
        /// Mail behavior: navigate to profile mailbox → send items.
        /// </summary>
        private Composite CreateMailBehavior()
        {
            return new Action(ctx =>
            {
                var settings = GatherBuddySettings.Instance;
                var mailbox = ProfileManager.CurrentProfile?.MailboxManager?.GetClosestMailbox();
                if (mailbox == null) return RunStatus.Failure;

                // Move to mailbox
                if (StyxWoW.Me!.Location.DistanceSqr(mailbox.Location) > 5 * 5)
                {
                    TreeRoot.StatusText = "Moving to mailbox";
                    Navigator.MoveTo(mailbox.Location);
                    return RunStatus.Running;
                }

                // Find mailbox game object
                var mailboxObj = ObjectManager.GetObjectsOfType<WoWGameObject>()
                    .Where(g => g.SubType == WoWGameObjectType.Mailbox)
                    .OrderBy(g => g.DistanceSqr)
                    .FirstOrDefault();

                if (mailboxObj == null)
                {
                    Logging.Write("[GatherBuddy] Mailbox not found at location");
                    return RunStatus.Failure;
                }

                WoWMovement.MoveStop();
                mailboxObj.Interact();
                SleepForLag();
                Thread.Sleep(1000); // Wait for mail frame

                // Collect items to mail based on quality settings
                var itemsToMail = GetItemsToMail();
                if (itemsToMail.Count > 0)
                {
                    MailFrame.Instance.SendMailWithManyAttachments(settings.MailRecipient, 0, itemsToMail.ToArray());
                    Logging.Write($"[GatherBuddy] Mailed {itemsToMail.Count} items to {settings.MailRecipient}");
                    SleepForLag();
                }

                MailFrame.Instance.Close();
                return RunStatus.Success;
            });
        }

        // ═══════════════════════════════════════════════════════════
        // HELPERS
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Finds the best node to harvest.
        /// Checks: distance, type, blacklist, anti-ninja, blackspot, AvoidMobs.
        /// </summary>
        private WoWGameObject? FindBestNode()
        {
            var settings = GatherBuddySettings.Instance;
            float maxRangeSqr = settings.NodeDetectionRange * settings.NodeDetectionRange;
            var blacklistedEntries = settings.BlacklistedEntries;
            var profile = ProfileManager.CurrentProfile;

            return ObjectManager.GetObjectsOfType<WoWGameObject>()
                .Where(obj =>
                {
                    // Distance check
                    if (obj.DistanceSqr > maxRangeSqr)
                        return false;

                    // Node type check
                    bool isValidType =
                        (settings.GatherHerbs && obj.IsHerb && obj.CanHarvest) ||
                        (settings.GatherMinerals && obj.IsMineral && obj.CanMine) ||
                        (settings.GatherChests && obj.IsChest && obj.CanLoot);

                    if (!isValidType)
                        return false;

                    // Node selection blacklist — unchecked entries in settings
                    if (blacklistedEntries.Count > 0 && blacklistedEntries.Contains(obj.Entry))
                        return false;

                    // Blacklist check (NodeTracker)
                    if (!NodeTracker.IsNodeValid(obj))
                        return false;

                    // Global blacklist check
                    if (Blacklist.Contains(obj.Guid))
                        return false;

                    // Blackspot check — skip nodes inside profile/global blackspots
                    if (BlackspotManager.IsBlackspotted(obj.Location))
                        return false;

                    // Anti-ninja: check if another player is near the node
                    if (settings.NoNinja)
                    {
                        bool playerNearby = ObjectManager.GetObjectsOfType<WoWPlayer>()
                            .Any(p => !p.IsMe && p.IsAlive &&
                                      p.Location.DistanceSqr(obj.Location) < 15 * 15);
                        if (playerNearby)
                            return false;
                    }

                    return true;
                })
                .OrderBy(obj => obj.DistanceSqr)
                .FirstOrDefault();
        }

        // ═══════════════════════════════════════════════════════════
        // TARGETING FILTERS (Faction, AvoidMobs, Blackspot-aware)
        // ═══════════════════════════════════════════════════════════

        private void IncludeTargetsFilter(List<WoWObject> incoming, HashSet<WoWObject> outgoing)
        {
            var settings = GatherBuddySettings.Instance;
            var profile = ProfileManager.CurrentProfile;

            for (int i = incoming.Count - 1; i >= 0; i--)
            {
                if (incoming[i] is not WoWUnit unit || incoming[i] is WoWPlayer)
                    continue;

                // Skip elites if configured
                if (settings.IgnoreElites && unit.Elite)
                    continue;

                // AvoidMobs from profile
                if (profile?.AvoidMobs != null && profile.AvoidMobs.Contains(unit.Entry))
                    continue;

                // Blackspot check
                if (BlackspotManager.IsBlackspotted(unit.Location))
                    continue;

                // Faction filtering from profile
                if (profile?.Factions != null && profile.Factions.Count > 0)
                {
                    if (!profile.Factions.Contains(unit.FactionId))
                        continue;
                }

                outgoing.Add(unit);
            }
        }

        private void IncludeLootFilter(List<WoWObject> incoming, HashSet<WoWObject> outgoing)
        {
            var settings = GatherBuddySettings.Instance;
            float lootRadius = settings.LootRadius;
            float lootRadiusSqr = lootRadius * lootRadius;

            for (int i = 0; i < incoming.Count; i++)
            {
                if (incoming[i] is WoWUnit unit)
                {
                    if (settings.LootMobs && unit.IsDead && unit.DistanceSqr < lootRadiusSqr &&
                        !Blacklist.Contains(unit.Guid))
                    {
                        if ((unit.KilledByMe && unit.CanLoot) ||
                            (unit.CanSkin && settings.SkinMobs && unit.KilledByMe))
                        {
                            outgoing.Add(unit);
                        }
                    }
                }
                else if (incoming[i] is WoWGameObject gameObj)
                {
                    if (gameObj.DistanceSqr < lootRadiusSqr && !Blacklist.Contains(gameObj.Guid))
                    {
                        bool isTarget =
                            (gameObj.IsHerb && settings.GatherHerbs) ||
                            (gameObj.IsMineral && settings.GatherMinerals) ||
                            (gameObj.IsChest && settings.GatherChests);

                        if (isTarget && gameObj.CanLoot)
                        {
                            // Blackspot check for nodes
                            if (BlackspotManager.IsBlackspotted(gameObj.Location))
                                Blacklist.Add(gameObj.Guid, TimeSpan.FromDays(3));
                            else
                                outgoing.Add(gameObj);
                        }
                    }
                }
            }
        }

        // ═══════════════════════════════════════════════════════════
        // VENDOR/MAIL HELPERS
        // ═══════════════════════════════════════════════════════════

        /// <summary>
        /// Get best vendor from profile VendorManager.
        /// </summary>
        private static Vendor? GetBestVendor(Vendor.VendorType type)
        {
            return ProfileManager.CurrentProfile?.VendorManager?.GetClosestVendor(type);
        }

        private static bool HasNearbyVendorNpc()
        {
            return ObjectManager.GetObjectsOfType<WoWUnit>()
                .Any(u => u.IsVendor && u.IsAlive && !u.IsHostile && u.DistanceSqr < 200 * 200);
        }

        private static bool HasNearbyRepairNpc()
        {
            return ObjectManager.GetObjectsOfType<WoWUnit>()
                .Any(u => u.IsRepairMerchant && u.IsAlive && !u.IsHostile && u.DistanceSqr < 200 * 200);
        }

        /// <summary>
        /// Handle GossipFrame (some vendors require gossip selection first).
        /// </summary>
        private static void HandleGossipFrame()
        {
            Thread.Sleep(500);
            var results = Lua.GetReturnValues("return GossipFrame and GossipFrame:IsVisible() and '1' or '0'");
            if (results != null && results.Count > 0 && results[0] == "1")
            {
                // Find the vendor gossip option (usually first one)
                Lua.DoString("SelectGossipOption(1)");
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Sell items respecting quality filters from GatherBuddy settings.
        /// Uses ProtectedItemsManager to protect specific items.
        /// </summary>
        private static void SellItemsWithQualityFilter()
        {
            var settings = GatherBuddySettings.Instance;

            // Build quality filter string for the Lua script
            var qualityList = new List<int>();
            if (settings.SellGrey) qualityList.Add(0);
            if (settings.SellWhite) qualityList.Add(1);
            if (settings.SellGreen) qualityList.Add(2);
            if (settings.SellBlue) qualityList.Add(3);
            if (settings.SellPurple) qualityList.Add(4);

            if (qualityList.Count == 0) return;

            // Build protected item IDs set
            var protectedIds = ProtectedItemsManager.GetAllItemIds();
            string protectedStr = protectedIds.Count > 0
                ? string.Join(",", protectedIds.Select(id => $"[{id}]=1"))
                : "";

            string qualFilter = string.Join(",", qualityList.Select(q => $"[{q}]=1"));

            Lua.DoString(
                $"local sell={{{qualFilter}}}; " +
                $"local prot={{{protectedStr}}}; " +
                "for bag=0,4 do " +
                "  for slot=1,GetContainerNumSlots(bag) do " +
                "    local link = GetContainerItemLink(bag,slot); " +
                "    if link then " +
                "      local name,_,quality,_,_,_,_,_,_,_,price = GetItemInfo(link); " +
                "      local id = tonumber(link:match('item:(%d+)')); " +
                "      if quality and sell[quality] and not prot[id] then " +
                "        UseContainerItem(bag,slot); " +
                "      end " +
                "    end " +
                "  end " +
                "end");
        }

        /// <summary>
        /// Get items to mail based on quality filters.
        /// Skips protected items and soulbound items.
        /// </summary>
        private static List<WoWItem> GetItemsToMail()
        {
            var settings = GatherBuddySettings.Instance;
            var items = new List<WoWItem>();

            foreach (var item in ObjectManager.GetObjectsOfType<WoWItem>())
            {
                if (item.IsSoulbound) continue;
                if (ProtectedItemsManager.Contains(item.Entry)) continue;

                bool shouldMail = item.Quality switch
                {
                    WoWItemQuality.Poor => settings.MailGrey,
                    WoWItemQuality.Common => settings.MailWhite,
                    WoWItemQuality.Uncommon => settings.MailGreen,
                    WoWItemQuality.Rare => settings.MailBlue,
                    WoWItemQuality.Epic => settings.MailPurple,
                    _ => false
                };

                if (shouldMail)
                    items.Add(item);
            }

            return items;
        }

        /// <summary>
        /// Returns the number of free bag slots via Lua.
        /// </summary>
        private static int GetFreeBagSlots()
        {
            var results = Lua.GetReturnValues("local free=0; for i=0,4 do free=free+GetContainerNumFreeSlots(i) end; return free");
            if (results != null && results.Count > 0 && int.TryParse(results[0], out int free))
                return free;
            return 999;
        }

        /// <summary>
        /// Returns average durability percentage (0-100) across equipped items.
        /// </summary>
        private static float GetDurabilityPercent()
        {
            var results = Lua.GetReturnValues(
                "local total,current=0,0; " +
                "for slot=1,18 do " +
                "  local cur,mx=GetInventoryItemDurability(slot); " +
                "  if cur and mx and mx>0 then total=total+mx; current=current+cur end " +
                "end; " +
                "if total==0 then return 100 end; " +
                "return math.floor(current/total*100)");
            if (results != null && results.Count > 0 && float.TryParse(results[0], out float pct))
                return pct;
            return 100f;
        }

        /// <summary>
        /// Sleep for estimated latency (100 + server latency) ms.
        /// </summary>
        private static void SleepForLag()
        {
            var results = Lua.GetReturnValues("local _, _, lag = GetNetStats(); return lag");
            int lag = 100;
            if (results != null && results.Count > 0 && int.TryParse(results[0], out int serverLag))
                lag = serverLag;
            Thread.Sleep(100 + lag);
        }

        /// <summary>
        /// Fisher-Yates shuffle for hotspot randomization.
        /// </summary>
        private static void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
