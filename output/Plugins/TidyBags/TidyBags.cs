/*
 * Tidy Bags v3.0.1.8 by LiquidAtoR
 * 
 * This is a trivial little addon that will tidy up on-use items like Clams and
 * Borean Leather Scraps.  It uses a stopwatch to stop it spamming Pulse() and
 * only runs when out of combat.  This item list is a little small at the moment
 * but can easily be extended. Originally Created by Ryns.
 * 
 * Credits to Ryns, MaiN, erenion, TIA, ShamWOW (Bobby53) and Gilderoy for their contributions
 * I would also like to thank everyone that has reported items that are added here in the list.
 *
 * 2011/06/02  v3.1.0.2
 *                              Too hasty with adding stuff. Corrected as reported by Tomei
 *
 * 2011/06/02  v3.1.0.1
 *              Added Satchels of Helpful Goods as reported by TheBuster
 *                              Added a extra satchel which was missing from the list
 *                              Added Sludge-Covered Object as reported by BlackBook
 *                              Added Hidden Stash as reported by tjhasty
 *
 * 2011/02/02  v3.1.0.0
 *                              Changed some behaviour regarding inventory checks (Only character inventory is checked now)
 *                              Tested with latest test release (2.0.0.3895)
 *
 * 2010/12/28  v3.0.1.8
 *                              Added the first Cataclysm Items (as reported and added by Maffyx)
 *                              More items may possibly follow (when reported and tested)
 *
 * 2010/09/5   v3.0.0.0
 *              Reloaded Codebase and tested against HB 1.9.5.9 and AutoAngler9007. Success!!
 *                              Added Oozing Bag and many Fished Items to the itemlist
 *                              More items may possibly follow (when reported and tested)
 * 
 * 2010/06/11   v2.1.0.0 - Gilderoy update
 *                              Removed the Lua code spam now the check for quantities is done internaly
 *                              Added a little log of the operation bone by the plugin
 * 
 * 2010/04/25   v2.0.0.0
 *                              Adapted Code to HB2 Beta (LiquidAtoR)
 * 
 * 2010/01/21   v1.3.1.0
 *                              Added more items to the list, tried resolve problem with Jaggal Clam opening
 *
 * 2009/12/26   v1.3.0.0
 *              Added more items to the list, added hack for motes & eternals
 *
 * 2009/12/26   v1.2.0.0
 *              Added more items to the list.
 *
 * 2009/12/26   v1.1.0.0
 *              Fix for error "Missing reagent: Borean Leather Scraps" if you
 *              have >1 but <5 in your bags.
 *
 * 2009/12/26   v1.0.0.0
 *              First release of the plugin, seems to work for me!
 * 
 */
namespace PluginTidyBags3
{
    using Styx;
    using Styx.Combat.CombatRoutine;
    using Styx.Helpers;
    using Styx.Logic;
    using Styx.Logic.Combat;
        using Styx.Logic.Inventory;
        using Styx.Logic.Inventory.Frames.Gossip;
    using Styx.Logic.Pathing;
        using Styx.Logic.Profiles;
    using Styx.Plugins.PluginClass;
    using Styx.WoWInternals;
    using Styx.WoWInternals.WoWObjects;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using System.Xml.Linq;

   public class TidyBags3 : HBPlugin
   {
      public override string Name { get { return "Tidy Bags 3 Reloaded"; } }
      public override string Author { get { return "LiquidAtoR"; } }
      public override Version Version { get { return _version; } }
      private readonly Version _version = new Version(3, 1, 0, 2);

