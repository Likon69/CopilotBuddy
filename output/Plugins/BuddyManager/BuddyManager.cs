using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using Styx;
using Styx.Helpers;
using Styx.Combat;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Combat;
using Styx.Logic.Inventory.Frames.Quest;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.WoWInternals.World;
using Styx.Plugins.PluginClass;
using Styx.Combat.CombatRoutine;
using Styx.Logic;
using System.IO;
using System.Windows.Forms;




namespace BuddyManager
{
    public partial class BuddyManager : HBPlugin
    {
        /*
         * Project:  BuddyManager
         * Main Use:  Switch Bot-Base & Profile & Zone
         * 
         * Author:  No1KnowsY
         * Last updated: 07DEC2011
         * 
         * Still needs:  Dalaran hearth support, etc. See thread...
         * 
         */
        public override string Name { get { return "BuddyManager"; } }
        public override string Author { get { return "No1KnowsY"; } }
        public override Version Version { get { return new Version(1, 1, 1, 19); } }
        public LocalPlayer Me { get { return ObjectManager.Me; } }

        public DateTime _TotalStartTime;
        public DateTime _BaseStartTime;
        public Stopwatch TimeToAlertTimer = new Stopwatch();
        public int _CurrentProfile = 0;
        public bool NeedToRepairMailSell = false;
        public bool ForcedSRM = false;
        public bool HitLevel = false;
        public bool StartTimeSet = false;
        public bool AfterHearth = false;
        public bool NeedHearth = false;
        public bool SitAtHearth = false;
        public bool IsNewZone = false;
        public bool LoadedFirst = false;
        public bool NeedToFly = false;
        public bool DidJustRestart = false;
        public bool DidUseHearth = false;
        public bool EXITNOW = false;
        public bool DidUsePortal = false;

        public override bool WantButton { get { return true; } }

        private Form _configForm;
        public override void OnButtonPress()
        {
            if (_configForm == null || _configForm.IsDisposed || _configForm.Disposing)
                _configForm = new UI();

                _configForm.ShowDialog();
        }

        public override void Initialize()
        {
            Styx.BotEvents.OnBotStarted += BotEvents_OnBotStarted;
            //Styx.BotEvents.OnBotStop += BotEvents_OnBotStop;
            BuddyManagerSettings.Instance.Load();
            _CurrentProfile = 0;
            Log("Initialized.");
            Log(string.Format("Version: {0}.{1}.{2} Revision: {3}", Version.Major.ToString(), Version.Minor.ToString(), Version.Build.ToString(), Version.Revision.ToString()));

            if (Me.Level < 80)
            {
                Log("Your toon is not level 80+."); 
                Log("Depending on what your settings are I CANNOT guarantee that it will work.");
                Log("For more information see the BuddyManager Thread on TheBuddyForum.");
            }
        }

        private void BotEvents_OnBotStarted(object _object)
        {
            _Manager = new _ProfileManager(null);
            if (!StartTimeSet) { _TotalStartTime = DateTime.Now; StartTimeSet = true; }
            _BaseStartTime = DateTime.Now;
            //Log("Loaded setting (Bot Started).");
        }

        private void BotEvents_OnBotStop(object _object)
        {
            TotalRunningTime();
            TotalBaseTime();
            Log("If you hit start again it will start at the same point it was at in the schedule.");
        }

        #region Settings Variables
        /*public struct _BotPath
        {
            public bool _Exists;
            public _BotPath(string folder)
            {
                if (Directory.Exists(Path.Combine(Logging.ApplicationPath, string.Format(@"Bots/{0}", folder))))
                { _Exists = true; }
                else { _Exists = false; }
            }
        }*/


        public _ProfileManager _Manager = new _ProfileManager(null);
        public struct _ProfileManager
        {
            public string[] FilePath, BotBase, Zone;
            public int[] Hours, Mins;
            public bool ThreeEnabled, FourEnabled, LogOutAfter, LoopAfter, Repair, Mail, Sell, HearthRMS;
            public int LoopHours, LoopMinutes;
            public Dictionary<uint, string> ZonePairHorde;
            public Dictionary<uint, string> ZonePairAlly;

            /*public bool ThreeEnabled
            {
                get { return BuddyManagerSettings.Instance.ThreeEnabled; }
            }*/


