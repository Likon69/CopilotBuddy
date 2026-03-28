// Decompiled with JetBrains decompiler
// Type: Bots.Quest.Objectives.CollectItemObjective
// Assembly: Honorbuddy, Version=2.0.0.5999, Culture=neutral, PublicKeyToken=50a565ab5c01ae50
// MVID: FB7FEB85-27C0-4D17-B8DE-615FDFDA7752
// Assembly location: C:\Users\Texy6\Desktop\Honorbuddy-cleaned.exe

using Bots.Grind;
using CommonBehaviors.Actions;
using CommonBehaviors.Decorators;
using Styx;
using Styx.Database;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.AreaManagement;
using Styx.Logic.Combat;
using Styx.Logic.Inventory.Frames.Gossip;
using Styx.Logic.Inventory.Frames.Merchant;
using Styx.Logic.Pathing;
using Styx.Logic.POI;
using Styx.Logic.Profiles;
using Styx.Logic.Profiles.Quest;
using Styx.Logic.Questing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using TreeSharp;

#nullable disable
namespace Bots.Quest.Objectives;

public class CollectItemObjective : QuestObjective
{
    private readonly HashSet<uint> _excludedMobs = new HashSet<uint>();
    private readonly HashSet<uint> _includedMobs = new HashSet<uint>();
    private Composite _behaviorTree;
    private WoWPoint? _cachedObjectiveLocation;
    private readonly CollectItemObjectiveInfo _collectItemInfo;
    private readonly MerchantFrame _merchantFrame = new MerchantFrame();
    private readonly Stopwatch _buyItemTimer = new Stopwatch();
    private readonly GossipFrame _gossipFrame = new GossipFrame();
    private readonly Stopwatch _interactTimer = new Stopwatch();
    private WoWPoint? _cachedVendorLocation;
    private uint? _cachedVendorId;
    private readonly HashSet<uint> _excludedVendors = new HashSet<uint>();
    private readonly HashSet<uint> _includedVendors = new HashSet<uint>();
    private readonly HashSet<uint> _excludedGameObjects = new HashSet<uint>();
    private readonly HashSet<uint> _includedGameObjects = new HashSet<uint>();

    public CollectItemObjective(
        PlayerQuest quest,
        List<WoWQuestStep> questSteps,
        Styx.Logic.Questing.Quest.QuestObjective questObjective,
        List<QuestObjective> prerequisites)
        : base(quest, questSteps, prerequisites)
    {
        Targeting.Instance.WeighTargetsFilter += new WeighTargetsDelegate(this.WeighTargets);
        Targeting.Instance.IncludeTargetsFilter += new IncludeTargetsFilterDelegate(this.IncludeTargets);
        LootTargeting.Instance.IncludeTargetsFilter += new IncludeTargetsFilterDelegate(this.IncludeLootTargets);
        this.Objective = questObjective;
        ProtectedItemsManager.Add((uint)this.Objective.ID);
        if (this.OverridedQuestInfo != null)
            this._collectItemInfo = this.OverridedQuestInfo.FindCollectItem((uint)this.Objective.ID);
    }

    public Styx.Logic.Questing.Quest.QuestObjective Objective { get; private set; }

    public override bool IsCompleted
    {
        get
        {
            return ObjectManager.Me.CarriedItems
                .Where(item => (int)item.Entry == this.Objective.ID)
                .Sum(item => (long)item.StackCount) >= (long)this.Objective.Count;
        }
    }

    public override bool CanComplete => this.DonePrerequisites && this.CanCompleteInternal();

    public override void Dispose()
    {
        ProtectedItemsManager.Remove((uint)this.Objective.ID);
        LootTargeting.Instance.IncludeTargetsFilter -= new IncludeTargetsFilterDelegate(this.IncludeLootTargets);
        Targeting.Instance.IncludeTargetsFilter -= new IncludeTargetsFilterDelegate(this.IncludeTargets);
        Targeting.Instance.WeighTargetsFilter -= new WeighTargetsDelegate(this.WeighTargets);
    }

