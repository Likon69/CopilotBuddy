using System;
using System.Linq;
using Styx;
using Styx.Logic.Combat;
using Styx.Logic.Inventory.Frames.LootFrame;
using Styx.Logic.Profiles;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Templar.GUI.Tabs;

namespace Templar.Helpers {
    public class Disenchant {

        // ===========================================================
        // Constants
        // ===========================================================

        public const int DisenchantId = 13262;

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
            Variables.DisenchantList.Clear();

            foreach(var bagItem in StyxWoW.Me.BagItems.Where(FinalItem)) {
                switch(bagItem.Quality) {
                    case WoWItemQuality.Uncommon:
                        if(DisenchantSettings.Instance.DisenchantGreens && !Variables.DisenchantList.Contains(bagItem)) {
                            Variables.DisenchantList.Add(bagItem);
                        }

                        break;

                    case WoWItemQuality.Rare:
                        if(DisenchantSettings.Instance.DisenchantBlues && !Variables.DisenchantList.Contains(bagItem)) {
                            Variables.DisenchantList.Add(bagItem);
                        }

                        break;

                    case WoWItemQuality.Epic:
                        if(DisenchantSettings.Instance.DisenchantPurples && !Variables.DisenchantList.Contains(bagItem)) {
                            Variables.DisenchantList.Add(bagItem);
                        }

                        break;
                }
            }
        }

        public static void HandleDisenchanting() {
            Variables.DisenchantItem = Variables.DisenchantList[0];

            if(StyxWoW.Me.IsMoving) {
                WoWMovement.MoveStop();
            } else {
                if(Variables.DisenchantItem.Guid != Variables.DisenchantItemGuid) {
                    Variables.DisenchantItemGuid = Variables.DisenchantItem.Guid;
                    Variables.DisenchantBlacklistStopwatch.Reset();
                }

                if(!Variables.DisenchantStopwatch.IsRunning) {
                    Variables.DisenchantStopwatch.Start();

                    if(!LootFrame.Instance.IsVisible) {
                        DisenchantItem(Variables.DisenchantItem);
                    }
                } else {
                    if(Variables.DisenchantStopwatch.ElapsedMilliseconds >= Conversion.SecondsToMilliseconds(DisenchantSettings.Instance.TimeBetweenDisenchants)) {
                        Variables.DisenchantStopwatch.Reset();
                    }
                }

                if(!Variables.DisenchantBlacklistStopwatch.IsRunning) {
                    Variables.DisenchantBlacklistStopwatch.Start();
                } else {
                    if(Variables.DisenchantBlacklistStopwatch.ElapsedMilliseconds >= 25000) {
                        if(Variables.DisenchantItemGuid == Variables.DisenchantItem.Guid) {
                            CustomLog.Diagnostic("Can't disenchant {0} in 25 seconds, blacklisting for this session.", Variables.DisenchantItem.Name);
                            CustomBlacklist.Add(Variables.DisenchantItem, TimeSpan.FromDays(365));
                            Variables.DisenchantItem = null;
                        }
                    }
                }
            }
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================

        private static bool Generic(WoWObject i) {
            return i != null && i.IsValid;
        }

        private static bool Weapon(WoWItem i) {
            return !i.ItemInfo.IsWeapon || DisenchantSettings.Instance.DisenchantWeapons;
        }

        private static bool IsProtectedItem(WoWObject i) {
            return ProtectedItemSettings.Instance.ProtectedItems.All(pi => pi.Entry == i.Entry) ||
                   ProtectedItemsManager.GetAllItemIds().Contains(i.Entry) || ForceMailManager.GetAllItemIds().Contains(i.Entry);
        }

        private static bool IsBlacklisted(WoWItem i) {
            return CustomBlacklist.Contains(i);
        }

        private static bool IsNewItem(WoWItem i) {
            if(!DisenchantSettings.Instance.DisenchantOnlyNewItems) {
                return true;
            }

            return !Variables.InventoryList.Contains(i);
        }

        private static bool FinalItem(WoWItem i) {
            return Generic(i) && Weapon(i) && !IsProtectedItem(i) && !IsBlacklisted(i) && IsNewItem(i);
        }

        private static void DisenchantItem(WoWItem item) {
            if(StyxWoW.Me.CurrentPendingCursorSpell == null) {
                if(SpellManager.CanCast(DisenchantId)) {
                    SpellManager.CastSpellById(DisenchantId);
                }
            }

            if(StyxWoW.Me.CurrentPendingCursorSpell != null && StyxWoW.Me.CurrentPendingCursorSpell.Name == "Disenchant") {
                item.Use();
            }
        }
    }
}