            public _ProfileManager(object _object)
            {
                
                FilePath = new string[] { BuddyManagerSettings.Instance.SelectedProfile1, BuddyManagerSettings.Instance.SelectedProfile2, BuddyManagerSettings.Instance.SelectedProfile3, BuddyManagerSettings.Instance.SelectedProfile4 };
                BotBase = new string[] { BuddyManagerSettings.Instance.SelectedBB1, BuddyManagerSettings.Instance.SelectedBB2, BuddyManagerSettings.Instance.SelectedBB3, BuddyManagerSettings.Instance.SelectedBB4 };
                Zone = new string[] { BuddyManagerSettings.Instance.Zone1, BuddyManagerSettings.Instance.Zone2, BuddyManagerSettings.Instance.Zone3, BuddyManagerSettings.Instance.Zone4 };
                Hours = new int[] { BuddyManagerSettings.Instance.P1Hours, BuddyManagerSettings.Instance.P2Hours, BuddyManagerSettings.Instance.P3Hours, BuddyManagerSettings.Instance.P4Hours };
                Mins = new int[] { BuddyManagerSettings.Instance.P1Mins, BuddyManagerSettings.Instance.P2Mins, BuddyManagerSettings.Instance.P3Mins, BuddyManagerSettings.Instance.P4Mins };
                ThreeEnabled = BuddyManagerSettings.Instance.ThreeEnabled;
                FourEnabled = BuddyManagerSettings.Instance.FourEnabled;
                LogOutAfter = BuddyManagerSettings.Instance.LogOutAfter;
                LoopAfter = BuddyManagerSettings.Instance.LoopAll;
                LoopHours = BuddyManagerSettings.Instance.LoopHours;
                LoopMinutes = BuddyManagerSettings.Instance.LoopMins;
                Repair = BuddyManagerSettings.Instance.Repair;
                Mail = BuddyManagerSettings.Instance.Mail;
                Sell = BuddyManagerSettings.Instance.Sell;
                HearthRMS = BuddyManagerSettings.Instance.HearthRMS;
                //////////////////////////////////////NEED TO CHANGE
                ZonePairHorde = new Dictionary<uint, string>();
                ZonePairHorde.Add(207687, "Uldum");
                ZonePairHorde.Add(207686, "Twilight Highlands");
                ZonePairHorde.Add(207689, "Deepholm");
                ZonePairHorde.Add(207688, "Hyjal");
                ZonePairHorde.Add(206595, "Tol'Barad Peninsula");
                ZonePairAlly = new Dictionary<uint, string>();
                ZonePairAlly.Add(207692,"Hyjal");
                ZonePairAlly.Add(207693,"Deepholm");
                ZonePairAlly.Add(207694,"Twilight Highlands");
                ZonePairAlly.Add(207695,"Uldum");
                ZonePairAlly.Add(206594,"Tol'Barad Peninsula");

           }
        }

        public List<string> NeedToMoveOutisde = new List<string> { "Twilight Highlands", "Vashj'ir"};
        public List<string> NeedToFlyBB = new List<string> { "Grind Bot", "Questing" };
        public List<string> NoProfileNeededBB = new List<string> { "BG Bot", "PvP", "ArchaeologyBuddy", "Instancebuddy" };
        public List<string> HearthCamper = new List<string> { "BG Bot", "PvP", "Instancebuddy" };
        /*public string BB1 { get { return BuddyManagerSettings.Instance.SelectedBB1; } }
        public string BB2 { get { return BuddyManagerSettings.Instance.SelectedBB2; } }
        public string BB3 { get { return BuddyManagerSettings.Instance.SelectedBB3; } }
        public string BB4 { get { return BuddyManagerSettings.Instance.SelectedBB4; } }
        public string SP1 { get { return BuddyManagerSettings.Instance.SelectedProfile1; } }
        public string SP2 { get { return BuddyManagerSettings.Instance.SelectedProfile2; } }
        public string SP3 { get { return BuddyManagerSettings.Instance.SelectedProfile3; } }
        public string SP4 { get { return BuddyManagerSettings.Instance.SelectedProfile4; } }
        public string Zone1 { get { return BuddyManagerSettings.Instance.Zone1; } }
        public string Zone2 { get { return BuddyManagerSettings.Instance.Zone2; } }
        public string Zone3 { get { return BuddyManagerSettings.Instance.Zone3; } }
        public string Zone4 { get { return BuddyManagerSettings.Instance.Zone4; } }
        public bool ThreeEnabled { get { return BuddyManagerSettings.Instance.ThreeEnabled; } }
        public bool FourEnabled { get { return BuddyManagerSettings.Instance.FourEnabled; } }
        public bool LogoutAfter { get { return BuddyManagerSettings.Instance.LogOutAfter; } }
        public bool LoopAfter { get { return BuddyManagerSettings.Instance.LoopAll; } }
        public int LoopHours { get { return BuddyManagerSettings.Instance.LoopHours; } }
        public int LoopMins { get { return BuddyManagerSettings.Instance.LoopMins; } }
        public int P1Hours { get { return BuddyManagerSettings.Instance.P1Hours; } }
        public int P2Hours { get { return BuddyManagerSettings.Instance.P2Hours; } }
        public int P3Hours { get { return BuddyManagerSettings.Instance.P3Hours; } }
        public int P4Hours { get { return BuddyManagerSettings.Instance.P4Hours; } }
        public int P1Mins { get { return BuddyManagerSettings.Instance.P1Mins; } }
        public int P2Mins { get { return BuddyManagerSettings.Instance.P2Mins; } }
        public int P3Mins { get { return BuddyManagerSettings.Instance.P3Mins; } }
        public int P4Mins { get { return BuddyManagerSettings.Instance.P4Mins; } }*/
        #endregion

