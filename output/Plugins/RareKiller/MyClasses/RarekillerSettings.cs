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
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Styx;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.WoWInternals;
using ObjectManager = Styx.WoWInternals.ObjectManager;


namespace katzerle
{

    // -------------- Settings -------------------
    class RarekillerSettings
    {
        public RarekillerSettings()
        {
            if (ObjectManager.Me != null)
                try
                {
                    Load();
                }
                catch (Exception e)
                {
                    Logging.Write(System.Drawing.Color.Red, e.Message);
                }
        }


        //Default Einstellungen GUI
        // Addons
        public bool CATA = true;
        public bool WOTLK = true;
        public bool BC = true;
        public bool LowRAR = false;
        public bool TLPD = true;
        public bool RaptorNest = true;
		public bool Poseidus = true;
		public bool Aeonaxx = false;
        public bool Camel = true;
        public bool Collect = true;
        public bool Dormus = false;
        // Hunt by ID
        public bool HUNTbyID = false;
        public string MobID = "";
        //Tamer
        public bool Hunteractivated = true;
        public bool NotKillTameable = false;
        public bool TameDefault = false;
        public bool TameByID = false;
        public string TameMobID = "";
        public Int32 Tamedistance = 12;
        // Slowfall
        public bool UseSlowfall = true;
        public bool Cloak = false;
        public bool Item = true;
        public bool Spell = false;
        public string Falltimer = "900";
        public string SlowfallSpell = "";
        public string SlowfallItem = "Snowfall Lager";
        // Pullspell
        public bool DefaultPull = true;
        public string Pull = "";
        public string Range = "25";
        public bool Vyragosa = true;
        public bool ShotVyra = false;
        public bool Blazewing = false;
        public bool BloodseekerSearch = false;
        public bool BloodseekerKill = false;
        //Misc
        public bool GroundMountMode = false;
        public bool ZoneSave = false;
        public string BlacklistTime = "180";
        public bool BlacklistCheck = true;
        public bool Alert = true;
        public bool BlacklistTagged = false;
        public bool DontKillTagged = false;
        public bool WinnerTagged = false;
        public bool Wisper = true;
        public bool Guild = false;
        public bool Keyer = true;
		public string SoundfileWisper = Rarekiller.Soundfile;
		public string SoundfileGuild = Rarekiller.Soundfile;
		public string SoundfileFoundRare = Rarekiller.Soundfile;
        //LogOut
        public bool ContinentChange = false;
        public bool KillWoW = false;
        public bool JustAlert = false;
        public bool LogOut = false;
        public string Message1 = "/s lol, why i'm now in";
        public string Message2 = "/g I log off, bb";
        public bool PlayerFollows = false;
        public bool PlayerFollowsLogout = false;
        public string PlayerFollowsTime = "180";
		public bool LootSuccess = false;
		//Screens
		public bool ScreenAeonaxx = true;
		public bool ScreenCamel = false;
		public bool ScreenRarekiller = false;
		public bool ScreenTamer = false;
		public bool ScreenFound = true;
		public bool ScreenSuccess = false;
        //Sonstiges
        public Int32 Level = 61;
        public string Continent = ObjectManager.Me.MapName;
        public string Zone = ObjectManager.Me.RealZoneText;
        public Int64 ContinentID = ObjectManager.Me.MapId;
        public Int64 Blacklist60 = 3600;
        public Int64 Blacklist15 = 900;
        public Int64 Blacklist5 = 300;
        public Int64 Blacklist2 = 120;
        public string AuraToID = "";
        public string AuraToID2 = "";
        public string TestAuraToID = "";
        //Developer Things
        public bool WispersForBuddyCenter = false;
        public bool DeveloperBoxActive = false;
        public bool DeveloperLogs = false;
        //Developer Testcases
        public bool TestRaptorNest = false;
        public bool TestFigurineInteract = false;
        public bool TestKillDormus = false;
        public bool TestMountAeonaxx = false;
        public bool TestWelpTargeting = false;
        public bool TestShootMob = false;
        public bool TestLogoutItem = false;
        public bool TestKillAeonaxx = false;