    public override WoWPoint GetObjectiveLocation()
    {
        List<WoWUnit> validUnits = ObjectManager.GetObjectsOfType<WoWUnit>()
            .Where(unit => this.IsValidMobTarget(unit))
            .ToList();
            
        List<WoWGameObject> validGameObjects = LootTargeting.Instance.LootingList
            .OfType<WoWGameObject>()
            .Where(go => this.IsValidGameObjectTarget(go))
            .ToList();
            
        if (validUnits.Count <= 0 && validGameObjects.Count <= 0)
        {
            if (!this._cachedObjectiveLocation.HasValue)
            {
                if (this._collectItemInfo != null && this._collectItemInfo.OverridedHotspots != null && this._collectItemInfo.OverridedHotspots.Count > 0)
                {
                    this._cachedObjectiveLocation = new WoWPoint?(this._collectItemInfo.OverridedHotspots.FindClosestTo(ObjectManager.Me.Location));
                }
                else
                {
                    WoWQuestStep closestQuestStep = this.GetClosestQuestStep();
                    Vector3 position = new Vector3((float)closestQuestStep.StepPosition.X, (float)closestQuestStep.StepPosition.Y, 0.0f);
                    if (!MeshHeightHelper.FindMeshHeight(ref position))
                    {
                        Logging.Write("Could not find mesh height for quest {0} on step {1}", this.Quest.Name, closestQuestStep.PoiID);
                        this._cachedObjectiveLocation = new WoWPoint?(WoWPoint.Zero);
                    }
                    else
                    {
                        this._cachedObjectiveLocation = new WoWPoint?(MeshHeightHelper.ToWoWPoint(position));
                    }
                }
            }
            return this._cachedObjectiveLocation.Value;
        }
        
        if (validUnits.Count <= 0)
            return validGameObjects[0].Location;
            
        WoWPoint myLocation = ObjectManager.Me.Location;
        WoWPoint closest = validUnits[0].Location;
        float closestDistSqr = myLocation.DistanceSqr(closest);
        
        for (int i = 1; i < validUnits.Count; i++)
        {
            WoWPoint unitLocation = validUnits[i].Location;
            float distSqr = myLocation.DistanceSqr(unitLocation);
            if (distSqr < closestDistSqr)
            {
                closest = unitLocation;
                closestDistSqr = distSqr;
            }
        }
        
        if (validGameObjects.Count <= 0)
            return closest;
            
        WoWPoint goLocation = validGameObjects[0].Location;
        return closestDistSqr >= myLocation.DistanceSqr(goLocation) ? goLocation : closest;
    }

    private bool CanCompleteInternal()
    {
        if (this._collectItemInfo != null && this._collectItemInfo.OverridedHotspots != null && this._collectItemInfo.OverridedHotspots.Count > 0)
            return true;
        if (!this.QuestArea.HotspotsCreated)
            this.QuestArea.CreateHotspots();
        if (this.QuestArea.Hotspots.Count > 0)
            return true;
        // item_loot.db can auto-generate hotspots in CreateBranch
        if (ItemLootQueries.IsAvailable && ItemLootQueries.GetCreaturesForItem((uint)this.Objective.ID).Count > 0)
            return true;
        return false;
    }

