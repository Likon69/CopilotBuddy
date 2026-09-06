// Decompiled with JetBrains decompiler
// Type: Bots.Quest.QuestOrder.ForcedQuestTurnIn
// Assembly: Honorbuddy, Version=2.0.0.5999, Culture=neutral, PublicKeyToken=50a565ab5c01ae50
// MVID: FB7FEB85-27C0-4D17-B8DE-615FDFDA7752
// Assembly location: C:\Users\Texy6\Desktop\Honorbuddy-cleaned.exe

using Bots.Quest.Actions;
using CommonBehaviors.Actions;
using CommonBehaviors.Decorators;
using Styx;
using Styx.Helpers;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Inventory.Frames;
using Styx.Logic.Inventory.Frames.Gossip;
using Styx.Logic.Inventory.Frames.Quest;
using Styx.Logic.Pathing;
using Styx.Logic.POI;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Collections.Generic;
using System.Threading;
using TreeSharp;

#nullable disable
namespace Bots.Quest.QuestOrder;

public class ForcedQuestTurnIn : ForcedBehavior
{
    private readonly Frame QuestTitleButton = new Frame("QuestTitleButton1");
    private static readonly Frame QuestFrameCompleteButton = new Frame("QuestFrameCompleteButton");
    private int completeQuestAttempts;

    public ForcedQuestTurnIn(uint questId, string questName, uint npcId, string npcName, WoWPoint location)
    {
        this.QuestId = questId;
        this.QuestName = questName;
        this.NpcId = npcId;
        this.NpcName = npcName;
        this.Location = location;
    }

    /// <summary>
    /// HB 4.3.4 signature (ForcedQuestTurnIn.cs:28), which carries no npc name.
    /// SafeQuestTurnin and the other stock behaviors construct the turn-in this way.
    /// </summary>
    public ForcedQuestTurnIn(uint questId, string questName, uint npcId, WoWPoint location)
        : this(questId, questName, npcId, string.Empty, location)
    {
    }

    public override bool IsDone => !ObjectManager.Me.QuestLog.ContainsQuest(this.QuestId);

    public uint QuestId { get; private set; }

    public string QuestName { get; private set; }

    public uint NpcId { get; private set; }

    public string NpcName { get; private set; }

    public WoWPoint Location { get; private set; }

    // Dispose removed - we should NEVER abandon a quest we're trying to turn in!
    
    public override void OnStart()
    {
        string goalText = this.GetGoalText();
        Logging.Write("[TurnIn] {0}", (object)goalText);
        TreeRoot.GoalText = goalText;
    }

    private string GetGoalText()
    {
        Styx.Logic.Questing.Quest quest = Styx.Logic.Questing.Quest.FromId(this.QuestId);
        if (quest != null)
            return string.Format("Turning in {0}", (object)quest.Name);
        return !string.IsNullOrEmpty(this.QuestName) ? string.Format("Turning in {0}", (object)this.QuestName) : string.Format("Turning in quest with ID {0}", (object)this.QuestId);
    }

