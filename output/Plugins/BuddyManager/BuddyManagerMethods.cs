using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using Styx;
using Styx.Helpers;
using Styx.Combat;
using Styx.Logic.BehaviorTree;
using Styx.Logic.POI;
using Styx.Logic.Combat;
using Styx.Logic.Inventory.Frames.Merchant;
using Styx.Logic.Inventory.Frames.Quest;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.WoWInternals.World;
using Styx.Plugins.PluginClass;
using Styx.Combat.CombatRoutine;
using Styx.Logic;
using System.Windows.Forms;
using Tripper.Navigation;
// Resolve ambiguity: all Navigator calls in this file target Styx.Logic.Pathing.Navigator
using Navigator = Styx.Logic.Pathing.Navigator;

namespace BuddyManager
{
    public partial class BuddyManager
    {
        private bool IsNotSafe()
        {
            using (new FrameLock())
            {
                ObjectManager.Update();
                if (StyxWoW.Me.IsInInstance || Styx.Logic.Battlegrounds.IsInsideBattleground || StyxWoW.Me.Combat || StyxWoW.Me.IsActuallyInCombat || !StyxWoW.IsInWorld || !StyxWoW.IsInGame || (Styx.Logic.POI.BotPoi.Current.Type != PoiType.None && Styx.Logic.POI.BotPoi.Current.Type != PoiType.Hotspot && Styx.Logic.POI.BotPoi.Current.Type != PoiType.Kill && Styx.Logic.POI.BotPoi.Current.Type != PoiType.QuestPickUp && Styx.Logic.POI.BotPoi.Current.Type != PoiType.QuestTurnIn))
                { return true; }
                else { return false; }
            }
        }

