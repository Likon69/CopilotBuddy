using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using Styx;
using Styx.Helpers;
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
using System.Windows.Forms;



namespace BuddyHelper
{
    class BuddyHelper : HBPlugin
    {

        public override string Name { get { return "BuddyHelper"; } }
        public override string Author { get { return "No1KnowsY"; } }
        public override Version Version { get { return new Version(1, 4, 0, 16); } }
        public override bool WantButton { get { return false; } }

        #region Variable Declarations
        public LocalPlayer Me { get { return ObjectManager.Me; } }
        public bool hasKnife = false;
        public bool informedKnife = false;
        public bool hasPick = false;
        public bool informedPick = false;
        public bool didSetLooting = false;
        public bool _settingsApplied = false;
        public bool HasSkinning = false;
        public bool HasHerbing = false;
        public bool HasMining = false;
        public bool HasEngineering = false;
        public Stopwatch JustInteracted = new Stopwatch();
        #endregion

        public override void OnButtonPress()
        {
        }

        public override void Initialize()
        {
            Styx.BotEvents.OnBotStarted += BotEvents_OnBotStarted;
            Log("Initialized.");
        }

        
        public override void Pulse()
        {
            if (Me == null) return;

            // Lazy settings init: applies professions + GB2 settings once Me is hydrated.
            // Safe regardless of OnBotStarted timing (CBuddy 1.3.0 may fire it pre-init).
            if (!_settingsApplied)
            {
                ApplySettings();
            }

            while (Styx.Logic.Inventory.Frames.LootFrame.LootFrame.Instance.IsVisible) { Thread.Sleep(100); }
            if (JustInteracted.IsRunning && JustInteracted.Elapsed.Seconds > 5) { JustInteracted.Stop(); JustInteracted.Reset(); }

            //Skinning Section
            if (HasSkinning)
            { _HasSkinning(); }

            //Mining Section
            if (HasMining)
            { _HasMining(); }

            //Herbalism Section
            if (HasHerbing)
            { _HasHerbalism(); }

            //Engineering Section
            if (HasEngineering)
            { _HasEngineering(); }


        //Pulse
        }

        #region Logging Methods
        public void Log(string argument) { Logging.Write(System.Drawing.Color.Orange, "[{0}] {1}", Name, argument); }
        public void DebLog(string argument, params object[] args) { Logging.WriteDebug(System.Drawing.Color.Red, "[{0}] {1} {2}", Name, argument, args); }
        public void Log(string argument, string argument2, string argument3) { Logging.Write(System.Drawing.Color.Orange, "[{0}] {1} {2} {3}", Name, argument, argument2, argument3); }
        #endregion

        #region OnBotStarted
        private void BotEvents_OnBotStarted(object _object)
        {
            // Mark settings as needing re-application. The actual skill scan is deferred
            // to the first Pulse where ObjectManager.Me is non-null, which works around
            // CBuddy 1.3.0 firing OnBotStarted before player hydration.
            Log("Bot Started. Settings will be applied once player is ready.");
            _settingsApplied = false;
        }

        private void ApplySettings()
        {
            // Guard: never apply without a hydrated player.
            if (Me == null) return;

            Log("Updating Settings.");
            HasSkinning = true;
            HasMining = true;
            HasHerbing = true;
            HasEngineering = true;
            if (Me.GetSkill(SkillLine.Skinning) == null || Me.GetSkill(SkillLine.Skinning).CurrentValue == 0)
            {
                HasSkinning = false;
            }
            if (Me.GetSkill(SkillLine.Herbalism) == null || Me.GetSkill(SkillLine.Herbalism).CurrentValue == 0)
            {
                HasHerbing = false;
            }
            if (Me.GetSkill(SkillLine.Mining) == null || Me.GetSkill(SkillLine.Mining).CurrentValue == 0)
            {
                HasMining = false;
            }
            if (Me.GetSkill(SkillLine.Engineering) == null || Me.GetSkill(SkillLine.Engineering).CurrentValue == 0)
            {
                HasEngineering = false;
            }
            if (HasSkinning) { Log("Toon has skinning."); }
            if (HasMining)
            {
                Log("Toon has Mining.");
                if (Styx.BotManager.Current != null && Styx.BotManager.Current.Name.Contains("Gatherbuddy2"))
                {
                    Log("Using GB2, turned on GatherMinerals.");
                    Bots.Gatherbuddy.GatherbuddySettings.Instance.GatherMinerals = true;
                }
            }
            if (HasHerbing)
            {
                Log("Toon has Herbalism.");
                if (Styx.BotManager.Current != null && Styx.BotManager.Current.Name.Contains("Gatherbuddy2"))
                {
                    Log("Using GB2, turned on GatherHerbs.");
                    Bots.Gatherbuddy.GatherbuddySettings.Instance.GatherHerbs = true;
                }
            }
            if (HasEngineering) { Log("Toon has Engineering."); }

            Bots.Gatherbuddy.GatherbuddySettings.Instance.LootMobs = true;
            Styx.Helpers.CharacterSettings.Instance.LootMobs = true;
            // LootTargeting.LootMobs is read-only (forwards to CharacterSettings.Instance.LootMobs)
            Log("Settings Updates Complete!", null, null);
            didSetLooting = true;
            _settingsApplied = true;
        }
        #endregion

