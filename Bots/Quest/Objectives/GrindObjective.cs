// Decompiled with JetBrains decompiler
// Type: Bots.Quest.Objectives.GrindObjective
// Assembly: Honorbuddy, Version=2.0.0.5999, Culture=neutral, PublicKeyToken=50a565ab5c01ae50
// MVID: FB7FEB85-27C0-4D17-B8DE-615FDFDA7752
// Assembly location: C:\Users\Texy6\Desktop\Honorbuddy-cleaned.exe

using Bots.Grind;
using CommonBehaviors.Decorators;
using Styx;
using Styx.Database;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.AreaManagement;
using Styx.Logic.Pathing;
using Styx.Logic.POI;
using Styx.Logic.Profiles.Quest;
using Styx.Logic.Questing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWCache;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TreeSharp;

#nullable disable
namespace Bots.Quest.Objectives;

public class GrindObjective : QuestObjective
{
    private readonly KillMobObjectiveInfo killMobInfo;
    private WoWPoint? cachedObjectiveLocation;
    private Composite behaviorTree;

    public GrindObjective(
        PlayerQuest quest,
        List<WoWQuestStep> questSteps,
        Styx.Logic.Questing.Quest.QuestObjective questObjective,
        List<QuestObjective> prerequisites)
        : base(quest, questSteps, prerequisites)
    {
        this.Objective = questObjective;
        if (this.OverridedQuestInfo != null)
        {
            this.killMobInfo = this.OverridedQuestInfo.FindKillMob((uint)this.Objective.ID);
            if (this.killMobInfo == null)
            {
                foreach (KillMobObjectiveInfo mobObjectiveInfo in this.OverridedQuestInfo.Objectives.OfType<KillMobObjectiveInfo>().Where<KillMobObjectiveInfo>((Func<KillMobObjectiveInfo, bool>)(info => info.Type == ObjectiveType.KillMob)))
                {
                    WoWCache.InfoBlock infoBlockById = StyxWoW.Cache[CacheDb.Creature].GetInfoBlockById(mobObjectiveInfo.MobID);
                    if (infoBlockById != null)
                    {
                        WoWCache.CreatureCacheEntry creature = infoBlockById.Creature;
                        if ((long)creature.GroupID == (long)this.Objective.ID || (long)creature.GroupID2 == (long)this.Objective.ID)
                        {
                            this.killMobInfo = mobObjectiveInfo;
                            break;
                        }
                    }
                }
            }
        }
        Targeting.Instance.IncludeTargetsFilter += new IncludeTargetsFilterDelegate(this.FilterObjectsForTargeting);
    }

    public Styx.Logic.Questing.Quest.QuestObjective Objective { get; private set; }

    public override void Dispose()
    {
        Targeting.Instance.IncludeTargetsFilter -= new IncludeTargetsFilterDelegate(this.FilterObjectsForTargeting);
    }

    private void FilterObjectsForTargeting(List<WoWObject> objects, HashSet<WoWObject> validTargets)
    {
        if (this.IsCompleted || StyxWoW.Me.IsActuallyInCombat)
            return;
        foreach (WoWObject obj in objects)
        {
            if (obj is WoWUnit && this.IsMobObjective(obj.ToUnit()))
                validTargets.Add(obj);
        }
    }

    public override bool IsCompleted
    {
        get
        {
            WoWDescriptorQuest data;
            return this.Quest.GetData(out data) && (int)data.ObjectivesDone[this.Objective.Index] >= this.Objective.Count;
        }
    }

    public override bool CanComplete => this.DonePrerequisites && this.HasValidHotspots();

