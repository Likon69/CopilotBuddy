using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonBehaviors.Actions;
using Styx;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
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
    public class ZulFarrak : Dungeon
    {
        #region Overrides of Dungeon
        public override uint DungeonId
        {
            get { return 24; }
        }

        public override void IncludeTargetsFilter(List<WoWObject> incomingunits, HashSet<WoWObject> outgoingunits)
        {
            foreach (var unit in incomingunits.Select(obj => obj.ToUnit()))
            {
                switch (unit.Entry)
                {
                    case 6017: // Lava Spout Totem
                    case 6066: // Earthgrab Totem
                    case 8179: // Greater Healing Ward
                    case 7785: // Ward of Zum'rah
                        outgoingunits.Add(unit);
                        break;

                    case 8877: // Sandfury Zealot
                    case 7787: // Sandfury Slave
                    case 7788: // Sandfury Drudge
                    case 8876: // Sandfury Acolyte
                    if (unit.Combat)
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
                    case 6017: // Lava Spout Totem
                    case 6066: // Earthgrab Totem
                    case 8179: // Greater Healing Ward
                    case 7785: // Ward of Zum'rah
                    case 7797: // Ruuzlu
                        p.Score += 120;
                        break;
                }

            }
        }
        #endregion

        [ObjectHandler(141832, "Gong of Zul'Farrak", 100)]
        public Composite GongHandler()
        {

            WoWGameObject gong = null;
            return new PrioritySelector(
                new Decorator(ctx => ScriptHelpers.IsBossAlive("Gahz'rilla") && StyxWoW.Me == ScriptHelpers.Tank && Targeting.Instance.FirstUnit == null && Gahzrilla == null,
                    new PrioritySelector(ctx => gong = ctx as WoWGameObject,
                        new ActionSetActivity("Interacting with {0}", gong),
                        new Decorator(ctx => gong != null && gong.DistanceSqr > 4 * 4,
                            new Action(ctx => Navigator.MoveTo(gong.Location))),

                        new Decorator(ctx => gong != null && gong.DistanceSqr <= 4 * 4,
                            new Sequence(
                                new Action(ctx => gong.Interact()))))));

        }

        static readonly WoWPoint FinalBlyLocation = new WoWPoint(1885.295, 1202.984, 8.877242);

        [ObjectHandler(146084, "End Door", 180)]
        public Composite PrisonerEvent()
        {
            WoWGameObject endDoor = null;
            WoWGameObject trollCage = null;
            WoWUnit sergeantBly = null;

            return new PrioritySelector(ctx => endDoor = ctx as WoWGameObject,
                new Decorator(ctx => endDoor.State != WoWGameObjectState.Active && StyxWoW.Me == ScriptHelpers.Tank,

                // context switch to troll cage
                new PrioritySelector(ctx => trollCage = TrollCage,
                // cages are closed, lets go open them.
                    new Decorator(ctx => trollCage != null && trollCage.State == WoWGameObjectState.Ready,
                        new PrioritySelector(
                            new Decorator(ctx => trollCage.DistanceSqr > 4 * 4,
                                new Action(ctx => ScriptHelpers.MoveTankTo(trollCage.Location))),
                            new Decorator(ctx => trollCage.DistanceSqr <= 4 * 4,
                                ScriptHelpers.CreateInteractWithObject(141071)))),

                    // cages are open lets help the prisoners fight off the waves of NPCs
                    new Decorator(ctx => trollCage != null && trollCage.State == WoWGameObjectState.Active,

                        // context switch to sergent bly,
                        new PrioritySelector(ctx => sergeantBly = SergeantBly,
                            new Decorator(ctx => sergeantBly != null && !sergeantBly.Combat && sergeantBly.Location.DistanceSqr(FinalBlyLocation) < 7,
                                ScriptHelpers.CreateTalkToNpc(7604)),

                            new Decorator(ctx => sergeantBly != null && sergeantBly.DistanceSqr >= 6 * 6,
                                new Action(ctx => ScriptHelpers.MoveTankTo(sergeantBly.Location))),
                // prevent bot from moving anywhere else.
                            new Decorator(ctx => sergeantBly != null && sergeantBly.DistanceSqr < 6 * 6 && Targeting.Instance.FirstUnit == null,
                                new ActionAlwaysSucceed()))))));
        }


        WoWGameObject TrollCage
        {
            get
            {
                return
                    ObjectManager.GetObjectsOfType<WoWGameObject>().Where(o => o.Entry == 141071).OrderBy(o => o.DistanceSqr)
                        .FirstOrDefault();
            }
        }

        WoWUnit SergeantBly
        {
            get
            {
                return
                    ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(o => o.Entry == 7604);
            }
        }

        WoWUnit Gahzrilla
        {
            get
            {
                return
                    ObjectManager.GetObjectsOfType<WoWUnit>().FirstOrDefault(o => o.Entry == 7273);
            }
        }
    }
}