    protected override Composite CreateBehavior()
    {
        // HB 4.3.4 behavior tree structure with 9 elements in the interaction sequence
        return (Composite)new DecoratorIsNotPoiType((IEnumerable<PoiType>)new PoiType[3]
        {
            PoiType.Harvest,
            PoiType.Skin,
            PoiType.Loot
        }, (Composite)new PrioritySelector((ContextChangeHandler)(context => (object)null), new Composite[3]
        {
            (Composite)new Decorator(new CanRunDecoratorDelegate(this.ShouldSetPoi), (Composite)new ActionSetPoi(true, (RetrieveBotPoiDelegate)(context => new BotPoi(PoiType.QuestTurnIn)
            {
                Name = this.NpcName,
                Entry = this.NpcId,
                Location = this.Location
            }))),
            (Composite)new Decorator((CanRunDecoratorDelegate)(context => !(BotPoi.Current.AsObject != (WoWObject)null) ? (double)ForcedQuestTurnIn.Me.Location.DistanceSqr(BotPoi.Current.Location) > 16.0 : !BotPoi.Current.AsObject.WithinInteractRange), (Composite)new ActionMoveToPoi()),
            (Composite)new Decorator((CanRunDecoratorDelegate)(context => BotPoi.Current.AsObject != (WoWObject)null && BotPoi.Current.AsObject.WithinInteractRange), (Composite)new Sequence((ContextChangeHandler)(context => (object)BotPoi.Current.AsObject), new Composite[9]
            {
                // 1. Stop moving
                (Composite)new ActionMoveStop(),
                // 2. Close any open frames first (HB 4.3.4 - ActionDelegate)
                (Composite)new TreeSharp.Action((ActionDelegate)(context => this.CloseFrames(context))),
                // 3. Target the NPC (ActionDelegate car retourne RunStatus)
                (Composite)new TreeSharp.Action((ActionDelegate)(context => this.TargetNpc(context))),
                // 4. Interact with NPC (HB 4.3.4 - ActionDelegate)
                (Composite)new TreeSharp.Action((ActionDelegate)(context => this.InteractWithNpc(context))),
                // 5. Wait for frame to open
                (Composite)new ActionSleep(250),
                // 6. Select quest from list while GossipFrame or QuestTitleButton1 visible
                (Composite)new DecoratorContinue(new CanRunDecoratorDelegate(this.IsGossipOrQuestListVisible), (Composite)new Sequence(new Composite[2]
                {
                    (Composite)new ActionSelectQuest((int)this.QuestId),
                    (Composite)new ActionSleep(500)
                })),
                // 7. Complete quest when QuestFrame is visible
                (Composite)new DecoratorContinue(new CanRunDecoratorDelegate(this.IsQuestFrameVisible), (Composite)new Sequence(new Composite[4]
                {
                    // Click Continue if objectives shown
                    (Composite)new DecoratorContinue(new CanRunDecoratorDelegate(this.IsCompleteButtonVisible), (Composite)new TreeSharp.Action((ActionDelegate)(context => this.ClickContinue(context)))),
                    // Select reward if there are choices
                    (Composite)new DecoratorContinue(new CanRunDecoratorDelegate(this.HasRewardChoice), (Composite)new Sequence(new Composite[3]
                    {
                        (Composite)new ActionSleep(750),
                        (Composite)new ActionSelectReward(),
                        (Composite)new ActionSleep(350)
                    })),
                    // Complete the quest (HB 4.3.4 - ActionDelegate)
                    (Composite)new TreeSharp.Action((ActionDelegate)(context => this.CompleteQuest(context))),
                    // Close frames after completion (HB 4.3.4 - ActionDelegate)
                    (Composite)new TreeSharp.Action((ActionDelegate)(context => this.CloseFrames(context)))
                })),
                // 8. Clear target (HB 4.3.4 - ActionDelegate)
                (Composite)new TreeSharp.Action((ActionDelegate)(context => ForcedQuestTurnIn.ClearTarget(context))),
                // 9. Clear POI
                (Composite)new ActionClearPoi("Quest Completed #2")
            }))
        }));
    }

    private bool ShouldSetPoi(object context)
    {
        BotPoi current = BotPoi.Current;
        return current.Type != PoiType.QuestTurnIn || (int)current.Entry != (int)this.NpcId;
    }

    private RunStatus CloseFrames(object context)
    {
        bool gossipVis = GossipFrame.Instance.IsVisible;
        bool questVis = QuestFrame.Instance.IsVisible;
        Logging.WriteDebug("[CloseFrames] GossipFrame: {0}, QuestFrame: {1}", gossipVis, questVis);
        
        if (!gossipVis && !questVis)
        {
            Logging.WriteDebug("[CloseFrames] No frames open, returning Success");
            return RunStatus.Success;
        }
        Logging.WriteDebug("[CloseFrames] Closing frames...");
        GossipFrame.Instance.Close();
        QuestFrame.Instance.Close();
        StyxWoW.Sleep(300);
        return RunStatus.Running;
    }

    private RunStatus TargetNpc(object context)
    {
        // HB 4.3.4: Target the NPC if it's a unit
        WoWUnit woWunit = context as WoWUnit;
        if ((WoWObject)woWunit != (WoWObject)null)
            woWunit.Target();
        return RunStatus.Success;
    }

    private RunStatus InteractWithNpc(object context)
    {
        bool gossipVis = GossipFrame.Instance.IsVisible;
        bool questVis = QuestFrame.Instance.IsVisible;
        Logging.WriteDebug("[InteractWithNpc] GossipFrame: {0}, QuestFrame: {1}", gossipVis, questVis);
        
        if (gossipVis || questVis)
        {
            Logging.WriteDebug("[InteractWithNpc] Frame already open, returning Success");
            return RunStatus.Success;
        }
        WoWObject woWobject = (WoWObject)context;
        if (!woWobject.WithinInteractRange)
        {
            Logging.WriteDebug("[InteractWithNpc] Not in range, returning Failure");
            return RunStatus.Failure;
        }
        Logging.WriteDebug("[InteractWithNpc] Interacting...");
        woWobject.Interact();
        StyxWoW.Sleep(300);
        return RunStatus.Running; 
    }

    private static LocalPlayer Me => ObjectManager.Me;