        #region Changer Thread
        private static Thread _ChangerThread;
        private static void ChangerThread(string _BotBase, string XMLFilepath)
        {
            bool GotBotBase = false;
            Logging.Write(System.Drawing.Color.Orange, "[BuddyManager] Background thread is shutting down Honorbuddy. Will restart HB in a few seconds.");
            if (TreeRoot.IsRunning)
            { TreeRoot.Stop(); Thread.Sleep(2000); }
            foreach (KeyValuePair<string, BotBase> Base in BotManager.Instance.Bots)
            {
                if (Base.Key.ToLower().Contains(_BotBase.ToLower()))
                {
                    BotManager.Instance.SetCurrent(Base.Value);
                    if (!BotManager.Current.Initialized) BotManager.Current.DoInitialize();
                    Thread.Sleep(500);
                    GotBotBase = true;
                }
            }
            if (!GotBotBase)
            {
                Logging.Write(System.Drawing.Color.Orange, string.Format("[BuddyManager]Couldn't load BotBase: {0}", _BotBase.ToString()));
                Logging.Write("[BuddyManager]Sending 'ForceQuit()' to Wow.exe");
                Styx.Helpers.InactivityDetector.ForceLogout(true);
                _ChangerThread.Abort();
            }
            Thread.Sleep(500);
            Styx.Logic.Profiles.ProfileManager.LoadEmpty();
            Thread.Sleep(500);
            if (XMLFilepath != "None")
            {
                Styx.Logic.Profiles.ProfileManager.LoadNew(XMLFilepath);
                Thread.Sleep(3000);
            }
            while (Styx.Logic.Profiles.ProfileManager.CurrentOuterProfile.Equals(null) || Styx.Logic.Profiles.ProfileManager.CurrentProfile.Equals(null)) { Thread.Sleep(500); }
            TreeRoot.Start();
            Logging.Write(System.Drawing.Color.Orange, string.Format("[BuddyManager] Restarted Bot with Bot-Base: {0} and Profile FilePath: \"{1}\"", _BotBase.ToString(), XMLFilepath.ToString()));
            _ChangerThread.Abort();
        }
        #endregion

