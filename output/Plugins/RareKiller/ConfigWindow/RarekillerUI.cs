//=================================================================
//
//				      Rarekiller - Plugin
//						Autor: katzerle
//			Honorbuddy Plugin - www.thebuddyforum.com
//    Credits to highvoltz, bloodlove, SMcCloud, Lofi, ZapMan 
//                and all the brave Testers
//
//==================================================================
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Media;

using Styx;
using Styx.Logic.Combat;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Plugins.PluginClass;
using Styx.Logic.BehaviorTree;

using Styx.Logic.Pathing;
using Styx.Combat.CombatRoutine;
using Styx.Logic.Inventory.Frames.Quest;
using Styx.Logic.Questing;
using Styx.Plugins;
using Styx.Logic.Inventory.Frames.Gossip;
using Styx.Logic.Common;
using Styx.Logic.Inventory.Frames.Merchant;
using Styx.Logic;
using Styx.Logic.Profiles;
using Styx.Logic.Inventory.Frames.LootFrame;

namespace katzerle
{
    public partial class RarekillerUI : Form
    {
        public RarekillerUI()
        {
            InitializeComponent();

            // Addons
            CBCata.Checked = Rarekiller.Settings.CATA;
            CBWotlk.Checked = Rarekiller.Settings.WOTLK;
            CBBC.Checked = Rarekiller.Settings.BC;
            CBLowRAR.Checked = Rarekiller.Settings.LowRAR;
            CBTLPD.Checked = Rarekiller.Settings.TLPD;
			CBPoseidus.Checked = Rarekiller.Settings.Poseidus;
            CBRaptorNest.Checked = Rarekiller.Settings.RaptorNest;
            // Hunt by ID
            CBHuntByID.Checked = Rarekiller.Settings.HUNTbyID;
            TBHuntByID.Text = Rarekiller.Settings.MobID;
            CBBlacklistCheck.Checked = Rarekiller.Settings.BlacklistCheck;
            TBBlacklistTime.Text = Rarekiller.Settings.BlacklistTime;
            //Misc
            CBGroundMount.Checked = Rarekiller.Settings.GroundMountMode;
            CBZoneSave.Checked = Rarekiller.Settings.ZoneSave;
			CBAlert.Checked = Rarekiller.Settings.Alert;
            CBTagged.Checked = Rarekiller.Settings.DontKillTagged;
            CBWinnerTagged.Checked = Rarekiller.Settings.WinnerTagged;
            CBBlacklistTagged.Checked = Rarekiller.Settings.BlacklistTagged;
            //Security
            CBWisper.Checked = Rarekiller.Settings.Wisper;
            CBGuild.Checked = Rarekiller.Settings.Guild;
            CBKeyer.Checked = Rarekiller.Settings.Keyer;
			TBSoundfileWisper.Text = Rarekiller.Settings.SoundfileWisper;
            TBSoundfileGuild.Text = Rarekiller.Settings.SoundfileGuild;
            TBSoundfileFoundRare.Text = Rarekiller.Settings.SoundfileFoundRare;
            //LogOut
            RBKillWoW.Checked = Rarekiller.Settings.KillWoW;
            RBLogOut.Checked = Rarekiller.Settings.LogOut;
            RBJustAlert.Checked = Rarekiller.Settings.JustAlert;
            TBMessage1.Text = Rarekiller.Settings.Message1;
            TBMessage2.Text = Rarekiller.Settings.Message2;
            CBPlayerFollow.Checked = Rarekiller.Settings.PlayerFollows;
            CBPlayerFollowLogout.Checked = Rarekiller.Settings.PlayerFollowsLogout;
            TBPlayerFollows.Text = Rarekiller.Settings.PlayerFollowsTime;
            CBContinentChange.Checked = Rarekiller.Settings.ContinentChange;
			CBLootSuccess.Checked = Rarekiller.Settings.LootSuccess;
			//Screenshot
			CBScreenAeonaxx.Checked = Rarekiller.Settings.ScreenAeonaxx;
			CBScreenCamel.Checked = Rarekiller.Settings.ScreenCamel;
			CBScreenRarekiller.Checked = Rarekiller.Settings.ScreenRarekiller;
			CBScreenTamer.Checked = Rarekiller.Settings.ScreenTamer;
			CBScreenFound.Checked = Rarekiller.Settings.ScreenFound;
			CBScreenSuccess.Checked = Rarekiller.Settings.ScreenSuccess;
            //Tamer
            CBTameDefault.Checked = Rarekiller.Settings.TameDefault;
            CBTameByID.Checked = Rarekiller.Settings.TameByID;
            CBNotKillTameable.Checked = Rarekiller.Settings.NotKillTameable;
            TBTameID.Text = Rarekiller.Settings.TameMobID;
            CBTameDefault.Enabled = Rarekiller.Settings.Hunteractivated;
            CBTameByID.Enabled = Rarekiller.Settings.Hunteractivated;
            TBTameID.Enabled = Rarekiller.Settings.Hunteractivated;
            // Slowfall
            CBUseSlowfall.Checked = Rarekiller.Settings.UseSlowfall;
            RBCloak.Checked = Rarekiller.Settings.Cloak;
            CBItem.Checked = Rarekiller.Settings.Item;
            TBSlowfallItem.Text = Rarekiller.Settings.SlowfallItem;
            CBSpell.Checked = Rarekiller.Settings.Spell;
            TBSlowfallSpell.Text = Rarekiller.Settings.SlowfallSpell;
            TBFalltimer.Text = Rarekiller.Settings.Falltimer;
            // Pullspell
            CBPull.Checked = Rarekiller.Settings.DefaultPull;
            TBPull.Text = Rarekiller.Settings.Pull;
			TBRange.Text = Rarekiller.Settings.Range;
            CBVyragosa.Checked = Rarekiller.Settings.Vyragosa;
            CBShotVyra.Checked = Rarekiller.Settings.ShotVyra;
            CBBlazewing.Checked = Rarekiller.Settings.Blazewing;
            CBBloodseekerSearch.Checked = Rarekiller.Settings.BloodseekerSearch;
            CBBloodseekerKill.Checked = Rarekiller.Settings.BloodseekerKill;
            //Testpart
            CBAeonaxx.Checked = Rarekiller.Settings.Aeonaxx;
            CBTestKillAeonaxx.Checked = Rarekiller.Settings.TestKillAeonaxx;
            CBCamel.Checked = Rarekiller.Settings.Camel;
            CBCollect.Checked = Rarekiller.Settings.Collect;
            CBDormus.Checked = Rarekiller.Settings.Dormus;
            //Developer Box
            CBTestRaptorNest.Enabled = Rarekiller.Settings.DeveloperBoxActive;
            CBTestCamel.Enabled = Rarekiller.Settings.DeveloperBoxActive;
            CBTestDormus.Enabled = Rarekiller.Settings.DeveloperBoxActive;
            CBTestAeonaxx.Enabled = Rarekiller.Settings.DeveloperBoxActive;
            CBTestWelpTargeting.Enabled = Rarekiller.Settings.DeveloperBoxActive;
            CBTestShootDown.Enabled = Rarekiller.Settings.DeveloperBoxActive;
            CBTestLogoutItem.Enabled = Rarekiller.Settings.DeveloperBoxActive;
            CBTestRaptorNest.Checked = Rarekiller.Settings.TestRaptorNest;
            CBTestCamel.Checked = Rarekiller.Settings.TestFigurineInteract;
            CBTestDormus.Checked = Rarekiller.Settings.TestKillDormus;
            CBTestAeonaxx.Checked = Rarekiller.Settings.TestMountAeonaxx;
            CBTestWelpTargeting.Checked = Rarekiller.Settings.TestWelpTargeting;
            CBTestShootDown.Checked = Rarekiller.Settings.TestShootMob;
            CBTestLogoutItem.Checked = Rarekiller.Settings.TestLogoutItem;
        }
		
