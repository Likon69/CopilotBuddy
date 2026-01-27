// Decompiled with JetBrains decompiler
// Type: Bots.Quest.Actions.ForcedBehaviorExecutor
// Assembly: Honorbuddy, Version=2.0.0.5999, Culture=neutral, PublicKeyToken=50a565ab5c01ae50
// MVID: FB7FEB85-27C0-4D17-B8DE-615FDFDA7752
// Assembly location: C:\Users\Texy6\Desktop\Honorbuddy-cleaned.exe

using Bots.Quest.QuestOrder;
using Styx;
using Styx.Database;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.AreaManagement;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles;
using Styx.Logic.Profiles.Quest;
using Styx.Logic.Questing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using TreeSharp;

#nullable disable
namespace Bots.Quest.Actions;

public class ForcedBehaviorExecutor : Composite
{
    public ForcedBehaviorExecutor(Bots.Quest.QuestOrder.QuestOrder order)
    {
        this.Order = order != null ? order : throw new ArgumentNullException(nameof(order));
    }

    public Bots.Quest.QuestOrder.QuestOrder Order { get; private set; }

    protected override IEnumerable<RunStatus> Execute(object context)
    {
        if (this.Order.CurrentNode == null)
        {
            yield return RunStatus.Failure;
        }
        else
        {
            if (this.Order.CurrentBehavior == null)
            {
                try
                {
                    this.Order.CurrentBehavior = this.CreateForcedBehavior(this.Order.CurrentNode);
                }
                catch (Exception ex)
                {
                    if (this.Order.CurrentNode.Element != null)
                        Logging.Write(Color.Red, "Could not create current in quest bot; exception was thrown, Element: {0}", (object)this.Order.CurrentNode.Element);
                    else
                        Logging.Write(Color.Red, "Could not create current in quest bot; exception was thrown");
                    Logging.WriteException(ex);
                    TreeRoot.Stop();
                }
                if (this.Order.CurrentBehavior == null)
                {
                    Logging.Write("Could not create current in quest bot.");
                    TreeRoot.Stop();
                    yield return RunStatus.Failure;
                    yield break;
                }
                TreeRoot.GoalText = "";
                this.Order.CurrentBehavior.OnStart();
            }
            while (this.Order.CurrentBehavior.IsDone)
            {
                this.Order.CurrentBehavior.Dispose();
                this.Order.CurrentBehavior = (ForcedBehavior)null;
                this.Order.Advance();
                if (this.Order.Nodes.Count > 0)
                {
                    try
                    {
                        this.Order.CurrentBehavior = this.CreateForcedBehavior(this.Order.CurrentNode);
                    }
                    catch (Exception ex)
                    {
                        Logging.Write(Color.Red, "Could not create current in quest bot; exception was thrown");
                        Logging.Write(Color.Red, ex.Message);
                        TreeRoot.Stop();
                    }
                    if (this.Order.CurrentBehavior != null)
                    {
                        TreeRoot.GoalText = "";
                        this.Order.CurrentBehavior.OnStart();
                    }
                    else
                    {
                        Logging.Write("Could not create current in quest bot.");
                        TreeRoot.Stop();
                        yield return RunStatus.Failure;
                        yield break;
                    }
                }
                else
                {
                    yield return RunStatus.Failure;
                    yield break;
                }
            }
            this.Order.CurrentBehavior.OnTick();
            this.Order.CurrentBehavior.Branch.Start(context);
            while (this.Order.CurrentBehavior.Branch.Tick(context) == RunStatus.Running)
                yield return RunStatus.Running;
            this.Order.CurrentBehavior.Branch.Stop(context);
            yield return (RunStatus)((int?)this.Order.CurrentBehavior.Branch.LastStatus ?? 0);
        }
    }