        #region Extensive Combat Check
        public bool ExtensiveCombatCheck()
        {
            using (new FrameLock())
            {
                if (Me.Combat) { return true; }
                if (Me.IsActuallyInCombat) { return true; }
                foreach (WoWUnit _Unit in ObjectManager.GetObjectsOfType<WoWUnit>())
                {
                    if (_Unit.Aggro || _Unit.PetAggro)
                    { return true; }
                }
                return false;
            }
        }
        #endregion

        #region Mob Gathering Methods
        //Skinning
        private void _HasSkinning()
        {
            if (!hasKnife)
            {
                foreach (WoWItem knife in ObjectManager.GetObjectsOfType<WoWItem>(false))
                {
                    if (knife.BagSlot != -1)
                    {
                        if (knife.Entry.Equals(7005)) { hasKnife = true; }
                        if (knife.Entry.Equals(12709)) { hasKnife = true; }
                        if (knife.Entry.Equals(40772)) { hasKnife = true; }
                        if (knife.Entry.Equals(40893)) { hasKnife = true; }
                        if (knife.Entry.Equals(19901)) { hasKnife = true; }
                    }
                }
                if (hasKnife) { Log("Character has a Skinning knife!", null, null); }
            }
            if (!hasKnife && !informedKnife) { Log("Character has no Skinning Knife! Go get one!", null, null); informedKnife = true; }
            if (hasKnife)
            {
                if (!ExtensiveCombatCheck() && !Me.Mounted && !Me.IsCasting)
                {
                    ObjectManager.Update();
                    var DeadMobs = (from o in ObjectManager.GetObjectsOfType<WoWUnit>(false, false)
                                    where !o.IsAlive && o.CanSkin && o.Distance < 15 && !Blacklist.Contains(o.Guid)
                                    orderby o.DistanceSqr ascending
                                    select o);
                    if (DeadMobs.Count() > 0)
                    {
                        foreach (WoWUnit _Mob in DeadMobs)
                        {
                            if (_Mob.SkinType == WoWCreatureSkinType.Leather && !_Mob.Lootable && !Me.Looting)
                            {
                                Log("Found Skin-Able: ", _Mob.Name, ".");
                                while (!ExtensiveCombatCheck() && _Mob.Distance > _Mob.InteractRange)
                                {
                                    Navigator.MoveTo(_Mob.Location);
                                }
                                Navigator.PlayerMover.MoveStop();
                                Thread.Sleep(100); StyxWoW.SleepForLagDuration();
                                _Mob.Interact();
                                while (Me.IsCasting)
                                {
                                    Thread.Sleep(100);
                                }
                                Thread.Sleep(250);
                                while (Styx.Logic.Inventory.Frames.LootFrame.LootFrame.Instance.IsVisible)
                                {
                                    Thread.Sleep(100);
                                }
                                Log("Finished Skinning.", null, null);
                                Blacklist.Add(_Mob.Guid, TimeSpan.FromMinutes(5));
                                JustInteracted.Start();
                            }
                        }
                    }
                    DeadMobs = null;
                    return;
                }
            }
        }
        //Skinning//

        //Mining
        private void _HasMining()
        {
            if (!hasPick)
            {
                foreach (WoWItem _item in ObjectManager.GetObjectsOfType<WoWItem>(false))
                {
                    if (_item.BagSlot != -1)
                    {
                        if (_item.Entry.Equals(2901)) { hasPick = true; }
                        if (_item.Entry.Equals(40893)) { hasPick = true; }
                        if (_item.Entry.Equals(40892)) { hasPick = true; }
                        if (_item.Entry.Equals(40772)) { hasPick = true; }
                        if (_item.Entry.Equals(20723)) { hasPick = true; }
                        if (_item.Entry.Equals(1959)) { hasPick = true; }
                        if (_item.Entry.Equals(756)) { hasPick = true; }
                        if (_item.Entry.Equals(9465)) { hasPick = true; }
                    }
                }
                if (hasPick) { Log("Character has a Pickaxe!", null, null); }
            }
            if (!hasPick && !informedPick) { Log("Character has no Pickaxe! Go get one!", null, null); informedPick = true; }
            if (hasPick)
            {
                var DeadMobs = (from o in ObjectManager.GetObjectsOfType<WoWUnit>(false, false)
                                where o.Distance < 15 && !o.IsAlive && !Blacklist.Contains(o.Guid)
                                orderby o.DistanceSqr ascending
                                select o);
                if (DeadMobs.Count() > 0 && !ExtensiveCombatCheck())
                {
                    foreach (WoWUnit _Mob in DeadMobs)
                    {
                        if (_Mob.SkinType == WoWCreatureSkinType.Rock && !_Mob.Lootable && !Me.Looting)
                        {
                            Log("Found Mine-Able: ", _Mob.Name, ".");
                            while (_Mob.Distance > 5 && !ExtensiveCombatCheck())
                            {
                                Navigator.MoveTo(_Mob.Location);
                            }
                            Navigator.PlayerMover.MoveStop();
                            Thread.Sleep(100); StyxWoW.SleepForLagDuration();
                            _Mob.Interact();
                            Thread.Sleep(500);
                            while (Me.IsCasting)
                            {
                                Thread.Sleep(100);
                            }
                            Thread.Sleep(250);
                            while (Styx.Logic.Inventory.Frames.LootFrame.LootFrame.Instance.IsVisible)
                            {
                                Thread.Sleep(250);
                            }
                            Log("Finished Mining.", null, null);
                            Blacklist.Add(_Mob.Guid, TimeSpan.FromMinutes(5));
                            JustInteracted.Start();
                        }
                    }
                    DeadMobs = null;
                    return;
                }
            }
        }
        //Mining//