    public override Composite CreateBranch()
    {
        int level = StyxWoW.Me.Level;
        if (this.behaviorTree == (Composite)null)
        {
            GrindArea area = null;
            
            // Priority 1: Profile-defined hotspots (OverridedHotspots from <Quest> tag)
            if (this.killMobInfo != null && this.killMobInfo.OverridedHotspots != null && this.killMobInfo.OverridedHotspots.Count > 0)
            {
                Logging.WriteDebug("[GrindObjective] Using profile hotspots ({0} points) for mob {1}", 
                    this.killMobInfo.OverridedHotspots.Count, this.killMobInfo.MobID);
                    
                area = new GrindArea(new HotspotManager((IEnumerable<WoWPoint>)this.killMobInfo.OverridedHotspots))
                {
                    TargetMaxLevel = this.killMobInfo.TargetMaxLevel > 0 ? this.killMobInfo.TargetMaxLevel : level + 5,
                    TargetMinLevel = this.killMobInfo.TargetMinLevel > 0 ? this.killMobInfo.TargetMinLevel : this.Quest.Level - 5
                };
            }
            // Priority 2: Quest area from client DB (only if it actually creates hotspots)
            else if (this.QuestArea != null && this.QuestArea.AreaDefinitions.Count > 0 && 
                     this.QuestArea.AreaDefinitions.Any(a => a.Count > 0))
            {
                Logging.WriteDebug("[GrindObjective] Trying client quest area for quest {0}", this.Quest.Name);
                this.QuestArea.CreateHotspots();
                
                if (this.QuestArea.Hotspots.Count > 0)
                {
                    Logging.WriteDebug("[GrindObjective] Client quest area created {0} hotspots", this.QuestArea.Hotspots.Count);
                    this.QuestArea.TargetMaxLevel = level + 5;
                    this.QuestArea.TargetMinLevel = this.Quest.Level - 5;
                    area = (GrindArea)this.QuestArea;
                }
                else
                {
                    Logging.WriteDiagnostic("[GrindObjective] Client quest area created 0 hotspots, trying DB...");
                }
            }
            
            // Priority 3: Auto-generate from CreatureSpawns.db (works without <Quest> tag)
            if (area == null && CreatureSpawnQueries.IsAvailable)
            {
                // Use Objective.ID directly - works even without <Quest> tag in profile
                var mobId = (uint)this.Objective.ID;
                var mapId = StyxWoW.Me.MapId;
                var autoHotspots = CreatureSpawnQueries.GenerateHotspots(mobId, mapId);
                
                if (autoHotspots.Count > 0)
                {
                    Logging.WriteDiagnostic("[GrindObjective] Auto-generated {0} hotspots from DB for mob {1} on map {2}", 
                        autoHotspots.Count, mobId, mapId);
                    
                    int targetMax = (this.killMobInfo != null && this.killMobInfo.TargetMaxLevel > 0) 
                        ? this.killMobInfo.TargetMaxLevel : level + 5;
                    int targetMin = (this.killMobInfo != null && this.killMobInfo.TargetMinLevel > 0) 
                        ? this.killMobInfo.TargetMinLevel : this.Quest.Level - 5;
                        
                    area = new GrindArea(new HotspotManager((IEnumerable<WoWPoint>)autoHotspots))
                    {
                        TargetMaxLevel = targetMax,
                        TargetMinLevel = targetMin
                    };
                }
                else
                {
                    Logging.Write("[GrindObjective] No spawns in DB for mob {0} on map {1}", mobId, mapId);
                }
            }
            
            // Final fallback: empty quest area
            if (area == null)
            {
                Logging.Write("[GrindObjective] No hotspots available - using empty quest area");
                this.QuestArea.CreateHotspots();
                this.QuestArea.TargetMaxLevel = level + 5;
                this.QuestArea.TargetMinLevel = this.Quest.Level - 5;
                area = (GrindArea)this.QuestArea;
            }
            
            StyxWoW.AreaManager.SetArea(area);
            this.behaviorTree = (Composite)new DecoratorIsNotPoiType((IEnumerable<PoiType>)new PoiType[2]
            {
                PoiType.Loot,
                PoiType.Skin
            }, (Composite)LevelBot.CreateRoamBehavior());
        }
        return this.behaviorTree;
    }

    public override WoWPoint GetObjectiveLocation()
    {
        List<WoWUnit> list = ObjectManager.GetObjectsOfType<WoWUnit>().Where<WoWUnit>((Func<WoWUnit, bool>)(unit => this.IsMobObjective(unit))).ToList<WoWUnit>();
        if (list.Count <= 0)
        {
            if (!this.cachedObjectiveLocation.HasValue)
            {
                if (this.killMobInfo != null && this.killMobInfo.OverridedHotspots != null && this.killMobInfo.OverridedHotspots.Count > 0)
                {
                    this.cachedObjectiveLocation = new WoWPoint?(this.killMobInfo.OverridedHotspots.FindClosestTo(ObjectManager.Me.Location));
                }
                else
                {
                    WoWQuestStep closestQuestStep = this.GetClosestQuestStep();
                    var xnaVec = new Tripper.XNAMath.Vector3((float)closestQuestStep.StepPosition.X, (float)closestQuestStep.StepPosition.Y, 0.0f);
                    if (!Navigator.FindHeight(ref xnaVec))
                    {
                        Logging.Write("GrindObjective: Could not find mesh height for quest {0} on step {1}", (object)this.Quest.Name, (object)closestQuestStep.PoiID);
                        this.cachedObjectiveLocation = new WoWPoint?(WoWPoint.Zero);
                    }
                    else
                        this.cachedObjectiveLocation = new WoWPoint?(new WoWPoint(xnaVec.X, xnaVec.Y, xnaVec.Z));
                }
            }
            return this.cachedObjectiveLocation.Value;
        }
        WoWPoint location1 = ObjectManager.Me.Location;
        WoWPoint objectiveLocation = list[0].Location;
        float closestDistanceSqr = objectiveLocation.DistanceSqr(location1);
        for (int index = 1; index < list.Count; ++index)
        {
            WoWPoint location2 = list[index].Location;
            float currentDistanceSqr = location2.DistanceSqr(location1);
            if ((double)currentDistanceSqr < (double)closestDistanceSqr)
            {
                closestDistanceSqr = currentDistanceSqr;
                objectiveLocation = location2;
            }
        }
        return objectiveLocation;
    }

    private bool HasValidHotspots()
    {
        if (this.killMobInfo != null && this.killMobInfo.OverridedHotspots != null && this.killMobInfo.OverridedHotspots.Count > 0)
            return true;
        if (!this.QuestArea.HotspotsCreated)
            this.QuestArea.CreateHotspots();
        return this.QuestArea.Hotspots.Count > 0;
    }

    private bool IsMobObjective(WoWUnit unit)
    {
        if (unit is WoWPlayer)
            return false;
        if ((long)unit.Entry == (long)this.Objective.ID)
            return true;
        WoWCache.CreatureCacheEntry info;
        if (!unit.GetCachedInfo(out info))
            return false;
        return (long)info.GroupID == (long)this.Objective.ID || (long)info.GroupID2 == (long)this.Objective.ID;
    }

    public override string ToString()
    {
        return string.Format("[GrindObjective MobID: {0}, Count: {1}]", (object)this.Objective.ID, (object)this.Objective.Count);
    }
}
