using System.Collections.Generic;
using Styx;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Combat;
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
    public class SlavePensHeroic : Dungeon
    {
        #region Overrides of Dungeon

        public override uint DungeonId { get { return 184; } }

        #endregion

        private readonly CircularQueue<WoWPoint> _corpseRunBreadCrumb = new CircularQueue<WoWPoint>
                                                                            {
                                                                                new WoWPoint(275.8778, 7105.589, 37.36536),
                                                                                new WoWPoint(509.0042, 6966.605, 18.2682),
                                                                                new WoWPoint(563.3763, 6942.223, -12.53707),
                                                                                new WoWPoint(577.0681, 6939.776, -33.42987),
                                                                                new WoWPoint(605.0372, 6913.489, -45.16651),
                                                                                new WoWPoint(611.9238, 6893.966, -52.48455),
                                                                                new WoWPoint(638.6559, 6868.03, -78.95798),
                                                                                new WoWPoint(734.209, 6862.973, -69.25185),
                                                                                // entrance to coilfang dungeon area.
                                                                                new WoWPoint(737.5855, 6911.762, -65.50684),
                                                                                new WoWPoint(713.8449, 6949.227, -66.97269),
                                                                                new WoWPoint(726.3149, 7009.365, -72.30524),
                                                                                new WoWPoint(739.5865, 7014.064, -72.70363),
                                                                            };

        public override CircularQueue<WoWPoint> CorpseRunBreadCrumb { get { return _corpseRunBreadCrumb; } }

        public override bool IsFlyingCorpseRun { get { return true; } }

        /// <summary>
        ///   Dungeon specific unit target removal.
        /// </summary>
        /// <param name="units"> The incomingunits. </param>
        public override void RemoveTargetsFilter(List<WoWObject> units)
        {
            for (int i = units.Count - 1; i >= 0; i--)
            {
                var u = units[i] as WoWUnit;
                if (u is WoWPlayer)
                    units.RemoveAt(i);
            }
        }

        public override void IncludeTargetsFilter(List<WoWObject> incomingunits, HashSet<WoWObject> outgoingunits)
        {
            foreach (WoWObject obj in incomingunits)
            {
                // for the mennu bossfight.
                const uint mennuHealingWard = 20208;
                const uint taintedStoneskinTotem = 18177;
                const uint mennuTheBetrayer = 17941;

                if ((obj.Entry == mennuHealingWard || obj.Entry == taintedStoneskinTotem) &&
                    TargetingHelper.GetCountWithin(StyxWoW.Me.Location, 40, extra => extra.Entry == mennuTheBetrayer) != 0)
                {
                    outgoingunits.Add(obj);
                }
            }
        }

        /// <summary>
        ///   Dungeon specific unit weighting.
        /// </summary>
        /// <param name="units"> The units. </param>
        public override void WeighTargetsFilter(List<Targeting.TargetPriority> units)
        {
            /*
            Information about Tainted Stoneskin Totem
            Name = Tainted Stoneskin Totem
            Wowhead Id = 18177
            Faction = 74 [Naga]
            
            Information about Mennu's Healing Ward
            Name = Mennu's Healing Ward
            Wowhead Id = 20208
            Faction = 74 [Naga]
            
            #1 Boss
            Information about Mennu the Betrayer
            Name = Mennu the Betrayer
            Wowhead Id = 17941
            Faction = 74 [Naga]
            
            #2 Boss
            Information about Rokmar the Crackler
            Name = Rokmar the Crackler
            Wowhead Id = 17991
            Faction = 16 [Monster]
            */

            foreach (Targeting.TargetPriority t in units)
            {
                WoWObject prioObject = t.Object;
                const uint mennuHealingWard = 20208;
                const uint taintedStoneskinTotem = 18177;

                if (prioObject.Entry == mennuHealingWard)
                {
                    t.Score += 400;
                }

                if (prioObject.Entry == taintedStoneskinTotem)
                {
                    t.Score += 100;
                }
                switch (prioObject.Entry)
                {
                    case 17960: //Coilfang Soothsayer
                        if (StyxWoW.Me.IsDps())
                            t.Score += 120;
                        break;
                    case 21126: //Coilfang Scale-Healer
                        if (StyxWoW.Me.IsDps())
                            t.Score += 100;
                        break;
                    case 21128: // Coilfang Scale-Healer
                        if (StyxWoW.Me.IsDps())
                            t.Score += 150;
                        break;
                    case 17957: // Coilfang Champion
                        if (StyxWoW.Me.IsDps())
                            t.Score += 180;
                        break;
                }
            }
        }

        private readonly WoWPoint _quagmirranTankSpot = new WoWPoint(-198.9592, -708.0936, 37.89237);
        private readonly WoWPoint _quagmirranFollowerSpot = new WoWPoint(-166.5996, -729.2723, 37.89237);

        private WoWUnit _quagmirran;

        [EncounterHandler(17942, "Quagmirran", BossRange = 100, Mode = CallBehaviorMode.Proximity)]
        public Composite QuagmirranEncounter()
        {
            return new PrioritySelector(
                ctx => _quagmirran = ctx as WoWUnit,
                new Decorator(
                    ctx => StyxWoW.Me.IsTank(),
                    new PrioritySelector(
                        new Decorator(
                            ctx => _quagmirran.IsCasting,
                            new PrioritySelector(
                                // taunt boss when he does the acid spray thing to make him face tank.
                                new Decorator(
                                    ctx => StyxWoW.Me.Class == WoWClass.Warrior && SpellManager.CanCast("Taunt"),
                                    new Action(ctx => SpellManager.Cast("Taunt"))),
                                new Decorator(
                                    ctx => StyxWoW.Me.Class == WoWClass.DeathKnight && SpellManager.CanCast("Dark Command"),
                                    new Action(ctx => SpellManager.Cast("Dark Command"))),
                                new Decorator(
                                    ctx => StyxWoW.Me.Class == WoWClass.Druid && SpellManager.CanCast("Growl"),
                                    new Action(ctx => SpellManager.Cast("Growl"))),
                                new Decorator(
                                    ctx => StyxWoW.Me.Class == WoWClass.Paladin && SpellManager.CanCast("Hand of Reckoning"),
                                    new Action(ctx => SpellManager.Cast("Hand of Reckoning")))
                                )),
                        new Decorator(
                            ctx => Targeting.Instance.FirstUnit == null,
                            new Action(ctx => ScriptHelpers.MoveTankTo(_quagmirran.Location))),
                        ScriptHelpers.CreateTankUnitAtLocation(() => _quagmirranTankSpot, 17f),
                        ScriptHelpers.CreateTankFaceAwayGroupUnit(40))),
                new Decorator(
                    ctx => StyxWoW.Me.IsFollower(),
                    new PrioritySelector(
                        new Decorator(
                            ctx => Targeting.Instance.FirstUnit == null && ScriptHelpers.Tank.Location.DistanceSqr(_quagmirranTankSpot) > 17*17 && StyxWoW.Me.Location.DistanceSqr(_quagmirranFollowerSpot) > 5*5,
                            new Action(ctx => Navigator.GetRunStatusFromMoveResult(Navigator.MoveTo(_quagmirranFollowerSpot)))),
                        ScriptHelpers.CreateSpreadOutLogic(ctx => true, 20))));
        }
    }
}