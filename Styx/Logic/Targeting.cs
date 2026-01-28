#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles;
using Styx.Logic.POI;
using Styx.WoWInternals;
using Styx.WoWInternals.World;
using Styx.WoWInternals.WoWObjects;

namespace Styx.Logic
{
    public class Targeting
    {
        private static readonly List<string> _blacklistedMobNames;
        private static Targeting _instance;
        private TargetListUpdateFinishedDelegate targetListUpdateFinishedDelegate_0;
        private IncludeTargetsFilterDelegate includeTargetsFilterDelegate_0;
        private RemoveTargetsFilterDelegate removeTargetsFilterDelegate_0;
        private WeighTargetsDelegate weighTargetsDelegate_0;
        private static readonly HashSet<uint> _blacklistedMobIds;
        private bool _includeWorldPlayers;
        private bool _includeElites;
        private bool _displayTargetingExceptions;
        private List<WoWObject> _objectList;
        private int _maxTargets;
        private static Converter<WoWObject, WoWUnit> converter_0;
        private static Func<WoWObject, bool> func_0;
        private static Func<WoWObject, bool> func_1;
        private static Func<WoWObject, bool> func_2;
        private static Func<WoWObject, TargetPriority> func_3;
        private static Func<TargetPriority, double> func_4;
        private static Func<TargetPriority, WoWObject> func_5;

        protected Targeting()
        {
            this.DisplayTargetingExceptions = true;
            this.ObjectList = new List<WoWObject>();
            this.MaxTargets = 5;
            this.InitializeFilters();
        }

        static Targeting()
        {
            Targeting._blacklistedMobNames = new List<string>();
            HashSet<uint> hashSet = new HashSet<uint>();
            hashSet.Add(28093U);
            hashSet.Add(22979U);
            hashSet.Add(24196U);
            hashSet.Add(13358U);
            hashSet.Add(23837U);
            hashSet.Add(13358U);
            hashSet.Add(9157U);
            hashSet.Add(17407U);
            hashSet.Add(17378U);
            hashSet.Add(17408U);
            hashSet.Add(24222U);
            hashSet.Add(32544U);
            hashSet.Add(32522U);
            hashSet.Add(24879U);
            hashSet.Add(25040U);
            Targeting._blacklistedMobIds = hashSet;
        }

        public static double PullDistance
        {
            get
            {
                return (double)LevelbotSettings.Instance.PullDistance;
            }
        }

        public static double PullDistanceSqr
        {
            get
            {
                double pullDistance = PullDistance;
                return pullDistance * pullDistance;
            }
        }

        public static int GetAggroOnMeWithin(WoWPoint position, float range)
        {
            float rangeSqr = range * range;
            return ObjectManager.GetObjectsOfType<WoWUnit>(true)
                .Where(u => u.Aggro && u.Location.DistanceSqr(position) <= rangeSqr)
                .Count();
        }

        public static int GetAggroWithin(WoWPoint position, float range)
        {
            float rangeSqr = range * range;
            return ObjectManager.GetObjectsOfType<WoWUnit>(true)
                .Where(u => u.Combat && u.Location.DistanceSqr(position) <= rangeSqr)
                .Count();
        }

        public static double CollectionRange
        {
            get
            {
                var area = StyxWoW.AreaManager.CurrentGrindArea;
                if (area != null)
                {
                    double? maxDistance = area.MaxDistance;
                    if (maxDistance != null && maxDistance.Value > 10.0)
                    {
                        return maxDistance.Value;
                    }
                }
                return 100.0;
            }
        }

        public bool IncludeWorldPlayers
        {
            get
            {
                if (Battlegrounds.IsInsideBattleground)
                {
                    return true;
                }
                return this._includeWorldPlayers;
            }
            set
            {
                this._includeWorldPlayers = value;
            }
        }

        public bool IncludeElites
        {
            get { return this._includeElites; }
            set { this._includeElites = value; }
        }

        public bool KillBetweenHotspots
        {
            get
            {
                return StyxSettings.Instance.KillBetweenHotspots;
            }
        }