    private ForcedBehavior CreateForcedBehavior(OrderNode orderNode_0)
    {
        switch (orderNode_0.Type)
        {
            case OrderNodeType.Checkpoint:
                return (ForcedBehavior)new ForcedNothing();
            case OrderNodeType.If:
                return (ForcedBehavior)new ForcedIf((IfNode)orderNode_0);
            case OrderNodeType.While:
                return (ForcedBehavior)new ForcedWhile((WhileNode)orderNode_0);
            case OrderNodeType.PickUp:
                return (ForcedBehavior)CreateQuestPickUp((PickUpNode)orderNode_0);
            case OrderNodeType.TurnIn:
                return (ForcedBehavior)CreateQuestTurnIn((TurnInNode)orderNode_0);
            case OrderNodeType.Objective:
                ObjectiveNode objectiveNode_0 = (ObjectiveNode)orderNode_0;
                Bots.Quest.Objectives.QuestObjective objective = CreateQuestObjective(objectiveNode_0);
                if (objective != (Bots.Quest.Objectives.QuestObjective)null)
                    return (ForcedBehavior)new ForcedQuestObjective(objective);
                Logging.Write("Could not create a performable quest objective for objective with ID {0}.", (object)objectiveNode_0.ObjectiveId);
                return (ForcedBehavior)null;
            case OrderNodeType.SetGrindArea:
                SetGrindAreaNode setGrindAreaNode = (SetGrindAreaNode)orderNode_0;
                return (ForcedBehavior)new ForcedSingleton(new System.Action(() =>
                {
                    QuestState.Instance.CurrentGrindArea = setGrindAreaNode.GetArea();
                    StyxWoW.AreaManager.SetArea(QuestState.Instance.CurrentGrindArea);
                }));
            case OrderNodeType.ClearGrindArea:
                return (ForcedBehavior)new ForcedSingleton(new System.Action(() => StyxWoW.AreaManager.SetArea((GrindArea)null)));
            case OrderNodeType.SetMailbox:
                SetMailboxNode setMailboxNode = (SetMailboxNode)orderNode_0;
                return (ForcedBehavior)new ForcedSingleton(new System.Action(() =>
                {
                    QuestState.Instance.CurrentMailboxes = setMailboxNode.Mailboxes;
                    ProfileManager.CurrentProfile.MailboxManager.ForcedMailboxes = setMailboxNode.Mailboxes;
                }));
            case OrderNodeType.ClearMailbox:
                return (ForcedBehavior)new ForcedSingleton(new System.Action(() => ProfileManager.CurrentProfile.MailboxManager.ForcedMailboxes = (List<Mailbox>)null));
            case OrderNodeType.SetVendor:
                SetVendorNode setVendorNode = (SetVendorNode)orderNode_0;
                return (ForcedBehavior)new ForcedSingleton(new System.Action(() =>
                {
                    QuestState.Instance.CurrentVendors = setVendorNode.Vendors;
                    ProfileManager.CurrentProfile.VendorManager.ForcedVendors = setVendorNode.Vendors;
                }));
            case OrderNodeType.ClearVendor:
                return (ForcedBehavior)new ForcedSingleton(new System.Action(() => ProfileManager.CurrentProfile.VendorManager.ForcedVendors = (List<Vendor>)null));
            case OrderNodeType.DisableRepair:
                return (ForcedBehavior)new ForcedSingleton(new System.Action(() => Vendors.RepairDisabled = true));
            case OrderNodeType.EnableRepair:
                return (ForcedBehavior)new ForcedSingleton(new System.Action(() => Vendors.RepairDisabled = false));
            case OrderNodeType.GrindTo:
                return (ForcedBehavior)new ForcedGrindTo((GrindToNode)orderNode_0);
            case OrderNodeType.AbandonQuest:
                AbandonQuestNode abandonQuestNode = (AbandonQuestNode)orderNode_0;
                return (ForcedBehavior)new ForcedSingleton(new System.Action(() =>
                {
                    StyxWoW.Me.QuestLog.AbandonQuestById(abandonQuestNode.QuestId);
                }));
            case OrderNodeType.MoveTo:
                MoveToNode moveToNode = (MoveToNode)orderNode_0;
                return (ForcedBehavior)new ForcedMoveTo(moveToNode.Location, moveToNode.LocationName, moveToNode.Precision, moveToNode.QuestId);
            case OrderNodeType.UseItem:
                UseItemNode useItemNode = (UseItemNode)orderNode_0;
                return (ForcedBehavior)new ForcedUseItem(useItemNode.ItemRetriever, useItemNode.TargetRetriever, useItemNode.ForceUse, useItemNode.QuestId, useItemNode.Location);
            case OrderNodeType.Code:
                return (ForcedBehavior)new ForcedCodeBehavior((CodeNode)orderNode_0);
            default:
                return (ForcedBehavior)null;
        }
    }

