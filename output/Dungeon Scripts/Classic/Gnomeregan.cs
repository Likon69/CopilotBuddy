using System.Linq;
using CommonBehaviors.Actions;
using Styx.Logic;
using Styx.Logic.POI;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Styx;
using System.Collections.Generic;
using Action = TreeSharp.Action;

#if USE_DUNGEONBUDDY_DLL
using Bots.DungeonBuddyDll;
using Bots.DungeonBuddyDll.Profiles;
using Bots.DungeonBuddyDll.Attributes;
using Bots.DungeonBuddyDll.Helpers;
namespace Bots.DungeonBuddyDll.Dungeon_Scripts.Classic
#else
    using Bots.DungeonBuddy.Profiles;
    using Bots.DungeonBuddy.Attributes;
    using Bots.DungeonBuddy.Helpers;
    namespace Bots.DungeonBuddy.Dungeon_Scripts.Classic
#endif
{
    public class Gnomeregan : Dungeon
    {
        #region Overrides of Dungeon

        /// <summary> The mapid of this dungeon. </summary>
        /// <value>The map identifier.</value>
        public override uint DungeonId
        {
            get { return 14; }
        }
        public override WoWPoint Entrance { get { return new WoWPoint(-5147.494, 899.52, 287.40); } }
        public override WoWPoint ExitLocation { get { return new WoWPoint(-325.03, -5.06, -152.84); } }

        public override void IncludeTargetsFilter(List<WoWObject> incomingunits, HashSet<WoWObject> outgoingunits)
        {
            foreach (var unit in incomingunits)
            {
                if (unit.Entry == 7915)
                    outgoingunits.Add(unit);

                if (unit.Entry == 7849 && StyxWoW.Me.IsTank())
                    outgoingunits.Add(unit);
            }
        }
        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            foreach (var t in units)
            {
                var prioObject = t.Object;

                //Bombs on the last boss vital to living
                if (prioObject.Entry == 7915)
                {
                    t.Score += 1000;
                }
                //mobile alert systems, they spawn adds that wreck
                if (prioObject.Entry == 7849)
                {
                    t.Score += 1000;
                }
            }
        }

        #endregion

        public WoWItem Item { get { return StyxWoW.Me.CarriedItems.FirstOrDefault(i => i.Entry == 60680); } }
        public WoWGameObject ParachutesBox { get { return (ObjectManager.GetObjectsOfType<WoWGameObject>()
                                     .Where(u => u.Entry == 208325 && u.Location.Distance(StyxWoW.Me.Location) < 40)
                                     .OrderBy(u => u.Distance).FirstOrDefault()); } }

        [EncounterHandler(0, "Root")]
        public Composite Root()
        {
            return
                new PrioritySelector(

                    new Decorator(ctx => ParachutesBox != null && Item == null && !StyxWoW.Me.IsActuallyInCombat,
                        new PrioritySelector(
                            new Decorator(ctx => !ParachutesBox.WithinInteractRange,
                                    new Action(ctx => Navigator.MoveTo(ParachutesBox.Location))),

                            new Decorator(ctx => ParachutesBox.WithinInteractRange,
                                new Sequence(
                                    new Action(ctx => WoWMovement.MoveStop()),
                                    new Action(ctx => ParachutesBox.Interact()))))),

                    new Decorator(ctx => StyxWoW.Me.IsFalling && Item != null && StyxWoW.Me.Location.Z < -230.953 && StyxWoW.Me.Location.Z > -260.953,
                        new Action(ctx => Item.Use())),

                    new Decorator(ctx => StyxWoW.Me.IsFalling && Item != null && StyxWoW.Me.Location.Z < -170.953 && StyxWoW.Me.Location.Z > -215.953,
                        new Action(ctx => Item.Use())),

                    
                    //ScriptHelpers.CreateTankTalkToThenEscortNpc(7998, 0, new WoWPoint(-514.94, -138.54, -152.48), 7361), new WoWPoint(-514.94, -138.54, -152.48), 7361), 5, )

                    new Decorator(ctx => ScriptHelpers.ShouldKillBoss("Grubbis") && ScriptHelpers.IsBossAlive("Grubbis"),
                        ScriptHelpers.CreateTankTalkToThenEscortNpc(7998, 0, new WoWPoint(-514.94, -138.54, -152.48), new WoWPoint(-519.99, -124.85, -156.11), 6, () => !ScriptHelpers.IsBossAlive("Grubbis"))));
        }

        [EncounterHandler(7079, "ViscousFallout")]
        public Composite ViscousFalloutFight()
        {
            return
                new PrioritySelector();
        }

        [EncounterHandler(6235, "Electrocutioner6000")]
        public Composite Electrocutioner6000Fight()
        {
            return
                new PrioritySelector();
        }

        [EncounterHandler(6229, "CrowdPummeler9-60")]
        public Composite CrowdPummeler960Fight()
        {
            return ScriptHelpers.CreateTankFaceAwayGroupUnit(20);
        }


        WoWUnit alert
        {
            get
            {
                return
                    ObjectManager.GetObjectsOfType<WoWUnit>().Where(x => x.Entry == 7849 && x.IsAlive && x.Distance < 20).OrderBy(x => x.Distance).
                        FirstOrDefault();
            }
        }

        WoWUnit bomb

        {
            get
            {
                return
                    ObjectManager.GetObjectsOfType<WoWUnit>().Where(x => x.Entry == 7915 && x.IsAlive).OrderBy(x => x.Distance).
                        FirstOrDefault();
            }
        }

        [EncounterHandler(7800, "MekgineerThermaplugg")]
        public Composite MekgineerThermapluggFight()
        {
            return
                new PrioritySelector();
                //new Decorator(r => !StyxWoW.Me.IsTank() && !StyxWoW.Me.IsHealer() && bomb != null, new ActionSetPoi(true, ret => new BotPoi(bomb, PoiType.Kill)));
        }
    }
}
