// Decompiled with JetBrains decompiler
// Type: Bots.Quest.QuestOrder.ForcedQuestPickUp
// Assembly: Honorbuddy, Version=2.0.0.5999, Culture=neutral, PublicKeyToken=50a565ab5c01ae50
// MVID: FB7FEB85-27C0-4D17-B8DE-615FDFDA7752
// Assembly location: C:\Users\Texy6\Desktop\Honorbuddy-cleaned.exe

using CommonBehaviors.Actions;
using CommonBehaviors.Decorators;
using Styx.Helpers;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Inventory.Frames;
using Styx.Logic.Inventory.Frames.Gossip;
using Styx.Logic.Inventory.Frames.Quest;
using Styx.Logic.Pathing;
using Styx.Logic.POI;
using Styx.Logic.Profiles.Quest;
using Styx.Logic.Questing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using TreeSharp;

#nullable disable
namespace Bots.Quest.QuestOrder;

public class ForcedQuestPickUp : ForcedBehavior
{
    private static readonly Frame frame_0 = new Frame("QuestTitleButton1");
    private static readonly Frame frame_1 = new Frame("QuestFrameCompleteQuestButton");
    private int int_0 = -1;

    public ForcedQuestPickUp(
        uint questId,
        string questName,
        uint giverId,
        WoWPoint giverLocation,
        QuestObjectType? giverType)
    {
        this.QuestId = questId;
        this.QuestName = questName;
        this.GiverId = giverId;
        this.GiverLocation = giverLocation;
        this.GiverType = giverType;
    }

    public override bool IsDone
    {
        get
        {
            // Skip if quest is already turned in (completed history)
            var completedQuests = ObjectManager.Me.QuestLog.GetCompletedQuests();
            if (completedQuests != null && completedQuests.Contains(this.QuestId))
                return true;

            // Skip if quest is already in quest log (either complete or in-progress)
            // This allows the profile to continue with quest objectives or turn-in
            if (ObjectManager.Me.QuestLog.ContainsQuest(this.QuestId))
                return true;

            return false;
        }
    }

    public uint QuestId { get; private set; }

    public string QuestName { get; private set; }

    public uint GiverId { get; private set; }

    public WoWPoint GiverLocation { get; private set; }

    public QuestObjectType? GiverType { get; private set; }

    public override void OnStart()
    {
        if (ObjectManager.Me.QuestLog.GetAllQuests().Count >= 25)
        {
            Logging.Write(Color.Red, "You do not have any space in your quest log.");
            Logging.Write(Color.Red, "CopilotBuddy stopped!");
            TreeRoot.Stop();
        }
        TreeRoot.GoalText = this.method_0();
    }

    private string method_0()
    {
        Styx.Logic.Questing.Quest quest = Styx.Logic.Questing.Quest.FromId(this.QuestId);
        QuestObjectType? giverType = this.GiverType;
        if ((giverType.GetValueOrDefault() != QuestObjectType.Item ? 0 : (giverType.HasValue ? 1 : 0)) != 0)
        {
            WoWItem woWitem = ObjectManager.Me.CarriedItems.FirstOrDefault<WoWItem>((Func<WoWItem, bool>)(woWItem_0 => (int)woWItem_0.Entry == (int)this.GiverId));
            if ((WoWObject)woWitem != (WoWObject)null && !string.IsNullOrEmpty(this.QuestName))
                return string.Format("Picking up quest {0} from item {1}", (object)this.QuestName, (object)woWitem.Name);
        }
        if (quest != null)
        {
            string str = string.Format("Picking up {0}", (object)quest.Name);
            Logging.WriteDebug("{0} : {1}", (object)str, (object)this.QuestId);
            return str;
        }
        if (string.IsNullOrEmpty(this.QuestName))
            return string.Format("Picking up quest with ID {0}", (object)this.QuestId);
        string str1 = string.Format("Picking up {0}", (object)this.QuestName);
        Logging.WriteDebug("{0} : {1}", (object)str1, (object)this.QuestId);
        return str1;
    }