        #region StartChanger
        private void StartChanger()
        {
            AfterHearth = false;
            SitAtHearth = false;
            IsNewZone = false;
            NeedToFly = false;
            NeedHearth = false;
            DidUseHearth = false;
            NeedToRepairMailSell = false;

            //If you happened to be BG Qued, get rid of that shit.
            if (Styx.Logic.Battlegrounds.GetQueuedBattlegroundInfo(1).Status != Styx.Logic.BattlegroundStatus.None)
            {
                Log("Had BG Ques. Got rid of them.");
                Styx.Logic.Battlegrounds.AcceptBattlefieldPort(2, false);
                Styx.Logic.Battlegrounds.AcceptBattlefieldPort(1, false);
            }
            

            string XMLfilepath = _Manager.FilePath[_CurrentProfile];
            string botbase = _Manager.BotBase[_CurrentProfile];
            //Set these to local variables for ease of typing

            if (_CurrentProfile == 0) { Log("Forcing load of first BB/Profile Group."); }
            Log("Switching to Bot-Base: ", botbase);
            if (NoProfileNeededBB.Contains(botbase))
            { XMLfilepath = "None"; }
            if (XMLfilepath != "None") Log("Loading profile from: ", XMLfilepath);
            else Log("No Profile Needed.");
            if (_Manager.Zone[_CurrentProfile] != "None") Log(string.Format("Next Zone is: {0}.", _Manager.Zone[_CurrentProfile]));
            else Log("Next zone is set to None.");
            //Simple notify the user logs()

            if (_Manager.Mail || _Manager.Sell || _Manager.Repair) { NeedToRepairMailSell = true; Log("User has Mail/Sell/Repair Enabled."); }
            else { NeedToRepairMailSell = false; Log("Mail/Sell/Repair all turned off."); }

            if (HearthCamper.Contains(botbase))
            { AfterHearth = true; NeedHearth = true; SitAtHearth = true; Log("Will be camping HearthStone upon restart."); }
            //Is a hearth camping bot-base (IB,BG,PvP)

            if (_CurrentProfile != 0 && _Manager.Zone[_CurrentProfile] != _Manager.Zone[(_CurrentProfile - 1)]) { NeedHearth = true; IsNewZone = true; Log("Next Profile is set to run in :", _Manager.Zone[_CurrentProfile]); }
            //If not on the first profile (since it would cause an exception)
            //Check if the zones are different, and if so, set it to switch zone

            if (_Manager.Zone[_CurrentProfile] == "None") { NeedHearth = true; AfterHearth = true; }
            //If there is no zone set, use hearth!

            if (_CurrentProfile == 0 && _Manager.Zone[_CurrentProfile] != "None") { NeedHearth = true; AfterHearth = true; IsNewZone = true; }
            //This is in case you start it somewherez else
            

            if (_CurrentProfile == 0 && Me.ZoneId == 1637 && _Manager.Zone[_CurrentProfile] != "None") { Log("Bot started in Orgrimmar, will use portal."); NeedHearth = false; AfterHearth = false; NeedToRepairMailSell = false; IsNewZone = true; }
            if (_CurrentProfile == 0 && Me.ZoneId == 1519 && _Manager.Zone[_CurrentProfile] != "None") { Log("Bot started in Stormwind, Will use portal."); NeedHearth = false; AfterHearth = false; NeedToRepairMailSell = false; IsNewZone = true; }
            //If bot started in main city, Will use respective portal and won't need to hearth, repair, or walk outside

            if (NeedToFlyBB.Contains(botbase))
            { NeedToFly = true; Log("Will be flying to first hotspot."); }
            //Grind bot and such will require flying to first hotspot

            if (_CurrentProfile > 0 && _Manager.Zone[(_CurrentProfile - 1)] == _Manager.Zone[_CurrentProfile] && !ForcedSRM && !HearthCamper.Contains(botbase))
            { NeedHearth = false; AfterHearth = false; NeedToRepairMailSell = false; IsNewZone = false; Log("This is not a switch for Repair/Vendor/Mail, and Zones are the same.  Staying in zone after switch."); }

            if (Me.ZoneId == 1637) { Log("In Orgrimmar already. No need to Hearth."); NeedHearth = false; AfterHearth = false; NeedToRepairMailSell = false; }
            if (Me.ZoneId == 1519) { Log("In Stormwind already. No need to Hearth."); NeedHearth = false; AfterHearth = false; NeedToRepairMailSell = false; }
            //For when we are on any but the first grouping.

            if (ForcedSRM) { NeedHearth = true; AfterHearth = true; NeedToRepairMailSell = true; ForcedSRM = false; Log("This is a switch for Repair/Vendor/Mail.  Hearthing Mandatory!"); }
            else { Log("Not a Forced Sell/Mail/Repair switch."); }

            DidJustRestart = true;

            Thread.Sleep(500);
            Navigator.PlayerMover.MoveStop(); StyxWoW.SleepForLagDuration();
            //_WaitingOnThread.Start();
            _ChangerThread = new Thread(() => ChangerThread(botbase, XMLfilepath));
            _ChangerThread.Start();
            Thread.Sleep(2500);
        }
        #endregion