    private bool IsGossipOrQuestListVisible(object context)
    {
        // HB 4.3.4: Return true while GossipFrame visible OR QuestTitleButton1 visible
        bool gossipVis = GossipFrame.Instance.IsVisible;
        bool titleBtnVis = this.QuestTitleButton.IsVisible;
        bool result = gossipVis || titleBtnVis;
        Logging.WriteDebug("[IsGossipOrQuestListVisible] GossipFrame: {0}, QuestTitleButton1: {1} => {2}", gossipVis, titleBtnVis, result);
        return result;
    }

    private bool IsQuestFrameVisible(object context)
    {
        // HB 4.3.4: Only enter completion sequence when QuestFrame is visible
        bool result = QuestFrame.Instance.IsVisible;
        Logging.WriteDebug("[IsQuestFrameVisible] QuestFrame.IsVisible: {0}", result);
        return result;
    }

    private bool IsCompleteButtonVisible(object context)
    {
        bool result = ForcedQuestTurnIn.QuestFrameCompleteButton.IsVisible;
        Logging.WriteDebug("[IsCompleteButtonVisible] QuestFrameCompleteButton.IsVisible: {0}", result);
        return result;
    }

    private RunStatus ClickContinue(object context)
    {
        Logging.WriteDebug("[ClickContinue] Clicking Continue button");
        QuestFrame.Instance.ClickContinue();
        StyxWoW.Sleep(1000);
        Logging.WriteDebug("[ClickContinue] After ClickContinue - QuestFrame.IsVisible: {0}", QuestFrame.Instance.IsVisible);
        return RunStatus.Success;
    }

    private bool HasRewardChoice(object context)
    {
        uint shownId = QuestFrame.Instance.CurrentShownQuestId;
        Styx.Logic.Questing.Quest quest = Styx.Logic.Questing.Quest.FromId(shownId);
        Logging.WriteDebug("[HasRewardChoice] CurrentShownQuestId: {0}, Quest found: {1}", shownId, quest != null);
        
        if (quest == null)
        {
            int luaChoices = Lua.GetReturnVal<int>("return GetNumQuestChoices()", 0U);
            Logging.WriteDebug("[HasRewardChoice] No quest cache, Lua GetNumQuestChoices: {0}", luaChoices);
            return luaChoices >= 1;
        }
        for (uint index = 0; (long)index < (long)quest.InternalInfo.RewardChoiceItem.Length; ++index)
        {
            if (quest.InternalInfo.RewardChoiceItem[(IntPtr)index] != 0)
            {
                Logging.WriteDebug("[HasRewardChoice] Found reward choice at index {0}", index);
                return true;
            }
        }
        int luaChoices2 = Lua.GetReturnVal<int>("return GetNumQuestChoices()", 0U);
        Logging.WriteDebug("[HasRewardChoice] No cache rewards, Lua GetNumQuestChoices: {0}", luaChoices2);
        return luaChoices2 >= 1;
    }

    private RunStatus CompleteQuest(object context)
    {
        bool qfVisible = QuestFrame.Instance.IsVisible;
        uint shownId = QuestFrame.Instance.CurrentShownQuestId;
        Logging.WriteDebug("[CompleteQuest] QuestFrame.IsVisible: {0}, CurrentShownQuestId: {1}, Expected: {2}", 
            qfVisible, shownId, this.QuestId);
        
        if (qfVisible && (shownId == this.QuestId || shownId == 0))
        {
            if (this.completeQuestAttempts++ == 5)
            {
                Logging.WriteDebug("[CompleteQuest] Tried 5 times, closing QuestFrame");
                QuestFrame.Instance.Close();
                this.completeQuestAttempts = 0;
                return RunStatus.Failure;
            }
            Logging.WriteDebug("[CompleteQuest] CompleteQuest attempt {0}/5", this.completeQuestAttempts);
            QuestFrame.Instance.CompleteQuest();
            StyxWoW.Sleep(500);
            Logging.WriteDebug("[CompleteQuest] After CompleteQuest - QuestFrame.IsVisible: {0}", QuestFrame.Instance.IsVisible);
            return RunStatus.Running;
        }
        Logging.WriteDebug("[CompleteQuest] Conditions not met, returning Success");
        this.completeQuestAttempts = 0;
        return RunStatus.Success;
    }

    private static RunStatus ClearTarget(object context)
    {
        if (!ForcedQuestTurnIn.Me.GotTarget)
            return RunStatus.Success;
        ForcedQuestTurnIn.Me.ClearTarget();
        StyxWoW.Sleep(300);
        return RunStatus.Running;
    }

    public override string ToString()
    {
        return string.Format("[ForcedQuestTurnIn QuestId: {0}, QuestName: {1}]", (object)this.QuestId, (object)this.QuestName);
    }
}