        private void RBItem_CheckedChanged(object sender, EventArgs e)
        {
            if (!CBItem.Checked)
            {
                TBSlowfallItem.Text = "";
                TBSlowfallItem.Enabled = false;
            }
            else
            {
                TBSlowfallItem.Text = Rarekiller.Settings.SlowfallItem;
                TBSlowfallItem.Enabled = true;
            }

        }
        private void CBSpell_CheckedChanged(object sender, EventArgs e)
        {
            if (!CBSpell.Checked)
            {
                TBSlowfallSpell.Text = "";
                TBSlowfallSpell.Enabled = false;
            }
            else
            {
                TBSlowfallSpell.Text = Rarekiller.Settings.SlowfallSpell;
                TBSlowfallSpell.Enabled = true;
            }
        }

        private void CBHuntByID_CheckedChanged(object sender, EventArgs e)
        {
            if (CBHuntByID.Checked)
            {
                TBHuntByID.Enabled = true;
                TBHuntByID.Text = Rarekiller.Settings.MobID;
            }
            else
            {
                TBHuntByID.Enabled = false;
                TBHuntByID.Text = "";
            }
        }

        private void CBTameID_CheckedChanged(object sender, EventArgs e)
        {
            if (CBTameByID.Checked)
            {
                TBTameID.Enabled = true;
                TBTameID.Text = Rarekiller.Settings.TameMobID;
            }
            else
            {
                TBTameID.Enabled = false;
                TBTameID.Text = "";
            }

        }
        private void CBVyragosa_CheckedChanged(object sender, EventArgs e)
        {
            if (!CBVyragosa.Checked)
            {
				CBShotVyra.Checked = false;
				CBShotVyra.Enabled = false;
			}
            else
            {
				CBShotVyra.Enabled = true;
				CBShotVyra.Checked = Rarekiller.Settings.ShotVyra;
            }
        }

        private void CBPull_CheckedChanged(object sender, EventArgs e)
        {
            if (!CBPull.Checked)
			{
                TBPull.Enabled = true;
				TBPull.Text = Rarekiller.Settings.Pull;
            }
			else
            {
                TBPull.Text = "";
                TBPull.Enabled = false;
            }

        }

        private void BAlertTest_Click(object sender, EventArgs e)
        {
            new SoundPlayer(Rarekiller.Soundfile).Play();
        }