    public override Composite CreateBranch()
    {
        int level = StyxWoW.Me.Level;
        if (this._behaviorTree == (Composite)null)
        {
            GrindArea area;
            if (this._collectItemInfo != null && this._collectItemInfo.OverridedHotspots != null && this._collectItemInfo.OverridedHotspots.Count > 0)
            {
                area = new GrindArea(new HotspotManager((IEnumerable<WoWPoint>)this._collectItemInfo.OverridedHotspots))
                {
                    TargetMaxLevel = this._collectItemInfo.TargetMaxLevel > 0 ? this._collectItemInfo.TargetMaxLevel : level + 5,
                    TargetMinLevel = this._collectItemInfo.TargetMinLevel > 0 ? this._collectItemInfo.TargetMinLevel : this.Quest.Level - 5
                };
            }
            else
            {
                this.QuestArea.CreateHotspots();
                this.QuestArea.TargetMaxLevel = level + 5;
                this.QuestArea.TargetMinLevel = this.Quest.Level - 5;
                area = (GrindArea)this.QuestArea;
                // If QuestArea produced no hotspots, auto-generate from item_loot.db + CreatureSpawns.db
                if (area.Hotspots.Count == 0)
                {
                    GrindArea autoArea = TryAutoGenerateHotspots(level);
                    if (autoArea != null)
                        area = autoArea;
                }
            }
            StyxWoW.AreaManager.SetArea(area);
            
            this._behaviorTree = (Composite)new PrioritySelector(new Composite[3]
            {
                (Composite)new Decorator(ctx => this.GetVendorId() != 0U, (Composite)new PrioritySelector((ContextChangeHandler)(ctx => (object)this.FindVendor()), new Composite[2]
                {
                    (Composite)new Decorator(ctx => ctx != null && ((WoWObject)ctx).WithinInteractRange, (Composite)new Sequence(new Composite[3]
                    {
                        (Composite)new TreeSharp.Action(ctx => this.InteractWithVendor(ctx)),
                        (Composite)new TreeSharp.Action(ctx => this.OpenMerchantFrame(ctx)),
                        (Composite)new TreeSharp.Action(ctx => this.BuyItem())
                    })),
                    (Composite)new NavigationAction((GetPointDelegate)(ctx => this.GetVendorLocation(ctx)))
                })),
                (Composite)new Decorator(new CanRunDecoratorDelegate(this.CanUseStartingItem), (Composite)new Decorator(ctx => this.QuestArea.CircledHotspots.Count == 1, (Composite)new PrioritySelector(new Composite[1]
                {
                    (Composite)new Decorator(ctx => (double)this.QuestArea.CurrentHotSpot.Position.Distance(StyxWoW.Me.Location) <= 5.0, (Composite)new TreeSharp.Action(ctx => this.UseStartingItem(null)))
                }))),
                (Composite)new DecoratorIsNotPoiType((IEnumerable<PoiType>)new PoiType[2]
                {
                    PoiType.Loot,
                    PoiType.Skin
                }, (Composite)LevelBot.CreateRoamBehavior())
            });
        }
        return this._behaviorTree;
    }

    private WoWPoint GetVendorLocation(object context)
    {
        return context == null ? this.GetCachedVendorLocation() : ((WoWObject)context).Location;
    }

    private RunStatus OpenMerchantFrame(object context)
    {
        if (this._merchantFrame.IsVisible)
            return RunStatus.Success;
        if (!this._gossipFrame.IsVisible)
            return RunStatus.Failure;
            
        List<GossipEntry> gossipOptions = this._gossipFrame.GossipOptionEntries;
        bool foundVendor = false;
        foreach (GossipEntry entry in gossipOptions)
        {
            if (entry.Type == GossipEntry.GossipEntryType.Vendor)
            {
                this._gossipFrame.SelectGossipOption(entry.Index);
                foundVendor = true;
                break;
            }
        }
        if (!foundVendor)
        {
            Logging.Write("This NPC is not a vendor.");
            return RunStatus.Failure;
        }
        StyxWoW.Sleep(500);
        return RunStatus.Running;
    }

