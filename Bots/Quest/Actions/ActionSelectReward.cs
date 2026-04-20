// Decompiled with JetBrains decompiler
// Type: Bots.Quest.Actions.ActionSelectReward
// Assembly: Honorbuddy, Version=2.0.0.5999, Culture=neutral, PublicKeyToken=50a565ab5c01ae50
// Based on HB 4.3.4 ActionSelectReward

using Styx.Helpers;
using Styx.Logic.Inventory;
using Styx.Logic.Inventory.Frames.Quest;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Action = TreeSharp.Action;

#nullable disable
namespace Bots.Quest.Actions;

/// <summary>
/// HB 4.3.4 ActionSelectReward — direct port.
/// First pass: WeightSetEx.EvaluateItem(itemInfo, new ItemStats(itemLink)) on equippable items.
/// Second pass: vendor sell-price fallback.
/// </summary>
public class ActionSelectReward : Action
{
    private readonly WeightSetEx _weightSet = WeightSetEx.CurrentWeightSet;

    protected override RunStatus Run(object context)
    {
        Styx.Logic.Questing.Quest currentShownQuest = QuestManager.QuestFrame.CurrentShownQuest;
        float bestScore = float.MinValue;
        int bestIndex = -1;
        string bestName = "";

        if (currentShownQuest != null)
        {
            Styx.WoWInternals.WoWCache.WoWCache.QuestCacheEntry internalInfo = currentShownQuest.InternalInfo;

            // First pass: stat-weight evaluation via WeightSetEx (HB 4.3.4 pattern)
            for (int i = 0; i < internalInfo.RewardChoiceItem.Length; i++)
            {
                int itemId = internalInfo.RewardChoiceItem[i];
                int itemCount = internalInfo.RewardChoiceItemCount[i];
                if (itemId == 0 || itemCount == 0)
                    continue;

                ItemInfo itemInfo = ItemInfo.FromId((uint)itemId);
                if (itemInfo == null || !ObjectManager.Me.CanEquipItem(itemInfo))
                    continue;

                string itemLink = Lua.GetReturnVal<string>(
                    string.Format("return GetQuestItemLink('choice', {0})", i + 1), 0U);
                if (string.IsNullOrEmpty(itemLink))
                    continue;

                ItemStats itemStats = new ItemStats(itemLink);
                float score = _weightSet.EvaluateItem(itemInfo, itemStats);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestIndex = i;
                    bestName = itemInfo.Name;
                }
            }

            // Second pass: vendor sell-price fallback (HB 4.3.4 pattern)
            if (bestIndex == -1)
            {
                float bestValue = float.MinValue;
                for (int j = 0; j < internalInfo.RewardChoiceItem.Length; j++)
                {
                    int itemId = internalInfo.RewardChoiceItem[j];
                    int itemCount = internalInfo.RewardChoiceItemCount[j];
                    if (itemId == 0 || itemCount <= 0)
                        continue;

                    ItemInfo itemInfo = ItemInfo.FromId((uint)itemId);
                    if (itemInfo == null)
                        continue;

                    float sellValue = (float)(itemInfo.SellPrice * itemCount);
                    Logging.Write("{0}{1} sells for {2}",
                        itemInfo.Name,
                        itemCount > 1 ? ("x" + itemCount) : "",
                        sellValue);

                    if (sellValue > bestValue)
                    {
                        bestName = itemInfo.Name;
                        bestValue = sellValue;
                        bestIndex = j;
                    }
                }
            }
        }

        if (bestIndex == -1)
        {
            Logging.Write("Selecting first reward as the QuestCache seems messed up and contains no questreward choices but we have questrewards to choose from.");
            Lua.DoString("QuestInfoItem1:Click()");
            return RunStatus.Success;
        }

        Logging.Write("Choosing {0}", bestName);
        QuestFrame.Instance.SelectQuestReward(bestIndex);
        return RunStatus.Success;
    }
}
