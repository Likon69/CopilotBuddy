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
    class RotaDruid
    {
        public static LocalPlayer Me = ObjectManager.Me;
		public static bool EclipseMoon = false;
		public static bool EclipseSun = false;
		
        public void DruidKill(WoWUnit Unit)
        {
			if (Me.ActiveAuras.ContainsKey("Eclipse (Lunar)"))
			{
				EclipseMoon = true;
				EclipseSun = false;
			}
			else if (Me.ActiveAuras.ContainsKey("Eclipse (Solar)"))
			{
				EclipseMoon = false;
				EclipseSun = true;
			}			
			
// -------- Heal
            if (Me.HealthPercent < 10 && SpellManager.HasSpell("Regrowth"))
            {
                while (Me.HealthPercent < 30 && SpellManager.HasSpell("Regrowth"))
                {
                    RarekillerSpells.CastSafe("Regrowth", Me, true);
                    Thread.Sleep(100);
                }
            }
            if (Me.HealthPercent < 30 && SpellManager.HasSpell("Healing Thouch"))
            {
                while (Me.HealthPercent < 50 && SpellManager.HasSpell("Healing Thouch"))
                {
                    RarekillerSpells.CastSafe("Healing Thouch", Me, true);
                    Thread.Sleep(100);
                }
            }
			
			if (SpellManager.HasSpell("Mangle") && SpellManager.HasSpell("Cat Form") && SpellManager.CanCast("Cat Form") && !Me.ActiveAuras.ContainsKey("Cat Form"))
			{
				RarekillerSpells.CastSafe("Cat Form", Me, true);
				Thread.Sleep(100);
				return;
			}
			if (SpellManager.HasSpell("Moonkin Form") && SpellManager.CanCast("Moonkin Form") && !Me.ActiveAuras.ContainsKey("Moonkin Form"))
			{
				RarekillerSpells.CastSafe("Moonkin Form", Me, true);
				Thread.Sleep(100);
				return;
			}
// -------- Damage Cat
			if (Me.ActiveAuras.ContainsKey("Cat Form"))
			{
				if (SpellManager.HasSpell("Rip") && !SpellManager.Spells["Rip"].Cooldown && (Me.ComboPoints > 4))
				{
					RarekillerSpells.CastSafe("Rip", Unit, true);
					Thread.Sleep(100);
					return;
				}
				if (SpellManager.HasSpell("Tiger's Fury") && !SpellManager.Spells["Tiger's Fury"].Cooldown)
				{
					RarekillerSpells.CastSafe("Tiger's Fury", Unit, true);
					Thread.Sleep(100);
					return;
				}
				if (SpellManager.HasSpell("Mangle") && !SpellManager.Spells["Mangle"].Cooldown)
				{
					RarekillerSpells.CastSafe("Mangle", Unit, true);
					Thread.Sleep(100);
					return;
				}
				if (SpellManager.HasSpell("Rake") && !SpellManager.Spells["Rake"].Cooldown)
				{
					RarekillerSpells.CastSafe("Rake", Unit, true);
					Thread.Sleep(100);
					return;
				}
			}
// -------- Damage Cat
			else
			{
				if (SpellManager.HasSpell("Insect Swarm") && !SpellManager.Spells["Rake"].Cooldown && !Unit.ActiveAuras.ContainsKey("Insect Swarm"))
				{
					RarekillerSpells.CastSafe("Insect Swarm", Unit, true);
					Thread.Sleep(100);
					return;
				}
				
				
				if (EclipseSun)
				{
					if (SpellManager.HasSpell("Sunfire") && !SpellManager.Spells["Sunfire"].Cooldown && !Unit.Auras.ContainsKey("Sunfire"))
					{
						RarekillerSpells.CastSafe("Sunfire", Unit, true);
						Thread.Sleep(100);
						return;
					}
					if (SpellManager.HasSpell("Starsurge") && !SpellManager.Spells["Starsurge"].Cooldown && Me.Auras.ContainsKey("Shooting Stars"))
					{
						RarekillerSpells.CastSafe("Starsurge", Unit, true);
						Thread.Sleep(100);
						return;
					}
					if (!SpellManager.HasSpell("Wrath") && !SpellManager.Spells["Wrath"].Cooldown && SpellManager.CanCast("Wrath"))
					{
						RarekillerSpells.CastSafe("Wrath", Unit, true);
						Thread.Sleep(100);
						return;
					}

				}
				else
				{
					if (SpellManager.HasSpell("Moonfire") && !SpellManager.Spells["Moonfire"].Cooldown && !Unit.Auras.ContainsKey("Moonfire"))
					{
						RarekillerSpells.CastSafe("Moonfire", Unit, true);
						Thread.Sleep(100);
						return;
					}
					if (SpellManager.HasSpell("Starsurge") && !SpellManager.Spells["Starsurge"].Cooldown && Me.Auras.ContainsKey("Shooting Stars"))
					{
						RarekillerSpells.CastSafe("Starsurge", Unit, true);
						Thread.Sleep(100);
						return;
					}
					if (SpellManager.HasSpell("Starfire") && !SpellManager.Spells["Starfire"].Cooldown)
					{
						RarekillerSpells.CastSafe("Starfire", Unit, true);
						Thread.Sleep(100);
						return;
					}
				}
			}
        }
    }    
}