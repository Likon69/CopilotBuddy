using System.Collections.Generic;
using System.Linq;
using CommonBehaviors.Actions;
using Styx;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Pathing;
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
    public class StratholmeServicesEntrance : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 274; } }

        public override WoWPoint Entrance { get { return new WoWPoint(3236.26, -4055.314, 108.464); } }
        public override WoWPoint ExitLocation { get { return new WoWPoint(3588.10, -3638.56, 138.47); } }

        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            foreach (Targeting.TargetPriority p in units)
            {
                WoWUnit unit = p.Object.ToUnit();
                if (unit.Entry == 10411) // Eye of Naxxramas
                    p.Score += 120;
            }
        }

        public override void IncludeTargetsFilter(List<WoWObject> incomingunits, HashSet<WoWObject> outgoingunits)
        {
            foreach (WoWUnit unit in incomingunits.Select(obj => obj.ToUnit()))
            {
                switch (unit.Entry)
                {
                    case 10411: // Eye of Naxxramas
                    case 10441: // Plague Rat
                        outgoingunits.Add(unit);
                        break;

                    case 10399: // Acrylic
                    case 10416: // Bile Spewer
                    case 10417: // Venom Belcher

                        if ((StyxWoW.Me.Role & WoWPartyMember.GroupRole.Tank) > 0)
                            outgoingunits.Add(unit);
                        break;
                }
            }
        }

        #endregion

        private static readonly CircularQueue<WoWPoint> CourtYardPath = new CircularQueue<WoWPoint>
                                                                            {
                                                                                new WoWPoint(4027.646, -3430.52, 118.1971),
                                                                                new WoWPoint(3986.372, -3397.229, 118.4226),
                                                                                new WoWPoint(4027.646, -3430.52, 118.1971),
                                                                                new WoWPoint(4085.29, -3395.159, 115.653)
                                                                            };


        [ObjectHandler(175405, "Doodad_ZigguratDoor04", 80)]
        public Composite ClearTheCourtYardEncounter()
        {
            WoWGameObject zigguratDoor04 = null;

            return new PrioritySelector(
                ctx => zigguratDoor04 = ctx as WoWGameObject,
                new Decorator(
                    ctx =>
                    StyxWoW.Me == ScriptHelpers.Tank && zigguratDoor04.State == WoWGameObjectState.Ready &&
                    (!ScriptHelpers.IsBossAlive("Magistrate Barthilas") || !ScriptHelpers.ShouldKillBoss("Magistrate Barthilas")),
                    new PrioritySelector(
                        ctx => CourtYardPath.Peek(),
                        new Decorator(
                            ctx => StyxWoW.Me.Location.Distance2DSqr((WoWPoint)ctx) < 5 * 5,
                            new Action(ctx => CourtYardPath.Dequeue())),
                        new Action(
                            ctx => ScriptHelpers.MoveTankTo((WoWPoint)ctx))))
                );
        }
    }
}