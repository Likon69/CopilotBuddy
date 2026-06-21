using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Styx;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Action = TreeSharp.Action;

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
    public class StratholmeMainGate : Dungeon
    {
        #region Overrides of Dungeon
        public override uint DungeonId
        {
            get { return 40; }
        }

        public override WoWPoint Entrance { get { return new WoWPoint(3392.334, -3358.307, 142.7716); } }
        public override WoWPoint ExitLocation { get { return new WoWPoint(3393.08, -3356.25, 142.25); } }

        public override void WeighTargetsFilter(List<Styx.Logic.Targeting.TargetPriority> units)
        {
            foreach (var p in units)
            {
                var unit = p.Object.ToUnit();
                if (unit.Entry == 10411) // Eye of Naxxramas
                    p.Score += 100;
            }
        }

        public override void IncludeTargetsFilter(List<WoWObject> incomingunits, HashSet<WoWObject> outgoingunits)
        {
            foreach (var unit in incomingunits.Select(obj => obj.ToUnit()))
            {
                switch (unit.Entry)
                {
                    case 10411: // Eye of Naxxramas
                    case 10441: // Plague Rat
                        outgoingunits.Add(unit);
                        break;
                }
            }
        }
        #endregion

        readonly WaitTimer _canonTimer = new WaitTimer(TimeSpan.FromSeconds(15));

        [EncounterHandler(10997, "Willey Hopebreaker")]
        public Composite WilleyHopebreakerFight()
        {
            return
                new PrioritySelector(
                    new Decorator(ctx => StyxWoW.Me == ScriptHelpers.Tank && _canonTimer.IsFinished,
                        new Sequence(
                            new Action(ctx => ScriptHelpers.CreateInteractWithObject(176215, 0, true)),
                            new Action(ctx => ScriptHelpers.CreateInteractWithObject(176216, 0, true)),
                            new Action(ctx => _canonTimer.Reset()))));
        }
    }
}