        public bool DisplayTargetingExceptions
        {
            get { return this._displayTargetingExceptions; }
            set { this._displayTargetingExceptions = value; }
        }

        public static Targeting Instance
        {
            get
            {
                Targeting targeting;
                if ((targeting = Targeting._instance) == null)
                {
                    targeting = (Targeting._instance = new Targeting());
                }
                return targeting;
            }
        }

        public WoWUnit FirstUnit
        {
            get
            {
                if (this.ObjectList.Count == 0)
                {
                    return null;
                }
                return (this.ObjectList[0] as WoWUnit);
            }
        }

        public List<WoWUnit> TargetList
        {
            get
            {
                List<WoWObject> objectList = this.ObjectList;
                if (Targeting.converter_0 == null)
                {
                    Targeting.converter_0 = new Converter<WoWObject, WoWUnit>(ToWoWUnit);
                }
                return objectList.ConvertAll<WoWUnit>(Targeting.converter_0);
            }
        }

        protected List<WoWObject> ObjectList
        {
            get { return this._objectList; }
            private set { this._objectList = value; }
        }

        public int MaxTargets
        {
            get { return this._maxTargets; }
            set { this._maxTargets = value; }
        }

        public event TargetListUpdateFinishedDelegate OnTargetListUpdateFinished
        {
            add
            {
                TargetListUpdateFinishedDelegate targetListUpdateFinishedDelegate = this.targetListUpdateFinishedDelegate_0;
                TargetListUpdateFinishedDelegate targetListUpdateFinishedDelegate2;
                do
                {
                    targetListUpdateFinishedDelegate2 = targetListUpdateFinishedDelegate;
                    TargetListUpdateFinishedDelegate targetListUpdateFinishedDelegate3 = (TargetListUpdateFinishedDelegate)Delegate.Combine(targetListUpdateFinishedDelegate2, value);
                    targetListUpdateFinishedDelegate = Interlocked.CompareExchange<TargetListUpdateFinishedDelegate>(ref this.targetListUpdateFinishedDelegate_0, targetListUpdateFinishedDelegate3, targetListUpdateFinishedDelegate2);
                }
                while (targetListUpdateFinishedDelegate != targetListUpdateFinishedDelegate2);
            }
            remove
            {
                TargetListUpdateFinishedDelegate targetListUpdateFinishedDelegate = this.targetListUpdateFinishedDelegate_0;
                TargetListUpdateFinishedDelegate targetListUpdateFinishedDelegate2;
                do
                {
                    targetListUpdateFinishedDelegate2 = targetListUpdateFinishedDelegate;
                    TargetListUpdateFinishedDelegate targetListUpdateFinishedDelegate3 = (TargetListUpdateFinishedDelegate)Delegate.Remove(targetListUpdateFinishedDelegate2, value);
                    targetListUpdateFinishedDelegate = Interlocked.CompareExchange<TargetListUpdateFinishedDelegate>(ref this.targetListUpdateFinishedDelegate_0, targetListUpdateFinishedDelegate3, targetListUpdateFinishedDelegate2);
                }
                while (targetListUpdateFinishedDelegate != targetListUpdateFinishedDelegate2);
            }
        }

        private void InitializeFilters()
        {
            this.IncludeTargetsFilter += this.DefaultIncludeTargetsFilter;
            this.RemoveTargetsFilter += this.DefaultRemoveTargetsFilter;
            this.WeighTargetsFilter += this.DefaultTargetWeight;
        }