    private static LocalPlayer Me => ObjectManager.Me;

    protected override Composite CreateBehavior()
    {
        return (Composite)new DecoratorIsNotPoiType((IEnumerable<PoiType>)new PoiType[3]
        {
            PoiType.Harvest,
            PoiType.Skin,
            PoiType.Loot
        }, (Composite)new PrioritySelector(new Composite[4]
        {
            (Composite)new Decorator(new CanRunDecoratorDelegate(this.method_1), (Composite)new ActionSetPoi(true, (RetrieveBotPoiDelegate)(object_0 => new BotPoi(new PickUpNode(this.GiverLocation, this.GiverId, (string)null, this.GiverType, this.QuestId, this.QuestName))))),
            (Composite)new Decorator((CanRunDecoratorDelegate)(object_0 =>
            {
                QuestObjectType? giverType = this.GiverType;
                return giverType.GetValueOrDefault() == QuestObjectType.Item && giverType.HasValue;
            }), (Composite)new Sequence((ContextChangeHandler)(object_0 => (object)ObjectManager.GetObjectsOfType<WoWItem>().FirstOrDefault<WoWItem>((Func<WoWItem, bool>)(woWItem_0 => (int)woWItem_0.Entry == (int)this.GiverId))), new Composite[1]
            {
                (Composite)new DecoratorContinue((CanRunDecoratorDelegate)(object_0 => object_0 != null && object_0 is WoWItem), (Composite)new Sequence(new Composite[3]
                {
                    (Composite)new TreeSharp.Action((ActionSucceedDelegate)(object_0 => ((WoWItem)object_0).UseContainerItem())),
                    (Composite)new WaitContinue(5, new CanRunDecoratorDelegate(this.method_5), (Composite)new TreeSharp.Action((ActionSucceedDelegate)(object_0 => QuestFrame.Instance.AcceptQuest()))),
                    (Composite)new WaitContinue(2, (CanRunDecoratorDelegate)(object_0 => false), (Composite)new ActionAlwaysSucceed())
                }))
            })),
            (Composite)new Decorator((CanRunDecoratorDelegate)(object_0 => !(BotPoi.Current.AsObject != (WoWObject)null) ? (double)ForcedQuestPickUp.Me.Location.DistanceSqr(BotPoi.Current.Location) > 16.0 : !BotPoi.Current.AsObject.WithinInteractRange), (Composite)new ActionMoveToPoi()),
            (Composite)new Decorator((CanRunDecoratorDelegate)(object_0 => BotPoi.Current.AsObject != (WoWObject)null && BotPoi.Current.AsObject.WithinInteractRange), (Composite)new Sequence((ContextChangeHandler)(object_0 => (object)BotPoi.Current.AsObject), new Composite[10]
            {
                (Composite)new ActionMoveStop(),
                (Composite)new TreeSharp.Action((ActionDelegate)(object_0 => this.method_2(object_0))),
                (Composite)new DecoratorContinue((CanRunDecoratorDelegate)(object_0 => object_0 is WoWUnit), (Composite)new TreeSharp.Action((ActionSucceedDelegate)(object_0 => ((WoWUnit)object_0).Target()))),
                (Composite)new TreeSharp.Action((ActionSucceedDelegate)(object_0 => ((WoWObject)object_0).Interact())),
                (Composite)new ActionSleep(1500),
                (Composite)new DecoratorContinue(new CanRunDecoratorDelegate(this.method_3), (Composite)new Sequence(new Composite[2]
                {
                    (Composite)new TreeSharp.Action((ActionDelegate)(object_0 => this.method_4(object_0))),
                    (Composite)new ActionSleep(500)
                })),
                (Composite)new DecoratorContinue(new CanRunDecoratorDelegate(this.method_5), (Composite)new Sequence(new Composite[3]
                {
                    (Composite)new DecoratorContinue(new CanRunDecoratorDelegate(this.method_6), (Composite)new Sequence(new Composite[2]
                    {
                        (Composite)new TreeSharp.Action((ActionDelegate)(object_0 => this.method_7(object_0))),
                        (Composite)new TreeSharp.Action((ActionDelegate)(object_0 => this.method_2(object_0)))
                    })),
                    (Composite)new TreeSharp.Action((ActionSucceedDelegate)(object_0 => QuestFrame.Instance.AcceptQuest())),
                    (Composite)new ActionSleep(500)
                })),
                (Composite)new TreeSharp.Action((ActionDelegate)(object_0 => this.method_8(object_0))),
                (Composite)new TreeSharp.Action((ActionDelegate)(object_0 => this.method_2(object_0))),
                (Composite)new ActionClearPoi("Quest Completed")
            }))
        }));
    }