    private RunStatus BuyItem()
    {
        if (!this._merchantFrame.IsVisible)
            return RunStatus.Failure;
            
        if (!this._buyItemTimer.IsRunning)
        {
            MerchantItem item = this._merchantFrame.GetAllMerchantItems()
                .FirstOrDefault(mi => (int)mi.ItemId == this.Objective.ID);
            if (item == null)
            {
                Logging.Write("Vendor does not sell item with ID {0}", this.Objective.ID);
                this._buyItemTimer.Stop();
                return RunStatus.Failure;
            }
            int amount = Math.Max(1, this.Objective.Count / item.Quantity);
            this._merchantFrame.BuyItem(item.Index, amount);
            this._buyItemTimer.Reset();
            this._buyItemTimer.Start();
            return RunStatus.Running;
        }
        
        if (this._buyItemTimer.Elapsed.TotalSeconds > 5.0)
        {
            this._buyItemTimer.Stop();
            return RunStatus.Failure;
        }
        
        if (ObjectManager.Me.CarriedItems.Where(i => (int)i.Entry == this.Objective.ID).Sum(i => (long)i.StackCount) < (long)this.Objective.Count)
            return RunStatus.Running;
            
        this._buyItemTimer.Stop();
        return RunStatus.Success;
    }

    private RunStatus InteractWithVendor(object context)
    {
        WoWUnit vendor = context as WoWUnit;
        if (!StyxWoW.Me.IsMoving && !StyxWoW.Me.Mounted)
        {
            using (new FrameLock())
            {
                if (this._merchantFrame.IsVisible || this._gossipFrame.IsVisible)
                {
                    this._interactTimer.Stop();
                    return RunStatus.Success;
                }
            }
            
            if (!this._interactTimer.IsRunning)
            {
                vendor.Interact();
                this._interactTimer.Reset();
                this._interactTimer.Start();
            }
            else if (this._interactTimer.Elapsed.TotalSeconds > 5.0)
            {
                this._interactTimer.Stop();
                return RunStatus.Failure;
            }
            StyxWoW.Sleep(500);
            return RunStatus.Running;
        }
        
        if (StyxWoW.Me.Mounted)
            Mount.Dismount("Frame");
        WoWMovement.MoveStop();
        StyxWoW.Sleep(250);
        return RunStatus.Running;
    }

    private WoWUnit FindVendor()
    {
        uint vendorId = this.GetVendorId();
        IEnumerable<WoWUnit> vendors = ObjectManager.GetObjectsOfType<WoWUnit>()
            .Where(u => u.IsAlive && u.MyReaction >= WoWUnitReaction.Neutral);
            
        return vendors.FirstOrDefault(u => this.IsKnownVendor(u)) 
            ?? vendors.FirstOrDefault(u => u.Entry == vendorId);
    }

    private WoWPoint GetCachedVendorLocation()
    {
        if (!this._cachedVendorLocation.HasValue)
        {
            if (this._collectItemInfo != null && this._collectItemInfo.OverridedHotspots != null && this._collectItemInfo.OverridedHotspots.Count == 1)
            {
                this._cachedVendorLocation = new WoWPoint?(this._collectItemInfo.OverridedHotspots[0]);
            }
            else
            {
                NpcResult npc = NpcQueries.GetNpcById(this.GetVendorId());
                if (npc != (NpcResult)null)
                {
                    this._cachedVendorLocation = new WoWPoint?(npc.Location);
                }
                else
                {
                    Logging.Write("Can not find NPC selling item with ID {0} in database. Please add a quest override for quest {1}", this.Objective.ID, this.Quest.Name);
                    this._cachedVendorLocation = new WoWPoint?(WoWPoint.Zero);
                }
            }
        }
        return this._cachedVendorLocation.Value;
    }

    private uint GetVendorId()
    {
        if (!this._cachedVendorId.HasValue)
        {
            if (this._collectItemInfo != null && this._collectItemInfo.OverridedCollectFrom != null && this._collectItemInfo.OverridedCollectFrom.Count > 0)
            {
                this._cachedVendorId = new uint?(this._collectItemInfo.OverridedCollectFrom
                    .Where(cf => cf.Type == CollectFromType.Vendor)
                    .Select(cf => cf.ID)
                    .FirstOrDefault());
            }
            else
            {
                this._cachedVendorId = new uint?(0U);
                foreach (KeyValuePair<uint, HashSet<uint>> vendor in DropDatabase.Vendors)
                {
                    if (vendor.Value.Contains((uint)this.Objective.ID))
                        this._cachedVendorId = new uint?(vendor.Key);
                }
            }
        }
        return this._cachedVendorId.Value;
    }

