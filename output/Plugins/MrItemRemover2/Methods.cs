using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Styx;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.Logic.Combat;
using Styx.Logic.Inventory.Frames.Merchant;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace MrItemRemover2
{
    public partial class MrItemRemover2
    {
        public void SellVenderItems(object sender, LuaEventArgs args)
        {
            if (MerchantFrame.Instance.IsVisible && IsInitialized &&
                MrItemRemover2Settings.Instance.EnableSell == "True")
            {
                LoadList(FoodList, _foodListPath);
                LoadList(DrinkList, _drinkListPath);
                LoadList(ItemNameSell, _sellListPath);
                LoadList(BagList, _bagListPath);

                foreach (WoWItem item in Me.BagItems)
                {
                    if (MrItemRemover2Settings.Instance.SellSoulbound == "False")
                    {
                        if (!item.IsSoulbound && !KeepList.Contains(item.Name) && !FoodList.Contains(item.Name) &&
                            !DrinkList.Contains(item.Name))
                        {
                            if (item.Quality == WoWItemQuality.Poor &&
                                MrItemRemover2Settings.Instance.SellGray == "True")
                            {
                                Slog("Selling Gray Item {0}", item.Name);
                                item.UseContainerItem();
                            }
                            if (item.Quality == WoWItemQuality.Common &&
                                MrItemRemover2Settings.Instance.SellWhite == "True" && !FoodList.Contains(item.Name) &&
                                !DrinkList.Contains(item.Name))
                            {
                                Slog("Selling White Item {0}", item.Name);
                                item.UseContainerItem();
                            }
                            if (item.Quality == WoWItemQuality.Uncommon &&
                                MrItemRemover2Settings.Instance.SellGreen == "True")
                            {
                                Slog("Selling Green Item {0}", item.Name);
                                item.UseContainerItem();
                            }
                            if (item.Quality == WoWItemQuality.Rare &&
                                MrItemRemover2Settings.Instance.SellBlue == "True")
                            {
                                Slog("Selling Blue Item {0}", item.Name);
                                item.UseContainerItem();
                            }
                            if (ItemNameSell.Contains(item.Name))
                            {
                                Slog("Item Matched List Selling {0}", item.Name);
                                item.UseContainerItem();
                            }

                            if (item.Quality == WoWItemQuality.Common && FoodList.Contains(item.Name) &&
                                MrItemRemover2Settings.Instance.SellFood == "True" && item.ItemInfo.RequiredLevel <= MrItemRemover2Settings.Instance.ReqRefLvl)
                            {
                                Slog("Item Matched Selling Food List {0}", item.Name);
                                item.UseContainerItem();
                            }

                            if (item.Quality == WoWItemQuality.Common && DrinkList.Contains(item.Name) &&
                                MrItemRemover2Settings.Instance.SellDrinks == "True" && item.ItemInfo.RequiredLevel <= MrItemRemover2Settings.Instance.ReqRefLvl)
                            {
                                Slog("Item Matched Selling Food List {0}", item.Name);
                                item.UseContainerItem();
                            }
                        }
                    }

                    if (MrItemRemover2Settings.Instance.SellSoulbound == "True")
                    {
                        if (!KeepList.Contains(item.Name) && !FoodList.Contains(item.Name) &&
                            !DrinkList.Contains(item.Name))
                        {
                            if (item.Quality == WoWItemQuality.Poor &&
                                MrItemRemover2Settings.Instance.SellGray == "True")
                            {
                                Slog("Selling Gray Item {0}", item.Name);
                                item.UseContainerItem();
                            }
                            if ((item.Quality == WoWItemQuality.Common &&
                                 MrItemRemover2Settings.Instance.SellWhite == "True" && !FoodList.Contains(item.Name) &&
                                !DrinkList.Contains(item.Name)))
                            {
                                Slog("Selling White Item {0}", item.Name);
                                item.UseContainerItem();
                            }
                            if (item.Quality == WoWItemQuality.Uncommon &&
                                MrItemRemover2Settings.Instance.SellGreen == "True")
                            {
                                Slog("Selling Green Item {0}", item.Name);
                                item.UseContainerItem();
                            }
                            if (item.Quality == WoWItemQuality.Rare &&
                                MrItemRemover2Settings.Instance.SellBlue == "True")
                            {
                                Slog("Selling Blue Item {0}", item.Name);
                                item.UseContainerItem();
                            }
                            if (ItemNameSell.Contains(item.Name))
                            {
                                Slog("Item Matched List Selling {0}", item.Name);
                                item.UseContainerItem();
                            }

                            if (item.Quality == WoWItemQuality.Common && FoodList.Contains(item.Name) &&
                                MrItemRemover2Settings.Instance.SellFood == "True" && item.ItemInfo.RequiredLevel <= MrItemRemover2Settings.Instance.ReqRefLvl)
                            {
                                Slog("Item Matched Selling Food List {0}", item.Name);
                                item.UseContainerItem();
                            }

                            if (item.Quality == WoWItemQuality.Common && DrinkList.Contains(item.Name) &&
                                MrItemRemover2Settings.Instance.SellDrinks == "True" && item.ItemInfo.RequiredLevel <= MrItemRemover2Settings.Instance.ReqRefLvl)
                            {
                                Slog("Item Matched Selling Drink List {0}", item.Name);
                                item.UseContainerItem();
                            }
                        }
                    }
                }
            }
        }

        private static void DeleteItemConfirmPopup(object sender, LuaEventArgs args)
        {
            string itemNamePopUp = args.Args[0].ToString();

            if (Me.CurrentTarget != null)
            {
                Slog("Clicking Yes to Comfirm {0}'s Removal From Inventory", itemNamePopUp);
                Lua.DoString("RunMacroText(\"/click StaticPopup1Button1\");");
            }
        }

        public void PrintSettings()
        {
            Dlog("Mr.ItemRemover2 Settings");
            Dlog("------------------------------------------");
            foreach (var setting in MrItemRemover2Settings.Instance.GetSettings())
            {
                string key = setting.Key;
                object value = setting.Value;
                Dlog(string.Format("{0} - {1}", key, value));
            }
            Dlog("------------------------------------------");
            Dlog(" ");
        }

        public void CheckForItems()
        {
            //Added to Make sure our list matches what we are looking for. 
            LoadList(ItemName, _removeListPath);
            LoadList(BagList, _bagListPath);

            // NB: Since we will be modifying the Me.BagItems list indirectly through WoWclient directives,
            // we can't use it as our iterator--we must make a copy, instead.
            List<WoWItem> itemsToVisit = Me.BagItems.ToList();

            foreach (WoWItem item in itemsToVisit)
            {
                StyxWoW.SleepForLagDuration();

                if (!item.IsValid)
                {
                    continue;
                }

                bool isQuestItem = IsQuestItem(item);

                if (BagList.Contains(item.Name))
                {
                    Slog("{0} is a bag, ignoring.", item.Name);
                    continue;
                }

                if (OpnList.Contains(item.Name) && item.IsOpenable &&
                    MrItemRemover2Settings.Instance.EnableOpen == "True")
                {
                    Slog("{0} can be opened. Opening.", item.Name);
                    Lua.DoString("UseItemByName(\"" + item.Name + "\")");
                }

                if (OpnList.Contains(item.Name) && item.StackCount == 1)
                {
                    Slog("{0} can be opened, so we're opening it.", item.Name);
                    Lua.DoString("UseItemByName(\"" + item.Name + "\")");
                }

                if (Combine3List.Contains(item.Name) && item.StackCount >= 3)
                {
                    uint timesToUse = (uint)(Math.Floor((double)(item.StackCount / 3)));
                    Slog("{0} can be combined {1} times, so we're combining it.", item.Name, timesToUse);
                    for (uint timesUsed = 0; timesUsed < timesToUse; timesUsed++)
                    {
                        Lua.DoString("UseItemByName(\"" + item.Name + "\")");
                        Thread.Sleep(SpellManager.GlobalCooldownLeft);
                    }
                }

                if (Combine5List.Contains(item.Name) && item.StackCount >= 5)
                {
                    uint timesToUse = (uint)(Math.Floor((double)(item.StackCount / 5)));
                    Slog("{0} can be combined {1} times, so we're combining it.", item.Name, timesToUse);
                    for (uint timesUsed = 0; timesUsed < timesToUse; timesUsed++)
                    {
                        Lua.DoString("UseItemByName(\"" + item.Name + "\")");
                        Thread.Sleep(SpellManager.GlobalCooldownLeft);
                    }
                }

                if (Combine10List.Contains(item.Name) && item.StackCount >= 10)
                {
                    uint timesToUse = (uint)(Math.Floor((double)(item.StackCount / 10)));
                    Slog("{0} can be combined {1} times, so we're combining it.", item.Name, timesToUse);
                    for (uint timesUsed = 0; timesUsed < timesToUse; timesUsed++)
                    {
                        Lua.DoString("UseItemByName(\"" + item.Name + "\")");
                        Thread.Sleep(SpellManager.GlobalCooldownLeft);
                    }
                }

                if (MrItemRemover2Settings.Instance.EnableRemove == "True" &&
                    MrItemRemover2Settings.Instance.RemoveFood == "True")
                {
                    if (!KeepList.Contains(item.Name) && FoodList.Contains(item.Name))
                    {
                        Slog("{0} was in the Food List and We want to Remove Food. Removing.", item.Name);
                        Lua.DoString("ClearCursor()");
                        item.PickUp();
                        Lua.DoString("DeleteCursorItem()");
                    }
                }

                if (MrItemRemover2Settings.Instance.EnableRemove == "True" &&
                    MrItemRemover2Settings.Instance.RemoveDrinks == "True")
                {
                    if (!KeepList.Contains(item.Name) && DrinkList.Contains(item.Name))
                    {
                        Slog("{0} was in the Drink List and We want to Remove Drinks. Removing.", item.Name);
                        Lua.DoString("ClearCursor()");
                        item.PickUp();
                        Lua.DoString("DeleteCursorItem()");
                    }
                }

                //if item name Matches whats in the text file / the internal list (after load)
                if (ItemName.Contains(item.Name) && !KeepList.Contains(item.Name))
                {
                    //probally not needed, but still user could be messing with thier inventory.
                    //Printing to the log, and Deleting the Item.
                    Slog("{0} Found Removing Item", item.Name);
                    item.PickUp();
                    Lua.DoString("DeleteCursorItem()");
                    //a small Sleep, might not be needed. 
                }

                if (MrItemRemover2Settings.Instance.DeleteQuestItems == "True" && item.ItemInfo.BeginQuestId != 0 &&
                    !KeepList.Contains(item.Name))
                {
                    Slog("{0}'s Began a Quest. Removing", item.Name);
                    item.PickUp();
                    Lua.DoString("DeleteCursorItem()");
                }

                

                //Process all Gray Items if enabled. 
                if (MrItemRemover2Settings.Instance.DeleteAllGray == "True" && item.Quality == WoWItemQuality.Poor && !BagList.Contains(item.Name))
                {
                    //Gold Format, goes in GXX SXX CXX 
                    string goldString = MrItemRemover2Settings.Instance.GoldGrays.ToString(CultureInfo.InvariantCulture);
                    int goldValue = goldString.ToInt32() * 10000;
                    string silverString =
                        MrItemRemover2Settings.Instance.SilverGrays.ToString(CultureInfo.InvariantCulture);
                    int silverValue = silverString.ToInt32() * 100;
                    string copperString =
                        MrItemRemover2Settings.Instance.CopperGrays.ToString(CultureInfo.InvariantCulture);
                    int copperValue = copperString.ToInt32();

                    //slog("Value of input sell string - " + (goldValue + silverValue + copperValue));

                    if (item.BagSlot != -1 && !isQuestItem &&
                        item.ItemInfo.SellPrice <= (goldValue + silverValue + copperValue) &&
                        !KeepList.Contains(item.Name) && !BagList.Contains(item.Name))
                    {
                        Slog("{0}'s Item Quality was Poor and only worth {1} copper. Removing.", item.Name,
                            item.ItemInfo.SellPrice);
                        Lua.DoString("ClearCursor()");
                        item.PickUp();
                        Lua.DoString("DeleteCursorItem()");
                    }
                }

                //Process all White Items if enabled.
                if (MrItemRemover2Settings.Instance.DeleteAllWhite == "True" && item.Quality == WoWItemQuality.Common && !BagList.Contains(item.Name))
                {
                    if (item.BagSlot != -1 && !isQuestItem && !KeepList.Contains(item.Name) &&
                        !BagList.Contains(item.Name) && !FoodList.Contains(item.Name) &&
                        !DrinkList.Contains(item.Name))
                    {
                        Slog("{0}'s Item Quality was Common. Removing.", item.Name);
                        Lua.DoString("ClearCursor()");
                        item.PickUp();
                        Lua.DoString("DeleteCursorItem()");
                    }
                }

                //Process all Green Items if enabled.
                if (MrItemRemover2Settings.Instance.DeleteAllGreen == "True" && item.Quality == WoWItemQuality.Uncommon && !BagList.Contains(item.Name))
                {
                    if (item.BagSlot != -1 && !isQuestItem &&
                        !KeepList.Contains(item.Name) && !BagList.Contains(item.Name))
                    {
                        Slog("{0}'s Item Quality was Uncommon. Removing.", item.Name);
                        Lua.DoString("ClearCursor()");
                        item.PickUp();
                        Lua.DoString("DeleteCursorItem()");
                    }
                }

                //Process all Blue Items if enabled.
                if (MrItemRemover2Settings.Instance.DeleteAllBlue == "True" && item.Quality == WoWItemQuality.Rare && !BagList.Contains(item.Name))
                {
                    if (item.BagSlot != -1 && !isQuestItem &&
                        !KeepList.Contains(item.Name) && !BagList.Contains(item.Name))
                    {
                        Slog("{0}'s Item Quality was Rare. Removing.", item.Name);
                        Lua.DoString("ClearCursor()");
                        item.PickUp();
                        Lua.DoString("DeleteCursorItem()");
                    }
                }    
            }
        }

        public string GetTime(DateTime input)
        {
            int hour = input.Hour;
            int min = input.Minute;
            int sec = input.Second;

            string timeInString = (hour < 10)
                ? "0" + hour.ToString(CultureInfo.InvariantCulture)
                : hour.ToString(CultureInfo.InvariantCulture);
            timeInString += ":" +
                            ((min < 10)
                                ? "0" + min.ToString(CultureInfo.InvariantCulture)
                                : min.ToString(CultureInfo.InvariantCulture));
            
            return
                timeInString + (":" +
                                ((sec < 10)
                                    ? "0" + sec.ToString(CultureInfo.InvariantCulture)
                                    : sec.ToString(CultureInfo.InvariantCulture)));
        }

        private bool IsQuestItem(WoWItem item)
        {
            if ((item == null) || !item.IsValid)
            {
                return false;
            }

            string luaCommand = string.Format("return GetContainerItemQuestInfo({0},{1});", item.BagIndex + 1,
                item.BagSlot + 1);
            bool isQuestItem =
                Lua.GetReturnVal<bool>(luaCommand, 0) // item is quest item?
                || (Lua.GetReturnVal<int>(luaCommand, 1) > 0); // item begins a quest?

            return isQuestItem;
        }
    }
}