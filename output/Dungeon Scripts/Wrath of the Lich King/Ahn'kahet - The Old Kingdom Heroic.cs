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
    public class AhnkahetTheOldKingdomHeroic : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 219; } }

        public override WoWPoint Entrance { get { return new WoWPoint(3640.21, 2028.928, 2.59325); } }

        public override void RemoveTargetsFilter(List<WoWObject> units)
        {
            units.RemoveAll(
                ret => { return false; });
        }

        public override void IncludeTargetsFilter(List<WoWObject> incomingunits, HashSet<WoWObject> outgoingunits)
        {
            foreach (var obj in incomingunits)
            {
                var unit = obj as WoWUnit;
                if (unit != null)
                {
                    if (unit.Entry == 30385 && unit.CanSelect && unit.Attackable) // Twilight Volunteer
                    {
                        var boss = ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.Entry == 29310);
                        if (boss != null && (!boss.Attackable || !boss.CanSelect))
                            outgoingunits.Add(unit);
                    }
                    if (StyxWoW.Me.HasAura("Insanity") && unit.CanSelect && unit.Attackable)
                    {
                        outgoingunits.Add(unit);
                    }
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
                    if (unit.Entry == 30176) // Ahn'kahar Guardian
                        priority.Score += 500;

                    if (unit.Entry == 30278 && StyxWoW.Me.IsRange()) // Spell Flinger
                        priority.Score += 500;
                }
            }
        }

        #endregion

        [EncounterHandler(0, "Root")]
        public Composite RootEncounter()
        {
            // handle a tough trash pul.
            var trashToNadoxTankLoc = new WoWPoint(598.4812, -1022.298, 38.35839);
            var trashToNadoxWaitLoc = new WoWPoint(615.3074, -1021.679, 32.71127);
            var trashToNadoxLoc = new WoWPoint(638.6355, -1004.077, 22.94104);
            WoWUnit trash = null;

            return new PrioritySelector(
                new Decorator(
                    ctx => StyxWoW.Me.Location.DistanceSqr(trashToNadoxTankLoc) <= 25*25 && ScriptHelpers.GetUnfriendlyNpsAtLocation(() => trashToNadoxLoc, 70, u => u.Entry == 30283).Any(),
                    new PrioritySelector(
                        ctx => trash = ScriptHelpers.GetUnfriendlyNpsAtLocation(() => trashToNadoxLoc, 70, u => u.Entry == 30283).FirstOrDefault(),
                        ScriptHelpers.CreatePullNpcToLocation(
                            ctx => trash != null, ctx => trash.Location.DistanceSqr(trashToNadoxLoc) <= 7*7, () => trash, () => trashToNadoxTankLoc,
                            () => StyxWoW.Me.IsTank() ? trashToNadoxWaitLoc : trashToNadoxTankLoc, 10)))
                );
        }

        [EncounterHandler(29309, "Elder Nadox")]
        public Composite ElderNadoxEncounter()
        {
            return new PrioritySelector(
                );
        }

        WoWUnit _princeTaldaram;

        [EncounterHandler(29308, "Prince Taldaram", Mode = CallBehaviorMode.CurrentBoss)]
        public Composite PrinceTaldaramEncounter()
        {
            WoWGameObject device = null;

            return new PrioritySelector(
                ctx =>
                    {
                        device =
                            ObjectManager.GetObjectsOfType<WoWGameObject>().Where(o => (o.Entry == 193093 || o.Entry == 193094) && o.State == WoWGameObjectState.Ready).OrderBy(o => o.DistanceSqr).FirstOrDefault
                                ();
                        return _princeTaldaram = ctx as WoWUnit;
                    },
                new Decorator(
                    ctx => device != null,
                    new PrioritySelector(
                        new Decorator(
                            ctx => device.DistanceSqr > 5*5,
                            new Action(ctx => ScriptHelpers.MoveTankTo(device.Location))),
                        new Decorator(
                            ctx => device.DistanceSqr <= 5*5 && !StyxWoW.Me.Combat,
                            ScriptHelpers.CreateInteractWithObject(() => device))
                        ))
                );
        }

        WoWUnit _jedogaShadowseeker;

        [EncounterHandler(29310, "Jedoga Shadowseeker", Mode = CallBehaviorMode.Proximity)]
        public Composite JedogaShadowseekerEncounter()
        {
            return new PrioritySelector(
                ctx => _jedogaShadowseeker = ctx as WoWUnit,
                ScriptHelpers.CreateRunAwayFromBad(ctx => true, 14, 60029, 56926),
                ScriptHelpers.CreateClearArea(() => _jedogaShadowseeker.Location, 30, u => u != _jedogaShadowseeker)
                );
        }

        WoWUnit _heraldVolazj;

        [EncounterHandler(29311, "Herald Volazj", Mode = CallBehaviorMode.Proximity, BossRange = 100)]
        public Composite HeraldVolazjEncounter()
        {
            var trashLoc = new WoWPoint(539.0362, -521.6564, 26.35604);

            return new PrioritySelector(
                ctx => _heraldVolazj = ctx as WoWUnit,
                ScriptHelpers.CreateClearArea(() => trashLoc, 100, u => u != _heraldVolazj)
                );
        }
    }
}