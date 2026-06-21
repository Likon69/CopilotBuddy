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
    class RarekillerAeonaxx
    {
        public static LocalPlayer Me = ObjectManager.Me;

        public void MountAeonaxx()
        {
            int loothelper = 0;
            bool CastSuccess = false;

            if(Rarekiller.Settings.DeveloperLogs)
                Logging.WriteDebug("Rarekiller: Scan for Aeonaxx to Mount");
            ObjectManager.Update();
            List<WoWUnit> objList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(o => ((o.Entry == 50062) // Aeonaxx friendly
				|| ((o.Entry == 42288) && Rarekiller.Settings.TestMountAeonaxx) //Testcase Robby Flay
                ))
                .OrderBy(o => o.Distance).ToList();
            foreach (WoWUnit o in objList)
            {
                if (Rarekiller.Settings.WispersForBuddyCenter)
                    Lua.DoString(string.Format("RunMacroText(\"/w {0} found {1}, ID {2}\")", Me.Name, o.Name, o.Entry), 0);

                if (!o.Dead && !Blacklist.Contains(o.Guid))
                {
                    Logging.Write(System.Drawing.Color.MediumPurple, "Rarekiller: Find a hunted Mob called {0} ID {1}", o.Name, o.Entry);
                    if (Rarekiller.Settings.GroundMountMode)
                    {
                        Logging.Write("Rarekiller Part Aeonaxx: Can't hunt {0} in Ground Mount Mode.", o.Name);
                        Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller Part Aeonaxx: Blacklist Mob for 5 Minutes.");
                        return;
                    }
                    if (Rarekiller.inCombat)
                    {
                        Logging.Write("Rarekiller Part Aeonaxx: ... but I'm in another Combat, Dead or Ghost :( !!!");
                        return;
                    }
					if (Me.IsOnTransport)
					{
						Logging.Write("Rarekiller Part Aeonaxx: ... but I'm on a Transport. So I already mounted him");
                        Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist15));
                        Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller Part Aeonaxx: Blacklist Mob for 15 Minutes.");
						return;
					}
                    while (Me.IsCasting)
                    {
                        Thread.Sleep(100);
                    }

                    if (Rarekiller.Settings.Alert)
                    {
                        if (File.Exists(Rarekiller.Soundfile))
                            new SoundPlayer(Rarekiller.Soundfile).Play();
                        else
                            Logging.Write(System.Drawing.Color.Red, "Rarekiller Part Aeonaxx: playing Soundfile failes");
                    }

// ------------------ Move to Aeonaxx ----------------
                    o.Target();
                    o.Face();
					Logging.Write("Rarekiller Part MoveTo: Fly to Target");
					while (o.Location.Distance(Me.Location) > 3)
					{
						Flightor.MoveTo(o.Location);
						Thread.Sleep(100);
					}
                    Logging.WriteDebug("Rarekiller Part Aeonaxx: {0} Location: {1} / {2} / {3}", o.Name , Convert.ToString(o.X), Convert.ToString(o.Y), Convert.ToString(o.Z));
                    Logging.WriteDebug("Rarekiller Part Aeonaxx: My Location: {0} / {1} / {2}", Convert.ToString(Me.X), Convert.ToString(Me.Y), Convert.ToString(Me.Z));

// ------------------ Mount Aeonaxx -----------------
					o.Interact();
					Thread.Sleep(2000);
					Logging.Write("Rarekiller Part Aeonaxx: Interact with {0}", o.Name);
					if (Rarekiller.Settings.WispersForBuddyCenter)
						Lua.DoString(string.Format("RunMacroText(\"/w {0} mounted Dragon {1}, ID {2}\")", Me.Name, o.Name, o.Entry), 0);
// ------------------ Monitor whos also there -----------------
					Logging.WriteDebug("Rarekiller Part Aeonaxx: Dragon Location: {0} / {1} / {2}", Convert.ToString(o.X), Convert.ToString(o.Y), Convert.ToString(o.Z));
					Logging.WriteDebug("Rarekiller Part Aeonaxx: My Location: {0} / {1} / {2}", Convert.ToString(Me.X), Convert.ToString(Me.Y), Convert.ToString(Me.Z));
					ObjectManager.Update();
					foreach (WoWPlayer player in ObjectManager.GetObjectsOfType<WoWPlayer>())
					{
						Logging.Write("Rarekiller Part Aeonaxx: {0} was also here", player.Name);
						Logging.WriteDebug("Rarekiller Part Aeonaxx: Player Location: {0} / {1} / {2}", Convert.ToString(player.X), Convert.ToString(player.Y), Convert.ToString(player.Z));
					}

					if(Rarekiller.Settings.ScreenAeonaxx)
					{
						Lua.DoString("TakeScreenshot()");
						Thread.Sleep(300);
						Logging.WriteDebug("Rarekiller Part Aeonaxx: Take Screenshot after Mount up");
					}
					
					if (RoutineManager.Current.NeedRest)
					{
						Logging.Write("Rarekiller Part Aeonaxx: CC says we need rest - Letting it do it before Fight.");
						RoutineManager.Current.Rest();
					}

                }
                else if (o.Dead)
                {
                    if (o.CanLoot)
                    {
// ----------------- Loot Helper for all killed Rare Mobs ---------------------
                        Logging.Write("Rarekiller Part Aeonaxx: Found lootable corpse, move to him");

// ----------------- Move to Corpse -------------------
                        Logging.Write("Rarekiller Part MoveTo: Fly to Target");
						while (o.Location.Distance(Me.Location) > 5)
						{
							Flightor.MoveTo(o.Location);
							Thread.Sleep(100);
							if (Rarekiller.inCombat) return;
						}
						WoWMovement.MoveStop();

                        if (Me.Auras.ContainsKey("Flight Form"))
                            Lua.DoString("CancelShapeshiftForm()");
                        else if (Me.Mounted)
                            Lua.DoString("Dismount()");

// ----------------- Loot Corpse -------------------
						if(Rarekiller.Settings.ScreenAeonaxx)
						{
							Lua.DoString("TakeScreenshot()");
							Thread.Sleep(500);
                            Logging.WriteDebug("Rarekiller Part Aeonaxx: Take Screenshot successfully killed Aeonaxx");
						}
						while (loothelper < 3)
                        {
                            Thread.Sleep(500);
                            WoWMovement.MoveStop();
                            o.Interact();
                            Thread.Sleep(2000);
                            Lua.DoString("RunMacroText(\"/click StaticPopup1Button1\");");
                            Thread.Sleep(4000);
                            if (!o.CanLoot)
                            {
                                Logging.Write("Rarekiller Part Aeonaxx: successfully looted");
                                Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                                Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller Part Aeonaxx: Blacklist Mob for 60 Minutes.");
                                return;
                            }
                            else
                            {
                                Logging.Write("Rarekiller Part Aeonaxx: Loot failed, try again");
                                loothelper = loothelper + 1;
                            }
                        }
                        Logging.Write("Rarekiller Part Aeonaxx: Loot failed 3 Times");
                    }
                    if (!Blacklist.Contains(o.Guid))
                    {
                        Logging.Write("Rarekiller Part Aeonaxx: Find {0}, but sadly he's dead", o.Name);
                        Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                        Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller Part Aeonaxx: Blacklist Mob for 60 Minutes.");
                    }
                }
            }
        }

        public void KillAeonaxx()
        {
            bool CastSuccess = false;
            bool CastSuccessThrow = true;
            bool CastSuccessShoot = true;

            ObjectManager.Update();
            List<WoWUnit> objList1 = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(aenoaxx => (!aenoaxx.Dead && (aenoaxx.Location.Distance(Me.Location) < 40) && ((Rarekiller.Settings.Aeonaxx && (aenoaxx.Entry == 51236)) //Aenoaxx hostile
                || (Rarekiller.Settings.TestWelpTargeting && Rarekiller.inCombat && (aenoaxx.Entry == 33687)) // Testcase Chillmaw
                || (Rarekiller.Settings.TestKillAeonaxx && (aenoaxx.Entry == 31146)) // Testcase Training Dummy
                )))
                .OrderBy(aenoaxx => aenoaxx.Entry).ToList();
            foreach (WoWUnit aeonaxx in objList1)
            {
                if (!aeonaxx.Dead && !aeonaxx.IsPet && !Blacklist.Contains(aeonaxx.Guid))
                {
                    if ((aeonaxx.Entry == 51236) && !Me.IsOnTransport)
                    {
                        Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller: Find {0} ID {1}, but I don't mounted him", aeonaxx.Name, aeonaxx.Entry);
                        Blacklist.Add(aeonaxx.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                        Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller: Blacklist Mob for 60 Minutes.");
                        return;
                    }

                    Logging.Write(System.Drawing.Color.MediumPurple, "Rarekiller: Find a hunted Mob called {0} ID {1}", aeonaxx.Name, aeonaxx.Entry);

                    while (!aeonaxx.Dead)
                    {
                        //if (Rarekiller.Settings.DeveloperLogs)
                        Logging.WriteDebug("Rarekiller: Scan for Aeonaxx Adds to Kill");
                        ObjectManager.Update();
                        List<WoWUnit> objList2 = ObjectManager.GetObjectsOfType<WoWUnit>()
                            .Where(add => (!add.Dead && (add.Location.Distance(Me.Location) < 40) && ((Rarekiller.Settings.Aeonaxx && (add.Entry == 44038)) //Young Stone Drake
                            || (Rarekiller.Settings.TestWelpTargeting && Rarekiller.inCombat && (add.Entry == 33695))))) // Testcase Cultist Bombadier
                            .OrderBy(add => add.Entry).ToList();

                        foreach (WoWUnit add in objList2)
                        {

                            // Kill Welpling
                            while (!add.Dead)
                            {
                                if (Me.CurrentTarget != add)
                                {
                                    add.Target();
                                    Logging.Write("Rarekiller Part Aeonaxx: Target {0}", Me.CurrentTarget.Name);

                                }
                                if (!Me.IsAutoAttacking)
                                    Lua.DoString("StartAttack()");

                                if (Me.Class == WoWClass.Hunter)
                                    Rarekiller.RotaHunter.HunterKill(add);
                                else if (Me.Class == WoWClass.Druid)
                                    Rarekiller.RotaDruid.DruidKill(add);
                                else if (Me.Class == WoWClass.DeathKnight)
                                    Rarekiller.RotaDK.DKKill(add);
                                else if (Me.Class == WoWClass.Warrior)
                                    Rarekiller.RotaWarrior.WarriorKill(add);
                                else if (Me.Class == WoWClass.Warlock)
                                    Rarekiller.RotaWarlock.WarlockKill(add);
                                else if (Me.Class == WoWClass.Shaman)
                                    Rarekiller.RotaShaman.ShamanKill(add);
                                else if (Me.Class == WoWClass.Paladin)
                                    Rarekiller.RotaPaladin.PaladinKill(add);
                                else if (Me.Class == WoWClass.Mage)
                                    Rarekiller.RotaMage.MageKill(add);
                                else if (Me.Class == WoWClass.Rogue)
                                    Rarekiller.RotaRogue.RogueKill(add);
                                else if (Me.Class == WoWClass.Priest)
                                    Rarekiller.RotaPriest.PriestKill(add);
                            }
                        }

                        //Damage to Aeonaxx
                        if (Me.CurrentTarget != aeonaxx)
                            aeonaxx.Target();
                        if (!Me.IsAutoAttacking)
                            Lua.DoString("StartAttack()");

                        if (Me.Class == WoWClass.Hunter)
                            Rarekiller.RotaHunter.HunterKill(aeonaxx);
                        else if (Me.Class == WoWClass.Druid)
                            Rarekiller.RotaDruid.DruidKill(aeonaxx);
                        else if (Me.Class == WoWClass.DeathKnight)
                            Rarekiller.RotaDK.DKKill(aeonaxx);
                        else if (Me.Class == WoWClass.Warrior)
                            Rarekiller.RotaWarrior.WarriorKill(aeonaxx);
                        else if (Me.Class == WoWClass.Warlock)
                            Rarekiller.RotaWarlock.WarlockKill(aeonaxx);
                        else if (Me.Class == WoWClass.Shaman)
                            Rarekiller.RotaShaman.ShamanKill(aeonaxx);
                        else if (Me.Class == WoWClass.Paladin)
                            Rarekiller.RotaPaladin.PaladinKill(aeonaxx);
                        else if (Me.Class == WoWClass.Mage)
                            Rarekiller.RotaMage.MageKill(aeonaxx);
                        else if (Me.Class == WoWClass.Rogue)
                            Rarekiller.RotaRogue.RogueKill(aeonaxx);
                        else if (Me.Class == WoWClass.Priest)
                            Rarekiller.RotaPriest.PriestKill(aeonaxx);
                    }
                }
            }
        } 
    }    
}