    private static ForcedQuestPickUp CreateQuestPickUp(PickUpNode pickUpNode_0)
    {
        WoWPoint giverLocation;
        if (pickUpNode_0.GiverLocation != WoWPoint.Zero)
        {
            giverLocation = pickUpNode_0.GiverLocation;
        }
        else
        {
            if (pickUpNode_0.GiverType.HasValue)
            {
                switch (pickUpNode_0.GiverType.Value)
                {
                    case QuestObjectType.GameObject:
                        Logging.Write("Can not pick up a quest from a gameobject without specifying a location. Please check your profile.");
                        return (ForcedQuestPickUp)null;
                    case QuestObjectType.Npc:
                        NpcResult npcById1 = NpcQueries.GetNpcById(pickUpNode_0.GiverId);
                        if (npcById1 == (NpcResult)null)
                        {
                            Logging.Write("Could not find quest giver NPC with ID {0} in database.", (object)pickUpNode_0.GiverId);
                            return (ForcedQuestPickUp)null;
                        }
                        giverLocation = npcById1.Location;
                        break;
                    case QuestObjectType.Item:
                        if ((WoWObject)StyxWoW.Me.CarriedItems.FirstOrDefault<WoWItem>((Func<WoWItem, bool>)(woWItem_0 => (int)woWItem_0.Entry == (int)pickUpNode_0.GiverId)) == (WoWObject)null)
                        {
                            Logging.Write(Color.Red, "Could not pickup quest from item with id:{0} the item was not found!", (object)pickUpNode_0.GiverId);
                            Logging.Write(Color.Red, "CopilotBuddy Stopped!");
                            TreeRoot.Stop();
                        }
                        giverLocation = WoWPoint.Empty;
                        break;
                    default:
                        return (ForcedQuestPickUp)null;
                }
            }
            else
            {
                NpcResult npcById2 = NpcQueries.GetNpcById(pickUpNode_0.GiverId);
                if (npcById2 == (NpcResult)null)
                {
                    Logging.Write("Could not find quest giver NPC with ID {0} in database.", (object)pickUpNode_0.GiverId);
                    return (ForcedQuestPickUp)null;
                }
                giverLocation = npcById2.Location;
            }
        }
        return new ForcedQuestPickUp(pickUpNode_0.QuestId, pickUpNode_0.QuestName, pickUpNode_0.GiverId, giverLocation, pickUpNode_0.GiverType);
    }

