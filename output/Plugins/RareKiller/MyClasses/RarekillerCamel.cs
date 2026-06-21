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
    class RarekillerCamel
    {
        public static LocalPlayer Me = ObjectManager.Me;
        private static Stopwatch BlacklistTimer = new Stopwatch();
        Int64 Place = 0;
        public static WoWPoint ProblemCamel1 = new WoWPoint(-8906.634, 312.6967, 349.2024);
        public static WoWPoint ProblemCamel2 = new WoWPoint(-9900.116, 461.3653, 45.62226);
        public static WoWPoint ProblemCamel3 = new WoWPoint(-10697.69, 1045.757, 24.125);
        public static WoWPoint ProblemCamel4 = new WoWPoint(-11066.67, -2100.342, 175.2816);

        public void findAndPickupObject()
        {
            bool CastSuccess = false;
			bool ForceGround = false;
			bool FirstTry = true;

            if (Rarekiller.Settings.DeveloperLogs)
                Logging.WriteDebug("Rarekiller: Scan for Camel Figurine");
            ObjectManager.Update();
            List<WoWUnit> objList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(o => ((o.Entry == 50409) || (o.Entry == 50410) // 50409 might be the porting Camel Figurine
                    || ((o.Entry == 48959) && Rarekiller.Settings.TestFigurineInteract) //Testcase rostiger Amboss - Schnotzz Landing
                ))
                .OrderBy(o => o.Distance).ToList();
            foreach (WoWUnit o in objList)
            {
                if (!Blacklist.Contains(o.Guid))
                {
                    Logging.Write(System.Drawing.Color.MediumPurple, "Rarekiller Part Camel: Find {0} ID {1}", o.Name, o.Entry); 
                    if (Rarekiller.inCombat)
                    {
                        Logging.WriteDebug("Rarekiller Part Camel: ... but first I have to finish fighting another Mob.");
                        return;
                    }
                    if (Me.IsOnTransport)
                    {
                        Logging.WriteDebug("Rarekiller Part Camel: ... but I'm on a Transport.");
                        return;
                    }
                    while (Me.IsCasting)
                    {
                        Thread.Sleep(100);
                    }

                    if (Rarekiller.Settings.WispersForBuddyCenter)
                        Lua.DoString(string.Format("RunMacroText(\"/w {0} found {1}, ID {2}\")", Me.Name, o.Name, o.Entry), 0);

                    if (Rarekiller.Settings.Alert)
                    {
                        if (File.Exists(Rarekiller.Settings.SoundfileFoundRare))
                            new SoundPlayer(Rarekiller.Settings.SoundfileFoundRare).Play(); 
                        else if (File.Exists(Rarekiller.Soundfile))
                            new SoundPlayer(Rarekiller.Soundfile).Play();
                        else
                            Logging.Write(System.Drawing.Color.Red, "Rarekiller Part Camel: playing Soundfile failes");
                    }
					
					if (Me.IsFlying && ((o.Location.Distance(ProblemCamel1) < 10) || (o.Location.Distance(ProblemCamel2) < 10) || 
							(o.Location.Distance(ProblemCamel3) < 10) || (o.Location.Distance(ProblemCamel4) < 20)))
					{
						Logging.Write("Rarekiller Part MoveTo: Found a Problem Figurine {0} so dismount and walk", o.Entry);
						while (o.Location.Distance(Me.Location) > 30)
						{
							if (Rarekiller.Settings.GroundMountMode || ForceGround)
								Navigator.MoveTo(o.Location);
							else
								Flightor.MoveTo(o.Location);
							Thread.Sleep(100);
							if (Rarekiller.inCombat) return;
						}
						WoWMovement.MoveStop();
						Thread.Sleep(1000);
						//Descend to Land
						WoWMovement.Move(WoWMovement.MovementDirection.Descend);
						Thread.Sleep(2000);
						WoWMovement.MoveStop();
						//Dismount
                        if (Me.Auras.ContainsKey("Flight Form"))
                            Lua.DoString("CancelShapeshiftForm()");
                        else if (Me.Mounted)
                            Lua.DoString("Dismount()");

						Thread.Sleep(300);
						ForceGround = true;
						FirstTry = false;						
					}
					
					Logging.Write("Rarekiller Part MoveTo: Move to target");
                    BlacklistTimer.Reset();
                    BlacklistTimer.Start();
                    Place = Me.ZoneId;


					while (o.Location.Distance(Me.Location) > 4)
					{
						if (Rarekiller.Settings.GroundMountMode || ForceGround)
							Navigator.MoveTo(o.Location);
						else
							Flightor.MoveTo(o.Location);
						Thread.Sleep(100);
						if (Rarekiller.inCombat) return;
						if (BlacklistTimer.Elapsed.TotalSeconds > 10 && FirstTry)
						{
							Logging.Write(System.Drawing.Color.Red, "Rarekiller Part Camel: Can't reach Figurine {0} so dismount and walk", o.Entry);

							WoWMovement.MoveStop();
							Thread.Sleep(1000);
							//Descend to Land
							WoWMovement.Move(WoWMovement.MovementDirection.Descend);
							Thread.Sleep(2000);
							WoWMovement.MoveStop();
							//Walk some Meters to avoid standing on a Tent
							WoWMovement.Move(WoWMovement.MovementDirection.StrafeLeft);
							Thread.Sleep(500);
							WoWMovement.MoveStop();							
							Thread.Sleep(1000);
							//Dismount
                            if (Me.Auras.ContainsKey("Flight Form"))
                                Lua.DoString("CancelShapeshiftForm()");
                            else if (Me.Mounted)
                                Lua.DoString("Dismount()");

							Thread.Sleep(300);
							ForceGround = true;
							FirstTry = false;
							BlacklistTimer.Reset();
							BlacklistTimer.Start();
						}
						if (Rarekiller.Settings.BlacklistCheck && !FirstTry && (BlacklistTimer.Elapsed.TotalSeconds > (Convert.ToInt32(Rarekiller.Settings.BlacklistTime))))
						{
                            Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist15));
							Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller Part Camel: Blacklist Figurine for 15 Minutes.");
							BlacklistTimer.Reset();
							WoWMovement.MoveStop();
							return;
						}
						if (Rarekiller.Settings.ZoneSave && (Me.ZoneId != Place))
						{
							Logging.Write(System.Drawing.Color.Red, "Rarekiller Part MoveTo: Left Zone while move to {0} is not allowed", o.Name);
                            Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
							Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller: Blacklist Figurine for 5 Minutes.");
							BlacklistTimer.Reset();
							WoWMovement.MoveStop();
							return;
						}
					}

                    BlacklistTimer.Reset();
                    Thread.Sleep(500);
                    WoWMovement.MoveStop();
					
                    Logging.WriteDebug("Rarekiller Part Camel: Figurine Location: {0} / {1} / {2}", Convert.ToString(o.X), Convert.ToString(o.Y), Convert.ToString(o.Z));
                    Logging.WriteDebug("Rarekiller Part Camel: My Location: {0} / {1} / {2}", Convert.ToString(Me.X), Convert.ToString(Me.Y), Convert.ToString(Me.Z));
					if(Rarekiller.Settings.ScreenCamel && Rarekiller.Settings.ScreenFound)
					{
						Lua.DoString("TakeScreenshot()");
						Thread.Sleep(500);
                        Logging.WriteDebug("Rarekiller Part Camel: Take Screenshot before Interact");
					}
                    if (Me.Auras.ContainsKey("Flight Form"))
                        Lua.DoString("CancelShapeshiftForm()");
                    else if (Me.Mounted)
                        Lua.DoString("Dismount()");

					Thread.Sleep(500);
                    o.Interact();
                    Thread.Sleep(10000);
                    if (o.Entry == 50410)
                        o.Interact();
                    Logging.Write("Rarekiller Part Camel: Interact with Figurine - ID {0}", o.Entry);
					ForceGround = false;
					FirstTry = true;
                    if (o.Entry == 50410 && Rarekiller.Settings.Collect)
                    {
                        Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist15));
                        Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller Part Camel: Blacklist Figurine for 15 Minutes.");
                    }
                }
            }
        }

        public void findAndKillDormus()
        {
            bool CastSuccess = false;

            if (Rarekiller.Settings.DeveloperLogs)
                Logging.WriteDebug("Rarekiller: Scan for Dormus");
            ObjectManager.Update();
            List<WoWUnit> objList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(o => ((o.Entry == 50245)
                    || ((o.Entry == 47755) && Rarekiller.Settings.TestKillDormus) //Testcase Warlord Ihsenn
                ))
                .OrderBy(o => o.Distance).ToList();
            foreach (WoWUnit o in objList)
            {
                if (!o.Dead)
                {
                    Logging.Write(System.Drawing.Color.MediumPurple, "Rarekiller Part Dormus: Find Dormus.");
                    
                    if (Rarekiller.Settings.Alert)
                    {
                        if (File.Exists(Rarekiller.Settings.SoundfileFoundRare))
                            new SoundPlayer(Rarekiller.Settings.SoundfileFoundRare).Play();
                        else if (File.Exists(Rarekiller.Soundfile))
                            new SoundPlayer(Rarekiller.Soundfile).Play();
                        else
                            Logging.Write(System.Drawing.Color.Red, "Rarekiller Part Dormus: playing Soundfile failes");
                    }


                    if (RoutineManager.Current.NeedRest)
                    {
                        Logging.Write("Rarekiller Part Dormus: CC says we need rest - Letting it do it before Fight.");
                        RoutineManager.Current.Rest();
                    }

                    if (Me.Auras.ContainsKey("Flight Form"))
                        Lua.DoString("CancelShapeshiftForm()");
                    else if (Me.Mounted)
                        Lua.DoString("Dismount()");

                    o.Target();
                    Thread.Sleep(500);

                    while (!Rarekiller.inCombat)
                    {
                        // ------------- Move to Dormus with Klick to Move -------------------					
                        Logging.Write("Rarekiller Part Dormus: Move to Dormus");

                        while (Me.IsSwimming)
                        {
                            WoWMovement.ClickToMove(o.Location);
                        }
                        WoWMovement.MoveStop();

                        while (o.Location.Distance(Me.Location) > Convert.ToInt64(Rarekiller.Settings.Range))
                        {
                            if (Rarekiller.Settings.GroundMountMode)
                                Navigator.MoveTo(o.Location);
                            else
                                Flightor.MoveTo(o.Location);
                            Thread.Sleep(100);
                        }
                        WoWMovement.MoveStop();
                        // ------------- pull Dormus  -------------------					
                        Logging.WriteDebug("Rarekiller Part Dormus: Distance: {0}", o.Location.Distance(Me.Location));
						o.Target();
						o.Face();
						Thread.Sleep(100);
						if(Rarekiller.Settings.ScreenCamel && Rarekiller.Settings.ScreenFound)
						{
							Lua.DoString("TakeScreenshot()");
							Thread.Sleep(500);
                            Logging.WriteDebug("Rarekiller Part Dormus: Take Screenshot before Pull");
						}

                        if (!(Rarekiller.Settings.DefaultPull) && SpellManager.HasSpell(Rarekiller.Settings.Pull))
                            CastSuccess = RarekillerSpells.CastSafe(Rarekiller.Settings.Pull, o, true);
                        else if (SpellManager.HasSpell(Rarekiller.Spells.LowPullspell))
                            CastSuccess = RarekillerSpells.CastSafe(Rarekiller.Spells.LowPullspell, o, true);
                        else
                            Logging.Write(System.Drawing.Color.Red, "Rarekiller Part Dormus: I have no Pullspell");
                        if (!CastSuccess && SpellManager.HasSpell("Shoot"))
                            CastSuccess = RarekillerSpells.CastSafe("Shoot", o, true);
                        if (CastSuccess)
                        {
                            Logging.Write("Rarekiller Part Dormus: successfully pulled Dormus");
                            Logging.WriteDebug("Rarekiller Part Dormus: Pull Distance: {0}", o.Location.Distance(Me.Location));
                            return;
                        }
                        else if (!CastSuccess && Rarekiller.inCombat)
                            Logging.Write("Rarekiller Part Dormus: got Aggro");
                        else
                        {
                            Logging.Write("Rarekiller Part Dormus: Pull Fails - Set Range to 3 for next try");
                            Rarekiller.Settings.Range = "3";
                        }
                    }
                }
                else
                {
                    Logging.Write("Rarekiller Part Dormus: Find {0}, but he's dead", o.Name);
                    // ----------------- Loot Helper ---------------------
                    if (o.CanLoot)
                    {
                        if (Me.Auras.ContainsKey("Flight Form"))
                            Lua.DoString("CancelShapeshiftForm()");
                        else if (Me.Mounted)
                            Lua.DoString("Dismount()");

                        o.Target();
                        // ------------- Move to Corpse with Klick to Move -------------------					
                        Logging.Write("Rarekiller Part Dormus: ... move to him to loot");
 						while (o.Location.Distance(Me.Location) > 3)
						{
							Navigator.MoveTo(o.Location);
							Thread.Sleep(100);
						}
						
                        Thread.Sleep(500);
                        WoWMovement.MoveStop();
						if(Rarekiller.Settings.ScreenCamel && Rarekiller.Settings.ScreenSuccess)
						{
							Lua.DoString("TakeScreenshot()");
							Thread.Sleep(500);
                            Logging.WriteDebug("Rarekiller Part Dormus: Take Screenshot successfully killed {0}", o.Name);
						}
                        o.Interact();
                        Thread.Sleep(2000);
                        Lua.DoString("RunMacroText(\"/click StaticPopup1Button1\");");
                        Thread.Sleep(4000);
                        if (!o.CanLoot)
                            Logging.Write("Rarekiller Part Dormus: successfuly looted");
                        else
                            Logging.Write("Rarekiller Part Dormus: Loot failed, try again");

                    }
                }
            }
        }
		
		public void AvoidDormusSpit(string buff)
        {
			Logging.Write("Rarekiller Part Dormus: Strafe left to avoid Damage");
			WoWMovement.Move(WoWMovement.MovementDirection.StrafeLeft);
            while (Me.ActiveAuras.ContainsKey(buff) || Me.ActiveAuras.ContainsKey(Rarekiller.Settings.AuraToID) || Me.ActiveAuras.ContainsKey(Rarekiller.Settings.AuraToID))
				Thread.Sleep(25);
			WoWMovement.MoveStop();
			Thread.Sleep(100);
		}
    }
}