        public event IncludeTargetsFilterDelegate IncludeTargetsFilter
        {
            add
            {
                IncludeTargetsFilterDelegate includeTargetsFilterDelegate = this.includeTargetsFilterDelegate_0;
                IncludeTargetsFilterDelegate includeTargetsFilterDelegate2;
                do
                {
                    includeTargetsFilterDelegate2 = includeTargetsFilterDelegate;
                    IncludeTargetsFilterDelegate includeTargetsFilterDelegate3 = (IncludeTargetsFilterDelegate)Delegate.Combine(includeTargetsFilterDelegate2, value);
                    includeTargetsFilterDelegate = Interlocked.CompareExchange<IncludeTargetsFilterDelegate>(ref this.includeTargetsFilterDelegate_0, includeTargetsFilterDelegate3, includeTargetsFilterDelegate2);
                }
                while (includeTargetsFilterDelegate != includeTargetsFilterDelegate2);
            }
            remove
            {
                IncludeTargetsFilterDelegate includeTargetsFilterDelegate = this.includeTargetsFilterDelegate_0;
                IncludeTargetsFilterDelegate includeTargetsFilterDelegate2;
                do
                {
                    includeTargetsFilterDelegate2 = includeTargetsFilterDelegate;
                    IncludeTargetsFilterDelegate includeTargetsFilterDelegate3 = (IncludeTargetsFilterDelegate)Delegate.Remove(includeTargetsFilterDelegate2, value);
                    includeTargetsFilterDelegate = Interlocked.CompareExchange<IncludeTargetsFilterDelegate>(ref this.includeTargetsFilterDelegate_0, includeTargetsFilterDelegate3, includeTargetsFilterDelegate2);
                }
                while (includeTargetsFilterDelegate != includeTargetsFilterDelegate2);
            }
        }

        public event RemoveTargetsFilterDelegate RemoveTargetsFilter
        {
            add
            {
                RemoveTargetsFilterDelegate removeTargetsFilterDelegate = this.removeTargetsFilterDelegate_0;
                RemoveTargetsFilterDelegate removeTargetsFilterDelegate2;
                do
                {
                    removeTargetsFilterDelegate2 = removeTargetsFilterDelegate;
                    RemoveTargetsFilterDelegate removeTargetsFilterDelegate3 = (RemoveTargetsFilterDelegate)Delegate.Combine(removeTargetsFilterDelegate2, value);
                    removeTargetsFilterDelegate = Interlocked.CompareExchange<RemoveTargetsFilterDelegate>(ref this.removeTargetsFilterDelegate_0, removeTargetsFilterDelegate3, removeTargetsFilterDelegate2);
                }
                while (removeTargetsFilterDelegate != removeTargetsFilterDelegate2);
            }
            remove
            {
                RemoveTargetsFilterDelegate removeTargetsFilterDelegate = this.removeTargetsFilterDelegate_0;
                RemoveTargetsFilterDelegate removeTargetsFilterDelegate2;
                do
                {
                    removeTargetsFilterDelegate2 = removeTargetsFilterDelegate;
                    RemoveTargetsFilterDelegate removeTargetsFilterDelegate3 = (RemoveTargetsFilterDelegate)Delegate.Remove(removeTargetsFilterDelegate2, value);
                    removeTargetsFilterDelegate = Interlocked.CompareExchange<RemoveTargetsFilterDelegate>(ref this.removeTargetsFilterDelegate_0, removeTargetsFilterDelegate3, removeTargetsFilterDelegate2);
                }
                while (removeTargetsFilterDelegate != removeTargetsFilterDelegate2);
            }
        }

        public event WeighTargetsDelegate WeighTargetsFilter
        {
            add
            {
                WeighTargetsDelegate weighTargetsDelegate = this.weighTargetsDelegate_0;
                WeighTargetsDelegate weighTargetsDelegate2;
                do
                {
                    weighTargetsDelegate2 = weighTargetsDelegate;
                    WeighTargetsDelegate weighTargetsDelegate3 = (WeighTargetsDelegate)Delegate.Combine(weighTargetsDelegate2, value);
                    weighTargetsDelegate = Interlocked.CompareExchange<WeighTargetsDelegate>(ref this.weighTargetsDelegate_0, weighTargetsDelegate3, weighTargetsDelegate2);
                }
                while (weighTargetsDelegate != weighTargetsDelegate2);
            }
            remove
            {
                WeighTargetsDelegate weighTargetsDelegate = this.weighTargetsDelegate_0;
                WeighTargetsDelegate weighTargetsDelegate2;
                do
                {
                    weighTargetsDelegate2 = weighTargetsDelegate;
                    WeighTargetsDelegate weighTargetsDelegate3 = (WeighTargetsDelegate)Delegate.Remove(weighTargetsDelegate2, value);
                    weighTargetsDelegate = Interlocked.CompareExchange<WeighTargetsDelegate>(ref this.weighTargetsDelegate_0, weighTargetsDelegate3, weighTargetsDelegate2);
                }
                while (weighTargetsDelegate != weighTargetsDelegate2);
            }
        }