    private bool IsKnownVendor(WoWUnit unit)
    {
        uint entry = unit.Entry;
        if (this._excludedVendors.Contains(entry))
            return false;
        if (!this._includedVendors.Contains(entry))
        {
            if (this._collectItemInfo != null && this._collectItemInfo.OverridedCollectFrom != null && this._collectItemInfo.OverridedCollectFrom.Count > 0)
            {
                if (this._collectItemInfo.OverridedCollectFrom.ContainsVendor(entry))
                {
                    this._includedVendors.Add(entry);
                    return true;
                }
                this._excludedVendors.Add(entry);
                return false;
            }
            if (!DropDatabase.VendorSellsItem(entry, (uint)this.Objective.ID))
            {
                this._excludedVendors.Add(entry);
                return false;
            }
            this._includedVendors.Add(entry);
        }
        return true;
    }

    private bool CanUseStartingItem(object context)
    {
        // In WoW 3.3.5, RelatedItemId is the quest-starting item (StartingItemId in 4.x)
        int startingItemId = (int)this.Quest.InternalInfo.RelatedItemId;
        if (startingItemId == 0)
            return false;
            
        WoWItem item = ObjectManager.GetObjectsOfType<WoWItem>()
            .FirstOrDefault(i => (int)i.Entry == startingItemId);
        if ((WoWObject)item == (WoWObject)null)
            return false;
            
        WoWItem.WoWItemSpell spell = item.GetSpell(0);
        return spell != null && (long)WoWSpell.FromId(spell.Id).CreatesItemId == (long)this.Objective.ID;
    }

    private RunStatus UseStartingItem(object context)
    {
        if (StyxWoW.Me.IsMoving)
        {
            WoWMovement.MoveStop();
            return RunStatus.Running;
        }
        if (StyxWoW.Me.IsCasting)
            return RunStatus.Running;
            
        // In WoW 3.3.5, RelatedItemId is the quest-starting item (StartingItemId in 4.x)
        int startingItemId = (int)this.Quest.InternalInfo.RelatedItemId;
        if (startingItemId == 0)
            return RunStatus.Success;
            
        WoWItem item = ObjectManager.GetObjectsOfType<WoWItem>()
            .FirstOrDefault(i => (int)i.Entry == startingItemId);
        if (!((WoWObject)item != (WoWObject)null))
            return RunStatus.Failure;
            
        Lua.DoString("UseItemByName(\"{0}\")", item.Entry);
        return RunStatus.Running;
    }

