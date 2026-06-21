using System.Collections.Generic;
using System.Linq;
using CommonBehaviors.Actions;
using Styx;
using Styx.Logic;
using Styx.Logic.Pathing;
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
    public class TheMechanar : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 172; } }

        public override void OnEnter()
        {
            _lootedCache = false;
        }

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
                        if (unit.Entry == 20481) // Raging Flames
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
                        if (unit.Entry == 21062) // Nether Wraith
                            priority.Score += 500;

                        if (StyxWoW.Me.IsRange() && unit.Entry == 19716) //  Mechanar Tinkerer
                            priority.Score += 500;
                    }
                }
            }
        }

        #endregion

        [ObjectHandler(184228, "Instance_Portal_Difficulty_1", ObjectRange = 100)]
        public Composite EntranceTrashHandler()
        {
            var patroller1Loc = new WoWPoint(30.21965, 2.915361, -0.0006945329);
            var trash1Loc = new WoWPoint(22.4355, -20.95673, 5.289912);
            var trash2Loc = new WoWPoint(22.14023, 20.44575, -0.0001794659);

            var tankLoc1 = new WoWPoint(-14.75135, 0.7233489, -1.8124);
            var tankLoc2 = new WoWPoint(-27.63513, 0.1582904, -1.812397);
            WoWUnit patroller1 = null, trash1 = null, trash2 = null;

            return new PrioritySelector(
                ctx =>
                {
                    patroller1 = ScriptHelpers.GetUnfriendlyNpsAtLocation(() => patroller1Loc, 2, u => u.Entry == 19166).FirstOrDefault();
                    trash1 = ScriptHelpers.GetUnfriendlyNpsAtLocation(() => trash1Loc, 10, u => u.Entry != 19166).FirstOrDefault();
                    trash2 = ScriptHelpers.GetUnfriendlyNpsAtLocation(() => trash2Loc, 10, u => u.Entry != 19166).FirstOrDefault();
                    return ctx;
                },
                // pull the 1st patroller when he's alone.
                ScriptHelpers.CreatePullNpcToLocation(
                    ctx => patroller1 != null,
                    ctx => ScriptHelpers.GetUnfriendlyNpsAtLocation(() => patroller1Loc, 25, u => u.Entry == 19166).Count() == 1, () => patroller1,
                    () => tankLoc2, () => tankLoc1, 10),
                // pull trash group1 if no pats are close.
                ScriptHelpers.CreatePullNpcToLocation(
                    ctx => trash1 != null && !ScriptHelpers.GetUnfriendlyNpsAtLocation(() => trash1Loc, 20, u => u.Entry == 19166).Any(),
                    ctx => !ScriptHelpers.GetUnfriendlyNpsAtLocation(() => trash1Loc, 20, u => u.Entry == 19166).Any(),
                    () => trash1, () => tankLoc1, 4),
                // pull trash group2 if no pats are close.
                ScriptHelpers.CreatePullNpcToLocation(
                    ctx => trash2 != null, ctx => !ScriptHelpers.GetUnfriendlyNpsAtLocation(() => trash2Loc, 20, u => u.Entry == 19166).Any(),
                    () => trash2, () => tankLoc1, 4)
                );
        }

        WoWUnit _mechanoLordCapacitus;
        WoWPlayer _chargedPlayer = null;

        [EncounterHandler(19219, "Mechano-Lord Capacitus")]
        public Composite MechanoLordCapacitusEncounter()
        {

            return new PrioritySelector(
                ctx => _mechanoLordCapacitus = ctx as WoWUnit,
                    new PrioritySelector(
                        ctx =>
                        {
                            _chargedPlayer =
                                StyxWoW.Me.PartyMembers.Where(
                                    p =>
                                    (StyxWoW.Me.HasAura("Positive Charge") && p.HasAura("Negative Charge")) ||
                                    StyxWoW.Me.HasAura("Negative Charge") && p.HasAura("Positive Charge") && p.DistanceSqr <= 10 * 10).OrderBy(
                                        p => p.DistanceSqr).FirstOrDefault();
                            return ctx;
                        },
                        ScriptHelpers.CreateRunAwayFromBad(
                            ctx => StyxWoW.Me.IsFollower() && _chargedPlayer != null, () => _mechanoLordCapacitus.Location, 35, 12, p => p == _chargedPlayer)


                        /*  just heal through this.. maybe not in hereoic... idk
                        // melee dps should stop attacking
                        new Decorator(ctx => boss.HasAura("Reflective Damage Shield") && StyxWoW.Me.IsDps() && StyxWoW.Me.IsMelee(),
                            new PrioritySelector(
                                new Decorator(ctx => StyxWoW.Me.IsAutoAttacking,
                                    new Action(ctx => Lua.DoString("StopAttack()"))),
                                new ActionAlwaysSucceed())),

                        // melee dps should stop attacking
                        new Decorator(ctx => boss.HasAura("Reflective Magic Shield") && StyxWoW.Me.IsDps() && StyxWoW.Me.IsRange(),
                            new PrioritySelector(
                                new Decorator(ctx => StyxWoW.Me.IsCasting,
                                    new Action(ctx => SpellManager.StopCasting())),
                                new ActionAlwaysSucceed())), */
                        )
                );
        }

        [EncounterHandler(20405, " Nether Charge", BossRange = 20, Mode = CallBehaviorMode.Proximity)]
        public Composite NetherChargeHandler()
        {
            return new PrioritySelector(
                ScriptHelpers.CreateRunAwayFromBad(ctx => StyxWoW.Me.IsFollower(), 14, 20405));
        }

        [EncounterHandler(19710, "Gatewatcher Iron-Hand")]
        public Composite GatewatcherIronHandEncounter()
        {
            return new PrioritySelector(
                ScriptHelpers.CreateTankFaceAwayGroupUnit(15)
                );
        }

        [EncounterHandler(19218, "Gatewatcher Gyro-Kill")]
        public Composite GatewatcherGyroKillEncounter()
        {
            return new PrioritySelector(
                ScriptHelpers.CreateTankFaceAwayGroupUnit(15)
                );
        }

        private WoWUnit _nethermancerSepethreaCapacitus;

        [EncounterHandler(19221, "Nethermancer Sepethrea", Mode = CallBehaviorMode.Proximity, BossRange = 60)]
        public Composite NethermancerSepethreaCapacitusEncounter()
        {
            WoWUnit destroyer1 = null, destroyer2 = null, trash1 = null, trash2 = null;
            var destroyer1Loc = new WoWPoint(291.155, 34.63794, 25.38616);
            var destroyer2Loc = new WoWPoint(296.6584, -17.13713, 25.3822);
            var trash1Loc = new WoWPoint(309.3312, 15.13393, 25.38623);
            var trash2Loc = new WoWPoint(272.1549, -24.6583, 26.3284);

            var tankloc1 = new WoWPoint(276.8195, 49.4281, 25.38621);
            var tankloc2 = new WoWPoint(275.7698, 26.60911, 25.38618);

            return new PrioritySelector(
                ctx => _nethermancerSepethreaCapacitus = ctx as WoWUnit,
                new Decorator(
                    ctx => !_nethermancerSepethreaCapacitus.Combat,
                    new PrioritySelector(
                        ctx =>
                        {
                            destroyer1 = ScriptHelpers.GetUnfriendlyNpsAtLocation(() => destroyer1Loc, 20, u => u.Entry == 19735).FirstOrDefault();
                            destroyer2 = ScriptHelpers.GetUnfriendlyNpsAtLocation(() => destroyer2Loc, 20, u => u.Entry == 19735).FirstOrDefault();
                            trash1 = ScriptHelpers.GetUnfriendlyNpsAtLocation(() => trash1Loc, 8, u => true).FirstOrDefault();
                            trash2 = ScriptHelpers.GetUnfriendlyNpsAtLocation(() => trash2Loc, 8, u => true).FirstOrDefault();
                            return ctx;
                        },
                        ScriptHelpers.CreatePullNpcToLocation(ctx => destroyer1 != null, () => destroyer1, () => tankloc1, 10),
                        ScriptHelpers.CreatePullNpcToLocation(ctx => trash1 != null, () => trash1, () => tankloc1, 3),
                        ScriptHelpers.CreatePullNpcToLocation(ctx => destroyer2 != null, () => destroyer2, () => tankloc2, 10),
                        ScriptHelpers.CreatePullNpcToLocation(ctx => trash2 != null, () => trash2, () => tankloc2, 3)
                        )),
                new Decorator(
                    ctx => _nethermancerSepethreaCapacitus.Combat,
                    new PrioritySelector(
                        ScriptHelpers.CreateRunAwayFromBad(ctx => true, () => _nethermancerSepethreaCapacitus.Location, 200, 12, 20481), // fire elements
                        ScriptHelpers.CreateRunAwayFromBad(ctx => true, () => _nethermancerSepethreaCapacitus.Location, 200, 12, 35278)))
                );
        }

        [EncounterHandler(19220, "Pathaleon the Calculator")]
        public Composite PathaleonTheCalculatorEncounter()
        {
            return new PrioritySelector(
                ScriptHelpers.CreateRunAwayFromBad(ctx => StyxWoW.Me.IsRange(), 25, 19220)
                );
        }

        WoWUnit _bloodwarderSlayer;

        [EncounterHandler(19167, "Bloodwarder Slayer")]
        public Composite BloodwarderSlayerEncounter()
        {
            return new PrioritySelector(
                ctx => _bloodwarderSlayer = ctx as WoWUnit,
                ScriptHelpers.CreateRunAwayFromBad(
                    ctx => StyxWoW.Me.IsFollower() && _bloodwarderSlayer.HasAura("Whirlwind"), 10, u => u == _bloodwarderSlayer)
                );
        }

        private bool _lootedCache;
        WoWGameObject _cache;

        [ObjectHandler(184465, "Cache of the Legion", ObjectRange = 20)]
        public Composite CacheoftheLegionHandler()
        {
            return new PrioritySelector(
                ctx => _cache = ctx as WoWGameObject,
                new Decorator(
                    ctx => Targeting.Instance.FirstUnit == null && _cache.CanUse() && !_lootedCache,
                    new Sequence(
                        ScriptHelpers.CreateInteractWithObjectContinue(184465, 10),
                        new TreeSharp.Action(ctx => _lootedCache = true)))
                );
        }

        /*
         Transport Type: Elevator
         Tile: TempestKeepFactory
         GameObject Id: 183788
         Location when resting at the bottom: <0,0,0>
         Location when resting at the top: <8.742277E-13, -1E-05, 25.43577>
         Player wait/exit spot at bottom: <248.6114, 52.36416, 0.2290135>
         Player wait/exit spot at top: <265.7578, 52.01247, 25.64683>
         Player board spot at bottom. <257.019, 52.3995, 0.2461777>
         Player board spot at top. <257.019, 52.39949, 25.67912>
         */
    }
}