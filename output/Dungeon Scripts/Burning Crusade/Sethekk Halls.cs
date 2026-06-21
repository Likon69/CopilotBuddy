using System.Collections.Generic;
using System.Linq;
using Styx;
using Styx.Logic;
using Styx.Logic.Pathing;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;

#if USE_DUNGEONBUDDY_DLL
using Bots.DungeonBuddyDll;
using Bots.DungeonBuddyDll.Actions;
using Bots.DungeonBuddyDll.Attributes;
using Bots.DungeonBuddyDll.Helpers;
namespace Bots.DungeonBuddyDll.Dungeon_Scripts.Burning_Crusade
#else
    using Bots.DungeonBuddy.Actions;
    using Bots.DungeonBuddy.Attributes;
    using Bots.DungeonBuddy.Helpers;
    namespace Bots.DungeonBuddy.Dungeon_Scripts.Burning_Crusade
#endif

{
    public class SethekkHalls : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 150; } }

        public override WoWPoint Entrance { get { return new WoWPoint(-3361.518, 4655.588, -101.0466); } }

        public override void IncludeTargetsFilter(List<WoWObject> incomingunits, HashSet<WoWObject> outgoingunits)
        {
            foreach (var unit in incomingunits)
            {
                if (unit.Entry == 20343) //  Charming Totem
                    outgoingunits.Add(unit);
            }
        }

        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            foreach (var priority in units)
            {
                var unit = priority.Object as WoWUnit;
                if (unit != null)
                {
                    if (unit.Entry == 20343) //  Charming Totem
                        priority.Score += 400;

                    if (unit.Entry == 18327 && StyxWoW.Me.IsDps()) //  Time-Lost Controllers 
                        priority.Score += 210;

                    if (unit.Entry == 18325 && StyxWoW.Me.IsDps()) //  Sethekk Prophet 
                        priority.Score += 200;

                    if (unit.Entry == 18319 && StyxWoW.Me.IsDps()) //  Time-Lost Scryer
                        priority.Score += 190;
                }
            }
        }

        public override void RemoveTargetsFilter(List<WoWObject> units) { units.RemoveAll(unit => unit is WoWPlayer); }

        #endregion

        WoWUnit _syth;

        [EncounterHandler(18472, "Darkweaver Syth")]
        public Composite DarkweaverSythEncounter()
        {
            return new PrioritySelector(
                ctx => _syth = ctx as WoWUnit,
                ScriptHelpers.CreateSpreadOutLogic(ctx => true, () => _syth.Location, 15, 30)
                );
        }

        WoWUnit _ghost = null;

        // run from the ghosts.. 
        [EncounterHandler(18703, "Sethekk Spirit", Mode = CallBehaviorMode.Proximity, BossRange = 15)]
        public Composite LostScryerEncounter()
        {
            return new PrioritySelector(
                ctx => _ghost = ctx as WoWUnit,
                ScriptHelpers.CreateRunAwayFromBad(ctx => true, () => _ghost.Location, 30, 12, 18703)
                );
        }

        WoWUnit _ikiss;

        [EncounterHandler(18473, "Talon King Ikiss")]
        public Composite TalonKingIkissEncounter()
        {
            var pilarLocations = new List<WoWPoint>
                                     {
                                         new WoWPoint(22.67584, 309.4335, 26.59805),
                                         new WoWPoint(67.06754, 308.9184, 26.62627),
                                         new WoWPoint(67.86166, 262.8065, 26.36894),
                                         new WoWPoint(22.76415, 264.541, 26.66963)
                                     };

            return new PrioritySelector(
                ctx => _ikiss = ctx as WoWUnit,
                ScriptHelpers.CreateLosLocation(
                    ctx => _ikiss.CastingSpellId == 38197 || _ikiss.CastingSpellId == 40425, () => _ikiss.Location, () => pilarLocations.OrderBy(loc => StyxWoW.Me.Location.DistanceSqr(loc)).FirstOrDefault(), () => 10));
        }

        // we need to not try use this door before it actually opens... 
        [ObjectHandler(183398, "Doodad_Auchindoun_Door_Swinging01")]
        public Composite Doodad_Auchindoun_Door_Swinging01Handler()
        {
            return new Decorator(
                ctx => ScriptHelpers.IsBossAlive("Talon King Ikiss") && ((WoWGameObject) ctx).State == WoWGameObjectState.Ready,
                new Action(
                    ctx =>
                        {
                            var boss = BossManager.BossEncounters.FirstOrDefault(b => b.Name == "Talon King Ikiss");
                            if (boss != null)
                            {
                                var breadCrumbs = boss.PathBreadCrumbs;
                                breadCrumbs.CycleTo(breadCrumbs.First);
                            }
                            return RunStatus.Failure;
                        }));
        }
    }
}