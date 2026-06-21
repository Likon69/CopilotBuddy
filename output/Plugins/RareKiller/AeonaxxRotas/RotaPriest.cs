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
    class RotaPriest
    {
        public static LocalPlayer Me = ObjectManager.Me;

        public void PriestKill(WoWUnit Unit)
        {
            if (Me.HealthPercent < 10 && SpellManager.HasSpell("Flash Heal"))
            {
                while (Me.HealthPercent < 30 && SpellManager.HasSpell("Flash Heal"))
                {
                    RarekillerSpells.CastSafe("Flash Heal", Me, true);
                    Thread.Sleep(100);
                }
            }
            if (Me.HealthPercent < 30 && SpellManager.HasSpell("Greater Heal"))
            {
                while (Me.HealthPercent < 50 && SpellManager.HasSpell("Greater Heal"))
                {
                    RarekillerSpells.CastSafe("Greater Heal", Me, true);
                    Thread.Sleep(100);
                }
            }
			if (Me.HealthPercent < 50 && SpellManager.HasSpell("Renew") && !SpellManager.Spells["Renew"].Cooldown)
            {
				RarekillerSpells.CastSafe("Renew", Me, true);
				Thread.Sleep(100);
            }
			if (Me.ManaPercent < 40 && SpellManager.HasSpell("Dispersion") && !SpellManager.Spells["Dispersion"].Cooldown)
            {
				RarekillerSpells.CastSafe("Dispersion", Me, true);
				Thread.Sleep(6500);
            }
			if (Me.ManaPercent < 10 && SpellManager.HasSpell("Shoot") && !SpellManager.Spells["Shoot"].Cooldown)
            {
				while (Me.ManaPercent < 30 && SpellManager.HasSpell("Shoot"))
                {
					RarekillerSpells.CastSafe("Shoot", Unit, true);
					Thread.Sleep(6500);
					return;
				}
            }
			
// -------- Shadow Form if possible
			if (SpellManager.HasSpell("Shadowform") && SpellManager.CanCast("Shadowform") && !Me.ActiveAuras.ContainsKey("Shadowform"))
            {
                RarekillerSpells.CastSafe("Shadowform", Me, true);
                Thread.Sleep(100);
            }

// -------- Damage DOTs Renew
			if (SpellManager.HasSpell("Vampiric Embrance") && !SpellManager.Spells["Vampiric Embrance"].Cooldown && !Me.ActiveAuras.ContainsKey("Vampiric Embrance"))
            {
                RarekillerSpells.CastSafe("Vampiric Embrance", Me, true);
                Thread.Sleep(100);
				return;
            }
            if (SpellManager.HasSpell("Vampiric Touch") && !SpellManager.Spells["Vampiric Touch"].Cooldown && !Unit.ActiveAuras.ContainsKey("Vampiric Touch"))
            {
                RarekillerSpells.CastSafe("Vampiric Touch", Unit, true);
                Thread.Sleep(100);
				return;
            }
			if (SpellManager.HasSpell("Shadow Word: Pain") && !SpellManager.Spells["Shadow Word: Pain"].Cooldown && !Unit.ActiveAuras.ContainsKey("Shadow Word: Pain"))
            {
                RarekillerSpells.CastSafe("Shadow Word: Pain", Unit, true);
                Thread.Sleep(100);
				return;
            }
			if (SpellManager.HasSpell("Devouring Plague") && !SpellManager.Spells["Devouring Plague"].Cooldown && !Unit.ActiveAuras.ContainsKey("Devouring Plague"))
            {
                RarekillerSpells.CastSafe("Devouring Plague", Unit, true);
                Thread.Sleep(100);
				return;
            }

// -------- Damage Spells			
            if (SpellManager.HasSpell("Mind Blast") && !SpellManager.Spells["Mind Blast"].Cooldown)
            {
                RarekillerSpells.CastSafe("Mind Blast", Unit, true);
                Thread.Sleep(100);
                return;
            }
            if (SpellManager.HasSpell("Mind Flay") && !SpellManager.Spells["Mind Flay"].Cooldown)
            {
                RarekillerSpells.CastSafe("Mind Flay", Unit, true);
                Thread.Sleep(100);
                return;
            }
			if (!SpellManager.HasSpell("Shadowform") && SpellManager.HasSpell("Smite") && !SpellManager.Spells["Smite"].Cooldown)
            {
                RarekillerSpells.CastSafe("Smite", Unit, true);
                Thread.Sleep(100);
                return;
            }
			if (!SpellManager.HasSpell("Shadowform") && SpellManager.HasSpell("Holy Fire") && !SpellManager.Spells["Holy Fire"].Cooldown)
            {
                RarekillerSpells.CastSafe("Holy Fire", Unit, true);
                Thread.Sleep(100);
                return;
            }
        }
    }    
}
