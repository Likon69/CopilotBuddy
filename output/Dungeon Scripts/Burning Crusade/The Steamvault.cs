using System.Collections.Generic;
using System.Linq;
using Styx;
using Styx.Logic;
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
    public class TheSteamvault : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 147; } }

        public override void IncludeTargetsFilter(List<WoWObject> incomingunits, HashSet<WoWObject> outgoingunits)
        {
            foreach (var obj in incomingunits)
            {
                var unit = obj as WoWUnit;
                if (unit != null)
                {
                    if (unit.Entry == 17954 && unit.CanSelect)
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
                    if (unit.Entry == 17954) // Naga Distiller
                        priority.Score += 5000;

                    if (StyxWoW.Me.IsDps())
                    {
                        if (unit.Entry == 17917) // Coilfang Water Elemental
                            priority.Score += 500;
                        ;
                    }
                }
            }
        }

        #endregion

        /*
     	<Boss isFinal="false" entry="17797" name="Hydromancer Thespia" killOrder="1" optional="false" X="88.39713" Y="-316.1105" Z="-7.870739" />
	    <Boss isFinal="false" entry="17796" name="Mekgineer Steamrigger" killOrder="2" optional="false" X="-331.3518" Y="-122.1729" Z="-8.083851" />
	    <Boss isFinal="true" entry="17798" name="Warlord Kalithresh" killOrder="3" optional="false" X="-95.41883" Y="-552.0314" Z="8.187087" />
     */

        WoWUnit _hydromancerThespia;

        [EncounterHandler(17797, "Hydromancer Thespia")]
        public Composite HydromancerThespiaEncounter()
        {
            return new PrioritySelector(
                ctx => _hydromancerThespia = ctx as WoWUnit,
                ScriptHelpers.CreateRunAwayFromBad(ctx => true, 16, 25033),
                ScriptHelpers.CreateSpreadOutLogic(ctx => true, () => _hydromancerThespia.Location, 10, 35)
                );
        }

        WoWGameObject _accessPanel;

        [ObjectHandler(184125, "Main Chambers Access Panel")]
        public Composite MainChambersAccessPanel1Handler()
        {
            return new PrioritySelector(
                ctx => _accessPanel = ctx as WoWGameObject,
                new Decorator(
                    ctx => !ScriptHelpers.IsBossAlive("Hydromancer Thespia") && _accessPanel.State == WoWGameObjectState.Ready,
                    ScriptHelpers.CreateInteractWithObject(184125, 4))
                );
        }

        [ObjectHandler(184126, "Main Chambers Access Panel")]
        public Composite MainChambersAccessPanel2Handler()
        {
            return new PrioritySelector(
                ctx => _accessPanel = ctx as WoWGameObject,
                new Decorator(
                    ctx => !ScriptHelpers.IsBossAlive("Mekgineer Steamrigger") && _accessPanel.State == WoWGameObjectState.Ready,
                    ScriptHelpers.CreateInteractWithObject(184126, 4))
                );
        }


        [EncounterHandler(17796, "Mekgineer Steamrigger")]
        public Composite MekgineerSteamriggerEncounter()
        {
            return new PrioritySelector(
                );
        }

        WoWUnit _warlordKalithresh;

        [EncounterHandler(17798, "Warlord Kalithresh")]
        public Composite WarlordKalithreshEncounter()
        {
            WoWUnit distiller = null;
            return new PrioritySelector(
                ctx =>
                {
                    distiller =
                        ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => o.Entry == 17954 && o.IsAlive).OrderBy(o => o.DistanceSqr).
                            FirstOrDefault();
                    return _warlordKalithresh = ctx as WoWUnit;
                },
                ScriptHelpers.CreateTankAgainstObject(ctx => distiller != null && _warlordKalithresh.CurrentTarget == StyxWoW.Me, () => distiller.Location, () => 5)
                );
        }
    }
}