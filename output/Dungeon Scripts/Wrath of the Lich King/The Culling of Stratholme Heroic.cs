using System;
using System.Collections.Generic;
using System.Linq;
using CommonBehaviors.Actions;
using CommonBehaviors.Decorators;
using Styx;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.POI;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Action = TreeSharp.Action;

#if USE_DUNGEONBUDDY_DLL
using Bots.DungeonBuddyDll;
using Bots.DungeonBuddyDll.Profiles;
using Bots.DungeonBuddyDll.Attributes;
using Bots.DungeonBuddyDll.Helpers;
namespace Bots.DungeonBuddyDll.Dungeon_Scripts.Wrath_of_the_Lich_King
#else
    using Bots.DungeonBuddy.Profiles;
    using Bots.DungeonBuddy.Attributes;
    using Bots.DungeonBuddy.Helpers;
namespace Bots.DungeonBuddy.Dungeon_Scripts.Wrath_of_the_Lich_King
#endif
{
    public class TheCullingOfStratholmeHeroic : Dungeon
    {
        #region Overrides of Dungeon
        public override uint DungeonId
        {
            get { return 210; }
        }
        public override WoWPoint Entrance { get { return new WoWPoint(-8756.695, -4461.556, -200.9562); } }

        public override void OnEnter()
        {
            _cratesDone = _gauntletStarted = _arthusSummoned = _gauntlet2Done = false;
            _suspiciousCrates.CycleTo(_suspiciousCrates.First);
        }

        public override void RemoveTargetsFilter(List<WoWObject> units)
        {
            units.RemoveAll(
                ret =>
                {
                    var unit = ret as WoWUnit;
                    if (unit != null)
                    {
                        if (unit.Entry == 27737 && !unit.Combat) // Risen Zombie
                            return true;
                    }
                    return false;
                });
        }

        public override void IncludeTargetsFilter(List<WoWObject> incomingunits, HashSet<WoWObject> outgoingunits)
        {
            foreach (var obj in incomingunits)
            {
                var unit = obj as WoWUnit;
                if (unit != null)
                {
                    if (unit.CurrentTarget != null && unit.CurrentTarget.Entry == 26499 && StyxWoW.Me.IsTank()) // Targeting Arthas
                    {
                        outgoingunits.Add(unit);
                    }
                    if (unit.Entry == 27737 && unit.Combat && StyxWoW.Me.IsDps()) // Risen Zombie.. kill these buggers
                        outgoingunits.Add(unit);
                }
            }
        }

        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            foreach (var priority in units)
            {
                var unit = priority.Object as WoWUnit;
                if (unit != null)
                {
                    if (unit.CurrentTarget != null && unit.CurrentTarget.Entry == 26499 && StyxWoW.Me.IsTank()) // Targeting Arthas
                        priority.Score += 150;
                }
            }
        }

        #endregion

        private WoWUnit Arthas { get { return ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.Entry == 26499); } }

        private readonly Landmarks _landmarks = new Landmarks();

        private const float PIx2 = (float)Math.PI * 2;

        private WoWPoint GetNextPackLocationFromLandMark()
        {
            _landmarks.Refresh();
            var lm = _landmarks.LandmarkList.FirstOrDefault();
            var landmarkLoc = lm != null ? lm.Location : WoWPoint.Zero;

            if (landmarkLoc != WoWPoint.Zero && !Navigator.CanNavigateFully(StyxWoW.Me.Location, landmarkLoc))
            {
                for (int range = 5; range <= 20; range += 5)
                {
                    for (float step = 0f; step < 1f; step += 0.05f)
                    {
                        var newPoint = landmarkLoc.RayCast(PIx2 * step, range);
                        var heights = Navigator.FindHeights(newPoint.X, newPoint.Z);
                        if (heights == null) continue;
                        newPoint.Z = heights.Max();
                        if (Navigator.CanNavigateFully(StyxWoW.Me.Location, newPoint))
                            return newPoint;
                    }
                }
            }
            return landmarkLoc;
        }

        // chomie at dungeon entrance
        [EncounterHandler(26527, "Chromie", Mode = CallBehaviorMode.Proximity)]
        public Composite ChromieEncounter()
        {
            WoWUnit boss = null;
            return new PrioritySelector(
                ctx => boss = ctx as WoWUnit,
                new Decorator(
                    ctx => boss.QuestGiverStatus == QuestGiverStatus.Available,
                    ScriptHelpers.CreatePickupQuest(26527)),

                // take teleport or pickup Arcane Disruptor .
                new Decorator(
                    ctx => !ScriptHelpers.IsBossAlive("Chrono-Lord Epoch"),
                    ScriptHelpers.CreateTalkToNpc(26527)),

                new Decorator(
                    ctx => !_cratesDone && !StyxWoW.Me.BagItems.Any(b => b.IsValid && b.Entry == 37888) && boss.QuestGiverStatus == QuestGiverStatus.None,
                    new Sequence(
                            ScriptHelpers.CreateTalkToNpcContinue(26527),
                            new WaitContinue(2, ctx => false, new ActionAlwaysSucceed()),
                            ScriptHelpers.CreateTalkToNpcContinue(26527),
                            new WaitContinue(2, ctx => false, new ActionAlwaysSucceed()),
                            ScriptHelpers.CreateTalkToNpcContinue(26527),
                            new WaitContinue(2, ctx => false, new ActionAlwaysSucceed()),
                            ScriptHelpers.CreateTalkToNpcContinue(26527),
                            new WaitContinue(4, ctx => false, new ActionAlwaysSucceed()),
                            new DecoratorContinue(ctx => !StyxWoW.Me.BagItems.Any(b => b.IsValid && b.Entry == 37888),
                                new Action(ctx => _cratesDone = true))
                        ))
                );
        }

        readonly CircularQueue<WoWPoint> _suspiciousCrates = new CircularQueue<WoWPoint>
                                                       {
                                                           new WoWPoint(1579.717, 622.6676, 99.81494),
                                                           new WoWPoint(1571.189, 669.42, 103.0486),
                                                           new WoWPoint(1629.116, 730.6912, 113.3107),
                                                           new WoWPoint(1629.287, 812.0453, 121.4282),
                                                           new WoWPoint(1667.026, 871.1566, 119.8063),
                                                       };
        Composite CratesEncounter()
        {
            var chromieLoc = new WoWPoint(1550.081, 574.4117, 92.60664);
            WoWGameObject crate = null;
            WoWItem disruptor = null;
            WoWPoint movetoLoc = WoWPoint.Zero;

            return new PrioritySelector(ctx =>
            {
                disruptor = StyxWoW.Me.BagItems.FirstOrDefault(b => b.Entry == 37888);
                movetoLoc = _suspiciousCrates.Peek();
                return crate = ObjectManager.GetObjectsOfType<WoWGameObject>().Where(o => o.Entry == 190094).OrderBy(o => o.DistanceSqr).FirstOrDefault();
            },
                // talk to chromie to get the disruptor
                new Decorator(ctx => !StyxWoW.Me.BagItems.Any(b => b.IsValid && b.Entry == 37888),
                    new PrioritySelector(
                        new Decorator(ctx => StyxWoW.Me.Location.DistanceSqr(chromieLoc) > 10 * 10,
                            new Action(ctx => Navigator.MoveTo(chromieLoc))))),

                new Decorator(ctx => crate != null && crate.DistanceSqr <= 200 * 200,
                    new Sequence(
                        new ActionSetActivity("Using Arcane Disruptor on Suspicious Grain Crate"),
                        ScriptHelpers.CreateMoveToContinue(() => crate),
                        new Action(ctx => disruptor.UseContainerItem()))),

                new Decorator(ctx => StyxWoW.Me.Location.DistanceSqr(movetoLoc) < 15 * 15,
                    new Sequence(
                        new Action(ctx => _suspiciousCrates.Dequeue()),
                        new DecoratorContinue(ctx => _suspiciousCrates.Peek() == _suspiciousCrates.First,
                            new Action(ctx => _cratesDone = true)))),

                ScriptHelpers.CreateMountBehavior(),
                new Action(ctx => Navigator.MoveTo(movetoLoc)));
        }


        private bool _arthusSummoned;
        private bool _gauntletStarted;
        private bool _cratesDone;

        Composite GauntletEncounter()
        {
            WoWUnit arthas = null, chromieAtOutsideTown = null;

            var questTurninLoc = new WoWPoint(1813.298, 1283.578, 142.2429);
            var chromieStartEventLoc = new WoWPoint(1813.298, 1283.578, 142.2428);
            var arthasStartEventLoc = new WoWPoint(2047.948, 1287.598, 142.8352);
            var packLoc = WoWPoint.Zero;

            return new PrioritySelector(
                ctx =>
                {
                    arthas = Arthas;
                    chromieAtOutsideTown = ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.Entry == 27915);
                    packLoc = GetNextPackLocationFromLandMark();
                    if (!_arthusSummoned)
                    {

                        if (arthas != null || packLoc != WoWPoint.Zero)
                            _arthusSummoned = true;
                    }

                    if (packLoc != WoWPoint.Zero && !_gauntletStarted)
                        _gauntletStarted = true;
                    return ctx;
                },

                new Decorator(ctx => !_cratesDone,
                    CratesEncounter()),

                // move to pack location
                new Decorator(
                    ctx => packLoc != WoWPoint.Zero && StyxWoW.Me.IsTank() && Targeting.Instance.FirstUnit == null && StyxWoW.Me.Location.DistanceSqr(packLoc) > 10 * 10,
                    new Action(ctx => ScriptHelpers.MoveTankTo(packLoc))),
                new Decorator(
                    ctx => chromieAtOutsideTown != null && chromieAtOutsideTown.QuestGiverStatus == QuestGiverStatus.Available,
                    ScriptHelpers.CreatePickupQuest(27915)),
                // turn in Dispelling Illusions and pickup the followup.
                new Decorator(
                    ctx => StyxWoW.Me.QuestLog.ContainsQuest(13149) && StyxWoW.Me.QuestLog.GetQuestById(13149).IsCompleted,
                    new Sequence(
                        ScriptHelpers.CreateMoveToContinue(() => questTurninLoc),
                        ScriptHelpers.CreateTurninQuest(27915, 1))),
                // summon arthas.
                new Decorator(
                    ctx => _cratesDone && !_arthusSummoned,
                    new Sequence(
                        ScriptHelpers.CreateMoveToContinue(() => chromieStartEventLoc),
                        ScriptHelpers.CreateTalkToNpcContinue(27915),
                        new WaitContinue(2, ctx => false, new ActionAlwaysSucceed()),
                        ScriptHelpers.CreateTalkToNpcContinue(27915),
                        new WaitContinue(2, ctx => false, new ActionAlwaysSucceed()),
                        ScriptHelpers.CreateTalkToNpcContinue(27915),
                        new WaitContinue(2, ctx => false, new ActionAlwaysSucceed()),
                        ScriptHelpers.CreateTalkToNpcContinue(27915),
                        new Action(ctx => _arthusSummoned = true))),

                new Decorator(
                    ctx => _arthusSummoned && !_gauntletStarted && StyxWoW.Me.Location.DistanceSqr(arthasStartEventLoc) > 25 * 25,
                    new Action(ctx => ScriptHelpers.MoveTankTo(arthasStartEventLoc))),
                // talk to arthus to start the event.
                new Decorator(
                    ctx => arthas != null && arthas.Location.DistanceSqr(arthasStartEventLoc) <= 3 * 3 && StyxWoW.Me.PartyMembers.Count(u => u.DistanceSqr <= 40 * 40) == 4,
                    new Sequence(
                        ScriptHelpers.CreateTalkToNpc(26499),
                        new Action(ctx => _gauntletStarted = true))),
                // check if a hidden door is open meaning that we allready killed this boss and should move on.
                new Decorator(
                    ctx => _arthusSummoned && !_gauntletStarted && StyxWoW.Me.Location.DistanceSqr(arthasStartEventLoc) <= 25 * 25,
                    new PrioritySelector(
                        ctx => ObjectManager.GetObjectsOfType<WoWGameObject>().FirstOrDefault(o => o.Entry == 188686),
                        new Decorator(
                            ctx => ctx != null && ((WoWGameObject)ctx).State == WoWGameObjectState.Active && BossManager.CurrentBoss != null && BossManager.CurrentBoss.Entry == 26532,
                            new Action(ctx => BossManager.CurrentBoss.MarkAsDead())),
                        new ActionAlwaysSucceed())),

                // Wait for packs to spawn.
                new Decorator(
                    ctx => (packLoc == WoWPoint.Zero || StyxWoW.Me.Location.DistanceSqr(packLoc) <= 10 * 10) && _gauntletStarted && StyxWoW.Me.IsTank() && Targeting.Instance.FirstUnit == null,
                    new ActionAlwaysSucceed())
                    );
        }


        [EncounterHandler(26529, "Meathook")]
        public Composite MeathookEncounter()
        {
            WoWUnit boss = null;
            return new PrioritySelector(
                ctx => boss = ctx as WoWUnit
                );
        }


        [EncounterHandler(26530, "Salramm the Fleshcrafter")]
        public Composite SalrammTheFleshcrafterEncounter()
        {
            WoWUnit boss = null;
            return new PrioritySelector(
                ctx => boss = ctx as WoWUnit
                );
        }

        [EncounterHandler(26532, "Chrono-Lord Epoch", Mode = CallBehaviorMode.CurrentBoss)]
        public Composite ChronoLordEpochEncounter()
        {
            var arthasStartLoc = new WoWPoint(2366.24, 1195.253, 131.9611);
            var arthasEndLoc = new WoWPoint(2425.898, 1118.842, 148.0759);

            WoWUnit boss = null;
            return new PrioritySelector(
                ctx => boss = ctx as WoWUnit,
                new Decorator(
                    ctx => ScriptHelpers.IsBossAlive("Meathook") || ScriptHelpers.IsBossAlive("Salramm the Fleshcrafter"),
                    GauntletEncounter()),
                new Decorator(
                    ctx => !ScriptHelpers.IsBossAlive("Meathook") && !ScriptHelpers.IsBossAlive("Salramm the Fleshcrafter"),
                    ScriptHelpers.CreateTankTalkToThenEscortNpc(26499, arthasStartLoc, arthasEndLoc))
                );
        }

        [EncounterHandler(32273, "Infinite Corruptor")]
        public Composite InfiniteCorruptorEncounter()
        {
            WoWUnit boss = null;
            return new PrioritySelector(
                ctx => boss = ctx as WoWUnit
                );
        }

        private bool _gauntlet2Done;

        [EncounterHandler(26533, "Mal'Ganis", Mode = CallBehaviorMode.CurrentBoss)]
        public Composite MalGanisEncounter()
        {
            var arthasStartLoc = new WoWPoint(2425.898, 1118.842, 148.0759);
            var arthasGauntletStartLoc = new WoWPoint(2534.988, 1126.163, 130.7619);
            var arthasGauntletEndLoc = new WoWPoint(2363.44, 1404.906, 128.7849);

            WoWUnit boss = null, arthas = null, chromie = null;
            WoWGameObject chest = null;

            return new PrioritySelector(
                ctx =>
                {
                    chromie = ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.Entry == 26527);
                    chest = ObjectManager.GetObjectsOfType<WoWGameObject>().FirstOrDefault(o => o.Entry == 190663 && o.CanUse());
                    arthas = Arthas;
                    if (!_gauntlet2Done && arthas != null)
                        _gauntlet2Done = Arthas.Location.DistanceSqr(arthasGauntletEndLoc) <= 3 * 3;
                    return boss = ctx as WoWUnit;
                },
                // face boss away from group because of the Carrion Swarm
                new Decorator(
                    ctx => boss != null && StyxWoW.Me.IsTank() && boss.Combat && boss.CurrentTarget == StyxWoW.Me,
                    ScriptHelpers.CreateTankFaceAwayGroupUnit(20)),
                new Decorator(// talk to arthas so he'll open the secret door and run down to the gauntlet start
                    ctx => arthas != null && arthas.Location.DistanceSqr(arthasStartLoc) <= 3 * 3 && arthas.CanGossip,
                    ScriptHelpers.CreateTalkToNpc(26499)),
                new Decorator(
                    ctx => arthas != null && arthas.Location.DistanceSqr(arthasGauntletEndLoc) <= 3 * 3 && arthas.CanGossip,
                    ScriptHelpers.CreateTalkToNpc(26499)),
                new Decorator(
                    ctx => !_gauntlet2Done && arthas != null && arthas.Location.DistanceSqr(arthasGauntletStartLoc) <= 3 * 3,
                    ScriptHelpers.CreateTankTalkToThenEscortNpc(26499, arthasGauntletStartLoc, arthasGauntletEndLoc)),
                new Decorator(
                    ctx => chromie != null && StyxWoW.Me.QuestLog.ContainsQuest(13151) && StyxWoW.Me.QuestLog.GetQuestById(13151).IsCompleted,
                    ScriptHelpers.CreateTurninQuest(26527, 1)),
                new Decorator(
                    ctx => chest != null,
                    ScriptHelpers.CreateInteractWithObject(190663))
                );
        }
    }
}