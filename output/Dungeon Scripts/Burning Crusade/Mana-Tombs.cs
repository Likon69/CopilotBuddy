using System.Linq;

using CommonBehaviors.Actions;
using Styx;
using Styx.Logic;
using Styx.Logic.Combat;
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
    public class ManaTombs : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 148; } }

        public override WoWPoint Entrance { get { return new WoWPoint(-3074.542, 4943.176, -101.0476); } }

        #endregion

        private WoWUnit _pandemonius;

        [EncounterHandler(18341, "Pandemonius", Mode = CallBehaviorMode.Proximity, BossRange = 80)]
        public Composite PandemoniusEncounter()
        {
            var pandemoniusRoomAddsloc1 = new WoWPoint(-44.84757, -80.27489, -2.326236);
            var pandemoniusRoomAddsloc2 = new WoWPoint(-94.44795, -85.13178, -2.020432);
            var pandemoniusTankSpot = new WoWPoint(-61.92603, -95.23901, -0.4504707);

            return new PrioritySelector(
                ctx => _pandemonius = ctx as WoWUnit,
                new Decorator(
                    ctx => ScriptHelpers.GetUnfriendlyNpsAtLocation(() => pandemoniusRoomAddsloc1, 25, u => u != _pandemonius).Any(),
                    ScriptHelpers.CreateClearArea(() => pandemoniusRoomAddsloc1, 25, u => u != _pandemonius)),
                new Decorator(
                    ctx => !ScriptHelpers.GetUnfriendlyNpsAtLocation(() => pandemoniusRoomAddsloc1, 25, u => u != _pandemonius).Any(),
                    ScriptHelpers.CreateClearArea(() => pandemoniusRoomAddsloc2, 25, u => u != _pandemonius)),
                // stop attacking if boss has Dark Shell
                new Decorator(
                    ctx => _pandemonius.HasAura("Dark Shell") && StyxWoW.Me.IsRange() && StyxWoW.Me.IsDps() && StyxWoW.Me.PowerType == WoWPowerType.Mana,
                    new PrioritySelector(
                        new ActionLogger("Dark Shell is up, stopping dps."),
                        new Decorator(
                            ctx => StyxWoW.Me.IsCasting,
                            new Action(ctx => SpellManager.StopCasting())),
                        new ActionAlwaysSucceed())),
                new Decorator(
                    ctx => Targeting.Instance.FirstUnit == _pandemonius,
                    ScriptHelpers.CreateTankUnitAtLocation(() => pandemoniusTankSpot, 3))
                );
        }

        [EncounterHandler(18373, "Tavarok")]
        public Composite TavarokEncounter()
        {
            return new PrioritySelector(
                ScriptHelpers.CreateTankFaceAwayGroupUnit(15)
                );
        }

        private WoWUnit _shaffar;

        [EncounterHandler(18344, "Nexus-Prince Shaffar")]
        public Composite NexusPrinceShaffarEncounter()
        {
            return new PrioritySelector(
                ctx => _shaffar = ctx as WoWUnit,
                ScriptHelpers.CreateClearArea(() => _shaffar.Location, 70, u => u != _pandemonius)
                );
        }
    }
}