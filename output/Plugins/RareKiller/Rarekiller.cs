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
using System.Media;
using System.Net;
using System.Globalization;

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
	class Rarekiller: HBPlugin
	{
		public static string name { get { return "Rare Mob Killer"; } }
		public override string Name { get { return name; } }
		public override string Author { get { return "katzerle"; } }
		private readonly static Version _version = new Version(3, 3);
		public override Version Version { get { return _version; } }
		public override string ButtonText { get { return "Settings"; } }
		public override bool WantButton { get { return true; } }
		
		public static LocalPlayer Me = ObjectManager.Me;
        public static RarekillerSettings Settings = new RarekillerSettings();
        public static RarekillerSlowfall Slowfall = new RarekillerSlowfall();
        public static RarekillerKiller Killer = new RarekillerKiller();
        public static RarekillerCamel Camel = new RarekillerCamel();
        public static RarekillerAeonaxx Aeonaxx = new RarekillerAeonaxx();
        public static RarekillerTamer Tamer = new RarekillerTamer();
        public static RarekillerRaptorNest RaptorNest = new RarekillerRaptorNest();
        public static RarekillerSecurity Security = new RarekillerSecurity();
        public static RarekillerPlayerManager PlayerManager = new RarekillerPlayerManager();
		public static RarekillerSpells Spells = new RarekillerSpells();

        public static RotaHunter RotaHunter = new RotaHunter();
        public static RotaDruid RotaDruid = new RotaDruid();
        public static RotaDK RotaDK = new RotaDK();
        public static RotaPriest RotaPriest = new RotaPriest();
        public static RotaMage RotaMage = new RotaMage();
        public static RotaWarrior RotaWarrior = new RotaWarrior();
        public static RotaPaladin RotaPaladin = new RotaPaladin();
        public static RotaWarlock RotaWarlock = new RotaWarlock();
        public static RotaShaman RotaShaman = new RotaShaman();
        public static RotaRogue RotaRogue = new RotaRogue();

        private static Stopwatch SWPlayerFollows = new Stopwatch();
        private static Stopwatch Checktimer = new Stopwatch();
        public static Random rnd = new Random();

		public static Dictionary<Int32, string> BlacklistMobsList = new Dictionary<Int32, string>();
        public static Dictionary<Int32, string> TameableMobsList = new Dictionary<Int32, string>();
        public static Dictionary<string, WoWPoint> ComplicatedFigurinesList = new Dictionary<string, WoWPoint>();
        public static List<string> TrustPlayersList = new List<string>();
		
        public static bool hasItBeenInitialized = false; 
        Int32 MoveTimer = 100;
		
        public Rarekiller()
        {
			UpdatePlugin();
            Settings.Load();
            Logging.Write(System.Drawing.Color.MediumPurple, "Rare Mob Killer - Version 3.3 Loaded.");
            if (Me.Class != WoWClass.Hunter)
            {
                Logging.Write("Rarekiller Part Tamer: I'm no Hunter. Deactivate the Tamer Part");
                Settings.TameByID = false;
                Settings.TameDefault = false;
                Settings.TameMobID = "";
                Settings.Hunteractivated = false;
            }
            if (Settings.DeveloperBoxActive)
                Logging.Write("Rarekiller: Developerpart activated");
			if (Settings.WispersForBuddyCenter)
				Logging.Write("Rarekiller: Wispers for Buddycenter activated");
			if (Settings.DeveloperLogs)
				Logging.Write("Rarekiller: Developer Logspam activated");
        }

        static private void BotStopped(EventArgs args)
        {
            if (Rarekiller.Settings.Wisper)
            {
                WoWChat.Whisper -= Security.newWhisper;
                Lua.Events.DetachEvent("CHAT_MSG_BN_WHISPER", Security.BNWhisper);
            }
            if (Rarekiller.Settings.Guild)
            {
                WoWChat.Guild -= Security.newGuild;
                WoWChat.Officer -= Security.newOfficer;
            }
			BotEvents.OnBotStopped -= BotStopped;
            BlacklistMobsList.Clear();
            TameableMobsList.Clear();
            hasItBeenInitialized = false;
        }
		
        public override void OnButtonPress()
        {
            Settings.Load();
            ConfigForm.ShowDialog();
        }

        private Form MyForm;
        public Form ConfigForm
        {
            get
            {
                if(MyForm == null)
                    MyForm = new RarekillerUI();
                return MyForm;
            }
        }
		
		static public void Initialize()
        {
// Register the events of the Start/Stop Button in HB
			BotEvents.OnBotStopped += BotStopped;
			
// Zone Changer
			if ((Rarekiller.Settings.LogOut || Rarekiller.Settings.KillWoW || Rarekiller.Settings.JustAlert) && Rarekiller.Settings.ContinentChange)
            {
                Rarekiller.Settings.Zone = Me.RealZoneText;
                Rarekiller.Settings.Continent = Me.MapName;
                Rarekiller.Settings.ContinentID = Me.MapId;
                Logging.Write(System.Drawing.Color.Red, "Rarekiller: You are now bound to Continent {0} Continent ID {1}, don't leave it with running Bot", Rarekiller.Settings.Continent, Rarekiller.Settings.ContinentID);
            }
//Alerts Wisper and Guild
            if (Rarekiller.Settings.Wisper)
            {
                WoWChat.Whisper += Security.newWhisper;
                Lua.Events.AttachEvent("CHAT_MSG_BN_WHISPER", Security.BNWhisper);
            }
            if (Rarekiller.Settings.Guild)
            {
                WoWChat.Guild += Security.newGuild;
                WoWChat.Officer += Security.newOfficer;
            }
//Blacklisted Mobs to List
            XmlDocument BlacklistMobsXML = new XmlDocument();
			string sPath = Path.Combine(FolderPath, "config\\BlacklistedMobs.xml"); 
			System.IO.FileStream fs = new System.IO.FileStream(@sPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
			try
            {
                BlacklistMobsXML.Load(fs);
                fs.Close();
            }
            catch (Exception e)
            {
                Logging.Write(System.Drawing.Color.Red, e.Message);
                fs.Close();
                return;
            }
            XmlElement root = BlacklistMobsXML.DocumentElement;
            foreach (XmlNode Node in root.ChildNodes)
			{
                Int32 Entry = Convert.ToInt32(Node.Attributes["Entry"].InnerText);
				string Name = Node.Attributes["Name"].InnerText;
				BlacklistMobsList.Add(Entry, Name);
			}
//Tameable Mobs to List
            XmlDocument TameableMobsXML = new XmlDocument();
            string sPath2 = Path.Combine(FolderPath, "config\\TameableMobs.xml");
            System.IO.FileStream fs2 = new System.IO.FileStream(@sPath2, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
            try
            {
                TameableMobsXML.Load(fs2);
                fs2.Close();
            }
            catch (Exception e)
            {
                Logging.Write(System.Drawing.Color.Red, e.Message);
                fs2.Close();
                return;
            }
            XmlElement root2 = TameableMobsXML.DocumentElement;
            foreach (XmlNode TameMob in root2.ChildNodes)
            {
                Int32 Entry2 = Convert.ToInt32(TameMob.Attributes["Entry"].InnerText);
                string Name2 = TameMob.Attributes["Name"].InnerText;
                TameableMobsList.Add(Entry2, Name2);
            }

//Trust Players to List
            XmlDocument TrustPlayersXML = new XmlDocument();
            string sPath3 = Path.Combine(FolderPath, "config\\TrustPlayers.xml");
            System.IO.FileStream fs3 = new System.IO.FileStream(@sPath3, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
            try
            {
                TrustPlayersXML.Load(fs3);
                fs2.Close();
            }
            catch (Exception e)
            {
                Logging.Write(System.Drawing.Color.Red, e.Message);
                fs3.Close();
                return;
            }
            XmlElement root3 = TrustPlayersXML.DocumentElement;
            foreach (XmlNode TrustPlayer in root3.ChildNodes)
            {
                string Name3 = TrustPlayer.Attributes["Name"].InnerText;
                TrustPlayersList.Add(Name3);
            }
			
//DormusAuras Init
            Settings.AuraToID = WoWSpell.FromId(94967).Name;
            Settings.AuraToID2 = WoWSpell.FromId(93568).Name;
            Settings.TestAuraToID = WoWSpell.FromId(87548).Name;
        }

        public override void Pulse()
		{
			try
			{
// ------------ Deactivate if not in Game etc
                if (Me == null || RoutineManager.Current == null || !ObjectManager.IsInGame)
                    return;

// ------------ Deactivate Plugin in BGs, Inis, while Casting and on Transport
                if (Battlegrounds.IsInsideBattleground || Me.IsInInstance)
				return;
				
// ------------ Part Init
                if (!hasItBeenInitialized)
                {
                    hasItBeenInitialized = true;
                    Initialize();
                }
// ------------ Start the Stopwatches
                if ((Settings.PlayerFollows || Settings.PlayerFollowsLogout) && !SWPlayerFollows.IsRunning)
                    SWPlayerFollows.Start();
                if (Settings.Keyer && !Checktimer.IsRunning)
                    Checktimer.Start();

// ------------ Part Slowfall
				if(Me.IsFalling && Settings.UseSlowfall)
				{
					Thread.Sleep(Convert.ToInt32(Rarekiller.Settings.Falltimer));
					if (Me.IsFalling && !Me.Dead && !Me.IsGhost)
						Slowfall.HelpFalling();
				}
// ------------ Part Ride Aeonaxx
                if ((Me.IsOnTransport && Settings.Aeonaxx) || Settings.TestWelpTargeting || Settings.TestKillAeonaxx)
                    Aeonaxx.KillAeonaxx();
					
				if (Me.ActiveAuras.ContainsKey(Settings.TestAuraToID) && Settings.TestKillDormus)
					Camel.AvoidDormusSpit(Settings.TestAuraToID);
					
// ------------ Part Rarekiller Aeonaxx hostile
				//if(Me.IsOnTransport && Settings.Aeonaxx)
				//	Killer.findAndKillMob();
					
				if (!inCombat)
				{
// ------------ Part Aeonaxx
                    if (!Me.IsOnTransport && (Settings.Aeonaxx || Settings.TestMountAeonaxx))
						Aeonaxx.MountAeonaxx();
// ------------ Part Rarekiller						
					if (Settings.WOTLK || Settings.BC || Settings.CATA || Settings.TLPD || Settings.LowRAR || Settings.HUNTbyID || Settings.Poseidus)
						Killer.findAndKillMob();

// ------------ Part The Tamer
					if ((Me.Class == WoWClass.Hunter) && (Rarekiller.Settings.TameDefault || Rarekiller.Settings.TameByID))
					{
						if (Me.HealthPercent > 30)
							Tamer.findAndTameMob();
					}

// ------------ Part Camel Figurine
					if (Settings.Camel || Settings.TestFigurineInteract)
						Camel.findAndPickupObject();
					if (Settings.Dormus || Settings.TestKillDormus)
						Camel.findAndKillDormus();

// ------------ Part Raptor Nest
					if (Settings.RaptorNest || Settings.TestRaptorNest)
						RaptorNest.findAndPickupNest();

// ------------ Part Security - Log Out
					if ((Settings.LogOut || Settings.KillWoW || Settings.JustAlert) && Settings.ContinentChange && (Me.MapId != Rarekiller.Settings.ContinentID))
						Security.LogMeOut("Continent Change");

					if (SWPlayerFollows.Elapsed.TotalSeconds > 20)
					{
						SWPlayerFollows.Reset();
						SWPlayerFollows.Start();
                        if (Settings.PlayerFollows || Settings.PlayerFollowsLogout)
						{
// ------------ Part Security - Player Followed
							PlayerManager.update();
							if (PlayerManager.needExit(Convert.ToInt64(Settings.PlayerFollowsTime)))
								Security.HeartMe("Followed for too long by player " + PlayerManager.FollowingPlayer.Name + ". Followed for " + PlayerManager.players[PlayerManager.FollowingPlayer] + " seconds!");
						}
					}

// ------------ Part Security - Keypresser
					if (Settings.Keyer)
					{
						if (!Checktimer.IsRunning || Checktimer.Elapsed.TotalSeconds > MoveTimer)
						{
							Checktimer.Reset();
							Checktimer.Start();
							MoveTimer = rnd.Next(90, 200);
							if(!Me.IsMoving)
								Security.Movearound();
						}
					}
					if(Settings.LootSuccess || Settings.TestLogoutItem)
						Security.LogoutForItem();
				}
				else if (inCombat)
				{
// ------------ Part Shoot Down Vyragosa
					if (Settings.ShotVyra || Settings.TestShootMob)
						Killer.ShootDownVyra();

// ------------ Part avoid Spit AOE of Dormus						
                    if ((Me.ActiveAuras.ContainsKey(Settings.AuraToID) || Me.ActiveAuras.ContainsKey(Settings.AuraToID2)) && Settings.Dormus)
                        Camel.AvoidDormusSpit(Settings.AuraToID);
				}
			}
			
			catch (ThreadAbortException) { }
			catch (Exception e)
			{
				Logging.Write(System.Drawing.Color.Red, "Rarekiller: Exception in Pulse: {0}", e);
			}
		}

// ------------ Misc Functions
		static public string Soundfile
        {
            get
            {
				string sPath = Path.Combine(Rarekiller.FolderPath, "Sounds\\siren.wav");
                return sPath;
            }
        }
		
		static public string FolderPath
        {
            get
            {
                string sPath = Process.GetCurrentProcess().MainModule.FileName;
                sPath = Path.GetDirectoryName(sPath);
                sPath = Path.Combine(sPath, "Plugins\\RareKiller\\");
                return sPath;
            }
        }
		
		static public void UpdatePlugin()
		{
			// Google Code was shut down in 2016 — remote update check permanently disabled.
		}

        static public bool inCombat
        {
            get
            {
                if (Me.Combat || Me.Dead || Me.IsGhost) return true;
                return false;
            }
        }
	}
}