        //Herbalism
        private void _HasHerbalism()
        {
            var DeadMobs = (from o in ObjectManager.GetObjectsOfType<WoWUnit>(false, false)
                            where o.Distance < 15 && !o.IsAlive && !Blacklist.Contains(o.Guid)
                            orderby o.DistanceSqr ascending
                            select o);
            if (DeadMobs.Count() > 0 && !ExtensiveCombatCheck())
            {
                foreach (WoWUnit _Mob in DeadMobs)
                {
                    if (_Mob.SkinType == WoWCreatureSkinType.Herb && !_Mob.Lootable && !Me.Looting)
                    {

                        Log("Found Herb-Able: ", _Mob.Name, ".");
                        while (_Mob.Distance > 5 && !ExtensiveCombatCheck())
                        {
                            Navigator.MoveTo(_Mob.Location);
                        }
                        Navigator.PlayerMover.MoveStop();
                        Thread.Sleep(100); StyxWoW.SleepForLagDuration();
                        _Mob.Interact();
                        Thread.Sleep(500);
                        while (Me.IsCasting)
                        {
                            Thread.Sleep(100);
                        }
                        Thread.Sleep(250);
                        while (Styx.Logic.Inventory.Frames.LootFrame.LootFrame.Instance.IsVisible)
                        {
                            Thread.Sleep(250);
                        }
                        Log("Finished Herbing.", null, null);
                        Blacklist.Add(_Mob.Guid, TimeSpan.FromMinutes(5));
                        JustInteracted.Start();
                    }
                }
                DeadMobs = null;
                return;
            }
        }
        //Herbalism//

        //Engineering
        private void _HasEngineering()
        {

            var DeadMobs = (from o in ObjectManager.GetObjectsOfType<WoWUnit>(false, false)
                            where o.Distance < 15 && !o.IsAlive && !Blacklist.Contains(o.Guid)
                            orderby o.DistanceSqr ascending
                            select o);
            if (DeadMobs.Count() > 0 && !ExtensiveCombatCheck())
            {
                foreach (WoWUnit _Mob in DeadMobs)
                {
                    if (_Mob.SkinType == WoWCreatureSkinType.Bolts && !_Mob.Lootable && !Me.Looting)
                    {
                        Log("Found Salvage-Able: ", _Mob.Name, ".");
                        while (_Mob.Distance > 5 && !ExtensiveCombatCheck())
                        {
                            Navigator.MoveTo(_Mob.Location);
                        }
                        Navigator.PlayerMover.MoveStop();
                        Thread.Sleep(100); StyxWoW.SleepForLagDuration();
                        _Mob.Interact();
                        Thread.Sleep(500);
                        while (Me.IsCasting)
                        {
                            Thread.Sleep(100);
                        }
                        Thread.Sleep(250);
                        while (Styx.Logic.Inventory.Frames.LootFrame.LootFrame.Instance.IsVisible)
                        {
                            Thread.Sleep(250);
                        }
                        Log("Finished Salvaging.", null, null);
                        Blacklist.Add(_Mob.Guid, TimeSpan.FromMinutes(5));
                        JustInteracted.Start();
                    }
                }
                DeadMobs = null;
                return;
            }
        }
        //Engineering//
        #endregion


    //Class
    }


    /* 
     * CHANGE NOTES
     * 
     * 
       :Current Version:
       Version 1.4.0 Revision 16 (02DEC2011)
       //Added Changing of GatherHerbs/GatherMinerals for PB users
       //-This is for when using PB and it loads GB2 without setting these to true
       //-Will only set GB2 to harvest what you have the respective profession for
       //Code relocations for future development
       //Plugin Renamed from 'Dr. Skinner' to 'BuddyHelper'
       
       Version 1.4.0 (27NOV2011)
       //Now changes all LootMobs settings to True 
       //Fixed issue where a Skinning Knife/Pickaxe could have been in bank, but thought it was in bags
       //Added logging, tells you when you don't have a knife/pickaxe (only if you have the respective profession)
       Version 1.3.6 - 1.3.8
       //Added Gathering of Creatures!
       //Added Stopwatch to prevent extra attempts
       //Blacklists mobs its already tried to gather (In case of insufficient Skill Level)
       //Basic code improvements
       Version 0.2.3
       //Added more extensive Combat checks
       //Will now path fully to mob before skinning
       Version 0.1 (Release)
     */


//Namespace
}


