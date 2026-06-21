using System.Collections.Generic;
using Styx;
using Styx.Logic;
using Styx.Logic.Pathing;
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
    public class UtgardeKeepHeroic : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 242; } }
        public override WoWPoint Entrance { get { return new WoWPoint(1235.027, -4860.007, 41.24839); } }

        private readonly WoWPoint _ignoreDragonflayerPackLoc = new WoWPoint(331.5634, 0.776747, 22.7549);

        public override void RemoveTargetsFilter(List<WoWObject> units)
        {
            units.RemoveAll(
                ret =>
                    {
                        // fix for HB trying to run through a flaming wall of fire to get to this pack
                        if ((ret.Entry == 24078 || ret.Entry == 24079 || ret.Entry == 24080) && !ret.ToUnit().Combat && ret.Location.DistanceSqr(_ignoreDragonflayerPackLoc) <= 20*20)
                            return true;
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
                    if (unit.Entry == 23965) // Frost Tomb
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
                    if (unit.Entry == 23965 && StyxWoW.Me.IsDps()) // Frost Tomb
                        priority.Score += 500;

                    if ((unit.Entry == 27389 || unit.Entry == 27390) && StyxWoW.Me.IsDps()) // Skarvald the Constructor and Dalronn the Controller in ghost forms.
                        priority.Score -= 500;
                }
            }
        }

        #endregion

        [ObjectHandler(186611, "Glowing Anvil", ObjectRange = 120)]
        public Composite GlowingAnvilHandler()
        {
            var movetoLoc1 = new WoWPoint(306.7014, -36.75847, 24.67742);
            var movetoLoc2 = new WoWPoint(344.295, -56.70161, 22.7549);
            var movetoLoc3 = new WoWPoint(397.0732, -41.2756, 22.75489);

            return new PrioritySelector(
                new Decorator(
                    ctx =>
                    StyxWoW.Me.Location.DistanceSqr(movetoLoc1) < 30*30 && Targeting.Instance.FirstUnit == null && (StyxWoW.Me.IsTank() || ScriptHelpers.Tank == null || ScriptHelpers.Tank.DistanceSqr > 50*50),
                    new Action(ctx => Navigator.MoveTo(movetoLoc3))),
                new Decorator(
                    ctx =>
                    StyxWoW.Me.Location.DistanceSqr(movetoLoc2) < 30*30 && Targeting.Instance.FirstUnit == null && (StyxWoW.Me.IsTank() || ScriptHelpers.Tank == null || ScriptHelpers.Tank.DistanceSqr > 50*50),
                    new Action(ctx => Navigator.MoveTo(movetoLoc3))));
        }

        [EncounterHandler(23953, "Prince Keleseth")]
        public Composite PrinceKelesethEncounter()
        {
            WoWUnit boss = null;
            return new PrioritySelector(
                ctx => boss = ctx as WoWUnit
                );
        }

        [EncounterHandler(24201, "Dalronn the Controller")]
        public Composite DalronnTheControllerEncounter()
        {
            WoWUnit boss = null;
            return new PrioritySelector(
                ctx => boss = ctx as WoWUnit
                );
        }

        [EncounterHandler(24200, "Skarvald the Constructor")]
        public Composite SkarvaldTheConstructorEncounter()
        {
            WoWUnit boss = null;
            return new PrioritySelector(
                ctx => boss = ctx as WoWUnit
                );
        }


        [EncounterHandler(23954, "Ingvar the Plunderer")]
        public Composite IngvarThePlundererEncounter()
        {
            return new PrioritySelector(
                ScriptHelpers.CreateRunAwayFromBad(ctx => true, 14, 23996), // Ingvar Throw Target - the spinning axe.
                ScriptHelpers.CreateTankFaceAwayGroupUnit(15)
                );
        }
    }
}