      private List<uint> _itemList = new List<uint>()
      {
         5523,  // Small Barnacled Clam
         5524,  // Thick-shelled Clam
         7973,  // Big-mouth Clam
         24476, // Jaggal Clam
         33567, // Borean Leather Scraps
         36781, // Darkwater Clam
         44700, // Brooding Darkwater Clam
         45909, // Giant Darkwater Clam
                 52340, // Abyssal Clam
                 67495, // Strange Bloated Stomach
         37700, // Crystallized Air
         37701, // Crystallized Earth
         37702, // Crystallized Fire
         37703, // Crystallized Shadow
         37704, // Crystallized Life
         37705, // Crystallized Water
         22572, // Mote of Air
         22573, // Mote of Earth 
         22574, // Mote of Fire
         22575, // Mote of Life
         22576, // Mote of Mana
         22577, // Mote of Shadow
         22578, // Mote of Water
                 3352,  // Ooze-covered Bag
                 20768, // Oozing Bag
                 20767, // Scum Covered Bag
                 44663, // Abandoned Adventurer's Satchel
                 27511, // Inscribed Scrollcase
                 20708, // Tightly Sealed Trunk
                 6351, // Dented Crate
                 6352, // Waterlogged Crate
                 6353, // Small Chest
                 6356, // Battered Chest
                 6357, // Sealed Crate
                 13874, // Heavy Crate
                 21113, // Watertight Trunk
                 21150, // Iron Bound Trunk
                 21228, // Mithril Bound Trunk
                 27513, // Curious Crate
                 27481, // Heavy Supply Crate
                 44475, // Reinforced Crate
             67539, // Tiny Treasure Chest
                 67597, // Sealed Crate (level 85 version)
                 24881, // Satchel of Helpful Goods (5-15 1st)
                 24889, // Satchel of Helpful Goods (5-15 others)
                 24882, // Satchel of Helpful Goods (15-25 1st)
                 24890, // Satchel of Helpful Goods (15-25 others)
                 51999, // Satchel of Helpful Goods (iLevel 25)
                 52000, // Satchel of Helpful Goods (31)
                 67248, // Satchel of Helpful Goods (39)
                 52001, // Satchel of Helpful Goods (41)
                 52002, // Satchel of Helpful Goods (50)
                 52003, // Satchel of Helpful Goods (57)
                 52004, // Satchel of Helpful Goods (62)
                 52005, // Satchel of Helpful Goods (66)
                 67250, // Satchel of Helpful Goods (85)
                 32724, // Sludge Covered Object
                 61387 // Hidden Stash
      };

      private static Stopwatch sw = new Stopwatch();
      
      public override void Pulse()
        {
            if (Battlegrounds.IsInsideBattleground || ObjectManager.Me.Mounted || ObjectManager.Me.Combat)
                return;

            if (!sw.IsRunning)
                sw.Start();

            if (sw.Elapsed.TotalSeconds > 1)
            {
                                foreach (WoWItem item in ObjectManager.GetObjectsOfType<WoWItem>())
                {
                    if (_itemList.Contains(item.Entry))
                    {
                                                if (item != null)
                                                {
                                                        if(item.BagSlot != -1)
                                                        {
                                                                if (item.Entry == 33567)
                                                                {
                                                                        if (item.StackCount>=5)
                                                                        {
                                                                                Styx.Helpers.Logging.Write(Color.FromName("DarkRed"), "[TidyBags 3]: Using {0} we have {1}", item.Name, item.StackCount);
                                                                                Lua.DoString("UseItemByName(\"" + item.Name + "\")");
                                                                        }
                                                                }
                                                        else if (item.Entry >= 22572 && item.Entry <= 22578)
                                                                {
                                                                        if (item.StackCount>=10)
                                                                        {
                                                                                Styx.Helpers.Logging.Write(Color.FromName("DarkRed"), "[TidyBags 3]: Using {0} we have {1}", item.Name, item.StackCount);
                                                                                Lua.DoString("UseItemByName(\"" + item.Name + "\")");
                                                                        }
                                                                }
                                                        else if (item.Entry >= 37700 && item.Entry <= 37705)
                                                                {
                                                                        if (item.StackCount>=10)
                                                                                {
                                                                                        Styx.Helpers.Logging.Write(Color.FromName("DarkRed"), "[TidyBags 3]: Using {0} we have {1}", item.Name, item.StackCount);
                                                                                        Lua.DoString("UseItemByName(\"" + item.Name + "\")");
                                                                                }
                                                                }
                                                        else
                                                                {
                                                                        if (item.StackCount>=1)
                                                                                {
                                                                                        Styx.Helpers.Logging.Write(Color.FromName("DarkRed"), "[TidyBags 3]: Using {0} we have {1}", item.Name, item.StackCount);
                                                                                        Lua.DoString("UseItemByName(\"" + item.Name + "\")");
                                                                                }
                                                                }
                                                        }
                                                }
                                        }
                                }
                sw.Reset();
                sw.Start();
            }
                }
    }
}
