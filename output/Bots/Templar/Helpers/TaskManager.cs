using System;
using System.Globalization;
using Styx;
using Styx.Helpers;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Inventory.Frames.LootFrame;
using Styx.Logic.Pathing;
using Styx.Logic.POI;
using Styx.WoWInternals;
using Templar.GUI.Tabs;

namespace Templar.Helpers {
    internal class TaskManager {
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

        public static void HandleErrorMessage(object sender, LuaEventArgs args) {
            // http://www.wowwiki.com/WoW_Constants/Errors
            // http://www.wowwiki.com/WoW_Constants/Spells

            var errorMessage = args.Args[0].ToString();

            var errLootDidntKill = Lua.GetReturnVal<string>("return ERR_LOOT_DIDNT_KILL", 0); // You don't have permission to loot that corpse.
            var errLootGone = Lua.GetReturnVal<string>("return ERR_LOOT_GONE", 0); // Already looted


            var spellFailedCantBeDisenchanted = Lua.GetReturnVal<string>("return SPELL_FAILED_CANT_BE_DISENCHANTED", 0); // Item cannot be disenchanted
            var errItemLocked = Lua.GetReturnVal<string>("return ERR_ITEM_LOCKED", 0); // Item is locked.
            var spellFailedLowCastLevel = Lua.GetReturnVal<string>("return SPELL_FAILED_LOW_CASTLEVEL", 0); // Skill not high enough


            if(errorMessage.Equals(errLootDidntKill)) {
                if(StyxWoW.Me.CurrentTarget != null && Variables.LootMob != null && StyxWoW.Me.CurrentTarget == Variables.LootMob) {
                    CustomLog.Normal("Don't have permission to loot mob, blacklisting for this session.");
                    CustomBlacklist.Add(Variables.LootMob.Guid, TimeSpan.FromDays(365));
                }

                if(StyxWoW.Me.CurrentTarget != null && Variables.SkinMob != null && StyxWoW.Me.CurrentTarget == Variables.SkinMob) {
                    CustomLog.Normal("Don't have permission to skin mob, blacklisting for this session.");
                    CustomBlacklist.Add(Variables.SkinMob.Guid, TimeSpan.FromDays(365));
                }
            }

            if(errorMessage.Equals(errLootGone)) {
                if(StyxWoW.Me.CurrentTarget != null && Variables.LootMob != null && StyxWoW.Me.CurrentTarget == Variables.LootMob) {
                    CustomLog.Normal("Loot mob has already been looted, blacklisting for this session.");
                    CustomBlacklist.Add(Variables.LootMob.Guid, TimeSpan.FromDays(365));
                }

                if(StyxWoW.Me.CurrentTarget != null && Variables.SkinMob != null && StyxWoW.Me.CurrentTarget == Variables.SkinMob) {
                    CustomLog.Normal("Skin mob has already been looted, blacklisting for this session.");
                    CustomBlacklist.Add(Variables.SkinMob.Guid, TimeSpan.FromDays(365));
                }
            }

            if(errorMessage.Equals(spellFailedCantBeDisenchanted) || errorMessage.Equals(errItemLocked) || errorMessage.Equals(spellFailedLowCastLevel)) {
                CustomBlacklist.Add(Variables.DisenchantItem, TimeSpan.FromDays(365));
                CustomLog.Normal("{0} has been blacklisted from disenchanting for this session.", Variables.DisenchantItem.Name);
            }
        }

        public static void HandleCombatBug() {
            if(!StyxWoW.Me.Combat) {
                Variables.CombatBugStopwatch.Stop();
                return;
            }

            if(StyxWoW.Me.IsAutoAttacking || StyxWoW.Me.IsCasting || StyxWoW.Me.IsChanneling ||
                !Variables.CombatBugStopwatch.IsRunning) {
                Variables.CombatBugStopwatch.Restart();
            }

            if(Variables.CombatBugStopwatch.ElapsedMilliseconds < 45000) {
                return;
            }

            CustomLog.Diagnostic("Combat bug, forcing a new pull.");
            PriorityTreeState.TreeState = PriorityTreeState.State.Pulling;
        }

