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
    class RarekillerKiller
    {

        public static LocalPlayer Me = ObjectManager.Me;
        private static Stopwatch BlacklistTimer = new Stopwatch();
        bool ForceGround = false;
        Int64 Place = 0;
        bool FirstTry = true;

        public void findAndKillMob()
        {
            bool CastSuccess = false;
			int loothelper = 0;

            if (Rarekiller.Settings.DeveloperLogs)
                Logging.WriteDebug("Rarekiller: Scan for Rare Mob");
            // ----------------- Generate a List with all wanted Rares found in Object Manager ---------------------		
            ObjectManager.Update();
            List<WoWUnit> objList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(o => (
                (Rarekiller.Settings.CATA && ((o.Entry == 50057)	// Blazewing Hyjal
                        || (o.Entry == 50053)           // Thartuk the Exile Hyjal
                        || (o.Entry == 50050)			// Shok'sharak Vashir
                        || (o.Entry == 50005)			// Poseidus Vashir
                        || (o.Entry == 50052)			// Burgy Blackheart Vashir
                        || (o.Entry == 49913)			// Lady La-La Vashir
                        || (o.Entry == 50060)			// Terborus Tiefenheim
                        || (o.Entry == 50059)			// Golgarok Tiefenheim
                        || (o.Entry == 49822)			// Jadefang Tiefenheim
                        || (o.Entry == 50065)			// Armagedillo Uldum
                        || (o.Entry == 50064)			// Cyrus the Black Uldum
                        || (o.Entry == 50085)			// Overlord Sunderfury Twilight Higlands
                        || (o.Entry == 50086)))			// Tarvus the Vile Twilight Higlands
                || (Rarekiller.Settings.Aeonaxx && (((o.Entry == 3868) && Rarekiller.Settings.BloodseekerSearch)	// Blood Seeker for Aeonaxx
                         ))			// Aeonaxx hostile || (o.Entry == 51236)
                || (Rarekiller.Settings.Poseidus && ((o.Entry == 50005)	// Poseidus
                        || (o.Entry == 9999999)))			// Platzhalter
                || (Rarekiller.Settings.TLPD && ((o.Entry == 32491)	// Timelost Protodrake
                        || (o.Entry == 32630)))			// Vyragosa
                || (Rarekiller.Settings.WOTLK && ((o.Entry == 32517)	// Loque'nahak
                        || (o.Entry == 32495)			// Hildana Deathstealer
                        || (o.Entry == 32358)			// Fumblub Gearwind
                        || (o.Entry == 32377)			// Perobas the Bloodthirster
                        || (o.Entry == 32398)			// King Ping
                        || (o.Entry == 32409)			// Crazed Indu'le Survivor
                        || (o.Entry == 32422)			// Grocklar
                        || (o.Entry == 32438)			// Syreian the Bonecarver
                        || (o.Entry == 32471)			// Griegen
                        || (o.Entry == 32481)			// Aotona
                        || (o.Entry == 32630)			// Vyragosa
                        || (o.Entry == 32487)			// Putridus the Ancient
                        || (o.Entry == 32501)			// High Thane Jorfus
                        || (o.Entry == 32357)			// Old Crystalbark
                        || (o.Entry == 32361)			// Icehorn
                        || (o.Entry == 32386)			// Vigdis the War Maiden
                        || (o.Entry == 32400)			// Tukemuth
                        || (o.Entry == 32417)			// Scarlet Highlord Daion
                        || (o.Entry == 32429)			// Seething Hate
                        || (o.Entry == 32447)			// Zul'drak Sentinel
                        || (o.Entry == 32475)			// Terror Spinner
                        || (o.Entry == 32485)			// King Krush
                        || (o.Entry == 32500)))			// Dirkee
                || (Rarekiller.Settings.BC && ((o.Entry == 18695)		// Ambassador Jerrikar
                        || (o.Entry == 18697)			// Chief Engineer Lorthander
                        || (o.Entry == 18694)			// Collidus the Warp-Watcher
                        || (o.Entry == 18686)			// Doomslayer Jurim
                        || (o.Entry == 18678)			// Fulgorge
                        || (o.Entry == 18692)			// Hemathion
                        || (o.Entry == 18680)			// Marticar
                        || (o.Entry == 18690)			// Morcrush
                        || (o.Entry == 18685)			// Okrek
                        || (o.Entry == 18683)			// Voidhunter Yar
                        || (o.Entry == 18682)			// Bog Lurker
                        || (o.Entry == 18681)			// Coilfang Emissary
                        || (o.Entry == 18689)			// Crippler
                        || (o.Entry == 18698)			// Ever-Core the Punisher
                        || (o.Entry == 17144)			// Goretooth
                        || (o.Entry == 18696)			// Kraator
                        || (o.Entry == 18677)			// Mekthorg the Wild
                        || (o.Entry == 20932)			// Nuramoc
                        || (o.Entry == 18693)			// Speaker Mar'grom
                        || (o.Entry == 18679)))			// Vorakem Doomspeaker	
                || ((o.Level < Rarekiller.Settings.Level) && Rarekiller.Settings.LowRAR && (o.CreatureRank == Styx.WoWUnitClassificationType.Rare)) // every single Rare Mob < Level 61 is hunted	
                || (Rarekiller.Settings.HUNTbyID && (o.Entry == Convert.ToInt64(Rarekiller.Settings.MobID)))				// Hunt special IDs 
                ))
                .OrderBy(o => o.Distance).ToList();
            foreach (WoWUnit o in objList)
            {
                if (!o.Dead && !o.IsPet && !Blacklist.Contains(o.Guid))
                {
                    Logging.Write(System.Drawing.Color.MediumPurple, "Rarekiller: Find a hunted Mob called {0} ID {1}", o.Name, o.Entry);
                    //if ((o.Entry == 51236) && !Me.IsOnTransport)
                    //{
                    //    Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller: Find {0} ID {1}, but I don't mounted him", o.Name, o.Entry);
                    //    Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                    //    Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller: Blacklist Mob for 60 Minutes.");
                    //    return;
                    //}
                    //else if (o.Entry == 51236)
                    //    return;
					
					if(o.Entry != 51236)
					{
						if (o.Level > (Me.Level + 4))
						{
							Logging.Write("Rarekiller: His Level is 5 over mine, better not to kill him.");
                            Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
							Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller: Blacklist Mob for 60 Minutes.");
							return;
						}
						if (o.IsFriendly)
						{
							Logging.Write("Rarekiller: Find {0}, but he's friendly", o.Name);
                            Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
							Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller: Blacklist Mob for 60 Minutes.");
							return;
						}
						if ((o.Entry == 32630) && !Rarekiller.Settings.Vyragosa)
						{
							Logging.Write("Rarekiller: Config says: don't kill Vyragosa.");
                            Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
							Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller: Blacklist Mob for 5 Minutes.");
							return;
						}
						if ((o.Entry == 50057) && !Rarekiller.Settings.Blazewing)
						{
							Logging.Write("Rarekiller: Config says: don't kill Blazewing.");
                            Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
							Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller: Blacklist Mob for 5 Minutes.");
							return;
						}
					    if ((o.Entry == 596) || (o.Entry == 599))
						{
	// ----------------- Distancecheck because of the Spawn Place is Underground
							Logging.Write("Rarekiller: Don't run wild because of Instance Mobs.");
                            Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
							Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller: Blacklist Mob for 60 Minutes.");
							return;
						}

						if(((o.Entry == 49822) || (o.Entry == 50086) || (o.Entry == 2744)) && (Me.IsFlying || o.Location.Distance(Me.Location) > 30))
						{
	// ----------------- Distancecheck because of the Spawn Place is Underground
							Logging.Write("Rarekiller: {0} is more then 20 yd away and Underground", o.Name);
							Logging.Write(System.Drawing.Color.MediumPurple, "Rarekiller: You have to place me next to him, if you want to hunt this Mob.");
                            Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
							Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller: Blacklist Mob for 5 Minutes.");
							return;
						}
						
						if(Rarekiller.BlacklistMobsList.ContainsKey(Convert.ToInt32(o.Entry)))
						{
							Logging.Write("Rarekiller: {0} is Member of the BlacklistedMobs.xml", o.Name);
                            Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist15));
							Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller: Blacklist Mob for 15 Minutes.");
							return;
						}
                        if (Rarekiller.Settings.NotKillTameable && o.IsTameable)
                        {
                            return;
                        }
                        if (Rarekiller.inCombat)
                        {
                            Logging.Write("Rarekiller: ... but first I have to finish fighting another one.");
                            return;
                        }
                        if (Me.IsOnTransport)
                        {
                            Logging.Write("Rarekiller: ... but I'm on a Transport.");
                            return;
                        }
                        if ((o.Entry == 3868) && !Rarekiller.Settings.BloodseekerKill)
                            Rarekiller.Settings.Range = "35";
	// ----------------- Check if hunted Mob is tagged by another Player ---------------------
                        if (Rarekiller.Settings.DontKillTagged)
                        {
                            if (o.TaggedByOther)
                            {
                                WoWMovement.MoveStop();
                                Logging.Write(System.Drawing.Color.Red, "Rarekiller: {0} is tagged by another player", o.Name);
                                if (Rarekiller.Settings.BlacklistTagged)
                                {
                                    Logging.Write("Rarekiller: -- I'm a nice Guy so let him have it");
                                    Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist2));
                                    Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller: Blacklist Mob for 2 Minutes.");
                                    return;
                                }
                                else
                                    Logging.Write("Rarekiller: -- lets take a look who wins :)");
                            }
                            while (o.TaggedByOther && !o.Dead)
                            {
                                if (Rarekiller.inCombat) return;
                                Thread.Sleep(1000);
                            }
                            if (o.Dead)
                            {
                                Logging.Write("Rarekiller: {0} was killed by another Player", o.Name);
                                Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                                Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller: Blacklist Mob for 60 Minutes.");
                                return;
                            }
                        }
					}
					
					if (Rarekiller.Settings.Alert)
					{
						if (File.Exists(Rarekiller.Settings.SoundfileFoundRare) && (o.Entry != 51236))
							new SoundPlayer(Rarekiller.Settings.SoundfileFoundRare).Play();
						else if (File.Exists(Rarekiller.Soundfile))
							new SoundPlayer(Rarekiller.Soundfile).Play();
						else
							Logging.Write(System.Drawing.Color.Red, "Rarekiller: playing Soundfile failes");
					}
					
					while (Me.IsCasting)
					{
						Thread.Sleep(100);
					}

