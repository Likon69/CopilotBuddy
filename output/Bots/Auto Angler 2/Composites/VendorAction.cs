using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Action = TreeSharp.Action;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx;
using Styx.WoWInternals;
using Styx.Logic.Profiles;
using TreeSharp;
using Styx.Logic.POI;
using Styx.Logic;
using Styx.WoWInternals.WoWObjects;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Inventory.Frames.MailBox;
using Styx.Logic.Inventory.Frames.Merchant;
namespace HighVoltz.Composites
{
    public class VendorAction:Action
    {
         LocalPlayer _me = ObjectManager.Me;

         protected override RunStatus Run(object context)
         {
             Vendor ven = BotPoi.Current.AsVendor;
             WoWUnit vendor = ObjectManager.GetObjectsOfType<WoWUnit>().
                        FirstOrDefault(m => m.Entry == ven.Entry || m.Entry == ven.Entry2);
             WoWPoint loc = vendor != null ? vendor.Location : ven.Location;
             if (_me.Location.Distance(loc) > 4)
             {
                 if (AutoAngler.Instance.MySettings.Fly)
                     Flightor.MoveTo(WoWMathHelper.CalculatePointFrom(_me.Location, loc, 4));
                 else
                 {
                     if (!ObjectManager.Me.Mounted && Mount.ShouldMount(loc) && Mount.CanMount())
                         Mount.MountUp();
                     Navigator.MoveTo(WoWMathHelper.CalculatePointFrom(_me.Location, loc, 4));
                 }
             }
             else
             {
                 if (MerchantFrame.Instance == null || !MerchantFrame.Instance.IsVisible)
                 {
                     if (vendor == null)
                     {
                         AutoAngler.Instance.Log("No vendor found at location {0}. hearth + logging out instead", loc);
                         BotPoi.Current = new BotPoi(PoiType.InnKeeper);
                         return RunStatus.Failure;
                     }
                     vendor.Interact();
                 }
                 else
                 {
                     // sell all poor and common items not in protected Items list.
                     List<WoWItem> itemList = _me.BagItems.Where(i => !ProtectedItemsManager.Contains(i.Entry) &&
                         !i.IsSoulbound && !i.IsConjured && 
                         (i.Quality == WoWItemQuality.Poor || i.Quality == WoWItemQuality.Common)).ToList();
                     foreach (var item in itemList)
                     {
                         item.UseContainerItem();
                     }
                     MerchantFrame.Instance.RepairAllItems();
                     BotPoi.Current = new BotPoi(PoiType.None);
                 }
             }
             return RunStatus.Success;
         }
    }
}
