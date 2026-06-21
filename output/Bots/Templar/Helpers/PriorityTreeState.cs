using Styx;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Templar.GUI.Tabs;

namespace Templar.Helpers {
    public class PriorityTreeState {

        // ===========================================================
        // Constants
        // ===========================================================

        public enum State {
            ReadyForTask,
            Dead,
            Looting,
            Skinning,
            Pulling,
            NoMobs,
            Vendoring,
            Mailing,
            Disenchanting,
            MoveToStartLocation,
        }

        // ===========================================================
        // Fields
        // ===========================================================

        public static State TreeState = State.ReadyForTask;

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

        public static void TreeStateHandler() {
            // Place checks that are state-independent here

            /////////////////////////////////////
            // START
            /////////////////////////////////////

            if(!StyxWoW.Me.IsValid) {
                return;
            }

            if(!Variables.AlteredSettings) {
                AlterSettings();
                LogSettings();

                Variables.InventoryList.Clear();
                foreach(var bagItem in StyxWoW.Me.BagItems) {
                    Variables.InventoryList.Add(bagItem);
                    CustomLog.Diagnostic("Inventory item added. Name: {0}, Entry: {1}, StackCount: {2}", bagItem.Name, bagItem.Entry, bagItem.StackCount);
                }
            }

            TaskManager.HandleStartLocation();
            TaskManager.HandleAFKFlag();
            TaskManager.HandleCombatBug();

            if(TreeState == State.Mailing) {
                CustomLog.Diagnostic("Mailing State");

                if(Variables.MobsTargettingMe != 0) {
                    return;
                }

                if(MailSettings.Instance.Mail) {
                    Mail.CheckBags();

                    if(Variables.MailList.Count > 0) {
                        Mail.HandleMailing();
                        return;
                    }

                    TreeState = State.Vendoring;
                } else {
                    TreeState = State.Vendoring;
                }
            }

            if(TreeState == State.Vendoring) {
                CustomLog.Diagnostic("Vendoring State");

                if(Variables.MobsTargettingMe != 0) {
                    return;
                }

                if(VendorSettings.Instance.Vendor) {
                    Vendor.CheckBags();

                    if(Variables.VendorSellList.Count > 0 || Variables.NeedToRepair) {
                        Vendor.HandleVendoring();
                        return;
                    }

                    TreeState = State.MoveToStartLocation;
                } else {
                    TreeState = State.MoveToStartLocation;
                }
            }

            if(TreeState == State.Disenchanting) {
                CustomLog.Diagnostic("Disenchanting State");

                if(Variables.MobsTargettingMe != 0) {
                    return;
                }

                if(DisenchantSettings.Instance.Disenchant) {
                    Disenchant.CheckBags();

                    if(Variables.DisenchantList.Count > 0) {
                        Disenchant.HandleDisenchanting();
                        return;
                    }

                    // Get rid of any active pending cursor spell (UNTESTED)
                    Lua.DoString("RunMacroText('/stopspelltarget')");

                    TreeState = State.ReadyForTask;
                } else {
                    TreeState = State.ReadyForTask;
                }
            }


            Variables.NeedToPull = true;

            if(GeneralSettings.Instance.LootMobs) {
                Variables.LootMob = Mob.GetLootMob;
            }

            if(GeneralSettings.Instance.SkinMobs) {
                Variables.SkinMob = Mob.GetSkinMob;
            }

            /////////////////////////////////////
            // END 
            ////////////////////////////////////////

            switch(TreeState) {
                case State.Dead:
                    CustomLog.Diagnostic("Dead State");
                    Variables.NextMob = null;
                    Variables.LootMob = null;
                    Variables.SkinMob = null;

                    if(StyxWoW.Me.IsAlive) {
                        TreeState = State.ReadyForTask;
                    }

                    break;

                case State.ReadyForTask:
                    // Ready to switch states when necessary

                    break;

                case State.Looting:
                    if(Variables.MobsTargettingMe == 0) {
                        CustomLog.Diagnostic("Looting State");
                        TaskManager.HandleLooting();
                    } else {
                        TreeState = State.ReadyForTask;
                    }

                    break;

                case State.Skinning:
                    if(Variables.MobsTargettingMe == 0) {
                        CustomLog.Diagnostic("Skinning State");
                        TaskManager.HandleSkinning();
                    } else {
                        TreeState = State.ReadyForTask;
                    }

                    break;

                case State.Pulling:
                    CustomLog.Diagnostic("Pulling State");

                    if(GeneralSettings.Instance.MultiPull) {
                        //CustomLog.Diagnostic("Mobs targetting me: {0}", Variables.MobsTargettingMe);

                        Variables.NeedToPull = (StyxWoW.Me.HealthPercent > GeneralSettings.Instance.HealthPercentThreshold &&
                                      Variables.MobsTargettingMe < GeneralSettings.Instance.MobThreshold);
                    }

                    if(GeneralSettings.Instance.LootMobs && Variables.LootMob != null) {
                        Variables.NeedToPull = false;
                    }

                    if(GeneralSettings.Instance.SkinMobs && Variables.SkinMob != null) {
                        Variables.NeedToPull = false;
                    }

                    if(!GeneralSettings.Instance.MultiPull && StyxWoW.Me.Combat) {
                        Variables.NeedToPull = false;
                    }

                    //CustomLog.Diagnostic("Need to pull = {0}", Variables.NeedToPull);

                    if(!Variables.NeedToPull) {
                        TreeState = State.ReadyForTask;
                        break;
                    }

                    TaskManager.HandlePulling();

                    break;

                case State.MoveToStartLocation:
                    if(!Variables.IsAtStartLocation) {
                        CustomLog.Normal("Returning to start location.");
                        Flightor.MoveTo(Variables.StartLocation);
                    } else {
                        if(StyxWoW.Me.IsFlying) {
                            Flightor.MountHelper.Dismount();
                        } else {
                            if(!StyxWoW.Me.IsFalling) {
                                TreeState = State.ReadyForTask;
                            }
                        }
                    }

                    break;
            }

            if(TreeState == State.ReadyForTask) {
                if(!StyxWoW.Me.Combat) {
                    if(StyxWoW.Me.IsGhost || StyxWoW.Me.IsDead) {
                        TreeState = State.Dead;
                        return;
                    }

                    if(Variables.NeedToVendor && (VendorSettings.Instance.Vendor || MailSettings.Instance.Mail)) {
                        TreeState = State.Mailing;
                        return;
                    }

                    if(GeneralSettings.Instance.LootMobs) {
                        if(Variables.LootMob != null) {
                            TreeState = State.Looting;
                            return;
                        }
                    }

                    if(GeneralSettings.Instance.SkinMobs) {
                        if(Variables.SkinMob != null) {
                            TreeState = State.Skinning;
                            return;
                        }
                    }
                }

                Variables.NextMob = Mob.GetNextMob;

                if(Variables.NextMob != null) {
                    if(Variables.NeedToPull) {
                        TreeState = State.Pulling;
                    }
                } else {
                    if(!StyxWoW.Me.Combat && !StyxWoW.Me.IsActuallyInCombat && Variables.LootMob == null && Variables.SkinMob == null) {
                        CustomLog.Normal("No more mobs available, moving back to start location.");
                        TreeState = State.MoveToStartLocation;
                    }
                }
            }
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================

        private static void AlterSettings() {
            // Enable auto loot
            Lua.DoString("SetCVar('AutoLootDefault','1')");
            CustomLog.Normal("Enabled autoloot in WoW.");

            // Set FPS in WoW
            Lua.DoString("SetCVar('maxFPS', '60')");
            CustomLog.Normal("Foreground FPS set to 60 in WoW.");
            Lua.DoString("SetCVar('maxFPSBk', '30')");
            CustomLog.Normal("Background FPS set to 30 in WoW.");

            Variables.AlteredSettings = true;
        }

        private static void LogSettings() {
            CustomLog.Diagnostic("**** GENERAL TAB ****");
            CustomLog.Diagnostic(string.Format("Ignore Elites: {0}", GeneralSettings.Instance.IgnoreElites));
            CustomLog.Diagnostic(string.Format("Loot Mobs: {0}", GeneralSettings.Instance.LootMobs));
            CustomLog.Diagnostic(string.Format("Time Between Loots: {0}", GeneralSettings.Instance.TimeBetweenLoots));
            CustomLog.Diagnostic(string.Format("Skin Mobs: {0}", GeneralSettings.Instance.SkinMobs));
            CustomLog.Diagnostic(string.Format("Ninja Skin: {0}", GeneralSettings.Instance.NinjaSkin));
            CustomLog.Diagnostic(string.Format("Time Between Skins: {0}", GeneralSettings.Instance.TimeBetweenSkins));
            CustomLog.Diagnostic(string.Format("Multi Pull: {0}", GeneralSettings.Instance.MultiPull));
            CustomLog.Diagnostic(string.Format("Multi Pull Mob Threshold: {0}%", GeneralSettings.Instance.MobThreshold));
            CustomLog.Diagnostic(string.Format("Multi Pull Health Threshold: {0}", GeneralSettings.Instance.HealthPercentThreshold));
            CustomLog.Diagnostic(string.Format("Start Location Max Distance: {0}", GeneralSettings.Instance.StartingLocationMaxDistance));
            CustomLog.Diagnostic(string.Format("Minimum Bag Slots: {0}", GeneralSettings.Instance.MinimumFreeBagSlots));
            CustomLog.Diagnostic("**** WHITELIST TAB ****");
            CustomLog.Diagnostic("Whitelisted units:");
            foreach(var whitelistedUnit in WhitelistSettings.Instance.WhitelistedUnits) {
                CustomLog.Diagnostic(string.Format("Name: {0} Entry: {1}", whitelistedUnit.Name, whitelistedUnit.Entry));
            }
            CustomLog.Diagnostic(string.Format("Attack Only Whitelist: {0}", WhitelistSettings.Instance.AttackWhitelistOnly));
            CustomLog.Diagnostic("**** BLACKLIST TAB ****");
            CustomLog.Diagnostic("Blacklisted units:");
            foreach(var blacklistedUnit in BlacklistSettings.Instance.BlacklistedUnits) {
                CustomLog.Diagnostic(string.Format("Name: {0} Entry: {1}", blacklistedUnit.Name, blacklistedUnit.Entry));
            }
            CustomLog.Diagnostic("**** PROTECTED ITEMS TAB ****");
            CustomLog.Diagnostic("Protected items:");
            foreach(var protectedItem in ProtectedItemSettings.Instance.ProtectedItems) {
                CustomLog.Diagnostic(string.Format("Name: {0} Entry: {1}", protectedItem.Name, protectedItem.Entry));
            }
            CustomLog.Diagnostic("**** MAIL TAB ****");
            CustomLog.Diagnostic(string.Format("Mailing: {0}", MailSettings.Instance.Mail));
            CustomLog.Diagnostic(string.Format("Mail Whites: {0}", MailSettings.Instance.MailWhites));
            CustomLog.Diagnostic(string.Format("Mail Greens: {0}", MailSettings.Instance.MailGreens));
            CustomLog.Diagnostic(string.Format("Mail Blues: {0}", MailSettings.Instance.MailBlues));
            CustomLog.Diagnostic(string.Format("Mail Purples: {0}", MailSettings.Instance.MailPurples));
            CustomLog.Diagnostic(string.Format("Recipient: {0}", MailSettings.Instance.Recipient == "" ? "empty" : "contains name"));
            CustomLog.Diagnostic("**** VENDOR TAB ****");
            CustomLog.Diagnostic(string.Format("Vendoring: {0}", VendorSettings.Instance.Vendor));
            CustomLog.Diagnostic(string.Format("Sell Grays: {0}", VendorSettings.Instance.SellGrays));
            CustomLog.Diagnostic(string.Format("Sell Whites: {0}", VendorSettings.Instance.SellWhites));
            CustomLog.Diagnostic(string.Format("Sell Greens: {0}", VendorSettings.Instance.SellGreens));
            CustomLog.Diagnostic(string.Format("Sell Blues: {0}", VendorSettings.Instance.SellBlues));
            CustomLog.Diagnostic(string.Format("Sell Purples: {0}", VendorSettings.Instance.SellPurples));
            CustomLog.Diagnostic(string.Format("Repair at: {0}%", VendorSettings.Instance.MinimumDurabilityForRepairs));
            CustomLog.Diagnostic("**** DISENCHANT TAB ****");
            CustomLog.Diagnostic(string.Format("Disenchanting: {0}", DisenchantSettings.Instance.Disenchant));
            CustomLog.Diagnostic(string.Format("Disenchant Only New Items: {0}", DisenchantSettings.Instance.DisenchantOnlyNewItems));
            CustomLog.Diagnostic(string.Format("Disenchant Greens: {0}", DisenchantSettings.Instance.DisenchantGreens));
            CustomLog.Diagnostic(string.Format("Disenchant Blues: {0}", DisenchantSettings.Instance.DisenchantBlues));
            CustomLog.Diagnostic(string.Format("Disenchant Purples: {0}", DisenchantSettings.Instance.DisenchantPurples));
            CustomLog.Diagnostic(string.Format("Time Between Disenchants: {0}", DisenchantSettings.Instance.TimeBetweenDisenchants));
        }
    }
}