// ----------------- Move to Mob Part ---------------------	
					if (o.Entry != 51236)
					{
						if ((o.Entry == 49822) || (Rarekiller.Settings.GroundMountMode &&
							(!(o.Entry == 50005)			// Poseidus Vashir
							|| !(o.Entry == 50052)			// Burgy Blackheart Vashir
							|| !(o.Entry == 49913)			// Lady La-La Vashir
							))) //In Vashir you have to use Flightor !
							ForceGround = true;
						else
							ForceGround = false;

						Logging.Write("Rarekiller Part MoveTo: Move to target");
						BlacklistTimer.Reset();
						BlacklistTimer.Start();
						Place = Me.ZoneId;
						if (Rarekiller.Settings.GroundMountMode && ((o.Entry == 33687) || (o.Entry == 32630) || (o.Entry == 32491) || (o.Entry == 50057) || (o.Entry == 50005) || (o.Entry == 50052) || (o.Entry == 49913)))
						{
							WoWPoint LastLocation = o.Location;
							while ((o.Location.Distance(Me.Location) > SpellManager.Spells[Rarekiller.Spells.FastPullspell].MaxRange) && !o.TaggedByOther && !o.Dead)
							{
								Logging.Write("Rarekiller: Try to hunt flying Mob {0} in Ground Mount Mode, maybe he comes in Range.", o.Name);
								o.Face();
								Thread.Sleep(100);
								if (o.Location.Distance(Me.Location) > LastLocation.Distance(Me.Location))
								{
									Logging.Write("Rarekiller: {0} is flying away", o.Name);
                                    if (!(o.Entry == 50005))
                                    {
                                        Blacklist.Add(o.Guid, TimeSpan.FromSeconds(10));
                                        Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller: Blacklist Mob for 10 Seconds.");
                                    }
									return;
								}
								else
									LastLocation = o.Location;
							}
						}
						else
						{
							while (o.Location.Distance(Me.Location) > Convert.ToInt64(Rarekiller.Settings.Range))
							{
								if (Rarekiller.Settings.GroundMountMode || ForceGround)
									Navigator.MoveTo(o.Location);
								else
									Flightor.MoveTo(o.Location);
								Thread.Sleep(100);

                                if (BlacklistTimer.Elapsed.TotalSeconds > 20 && FirstTry)
                                {
                                    Logging.Write(System.Drawing.Color.Red, "Rarekiller Part Rarekiller: Can't reach Mob {0} so try to dismount and walk", o.Name);

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
									Logging.Write(System.Drawing.Color.Red, "Rarekiller Part MoveTo: Can't reach Mob {0}, Blacklist and Move on", o.Name);
                                    Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
									Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller: Blacklist Mob for 5 Minutes.");
									BlacklistTimer.Reset();
									WoWMovement.MoveStop();
									return;
								}
								if (Rarekiller.Settings.ZoneSave && (Me.ZoneId != Place))
								{
									Logging.Write(System.Drawing.Color.Red, "Rarekiller Part MoveTo: Left Zone while move to {0} is not allowed", o.Name);
                                    Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
									Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller: Blacklist Mob for 5 Minutes.");
									BlacklistTimer.Reset();
									WoWMovement.MoveStop();
									return;
								}
							}
							BlacklistTimer.Reset();
							WoWMovement.MoveStop();
						}
							
	// ----------------- another Check if hunted Mob is tagged by another Player ---------------------	
						o.Target();
                        if (Rarekiller.Settings.DontKillTagged)
                        {
                            if (o.TaggedByOther)
                            {
                                Logging.Write(System.Drawing.Color.Red, "Rarekiller: {0} is tagged by another player", o.Name);
                                if (Rarekiller.Settings.BlacklistTagged)
                                {
                                    Logging.Write("Rarekiller: -- I'm a nice Guy so let him have it");
                                    Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist2));
                                    Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller: Blacklist Mob for 2 Minutes.");
                                    return;
                                }
                                else
                                    Logging.Write("Rarekiller: -- lets take a look who wins :)");
                            }
                            while (o.TaggedByOther && !o.Dead)
                            {
                                if (Rarekiller.inCombat) return;
                                Thread.Sleep(1000);
                            }
                            if (o.Dead)
                            {
                                Logging.Write("Rarekiller: {0} was killed by another Player", o.Name);
                                Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                                Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller: Blacklist Mob for 60 Minutes.");
                                return;
                            }
                        }

						if(Rarekiller.Settings.ScreenRarekiller && Rarekiller.Settings.ScreenFound)
						{
							Lua.DoString("TakeScreenshot()");
							Thread.Sleep(300);
							Logging.WriteDebug("Rarekiller Part Rarekiller: Take Screenshot before Pull");
						}
						Logging.WriteDebug("Rarekiller: Pull at Mob Location: {0} / {1} / {2}", Convert.ToString(o.X), Convert.ToString(o.Y), Convert.ToString(o.Z));
						Logging.WriteDebug("Rarekiller: ... my Location: {0} / {1} / {2}", Convert.ToString(Me.X), Convert.ToString(Me.Y), Convert.ToString(Me.Z));
						Logging.Write("Rarekiller: Target is Flying - {0}", o.IsFlying);
						// move closer again
						if (Rarekiller.Settings.GroundMountMode && ((o.Entry == 33687) || (o.Entry == 32630) || (o.Entry == 32491) || (o.Entry == 50057) || (o.Entry == 50005) || (o.Entry == 50052) || (o.Entry == 49913)))
							o.Face();
						else
						{
							while (o.Location.Distance(Me.Location) > Convert.ToInt64(Rarekiller.Settings.Range))
							{
								if (Rarekiller.Settings.GroundMountMode || ForceGround)
									Navigator.MoveTo(o.Location);
								else
									Flightor.MoveTo(o.Location);
								Thread.Sleep(100);
							}
						}
	//----------------- Dismount -----------------------
						if (Me.IsFlying && !((o.Entry == 33687) || (o.Entry == 32630) || (o.Entry == 32491) || (o.Entry == 50057)
                        || (o.Entry == 50005) || (o.Entry == 50052) || (o.Entry == 49913) || (o.Entry == 29753) || (o.Entry == 3868) || o.IsFlying))
						{
							//Descend to Land
							WoWMovement.Move(WoWMovement.MovementDirection.Descend);
							Thread.Sleep(1000);
							if(Me.IsFlying && !Rarekiller.inCombat)
								Thread.Sleep(1000);
							WoWMovement.MoveStop();
							//Dismount
                            if (Me.Auras.ContainsKey("Flight Form"))
                                Lua.DoString("CancelShapeshiftForm()");
                            else if (Me.Mounted)
                                Lua.DoString("Dismount()");

							Thread.Sleep(150);
						}

					}
					else
						o.Target();