        public override void Pulse()
        {
            if (Styx.Logic.BehaviorTree.TreeRoot.StatusText.ToLower().Contains("loading tile")) { return; }
            if (!TimeToAlertTimer.IsRunning) TimeToAlertTimer.Start();
            if (TimeToAlertTimer.Elapsed.Minutes >= 15) { TotalBaseTime(); TimeToAlertTimer.Reset(); TimeToAlertTimer.Start(); }
                if (IsNotSafe()) return;
                /*if (System.Math.Round((DateTime.Now - _TotalStartTime).TotalMinutes) != 0 && Convert.ToInt32((DateTime.Now - _TotalStartTime).TotalMinutes) % 2 == 0)
                { Log(string.Format("Total time since originally started; Hours:{0} Mins:{1}", (DateTime.Now - _TotalStartTime).TotalHours.ToString(), (DateTime.Now - _TotalStartTime).TotalMinutes.ToString())); }
                if (System.Math.Round((DateTime.Now - _BaseStartTime).TotalMinutes) != 0 && Convert.ToInt32((DateTime.Now - _BaseStartTime).TotalMinutes) % 2 == 0)
                { Log(string.Format("Total time since Group {0} started; Hours:{1} Mins:{2}",_CurrentProfile.ToString(), (DateTime.Now - _BaseStartTime).TotalHours.ToString(), (DateTime.Now - _BaseStartTime).TotalMinutes.ToString())); }
                */
                if (!LoadedFirst)
                {
                    _CurrentProfile = 0;
                    /*if (!BotManager.Current.Name.Contains(_Manager.BotBase[_CurrentProfile]) || _Manager.FilePath[_CurrentProfile].Contains(Styx.Logic.Profiles.ProfileManager.XmlLocation))*/
                    LoadedFirst = true; StartChanger();
                }


                if (_Manager.LoopAfter && !IsNotSafe() && ((DateTime.Now - _TotalStartTime).TotalMinutes >= ((_Manager.LoopHours * 60) + _Manager.LoopMinutes)))
                {
                    TotalRunningTime();
                    Log("Total Time to loop has been reached. Sending 'ForceQuit()' to WoW.");
                    Styx.Helpers.InactivityDetector.ForceLogout(true);
                    TreeRoot.Stop();
                }
                if (EXITNOW) { TotalRunningTime(); Log("ExitNow set to true. Sending 'ForceQuit()' to WoW."); Styx.Helpers.InactivityDetector.ForceLogout(true); TreeRoot.Stop(); }


                #region NeedHearth
                if (NeedHearth)
                {
                    Stopwatch HearthTimer = new Stopwatch();
                    bool HasHeartStone = false;
                    ObjectManager.Update();
                    WoWItem stone;
                    SpellManager.StopCasting();
                    if (Me.IsFlying)
                    {
                        Log("Landing.");
                        Navigator.PlayerMover.MoveStop();
                        StyxWoW.SleepForLagDuration();
                        while (Me.IsFlying) { WoWMovement.Move(WoWMovement.MovementDirection.Descend, TimeSpan.FromSeconds(5)); }
                        WoWMovement.MoveStop();
                        Log("Landed.");
                    }
                    Styx.Logic.Mount.Dismount();
                    Navigator.PlayerMover.MoveStop();
                    StyxWoW.SleepForLagDuration();
                    Thread.Sleep(500);
                    /*if (Me.Class == WoWClass.Mage)
                    {
                        SpellManager.StopCasting();
                        Styx.Logic.Mount.Dismount();
                        Navigator.PlayerMover.MoveStop();
                        StyxWoW.SleepForLagDuration();
                        if (SpellManager.CanCast(3567))
                        {
                            SpellManager.Cast(3567);
                            Thread.Sleep(2500);
                            while (Me.IsCasting || !StyxWoW.IsInWorld)
                            {
                                Log("Sleeping while casting", null);
                                Thread.Sleep(5000);
                            }
                            Log("End of Sleep 1, Sleep additional 5 seconds.", null);
                            Thread.Sleep(5000);
                        }
                    }
                    else
                    {*/
                    Log("Looking for Hearthstone!.");
                    foreach (WoWItem _item in ObjectManager.GetObjectsOfType<WoWItem>().Where(o => o.BagSlot != 1 && o.Entry == 6948))
                    {
                        HasHeartStone = true;
                        Log("Has a Stone.");
                        if (_item.Cooldown < 1)
                        {
                            Log("Not on cooldown!");
                            DidUseHearth = true;
                            stone = _item;
                            Thread.Sleep(1000);
                            _item.Use();
                            HearthTimer.Start();
                            Thread.Sleep(2000);
                            while (Me.IsCasting) { Log("Sleep while Casting."); Thread.Sleep(100); }
                            Thread.Sleep(500);
                            if (HearthTimer.Elapsed.Seconds >= 9) { HearthTimer.Reset(); NeedHearth = false; Log("Hearthstone worked!"); }
                            else { HearthTimer.Reset(); NeedHearth = true; Log("Something Interrupted Hearthstone!"); return; }
                        }
                    }
                    if (!HasHeartStone) { Log("You don't have a HearthStone idiot. GO GET ONE!  ExitNow!"); EXITNOW = true; return; }
                    return;
                }
                #endregion

                #region AfterHearth
                if (AfterHearth && !NeedHearth)
                {
                    WoWPoint Outside = new WoWPoint(0, 0, 0);
                    Log("Sleepy for 5 seconds. At Home City.");
                    Thread.Sleep(5000);
                    ObjectManager.Update();

                    foreach (WoWUnit iKeeper in ObjectManager.GetObjectsOfType<WoWUnit>().Where(o => (o.Entry == 6929 || o.Entry == 46642 || o.Entry == 44235 || o.Entry == 6740) && o.Location.Distance(Me.Location) < 30))
                    {
                        if (iKeeper.Entry == 6929)
                        { Outside = new WoWPoint(1558.945, -4385.643, 16.88019); Log("Hearth set to Valley of Strength."); }
                        if (iKeeper.Entry == 46642)
                        { Outside = new WoWPoint(1935.14, -4696.542, 35.96473); Log("Hearth set to Valley of Honor."); }
                        if (iKeeper.Entry == 44235)
                        { Outside = new WoWPoint(-8380.801, 618.5749, 95.62397); Log("Hearth set to Dwarven district."); }
                        if (iKeeper.Entry == 6740)
                        { Outside = new WoWPoint(-8853.698, 656.3289, 96.68601); Log("Hearth set to Trade district."); }
                        break;
                    }
                    Log("Moving Outside to prevent flying stucks.");
                    while (Outside.Distance(Me.Location) >= 3) { Navigator.MoveTo(Outside); }
                    AfterHearth = false;
                    return;
                }
                #endregion

                if (!NeedHearth && !AfterHearth && NeedToRepairMailSell)
                { RepairSellMail(_Manager.Repair, _Manager.Sell, _Manager.Mail); return; }

                #region IsNewZone
                if (!SitAtHearth && !NeedHearth && !AfterHearth && !NeedToRepairMailSell && IsNewZone)
                {
                    WoWPoint PortalsHorde = new WoWPoint(2055.473, -4378.082, 98.84528);
                    WoWPoint PortalsAlly = new WoWPoint(-8209.668, 428.5376, 118.0617);

                    if (Me.IsHorde)
                    {
                        //if (RepairAtHearth) { } //Embedded if sell at hearth
                        //if (MailAtHearth) { }

                        ObjectManager.Update();
                        while (PortalsHorde.Distance(Me.Location) > 5)
                        {
                            if (!Flightor.MountHelper.Mounted && Flightor.MountHelper.CanMount)
                            {
                                Flightor.MountHelper.MountUp();
                                StyxWoW.SleepForLagDuration();
                                Thread.Sleep(500);
                                while (Me.IsCasting) { Thread.Sleep(100); }
                            }
                            Log("Mounted and flying to Org Portals!");
                            Flightor.MoveTo(PortalsHorde);
                            Thread.Sleep(250);
                        }
                        Log("Dismounting.");
                        Flightor.MountHelper.Dismount();
                        StyxWoW.SleepForLagDuration();
                        WoWMovement.MoveStop(WoWMovement.MovementDirection.All);
                        Navigator.PlayerMover.MoveStop();

                        Log("Sleep a little bit at portals.");
                        Thread.Sleep(ranNum(10000, 10000));
                        Thread.Sleep(10000);
                        //if (!DidReturnForFly) { DidReturnForFly = true; return; }
                        ObjectManager.Update();
                        uint Portal = 0;
                        foreach (KeyValuePair<uint, string> check in _Manager.ZonePairHorde)
                        {
                            if (check.Value.ToLower() == _Manager.Zone[_CurrentProfile].ToLower()) { Portal = check.Key; Log("Portal Id is: ", check.Key.ToString()); }
                        }
                        foreach (WoWGameObject _portal in ObjectManager.GetObjectsOfType<WoWGameObject>().Where(o => o.Entry == Portal))
                        {
                            while (_portal.Location.Distance(Me.Location) > _portal.InteractRange)
                            {
                                Navigator.MoveTo(_portal.Location);
                                Log(string.Format("Moving towards: {0}.", _portal.Name));
                                Thread.Sleep(100);
                            }
                            Navigator.PlayerMover.MoveStop();
                            WoWMovement.MoveStop(WoWMovement.MovementDirection.All);
                            WoWMovement.MoveStop();
                            Thread.Sleep(ranNum(500, 2000));
                            _portal.Interact();
                            Thread.Sleep(1000);
                            IsNewZone = false;
                        }


                    }


                    if (Me.IsAlliance)
                    {
                        ObjectManager.Update();
                        while (PortalsAlly.Distance(Me.Location) > 5)
                        {
                            if (!Flightor.MountHelper.Mounted && Flightor.MountHelper.CanMount)
                            {
                                Flightor.MountHelper.MountUp();
                                StyxWoW.SleepForLagDuration();
                                Thread.Sleep(500);
                                while (Me.IsCasting) { Thread.Sleep(100); }
                            }
                            Log("Mounted and flying to Storm Portals!");
                            Flightor.MoveTo(PortalsAlly);
                            Thread.Sleep(250);
                        }
                        Log("Dismounting.");
                        Flightor.MountHelper.Dismount();
                        StyxWoW.SleepForLagDuration();
                        WoWMovement.MoveStop(WoWMovement.MovementDirection.All);
                        Navigator.PlayerMover.MoveStop();


                        Log("Sleep a little bit at portals.");
                        Thread.Sleep(ranNum(10000, 20000));
                        
                        ObjectManager.Update();
                        uint Portal = 0;
                        foreach (KeyValuePair<uint, string> check in _Manager.ZonePairAlly)
                        {
                            if (check.Value.ToLower() == _Manager.Zone[_CurrentProfile].ToLower()) { Portal = check.Key; Log("Portal Id is: ", check.Key.ToString()); }
                        }
                        foreach (WoWGameObject _portal in ObjectManager.GetObjectsOfType<WoWGameObject>().Where(o => o.Entry == Portal))
                        {
                            while (_portal.Location.Distance(Me.Location) > _portal.InteractRange)
                            {
                                Navigator.MoveTo(_portal.Location);
                                Log(string.Format("Moving towards: {0}.", _portal.Name));
                                Thread.Sleep(100);
                            }
                            Navigator.PlayerMover.MoveStop();
                            WoWMovement.MoveStop(WoWMovement.MovementDirection.All);
                            WoWMovement.MoveStop();
                            Thread.Sleep(ranNum(500, 2000));
                            _portal.Interact();
                            Thread.Sleep(1000);
                            IsNewZone = false;
                        }
                        //Need someone to get the Innkeepers and XYZs!
                        /*outside the inn <Hotspot X="-8858.316" Y="658.7656" Z="96.58434" />
                         *<Vendor Name="Innkeeper Allison" Entry="6740" Type="Food" X="-8867.786" Y="673.6729" Z="97.90324" />
                         *[12:45:57 PM] codenamegamma1: Potal to Blasted Lands - ID 195141
                         *
                         * [12:48:55 PM] codenamegamma1: Portal to Tol Barad - ID 206594 - Location <Hotspot X="-8208.711" Y="450.1579" Z="117.7044" />
                         * 
                         * 
                         */
                    }
                    if (IsNewZone) { Log("Failed to find the portal.  This will cause HB to endlessly loop. Will instead ExitNow."); EXITNOW = true; return; }
                    else Log("Sent your toon through the portal to :", _Manager.Zone[_CurrentProfile]);
                    DidUsePortal = true;
                    return;

                }
                #endregion

                if (DidUsePortal)
                { MoveAfterPortal(); }

                #region NeedToFly
                if (NeedToFly && !NeedHearth && !IsNewZone && !SitAtHearth)
                {
                    
                    Log("Sleepy for 5 seconds.");
                    Thread.Sleep(5000);
                    //Need a returnBool from Lua IsFlyableArea()
                    ObjectManager.Update();
                    if (_Manager.BotBase[_CurrentProfile] == "Grind Bot")
                    {
                        Log("BotBase is 'Grind Bot'.  Flying started.");
                        //StopWatch GrindTimer = new StopWatch();
                        //GrindTimer.Start()
                        Styx.Logic.Profiles.ProfileManager.CurrentProfile.GrindArea.GetNextHotspot();
                        Styx.Logic.AreaManagement.Hotspot hotspot = Styx.Logic.Profiles.ProfileManager.CurrentProfile.GrindArea.CurrentHotSpot;
                        while (hotspot.Position.Distance(Me.Location) > 30 && !IsNotSafe())
                        {
                            if (!Flightor.MountHelper.Mounted && Flightor.MountHelper.CanMount)
                            {
                                Flightor.MountHelper.MountUp();
                                StyxWoW.SleepForLagDuration();
                                Thread.Sleep(250);
                                while (Me.IsCasting) { Thread.Sleep(100); }
                            }
                            Log("Mounted and flying to HotSpot!");
                            Flightor.MoveTo(hotspot.Position);
                            Thread.Sleep(100);
                            //if (GrindTimer.Elapsed.Minutes > 5) { Log("Took over 5 minutes to attempt flying to first hotspot.\nBreaking while loop."); whileTimer.Reset(); break; }
                        }
                        if (hotspot.Position.Distance(Me.Location) <= 30)
                        {
                            Log("Landing.");
                            Navigator.PlayerMover.MoveStop();
                            StyxWoW.SleepForLagDuration();
                            WoWMovement.Move(WoWMovement.MovementDirection.Descend, TimeSpan.FromSeconds(1));
                            WoWMovement.MoveStop();
                            Flightor.MountHelper.Dismount();
                            Navigator.Clear();
                            Log("Landed at HotSpot. Thread released.");
                            NeedToFly = false;
                        }
                    }

                    //Questing is not currently supported.  Lot of shit to look at with this.
                    if (_Manager.BotBase[_CurrentProfile] == "Questing")
                    {

                        Stopwatch QuestTimer = new Stopwatch();
                        if (!QuestTimer.IsRunning) { QuestTimer.Start(); return; }
                        QuestTimer.Reset();
                        WoWPoint POI_ = Styx.Logic.POI.BotPoi.Current.Location;
                        while (POI_.Distance(Me.Location) > 30 && !IsNotSafe())
                        {
                            if (!Flightor.MountHelper.Mounted && Flightor.MountHelper.CanMount)
                            {
                                Flightor.MountHelper.MountUp();
                                StyxWoW.SleepForLagDuration();
                                Thread.Sleep(250);
                                while (Me.IsCasting) { Thread.Sleep(100); }
                            }
                            Log("Mounted and flying to first POI!");
                            Flightor.MoveTo(POI_);
                            Thread.Sleep(100);
                            //if (whileTimer.Elapsed.Minutes > 5) { Log("Took over 5 minutes to attempt flying to first hotspot.\nBreaking while loop."); whileTimer.Reset(); break; }
                        }
                        if (POI_.Distance(Me.Location) <= 30)
                        {
                            Log("Landing.");
                            Navigator.PlayerMover.MoveStop();
                            StyxWoW.SleepForLagDuration();
                            while (Me.IsFlying) { WoWMovement.Move(WoWMovement.MovementDirection.Descend, TimeSpan.FromSeconds(1)); }
                            WoWMovement.MoveStop();
                            Log("Landed at POI. Thread released.");
                            NeedToFly = false;
                        }
                    }
                }
                #endregion

                if (!NeedToFly && DidJustRestart) { Log("We're done here for now.  Awaiting something to do :) "); DidJustRestart = false; }


                if (ItemCheck(6948).Cooldown <= 5)
                { return; }

                if (_Manager.HearthRMS  && ((Me.BagsFull || Me.FreeBagSlots <= (Styx.Logic.Profiles.ProfileManager.CurrentProfile.MinFreeBagSlots + 1) && _Manager.Sell) || (Styx.Logic.POI.BotPoi.Current.Type == Styx.Logic.POI.PoiType.Mail && _Manager.Mail) || (Styx.Logic.POI.BotPoi.Current.Type == Styx.Logic.POI.PoiType.Sell && _Manager.Sell) || (Styx.Logic.POI.BotPoi.Current.Type == Styx.Logic.POI.PoiType.Repair && _Manager.Repair)))
                {
                    _CurrentProfile++;
                    Log("Found HB needs to Mail/Sell/Repair.");
                    ForcedSRM = true;
                    Log(string.Format("Profile {0} completed", _CurrentProfile.ToString()));
                    TotalRunningTime();
                    if (_CurrentProfile >= 2)
                    {
                        if (!_Manager.ThreeEnabled && _Manager.FourEnabled) { _CurrentProfile = 3; }
                        if (!_Manager.ThreeEnabled && !_Manager.FourEnabled && _Manager.LoopAfter) { _CurrentProfile = 0; }
                        if (!_Manager.ThreeEnabled && !_Manager.FourEnabled && !_Manager.LoopAfter && _Manager.LogOutAfter) { EXITNOW = true; return; }
                    }
                    StartChanger();
                }


                if ((DateTime.Now - _BaseStartTime).TotalMinutes >= ((_Manager.Hours[_CurrentProfile] * 60) + _Manager.Mins[_CurrentProfile]))
                {
                    _CurrentProfile++;
                    Log(string.Format("Profile {0} completed", _CurrentProfile.ToString()));
                    TotalRunningTime();
                    if (_CurrentProfile >= 2)
                    {
                        if (!_Manager.ThreeEnabled && _Manager.FourEnabled) { _CurrentProfile = 3; }
                        if (!_Manager.ThreeEnabled && !_Manager.FourEnabled && _Manager.LoopAfter) { _CurrentProfile = 0; }
                        if (!_Manager.ThreeEnabled && !_Manager.FourEnabled && !_Manager.LoopAfter && _Manager.LogOutAfter) { EXITNOW = true; return; }
                    }
                    StartChanger();
                }
            

        }//End of Pulse
    }
}