        public static void HandleClearTarget() {
            // Clear target every so often to ensure we get the closest target possible
            if(!Variables.ClearTargetTimer.IsRunning) {
                if(StyxWoW.Me.CurrentTarget != null && !StyxWoW.Me.IsActuallyInCombat) {
                    StyxWoW.Me.ClearTarget();
                }

                Variables.ClearTargetTimer.Start();
            } else {
                if(Variables.ClearTargetTimer.ElapsedMilliseconds > 5000) {
                    Variables.ClearTargetTimer.Reset();
                }
            }
        }

        public static void HandleAFKFlag() {
            if(!StyxWoW.Me.IsAFKFlagged) {
                return;
            }

            if(!Variables.AFKTimer.IsRunning) {
                if(StyxWoW.Me.Mounted || StyxWoW.Me.IsFlying) {
                    return;
                }

                KeyboardManager.KeyUpDown((char)32);
                Lua.DoString("JumpOrAscendStart()");
                StyxWoW.ResetAfk();

                CustomLog.Normal("AFK flag detected, jumped at {0}.",
                    DateTime.Now.ToString(CultureInfo.InvariantCulture));

                Variables.AFKTimer.Start();
            } else {
                if(Variables.AFKTimer.ElapsedMilliseconds > 1000) {
                    Variables.AFKTimer.Reset();
                }
            }
        }

        public static void HandlePulling() {
            if(Variables.NextMob != null && Variables.NextMob.IsValid) {
                if(Variables.NextMob.Guid != Variables.NextMobGuid) {
                    Variables.NextMobHP = (uint)Variables.NextMob.CurrentHealth;
                    Variables.NextMobGuid = Variables.NextMob.Guid;
                    Variables.PullBlacklistStopwatch.Reset();
                }

                if(Variables.NextMob.IsAlive && !Variables.NextMob.TaggedByOther) {
                    if(StyxWoW.Me.CurrentTarget != Variables.NextMob) {
                        if(GeneralSettings.Instance.MultiPull) {
                            BotPoi.Current = new BotPoi(PoiType.Kill) {
                                Name = Variables.NextMob.Name,
                                Guid = Variables.NextMob.Guid,
                                Location = Variables.NextMob.Location
                            };
                        }

                        Variables.NextMob.Target();
                    }

                    if(Variables.NextMob.Location.Distance(StyxWoW.Me.Location) > 50) {
                        Navigator.MoveTo(Variables.NextMob.Location);
                    } else {
                        if(!Variables.PullBlacklistStopwatch.IsRunning) {
                            Variables.PullBlacklistStopwatch.Start();
                        } else {
                            if(Variables.PullBlacklistStopwatch.ElapsedMilliseconds >= 25000) {
                                if(Variables.NextMobGuid == Variables.NextMob.Guid) {
                                    if(Variables.NextMob.CurrentHealth >= Variables.NextMobHP) {
                                        CustomLog.Diagnostic("Health for {0} hasn't changed in 25 seconds, blacklisting for this session.", Variables.NextMob.Name);
                                        CustomBlacklist.Add(Variables.NextMob.Guid, TimeSpan.FromDays(365));
                                        Variables.NextMob = null;
                                    }
                                }
                            }
                        }
                    }
                } else {
                    Variables.NextMob = null;
                }
            }

            PriorityTreeState.TreeState = PriorityTreeState.State.ReadyForTask;
        }

