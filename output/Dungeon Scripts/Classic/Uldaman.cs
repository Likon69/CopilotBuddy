using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonBehaviors.Actions;
using Styx;
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
    public class Uldaman : Dungeon
    {
        #region Overrides of Dungeon
        public override uint DungeonId
        {
            get { return 22; }
        }

        public override WoWPoint Entrance { get { return new WoWPoint(-6063.326,-2955.967,209.7707); } }

        public override void IncludeTargetsFilter(List<WoWObject> incomingunits, HashSet<WoWObject> outgoingunits)
        {
            foreach (var unit in incomingunits.Select(obj => obj.ToUnit()))
            {
                switch (unit.Entry)
                {
                    case 3560: // healing totem
                    case 6017: // Lava sprout totem
                        outgoingunits.Add(unit);
                        break;
                }
            }
        }

        public override void WeighTargetsFilter(List<Styx.Logic.Targeting.TargetPriority> units)
        {
            foreach (var p in units)
            {
                WoWUnit unit = p.Object.ToUnit();
                switch (unit.Entry)
                {
                    case 3560: // healing totem
                    case 6017: // Lava sprout totem
                        p.Score += 100;
                        break;
                }

            }
        }

        #endregion

        [ObjectHandler(124371, "Keystone")]
        public Composite KeystoneObject()
        {
            return new Decorator(ctx => !StyxWoW.Me.Combat && ((WoWGameObject)ctx).Locked == false,
                ScriptHelpers.CreateInteractWithObject(124371));
        }

        [ObjectHandler(130511, "Altar of The Keepers")]
        public Composite AltarofTheKeepers()
        {
            WoWGameObject door = null;
            return new PrioritySelector(ctx =>
                {
                    door = HallOfCraftersDoor; // cache the door object.
                    return ctx;
                },
                 new Decorator(ctx => !StyxWoW.Me.Combat && door != null && door.State != WoWGameObjectState.Active,
                    ScriptHelpers.CreateInteractWithObject(130511,12)));
        }

        [ObjectHandler(133234, "Altar of The Archaeda")]
        public Composite AltarofTheArchaedas()
        {
            return new Decorator(ctx => !StyxWoW.Me.Combat && Math.Abs(((WoWGameObject)ctx).Location.Z - StyxWoW.Me.Location.Z) < 10
                                && BossManager.CurrentBoss != null && BossManager.CurrentBoss.Entry == 2748 && BossManager.CurrentBoss.ToWoWUnit() == null,
                    ScriptHelpers.CreateInteractWithObject(133234, 12));
        }

        [ObjectHandler(141979, "Ancient Treasure")]
        public Composite AncientTreasure()
        {
            return new Decorator(ctx => BossManager.CurrentBoss == null && ((WoWGameObject)ctx).CanLoot,
                    ScriptHelpers.CreateInteractWithObject(141979, 4));
        }

        WoWGameObject HallOfCraftersDoor
        {
            get { return ObjectManager.GetObjectsOfType<WoWGameObject>().FirstOrDefault(o => o.Entry == 124367); }
        }

    }
}
