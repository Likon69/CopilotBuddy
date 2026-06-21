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
    class RarekillerRaptorNest
    {
		public static LocalPlayer Me = ObjectManager.Me;
        private static Stopwatch BlacklistTimer = new Stopwatch();
        Int64 Place = 0;

        public void findAndPickupNest()
        {

            if (Rarekiller.Settings.DeveloperLogs)
                Logging.WriteDebug("Rarekiller: Scan for RaptorNest");
			ObjectManager.Update();
            List<WoWGameObject> objList = ObjectManager.GetObjectsOfType<WoWGameObject>()
                .Where(o => ((o.Entry == 202082)
                || (o.Entry == 202080)
                || (o.Entry == 202083)
                || (o.Entry == 202081)
                || ((o.Entry == 206195) && Rarekiller.Settings.TestRaptorNest) //Testcase Thundermar Ale Keg
                ))
                .OrderBy(o => o.Distance).ToList();
            foreach (WoWGameObject o in objList)
            {
                Logging.Write(System.Drawing.Color.MediumPurple, "Rarekiller: Find Raptornest {0} ID {1}", o.Name, o.Entry);
				if(Blacklist.Contains(o.Guid))
					return;
                if (Rarekiller.inCombat)
                {
                    Logging.Write("Rarekiller Part RaporNest: ... but first I have to finish fighting another Mob.");
                    return;
                }
                if (Me.IsOnTransport)
                {
                    Logging.Write("Rarekiller Part RaporNest: ... but I'm on a Transport.");
                    return;
                }
				
				if((o.Entry == 202083) && (Me.IsFlying || o.Location.Distance(Me.Location) > 30))
				{
// ----------------- Distancecheck because of the Spawn Place is Underground
					Logging.Write("Rarekiller Part RaporNest: {0} is more then 20 yd away and Underground", o.Name);
					Logging.Write(System.Drawing.Color.MediumPurple, "Rarekiller Part RaporNest: You have to place me next to him, if you want to hunt this Mob.");
					Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
					Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller Part RaporNest: Blacklist Nest for 5 Minutes.");
					return;
				}
				if(Rarekiller.BlacklistMobsList.ContainsKey(Convert.ToInt32(o.Entry)))
				{
					Logging.Write("Rarekiller Part RaporNest: {0} is Member of the BlacklistedMobs.xml", o.Name);
					Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist15));
					Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller Part RaporNest: Blacklist Nest for 15 Minutes.");
					return;
				}
				
				while (Me.IsCasting)
                {
                    Thread.Sleep(100);
                }
                if (Rarekiller.Settings.Alert)
                {
                    if (File.Exists(Rarekiller.Settings.SoundfileFoundRare))
                        new SoundPlayer(Rarekiller.Settings.SoundfileFoundRare).Play();
                    else if (File.Exists(Rarekiller.Soundfile))
                        new SoundPlayer(Rarekiller.Soundfile).Play();
                    else
                        Logging.Write(System.Drawing.Color.Red, "Rarekiller Part RaptorNest: playing Soundfile failes");
                }
                Logging.Write("Rarekiller Part MoveTo: Move to Nest");
                BlacklistTimer.Reset();
                BlacklistTimer.Start();
				Place = Me.ZoneId;
				while (o.Location.Distance(Me.Location) > 4)
				{
					if (Rarekiller.Settings.GroundMountMode)
						Navigator.MoveTo(o.Location);
					else
						Flightor.MoveTo(o.Location);
					Thread.Sleep(100);
					if (Rarekiller.inCombat) return;
                    if (Rarekiller.Settings.BlacklistCheck && (BlacklistTimer.Elapsed.TotalSeconds > (Convert.ToInt32(Rarekiller.Settings.BlacklistTime))))
                    {
                        Logging.Write(System.Drawing.Color.Red, "Rarekiller Part MoveTo: Can't reach Nest {0}, Blacklist and Move on", o.Name);
                        Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        BlacklistTimer.Reset();
                        WoWMovement.MoveStop();
                        return;
                    }
                    if (Rarekiller.Settings.ZoneSave && (Me.ZoneId != Place))
                    {
                        Logging.Write(System.Drawing.Color.Red, "Rarekiller Part MoveTo: Left Zone while move to {0} is not allowed", o.Name);
                        Blacklist.Add(o.Guid, TimeSpan.FromSeconds(Rarekiller.Settings.Blacklist5));
                        Logging.Write(System.Drawing.Color.DarkOrange, "Rarekiller: Blacklist Nest for 5 Minutes.");
                        BlacklistTimer.Reset();
                        WoWMovement.MoveStop();
                        return;
                    }

				}
                BlacklistTimer.Reset();
				Thread.Sleep(300);
				WoWMovement.MoveStop();	
                Logging.WriteDebug("Rarekiller Part RaptorNest: Nest Location: {0} / {1} / {2}", Convert.ToString(o.X), Convert.ToString(o.Y), Convert.ToString(o.Z));
                Logging.WriteDebug("Rarekiller Part RaptorNest: My Location: {0} / {1} / {2}", Convert.ToString(Me.X), Convert.ToString(Me.Y), Convert.ToString(Me.Z));
                if (Me.Auras.ContainsKey("Flight Form"))
                    Lua.DoString("CancelShapeshiftForm()");
                else if (Me.Mounted)
                    Lua.DoString("Dismount()");

				Thread.Sleep(500);
                o.Interact();
				Thread.Sleep(2000);
				Lua.DoString("RunMacroText(\"/click StaticPopup1Button1\");");
				Thread.Sleep(4000);
                Logging.Write("Rarekiller Part RaptorNest: Interact with {0} - ID {1}", o.Name, o.Entry);
            }
        }
    }
}
