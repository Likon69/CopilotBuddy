// Decompiled with JetBrains decompiler
// Type: Bots.Quest.Actions.ActionSelectQuest
// Assembly: Honorbuddy, Version=2.0.0.5999, Culture=neutral, PublicKeyToken=50a565ab5c01ae50
// MVID: FB7FEB85-27C0-4D17-B8DE-615FDFDA7752
// Assembly location: C:\Users\Texy6\Desktop\Honorbuddy-cleaned.exe

using Styx.Logic.Inventory.Frames.Gossip;
using Styx.Logic.Questing;
using Styx.WoWInternals;
using System.Collections.Generic;
using TreeSharp;
using Action = TreeSharp.Action;

#nullable disable
namespace Bots.Quest.Actions;

public class ActionSelectQuest : Action
{
    private readonly int questId;

    public ActionSelectQuest() => this.questId = -1;

    public ActionSelectQuest(int id) => this.questId = id;

    protected override RunStatus Run(object context)
    {
        Styx.Helpers.Logging.WriteDiagnostic("[ActionSelectQuest] Called with QuestId: {0}, QuestFrame.IsVisible: {1}, GossipFrame.IsVisible: {2}", 
            this.questId, 
            QuestManager.QuestFrame.IsVisible, 
            QuestManager.GossipFrame.IsVisible);
        
        if (QuestManager.QuestFrame.IsVisible && QuestManager.QuestFrame.CurrentShownQuestId != 0U)
        {
            Styx.Helpers.Logging.WriteDiagnostic("[ActionSelectQuest] QuestFrame already showing quest {0} - returning Success", QuestManager.QuestFrame.CurrentShownQuestId);
            return RunStatus.Success;
        }
        if (QuestManager.GossipFrame.IsVisible)
        {
            List<GossipQuestEntry> activeQuests = QuestManager.GossipFrame.ActiveQuests;
            Styx.Helpers.Logging.WriteDiagnostic("[ActionSelectQuest] GossipFrame active quests count: {0}", activeQuests.Count);
            for (int index = 0; index < activeQuests.Count; ++index)
            {
                Styx.Helpers.Logging.WriteDiagnostic("[ActionSelectQuest] Checking GossipQuest[{0}]: Id={1}, Index={2}", index, activeQuests[index].Id, activeQuests[index].Index);
                if (this.questId == -1 || activeQuests[index].Id == this.questId)
                {
                    PlayerQuest questById = ObjectManager.Me.QuestLog.GetQuestById((uint)activeQuests[index].Id);
                    Styx.Helpers.Logging.WriteDiagnostic("[ActionSelectQuest] Quest {0} - IsCompleted: {1}", activeQuests[index].Id, questById != null ? questById.IsCompleted.ToString() : "null");
                    if (this.questId != -1 || questById.IsCompleted)
                    {
                        Styx.Helpers.Logging.WriteDiagnostic("[ActionSelectQuest] Selecting active quest at Index {0} (QuestId: {1})", activeQuests[index].Index, activeQuests[index].Id);
                        QuestManager.GossipFrame.SelectActiveQuest(activeQuests[index].Index);
                        return RunStatus.Success;
                    }
                }
            }
            Styx.Helpers.Logging.WriteDiagnostic("[ActionSelectQuest] No matching completed quest found in GossipFrame - returning Failure");
        }
        else if (QuestManager.QuestFrame.IsVisible)
        {
            List<uint> quests = QuestManager.QuestFrame.Quests;
            Styx.Helpers.Logging.WriteDiagnostic("[ActionSelectQuest] QuestFrame quests count: {0}", quests.Count);
            if (this.questId != -1 && !quests.Contains((uint)this.questId))
            {
                Styx.Helpers.Logging.WriteDiagnostic("[ActionSelectQuest] Quest {0} not in QuestFrame list - closing and returning Failure", this.questId);
                QuestManager.QuestFrame.Close();
                return RunStatus.Failure;
            }
            for (int index = 0; index < quests.Count; ++index)
            {
                Styx.Helpers.Logging.WriteDiagnostic("[ActionSelectQuest] Checking QuestFrame Quest[{0}]: Id={1}", index, quests[index]);
                if (this.questId == -1 || (long)quests[index] == (long)this.questId)
                {
                    PlayerQuest questById = ObjectManager.Me.QuestLog.GetQuestById(quests[index]);
                    Styx.Helpers.Logging.WriteDiagnostic("[ActionSelectQuest] Quest {0} - IsCompleted: {1}", quests[index], questById != null ? questById.IsCompleted.ToString() : "null");
                    if (this.questId != -1 || questById.IsCompleted)
                    {
                        Styx.Helpers.Logging.WriteDiagnostic("[ActionSelectQuest] Calling GossipFrame.Instance.SelectActiveQuest({0})", quests[index]);
                        GossipFrame.Instance.SelectActiveQuest((int)quests[index]);
                        return RunStatus.Success;
                    }
                }
            }
            Styx.Helpers.Logging.WriteDiagnostic("[ActionSelectQuest] No matching completed quest found in QuestFrame - returning Failure");
        }
        Styx.Helpers.Logging.WriteDiagnostic("[ActionSelectQuest] Neither GossipFrame nor QuestFrame visible - returning Failure");
        return RunStatus.Failure;
    }
}