        public static void HandleLooting() {
            if(Variables.LootMob != null && Variables.LootMob.IsValid) {
                if(Variables.LootMob.Guid != Variables.LootMobGuid) {
                    Variables.LootMobGuid = Variables.LootMob.Guid;
                    Variables.LootBlacklistStopwatch.Reset();
                }

                    if(Variables.LootMob.Location.Distance(StyxWoW.Me.Location) > 50) {
                        Navigator.MoveTo(Variables.LootMob.Location);
                    } else {
                        if(Variables.LootMob.Location.Distance(StyxWoW.Me.Location) > 3) {
                            Navigator.MoveTo(Variables.LootMob.Location);
                        } else {
                            if(StyxWoW.Me.IsMoving) {
                                WoWMovement.MoveStop();
                            }

                            if(!Variables.LootStopwatch.IsRunning) {
                                Variables.LootStopwatch.Start();

                                if(!LootFrame.Instance.IsVisible) {
                                    Variables.LootMob.Interact();
                                }
                            } else {
                                if(Variables.LootStopwatch.ElapsedMilliseconds >= Conversion.SecondsToMilliseconds(GeneralSettings.Instance.TimeBetweenLoots)) {
                                    Variables.LootStopwatch.Reset();
                                }
                            }
                        }

                        if(!Variables.LootBlacklistStopwatch.IsRunning) {
                            Variables.LootBlacklistStopwatch.Start();
                        } else {
                            if(Variables.LootBlacklistStopwatch.ElapsedMilliseconds >= 25000) {
                                if(Variables.LootMobGuid == Variables.LootMob.Guid) {
                                    CustomLog.Diagnostic("Can't loot {0} in 25 seconds, blacklisting for this session.", Variables.LootMob.Name);
                                    CustomBlacklist.Add(Variables.LootMob.Guid, TimeSpan.FromDays(365));
                                    Variables.LootMob = null;
                                }
                            }
                        }
                    }

            }

            PriorityTreeState.TreeState = PriorityTreeState.State.ReadyForTask;
        }

        public static void HandleSkinning() {
            if(Variables.SkinMob != null && Variables.SkinMob.IsValid) {
                if(Variables.SkinMob.Guid != Variables.SkinMobGuid) {
                    Variables.SkinMobGuid = Variables.SkinMob.Guid;
                    Variables.SkinBlacklistStopwatch.Reset();
                }

                    if(Variables.SkinMob.Location.Distance(StyxWoW.Me.Location) > 50) {
                        Navigator.MoveTo(Variables.SkinMob.Location);
                    } else {
                        if(Variables.SkinMob.Location.Distance(StyxWoW.Me.Location) > 3) {
                            Navigator.MoveTo(Variables.SkinMob.Location);
                        } else {
                            if(StyxWoW.Me.IsMoving) {
                                WoWMovement.MoveStop();
                            }

                            if(!Variables.LootStopwatch.IsRunning) {
                                Variables.LootStopwatch.Start();

                                if(!LootFrame.Instance.IsVisible) {
                                    Variables.SkinMob.Interact();
                                }
                            } else {
                                if(Variables.LootStopwatch.ElapsedMilliseconds >= Conversion.SecondsToMilliseconds(GeneralSettings.Instance.TimeBetweenSkins)) {
                                    Variables.LootStopwatch.Reset();
                                }
                            }
                        }

                        if(!Variables.SkinBlacklistStopwatch.IsRunning) {
                            Variables.SkinBlacklistStopwatch.Start();
                        } else {
                            if(Variables.SkinBlacklistStopwatch.ElapsedMilliseconds >= 25000) {
                                if(Variables.SkinMobGuid == Variables.SkinMob.Guid) {
                                    CustomLog.Diagnostic("Can't skin {0} in 25 seconds, blacklisting for this session.", Variables.SkinMob.Name);
                                    CustomBlacklist.Add(Variables.SkinMob.Guid, TimeSpan.FromDays(365));
                                    Variables.SkinMob = null;
                                }
                            }
                        }
                    }
            }

            PriorityTreeState.TreeState = PriorityTreeState.State.ReadyForTask;
        }

        public static void HandleStartLocation() {
            if(Variables.SetStartLocation) {
                return;
            }

            if(StyxWoW.Me.IsFlying) {
                CustomLog.Normal("Must be on the ground to begin.");
                TreeRoot.Stop("Must be on the ground to begin.");
                return;
            }

            if(StyxWoW.Me.IsAlive) {
                Variables.StartLocation = StyxWoW.Me.Location;
                Variables.StartLocationZone = StyxWoW.Me.ZoneText;
                Variables.StartLocationSubzone = StyxWoW.Me.SubZoneText;
                Variables.StartLocationContinent = StyxWoW.Me.CurrentMap.Name;

                CustomLog.Normal("Start location set. Continent: {0} Zone: {1}, Subzone: {2}, XYZ: {3}", Variables.StartLocationContinent, Variables.StartLocationZone, Variables.StartLocationSubzone, Variables.StartLocation);

                Variables.SetStartLocation = true;
            }
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================

    }
}