    private bool method_1(object object_0)
    {
        BotPoi current = BotPoi.Current;
        return current.Type != PoiType.QuestPickUp || (int)current.Entry != (int)this.GiverId;
    }

    private RunStatus method_2(object object_0)
    {
        if (!GossipFrame.Instance.IsVisible && !QuestFrame.Instance.IsVisible)
            return RunStatus.Success;
        GossipFrame.Instance.Close();
        QuestFrame.Instance.Close();
        Thread.Sleep(500);
        return RunStatus.Running;
    }

    private bool method_3(object object_0)
    {
        return GossipFrame.Instance.IsVisible || ForcedQuestPickUp.frame_0.IsVisible;
    }

    private RunStatus method_4(object object_0)
    {
        if (!GossipFrame.Instance.IsVisible && !ForcedQuestPickUp.frame_0.IsVisible)
            return RunStatus.Success;
        List<GossipQuestEntry> availableQuests1 = GossipFrame.Instance.AvailableQuests;
        List<uint> availableQuests2 = QuestFrame.Instance.AvailableQuests;
        int index1 = -1;
        if (availableQuests1.Count > 0)
        {
            for (int index2 = 0; index2 < availableQuests1.Count; ++index2)
            {
                if ((long)availableQuests1[index2].Id == (long)this.QuestId)
                {
                    index1 = index2;
                    break;
                }
            }
        }
        else
        {
            if (availableQuests2.Count <= 0)
                return RunStatus.Success;
            for (int index3 = 0; index3 < availableQuests2.Count; ++index3)
            {
                if ((int)availableQuests2[index3] == (int)this.QuestId)
                {
                    index1 = index3;
                    break;
                }
            }
        }
        if (index1 == -1)
            return RunStatus.Failure;
        GossipFrame.Instance.SelectAvailableQuest(index1);
        Thread.Sleep(500);
        return RunStatus.Running;
    }

    private bool method_5(object object_0)
    {
        bool isVisible;
        return isVisible = QuestFrame.Instance.IsVisible;
    }

    private bool method_6(object object_0) => ForcedQuestPickUp.frame_1.IsVisible;

    private RunStatus method_7(object object_0)
    {
        if (this.int_0 == -1)
            this.int_0 = (int)QuestFrame.Instance.CurrentShownQuestId;
        if (QuestFrame.Instance.IsVisible && (long)this.int_0 == (long)QuestFrame.Instance.CurrentShownQuestId)
        {
            QuestFrame.Instance.CompleteQuest();
            Thread.Sleep(500);
            return RunStatus.Running;
        }
        this.int_0 = -1;
        return RunStatus.Success;
    }

    private RunStatus method_8(object object_0)
    {
        if (!ObjectManager.Me.GotTarget)
            return RunStatus.Success;
        ObjectManager.Me.ClearTarget();
        Thread.Sleep(300);
        return RunStatus.Running;
    }

    public override string ToString()
    {
        return string.Format("[ForcedQuestPickUp QuestId: {0}, QuestName: {1}]", (object)this.QuestId, (object)this.QuestName);
    }
}
