using System.Collections.Generic;
using System.Linq;
using Styx;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;

#if USE_DUNGEONBUDDY_DLL
using Bots.DungeonBuddyDll;
using Bots.DungeonBuddyDll.Attributes;
using Bots.DungeonBuddyDll.Helpers;
namespace Bots.DungeonBuddyDll.Dungeon_Scripts.Burning_Crusade
#else
    using Bots.DungeonBuddy.Attributes;
    using Bots.DungeonBuddy.Helpers;
namespace Bots.DungeonBuddy.Dungeon_Scripts.Burning_Crusade
#endif
{
    public class ShadowLabyrinth : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 151; } }

        public override WoWPoint Entrance { get { return new WoWPoint(-3649.739, 4943.619, -101.0474); } }

        public override void RemoveTargetsFilter(List<WoWObject> units)
        {
            units.RemoveAll(
                u =>
                    { // remove low level units.
                        var unit = u as WoWUnit;
                        if (unit != null)
                        {
                            if (unit is WoWPlayer)
                                return true;
                            if (unit.Entry == 18731 && unit.HasAura("Banish")) // Ambassador Hellmaw
                                return true;
                        }
                        return false;
                    });
        }

        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            foreach (var priority in units)
            {
                var unit = priority.Object as WoWUnit;
                if (unit != null)
                {
                    if (StyxWoW.Me.IsDps())
                    {
                        if (unit.Entry == 18796) // Fel Overseer
                            priority.Score += 210;

                        if (unit.Entry == 18634) // Cabal Summoner
                            priority.Score += 210;

                        if (unit.Entry == 19226) // Void Traveler
                            priority.Score += 400;
                    }
                }
            }
        }

        public override void OnEnter() { _blackHeartRoomCleared = false; }

        #endregion

        WoWUnit _cabalExecutioner;

        [EncounterHandler(18632, "Cabal Executioner")]
        public Composite CabalExecutionerHandler()
        {
            return new PrioritySelector(
                ctx => _cabalExecutioner = ctx as WoWUnit,
                ScriptHelpers.CreateRunAwayFromBad(
                    ctx => StyxWoW.Me.IsFollower() && ((StyxWoW.Me.IsMelee() && _cabalExecutioner.HasAura("Whirlwind")) || StyxWoW.Me.IsRange()), 10, u => u == _cabalExecutioner)
                );
        }

        [EncounterHandler(18796, "Fel Overseer")]
        public Composite FelOverseerHandler()
        {
            return new PrioritySelector(
                // stay away from overseer to prevent getting feared if ranged.
                ScriptHelpers.CreateRunAwayFromBad(ctx => StyxWoW.Me.IsRange(), 12, 18796),
                ScriptHelpers.CreateTankFaceAwayGroupUnit(10)
                );
        }

        /*
        [EncounterHandler(18797, "Tortured Skeleton", BossRange = 40, Mode = CallBehaviorMode.Proximity)]
        public Composite TorturedSkeletonEncounter()
        {
            WoWUnit skelly = null;
            return new PrioritySelector(
                ctx => skelly = ctx as WoWUnit,
                new Decorator(ctx => Targeting.Instance.FirstUnit != null,
                    new Action(ctx => Navigator.MoveTo (skelly.Location)))
                );
        }*/


        WoWUnit _ambassadorHellmaw;

        [EncounterHandler(18731, "Ambassador Hellmaw", 150, CallBehaviorMode.Proximity)]
        public Composite AmbassadorHellmawEncounter()
        {

            List<WoWUnit> trashGroup1 = null;
            List<WoWUnit> overSeers = null;

            var trashGroup1PullSpot = new WoWPoint(-152.6901, -121.3326, 6.650446);
            var trashGroup1TankSpot = new WoWPoint(-130.4145, -129.5648, 4.572331);
            var seerAtEntranceLoc = new WoWPoint(-156.8657, -93.51019, 8.07323);
            var trashGroup1Loc = new WoWPoint(-156.003, -74.48317, 8.073184);
            var overSeerCenterLoc = new WoWPoint(-158.9956, -39.65439, 8.073073);
            var overSeerTankLocation = new WoWPoint(-157.2833, -85.43105, 8.073076);
            return new PrioritySelector(
                ctx =>
                {
                    trashGroup1 = ScriptHelpers.GetUnfriendlyNpsAtLocation(() => trashGroup1Loc, 25, u => u.Entry == 18794);
                    overSeers = ScriptHelpers.GetUnfriendlyNpsAtLocation(() => overSeerCenterLoc, 100, u => u.Entry == 18796);
                    return _ambassadorHellmaw = ctx as WoWUnit;
                },
                new Decorator(
                    ctx => _ambassadorHellmaw.Combat,
                    ScriptHelpers.CreateTankFaceAwayGroupUnit(20)),
                new Decorator(
                    ctx => !_ambassadorHellmaw.Combat && StyxWoW.Me.Z > 3f,
                    new PrioritySelector(
                // pull seer if it's close enough.
                        ScriptHelpers.CreatePullNpcToLocation(
                            ctx => overSeers.Any() && overSeers[0].Location.DistanceSqr(seerAtEntranceLoc) <= 10 * 10, ctx => true, () => overSeers[0], () => trashGroup1TankSpot, () => trashGroup1PullSpot),
                // clear the 1st group of trash while avoiding overseers.
                        ScriptHelpers.CreatePullNpcToLocation(
                            ctx => trashGroup1.Any(), ctx => !overSeers.Any(u => u.Location.DistanceSqr(trashGroup1[0].Location) <= 40 * 40), () => trashGroup1[0], () => trashGroup1TankSpot,
                            () => trashGroup1PullSpot),
                // kill the overseers.
                        ScriptHelpers.CreatePullNpcToLocation(
                            ctx => overSeers.Any(), ctx => !ScriptHelpers.GetUnfriendlyNpsAtLocation(() => overSeers[0].Location, 30, u => u != overSeers[0]).Any(), () => overSeers[0],
                            () => overSeers.Count > 1 ? trashGroup1PullSpot : overSeerTankLocation, () => overSeers.Count > 1 ? trashGroup1PullSpot : overSeerTankLocation)
                        )
                    ));
        }

        private readonly CircularQueue<WoWPoint> _clearRoomPath = new CircularQueue<WoWPoint>
                                                                      {
                                                                          new WoWPoint(-233.7071, -76.19012, 8.073038),
                                                                          new WoWPoint(-243.252, 4.626983, 8.073118)
                                                                      };

        private bool _blackHeartRoomCleared;
        WoWUnit _blackheartTheInciter;

        [EncounterHandler(18667, "Blackheart the Inciter", BossRange = 120, Mode = CallBehaviorMode.Proximity)]
        public Composite BlackheartTheInciterEncounter()
        {
            var pillarsByBlackHeart = new List<KeyValuePair<WoWPoint, float>>
                                          {
                                              new KeyValuePair<WoWPoint, float>(new WoWPoint(-305.4713, -24.4433, 8.072822), 3),
                                              new KeyValuePair<WoWPoint, float>(new WoWPoint(-305.386, -54.1253, 8.072817), 3),
                                              new KeyValuePair<WoWPoint, float>(new WoWPoint(-284.017, -51.09766, 8.072872), 6),
                                              new KeyValuePair<WoWPoint, float>(new WoWPoint(-284.1259, -26.82462, 8.167638), 6),
                                          };

            var blackheartRoom = new WoWPoint(-261.3178, -38.62257, 8.072852);
            var nearestPillar = new KeyValuePair<WoWPoint, float>();

            return new Decorator(
                ctx => !ScriptHelpers.IsBossAlive("Ambassador Hellmaw"),
                new PrioritySelector(
                    ctx =>
                    {
                        nearestPillar = pillarsByBlackHeart.OrderBy(kv => StyxWoW.Me.Location.DistanceSqr(kv.Key)).FirstOrDefault();
                        return _blackheartTheInciter = ctx as WoWUnit;
                    },
                    new Decorator(
                        ctx => !_blackheartTheInciter.Combat,
                        new PrioritySelector(
                            new Decorator(
                                ctx => ScriptHelpers.GetUnfriendlyNpsAtLocation(() => blackheartRoom, 100, u => u != _blackheartTheInciter).Any(),
                                ScriptHelpers.CreateClearArea(() => blackheartRoom, 100, u => u != _blackheartTheInciter)),
                // make sure the room is completely cleared before we continue.
                            new Decorator(
                                ctx => !ScriptHelpers.GetUnfriendlyNpsAtLocation(() => blackheartRoom, 100, u => u != _blackheartTheInciter).Any() && !_blackHeartRoomCleared,
                                new PrioritySelector(
                                    ctx => _clearRoomPath.Peek(),
                                    new Decorator(
                                        ctx => StyxWoW.Me.Location.Distance2DSqr((WoWPoint)ctx) < 5 * 5,
                // we have reached the last point.
                                        new Action(
                                            ctx =>
                                            {
                                                _clearRoomPath.Dequeue();
                                                if (_clearRoomPath.Peek() == _clearRoomPath.First)
                                                    _blackHeartRoomCleared = true;
                                            })),
                                    new Action(ctx => ScriptHelpers.MoveTankTo((WoWPoint)ctx))
                                    )))),
                    new Decorator(
                        ctx => _blackheartTheInciter.Combat,
                        new PrioritySelector(
                            ScriptHelpers.CreateTankAgainstObject(ctx => _blackheartTheInciter.Combat && _blackheartTheInciter.CurrentTarget == StyxWoW.Me, () => nearestPillar.Key, () => nearestPillar.Value)
                            ))));
        }

        WoWUnit _grandmasterVorpil;
        WoWUnit _voidTraveler;

        [EncounterHandler(18732, "Grandmaster Vorpil", BossRange = 120, Mode = CallBehaviorMode.Proximity)]
        public Composite GrandmasterVorpilEncounter()
        {

            List<WoWUnit> patCirclingBoss = null;
            List<WoWUnit> patInfrontOfBoss = null;
            List<WoWUnit> skellies = null;
            var waitForPatLoc = new WoWPoint(-312.7558, -262.3074, 12.6841);
            var patInfrontOfBossLoc = new WoWPoint(-271.8994, -263.7166, 12.68043);
            var kiteCasterToLoc = new WoWPoint(-356.7253, -264.3843, 12.68629);

            return new PrioritySelector(
                ctx =>
                {
                    _voidTraveler = ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.Entry == 19226).OrderBy(u => u.DistanceSqr).FirstOrDefault();

                    _grandmasterVorpil = ctx as WoWUnit;
                    if (_grandmasterVorpil != null)
                    {
                        patCirclingBoss = ScriptHelpers.GetUnfriendlyNpsAtLocation(() => _grandmasterVorpil.Location, 35, u => u != _grandmasterVorpil && u.Entry != 18797);
                        patInfrontOfBoss = ScriptHelpers.GetUnfriendlyNpsAtLocation(() => patInfrontOfBossLoc, 25, u => u != _grandmasterVorpil && u.Entry != 18797);
                        // GetUnfriendlyNpcs disgards any npcs that can't be selected..
                        skellies =
                            ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.Entry == 18797 && u.IsAlive && u.Location.DistanceSqr(waitForPatLoc) < 25 * 25).OrderBy(u => u.DistanceSqr).ToList(

                                );
                    }
                    return _grandmasterVorpil;
                },
                // Trash Behavior
                new Decorator(
                    ctx => !_grandmasterVorpil.Combat && _grandmasterVorpil.DistanceSqr <= 80 * 80,
                    new PrioritySelector(
                // kite the casters away from boss to not agrro boss.
                        new Decorator(
                            ctx => patCirclingBoss.Any(u => u.Entry == 18638 && u.Combat),
                            new Action(ctx => Navigator.MoveTo(kiteCasterToLoc))),
                // kill Skillies
                        new Decorator(
                            ctx => StyxWoW.Me.IsTank() && !StyxWoW.Me.Combat && skellies.Any() && patInfrontOfBoss.Count <= 4,
                            new Action(ctx => Navigator.MoveTo(skellies[0].Location))),
                        ScriptHelpers.CreatePullNpcToLocation(
                            ctx => patCirclingBoss.Any(), ctx => patInfrontOfBoss.Any() && patInfrontOfBoss.Count <= 4, () => patInfrontOfBoss[0], () => waitForPatLoc, () => StyxWoW.Me.Location)
                        )),
                // Boss Behavior
                new Decorator(
                    ctx => _grandmasterVorpil.Combat,
                    ScriptHelpers.CreateRunAwayFromBad(ctx => _voidTraveler != null && StyxWoW.Me.IsTank(), () => _voidTraveler.Location, 45, 30, 19226))
                );
        }

        WoWUnit _murmur;

        [EncounterHandler(18708, "Murmur")]
        public Composite MurmurEncounter()
        {
            return new PrioritySelector(
                ctx => _murmur = ctx as WoWUnit,
                ScriptHelpers.CreateRunAwayFromBad(ctx => _murmur.CastingSpellId == 38796 || _murmur.CastingSpellId == 33923, () => _murmur.Location, 100, 40, 18708), // big bang
                ScriptHelpers.CreateSpreadOutLogic(ctx => StyxWoW.Me.HasAura("Murmur's Touch"), () => _murmur.Location, 15, 35)
                );
        }
    }
}