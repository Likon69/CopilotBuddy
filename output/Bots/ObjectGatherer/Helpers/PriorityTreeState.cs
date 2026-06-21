using System;
using ObjectGatherer.GUI.Tabs;
using Styx;
using Styx.Combat.CombatRoutine;
using Styx.Logic.Inventory.Frames.LootFrame;
using Styx.Logic.Pathing;
using Styx.WoWInternals;

namespace ObjectGatherer.Helpers {
    public class PriorityTreeState {
        // ===========================================================
        // Constants
        // ===========================================================

        public enum State {
            ReadyForTask,
            Moving,
            LootObject,
            LootMob
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
            if(!StyxWoW.Me.IsValid) {
                return;
            }

            if(!Variables.CanSafelyLoot) {
                return;
            }

            Variables.LootMob = Variables.GetNextCorpse;
            Variables.LootObject = Variables.GetNextObject;
            Rules.Check();

            Variables.NeedToLoot = Variables.LootMob != null || Variables.LootObject != null;

            switch(TreeState) {
                case State.ReadyForTask:

                    break;

                case State.Moving:
                    CustomLog.Diagnostic("Moving State");

                    if(Variables.LootMob != null && Variables.LootMob.IsValid && GeneralSettings.Instance.LootCorpses) {
                        if(Variables.LootMob.Guid != Variables.LootMobGuid) {
                            Variables.LootMobGuid = Variables.LootMob.Guid;
                            Variables.LootMobBlacklistStopwatch.Reset();
                        }

                        //                      if(StyxWoW.Me.IsFlying ||
                        //                            Navigator.LookupPathInfo(Variables.LootMob).Navigability >= 0) {
                        CustomLog.Normal("Found: {0}'s corpse at location: {1}", Variables.LootMob.Name,
                            Variables.LootMob.Location);
                        if(!Variables.LootMob.WithinInteractRange) {
                            Flightor.MoveTo(Variables.LootMob.Location);
                        }
                        //                        } else {
                        //                            CustomLog.Diagnostic("Can't navigate fully to loot mob {0}, blacklisting for 1 minute.",
                        //                                Variables.LootMob.SafeName);
                        //                            CustomBlacklist.Add(Variables.LootMob.Guid, TimeSpan.FromMinutes(1));
                        //                            Variables.LootMob = null;
                        //                        }
                    } else {
                        if(Variables.LootObject != null && Variables.LootObject.IsValid) {
                            if(Variables.LootObject.Guid != Variables.LootMobGuid) {
                                Variables.LootMobGuid = Variables.LootObject.Guid;
                                Variables.LootMobBlacklistStopwatch.Reset();
                            }

                            //                            if(StyxWoW.Me.IsFlying ||
                            //                                Navigator.LookupPathInfo(Variables.LootObject).Distance > 0) {
                            CustomLog.Normal("Found: {0} at location: {1}", Variables.LootObject.Name,
                                Variables.LootObject.Location);
                            if(!Variables.LootObject.WithinInteractRange) {
                                Flightor.MoveTo(Variables.LootObject.Location);
                                //                                }
                                //                            } else {
                                //                                CustomLog.Diagnostic("Can't navigate fully to object {0}, blacklisting for 1 minute.",
                                //                                    Variables.LootObject.SafeName);
                                //                                CustomBlacklist.Add(Variables.LootObject.Guid, TimeSpan.FromMinutes(1));
                                //                                Variables.LootObject = null;
                            }
                        }
                    }

                    TreeState = State.ReadyForTask;

                    break;

                case State.LootObject:
                    CustomLog.Diagnostic("LootObject State");

                    if(Variables.LootObject == null) {
                        TreeState = State.ReadyForTask;
                        return;
                    }

                    if(StyxWoW.Me.IsMoving) {
                        WoWMovement.MoveStop();
                    }

                    if(StyxWoW.Me.IsFlying) {
                        Variables.NeedToDismount = true;
                    }

                    if(StyxWoW.Me.Class == WoWClass.Druid && Variables.LootObject.IsHerb) {
                        Variables.NeedToDismount = false;
                    }

                    if(Variables.NeedToDismount) {
                        // CommonCoroutines.Dismount();
                        Flightor.MountHelper.Dismount();
                        Variables.NeedToDismount = false;
                    } else {
                        if(!StyxWoW.Me.IsFalling) {
                            if(!Variables.LootStopwatch.IsRunning) {
                                Variables.LootStopwatch.Start();

                                if(!LootFrame.Instance.IsVisible) {
                                    Variables.LootObject.Interact();
                                }
                            } else {
                                if(Variables.LootStopwatch.ElapsedMilliseconds >= 5000) {
                                    Variables.LootStopwatch.Reset();
                                }
                            }

                            if(!Variables.LootMobBlacklistStopwatch.IsRunning) {
                                Variables.LootMobBlacklistStopwatch.Start();
                            } else {
                                if(Variables.LootMobBlacklistStopwatch.ElapsedMilliseconds >= 25000) {
                                    if(Variables.LootMobGuid == Variables.LootObject.Guid) {
                                        CustomLog.Diagnostic(
                                            "Can't loot {0} in 25 seconds, blacklisting for this session.",
                                            Variables.LootObject.Name);
                                        CustomBlacklist.Add(Variables.LootObject.Guid, TimeSpan.FromDays(365));
                                        Variables.LootObject = null;
                                    }
                                }
                            }
                        }
                    }
                    TreeState = State.ReadyForTask;

                    break;

                case State.LootMob:
                    CustomLog.Diagnostic("LootMob State");

                    if(Variables.LootMob == null) {
                        TreeState = State.ReadyForTask;
                        return;
                    }

                    if(StyxWoW.Me.IsMoving) {
                        WoWMovement.MoveStop();
                    }

                    if(StyxWoW.Me.IsFlying) {
                        // CommonCoroutines.Dismount();
                        Flightor.MountHelper.Dismount();
                    } else {
                        if(!StyxWoW.Me.IsFalling) {
                            if(!Variables.LootStopwatch.IsRunning) {
                                Variables.LootStopwatch.Start();

                                if(!LootFrame.Instance.IsVisible) {
                                    Variables.LootMob.Interact();
                                }
                            } else {
                                if(Variables.LootStopwatch.ElapsedMilliseconds >= 3000) {
                                    Variables.LootStopwatch.Reset();
                                }
                            }

                            if(!Variables.LootMobBlacklistStopwatch.IsRunning) {
                                Variables.LootMobBlacklistStopwatch.Start();
                            } else {
                                if(Variables.LootMobBlacklistStopwatch.ElapsedMilliseconds >= 25000) {
                                    if(Variables.LootMobGuid == Variables.LootMob.Guid) {
                                        CustomLog.Diagnostic(
                                            "Can't loot {0} in 25 seconds, blacklisting for this session.",
                                            Variables.LootMob.Name);
                                        CustomBlacklist.Add(Variables.LootMob.Guid, TimeSpan.FromDays(365));
                                        Variables.LootMob = null;
                                    }
                                }
                            }
                        }
                    }

                    TreeState = State.ReadyForTask;

                    break;
            }

            if(TreeState != State.ReadyForTask || StyxWoW.Me.Combat) {
                return;
            }

            if(Variables.LootMob != null) {
                TreeState = !Variables.LootMob.WithinInteractRange ? State.Moving : State.LootMob;
                return;
            }

            if(Variables.LootObject != null) {
                TreeState = !Variables.LootObject.WithinInteractRange ? State.Moving : State.LootObject;
            }
        }

        // ===========================================================
        // Inner and Anonymous Classes
        // ===========================================================

    }
}