        private void CBUseSlowfall_CheckedChanged(object sender, EventArgs e)
        {
            if (!CBUseSlowfall.Checked)
            {
                CBSpell.Checked = false;
                CBItem.Checked = false;
				RBCloak.Checked = false;
                CBItem.Enabled = false;
                CBSpell.Enabled = false;
                RBCloak.Enabled = false; 
                TBSlowfallSpell.Text = "";
                TBSlowfallSpell.Enabled = false;
                TBSlowfallItem.Text = "";
                TBSlowfallItem.Enabled = false;
            }
            else
            {
				RBCloak.Checked = Rarekiller.Settings.Cloak;
				CBItem.Checked = Rarekiller.Settings.Item;
				TBSlowfallItem.Text = Rarekiller.Settings.SlowfallItem;
				CBSpell.Checked = Rarekiller.Settings.Spell;
				TBSlowfallSpell.Text = Rarekiller.Settings.SlowfallSpell;
                CBItem.Enabled = true;
                CBSpell.Enabled = true;
                RBCloak.Enabled = true; 
                TBSlowfallSpell.Enabled = true;
                TBSlowfallItem.Enabled = true;
            }
        }

        private void RBLogOut_CheckedChanged(object sender, EventArgs e)
        {
            if (RBLogOut.Checked)
            {
                TBMessage1.Enabled = true;
                TBMessage2.Enabled = true;
                TBMessage1.Text = Rarekiller.Settings.Message1;
                TBMessage2.Text = Rarekiller.Settings.Message2;
            }
            else
            {
                TBMessage1.Enabled = false;
                TBMessage2.Enabled = false;
                TBMessage1.Text = "";
                TBMessage2.Text = "";
            }
        }
		

