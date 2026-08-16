using System;
using System.Collections.Generic;
using System.Linq;
using Styx.Logic;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Styx.CommonBot
{
    public class HealTargeting : Targeting
    {
        private static HealTargeting _instance;

        public new static HealTargeting Instance
        {
            get { return _instance ?? (_instance = new HealTargeting()); }
            set { _instance = value; }
        }

        protected override List<WoWObject> GetInitialObjectList()
        {
            return ObjectManager.CachedUnits.Cast<WoWObject>().ToList();
        }

        public override void Pulse()
        {
            using (StyxWoW.Memory.AcquireFrame())
            {
                base.Pulse();
            }
        }

        protected override void DefaultRemoveTargetsFilter(List<WoWObject> units)
        {
            WoWPoint myLocation = StyxWoW.Me.Location;
            double rangeSqr = Targeting.CollectionRange * Targeting.CollectionRange;

            units.RemoveAll(obj =>
            {
                if (!obj.IsValid)
                    return true;
                WoWUnit unit = obj as WoWUnit;
                return unit == null
                    || (double)myLocation.DistanceSqr(unit.Location) > rangeSqr
                    || unit.IsDead
                    || !unit.CanSelect
                    || !unit.IsFriendly;
            });
        }

        protected override void DefaultIncludeTargetsFilter(List<WoWObject> incomingUnits, HashSet<WoWObject> outgoingUnits)
        {
            var guidSet = new HashSet<ulong>(StyxWoW.Me.PartyMemberGuids);
            guidSet.UnionWith(StyxWoW.Me.RaidMemberGuids);

            foreach (WoWObject obj in incomingUnits)
            {
                WoWUnit unit = obj.ToUnit();
                if (unit == null)
                    continue;

                if (unit.IsPet)
                {
                    WoWPlayer owner = unit.OwnedByRoot as WoWPlayer;
                    if (owner != null && guidSet.Contains(owner.Guid))
                    {
                        outgoingUnits.Add(unit);
                    }
                }
                WoWPlayer player = unit as WoWPlayer;
                if (player != null && guidSet.Contains(player.Guid))
                {
                    outgoingUnits.Add(player);
                }
            }
        }

        protected override void DefaultTargetWeight(List<Targeting.TargetPriority> units)
        {
            var tanks = StyxWoW.Me.RaidMemberInfos
                .Where(m => m.IsMainTank || m.HasRole(WoWPartyMember.GroupRole.Tank))
                .Select(m => m.ToPlayer())
                .Where(p => p != null)
                .ToList();

            WoWPoint location = StyxWoW.Me.Location;

            foreach (Targeting.TargetPriority targetPriority in units)
            {
                targetPriority.Score = 100.0;
                WoWUnit unit = targetPriority.Object.ToUnit();
                float healthPct = (float)unit.HealthPercent;

                WoWPlayer player = unit as WoWPlayer;
                if (player == null)
                {
                    targetPriority.Score -= 50.0;
                }
                else if (tanks.Contains(player) && healthPct < 70f)
                {
                    targetPriority.Score += 20.0;
                }

                float maxRange = 35f + unit.CombatReach + 1f;
                float distance = location.Distance(unit.Location);
                if (distance > maxRange)
                {
                    targetPriority.Score -= Math.Min(Math.Pow((double)(distance - maxRange), 1.6) * 0.5, 100.0);
                }

                targetPriority.Score -= (double)healthPct;
            }
        }
    }
}
