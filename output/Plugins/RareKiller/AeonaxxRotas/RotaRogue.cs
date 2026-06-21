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
    class RotaRogue
    {
        public static LocalPlayer Me = ObjectManager.Me;

        public void RogueKill(WoWUnit Unit)
        {
			if (SpellManager.HasSpell("Revealing Strike") && !SpellManager.Spells["Revealing Strike"].Cooldown && !Unit.Dead && (Me.ComboPoints > 4))
			{
				RarekillerSpells.CastSafe("Revealing Strike", Unit, true);
				Thread.Sleep(100);
				return;
			}
			if (SpellManager.HasSpell("Eviscerate") && !SpellManager.Spells["Eviscerate"].Cooldown && !Unit.Dead && (Me.ComboPoints > 4))
			{
				RarekillerSpells.CastSafe("Eviscerate", Unit, true);
				Thread.Sleep(100);
				return;
			}
			
			if (SpellManager.HasSpell("Sinister Strike") && !SpellManager.Spells["Sinister Strike"].Cooldown && !Unit.Dead)
			{
				RarekillerSpells.CastSafe("Sinister Strike", Unit, true);
				Thread.Sleep(100);
				return;
			}
        }
    }    
}