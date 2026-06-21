using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonBehaviors.Actions;
using Styx;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.POI;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Action = TreeSharp.Action;

#if USE_DUNGEONBUDDY_DLL
using Bots.DungeonBuddyDll;
using Bots.DungeonBuddyDll.Attributes;
using Bots.DungeonBuddyDll.Helpers;
namespace Bots.DungeonBuddyDll.Dungeon_Scripts.Classic
#else
    using Bots.DungeonBuddy.Attributes;
    using Bots.DungeonBuddy.Helpers;
    namespace Bots.DungeonBuddy.Dungeon_Scripts.Classic
#endif
{
    public class DireMaulCapitalGardens : Dungeon
    {
        #region Overrides of Dungeon
        public override uint DungeonId
        {
            get { return 36; }
        }

        public override WoWPoint Entrance { get { return new WoWPoint(-3817.948, 1250.378, 160.2729); } }

       // public override void IncludeTargetsFilter(List<WoWObject> incomingunits, HashSet<WoWObject> outgoingunits)
       // {
            //foreach (var unit in incomingunits.Select(obj => obj.ToUnit()))
            //{
            //    switch (unit.Entry)
            //    {
            //        case 11480: // Arcane Aberration
            //        case 11483: // Mana Remnant

            //            if (unit.IsAlive)
            //                outgoingunits.Add(unit);
            //            break;
            //    }
            //}
       // }

        public override void WeighTargetsFilter(List<Styx.Logic.Targeting.TargetPriority> units)
        {
            foreach (var p in units)
            {
                WoWUnit unit = p.Object.ToUnit();
                switch (unit.Entry)
                {
                    case 11459: // Ironbark Protector - adds on boss Tendris Warpwood
                        p.Score += 100;
                        break;
                }

            }
        }
        #endregion

        private bool _generator1IsDisabled, _generator2IsAliveIsDisabled, _generator3IsAliveIsDisabled, _generator4IsAliveIsDisabled, _generator5IsAliveIsDisabled;

        private readonly Dictionary<string, WoWPoint> _generatorLocations = new Dictionary<string, WoWPoint>
                                                                      {
                                                                          {"Crystal Generator 1", new WoWPoint(3.172923, 274.9728, -8.346642)},
                                                                          {"Crystal Generator 2", new WoWPoint(-81.00807, 442.6851, 28.60135)}, 
                                                                          {"Crystal Generator 3", new WoWPoint(113.8273, 435.8742, 28.60132)}, 
                                                                          {"Crystal Generator 4", new WoWPoint(66.86565, 739.7091, -24.58033)},
                                                                          {"Crystal Generator 5", new WoWPoint(-141.7635, 729.8258, -24.57926)},
                                                                      };

        // walk around the force field to take out 2 remaining crystal generators
        [ObjectHandler(179503, "Force Field", 2000)]
        public Composite ForceField()
        {
            return new PrioritySelector(
                new Decorator(ctx => !StyxWoW.Me.Combat && StyxWoW.Me == ScriptHelpers.Tank && ((WoWGameObject)ctx).State != WoWGameObjectState.Active 
                && Targeting.Instance.FirstUnit == null && BotPoi.Current.Type == PoiType.None,
                new PrioritySelector(ctx =>
                    { // update the crystal genorator status.
                        foreach (var obj in ObjectManager.GetObjectsOfType<WoWGameObject>())
                        {
                            switch (obj.Entry)
                            {
                                case 177259: // Crystal Generator 1
                                    _generator1IsDisabled = obj.State == WoWGameObjectState.Active;
                                    break;
                                case 177257: // Crystal Generator 2
                                    _generator2IsAliveIsDisabled = obj.State == WoWGameObjectState.Active;
                                    break;
                                case 177258: // Crystal Generator 3
                                    _generator3IsAliveIsDisabled = obj.State == WoWGameObjectState.Active;
                                    break;
                                case 179504: // Crystal Generator 4
                                    _generator4IsAliveIsDisabled = obj.State == WoWGameObjectState.Active;
                                    break;
                                case 179505: // Crystal Generator 5
                                    _generator5IsAliveIsDisabled = obj.State == WoWGameObjectState.Active;
                                    break;
                            }
                        }
                        return ctx;
                    },

                    new PrioritySelector(
                        new Decorator(ctx => !_generator1IsDisabled,
                            new Sequence(
                                new ActionSetActivity("Moving towards Crystal Generator 1"),
                                new Action(ctx => ScriptHelpers.MoveTankTo(_generatorLocations["Crystal Generator 1"])))),

                        new Decorator(ctx => !_generator3IsAliveIsDisabled && !ScriptHelpers.IsBossAlive("Tendris Warpwood") ,
                            new Sequence(
                                new ActionSetActivity("Moving towards Crystal Generator 3"),
                                new Action(ctx => ScriptHelpers.MoveTankTo(_generatorLocations["Crystal Generator 3"])))),

                        new Decorator(ctx => !_generator2IsAliveIsDisabled && !ScriptHelpers.IsBossAlive("Tendris Warpwood") ,
                            new Sequence(
                                new ActionSetActivity("Moving towards Crystal Generator 2"),
                                new Action(ctx => ScriptHelpers.MoveTankTo(_generatorLocations["Crystal Generator 2"])))),

                        new Decorator(ctx => !_generator4IsAliveIsDisabled && !ScriptHelpers.IsBossAlive("Magister Kalendris"),
                            new Sequence(
                                new ActionSetActivity("Moving towards Crystal Generator 4"),
                                new Action(ctx => ScriptHelpers.MoveTankTo(_generatorLocations["Crystal Generator 4"])))),

                        new Decorator(ctx => !_generator5IsAliveIsDisabled && !ScriptHelpers.IsBossAlive("Magister Kalendris"),
                            new Sequence(
                                new ActionSetActivity("Moving towards Crystal Generator 5"),
                                new Action(ctx => ScriptHelpers.MoveTankTo(_generatorLocations["Crystal Generator 5"]))))))));
        }
    }
}
