using System.Linq;
using Styx;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Inventory.Frames.Merchant;
using Styx.Logic.Pathing;
using Styx.Logic.Profiles;
using Templar.GUI.Tabs;

namespace Templar.Helpers {
    public class Vendor {

        // ===========================================================
        // Constants
        // ===========================================================

        // ===========================================================
        // Fields
        // ===========================================================

        // ===========================================================
        // Constructors
        // ===========================================================

        // ===========================================================
        // Getter & Setter
        // ===========================================================

        // ===========================================================
        // Methods for/from SuperClass/Interfaces
        // ===========================================================

        // ===========================================================
        // Methods
        // ===========================================================

        public static void CheckBags() {
            Variables.VendorSellList.Clear();

            foreach(var bagItem in StyxWoW.Me.BagItems.Where(bagItem =>
                    ProtectedItemSettings.Instance.ProtectedItems.All(pi => pi.Entry != bagItem.Entry) &&
                    !ProtectedItemsManager.GetAllItemIds().Contains(bagItem.Entry) &&
                    bagItem.ItemInfo.SellPrice > 0 && bagItem.ItemInfo.BeginQuestId == 0 && bagItem.ItemInfo.Bond != WoWItemBondType.Quest)) {

                switch(bagItem.Quality) {
                    case WoWItemQuality.Poor:
                        if(VendorSettings.Instance.SellGrays) {
                            Variables.VendorSellList.Add(bagItem);
                        }

                        break;

                    case WoWItemQuality.Common:
                        if(VendorSettings.Instance.SellWhites) {
                            Variables.VendorSellList.Add(bagItem);
                        }

                        break;

                    case WoWItemQuality.Uncommon:
                        if(VendorSettings.Instance.SellGreens) {
                            Variables.VendorSellList.Add(bagItem);
                        }

                        break;

                    case WoWItemQuality.Rare:
                        if(VendorSettings.Instance.SellBlues) {
                            Variables.VendorSellList.Add(bagItem);
                        }

                        break;

                    case WoWItemQuality.Epic:
                        if(VendorSettings.Instance.SellPurples) {
                            Variables.VendorSellList.Add(bagItem);
                        }

                        break;
                }
            }
        }

        public static void HandleVendoring() {
            if(Variables.NeedToRepair) {
                if(Variables.CloseRepairVendor != null) {
                    HandleCloseVendor();
                    return;
                }

                if(Variables.FarRepairVendor != null) {
                    HandleFarVendor();
                    return;
                }

                var profileRepairVendor = ProfileManager.CurrentProfile?.VendorManager?.GetClosestVendor(Styx.Logic.Profiles.Vendor.VendorType.Repair);
                if(profileRepairVendor != null) {
                    Flightor.MoveTo(profileRepairVendor.Location);
                    return;
                }

                CustomLog.Normal("Could not find any repair vendors in the cache.");
                CustomLog.Normal("Stopping the bot to prevent that we get stuck here.");
                CustomLog.Normal("To prevent this from happening, manually find a repair vendor on this continent and start the bot near it.");
                CustomLog.Normal("Then you can go back to your spot and start the botbase again.");
                TreeRoot.Stop("Could not find any repair vendor on this continent in the cache.");
                return;
            }

            if(Variables.CloseVendor != null) {
                HandleCloseSellVendor();
                return;
            }

            if(Variables.FarVendor != null) {
                HandleFarSellVendor();
                return;
            }

            var profileSellVendor = ProfileManager.CurrentProfile?.VendorManager?.GetClosestVendor(Styx.Logic.Profiles.Vendor.VendorType.Sell);
            if(profileSellVendor != null) {
                Flightor.MoveTo(profileSellVendor.Location);
                return;
            }

            CustomLog.Normal("Could not find any vendors in the cache.");
            CustomLog.Normal("Stopping the bot to prevent that we get stuck here.");
            TreeRoot.Stop("Could not find any vendor on this continent in the cache.");
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================

        private static void HandleFarVendor() {
            if(Variables.FarRepairVendor == null) {
                CustomLog.Normal("Could not find far repair vendor.");
            } else {
                //CustomLog.Normal("We have a repair vendor! Name: {0}, Distance: {1}", repairVendor.Name, repairVendor.Location.Distance(StyxWoW.Me.Location));

                if(Variables.FarRepairVendor.Location.Distance(StyxWoW.Me.Location) > 8) {
                    Flightor.MoveTo(Variables.FarRepairVendor.Location);
                } else {
                    HandleCloseVendor();
                }
            }
        }

        private static void HandleCloseVendor() {
            if(Variables.CloseRepairVendor == null) {
                CustomLog.Normal("Could not find close repair vendor.");
            } else {
                //CustomLog.Normal("We have a repair vendor! Name: {0}, Distance: {1}", nearRepairVendor.Name, nearRepairVendor.Location.Distance(StyxWoW.Me.Location));

                if(!Variables.CloseRepairVendor.WithinInteractRange) {
                    Flightor.MoveTo(Variables.CloseRepairVendor.Location);
                } else {
                    if(!MerchantFrame.Instance.IsVisible) {
                        Variables.CloseRepairVendor.Interact();
                    } else {
                        HandleRepairAndSales();
                    }
                }
            }
        }

        private static void HandleFarSellVendor() {
            if(Variables.FarVendor == null) {
                CustomLog.Normal("Could not find far vendor.");
            } else {
                if(Variables.FarVendor.Location.Distance(StyxWoW.Me.Location) > 8) {
                    Flightor.MoveTo(Variables.FarVendor.Location);
                } else {
                    HandleCloseSellVendor();
                }
            }
        }

        private static void HandleCloseSellVendor() {
            if(Variables.CloseVendor == null) {
                CustomLog.Normal("Could not find close vendor.");
            } else {
                if(!Variables.CloseVendor.WithinInteractRange) {
                    Flightor.MoveTo(Variables.CloseVendor.Location);
                } else {
                    if(!MerchantFrame.Instance.IsVisible) {
                        Variables.CloseVendor.Interact();
                    } else {
                        HandleSalesOnly();
                    }
                }
            }
        }

        private static void HandleRepairAndSales() {
            MerchantFrame.Instance.SellItemQualities(BuildQualityMask(), null, ProtectedItemsManager.GetAllItemIds());

            if(Variables.HasEnoughForRepairs) {
                MerchantFrame.Instance.RepairAllItems();
                MerchantFrame.Instance.Close();
            } else {
                CustomLog.Normal("Did not have enough gold to repair.");
                TreeRoot.Stop("Did not have enough gold to repair.");
            }
        }

        private static void HandleSalesOnly() {
            MerchantFrame.Instance.SellItemQualities(BuildQualityMask(), null, ProtectedItemsManager.GetAllItemIds());
            MerchantFrame.Instance.Close();
        }

        private static ItemQuality BuildQualityMask() {
            ItemQuality mask = ItemQuality.None;
            if(VendorSettings.Instance.SellGrays)    mask |= ItemQuality.Poor;
            if(VendorSettings.Instance.SellWhites)   mask |= ItemQuality.Common;
            if(VendorSettings.Instance.SellGreens)   mask |= ItemQuality.Uncommon;
            if(VendorSettings.Instance.SellBlues)    mask |= ItemQuality.Rare;
            if(VendorSettings.Instance.SellPurples)  mask |= ItemQuality.Epic;
            return mask;
        }
    }
}