        // -------------- Load ConfigFile ---------------
        public void Load()
        {
            //    XmlTextReader reader;
            XmlDocument xml = new XmlDocument();
            XmlNode xvar;
			
            string sPath = Path.Combine(Rarekiller.FolderPath, "config\\");

            if (!Directory.Exists(sPath))
            {
                Logging.WriteDebug("Rarekiller: Creating config directory");
                Directory.CreateDirectory(sPath);
            }

            sPath = Path.Combine(sPath, "Rarekiller.config");

            Logging.WriteDebug("Rarekiller: Loading config file: {0}", sPath);
            System.IO.FileStream fs = new System.IO.FileStream(@sPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
            try
            {
                xml.Load(fs);
                fs.Close();
            }
            catch (Exception e)
            {
                Logging.WriteDebug(e.Message);
                Logging.Write("Rarekiller: Continuing with Default Config Values");
                fs.Close();
                return;
            }

            //            xml = new XmlDocument();

            try
            {
                //                xml.Load(reader);
                if (xml == null)
                    return;
                // Load Variables - Addons
                xvar = xml.SelectSingleNode("//Rarekiller/CATA");
                if (xvar != null)
                {
                    CATA = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + CATA.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/WOTLK");
                if (xvar != null)
                {
                    WOTLK = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + WOTLK.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/BC");
                if (xvar != null)
                {
                    BC = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + BC.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/LowRAR");
                if (xvar != null)
                {
                    LowRAR = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + LowRAR.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/TLPD");
                if (xvar != null)
                {
                    TLPD = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + TLPD.ToString());
                }
				xvar = xml.SelectSingleNode("//Rarekiller/Poseidus");
                if (xvar != null)
                {
                    Poseidus = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + Poseidus.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/RaptorNest");
                if (xvar != null)
                {
                    RaptorNest = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + RaptorNest.ToString());
                }

                xvar = xml.SelectSingleNode("//Rarekiller/HuntByID");
                if (xvar != null)
                {
                    HUNTbyID = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + HUNTbyID.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/MobID");
                if (xvar != null)
                {
                    MobID = Convert.ToString(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + MobID.ToString());
                }

                // Tamer
                xvar = xml.SelectSingleNode("//Rarekiller/NotKillTameable");
                if (xvar != null)
                {
                    NotKillTameable = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + NotKillTameable.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/TameDefault");
                if (xvar != null)
                {
                    TameDefault = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + TameDefault.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/TameByID");
                if (xvar != null)
                {
                    TameByID = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + TameByID.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/TameMobID");
                if (xvar != null)
                {
                    TameMobID = Convert.ToString(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + TameMobID.ToString());
                }

                // Load Variables - Slowfall
                xvar = xml.SelectSingleNode("//Rarekiller/UseSlowfall");
                if (xvar != null)
                {
                    UseSlowfall = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + UseSlowfall.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Spell");
                if (xvar != null)
                {
                    Spell = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + Spell.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/SlowfallSpell");
                if (xvar != null)
                {
                    SlowfallSpell = Convert.ToString(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + SlowfallSpell.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Cloak");
                if (xvar != null)
                {
                    Cloak = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + Cloak.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Item");
                if (xvar != null)
                {
                    Item = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + Item.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/SlowfallItem");
                if (xvar != null)
                {
                    SlowfallItem = Convert.ToString(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + SlowfallItem.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Falltimer");
                if (xvar != null)
                {
                    Falltimer = Convert.ToString(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + Falltimer.ToString());
                }

                // Load Variables - Pullspell
                xvar = xml.SelectSingleNode("//Rarekiller/DefaultPull");
                if (xvar != null)
                {
                    DefaultPull = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + DefaultPull.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Pull");
                if (xvar != null)
                {
                    Pull = Convert.ToString(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + Pull.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Range");
                if (xvar != null)
                {
                    Range = Convert.ToString(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + Range.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Vyragosa");
                if (xvar != null)
                {
                    Vyragosa = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + Vyragosa.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/ShootVyra");
                if (xvar != null)
                {
                    ShotVyra = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + ShotVyra.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Blazewing");
                if (xvar != null)
                {
                    Blazewing = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + Blazewing.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/BloodseekerSearch");
                if (xvar != null)
                {
                    BloodseekerSearch = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + BloodseekerSearch.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/BloodseekerKill");
                if (xvar != null)
                {
                    BloodseekerKill = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + BloodseekerKill.ToString());
                }

                // Load Variables - Other Settings
                xvar = xml.SelectSingleNode("//Rarekiller/BlacklistCheck");
                if (xvar != null)
                {
                    BlacklistCheck = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + BlacklistCheck.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/BlacklistTime");
                if (xvar != null)
                {
                    BlacklistTime = Convert.ToString(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + BlacklistTime.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/GroundMountMode");
                if (xvar != null)
                {
                    GroundMountMode = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + GroundMountMode.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/DontKillTagged");
                if (xvar != null)
                {
                    DontKillTagged = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + DontKillTagged.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/BlacklistTagged");
                if (xvar != null)
                {
                    BlacklistTagged = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + BlacklistTagged.ToString());
                } xvar = xml.SelectSingleNode("//Rarekiller/WinnerTagged");
                if (xvar != null)
                {
                    WinnerTagged = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + WinnerTagged.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/ZoneSave");
                if (xvar != null)
                {
                    ZoneSave = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + ZoneSave.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Alert");
                if (xvar != null)
                {
                    Alert = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + Alert.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Wisper");
                if (xvar != null)
                {
                    Wisper = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + Wisper.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/GuildChat");
                if (xvar != null)
                {
                    Guild = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + Guild.ToString());
                }
				
				xvar = xml.SelectSingleNode("//Rarekiller/SoundfileFoundRare");
                if (xvar != null)
                {
                    if (Convert.ToString(xvar.InnerText) != "")
                        SoundfileFoundRare = Convert.ToString(xvar.InnerText);
                    else
                        SoundfileFoundRare = Rarekiller.Soundfile;
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + SoundfileFoundRare.ToString());
                }
				xvar = xml.SelectSingleNode("//Rarekiller/SoundfileWisper");
                if (xvar != null)
                {
                    if (Convert.ToString(xvar.InnerText) != "")
                        SoundfileWisper = Convert.ToString(xvar.InnerText);
                    else
                        SoundfileWisper = Rarekiller.Soundfile;
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + SoundfileWisper.ToString());
                }
				xvar = xml.SelectSingleNode("//Rarekiller/SoundfileGuild");
                if (xvar != null)
                {
                    if (Convert.ToString(xvar.InnerText) != "")
                        SoundfileGuild = Convert.ToString(xvar.InnerText);
                    else
                        SoundfileGuild = Rarekiller.Soundfile;
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + SoundfileGuild.ToString());
                }
				
                xvar = xml.SelectSingleNode("//Rarekiller/Keyer");
                if (xvar != null)
                {
                    Keyer = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + Keyer.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/ContinentChange");
                if (xvar != null)
                {
                    ContinentChange = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + ContinentChange.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/JustAlert");
                if (xvar != null)
                {
                    JustAlert = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + JustAlert.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/KillWoW");
                if (xvar != null)
                {
                    KillWoW = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + KillWoW.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/LogOut");
                if (xvar != null)
                {
                    LogOut = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + LogOut.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Message1");
                if (xvar != null)
                {
                    Message1 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDebug("Plugin Rarekiller Load: " + xvar.Name + "=" + Message1.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Message2");
                if (xvar != null)
                {
                    Message2 = Convert.ToString(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + Message2.ToString());
                }
				xvar = xml.SelectSingleNode("//Rarekiller/PlayerFollows");
                if (xvar != null)
                {
                    PlayerFollows = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + PlayerFollows.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/PlayerFollowsLogout");
                if (xvar != null)
                {
                    PlayerFollowsLogout = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + PlayerFollowsLogout.ToString());
                }

                xvar = xml.SelectSingleNode("//Rarekiller/PlayerFollowsTime");
                if (xvar != null)
                {
                    PlayerFollowsTime = Convert.ToString(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + PlayerFollowsTime.ToString());
                }
				xvar = xml.SelectSingleNode("//Rarekiller/LootSuccess");
                if (xvar != null)
                {
                    LootSuccess = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + LootSuccess.ToString());
                }
				
                xvar = xml.SelectSingleNode("//Rarekiller/ScreenAeonaxx");
                if (xvar != null)
                {
                    ScreenAeonaxx = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + ScreenAeonaxx.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/ScreenRarekiller");
                if (xvar != null)
                {
                    ScreenRarekiller = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + ScreenRarekiller.ToString());
                }
				xvar = xml.SelectSingleNode("//Rarekiller/ScreenCamel");
                if (xvar != null)
                {
                    ScreenCamel = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + ScreenCamel.ToString());
                }
				xvar = xml.SelectSingleNode("//Rarekiller/ScreenTamer");
                if (xvar != null)
                {
                    ScreenTamer = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + ScreenTamer.ToString());
                }
				xvar = xml.SelectSingleNode("//Rarekiller/ScreenFound");
                if (xvar != null)
                {
                    ScreenFound = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + ScreenFound.ToString());
                }
				xvar = xml.SelectSingleNode("//Rarekiller/ScreenSuccess");
                if (xvar != null)
                {
                    ScreenSuccess = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + ScreenSuccess.ToString());
                }				
                // Load Variables - Testpart
                xvar = xml.SelectSingleNode("//Rarekiller/Aeonaxx");
                if (xvar != null)
                {
                    Aeonaxx = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + Aeonaxx.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/AeonaxxTestcaseTrainingDummy");
                if (xvar != null)
                {
                    TestKillAeonaxx = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + TestKillAeonaxx.ToString());
                }

                xvar = xml.SelectSingleNode("//Rarekiller/Camel");
                if (xvar != null)
                {
                    Camel = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + Camel.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Collect");
                if (xvar != null)
                {
                    Collect = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + Collect.ToString());
                }
                xvar = xml.SelectSingleNode("//Rarekiller/Dormus");
                if (xvar != null)
                {
                    Dormus = Convert.ToBoolean(xvar.InnerText);
                    Logging.WriteDebug("Rarekiller Load: " + xvar.Name + "=" + Dormus.ToString());
                }
            }
            catch (Exception e)
            {
                Logging.WriteDebug("Rarekiller: PROJECTE EXCEPTION, STACK=" + e.StackTrace);
                Logging.WriteDebug("Rarekiller: PROJECTE EXCEPTION, SRC=" + e.Source);
                Logging.WriteDebug("Rarekiller: PROJECTE : " + e.Message);
            }
        }

    }
}
