using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonBehaviors.Actions;
using Styx;
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
    class BlackfathomDeeps : Dungeon
    {
        #region Overrides of Dungeon
        public override uint DungeonId { get { return 10; } }

        public override WoWPoint Entrance { get { return new WoWPoint(4247.842, 750.5999, -22.39564); } }
        public override WoWPoint ExitLocation { get { return new WoWPoint(-150.17, 113.01, -40.54); } }

        public override void IncludeTargetsFilter(List<WoWObject> incomingunits, HashSet<WoWObject> outgoingunits)
        {
            base.IncludeTargetsFilter(incomingunits, outgoingunits);

            foreach (var unit in incomingunits.Select(obj => obj.ToUnit()))
            {
                if (unit.Entry == 4825 && WoWGameObject != null)
                {
                    outgoingunits.Remove(unit);
                }
            }
        }

        #endregion

        #region Encounter Handlers


        public WoWGameObject WoWGameObject
        {
            get
            {
                return (ObjectManager.GetObjectsOfType<WoWGameObject>().Where(u => (u.Entry == 21118 || u.Entry == 21121 || u.Entry == 21120 || u.Entry == 21119) && !u.Locked)).FirstOrDefault();
            }
        }


        public WoWUnit DetectedMobs
        {
            get
            {
                return (ObjectManager.GetObjectsOfType<WoWUnit>().Where(u => (u.Z >= WoWGameObject.Z - 5) && u.IsAlive && u.IsHostile && u.Entry != 4825 && u.Location.Distance(WoWGameObject.Location) < 50).OrderByDescending(u => u.DistanceSqr)).FirstOrDefault();

            }
        }
    

        [EncounterHandler(0)]
        public Composite RootLogic()
        {
            return
                new PrioritySelector(
                     ScriptHelpers.CreateForceJump(nat => StyxWoW.Me.Location.Distance(new WoWPoint(-360.4622f, 35.82073f, -53.28525f)) < 3, true, (new WoWPoint(-360.4622f, 35.82073f, -53.28525f)), (new WoWPoint(-354.1001f, 35.91189f, -53.12907f))),
                     ScriptHelpers.CreateForceJump(nat => StyxWoW.Me.Location.Distance(new WoWPoint(-337.5258f, 43.51491f, -53.12798f)) < 3, true, (new WoWPoint(-337.5258f, 43.51491f, -53.12798f)), (new WoWPoint(-334.1969f, 48.62948f, -53.12798f))),
                     ScriptHelpers.CreateForceJump(nat => StyxWoW.Me.Location.Distance(new WoWPoint(-329.5626f, 49.79887f, -53.12798f)) < 3, true, (new WoWPoint(-329.5626f, 49.79887f, -53.12798f)), (new WoWPoint(-322.9993f, 49.80068f, -53.12935f))),
                     ScriptHelpers.CreateForceJump(nat => StyxWoW.Me.Location.Distance(new WoWPoint(-314.7013f, 62.1413f, -53.12996f)) < 3, true, (new WoWPoint(-314.7013f, 62.1413f, -53.12996f)), (new WoWPoint(-314.6349f, 68.79098f, -53.5784f))),
                     


					new Decorator(ctx => ScriptHelpers.Tank == StyxWoW.Me && !ScriptHelpers.IsBossAlive("Twilight Lord Kelris") && WoWGameObject != null && DetectedMobs == null, // Check if boss is dead
                        new PrioritySelector(
                            new Decorator(ctx => !WoWGameObject.WithinInteractRange, // If no mobs are detected that are hostile and there are still torches
                                       new Action(ctx => Navigator.MoveTo(WoWGameObject.Location))),
                            new Decorator(ctx => WoWGameObject.WithinInteractRange, // Activates the torches
                                       new Sequence(new Action(ctx => WoWGameObject.Interact()), new Wait(10, r=> DetectedMobs != null,new ActionAlwaysSucceed()))))
                                       ));

        }

        #endregion
    }
}
