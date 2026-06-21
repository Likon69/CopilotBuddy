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
    class RarekillerSpells
    {
        public static LocalPlayer Me = ObjectManager.Me;

		// ------------ Spell Functions
        static public bool CastSafe(string spellName, WoWUnit Unit, bool wait)
        {
            bool SpellSuccess = false;
            if (Me.IsCasting)
            {
                Logging.WriteDebug("Rarekiller Part Spells: I was already Casting");
                return false;
            }
            if (!SpellManager.HasSpell(spellName))
            {
                Logging.WriteDebug("Rarekiller Part Spells: I don't have Spell {0}", spellName);
                return false;
            }

            if (SpellManager.HasSpell(spellName) && !Me.IsCasting)
            {
                if (SpellManager.Spells[spellName].CastTime > 1)
                    WoWMovement.MoveStop();
                Unit.Target();
                Thread.Sleep(100);
                Unit.Face();
                Thread.Sleep(150);
                
                while (StyxWoW.GlobalCooldown)
                {
                    Thread.Sleep(10);
                }

                if (!SpellManager.CanCast(spellName))
                {
                    Logging.WriteDebug("Rarekiller Part Spells: cannot cast spell '{0}' yet - cd={1}, gcd={2}, casting={3} ",
                        SpellManager.Spells[spellName].Name,
                        SpellManager.Spells[spellName].Cooldown,
                        StyxWoW.GlobalCooldown,
                        Me.IsCasting
                        );
                    return false;
                }

                SpellSuccess = SpellManager.Cast(spellName);
				
				Thread.Sleep(200);
				
				//if (StyxWoW.GlobalCooldown || Me.IsCasting)
				//	SpellSuccess = true;
				if (SpellSuccess)
					Logging.Write("Rarekiller Part Spells: * {0}.", spellName);	
                if (wait)
                {
                    while (StyxWoW.GlobalCooldown || Me.IsCasting)
                    {
                        Thread.Sleep(100);
                    }
                }

                Logging.WriteDebug("Rarekiller Part Spells: Spell successfull? {0}.", SpellSuccess);
                return SpellSuccess;
            }

            Logging.WriteDebug("Rarekiller Part Spells: Can't cast {0}.", spellName);
            return false;
        }

        public string FastPullspell
        {
            get
            {
                if (SpellManager.HasSpell("Shadow Word: Pain"))
                    return "Shadow Word: Pain";
                else if (SpellManager.HasSpell("Ice Lance"))
                    return "Ice Lance";
                else if (SpellManager.HasSpell("Heroic Throw"))
                    return "Heroic Throw";
                else if (SpellManager.HasSpell("Arcane Shot"))
                    return "Arcane Shot";
                else if (SpellManager.HasSpell("Fan of Knives"))
                    return "Fan of Knives";
                else if (SpellManager.HasSpell("Icy Touch"))
                    return "Icy Touch";
                else if (SpellManager.HasSpell("Crusader Strike"))
                    return "Crusader Strike";
                else if (SpellManager.HasSpell("Flame Shock"))
                    return "Flame Shock";
                else if (SpellManager.HasSpell("Corruption"))
                    return "Corruption";
                else if (SpellManager.HasSpell("Moonfire"))
                    return "Moonfire";
                else
                    return "kein Spell";
            }
        }
		
        public string LowPullspell
        {
            get
            {
                if (SpellManager.HasSpell("Shadow Word: Pain"))
                    return "Shadow Word: Pain";
                else if (SpellManager.HasSpell("Ice Lance"))
                    return "Ice Lance";
                else if (SpellManager.HasSpell("Heroic Throw"))
                    return "Heroic Throw";
                else if (SpellManager.HasSpell("Arcane Shot"))
                    return "Arcane Shot";
                else if (SpellManager.HasSpell("Sinister Strike"))
                    return "Sinister Strike";
                else if (SpellManager.HasSpell("Icy Touch"))
                    return "Icy Touch";
                else if (SpellManager.HasSpell("Exorcism"))
                    return "Exorcism";
                else if (SpellManager.HasSpell("Lightning Bolt"))
                    return "Lightning Bolt";
                else if (SpellManager.HasSpell("Shadow Bolt"))
                    return "Shadow Bolt";
                else if (SpellManager.HasSpell("Moonfire"))
                    return "Moonfire";
                else if (SpellManager.HasSpell("Throw"))
                    return "Throw";
                else
                    return "kein Spell";
            }
        }
    }    
}
