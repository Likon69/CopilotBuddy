using System.Collections.Generic;
using System.Linq;
using CommonBehaviors.Actions;
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
    public class HallsOfStone : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 208; } }

        public override WoWPoint Entrance { get { return new WoWPoint(8921.653, -966.4353, 1039.165); } }

        public override void OnEnter() { _escortedBranToEncounter = false; }

        public override bool IsFlyingCorpseRun { get { return true; } }

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
                    if (unit.CurrentTarget != null && unit.CurrentTarget.Entry == 28070) // targeting Brann
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
                    if (unit.CurrentTarget != null && unit.CurrentTarget.Entry == 28070) // targeting Brann.
                        priority.Score += 500;
                }
            }
        }

        #endregion

        WoWUnit _krystallus = null;

        [EncounterHandler(27977, "Krystallus")]
        public Composite KrystallusEncounter()
        {
            return new PrioritySelector(
                ctx => _krystallus = ctx as WoWUnit,
                ScriptHelpers.CreateRunAwayFromBad(
                // run from party members that have Petrifying Grip
                    ctx => StyxWoW.Me.HasAura("Petrifying Grip"), () => _krystallus.Location, 40, 15, u => u != StyxWoW.Me && u is WoWPlayer && ((WoWPlayer)u).HasAura("Petrifying Grip"))
                );
        }

        [EncounterHandler(27975, "Maiden of Grief")]
        public Composite MaidenOfGriefEncounter()
        {
            return new PrioritySelector(
                ScriptHelpers.CreateRunAwayFromBad(ctx => true, 15, 50752) // Storm of Grief
                );
        }

        [EncounterHandler(28234, "Tribunal of Ages", Mode = CallBehaviorMode.CurrentBoss)]
        public Composite TribunalOfAgesEncounter()
        {
            WoWUnit brann = null;
            WoWGameObject chest = null, floor = null;
            var brannOriginalLoc = new WoWPoint(1077.41, 474.1604, 207.7255);
            var brannEncounterEntranceLoc = new WoWPoint(939.6468, 375.4893, 207.4221);
            var brannEncounterFinishedLoc = new WoWPoint(917.253, 351.925, 203.7064);

            return new PrioritySelector(
                ctx =>
                {
                    floor = ObjectManager.GetObjectsOfType<WoWGameObject>().FirstOrDefault(o => o.Entry == 191527);
                    chest = ObjectManager.GetObjectsOfType<WoWGameObject>().FirstOrDefault(o => o.Entry == 190586 && o.CanUse());
                    brann = ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.Entry == 28070);
                    if (!_escortedBranToEncounter && brann != null)
                        _escortedBranToEncounter = brann.Location.DistanceSqr(brannEncounterEntranceLoc) < 5 * 5;
                    return ctx;
                },
                new Decorator(
                    ctx => !_escortedBranToEncounter,
                    new PrioritySelector(
                        new Decorator(
                            ctx => brann == null,
                            new PrioritySelector(
                                new Decorator(
                                    ctx => Targeting.Instance.FirstUnit == null && StyxWoW.Me.Location.DistanceSqr(brannOriginalLoc) > 50 * 50,
                                    new Action(ctx => ScriptHelpers.MoveTankTo(brannOriginalLoc))),
                                new Decorator(
                                    ctx => StyxWoW.Me.Location.DistanceSqr(brannOriginalLoc) <= 50 * 50,
                                    new Action(ctx => _escortedBranToEncounter = true))
                                )),
                        new Decorator(
                            ctx => brann != null,
                            ScriptHelpers.CreateTankTalkToThenEscortNpc(28070, brannOriginalLoc, brannEncounterEntranceLoc)))),
                new Decorator(
                    ctx => _escortedBranToEncounter && chest == null,
                    new PrioritySelector(
                // talk to Brann to start the event.
                        new Decorator(
                            ctx => brann != null && brann.Location.DistanceSqr(brannEncounterEntranceLoc) < 3 * 3 && brann.CanGossip && StyxWoW.Me.IsTank(),
                            ScriptHelpers.CreateTalkToNpc(28070)),
                // talk to Brann to make him stfu
                        new Decorator(
                            ctx => brann != null && brann.Location.DistanceSqr(brannEncounterFinishedLoc) < 3 * 3 && brann.CanGossip && StyxWoW.Me.IsTank(),
                            ScriptHelpers.CreateTalkToNpc(28070)),
                // move to encounter area
                        new Decorator(
                            ctx => brann == null && StyxWoW.Me.Location.DistanceSqr(brannEncounterEntranceLoc) > 50 * 50 && StyxWoW.Me.IsTank(),
                            new Action(ctx => ScriptHelpers.MoveTankTo(brannEncounterEntranceLoc))),
                        new Decorator(
                            ctx => brann == null && StyxWoW.Me.Location.DistanceSqr(brannEncounterEntranceLoc) <= 15 * 15 && BossManager.CurrentBoss != null && BossManager.CurrentBoss.Entry == 28234,
                            new Action(ctx => BossManager.CurrentBoss.MarkAsDead())),
                        ScriptHelpers.CreateRunAwayFromBad(ctx => (StyxWoW.Me.IsTank() && !StyxWoW.Me.IsActuallyInCombat) || StyxWoW.Me.IsFollower(), 10, 28265), // Searing Gaze
                        ScriptHelpers.CreateRunAwayFromBad(ctx => (StyxWoW.Me.IsTank() && !StyxWoW.Me.IsActuallyInCombat) || StyxWoW.Me.IsFollower(), 13, 28237), // Dark Matter Target 

                        new Decorator(
                            ctx => StyxWoW.Me.IsTank() && Targeting.Instance.FirstUnit == null && brann != null,
                            new ActionAlwaysSucceed()) // don't go anywhere.
                        )),
                new Decorator(
                    ctx => chest != null, // loot chest
                    ScriptHelpers.CreateInteractWithObject(() => chest))
                );
        }

        private bool _escortedBranToEncounter;

        WoWUnit _sjonnirTheIronshaper;

        [EncounterHandler(27978, "Sjonnir The Ironshaper", Mode = CallBehaviorMode.CurrentBoss)]
        public Composite SjonnirTheIronshaperEncounter()
        {
            WoWUnit brann = null;
            var brannStartingLoc = new WoWPoint(1199.685, 667.155, 196.2405);

            return new PrioritySelector(
                ctx =>
                {
                    brann = ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(u => u.Entry == 28070);
                    return _sjonnirTheIronshaper = ctx as WoWUnit;
                },
                new Decorator(
                    ctx => Targeting.Instance.FirstUnit == null && brann != null && brann.Location.DistanceSqr(brannStartingLoc) < 2 * 2,
                    ScriptHelpers.CreateTalkToNpc(28070)),
                new Decorator(
                    ctx => _sjonnirTheIronshaper != null,
                    new PrioritySelector(
                        ScriptHelpers.CreateRunAwayFromBad(ctx => _sjonnirTheIronshaper.HasAura("Lightning Ring"), 14, 27978),
                // run from party members if afflicted with Static Charge.
                        ScriptHelpers.CreateRunAwayFromBad(ctx => StyxWoW.Me.HasAura("Static Charge"), 14, u => u is WoWPlayer && ((WoWPlayer)u).IsInMyParty),
                        ScriptHelpers.CreateDispellEnemy("Lightning Shield", ScriptHelpers.EnemyDispellType.Magic,() => _sjonnirTheIronshaper)
                        ))
                );
        }
    }
}