    private void WeighTargets(List<Targeting.TargetPriority> targets)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            float bonus = 0.0f;
            if (this.IsValidMobTarget(targets[i].Object.ToUnit()))
                bonus += 40f;
            targets[i].Score += (double)bonus;
        }
    }

    private void IncludeTargets(List<WoWObject> incomingUnits, HashSet<WoWObject> outgoingUnits)
    {
        if (this.IsCompleted || StyxWoW.Me.IsActuallyInCombat)
            return;
        foreach (WoWObject obj in incomingUnits)
        {
            WoWUnit unit = obj.ToUnit();
            if (this.IsValidMobTarget(unit))
                outgoingUnits.Add((WoWObject)unit);
        }
    }

    private void IncludeLootTargets(List<WoWObject> incomingObjects, HashSet<WoWObject> outgoingObjects)
    {
        if (this.IsCompleted || StyxWoW.Me.Combat)
            return;
        for (int i = 0; i < incomingObjects.Count; i++)
        {
            if (incomingObjects[i] is WoWGameObject && this.IsValidGameObjectTarget(incomingObjects[i].ToGameObject()) && !outgoingObjects.Contains(incomingObjects[i]))
                outgoingObjects.Add(incomingObjects[i]);
            if (incomingObjects[i] is WoWUnit && this.IsValidMobTarget(incomingObjects[i].ToUnit()) && !outgoingObjects.Contains(incomingObjects[i]))
                outgoingObjects.Add(incomingObjects[i]);
        }
    }

    private bool IsValidMobTarget(WoWUnit unit)
    {
        uint entry = unit.Entry;
        if (this._excludedMobs.Contains(entry))
            return false;
        if (!this._includedMobs.Contains(entry))
        {
            Styx.WoWInternals.WoWCache.WoWCache.CreatureCacheEntry info;
            if (!unit.GetCachedInfo(out info))
                return false;
            CollectFromCollection collectFrom = this._collectItemInfo?.OverridedCollectFrom;
            if ((collectFrom == null || collectFrom.Count <= 0 || !collectFrom.ContainsMob(entry)) && 
                !DropDatabase.UnitDropsItem(entry, (uint)this.Objective.ID) && 
                !ItemLootQueries.UnitDropsItem(entry, (uint)this.Objective.ID) &&
                !info.QuestItems.Contains((uint)this.Objective.ID))
            {
                this._excludedMobs.Add(entry);
            }
            else
            {
                this._includedMobs.Add(entry);
            }
        }
        return this._includedMobs.Contains(entry);
    }

    private bool IsValidGameObjectTarget(WoWGameObject gameObject)
    {
        uint entry = gameObject.Entry;
        if (this._excludedGameObjects.Contains(entry))
            return false;
        if (!this._includedGameObjects.Contains(entry))
        {
            Styx.WoWInternals.WoWCache.WoWCache.GameObjectCacheEntry info;
            if (!gameObject.GetCachedInfo(out info))
                return false;
            CollectFromCollection collectFrom = this._collectItemInfo?.OverridedCollectFrom;
            if ((collectFrom == null || collectFrom.Count <= 0 || !collectFrom.ContainsGameObject(entry)) && 
                !DropDatabase.GameObjectDropsItem(entry, (uint)this.Objective.ID) && 
                !ItemLootQueries.GameObjectDropsItem(entry, (uint)this.Objective.ID) &&
                !info.QuestItems.Contains(this.Objective.ID))
            {
                this._excludedGameObjects.Add(entry);
            }
            else
            {
                this._includedGameObjects.Add(entry);
            }
        }
        return this._includedGameObjects.Contains(entry) && gameObject.CanLoot;
    }

    public override string ToString()
    {
        return string.Format("[CollectItemObjective ItemID: {0}, Count: {1}]", this.Objective.ID, this.Objective.Count);
    }

    /// <summary>
    /// Auto-generates hotspots from item_loot.db (creature entries) + CreatureSpawns.db (spawn positions).
    /// </summary>
    private GrindArea TryAutoGenerateHotspots(int level)
    {
        uint itemId = (uint)this.Objective.ID;
        uint mapId = StyxWoW.Me.MapId;
        int targetMax = level + 5;
        int targetMin = this.Quest.Level - 5;

        if (!ItemLootQueries.IsAvailable || !CreatureSpawnQueries.IsAvailable)
            return null;

        var creatureEntries = ItemLootQueries.GetCreaturesForItem(itemId);
        if (creatureEntries.Count == 0)
            return null;

        var allHotspots = new List<WoWPoint>();
        foreach (uint creatureEntry in creatureEntries)
            allHotspots.AddRange(CreatureSpawnQueries.GenerateHotspots(creatureEntry, mapId));

        if (allHotspots.Count == 0)
            return null;

        Logging.Write("[CollectItem] Auto-generated {0} hotspots for item {1} from {2} creatures (quest: {3})",
            allHotspots.Count, itemId, creatureEntries.Count, this.Quest.Name);
        return new GrindArea(new HotspotManager((IEnumerable<WoWPoint>)allHotspots))
        {
            TargetMaxLevel = targetMax,
            TargetMinLevel = targetMin
        };
    }
}
