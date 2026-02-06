#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Styx.Combat.CombatRoutine;
using Styx.Database;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.WoWInternals;

namespace Styx.Logic.Profiles
{
    /// <summary>
    /// Manages vendor NPCs from profiles.
    /// </summary>
    public class VendorManager
    {
        private readonly List<Vendor> _filteredVendors;

        /// <summary>
        /// Creates an empty vendor manager.
        /// </summary>
        public VendorManager()
        {
            Blacklist = new HashSet<Vendor>();
            AllVendors = new List<Vendor>();
            ForcedVendors = new List<Vendor>();
        }

        /// <summary>
        /// Creates a vendor manager from XML.
        /// </summary>
        public VendorManager(XElement element) : this()
        {
            foreach (var child in element.Elements().ToList())
            {
                try
                {
                    if (child.Name != "Vendor")
                        throw new ProfileUnknownElementException(child, "Vendor");

                    AllVendors.Add(new Vendor(child));
                }
                catch (ProfileException ex)
                {
                    Logging.WriteException(ex);
                }
            }
            _filteredVendors = AllVendors;
        }

        /// <summary>
        /// Gets or sets forced vendors that override profile vendors.
        /// </summary>
        public List<Vendor> ForcedVendors { get; set; }

        /// <summary>
        /// Gets all vendors in the profile.
        /// </summary>
        public List<Vendor> AllVendors { get; private set; }

        /// <summary>
        /// Gets vendors grouped by type, excluding blacklisted.
        /// </summary>
        public Lookup<Vendor.VendorType, Vendor> Vendors
        {
            get
            {
                if (_filteredVendors == null)
                    return null;
                RemoveBlacklisted();
                return (Lookup<Vendor.VendorType, Vendor>)_filteredVendors.ToLookup(v => v.Type);
            }
        }

        /// <summary>
        /// Gets the blacklisted vendors.
        /// </summary>
        public HashSet<Vendor> Blacklist { get; private set; }

        /// <summary>
        /// Gets the closest vendor of any type.
        /// </summary>
        public Vendor GetClosestVendor()
        {
            return GetClosestVendor(Vendor.VendorType.Unknown);
        }

        /// <summary>
        /// Gets the closest vendor of a specific type.
        /// For Sell type, also accepts Repair and Ammo vendors (they can all buy items).
        /// </summary>
        public Vendor GetClosestVendor(Vendor.VendorType type)
        {
            try
            {
                List<Vendor> source = null;

                // Use forced vendors if available
                if (ForcedVendors != null && ForcedVendors.Count > 0)
                {
                    source = ForcedVendors.Where(v => MatchesVendorType(v, type)).ToList();
                }
                else
                {
                    if (type != Vendor.VendorType.Unknown)
                    {
                        // For Sell type, also accept Repair and Ammo vendors (like HB 6.2.3)
                        if (type == Vendor.VendorType.Sell)
                        {
                            source = AllVendors?.Where(v => 
                                v.Type == Vendor.VendorType.Sell || 
                                v.Type == Vendor.VendorType.Repair ||
                                v.Type == Vendor.VendorType.Ammo).ToList();
                        }
                        else if (Vendors != null)
                        {
                            source = Vendors.Contains(type) ? Vendors[type].ToList() : null;
                        }
                    }
                    else
                    {
                        source = AllVendors;
                    }
                }

                if (source == null || source.Count == 0)
                {
                    // Only fall back to Data.bin if FindVendorsAutomatically is enabled
                    // AND the profile has no vendors defined at all
                    if (Styx.Helpers.CharacterSettings.Instance.FindVendorsAutomatically && 
                        (AllVendors == null || AllVendors.Count == 0))
                    {
                        try
                        {
                            NpcResult nearestNpc = NpcQueries.GetNearestNpc(
                                StyxWoW.Me.FactionTemplate.Faction,
                                StyxWoW.Me.MapId,
                                StyxWoW.Me.Location,
                                type.AsNpcFlag());
                            if (nearestNpc != null)
                            {
                                Logging.WriteDebug("[VendorManager] Using Data.bin fallback: {0} Entry={1}", 
                                    nearestNpc.Name, nearestNpc.Entry);
                                return new Vendor(nearestNpc.Entry, nearestNpc.Name, type, nearestNpc.Location);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging.Write(ex.ToString());
                        }
                    }
                    else
                    {
                        Logging.WriteDebug("[VendorManager] No vendor found for type {0}. Profile has {1} vendors. FindVendorsAutomatically={2}", 
                            type, AllVendors?.Count ?? 0, Styx.Helpers.CharacterSettings.Instance.FindVendorsAutomatically);
                    }
                    return null;
                }

                WoWPoint location = ObjectManager.Me.Location;
                WoWClass playerClass = StyxWoW.Me.Class;

                if (type == Vendor.VendorType.Train)
                {
                    var vendor = source
                        .Where(v => v.TrainClass == playerClass)
                        .OrderBy(v => location.Distance(v.Location))
                        .FirstOrDefault();
                    if (vendor != null)
                        Logging.WriteDebug("[VendorManager] Found trainer from profile: {0} Entry={1}", vendor.Name, vendor.Entry);
                    return vendor;
                }
                else
                {
                    var vendor = source
                        .Where(v => !Blacklist.Contains(v))
                        .OrderBy(v => location.Distance(v.Location))
                        .FirstOrDefault();
                    if (vendor != null)
                        Logging.WriteDebug("[VendorManager] Found vendor from profile: {0} Entry={1} Type={2}", vendor.Name, vendor.Entry, vendor.Type);
                    return vendor;
                }
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
                return null;
            }
        }

        /// <summary>
        /// Removes blacklisted vendors from the filtered list.
        /// </summary>
        private void RemoveBlacklisted()
        {
            _filteredVendors?.RemoveAll(v => Blacklist.Contains(v));
        }

        /// <summary>
        /// Checks if a vendor matches the requested type.
        /// For Sell type, also matches Repair and Ammo vendors.
        /// </summary>
        private static bool MatchesVendorType(Vendor vendor, Vendor.VendorType type)
        {
            if (type == Vendor.VendorType.Unknown)
                return true;
            
            // For Sell type, also accept Repair and Ammo vendors
            if (type == Vendor.VendorType.Sell)
            {
                return vendor.Type == Vendor.VendorType.Sell || 
                       vendor.Type == Vendor.VendorType.Repair ||
                       vendor.Type == Vendor.VendorType.Ammo;
            }
            
            return vendor.Type == type;
        }
    }
}
