using System.Collections.Generic;
using System.Linq;
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
    public class TheBotanicaHeroic : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 191; } }

        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            foreach (var priority in units)
            {
                var unit = priority.Object as WoWUnit;
                if (unit != null)
                {
                    if (unit.Entry == 19919) // Thorn Lasher
                    {
                        if (StyxWoW.Me.IsDps())
                            priority.Score += 5000;
                        else if (StyxWoW.Me.IsTank())
                            priority.Score -= 100;
                    }
                    if (unit.Entry == 19920) // Thorn Flayer
                    {
                        if (StyxWoW.Me.IsDps())
                            priority.Score += 5000;
                        else if (StyxWoW.Me.IsTank())
                            priority.Score -= 100;
                    }
                    if (unit.Entry == 19949 && StyxWoW.Me.IsTank()) //Sapling
                        priority.Score -= 120;
                }
            }
        }

        #endregion

        [EncounterHandler(19486, "Sunseeker Chemist")]
        public Composite SunseekerChemistEncounter()
        {
            return new PrioritySelector(
                ScriptHelpers.CreateRunAwayFromBad(ctx => true, 8, 34358)
                );
        }

        private WoWUnit _commanderSarannis;

        [EncounterHandler(17976, "Commander Sarannis", Mode = CallBehaviorMode.Proximity, BossRange = 60)]
        public Composite CommanderSarannisEncounter()
        {
            WoWUnit trash = null;
            var tankTrashLoc = new WoWPoint(107.7079, 290.2441, -6.796101);
            var trashWaitLoc = new WoWPoint(117.333, 281.3033, -5.778226);
            var trashLoc = new WoWPoint(150.9637, 296.034, -4.57425);

            return new PrioritySelector(
                ctx => _commanderSarannis = ctx as WoWUnit,
                new Decorator(
                    ctx => !_commanderSarannis.Combat,
                    new PrioritySelector(
                        ctx => trash = ScriptHelpers.GetUnfriendlyNpsAtLocation(() => trashLoc, 25, u => u != _commanderSarannis).FirstOrDefault(),
                        ScriptHelpers.CreatePullNpcToLocation(ctx => trash != null, ctx => _commanderSarannis.Location.DistanceSqr(trashLoc) > 25 * 25, () => trash, () => tankTrashLoc, () => trashWaitLoc, 10)
                        )),
                new Decorator(
                    ctx => _commanderSarannis.Combat,
                    new PrioritySelector(
                        
                        ))
                );
        }

        // WoWUnit _highBotanistFreywinn;

        [EncounterHandler(17975, "High Botanist Freywinn")]
        public Composite HighBotanistFreywinnEncounter()
        {
            return new PrioritySelector(
               // ctx => _highBotanistFreywinn = ctx as WoWUnit
                );
        }

        WoWUnit _thorngrinTheTender;

        [EncounterHandler(17978, "Thorngrin the Tender")]
        public Composite ThorngrinTheTenderEncounter()
        {
            return new PrioritySelector(
                ctx => _thorngrinTheTender = ctx as WoWUnit,
                ScriptHelpers.CreateRunAwayFromBad(ctx => StyxWoW.Me.IsFollower() && _thorngrinTheTender != null && (_thorngrinTheTender.CastingSpellId == 34659 || _thorngrinTheTender.CastingSpellId == 39131), 20, 17978)
                );
        }

        WoWUnit _laj;

        [EncounterHandler(17980, "Laj", Mode = CallBehaviorMode.Proximity)]
        public Composite LajEncounter()
        {
            var centerRoomLoc = new WoWPoint(-165.5613, 393.9581, -17.69983);
            return new PrioritySelector(
                ctx => _laj = ctx as WoWUnit,
                ScriptHelpers.CreateClearArea(() => centerRoomLoc, 60, u => u != _laj)
                );
        }

        //WoWUnit _warpSplinter;

        [EncounterHandler(17977, "Warp Splinter")]
        public Composite WarpSplinterEncounter()
        {
            return new PrioritySelector(
                //ctx => _warpSplinter = ctx as WoWUnit
                );
        }
    }
}