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
    class RotaPaladin
    {
        public static LocalPlayer Me = ObjectManager.Me;

        public void PaladinKill(WoWUnit Unit)
        {
// -------- Heal
            if (Me.HealthPercent < 10 && SpellManager.HasSpell("Flash of Light"))
            {
                while (Me.HealthPercent < 30 && SpellManager.HasSpell("Flash of Light"))
                {
                    RarekillerSpells.CastSafe("Flash of Light", Me, true);
                    Thread.Sleep(100);
                }
            }
            if (Me.HealthPercent < 30 && SpellManager.HasSpell("Divine Light"))
            {
                while (Me.HealthPercent < 50 && SpellManager.HasSpell("Divine Light"))
                {
                    RarekillerSpells.CastSafe("Divine Light", Me, true);
                    Thread.Sleep(100);
                }
            }
            //Retribution Paladin
            if (SpellManager.HasSpell("Templar's Verdict"))
            {
                if (SpellManager.HasSpell("Seal of Truth") && !SpellManager.Spells["Seal of Truth"].Cooldown && !Me.ActiveAuras.ContainsKey("Seal of Truth") && !Unit.Dead)
                {
                    RarekillerSpells.CastSafe("Seal of Truth", Me, true);
                    Thread.Sleep(100);
                }
				
				if (SpellManager.HasSpell("Guardian of Ancient Kings") && !SpellManager.Spells["Guardian of Ancient Kings"].Cooldown && !Unit.Dead)
                {
                    RarekillerSpells.CastSafe("Guardian of Ancient Kings", Unit, true);
                    Thread.Sleep(100);
                }	
                if (SpellManager.HasSpell("Hammer of Wrath") && !SpellManager.Spells["Hammer of Wrath"].Cooldown && !Unit.Dead)
                {
                    RarekillerSpells.CastSafe("Hammer of Wrath", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Exorcism") && !SpellManager.Spells["Exorcism"].Cooldown && !Unit.Dead)
                {
                    RarekillerSpells.CastSafe("Exorcism", Unit, true);
                    Thread.Sleep(100);
                }
				if (SpellManager.HasSpell("Judgement") && !SpellManager.Spells["Judgement"].Cooldown && !Unit.Dead)
                {
                    RarekillerSpells.CastSafe("Judgement", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Holy Wrath") && !SpellManager.Spells["Holy Wrath"].Cooldown && !Unit.Dead)
                {
                    RarekillerSpells.CastSafe("Holy Wrath", Unit, true);
                    Thread.Sleep(100);
                }				
			}
			else
			{
				if (SpellManager.HasSpell("Seal of Truth") && !SpellManager.Spells["Seal of Truth"].Cooldown && !Me.ActiveAuras.ContainsKey("Seal of Truth") && !Unit.Dead)
                {
                    RarekillerSpells.CastSafe("Seal of Truth", Me, true);
                    Thread.Sleep(100);
                }
				
				if (SpellManager.HasSpell("Exorcism") && !SpellManager.Spells["Exorcism"].Cooldown && !Unit.Dead)
                {
                    RarekillerSpells.CastSafe("Exorcism", Unit, true);
                    Thread.Sleep(100);
                }
				if (SpellManager.HasSpell("Judgement") && !SpellManager.Spells["Judgement"].Cooldown && !Unit.Dead)
                {
                    RarekillerSpells.CastSafe("Judgement", Unit, true);
                    Thread.Sleep(100);
                }
                if (SpellManager.HasSpell("Holy Wrath") && !SpellManager.Spells["Holy Wrath"].Cooldown && !Unit.Dead)
                {
                    RarekillerSpells.CastSafe("Holy Wrath", Unit, true);
                    Thread.Sleep(100);
                }		
				if (SpellManager.HasSpell("Crusader Strike") && !SpellManager.Spells["Crusader Strike"].Cooldown && !Unit.Dead)
                {
                    RarekillerSpells.CastSafe("Crusader Strike", Unit, true);
                    Thread.Sleep(100);
                }
			}
        }
    }    
}