    private static ForcedQuestTurnIn CreateQuestTurnIn(TurnInNode turnInNode_0)
    {
        // Check if quest is in log using ContainsQuest (more reliable than GetQuestById which can fail on cache miss)
        if (!ObjectManager.Me.QuestLog.ContainsQuest(turnInNode_0.QuestId))
        {
            Logging.Write("Can not turn in quest {0} (ID: {1}) because it's not in the quest log.", (object)Utilities.GetObjectString((object)turnInNode_0.QuestName, "(null)"), (object)turnInNode_0.QuestId);
            return (ForcedQuestTurnIn)null;
        }
        
        // Try to get quest object for completion info (may be null if not in cache)
        PlayerQuest questById = ObjectManager.Me.QuestLog.GetQuestById(turnInNode_0.QuestId);
        if (ProfileManager.CurrentProfile != (Profile)null)
        {
            QuestInfo quest = ProfileManager.CurrentProfile.FindQuest(turnInNode_0.QuestId);
            if (quest != null)
            {
                TurnInObjectiveInfo turnIn = quest.FindTurnIn();
                if (turnIn != null)
                    return new ForcedQuestTurnIn(turnInNode_0.QuestId, turnInNode_0.QuestName, turnInNode_0.TurnInId, turnIn.Location);
            }
        }
        
        // If quest object is null (cache miss), use profile or NPC location as fallback
        WoWQuestCompletionInfo completionInfo = new WoWQuestCompletionInfo();
        if (questById != null)
            completionInfo = questById.GetCompletionInfo();
        WoWQuestStep? nullable = new WoWQuestStep?();
        foreach (WoWQuestStep step in completionInfo.Steps.Steps)
        {
            if (step.PoiObjectiveIndex == -1)
            {
                nullable = new WoWQuestStep?(step);
                break;
            }
        }
        if (turnInNode_0.TurnInLocation != WoWPoint.Zero)
            return new ForcedQuestTurnIn(turnInNode_0.QuestId, turnInNode_0.QuestName, turnInNode_0.TurnInId, turnInNode_0.TurnInLocation);
        NpcResult npcById = NpcQueries.GetNpcById(turnInNode_0.TurnInId);
        if (npcById != (NpcResult)null && (!nullable.HasValue || (double)npcById.Location.Distance2DSqr(new WoWPoint(nullable.Value.StepPosition.X, nullable.Value.StepPosition.Y, 0.0f)) <= 400.0))
            return new ForcedQuestTurnIn(turnInNode_0.QuestId, turnInNode_0.QuestName, turnInNode_0.TurnInId, npcById.Location);
        if (!nullable.HasValue)
        {
            Logging.Write("Could not find turn in step. Please specify a turn in override.");
            return (ForcedQuestTurnIn)null;
        }
        var xnaVec = new Tripper.XNAMath.Vector3(nullable.Value.StepPosition.X, nullable.Value.StepPosition.Y, 0.0f);
        if (Navigator.FindHeight(ref xnaVec))
            return new ForcedQuestTurnIn(turnInNode_0.QuestId, turnInNode_0.QuestName, turnInNode_0.TurnInId, new WoWPoint(xnaVec.X, xnaVec.Y, xnaVec.Z));
        Logging.Write("Could not find a height to turn in quest {0}. Consider overriding this quest in your profile.", (object)questById.Name);
        return (ForcedQuestTurnIn)null;
    }

    private static Bots.Quest.Objectives.QuestObjective CreateQuestObjective(ObjectiveNode objectiveNode_0)
    {
        // Check if quest is in log using ContainsQuest
        if (!ObjectManager.Me.QuestLog.ContainsQuest(objectiveNode_0.QuestId))
        {
            Logging.Write("Could not find quest with ID {0} in quest log.", (object)objectiveNode_0.QuestId);
            return (Bots.Quest.Objectives.QuestObjective)null;
        }
        
        // Try to get quest object (may be null if not in cache)
        PlayerQuest questById = ObjectManager.Me.QuestLog.GetQuestById(objectiveNode_0.QuestId);
        if (questById == null)
        {
            Logging.Write("Quest {0} is in log but not in cache - skipping objective creation.", (object)objectiveNode_0.QuestId);
            return (Bots.Quest.Objectives.QuestObjective)null;
        }
        WoWQuestCompletionInfo completionInfo = questById.GetCompletionInfo();
        List<Styx.Logic.Questing.Quest.QuestObjective> objectives = questById.GetObjectives();
        Styx.Logic.Questing.Quest.QuestObjective? nullable = new Styx.Logic.Questing.Quest.QuestObjective?();
        int objectiveIndex = 0;
        for (int index = 0; index < objectives.Count; ++index)
        {
            if ((long)objectives[index].ID == (long)objectiveNode_0.ObjectiveId)
            {
                nullable = new Styx.Logic.Questing.Quest.QuestObjective?(objectives[index]);
                objectiveIndex = index;
                break;
            }
        }
        if (!nullable.HasValue)
        {
            Logging.Write("Could not find objective with ID {0} in quest {1}.", (object)objectiveNode_0.ObjectiveId, (object)questById.Name);
            return (Bots.Quest.Objectives.QuestObjective)null;
        }
        List<WoWQuestStep> list = ((IEnumerable<WoWQuestStep>)completionInfo.Steps.Steps).Where<WoWQuestStep>((Func<WoWQuestStep, bool>)(s => s.PoiObjectiveIndex == objectiveIndex)).ToList<WoWQuestStep>();
        return QuestManager.CreateQuestObjective(nullable.Value, questById, list, (List<Bots.Quest.Objectives.QuestObjective>)null);
    }
}
