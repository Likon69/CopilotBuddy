using System;
using System.Linq;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Logic;
using Styx.Logic.POI;
using System.Diagnostics;

namespace HighVoltz
{
    public class Util1
    {
        static LocalPlayer me = ObjectManager.Me;

        static public bool IsItemInBag(uint entry)
        {
            return me.BagItems.Any(i => i.Entry == entry);
        }

        static public WoWItem GetIteminBag(uint entry)
        {
            return ObjectManager.Me.BagItems.FirstOrDefault(i => i.Entry == entry);
        }

        static public void UseItemByID(int ID)
        {
            Lua.DoString("UseItemByName(\"" + ID + "\")");
        }

        static public void EquipItemByName(String name)
        {
            Lua.DoString("EquipItemByName (\"" + name + "\")");
        }

        static public void EquipItemByID(uint ID)
        {
            Lua.DoString("EquipItemByName ({0})", ID);
        }

        static public bool IsLureOnPole 
        { 
            get 
            {
                //if poolfishing, dont need lure say we have one
                if (AutoAngler.Instance.MySettings.Poolfishing)
                    return true;
                return Lua.GetReturnValues("return GetWeaponEnchantInfo()")[0] == "1"; 
            } 
        }

        static public void BlacklistPool(WoWGameObject pool, TimeSpan time, string reason)
        {
            Blacklist.Add(pool.Guid, time);
            AutoAngler.Instance.Log("Blacklisting {0} for {1} Reason: {2}", pool.Name, time, reason);
            BotPoi.Current = new BotPoi(PoiType.None);
        }

        static uint _ping = Lua.GetReturnVal<uint>("return GetNetStats()", 3);
        static Stopwatch _pingSW = new Stopwatch();
        /// <summary>
        /// Returns WoW's ping, refreshed every 30 seconds.
        /// </summary>
        static public uint WoWPing
        {
            get {
                if (!_pingSW.IsRunning)
                    _pingSW.Start();
                if (_pingSW.ElapsedMilliseconds > 30000)
                {
                    _ping = Lua.GetReturnVal<uint>("return GetNetStats()", 3);
                    _pingSW.Reset();
                    _pingSW.Start();
                }
                return _ping;
            }
        }
    }
}