        private void RepairSellMail(bool repair, bool vendor, bool mail)
        {
            uint VendorEntry = 0;
            WoWPoint VendorLoc = new WoWPoint();
            WoWPoint MailboxLoc = new WoWPoint();
            if (Me.IsHorde)
            {
                VendorEntry = 3319;
                VendorLoc = new WoWPoint(1584.327, -4318.02, 21.18613);
                MailboxLoc = new WoWPoint(1551.957, -4359.154, 17.89191);
            }
            if (Me.IsAlliance)
            {
                VendorEntry = 54321;
                VendorLoc = new WoWPoint(0, 0, 0);
                MailboxLoc = new WoWPoint(0, 0, 0);
            }
            if (repair || vendor)
            {
                while (VendorLoc.Distance(Me.Location) > 1)
                {
                    if (!Flightor.MountHelper.Mounted && Flightor.MountHelper.CanMount)
                    {
                        Flightor.MountHelper.MountUp();
                        StyxWoW.SleepForLagDuration();
                        Thread.Sleep(500);
                        while (Me.IsCasting) { Thread.Sleep(100); }
                    }
                    Log("Mounted and flying to Vendor!");
                    Flightor.MoveTo(VendorLoc);
                    Thread.Sleep(250);
                }

                Log("Dismounting.");
                //while (Me.IsFlying) { WoWMovement.Move(WoWMovement.MovementDirection.Descend, TimeSpan.FromSeconds(1)); }
                //Log("Landed.");
                Flightor.MountHelper.Dismount();
                StyxWoW.SleepForLagDuration();
                WoWMovement.MoveStop(WoWMovement.MovementDirection.All);
                Navigator.PlayerMover.MoveStop();

                Thread.Sleep(ranNum(500, 1500));
                foreach (WoWUnit unit in ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => o.Entry == VendorEntry))
                {
                    while (unit.Location.Distance(Me.Location) > unit.InteractRange)
                    {
                        Navigator.MoveTo(unit.Location);
                        Log(string.Format("Moving towards: {0}.", unit.Name));
                        unit.Target();
                        unit.Interact();
                        Thread.Sleep(500);
                        unit.Interact();
                        Thread.Sleep(ranNum(1000, 3000));
                        unit.Interact();
                    }
                    break;
                }
                StyxWoW.SleepForLagDuration();
            }
            if (Me.CurrentTarget != null)
            {
                if (repair && Me.CurrentTarget.Entry == VendorEntry && Me.CurrentTarget.IsRepairMerchant)
                { Styx.Logic.Vendors.RepairAllItems(); Thread.Sleep(ranNum(500, 3000)); }
                if (Me.Auras.ContainsKey("Herbouflage")) { string.Format("RunMacroText(\"{0}\")", "/cancelaura Herbouflage"); }
                if (Me.ActiveAuras.ContainsKey("Herbouflage")) { string.Format("RunMacroText(\"{0}\")", "/cancelaura Herbouflage"); }
                if (vendor && Me.CurrentTarget.Entry == VendorEntry && Me.CurrentTarget.IsVendor)
                { Styx.Logic.Vendors.SellAllItems(); Thread.Sleep(ranNum(500, 3000)); }
                if (Me.Auras.ContainsKey("Herbouflage")) { string.Format("RunMacroText(\"{0}\")", "/cancelaura Herbouflage"); }
                if (Me.ActiveAuras.ContainsKey("Herbouflage")) { string.Format("RunMacroText(\"{0}\")", "/cancelaura Herbouflage"); }
            }
            if (mail)
            {
                if (Styx.Logic.Inventory.InventoryManager.HaveItemsToMail)
                {
                    if (Styx.Helpers.CharacterSettings.Instance.MailRecipient != null && Styx.Helpers.CharacterSettings.Instance.MailRecipient != "")
                    {
                        bool Foundbox = false;
                        ObjectManager.Update();
                        Log("You have items to mail, and a Recipient set!");
                        var Mailboxes = ObjectManager.GetObjectsOfType<WoWGameObject>().Where(o => Navigator.CanNavigateFully(Me.Location, o.Location) && o.SubType == WoWGameObjectType.Mailbox);
                        foreach (WoWGameObject Mailbox in ObjectManager.GetObjectsOfType<WoWGameObject>().Where(o => Navigator.CanNavigateFully(Me.Location, o.Location) && o.SubType == WoWGameObjectType.Mailbox))
                        {
                            Log("Found a Mailbox (Auto).  Moving to it.", null);
                            while (Mailbox.Location.Distance(Me.Location) > Mailbox.InteractRange) { Navigator.MoveTo(Mailbox.Location); }
                            Thread.Sleep(500);
                            Navigator.PlayerMover.MoveStop();
                            WoWMovement.MoveStop(WoWMovement.MovementDirection.All);
                            Thread.Sleep(1000);
                            Mailbox.Interact();
                            Thread.Sleep(500);
                            Mailbox.Interact();
                            StyxWoW.SleepForLagDuration();
                            Log("Mailing Items!");
                            Thread.Sleep(ranNum(2000, 5000));
                            Styx.Logic.Vendors.MailAllItems();
                            if (Me.Auras.ContainsKey("Herbouflage")) { string.Format("RunMacroText(\"{0}\")", "/cancelaura Herbouflage"); }
                            if (Me.ActiveAuras.ContainsKey("Herbouflage")) { string.Format("RunMacroText(\"{0}\")", "/cancelaura Herbouflage"); }
                            StyxWoW.SleepForLagDuration();
                            Foundbox = true;
                            break;
                        }
                        if (!Foundbox)
                        {
                            Log("Couldn't find a mailbox automatically.  Using back-up.");
                            while (MailboxLoc.Distance(Me.Location) > 5)
                            {
                                if (!Flightor.MountHelper.Mounted && Flightor.MountHelper.CanMount)
                                {
                                    Flightor.MountHelper.MountUp();
                                    StyxWoW.SleepForLagDuration();
                                    Thread.Sleep(500);
                                    while (Me.IsCasting) { Thread.Sleep(100); }
                                }
                                Log("Mounted and flying to Mailbox!");
                                Flightor.MoveTo(MailboxLoc);
                                Thread.Sleep(250);
                            }
                            Log("Dismounting.");
                            Flightor.MountHelper.Dismount();
                            StyxWoW.SleepForLagDuration();
                            WoWMovement.MoveStop(WoWMovement.MovementDirection.All);
                            Navigator.PlayerMover.MoveStop();

                            Thread.Sleep(ranNum(1500, 3000));
                            ObjectManager.Update();
                            Foundbox = true;

                        }
                        if (Foundbox)
                        {
                            while (VendorLoc.Distance(Me.Location) > 1)
                            {
                                Log("Walking back to Original Vendor Location.");
                                Thread.Sleep(250);
                                Navigator.MoveTo(VendorLoc);
                            }
                        }
                        if (!Foundbox) { Log("Didn't find a Mailbox to use.  Please head to TheBuddyForum and report this issue in the BuddyManager thread!!!"); }
                    }
                    else Log("You don't have a mail recipient set!");
                }
                else Log("Found no items to mail.");
            }

            while (VendorLoc.Distance(Me.Location) > 5)
            { Navigator.MoveTo(VendorLoc); Thread.Sleep(100); }

            NeedToRepairMailSell = false;
        }