        private void BSave_Click(object sender, EventArgs e)
        {
//----------------- Save Configfile and set Settings ---------------- 
            XmlDocument xml;
            XmlElement root;
            XmlElement element;
            XmlText text;
            XmlComment xmlComment;

// ---------- Check Plausibility of the Config ----------------------------

//Pull Spell
            if (!CBPull.Checked && !(SpellManager.HasSpell(TBPull.Text)))
            {
                Logging.Write(System.Drawing.Color.Red, "Rarekiller Part Spells: Don't have your configured Pull Spell - setting to Default");
                CBPull.Checked = true;
                TBPull.Text = "";
            }
            if (!CBPull.Checked && (TBPull.Text == ""))
            {
                Logging.Write(System.Drawing.Color.Red, "Rarekiller: You must insert Pullspell");
                CBPull.Checked = true;
            }
            if (Convert.ToInt32(TBRange.Text) > 40)
            {
                Logging.Write(System.Drawing.Color.Red, "Rarekiller: Set Range to 25, over 40 is impossible");
                TBRange.Text = "25";
            }
            if (CBPull.Checked && SpellManager.HasSpell("Crusader Strike") && (Convert.ToInt32(TBRange.Text) > 3))
            {
                Logging.Write(System.Drawing.Color.Red, "Rarekiller: Set Range to 3 because of Low Range of Default Pull Spell Crusader Strike");
                TBRange.Text = "3";
            }
            if (CBPull.Checked && SpellManager.HasSpell("Sinister Strike") && (Convert.ToInt32(Rarekiller.Settings.Range) > 3))
            {
                Logging.Write(System.Drawing.Color.Red, "Rarekiller: Set Range to 3 because of Low Range of Default Spell Sinister Strike");
                TBRange.Text = "3";
            }
            if ((CBWotlk.Checked || CBBC.Checked || CBCata.Checked || CBLowRAR.Checked || CBHuntByID.Checked
                || CBDormus.Checked || CBAeonaxx.Checked || CBTestWelpTargeting.Checked)
                && CBPull.Checked && !(SpellManager.HasSpell(Rarekiller.Spells.LowPullspell)))
            {
                Logging.Write(System.Drawing.Color.Red, "Rarekiller Part Spells: Don't have your Pull Spell - please config one");
                CBWotlk.Checked = false;
                CBBC.Checked = false;
                CBCata.Checked = false;
                CBHuntByID.Checked = false;
                CBLowRAR.Checked = false;
                CBDormus.Checked = false;
                CBAeonaxx.Checked = false;
            }
            if (CBTLPD.Checked && CBPull.Checked
                && !(SpellManager.HasSpell(Rarekiller.Spells.FastPullspell)))
            {
                Logging.Write(System.Drawing.Color.Red, "Rarekiller Part Spells: Don't have a valid Pull Spell for TLPD - please check your Config");
                CBTLPD.Checked = false;
            }

//Hunt and Tame by ID
            if (CBHuntByID.Checked && (TBHuntByID.Text == ""))
            {
                Logging.Write(System.Drawing.Color.Red, "Rarekiller: You must insert the ID of the Mob you want to hunt");
                CBHuntByID.Checked = false;
            }
            if (CBTameByID.Checked && (TBTameID.Text == ""))
            {
                Logging.Write(System.Drawing.Color.Red, "Rarekiller: You must insert the ID of the Mob you want to tame");
                CBTameByID.Checked = false;
            }


            //Slowfall
            if (CBItem.Checked && (TBSlowfallItem.Text == ""))
            {
                Logging.Write(System.Drawing.Color.Red, "Rarekiller: You must insert a Slowfall Item");
                CBItem.Checked = false;
            }
            if (CBSpell.Checked && (TBSlowfallSpell.Text == ""))
            {
                Logging.Write(System.Drawing.Color.Red, "Rarekiller: You must insert a Slowfall Spell");
                CBSpell.Checked = false;
            }
            if (CBUseSlowfall.Checked && CBSpell.Checked && !(SpellManager.HasSpell(TBSlowfallSpell.Text)))
            {
                Logging.Write(System.Drawing.Color.Red, "Rarekiller: Don't have your Slowfall Spell - please check your Config");
                CBSpell.Checked = false;
                TBSlowfallSpell.Text = "";
            }
            if (CBTLPD.Checked && !CBUseSlowfall.Checked)
                Logging.Write(System.Drawing.Color.Red, "Rarekiller Warning: You will probably die, hunting the TLPD without Slowfall.");
            if (CBWotlk.Checked && CBVyragosa.Checked && !CBUseSlowfall.Checked)
                Logging.Write(System.Drawing.Color.Red, "Rarekiller Warning: You will probably die, hunting Vyragosa without Slowfall. Please Check - don't kill Vyragosa");
            if (TBFalltimer.Text == "")
                TBFalltimer.Text = "10";
//Diverses
            if (CBTestShootDown.Checked && (!SpellManager.HasSpell("Lightning Bolt")))
            {
                Logging.Write(System.Drawing.Color.Red, "Rarekiller: You don't have Lightning Bolt to use this");
                CBTestShootDown.Checked = false;
            }
            if (CBShotVyra.Checked && (!SpellManager.HasSpell("Shoot") || !SpellManager.HasSpell("Throw")))
            {
                Logging.Write(System.Drawing.Color.Red, "Rarekiller: You don't have Shoot or Throw to use this");
                CBShotVyra.Checked = false;
            }
			
			if (CBContinentChange.Checked && !RBKillWoW.Checked && !RBLogOut.Checked && !RBJustAlert.Checked)
                RBJustAlert.Checked = true;
            if (CBAeonaxx.Checked && CBGroundMount.Checked)
            {
                Logging.Write(System.Drawing.Color.Red, "Rarekiller: Can't hunt Aeonaxx in GroundMountMode");
                CBAeonaxx.Checked = false;
            }
            if (!CBContinentChange.Checked)
            {
                RBKillWoW.Checked = false;
                RBLogOut.Checked = false;
                RBJustAlert.Checked = false;
            }
            if (CBBlacklistCheck.Checked && (TBBlacklistTime.Text == ""))
            {
                TBBlacklistTime.Text = "180";
                Logging.Write(System.Drawing.Color.Red, "Rarekiller: Set Blacklist Time to Default");
            }
            if (CBTagged.Checked && !CBBlacklistTagged.Checked && !CBWinnerTagged.Checked)
            {
                Logging.Write(System.Drawing.Color.Red, "You have to define what he should do instead of killing tagged Mobs, set these Settings to default");
                CBWinnerTagged.Checked = true;
            }
            if (CBBloodseekerKill.Checked && !CBUseSlowfall.Checked)
            {
                CBBloodseekerKill.Checked = false;
                Logging.Write(System.Drawing.Color.Red, "Rarekiller: You can't hunt Bloodseeker without Slowfall (Setup in Tap 2).");
            }


// Variablen nach Settings übernehmen
            // Addons
            Rarekiller.Settings.CATA = CBCata.Checked;
            Rarekiller.Settings.WOTLK = CBWotlk.Checked;
            Rarekiller.Settings.BC = CBBC.Checked;
            Rarekiller.Settings.LowRAR = CBLowRAR.Checked;
            Rarekiller.Settings.TLPD = CBTLPD.Checked;
			Rarekiller.Settings.Poseidus = CBPoseidus.Checked;
            Rarekiller.Settings.RaptorNest = CBRaptorNest.Checked;
            // Hunt by ID
            Rarekiller.Settings.HUNTbyID = CBHuntByID.Checked;
            Rarekiller.Settings.MobID = TBHuntByID.Text;
            Rarekiller.Settings.BlacklistCheck = CBBlacklistCheck.Checked;
            Rarekiller.Settings.BlacklistTime = TBBlacklistTime.Text;
            Rarekiller.Settings.ZoneSave = CBZoneSave.Checked;
            //Tamer
            Rarekiller.Settings.TameDefault = CBTameDefault.Checked;
            Rarekiller.Settings.TameByID = CBTameByID.Checked;
            Rarekiller.Settings.NotKillTameable = CBNotKillTameable.Checked;
            Rarekiller.Settings.TameMobID = TBTameID.Text;
            // Slowfall
            Rarekiller.Settings.UseSlowfall = CBUseSlowfall.Checked;
            Rarekiller.Settings.Cloak = RBCloak.Checked;
            Rarekiller.Settings.Item = CBItem.Checked;
            Rarekiller.Settings.SlowfallItem = TBSlowfallItem.Text;
            Rarekiller.Settings.Spell = CBSpell.Checked;
            Rarekiller.Settings.SlowfallSpell = TBSlowfallSpell.Text;
            Rarekiller.Settings.Falltimer = TBFalltimer.Text;
            // Pullspell
            Rarekiller.Settings.DefaultPull = CBPull.Checked;
            Rarekiller.Settings.Pull = TBPull.Text;
            Rarekiller.Settings.Range = TBRange.Text;
            Rarekiller.Settings.Vyragosa = CBVyragosa.Checked;
            Rarekiller.Settings.ShotVyra = CBShotVyra.Checked;
            Rarekiller.Settings.Blazewing = CBBlazewing.Checked;
            Rarekiller.Settings.BloodseekerSearch = CBBloodseekerSearch.Checked;
            Rarekiller.Settings.BloodseekerKill = CBBloodseekerKill.Checked;
            //Testpart
            Rarekiller.Settings.Aeonaxx = CBAeonaxx.Checked;
            Rarekiller.Settings.TestKillAeonaxx = CBTestKillAeonaxx.Checked;
            Rarekiller.Settings.Camel = CBCamel.Checked;
            Rarekiller.Settings.Collect = CBCollect.Checked;
            Rarekiller.Settings.Dormus = CBDormus.Checked;
            //Alert etc
            Rarekiller.Settings.GroundMountMode = CBGroundMount.Checked;
            Rarekiller.Settings.Alert = CBAlert.Checked;
            Rarekiller.Settings.BlacklistTagged = CBBlacklistTagged.Checked;
            Rarekiller.Settings.WinnerTagged = CBWinnerTagged.Checked;
            Rarekiller.Settings.DontKillTagged = CBTagged.Checked;
            Rarekiller.Settings.Wisper = CBWisper.Checked;
            Rarekiller.Settings.Guild = CBGuild.Checked;
            Rarekiller.Settings.Keyer = CBKeyer.Checked;
            Rarekiller.Settings.SoundfileWisper = TBSoundfileWisper.Text;
            Rarekiller.Settings.SoundfileGuild = TBSoundfileGuild.Text;
            Rarekiller.Settings.SoundfileFoundRare = TBSoundfileFoundRare.Text;
            //Security
            Rarekiller.Settings.KillWoW = RBKillWoW.Checked;
            Rarekiller.Settings.LogOut = RBLogOut.Checked;
            Rarekiller.Settings.JustAlert = RBJustAlert.Checked;
            Rarekiller.Settings.Message1 = TBMessage1.Text;
            Rarekiller.Settings.Message2 = TBMessage2.Text;
            Rarekiller.Settings.PlayerFollows = CBPlayerFollow.Checked;
            Rarekiller.Settings.PlayerFollowsLogout = CBPlayerFollowLogout.Checked;
            Rarekiller.Settings.PlayerFollowsTime = TBPlayerFollows.Text;
            Rarekiller.Settings.ContinentChange = CBContinentChange.Checked;
			Rarekiller.Settings.LootSuccess = CBLootSuccess.Checked;
			//Screenshot
			Rarekiller.Settings.ScreenAeonaxx = CBScreenAeonaxx.Checked;
			Rarekiller.Settings.ScreenCamel = CBScreenCamel.Checked;
			Rarekiller.Settings.ScreenRarekiller = CBScreenRarekiller.Checked;
			Rarekiller.Settings.ScreenTamer = CBScreenTamer.Checked;
			Rarekiller.Settings.ScreenFound = CBScreenFound.Checked;
			Rarekiller.Settings.ScreenSuccess = CBScreenSuccess.Checked;
            //Developer Box
            Rarekiller.Settings.TestRaptorNest = CBTestRaptorNest.Checked; 
            Rarekiller.Settings.TestFigurineInteract = CBTestCamel.Checked;
            Rarekiller.Settings.TestKillDormus = CBTestDormus.Checked;
            Rarekiller.Settings.TestMountAeonaxx = CBTestAeonaxx.Checked;
            Rarekiller.Settings.TestWelpTargeting = CBTestWelpTargeting.Checked; 
            Rarekiller.Settings.TestShootMob = CBTestShootDown.Checked;
            Rarekiller.Settings.TestLogoutItem = CBTestLogoutItem.Checked;

            // Rarekiller
			Logging.WriteDebug("Rarekiller Save: CATA = {0}", CBCata.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: WOTLK = {0}", CBWotlk.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: BC = {0}", CBBC.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: LowRare = {0}", CBLowRAR.Checked.ToString());
			// Mount Rares
            Logging.WriteDebug("Rarekiller Save: TLPD = {0}", CBTLPD.Checked.ToString());
			Logging.WriteDebug("Rarekiller Save: Poseidus = {0}", CBPoseidus.Checked.ToString());
			Logging.WriteDebug("Rarekiller Save: Aeonaxx = {0}", CBAeonaxx.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: Testcase Rota Aeonaxx = {0}", CBTestKillAeonaxx.Checked.ToString());
			// Collector
            Logging.WriteDebug("Rarekiller Save: Camel = {0}", CBCamel.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: CollectOnce = {0}", CBCollect.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: Dormus = {0}", CBDormus.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: RaptorNest = {0}", CBRaptorNest.Checked.ToString());
			// Problem Mobs
            Logging.WriteDebug("Rarekiller Save: Vyragosa = {0}", CBVyragosa.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: ShotVyra = {0}", CBShotVyra.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: Blazewing = {0}", CBBlazewing.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: BloodseekerSearch = {0}", CBBloodseekerSearch.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: BloodseekerKill = {0}", CBBloodseekerKill.Checked.ToString());
            // Hunt by ID
            Logging.WriteDebug("Rarekiller Save: HuntByID = {0}", CBHuntByID.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: MobID = {0}", TBHuntByID.Text.ToString());
            // Tamer
            Logging.WriteDebug("Rarekiller Save: TameDefault = {0}", CBTameDefault.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: TameByID = {0}", CBTameByID.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: NotKillTameable = {0}", CBNotKillTameable.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: TameID = {0}", TBTameID.Text.ToString());
            // Pullspell
            Logging.WriteDebug("Rarekiller Save: DefaultPull = {0}", CBPull.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: Spell = {0}", TBPull.Text.ToString());
            Logging.WriteDebug("Rarekiller Save: Range = {0}", TBRange.Text.ToString());
            // Slowfall
            Logging.WriteDebug("Rarekiller Save: UseSlowfall = {0}", CBUseSlowfall.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: Cloak = {0}", RBCloak.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: UseSlowfallItem = {0}", CBItem.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: SlowfallItem = {0}", TBSlowfallItem.Text.ToString());
            Logging.WriteDebug("Rarekiller Save: UseSlowfallSpell = {0}", CBSpell.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: SlowfallSpell = {0}", TBSlowfallSpell.Text.ToString());
            Logging.WriteDebug("Rarekiller Save: Falltimer = {0}", TBFalltimer.Text.ToString());
			// Miscs
			Logging.WriteDebug("Rarekiller Save: BlacklistCheck = {0}", CBBlacklistCheck.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: BlacklistTime = {0}", TBBlacklistTime.Text.ToString());
            Logging.WriteDebug("Rarekiller Save: ZoneSafe = {0}", CBZoneSave.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: GroundmountMode = {0}", CBGroundMount.Checked.ToString());
			Logging.WriteDebug("Rarekiller Save: MoveAround = {0}", CBKeyer.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: DontKillTagged = {0}", CBTagged.Checked.ToString());
			Logging.WriteDebug("Rarekiller Save: BlacklistTagged = {0}", CBBlacklistTagged.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: WinnerTagged = {0}", CBWinnerTagged.Checked.ToString());
            // Screenshot
            Logging.WriteDebug("Rarekiller Save: ScreenAeonaxx = {0}", CBScreenAeonaxx.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: ScreenCamel = {0}", CBScreenCamel.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: ScreenRarekiller = {0}", CBScreenRarekiller.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: ScreenTamer = {0}", CBScreenTamer.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: ScreenFound = {0}", CBScreenFound.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: ScreenSuccess = {0}", CBScreenSuccess.Checked.ToString());
			// Alerts
            Logging.WriteDebug("Rarekiller Save: Alert = {0}", CBAlert.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: Wisper = {0}", CBWisper.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: Guild = {0}", CBGuild.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: FileWisper = {0}", TBSoundfileWisper.Text.ToString());
            Logging.WriteDebug("Rarekiller Save: FileGuild = {0}", TBSoundfileGuild.Text.ToString());
            Logging.WriteDebug("Rarekiller Save: FileFound = {0}", TBSoundfileFoundRare.Text.ToString());
            // Security
            Logging.WriteDebug("Rarekiller Save: ContinentChange = {0}", CBContinentChange.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: JustAlert = {0}", RBJustAlert.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: KillWoW = {0}", RBKillWoW.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: LogOut = {0}", RBLogOut.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: Message1 = {0}", TBMessage1.Text.ToString());
            Logging.WriteDebug("Rarekiller Save: Message2 = {0}", TBMessage2.Text.ToString());
            Logging.WriteDebug("Rarekiller Save: PlayerFollowedHeart = {0}", CBPlayerFollow.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: PlayerFollowLogout = {0}", CBPlayerFollowLogout.Checked.ToString());
            Logging.WriteDebug("Rarekiller Save: PlayerFollowsTime = {0}", TBPlayerFollows.Text.ToString());
			Logging.WriteDebug("Rarekiller Save: LootSuccess = {0}", CBLootSuccess.Checked.ToString());

// ---------- Save ----------------------------			

            string sPath = Path.Combine(Rarekiller.FolderPath, "config\\");

            if (!Directory.Exists(sPath))
            {
                Logging.WriteDebug("Rarekiller: Creating config directory");
                Directory.CreateDirectory(sPath);
            }

            sPath = Path.Combine(sPath, "Rarekiller.config");

            Logging.WriteDebug("Rarekiller: Saving config file: {0}", sPath);
            Logging.Write("Rarekiller: Settings Saved");
            xml = new XmlDocument();
            XmlDeclaration dc = xml.CreateXmlDeclaration("1.0", "utf-8", null);
            xml.AppendChild(dc);

            xmlComment = xml.CreateComment(
                "=======================================================================\n" +
                ".CONFIG  -  This is the Config File For Rarekiller\n\n" +
                "XML file containing settings to customize in the Rarekiller Plugin\n" +
                "It is STRONGLY recommended you use the Configuration UI to change this\n" +
                "file instead of direct changein it here.\n" +
                "========================================================================");

            //let's add the root element
            root = xml.CreateElement("Rarekiller");
            root.AppendChild(xmlComment);

			//Rarekiller
            //let's add another element (child of the root)
            element = xml.CreateElement("CATA");
            text = xml.CreateTextNode(CBCata.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("WOTLK");
            text = xml.CreateTextNode(CBWotlk.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("BC");
            text = xml.CreateTextNode(CBBC.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("LowRAR");
            text = xml.CreateTextNode(CBLowRAR.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			
			//Rare Mounts
            //let's add another element (child of the root)
            element = xml.CreateElement("TLPD");
            text = xml.CreateTextNode(CBTLPD.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			//let's add another element (child of the root)
            element = xml.CreateElement("Poseidus");
            text = xml.CreateTextNode(CBPoseidus.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Aeonaxx");
            text = xml.CreateTextNode(CBAeonaxx.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("AeonaxxTestcaseTrainingDummy");
            text = xml.CreateTextNode(CBTestKillAeonaxx.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

			//Collector
            //let's add another element (child of the root)
            element = xml.CreateElement("Camel");
            text = xml.CreateTextNode(CBCamel.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Collect");
            text = xml.CreateTextNode(CBCollect.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Dormus");
            text = xml.CreateTextNode(CBDormus.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("RaptorNest");
            text = xml.CreateTextNode(CBRaptorNest.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			
			//Problem Mobs
            //let's add another element (child of the root)
            element = xml.CreateElement("Blazewing");
            text = xml.CreateTextNode(CBBlazewing.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);			
			//let's add another element (child of the root)
            element = xml.CreateElement("Vyragosa");
            text = xml.CreateTextNode(CBVyragosa.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("ShootVyra");
            text = xml.CreateTextNode(CBShotVyra.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("BloodseekerSearch");
            text = xml.CreateTextNode(CBBloodseekerSearch.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("BloodseekerKill");
            text = xml.CreateTextNode(CBBloodseekerKill.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

			//Hunt by ID
            //let's add another element (child of the root)
            element = xml.CreateElement("HuntByID");
            text = xml.CreateTextNode(CBHuntByID.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("MobID");
            text = xml.CreateTextNode(TBHuntByID.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			
            //Tamer
            //let's add another element (child of the root)
            element = xml.CreateElement("NotKillTameable");
            text = xml.CreateTextNode(CBNotKillTameable.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("TameDefault");
            text = xml.CreateTextNode(CBTameDefault.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("TameByID");
            text = xml.CreateTextNode(CBTameByID.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("TameMobID");
            text = xml.CreateTextNode(TBTameID.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			
			//Pullspell
            //let's add another element (child of the root)
            element = xml.CreateElement("DefaultPull");
            text = xml.CreateTextNode(CBPull.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Pull");
            text = xml.CreateTextNode(TBPull.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Range");
            text = xml.CreateTextNode(TBRange.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

			//Slowfall
            //let's add another element (child of the root)
            element = xml.CreateElement("UseSlowfall");
            text = xml.CreateTextNode(CBUseSlowfall.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Spell");
            text = xml.CreateTextNode(CBSpell.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("SlowfallSpell");
            text = xml.CreateTextNode(TBSlowfallSpell.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Cloak");
            text = xml.CreateTextNode(RBCloak.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Item");
            text = xml.CreateTextNode(CBItem.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("SlowfallItem");
            text = xml.CreateTextNode(TBSlowfallItem.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Falltimer");
            text = xml.CreateTextNode(TBFalltimer.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

			//Miscs
            //let's add another element (child of the root)
            element = xml.CreateElement("GroundMountMode");
            text = xml.CreateTextNode(CBGroundMount.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			//let's add another element (child of the root)
            element = xml.CreateElement("Keyer");
            text = xml.CreateTextNode(CBKeyer.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //Tagged
            //let's add another element (child of the root)
            element = xml.CreateElement("DontKillTagged");
            text = xml.CreateTextNode(CBTagged.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("BlacklistTagged");
            text = xml.CreateTextNode(CBBlacklistTagged.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("WinnerTagged");
            text = xml.CreateTextNode(CBWinnerTagged.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			
			//Blacklist Mobs
            //let's add another element (child of the root)
            element = xml.CreateElement("ZoneSave");
            text = xml.CreateTextNode(CBZoneSave.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			//let's add another element (child of the root)
            element = xml.CreateElement("BlacklistCheck");
            text = xml.CreateTextNode(CBBlacklistCheck.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("BlacklistTime");
            text = xml.CreateTextNode(TBBlacklistTime.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			
			//Screens
			//let's add another element (child of the root)
            element = xml.CreateElement("ScreenAeonaxx");
            text = xml.CreateTextNode(CBScreenAeonaxx.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			//let's add another element (child of the root)
            element = xml.CreateElement("ScreenCamel");
            text = xml.CreateTextNode(CBScreenCamel.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			//let's add another element (child of the root)
            element = xml.CreateElement("ScreenRarekiller");
            text = xml.CreateTextNode(CBScreenRarekiller.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			//let's add another element (child of the root)
            element = xml.CreateElement("ScreenTamer");
            text = xml.CreateTextNode(CBScreenTamer.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			//let's add another element (child of the root)
            element = xml.CreateElement("ScreenFound");
            text = xml.CreateTextNode(CBScreenFound.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			//let's add another element (child of the root)
            element = xml.CreateElement("ScreenSuccess");
            text = xml.CreateTextNode(CBScreenSuccess.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);

            //Alerts
			//let's add another element (child of the root)
            element = xml.CreateElement("Alert");
            text = xml.CreateTextNode(CBAlert.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Wisper");
            text = xml.CreateTextNode(CBWisper.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("GuildChat");
            text = xml.CreateTextNode(CBGuild.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			//let's add another element (child of the root)
            element = xml.CreateElement("SoundfileFoundRare");
            text = xml.CreateTextNode(TBSoundfileFoundRare.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			//let's add another element (child of the root)
            element = xml.CreateElement("SoundfileWisper");
            text = xml.CreateTextNode(TBSoundfileWisper.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			//let's add another element (child of the root)
            element = xml.CreateElement("SoundfileGuild");
            text = xml.CreateTextNode(TBSoundfileGuild.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			
            //Security
			//let's add another element (child of the root)
            element = xml.CreateElement("ContinentChange");
            text = xml.CreateTextNode(CBContinentChange.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("KillWoW");
            text = xml.CreateTextNode(RBKillWoW.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("JustAlert");
            text = xml.CreateTextNode(RBJustAlert.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("LogOut");
            text = xml.CreateTextNode(RBLogOut.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Message1");
            text = xml.CreateTextNode(TBMessage1.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("Message2");
            text = xml.CreateTextNode(TBMessage2.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			//let's add another element (child of the root)
            element = xml.CreateElement("PlayerFollows");
            text = xml.CreateTextNode(CBPlayerFollow.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("PlayerFollowsLogout");
            text = xml.CreateTextNode(CBPlayerFollowLogout.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
            //let's add another element (child of the root)
            element = xml.CreateElement("PlayerFollowsTime");
            text = xml.CreateTextNode(TBPlayerFollows.Text.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			//let's add another element (child of the root)
            element = xml.CreateElement("LootSuccess");
            text = xml.CreateTextNode(CBLootSuccess.Checked.ToString());
            element.AppendChild(text);
            root.AppendChild(element);
			
            xml.AppendChild(root);

            System.IO.FileStream fs = new System.IO.FileStream(@sPath, System.IO.FileMode.Create,
                                                               System.IO.FileAccess.Write);
            try
            {
                xml.Save(fs);
                fs.Close();
            }
            catch (Exception np)
            {
                Logging.Write(System.Drawing.Color.Red, np.Message);
            }
        }

        private void BSoundfileFoundRare_Click(object sender, EventArgs e)
        {
            string sPath = Path.Combine(Rarekiller.FolderPath, "Sounds\\");
            
            var loadSoundfileFoundRare = new OpenFileDialog
            {
                Filter = "WAVE File (*.wav)|*.wav|WAVE File (*.wave)|*.wave",
                Title = "Select wav file to load",
                InitialDirectory = sPath
             };

            if (loadSoundfileFoundRare.ShowDialog() == DialogResult.OK)
            {
                string fileName1 = loadSoundfileFoundRare.FileName;
                if (!string.IsNullOrEmpty(fileName1))
                {
                    TBSoundfileFoundRare.Text = fileName1;
                }
            }
        }

        private void BSoundfileWisper_Click(object sender, EventArgs e)
        {
            string sPath = Path.Combine(Rarekiller.FolderPath, "Sounds\\");

            var loadSoundfileWisper = new OpenFileDialog
            {
                Filter = "WAVE File (*.wav)|*.wav|WAVE File (*.wave)|*.wave",
                Title = "Select wav file to load",
                InitialDirectory = sPath
            };

            if (loadSoundfileWisper.ShowDialog() == DialogResult.OK)
            {
                string fileName2 = loadSoundfileWisper.FileName;
                if (!string.IsNullOrEmpty(fileName2))
                {
                    TBSoundfileWisper.Text = fileName2;
                }
            }
        }

        private void BSoundfileGuild_Click(object sender, EventArgs e)
        {
            string sPath = Path.Combine(Rarekiller.FolderPath, "Sounds\\");

            var loadSoundfileGuild = new OpenFileDialog
            {
                Filter = "WAVE File (*.wav)|*.wav|WAVE File (*.wave)|*.wave",
                Title = "Select wav file to load",
                InitialDirectory = sPath
            };

            if (loadSoundfileGuild.ShowDialog() == DialogResult.OK)
            {
                string fileName3 = loadSoundfileGuild.FileName;
                if (!string.IsNullOrEmpty(fileName3))
                {
                    TBSoundfileGuild.Text = fileName3;
                }
            }
        }

        private void CBTagged_CheckedChanged(object sender, EventArgs e)
        {
            if (!CBTagged.Checked)
            {
                CBWinnerTagged.Checked = false;
                CBBlacklistTagged.Checked = false;
                CBWinnerTagged.Enabled = false;
                CBBlacklistTagged.Enabled = false;
            }
            else
            {
                CBWinnerTagged.Checked = Rarekiller.Settings.WinnerTagged;
                CBBlacklistTagged.Checked = Rarekiller.Settings.BlacklistTagged;
                CBWinnerTagged.Enabled = true;
                CBBlacklistTagged.Enabled = true;
            }
        }

        private void CBWinnerTagged_CheckedChanged(object sender, EventArgs e)
        {
            if (CBWinnerTagged.Checked)
                CBBlacklistTagged.Checked = false;
        }

        private void CBBlacklistTagged_CheckedChanged(object sender, EventArgs e)
        {
            if (CBBlacklistTagged.Checked)
                CBWinnerTagged.Checked = false;

        }

        private void CBBloodseekerSearch_CheckedChanged(object sender, EventArgs e)
        {
            if (CBBloodseekerSearch.Checked)
            {
                CBBloodseekerKill.Enabled = true;
                CBBloodseekerKill.Checked = Rarekiller.Settings.BloodseekerKill;
            }
            else
            {
                CBBloodseekerKill.Enabled = false;
                CBBloodseekerKill.Checked = false;
            }
        }
    }
}
