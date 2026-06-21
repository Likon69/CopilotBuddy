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
    class RotaMage
    {
        public static LocalPlayer Me = ObjectManager.Me;

        public void MageKill(WoWUnit Unit)
        {
            //Frostmage:
            if (SpellManager.HasSpell("Frost Specialization"))
            {
                if (SpellManager.HasSpell("Frostfire Bolt") && !SpellManager.Spells["Frostfire Bolt"].Cooldown && Me.ActiveAuras.ContainsKey("Brain Freeze"))
                {
                    RarekillerSpells.CastSafe("Frostfire Bolt", Unit, true);
                    Thread.Sleep(100);
                    return;
                }
                if (SpellManager.HasSpell("Frostbolt") && !SpellManager.Spells["Frostbolt"].Cooldown)
                {
                    RarekillerSpells.CastSafe("Frostbolt", Unit, true);
                    Thread.Sleep(100);
                    return;
                }
            }
            //Arcane Mage
            if (SpellManager.HasSpell("Arcane Specialization"))
            {
                if (SpellManager.HasSpell("Arcane Missiles") && !SpellManager.Spells["Arcane Missiles"].Cooldown && Me.ActiveAuras["Arcane Blast"].StackCount > 3 && Me.ActiveAuras.ContainsKey("Arcane Missiles"))
                {
                    RarekillerSpells.CastSafe("Arcane Missiles", Unit, true);
                    Thread.Sleep(100);
                    return;
                }
				if (SpellManager.HasSpell("Arcane Barrage") && !SpellManager.Spells["Arcane Barrage"].Cooldown && Me.ActiveAuras["Arcane Blast"].StackCount > 3)
                {
                    RarekillerSpells.CastSafe("Arcane Barrage", Unit, true);
                    Thread.Sleep(100);
                    return;
                }
                
                if (SpellManager.HasSpell("Arcane Blast") && !SpellManager.Spells["Arcane Blast"].Cooldown)
                {
                    RarekillerSpells.CastSafe("Arcane Blast", Unit, true);
                    Thread.Sleep(100);
                    return;
                }
            }
            //FireMage
            if (SpellManager.HasSpell("Fire Specialization"))
            {
                if (SpellManager.HasSpell("Combustion") && !SpellManager.Spells["Combustion"].Cooldown && Unit.ActiveAuras.ContainsKey("Living Bomb") 
						&& !Unit.ActiveAuras.ContainsKey("Pyroblast") && !Unit.ActiveAuras.ContainsKey("Ignite"))
                {
                    RarekillerSpells.CastSafe("Combustion", Unit, true);
                    Thread.Sleep(100);
                    return;
                }
                if (SpellManager.HasSpell("Living Bomb") && !SpellManager.Spells["Living Bomb"].Cooldown && !Unit.ActiveAuras.ContainsKey("Living Bomb"))
                {
                    RarekillerSpells.CastSafe("Living Bomb", Unit, true);
                    Thread.Sleep(100);
                    return;
                }
                if (SpellManager.HasSpell("Pyroblast") && !SpellManager.Spells["Pyroblast"].Cooldown && Me.ActiveAuras.ContainsKey("Hot Streak"))
                {
                    RarekillerSpells.CastSafe("Pyroblast", Unit, true);
                    Thread.Sleep(100);
                    return;
                }
                if (SpellManager.HasSpell("Flame Orb") && !SpellManager.Spells["Flame Orb"].Cooldown)
                {
                    RarekillerSpells.CastSafe("Flame Orb", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Fireball") && !SpellManager.Spells["Fireball"].Cooldown)
                {
                    RarekillerSpells.CastSafe("Fireball", Unit, true);
                    Thread.Sleep(100);
                }
            }
        }
    }    
}