        private void MoveAfterPortal()
        {
            if (_Manager.Zone[_CurrentProfile] == "Twilight Highlands")
            {
                if (Me.IsHorde)
                {
                    Thread.Sleep(5000);
                    Log("Moving Outside (Twilight Highlands/Horde).");
                    Stopwatch MovementTimer = new Stopwatch();
                    MovementTimer.Start();
                    Thread.Sleep(100); WoWMovement.Move(WoWMovement.MovementDirection.Forward, TimeSpan.FromSeconds(5)); MovementTimer.Reset();
                    Thread.Sleep(5000);
                    WoWMovement.MoveStop();
                    Navigator.PlayerMover.MoveStop();
                }
            }
            /*
             * <SubRoutine SubRoutineName="Go to Vashj">
                <If Condition="Me.IsAlliance" IgnoreCanRun="True">
                  <FlyToAction Dismount="True" X="-8192.315" Y="448.0859" Z="116.8438" />
                  <InteractionAction Entry="207691" InteractDelay="0" InteractType="GameObject" GameObjectType="MapObjectTransport" SpellFocus="Anvil" />
                </If>
                <If Condition="Me.IsHorde" IgnoreCanRun="True">
                  <FlyToAction Dismount="True" X="2063.337" Y="-4362.29" Z="98.11018" />
                  <InteractionAction Entry="207690" InteractDelay="0" InteractType="GameObject" GameObjectType="MapObjectTransport" SpellFocus="Anvil" />
                </If>
                <WaitAction Condition="Me.ZoneId == 5144 || Me.ZoneId == 4815 || Me.ZoneId == 5145" Timeout="10000" />
                <If Condition="DistanceTo(-4458.113, 3805.779, -82.66076) &lt; 20" IgnoreCanRun="True">
                  <MoveToAction MoveType="Location" Pathing="ClickToMove" Entry="0" X="-4448.744" Y="3808.145" Z="-84.44801" />
                  <CustomAction Code="Lua.DoString(&quot;CallCompanion(\&quot;mount\&quot;, 1)&quot;);" />
                  <WaitAction Condition="Me.Auras.ContainsKey(&quot;Abyssal Seahorse&quot;)" Timeout="5000" />
                  <MoveToAction MoveType="Location" Pathing="ClickToMove" Entry="0" X="-4452.531" Y="3805.55" Z="-87.74911" />
                  <MoveToAction MoveType="Location" Pathing="ClickToMove" Entry="0" X="-4461.966" Y="3800.289" Z="-88.81821" />
                  <MoveToAction MoveType="Location" Pathing="ClickToMove" Entry="0" X="-4455.411" Y="3784.705" Z="-92.53719" />
                </If>
                <If Condition="DistanceTo(-6120.206, 4280.641, -348.8216) &lt; 20" IgnoreCanRun="True">
                  <MoveToAction MoveType="Location" Pathing="ClickToMove" Entry="0" X="-6090.067" Y="4273.509" Z="-352.9627" />
                  <CustomAction Code="Lua.DoString(&quot;CallCompanion(\&quot;mount\&quot;, 1)&quot;);" />
                  <WaitAction Condition="Me.Auras.ContainsKey(&quot;Abyssal Seahorse&quot;)" Timeout="5000" />
                  <MoveToAction MoveType="Location" Pathing="ClickToMove" Entry="0" X="-6105.042" Y="4176.022" Z="-387.1256" />
                </If>
              </SubRoutine>
             */
            DidUsePortal = false;
        }

        public void TotalRunningTime()
        {
            Log(string.Format("Total Running Time:  Hrs:{0} Mins{1}", (DateTime.Now - _TotalStartTime).TotalHours, (DateTime.Now - _TotalStartTime).TotalMinutes));
        }

        public void TotalBaseTime()
        {
            Log(string.Format("Total Time this Grouping:  Hrs:{0} Mins{1}", (DateTime.Now - _BaseStartTime).TotalHours, (DateTime.Now - _BaseStartTime).TotalMinutes));
        }

        Random rnd = new Random();
        public int ranNum(int min, int max)
        {
            return rnd.Next(min, max);
        }

        public void Log(string argument)
        {
            Logging.Write(System.Drawing.Color.Orange, "[{0}] {1}", Name, argument);
        }

        public void DebLog(string argument, params object[] args)
        {
            Logging.WriteDebug(System.Drawing.Color.IndianRed, "Non-Failure Error: {0} {1}", argument, args);
        }

        public void Log(string argument, string target)
        {
            Logging.Write(System.Drawing.Color.Orange, "[{0}] {1} {2}", Name, argument, target);
        }

        public WoWItem ItemCheck(uint _Entry)
        {
            WoWItem ReturnItem = null;
            foreach (WoWItem item in ObjectManager.GetObjectsOfType<WoWItem>().Where(o => o.BagSlot != 1 && o.Entry == _Entry))
            {
                ReturnItem = item;
            }
            return ReturnItem;
        }

        public WoWUnit UnitCheck(uint _Entry)
        {
            WoWUnit ReturnUnit = null;
            ObjectManager.Update();
            foreach (WoWUnit unit in ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => o.Entry == _Entry))
            {
                ReturnUnit = unit;
            }
            return ReturnUnit;
        }

        public WoWObject ObjectCheck(uint _Entry)
        {
            WoWObject ReturnObject = null;
            ObjectManager.Update();
            foreach (WoWObject obj in ObjectManager.GetObjectsOfType<WoWObject>().Where(o => o.Entry == _Entry))
            {
                ReturnObject = obj;
            }
            return ReturnObject;
        }


    }
}
