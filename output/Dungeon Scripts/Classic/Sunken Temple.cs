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
namespace Bots.DungeonBuddyDll.Dungeon_Scripts.Classic
#else
    using Bots.DungeonBuddy.Profiles;
    using Bots.DungeonBuddy.Attributes;
    using Bots.DungeonBuddy.Helpers;
namespace Bots.DungeonBuddy.Dungeon_Scripts.Classic
#endif
{
    public class SunkenTemple : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 28; } }

        public override WoWPoint Entrance { get { return new WoWPoint(-10292.47, -3990.68, -70.85); } }
        public override WoWPoint ExitLocation { get { return new WoWPoint(-318.16, 94.90, -91.31); } }

        public override void RemoveTargetsFilter(List<WoWObject> units)
        {
            // don't kill MCed party members.
            units.RemoveAll(
                o =>
                    {
                        if (o is WoWPlayer)
                            return true;

                        if (o.Entry == 8317) // ghost
                            return true;

                        return false;
                    });
        }

        public override void IncludeTargetsFilter(List<WoWObject> incomingunits, HashSet<WoWObject> outgoingunits)
        {
            foreach (var unit in incomingunits.Select(obj => obj.ToUnit()))
            {
                switch (unit.Entry)
                {
                    case 6066: // Earthgrab Totem
                        outgoingunits.Add(unit);
                        break;
                }
            }
        }

        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            foreach (var p in units)
            {
                WoWUnit unit = p.Object.ToUnit();
                switch (unit.Entry)
                {
                    case 6066: // Earthgrab Totem
                        p.Score += 100;
                        break;
                }
            }
        }

        #endregion

        [EncounterHandler(5710, "Jammal'an the Prophet", BossRange = 100, Mode = CallBehaviorMode.Proximity)]
        public Composite OgomTheWretchedEncounter()
        {
            // clear room of argo.
            List<WoWUnit> targets = null;
            return new PrioritySelector(
                ctx => targets = ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => u.IsValid && u.IsAlive && u.Entry == 5271).OrderBy(u => u.DistanceSqr).ToList(),
                new Decorator(
                    ctx => targets != null && targets.Count > 0 && Targeting.Instance.FirstUnit == null && Navigator.CanNavigateFully(StyxWoW.Me.Location, targets[0].Location),
                    new Action(ctx => ScriptHelpers.MoveTankTo(targets[0].Location)))
                );
        }

        [EncounterHandler(8317, "Atal'ai Deathwalker's Spirit")]
        public Composite RunFromGhostEncounter()
        {
            return new PrioritySelector(
                ScriptHelpers.CreateRunAwayFromBad(ctx => true,6f, 8317)
                );
        }

        [EncounterHandler(5722, "Hazzas")]
        [EncounterHandler(5709, "Shade of Eranikus")]
        public Composite HazzasEncounter()
        {
            return new PrioritySelector(
                ScriptHelpers.CreateTankFaceAwayGroupUnit(10)
                );
        }
    }
}