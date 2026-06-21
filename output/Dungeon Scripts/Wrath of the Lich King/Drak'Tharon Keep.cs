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
    public class DrakTharonKeep : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 214; } }

        public override WoWPoint Entrance { get { return new WoWPoint(4774.611, -2023.276, 229.3549); } }

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
                }
            }
        }

        #endregion

        [EncounterHandler(0, "Root Behavior")]
        public Composite RootBehavior()
        {
            return new PrioritySelector(
                ScriptHelpers.CreateRunAwayFromBad(ctx => true, 13, 55847, 59014) // shadow voids.
                );
        }


        [EncounterHandler(26624, "Wretched Belcher")]
        public Composite WretchedBelcherEncounter()
        {
            return new PrioritySelector(
                new Decorator(
                    ctx => StyxWoW.Me.CurrentTarget == (WoWUnit)ctx,
                    ScriptHelpers.CreateTankFaceAwayGroupUnit(15))
                );
        }

        WoWUnit _trollgore;

        [EncounterHandler(26630, "Trollgore", Mode = CallBehaviorMode.Proximity, BossRange = 110)]
        public Composite TrollgoreEncounter()
        {
            List<WoWUnit> belchers = null;

            var trashTankLoc = new WoWPoint(-355.6056, -624.7963, 11.02102);
            var followerWaitLoc = new WoWPoint(-347.375, -614.9806, 11.01204);
            var pullLoc = new WoWPoint(-338.9121, -630.8854, 11.38);
            var roomCenterLoc = new WoWPoint(-312.3778, -659.7048, 10.28416);

            return new PrioritySelector(
                ctx =>
                {
                    belchers = ScriptHelpers.GetUnfriendlyNpsAtLocation(() => roomCenterLoc, 30, u => u.Entry == 26624); // Wretched Belcher
                    return _trollgore = ctx as WoWUnit;
                },
                ScriptHelpers.CreateRunAwayFromBad(ctx => _trollgore.CastingSpellId == 49555 || _trollgore.CastingSpellId == 59807, () => _trollgore.Location, 35, 10, u => u.Entry == 27753 && ((WoWUnit)u).Dead),
                ScriptHelpers.CreatePullNpcToLocation(
                    ctx => belchers.Any(),
                    ctx => belchers[0].DistanceSqr <= 30 * 30 && (belchers.Count == 1 || belchers[0].Location.DistanceSqr(belchers[1].Location) > 25 * 25),
                    () => belchers[0], () => trashTankLoc, () => StyxWoW.Me.IsTank() ? pullLoc : followerWaitLoc, 10)
                );
        }

        [EncounterHandler(26631, "Novos the Summoner")]
        public Composite NovosTheSummonerEncounter()
        {
            return new PrioritySelector(
                ScriptHelpers.CreateRunAwayFromBad(ctx => true, 15, 47346), // Arcane field
                ScriptHelpers.CreateRunAwayFromBad(ctx => StyxWoW.Me.IsRange(), 13, 49034) // blizard
                );
        }

        private WoWUnit _kingDred;

        [EncounterHandler(27483, "King Dred", Mode = CallBehaviorMode.Proximity, BossRange = 120)]
        public Composite KingDredEncounter()
        {
            var tankLoc = new WoWPoint(-494.3439, -721.4702, 30.24773);
            var dredSafeLoc = new WoWPoint(-535.8426, -664.3137, 30.2464);
            var trashLoc = new WoWPoint(-525.6827, -714.9271, 30.24642);
            WoWUnit trash = null;
            return new PrioritySelector(
                ctx =>
                {
                    _kingDred = ctx as WoWUnit;
                    trash = ScriptHelpers.GetUnfriendlyNpsAtLocation(() => trashLoc, 30, u => u != _kingDred).FirstOrDefault();
                    return _kingDred;
                },
                new Decorator(
                    ctx => !_kingDred.Combat,
                    new PrioritySelector(
                        new Decorator(
                            ctx => StyxWoW.Me.IsTank() && Targeting.Instance.FirstUnit == null && StyxWoW.Me.Location.DistanceSqr(tankLoc) > 25,
                            new TreeSharp.Action(ctx => ScriptHelpers.MoveTankTo(tankLoc))),
                        ScriptHelpers.CreatePullNpcToLocation(ctx => trash != null, ctx => _kingDred.Location.DistanceSqr(dredSafeLoc) <= 20 * 20 && !StyxWoW.Me.Combat, () => trash, () => tankLoc, () => tankLoc, 10),
                        ScriptHelpers.CreatePullNpcToLocation(ctx => trash == null, ctx => _kingDred.Location.DistanceSqr(trashLoc) <= 25 * 25 && !StyxWoW.Me.Combat, () => _kingDred, () => tankLoc, () => tankLoc, 10)))
                );
        }

        [EncounterHandler(26632, "The Prophet Tharon'ja")]
        public Composite TheProphetTharonjaEncounter()
        {
            return new PrioritySelector(
                ScriptHelpers.CreateRunAwayFromBad(ctx => true, 12, 49548, 59969, 49518, 59971),
                new Decorator(
                    ctx => StyxWoW.Me.HasAura("Gift of Tharon'ja"),
                    new TreeSharp.Action(ctx => Lua.DoString(ZombieDpsRotation)))
                );
        }

        private const string ZombieDpsRotation =
            @"local _,s,_ = GetActionInfo(120 + 4) if s then CastSpellByID(s) end s,_ = GetActionInfo(120 + 3) if s then CastSpellByID(s) end s,_ = GetActionInfo(120 + 1) if s then CastSpellByID(s) end 
";
    }
}