        public void Clear()
        {
            if (this.ObjectList != null)
            {
                this.ObjectList.Clear();
            }
        }

        protected virtual List<WoWObject> GetInitialObjectList()
        {
            if (Battlegrounds.IsInsideBattleground)
            {
                IEnumerable<WoWObject> objectList = ObjectManager.ObjectList;
                if (Targeting.func_0 == null)
                {
                    Targeting.func_0 = new Func<WoWObject, bool>(IsUnitOrPlayer);
                }
                return objectList.Where(Targeting.func_0).ToList<WoWObject>();
            }
            else if (!this.IncludeWorldPlayers)
            {
                IEnumerable<WoWObject> objectList2 = ObjectManager.ObjectList;
                if (Targeting.func_2 == null)
                {
                    Targeting.func_2 = new Func<WoWObject, bool>(IsUnitNotPlayer);
                }
                return objectList2.Where(Targeting.func_2).ToList<WoWObject>();
            }
            else
            {
                IEnumerable<WoWObject> objectList3 = ObjectManager.ObjectList;
                if (Targeting.func_1 == null)
                {
                    Targeting.func_1 = new Func<WoWObject, bool>(IsUnit);
                }
                return objectList3.Where(Targeting.func_1).ToList<WoWObject>();
            }
        }

        internal virtual void Update()
        {
            try
            {
                if (!(StyxWoW.Me == null))
                {
                    if (StyxWoW.IsInGame)
                    {
                        List<WoWObject> initialObjectList = this.GetInitialObjectList();
                        Delegate e = this.removeTargetsFilterDelegate_0;
                        object[] array = new object[] { initialObjectList };
                        this.InvokeFilterDelegate(e, array);
                        HashSet<WoWObject> hashSet = new HashSet<WoWObject>();
                        Delegate e2 = this.includeTargetsFilterDelegate_0;
                        object[] array2 = new object[] { initialObjectList, hashSet };
                        this.InvokeFilterDelegate(e2, array2);
                        IEnumerable<WoWObject> source = hashSet;
                        if (Targeting.func_3 == null)
                        {
                            Targeting.func_3 = new Func<WoWObject, TargetPriority>(CreateTargetPriority);
                        }
                        List<TargetPriority> list = source.Select(Targeting.func_3).ToList<TargetPriority>();
                        Delegate e3 = this.weighTargetsDelegate_0;
                        object[] array3 = new object[] { list };
                        this.InvokeFilterDelegate(e3, array3);
                        IEnumerable<TargetPriority> source2 = list;
                        if (Targeting.func_4 == null)
                        {
                            Targeting.func_4 = new Func<TargetPriority, double>(GetScore);
                        }
                        list = source2.OrderByDescending(Targeting.func_4).Take(this.MaxTargets).ToList<TargetPriority>();
                        IEnumerable<TargetPriority> source3 = list;
                        if (Targeting.func_5 == null)
                        {
                            Targeting.func_5 = new Func<TargetPriority, WoWObject>(GetObject);
                        }
                        this.ObjectList = source3.Select(Targeting.func_5).ToList<WoWObject>();
                        Targeting._blacklistedMobNames.Clear();
                        foreach (TargetPriority targetPriority in list)
                        {
                            Targeting._blacklistedMobNames.Add(string.Format("{0} Dist: {1}", targetPriority.Object.Name, Math.Ceiling(targetPriority.Object.Distance)));
                        }
                        if (this.targetListUpdateFinishedDelegate_0 != null)
                        {
                            this.targetListUpdateFinishedDelegate_0(Targeting._blacklistedMobNames);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.DisplayTargetingExceptions)
                {
                    Logging.WriteException(ex);
                }
            }
        }

        public virtual void Pulse()
        {
            Update();
        }

        private void InvokeFilterDelegate(Delegate e, params object[] args)
        {
            if (e != null)
            {
                foreach (Delegate @delegate in e.GetInvocationList())
                {
                    try
                    {
                        @delegate.DynamicInvoke(args);
                    }
                    catch (Exception ex)
                    {
                        if (this.DisplayTargetingExceptions)
                        {
                            Logging.WriteException(ex);
                        }
                    }
                }
            }
        }

        protected virtual void DefaultRemoveTargetsFilter(List<WoWObject> units)
        {
            Blacklist.Flush();
            bool flag = StyxWoW.Me.IsInParty || StyxWoW.Me.IsInRaid;
            double num = Targeting.CollectionRange * Targeting.CollectionRange;
            int num2 = 0;
            int num3 = 1000;
            bool combat = StyxWoW.Me.Combat;
            var currentGrindArea = StyxWoW.AreaManager.CurrentGrindArea;
            if (currentGrindArea != null && !combat)
            {
                num2 = currentGrindArea.TargetMinLevel;
                num3 = currentGrindArea.TargetMaxLevel;
            }
            for (int i = units.Count - 1; i >= 0; i--)
            {
                WoWObject woWObject = units[i];
                if (!(woWObject is WoWUnit))
                {
                    units.RemoveAt(i);
                }
                else
                {
                    WoWUnit woWUnit = (WoWUnit)woWObject;
                    if ((woWUnit is WoWPlayer || !woWUnit.Combat || !woWUnit.IsTargetingMeOrPet) && 
                        (Targeting._blacklistedMobIds.Contains(woWUnit.Entry) || 
                         woWUnit.Level < num2 || 
                         woWUnit.Level > num3 || 
                         Blacklist.Contains(woWUnit.Guid, false) || 
                         woWUnit.Dead || 
                         woWUnit.DistanceSqr > num || 
                         woWUnit.OnTaxi || 
                         woWUnit.CreatureType == WoWCreatureType.Critter || 
                         woWUnit.IsFlightMaster || 
                         woWUnit.IsFlying || 
                         !woWUnit.Attackable || 
                         woWUnit.IsFriendly || 
                         (woWUnit.TaggedByOther && !flag)))
                    {
                        units.RemoveAt(i);
                    }
                }
            }
        }

        protected virtual void DefaultIncludeTargetsFilter(List<WoWObject> incomingUnits, HashSet<WoWObject> outgoingUnits)
        {
            bool flag = StyxWoW.Me.Combat || StyxWoW.Me.PetInCombat;
            foreach (WoWObject woWObject in incomingUnits)
            {
                WoWUnit woWUnit = woWObject.ToUnit();
                if (flag && (woWUnit.Aggro || woWUnit.PetAggro || (woWUnit.Fleeing && woWUnit.TaggedByMe)))
                {
                    outgoingUnits.Add(woWUnit);
                }
            }
            if (Styx.Logic.POI.BotPoi.Current.Type == Styx.Logic.POI.PoiType.Kill)
            {
                var poiObj = Styx.Logic.POI.BotPoi.Current.AsObject;
                if (poiObj != null && !outgoingUnits.Contains(poiObj))
                {
                    outgoingUnits.Add(poiObj);
                }
            }
        }

        protected virtual void DefaultTargetWeight(List<TargetPriority> units)
        {
            bool flag = StyxWoW.Me.PetInCombat || StyxWoW.Me.Combat;
            ulong currentTargetGuid = StyxWoW.Me.CurrentTargetGuid;
            bool flag2 = currentTargetGuid != 0UL;
            LocalPlayer me = StyxWoW.Me;
            Profile currentProfile = ProfileManager.CurrentProfile;

            // Build trace lines for LOS check
            WorldLine[] traceLines = new WorldLine[units.Count];
            WoWPoint traceLinePos = me.GetTraceLinePos();
            for (int i = units.Count - 1; i >= 0; i--)
            {
                traceLines[i] = new WorldLine(traceLinePos, units[i].Object.ToUnit().GetTraceLinePos());
            }

            // Perform mass trace line check
            bool[] losResults;
            GameWorld.MassTraceLine(traceLines, GameWorld.TraceLineHitFlags.All, out losResults);

            int j = units.Count - 1;
            while (j >= 0)
            {
                WoWUnit woWUnit = units[j].Object.ToUnit();
                double num = 200.0 - 2.0 * woWUnit.Distance;
                double distance = woWUnit.Distance;
                ulong guid = woWUnit.Guid;
                ulong currentTargetGuid2 = woWUnit.CurrentTargetGuid;
                if (flag)
                {
                    if (flag2 && me.CurrentTarget != null && me.CurrentTarget.MaxMana > 1 && me.CurrentTarget.ManaPercent > 0.0)
                    {
                        num += woWUnit.ManaPercent;
                    }
                    num -= woWUnit.HealthPercent;
                }
                else
                {
                    if (currentProfile != null && !currentProfile.TargetElites && woWUnit.Elite)
                    {
                        if (distance > 20.0)
                        {
                            units.RemoveAt(j);
                            j--;
                            continue;
                        }
                        num -= 1000.0;
                    }
                    if (Blacklist.Contains(guid))
                    {
                        if (distance > 20.0)
                        {
                            units.RemoveAt(j);
                            j--;
                            continue;
                        }
                        num -= 1000.0;
                    }
                    if (woWUnit.MyReaction <= WoWUnitReaction.Neutral && distance < 30.0)
                    {
                        num += (30.0 - distance) * (double)(WoWUnitReaction.Friendly - woWUnit.MyReaction);
                    }

                    // Check current target or POI
                    if (currentTargetGuid2 == guid || (BotPoi.Current.Type == PoiType.Kill && BotPoi.Current.Guid == woWUnit.Guid))
                    {
                        num += 150.0;
                    }

                    if (woWUnit.IsPet)
                    {
                        num -= 50.0;
                    }
                    float myAggroRange = woWUnit.MyAggroRange;
                    if (myAggroRange != 0f && distance < (double)(myAggroRange + 5f))
                    {
                        num += 100.0;
                    }

                    // Adjust score based on line of sight
                    if (!losResults[j])
                    {
                        num += 25.0; // In line of sight - bonus
                    }
                    else
                    {
                        num -= 25.0; // Not in line of sight - penalty
                    }

                    // Penalty for nearby units
                    num -= (double)(5 * CountUnitsNearLocation(woWUnit.Location, 15f));
                }
                units[j].Score += num;
                j--;
            }
        }

        /// <summary>
        /// Counts units near a location.
        /// </summary>
        private static int CountUnitsNearLocation(WoWPoint location, float radius)
        {
            int count = 0;
            foreach (WoWObject obj in ObjectManager.ObjectList)
            {
                if (obj is WoWUnit unit && unit.Location.Distance(location) <= radius)
                {
                    count++;
                }
            }
            return count;
        }

        private static WoWUnit ToWoWUnit(WoWObject o)
        {
            return o.ToUnit();
        }

        private static bool IsPlayer(WoWObject o)
        {
            return (o is WoWPlayer);
        }

        private static bool IsUnit(WoWObject o)
        {
            return (o is WoWUnit);
        }

        private static bool IsUnitOrPlayer(WoWObject o)
        {
            return (o is WoWUnit) || (o is WoWPlayer);
        }

        private static bool IsUnitNotPlayer(WoWObject o)
        {
            if (o is WoWUnit)
            {
                return !(o is WoWPlayer);
            }
            return false;
        }

        private static TargetPriority CreateTargetPriority(WoWObject unit)
        {
            TargetPriority targetPriority = new TargetPriority();
            targetPriority.Object = unit;
            targetPriority.Score = 0.0;
            return targetPriority;
        }

        private static double GetScore(TargetPriority p)
        {
            return p.Score;
        }

        private static WoWObject GetObject(TargetPriority p)
        {
            return p.Object;
        }

        public sealed class TargetPriority
        {
            public WoWObject Object;
            public double Score;
        }
    }
}
