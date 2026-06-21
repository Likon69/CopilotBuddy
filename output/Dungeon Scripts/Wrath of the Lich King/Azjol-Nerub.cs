using System.Collections.Generic;
using System.Linq;
using Styx;
using Styx.Logic;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;

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
    public class AzjolNerub : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 204; } }

        public override WoWPoint Entrance { get { return new WoWPoint(3671.12, 2171.487, 35.94287); } }

        public override void RemoveTargetsFilter(List<WoWObject> units)
        {
            units.RemoveAll(
                ret =>
                    {
                        var unit = ret as WoWUnit;
                        if (unit != null)
                        {
                            if ( (unit.IsTargetingMyPartyMember || unit.IsTargetingMeOrPet) && unit.DistanceSqr > 30*30)
                                return true;
                            if (unit.Entry == 29209 && StyxWoW.Me.IsTank())
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
                    if (unit.Entry == 28619) // Web Wrap
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
                    if (unit.Entry == 28619 && StyxWoW.Me.IsDps())
                        priority.Score += 500; // kill the web wrap...

                    if (unit.Entry == 28921 && StyxWoW.Me.IsDps() && unit.TaggedByMe) // burn down Hadronox
                        priority.Score += 500;
                }
            }
        }

        #endregion

        private WoWUnit _silithik;

        [EncounterHandler(28684, "Krik'thir the Gatewatcher", Mode = CallBehaviorMode.Proximity, BossRange = 65)]
        public Composite KrikthirTheGatewatcherEncounter()
        {
            WoWUnit gashra = null, narjil = null;
            var tankLoc = new WoWPoint(541.2601, 701.8342, 776.805);
            return new PrioritySelector(
                ctx =>
                {
                    _silithik = ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.Entry == 28731 && u.IsAlive);
                    gashra = ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.Entry == 28730 && u.IsAlive);
                    narjil = ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.Entry == 28729 && u.IsAlive);
                    return ctx as WoWUnit;
                },
                ScriptHelpers.CreatePullNpcToLocation(ctx => _silithik != null, () => _silithik, () => tankLoc, 10),
                ScriptHelpers.CreatePullNpcToLocation(ctx => gashra != null, () => gashra, () => tankLoc, 10),
                ScriptHelpers.CreatePullNpcToLocation(ctx => narjil != null, () => narjil, () => tankLoc, 10)
                );
        }

        private WoWUnit _hadronox;

        [EncounterHandler(28921, "Hadronox", Mode = CallBehaviorMode.Proximity, BossRange = 150)]
        public Composite HadronoxEncounter()
        {
            WoWUnit trash = null;

            var trashTankSpot = new WoWPoint(507.6383, 515.5826, 748.325);
            var trashLoc = new WoWPoint(529.6913, 547.1257, 731.8326);

            return new PrioritySelector(
                ctx =>
                {
                    trash = ScriptHelpers.GetUnfriendlyNpsAtLocation(() => trashLoc, 17, u => u.Entry == 29117 || u.Entry == 28922 || u.Entry == 29118).FirstOrDefault();
                    return _hadronox = ctx as WoWUnit;
                },
                ScriptHelpers.CreateRunAwayFromBad(ctx => true, 10, 53400),
                new Decorator(
                    ctx => !_hadronox.TaggedByMe,
                    new PrioritySelector(
                        ScriptHelpers.CreatePullNpcToLocation(
                            ctx => trash != _hadronox, ctx => Targeting.Instance.FirstUnit == null || Targeting.Instance.FirstUnit.DistanceSqr > 20 * 20 && trash != null, () => trash, () => trashTankSpot,
                            () => trashTankSpot,
                            10),
                        new Decorator(
                            ctx => StyxWoW.Me.Location.DistanceSqr(trashTankSpot) > 25 * 25,
                            new Action(ctx => ScriptHelpers.ControlledMoveTo(trashTankSpot)))
                        )),
                ScriptHelpers.CreatePullNpcToLocation(ctx => _hadronox != null, ctx => _hadronox.IsTargetingMeOrPet || _hadronox.IsTargetingMyPartyMember, () => _hadronox, () => StyxWoW.Me.Location, () => trashTankSpot, 10)
                );
        }

        WoWUnit _anubarak;

        [EncounterHandler(29120, "Anub'arak", Mode = CallBehaviorMode.Proximity)]
        public Composite AnubarakEncounter()
        {
            var startLoc = new WoWPoint(550.2374, 276.3922, 223.8891);

            return new PrioritySelector(
                ctx => _anubarak = ctx as WoWUnit,
                new Decorator(
                    ctx => StyxWoW.Me.Y > 276,
                    new Action(ctx => Navigator.MoveTo(startLoc))),
                ScriptHelpers.CreateRunAwayFromBad(ctx => true, 10, 29184), // impale target
                new Decorator(
                    ctx => StyxWoW.Me.CurrentTarget == _anubarak,
                    ScriptHelpers.CreateTankFaceAwayGroupUnit(15)),
                ScriptHelpers.CreateRunAwayFromBad(ctx => StyxWoW.Me.IsRange(), 15, 29120)
                );
        }
    }
}