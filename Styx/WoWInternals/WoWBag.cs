using System;
using System.Collections.Generic;
using System.Linq;
using GreenMagic;
using Styx.Helpers;
using Styx.WoWInternals.WoWObjects;

namespace Styx.WoWInternals
{
    /// <summary>
    /// Represents a WoW bag container.
    /// Ported from HB 3.3.5a Styx.WoWInternals.WoWBag
    /// </summary>
    public class WoWBag
    {
        private readonly BagStructure _bagStructure;

        /// <summary>
        /// Creates a WoWBag from a bag structure.
        /// </summary>
        internal WoWBag(BagStructure bag) : this(bag, 0U, bag.Slots)
        {
        }

        /// <summary>
        /// Creates a WoWBag from a bag structure with specified slot range.
        /// </summary>
        internal WoWBag(BagStructure bag, uint firstSlotIndex, uint slots)
        {
            try
            {
                _bagStructure = bag;
                FirstSlotIndex = firstSlotIndex;
                Slots = slots;
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
                throw;
            }
        }

        /// <summary>
        /// First slot index in the bag
        /// </summary>
        public uint FirstSlotIndex { get; private set; }

        /// <summary>
        /// Number of slots in the bag
        /// </summary>
        public uint Slots { get; private set; }

        /// <summary>
        /// Name of the bag
        /// </summary>
        public string Name
        {
            get
            {
                try
                {
                    if (!IsInventory)
                    {
                        if (Guid != ObjectManager.LocalGuid)
                        {
                            // BUG-27: Look up WoWContainer to get real bag name (HB 3.3.5a)
                            var container = ObjectManager.GetObjectByGuid<WoWObjects.WoWContainer>(Guid);
                            if (container != null)
                                return container.Name;
                            return "Unknown";
                        }
                    }
                    return "Inventory";
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    return "Unknown";
                }
            }
        }

        /// <summary>
        /// True if this is an inventory bag (not a physical bag)
        /// </summary>
        public bool IsInventory
        {
            get
            {
                try
                {
                    return _bagStructure.IsInventory != 0;
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// GUID of the bag container
        /// </summary>
        public ulong Guid
        {
            get
            {
                try
                {
                    return _bagStructure.Guid;
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    return 0UL;
                }
            }
        }

        /// <summary>
        /// Gets all item GUIDs in the bag
        /// </summary>
        public unsafe ulong[] ItemGuids
        {
            get
            {
                try
                {
                    ulong[] array = new ulong[Slots];
                    if (array.Length == 0)
                        return array;

                    fixed (ulong* ptr = array)
                    {
                        ObjectManager.Wow.ReadBytes(
                            _bagStructure.ItemsBaseAddress + FirstSlotIndex * 8U,
                            (void*)ptr,
                            (int)(Slots * 8U));
                    }

                    return array;
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    return new ulong[0];
                }
            }
        }

        /// <summary>
        /// Gets all items in the bag
        /// </summary>
        public WoWItem[] Items
        {
            get
            {
                try
                {
                    return ItemGuids.Select(guid => ObjectManager.GetObjectByGuid<WoWItem>(guid)).ToArray();
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    return new WoWItem[0];
                }
            }
        }

        /// <summary>
        /// Gets all non-zero item GUIDs (physical items)
        /// </summary>
        public ulong[] PhysicalItemGuids
        {
            get
            {
                try
                {
                    return ItemGuids.Where(u => u != 0UL).ToArray();
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    return new ulong[0];
                }
            }
        }

        /// <summary>
        /// Gets all physical items (non-null)
        /// </summary>
        public WoWItem[] PhysicalItems
        {
            get
            {
                try
                {
                    return Items.Where(i => i != null).ToArray();
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    return new WoWItem[0];
                }
            }
        }

        /// <summary>
        /// Number of free slots
        /// </summary>
        public uint FreeSlots
        {
            get
            {
                try
                {
                    return Slots - UsedSlots;
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    return 0;
                }
            }
        }

        /// <summary>
        /// Number of used slots
        /// </summary>
        public uint UsedSlots
        {
            get
            {
                try
                {
                    ulong[] itemGuids = ItemGuids;
                    uint usedSlotCount = 0U;
                    for (int i = 0; i < itemGuids.Length; i++)
                    {
                        if (itemGuids[i] != 0UL)
                        {
                            usedSlotCount += 1U;
                        }
                    }
                    return usedSlotCount;
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets the item GUID at a specific slot
        /// </summary>
        public ulong GetItemGuidBySlot(uint slot)
        {
            try
            {
                if (slot >= Slots)
                {
                    throw new ArgumentOutOfRangeException("slot");
                }

                Memory wow = ObjectManager.Wow;
                uint[] array = new uint[]
                {
                    _bagStructure.ItemsBaseAddress + (FirstSlotIndex + slot) * 8U
                };
                return wow.Read<ulong>(array);
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
                return 0UL;
            }
        }

        /// <summary>
        /// Gets the item at a specific slot
        /// </summary>
        public WoWItem GetItemBySlot(uint slot)
        {
            try
            {
                if (slot >= Slots)
                {
                    throw new ArgumentOutOfRangeException("slot");
                }

                Memory wow = ObjectManager.Wow;
                uint[] array = new uint[]
                {
                    _bagStructure.ItemsBaseAddress + (FirstSlotIndex + slot) * 8U
                };
                return ObjectManager.GetObjectByGuid<WoWItem>(wow.Read<ulong>(array));
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
                return null;
            }
        }

        /// <summary>
        /// Checks if the bag contains an item
        /// </summary>
        public bool Contains(WoWItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            return Contains(item.Guid);
        }

        /// <summary>
        /// Checks if the bag contains an item with the specified GUID
        /// </summary>
        public bool Contains(ulong itemGuid)
        {
            try
            {
                return IndexOf(itemGuid) != -1;
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
                return false;
            }
        }

        /// <summary>
        /// Gets the index of an item in the bag
        /// </summary>
        public int IndexOf(WoWItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            return IndexOf(item.Guid);
        }

        /// <summary>
        /// Gets the index of an item GUID in the bag
        /// </summary>
        public int IndexOf(ulong guid)
        {
            try
            {
                ulong[] itemGuids = ItemGuids;
                for (int i = 0; i < itemGuids.Length; i++)
                {
                    if (itemGuids[i] == guid)
                    {
                        return i;
                    }
                }
                return -1;
            }
            catch (Exception ex)
            {
                Logging.WriteException(ex);
                return -1;
            }
        }
    }
}