// ----------------- Pull Part --------------------
					ForceGround = false;
					FirstTry = true;
					if (!Me.Mounted && ((Me.Class == WoWClass.Hunter) || (Me.Class == WoWClass.Warlock)))
						Lua.DoString(string.Format("RunMacroText(\"/petpassive\")"), 0);

                    if ((o.Entry == 3868) && !Rarekiller.Settings.BloodseekerKill)
                    {
                        while (!o.Dead)
                        {
                            WoWMovement.MoveStop();
                            o.Target();
                            o.Face();
                            Thread.Sleep(1000);
                        }
                        return;
                    }
					
                    if (!(Rarekiller.Settings.DefaultPull) && SpellManager.HasSpell(Rarekiller.Settings.Pull))
                        CastSuccess = RarekillerSpells.CastSafe(Rarekiller.Settings.Pull, o, false);
                    else if (SpellManager.HasSpell(Rarekiller.Spells.FastPullspell))
                        CastSuccess = RarekillerSpells.CastSafe(Rarekiller.Spells.FastPullspell, o, false);
                    else if ((o.Entry != 32630) && (o.Entry != 32491) && (o.Entry != 33687) && (o.Entry != 50057) && SpellManager.HasSpell(Rarekiller.Spells.LowPullspell))
                    {
                        if (Me.Auras.ContainsKey("Flight Form") && (o.Entry != 51236))
                            Lua.DoString("CancelShapeshiftForm()");
                        else if (Me.Mounted && (o.Entry != 51236))
                            Lua.DoString("Dismount()");

                        Thread.Sleep(150);
                        WoWMovement.MoveStop();
                        CastSuccess = RarekillerSpells.CastSafe(Rarekiller.Spells.LowPullspell, o, true);
						if (o.Entry == 51236)
							Lua.DoString("StartAttack()");
                    }
                    else
                    {
                        Logging.Write(System.Drawing.Color.Red, "Rarekiller: I have no valid Pullspell - set Range to 3 for next try");
                        Rarekiller.Settings.Range = "3";
                        return;
                    }

                    if (CastSuccess)
                        Logging.Write("Rarekiller: Successfully pulled {0}", o.Name);
                    else
                    {
                        Logging.Write("Rarekiller: Pull fails - set Range to 3 for next try", o.Name);
                        Rarekiller.Settings.Range = "3";
                    }
					if (!Me.Mounted && ((Me.Class == WoWClass.Hunter) || (Me.Class == WoWClass.Warlock)))
						Lua.DoString(string.Format("RunMacroText(\"/petdefensive\")"), 0);

                    Thread.Sleep(100);
					WoWMovement.MoveStop();
                    Logging.WriteDebug("Rarekiller: Use Quick Slowfall: {0} Mob: {1}", Me.IsFalling, o.Name);
                    if (Me.IsFalling && Rarekiller.Settings.UseSlowfall && ((o.Entry == 29753) || (o.Entry == 3868) || (o.Entry == 32491) || (o.Entry == 32630) || (o.Entry == 33687)))
					{
						Thread.Sleep(200);
						Rarekiller.Slowfall.HelpFalling();
					}
					if(Me.CurrentTarget != o)
						o.Target();
					o.Face();
                    return;					
                }
                else if (o.Dead)
                {
                    if (o.CanLoot)
                    {
// ----------------- Loot Helper for all killed Rare Mobs ---------------------
                        Logging.Write("Rarekiller: Found lootable corpse, move to him");

// ----------------- Move to Corpse -------------------
                        if (Rarekiller.Settings.GroundMountMode && (!(o.Entry == 50005)			// Poseidus Vashir
                            || !(o.Entry == 50052)			// Burgy Blackheart Vashir
                            || !(o.Entry == 49913)			// Lady La-La Vashir
                            )) //In Vashir you have to use Flightor !
                            ForceGround = true;
                        else
                            ForceGround = false;
                        Logging.Write("Rarekiller Part MoveTo: Move to target");
						while (o.Location.Distance(Me.Location) > 5)
						{
							if (Rarekiller.Settings.GroundMountMode || ForceGround)
								Navigator.MoveTo(o.Location);
							else
								Flightor.MoveTo(o.Location);
							Thread.Sleep(100);
							if (Rarekiller.inCombat) return;
						}
						WoWMovement.MoveStop();

                        if (Me.Auras.ContainsKey("Flight Form"))
                            Lua.DoString("CancelShapeshiftForm()");
                        else if (Me.Mounted)
                            Lua.DoString("Dismount()");

						if(Rarekiller.Settings.ScreenRarekiller && Rarekiller.Settings.ScreenSuccess)
						{
							Lua.DoString("TakeScreenshot()");
							Thread.Sleep(500);
                            Logging.WriteDebug("Rarekiller Part Rarekiller: Take Screenshot successfully killed {0}", o.Name);
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
                                Logging.Write("Rarekiller: successfully looted");
                                Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                                Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller: Blacklist Mob for 60 Minutes.");
                                return;
                            }
                            else
                            {
                                Logging.Write("Rarekiller: Loot failed, try again");
                                loothelper = loothelper + 1;
                            }
                            Logging.Write("Rarekiller: Loot failed 3 Times");
                        }
                    }

                    if (!Blacklist.Contains(o.Guid))
                    {
                        Logging.Write("Rarekiller: Find {0}, but sadly he's dead", o.Name);
                        Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist60));
                        Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller: Blacklist Mob for 60 Minutes.");
                    }
                }
            }
        }

        public void ShootDownVyra()
        {
			bool CastSuccessThrow = true;
			bool CastSuccessShoot = true;

            if (Rarekiller.Settings.DeveloperLogs)
                Logging.WriteDebug("Rarekiller: Scan for Vyragosa");
            ObjectManager.Update();
            List<WoWUnit> objList = ObjectManager.GetObjectsOfType<WoWUnit>()
                .Where(o => ((o.Entry == 32630)
                    || ((o.Entry == 33687) && Rarekiller.Settings.TestShootMob) //Testcase Chillmaw
                ))
                .OrderBy(o => o.Distance).ToList();
            foreach (WoWUnit o in objList)
            {
                if (!o.Dead && o.CurrentTarget.IsMe && (o.Location.Distance(Me.Location) > 5))
                {
                    Logging.Write("Rarekiller Part Vyragosa: Shoot down Vyragosa");
                    ShootDown(o);
                }
            }
        }

        public void ShootDown(WoWUnit Unit)
        {
// Quick and dirty, Try and Error to find out if I could shoot or throw
            while (!Unit.Dead)
            {
                if (SpellManager.HasSpell("Throw") && SpellManager.CanCast("Throw"))
                {
                    RarekillerSpells.CastSafe("Throw", Unit, true);
                    Logging.WriteDebug("Rarekiller Part Shootdown: Throw");
                }
                if (SpellManager.HasSpell("Shoot") && SpellManager.CanCast("Shoot"))
                {
                    RarekillerSpells.CastSafe("Shoot", Unit, true);
                    Logging.WriteDebug("Rarekiller Part Shootdown: Shoot");
                }
                if (Rarekiller.Settings.TestShootMob && SpellManager.HasSpell("Lightning Bolt") && SpellManager.CanCast("Lightning Bolt"))
                {
                    RarekillerSpells.CastSafe("Lightning Bolt", Unit, true);
                    Logging.WriteDebug("Rarekiller Part Shootdown: Lightning Bolt");
                }
                if (SpellManager.HasSpell("Heroic Throw") && SpellManager.CanCast("Heroic Throw"))
                {
                    RarekillerSpells.CastSafe("Heroic Throw", Unit, true);
                    Logging.WriteDebug("Rarekiller Part Shootdown: Heroic Throw");
                }
                if (SpellManager.HasSpell("Deadly Throw") && SpellManager.CanCast("Deadly Throw"))
                {
                    RarekillerSpells.CastSafe("Deadly Throw", Unit, true);
                    Logging.WriteDebug("Rarekiller Part Shootdown: Deadly Throw");
                }
            }
        }
    }
}
