// Decompiled with JetBrains decompiler
// Type: Bots.Quest.Actions.ActionSelectReward
// Assembly: Honorbuddy, Version=2.0.0.5999, Culture=neutral, PublicKeyToken=50a565ab5c01ae50
// MVID: FB7FEB85-27C0-4D17-B8DE-615FDFDA7752
// Assembly location: C:\Users\Texy6\Desktop\Honorbuddy-cleaned.exe

using Styx.Helpers;
using Styx.Logic.Inventory;
using Styx.Logic.Inventory.Frames.Quest;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using TreeSharp;
using Action = TreeSharp.Action;

#nullable disable
namespace Bots.Quest.Actions;

public class ActionSelectReward : Action
{
    protected override RunStatus Run(object context)
    {
        // Get weight set lazily (may be null if not loaded)
        WeightSetEx weightSetEx_0 = WeightSetEx.CurrentWeightSet;
        
        Styx.Logic.Questing.Quest currentShownQuest = QuestManager.QuestFrame.CurrentShownQuest;
        float num1 = float.MinValue;
        int index1 = -1;
        string str = "";
        Styx.WoWInternals.WoWCache.WoWCache.QuestCacheEntry internalInfo = currentShownQuest.InternalInfo;
        
        // Only use weight set evaluation if we have one loaded
        if (weightSetEx_0 != null)
        {
            for (int index2 = 0; index2 < internalInfo.RewardChoiceItem.Length; ++index2)
            {
                int itemId = internalInfo.RewardChoiceItem[index2];
                int num2 = internalInfo.RewardChoiceItemCount[index2];
                if (itemId != 0 && num2 != 0)
                {
                    ItemInfo itemInfo = ItemInfo.FromId((uint)itemId);
                    if (itemInfo != null && ObjectManager.Me.CanEquipItem(itemInfo))
                    {
                        string returnVal = Lua.GetReturnVal<string>($"return GetQuestItemLink('choice', {index2 + 1})", 0U);
                        if (!string.IsNullOrEmpty(returnVal))
                        {
                            ItemStats itemStats = new ItemStats(returnVal);
                            float num3 = weightSetEx_0.EvaluateItem(itemInfo, itemStats);
                            if ((double)num3 > (double)num1)
                            {
                                num1 = num3;
                                index1 = index2;
                                str = itemInfo.Name;
                            }
                        }
                    }
                }
            }
        }
        
        // Fallback: select based on vendor price if no weight set match or no weight set loaded
        if (index1 == -1)
        {
            float num4 = float.MinValue;
            for (int index3 = 0; index3 < internalInfo.RewardChoiceItem.Length; ++index3)
            {
                int itemId = internalInfo.RewardChoiceItem[index3];
                int num5 = internalInfo.RewardChoiceItemCount[index3];
                if (itemId != 0 && num5 > 0)
                {
                    ItemInfo itemInfo = ItemInfo.FromId((uint)itemId);
                    if (itemInfo != null)
                    {
                        float num6 = (float)(itemInfo.SellPrice * num5);
                        Logging.Write("{0}{1} sells for {2}", (object)itemInfo.Name, num5 > 1 ? (object)("x" + (object)num5) : (object)"", (object)num6);
                        if ((double)num6 > (double)num4)
                        {
                            str = itemInfo.Name;
                            num4 = num6;
                            index1 = index3;
                        }
                    }
                }
            }
        }
        if (index1 == -1)
        {
            Logging.Write("Selecting first reward as the QuestCache seems messed up and contains no questreward choices but we have questrewards to choose from.");
            Lua.DoString("QuestInfoItem1:Click()");
            return RunStatus.Success;
        }
        Logging.Write("Choosing {0}", (object)str);
        QuestFrame.Instance.SelectQuestReward(index1);
        return RunStatus.Success;
    }
}
