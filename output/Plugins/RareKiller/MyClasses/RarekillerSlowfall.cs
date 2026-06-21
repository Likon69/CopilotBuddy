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
    class RarekillerSlowfall
    {
        public static LocalPlayer Me = ObjectManager.Me;

        public void HelpFalling()
        {
			int UseSlowfall = 1;
			if ((Me.Class == WoWClass.Druid) && !Rarekiller.inCombat && !Me.HasAura("Snowfall Lager") && !Me.HasAura("Parachute") && !Me.HasAura(Rarekiller.Settings.SlowfallSpell) 
				&& !Me.HasAura("Levitate") && Rarekiller.Settings.Spell && !Me.HasAura("Flight Form") && !Me.HasAura("Swift Flight Form") && !Me.HasAura("Slow Fall")
				&& ((SpellManager.CanCast("Swift Flight Form") && SpellManager.HasSpell("Swift Flight Form")) || (SpellManager.CanCast("Flight Form") && SpellManager.HasSpell("Flight Form"))))
			{
				if (SpellManager.HasSpell("Swift Flight Form") && SpellManager.CanCast("Swift Flight Form"))
					SpellManager.Cast("Swift Flight Form");
				else
					SpellManager.Cast("Flight Form");
				Thread.Sleep(300);
				if (Me.HasAura("Flight Form") || Me.HasAura("Swift Flight Form"))
				{
					Logging.Write("Rarekiller Part Slowfall: Used Slowfall Ability Flight Form");
					return;
				}
			}
			if (!Me.HasAura("Snowfall Lager") && !Me.HasAura("Parachute") && !Me.HasAura(Rarekiller.Settings.SlowfallSpell) && !Me.HasAura("Slow Fall") 
				&& !Me.HasAura("Flight Form") && !Me.HasAura("Swift Flight Form") && !Me.HasAura("Levitate") && Rarekiller.Settings.Spell 
				&& SpellManager.CanCast(Rarekiller.Settings.SlowfallSpell) && SpellManager.HasSpell(Rarekiller.Settings.SlowfallSpell))
			{
				SpellManager.Cast(Rarekiller.Settings.SlowfallSpell);
				Thread.Sleep(300);
				if (Me.HasAura(Rarekiller.Settings.SlowfallSpell))
				{
					Logging.Write("Rarekiller Part Slowfall: Used Slowfall Ability {0}", Rarekiller.Settings.SlowfallSpell);
					return;
				}
			}
			if (!Me.HasAura("Snowfall Lager") && !Me.HasAura("Parachute") && !Me.HasAura(Rarekiller.Settings.SlowfallSpell) && !Me.HasAura("Flight Form") && !Me.HasAura("Swift Flight Form")
				&& !Me.HasAura("Slow Fall") && !Me.HasAura("Levitate") && Rarekiller.Settings.Cloak && (Me.Inventory.Equipped.Back.Cooldown == 0))
			{
				Me.Inventory.Equipped.Back.Use();
				Thread.Sleep(300);
				if (Me.HasAura("Parachute") || Me.HasAura("Slow Fall"))
				{
					Logging.Write("Rarekiller Part Slowfall: Used Slowfall Ability Cloak");
					return;
				}
			}
			if (!Me.HasAura("Snowfall Lager") && !Me.HasAura("Parachute") && !Me.HasAura(Rarekiller.Settings.SlowfallSpell) && !Me.HasAura("Flight Form") && !Me.HasAura("Swift Flight Form")
				&& !Me.HasAura("Slow Fall") && !Me.HasAura("Levitate") && Rarekiller.Settings.Item)
			{
				Lua.DoString("UseItemByName(\"" + Rarekiller.Settings.SlowfallItem + "\")"); // or use Item
				Thread.Sleep(300);
				if (Me.HasAura("Snowfall Lager") || Me.HasAura("Parachute") || Me.HasAura("Slow Fall"))
				{
					Logging.Write("Rarekiller Part Slowfall: Used Slowfall Ability {0}", Rarekiller.Settings.SlowfallItem);
					return;
				}
			}
			if (!Me.HasAura("Snowfall Lager") && !Me.HasAura("Parachute") && !Me.HasAura(Rarekiller.Settings.SlowfallSpell) && !Me.HasAura("Flight Form") && !Me.HasAura("Swift Flight Form")
				&& !Me.HasAura("Slow Fall") && !Me.HasAura("Levitate") && SpellManager.CanCast("Slow Fall") && SpellManager.HasSpell("Slow Fall"))
			{
				SpellManager.Cast("Slow Fall");
				Thread.Sleep(300);
				if (Me.HasAura("Slow Fall"))
				{
					Logging.Write("Rarekiller Part Slowfall: Used Slowfall Ability Slow Fall");
					return;
				}
			}
			if (!Me.HasAura("Snowfall Lager") && !Me.HasAura("Parachute") && !Me.HasAura(Rarekiller.Settings.SlowfallSpell) && !Me.HasAura("Flight Form") && !Me.HasAura("Swift Flight Form")
				&& !Me.HasAura("Slow Fall") && !Me.HasAura("Levitate") && SpellManager.CanCast("Levitate") && SpellManager.HasSpell("Levitate"))
			{
				SpellManager.Cast("Levitate");
				Thread.Sleep(300);
				if (Me.HasAura("Levitate"))
				{
					Logging.Write("Rarekiller Part Slowfall: Used Slowfall Ability Levitate");
					return;
				}
			}
			
			if (Me.HasAura("Snowfall Lager") || Me.HasAura("Parachute") || Me.HasAura(Rarekiller.Settings.SlowfallSpell) || Me.HasAura("Slow Fall") || Me.HasAura("Levitate")
				|| Me.HasAura("Flight Form") || Me.HasAura("Swift Flight Form"))
			{
				Logging.Write("Rarekiller Part Slowfall: Slowfall successfull, Parachute to Ground");
				//Überprüfen:

				if (Me.CurrentTarget != null)
				{
					if (Me.CurrentTarget.IsMe)
						Me.ClearTarget();
					if(!Me.CurrentTarget.IsMe)
						Me.CurrentTarget.Face();
				}
                //while (Me.IsFalling && (Me.HasAura("Snowfall Lager") || Me.HasAura("Parachute") || Me.HasAura(Rarekiller.Settings.SlowfallSpell) || Me.HasAura("Slow Fall") || Me.HasAura("Levitate")
                //|| Me.HasAura("Flight Form") || Me.HasAura("Swift Flight Form")))
                //    Thread.Sleep(100);
			}
			else
			{
				if (Me.Dead || Me.IsGhost)
				{
					Logging.Write("Rarekiller Part Slowfall: I'm dead");
					return;
				}
				if (!Me.IsFalling)
				{
					Logging.Write("Rarekiller Part Slowfall: Don't fall any more");
					return;
				}


				if (UseSlowfall >= 3)
				{
					Logging.Write("Rarekiller Part Slowfall: Slowfall failed 3 Times");
					return;
				}
				Logging.Write("Rarekiller Part Slowfall: Slowfall failed");
				UseSlowfall = UseSlowfall + 1;
				Thread.Sleep(300);
				Rarekiller.Slowfall.HelpFalling();
			}
// Slowfall Part End
        }
    }
}
