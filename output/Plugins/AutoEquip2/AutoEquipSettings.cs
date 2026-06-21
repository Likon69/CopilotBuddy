using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Styx.Helpers;
using Styx.Logic.Inventory;
using DefaultValue = Styx.Helpers.DefaultValueAttribute;

namespace Styx.Bot.Plugins.AutoEquip2
{
    public class AutoEquipSettings : Settings
    {
        private static AutoEquipSettings _instance;

        public AutoEquipSettings()
            : base(Path.Combine(Path.Combine(Logging.ApplicationPath, "Settings"), string.Format("AutoEquipSettings_{0}.xml", StyxWoW.Me.Name)))
        {
            if (IgnoreInvTypes == null || IgnoreInvTypes.Length == 0)
            {
                IgnoreInvTypes = new[] { InventoryType.Trinket };
            }
            if (ProtectedSlots == null || ProtectedSlots.Length == 0)
            {
                ProtectedSlots = new[] { InventorySlot.Trinket0Slot, InventorySlot.Trinket1Slot };
            }
        }

        public static AutoEquipSettings Instance { get { return _instance ?? (_instance = new AutoEquipSettings()); } }

        #region Category: Item Types

        [Setting]
        [DefaultValue(false)]
        [Category("Item Types")]
        [DisplayName("Equip Purples")]
        [Description("Toggles if AutoEquip should equip purple items.")]
        public bool EquipPurples { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Item Types")]
        [DisplayName("Equip Blues")]
        [Description("Toggles if AutoEquip should equip blue items.")]
        public bool EquipBlues { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("Item Types")]
        [DisplayName("Equip Greens")]
        [Description("Toggles if AutoEquip should equip green items.")]
        public bool EquipGreens { get; set; }

        [Setting] 
        [DefaultValue(true)]
        [Category("Item Types")]
        [DisplayName("Equip Whites")]
        [Description("Toggles if AutoEquip should equip white items.")]
        public bool EquipWhites { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Item Types")]
        [DisplayName("Equip Grays")]
        [Description("Toggles if AutoEquip should equip grey items.")]
        public bool EquipGrays { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Item Types")]
        [DisplayName("Equip Heirlooms")]
        [Description("Toggles if AutoEquip should equip heirloom items.")]
        public bool EquipHeirlooms { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("BoE Item Types")]
        [DisplayName("Equip BoE Greens")]
        [Description("Toggles if AutoEquip should equip green 'Bind on Equip' items.")]
        public bool EquipBoEGreens { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("BoE Item Types")]
        [DisplayName("Equip BoE Blues")]
        [Description("Toggles if AutoEquip should equip blue 'Bind on Equip' items.")]
        public bool EquipBoEBlues { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("BoE Item Types")]
        [DisplayName("Equip BoE Epics")]
        [Description("Toggles if AutoEquip should equip purple 'Bind on Equip' items.")]
        public bool EquipBoEPurples { get; set; }

        #endregion

        #region Category: Protected Slots and Types 

        [Setting]
        [Category("Protected Slots and Types")]
        [DisplayName("Ignored Inventory Types")]
        [Description("Items with this inventory type will be ignored when looking for new items to equip.")]
        public InventoryType[] IgnoreInvTypes { get; set; }

        [Setting]
        [Category("Protected Slots and Types")]
        [DisplayName("Protected Slots")]
        [Description("InventorySlots of this type will be protected, AutoEquip will never equip new items into theese slots.")]
        public InventorySlot[] ProtectedSlots { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Protected Slots and Types")]
        [DisplayName("Replace Heirlooms")]
        [Description("Toggles if AutoEquip should replace heirlooms.")]
        public bool ReplaceHeirlooms { get; set; }

        #endregion

        #region Category: General

        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Equip Items")]
        [Description("Toggles if AutoEquip should equip items.")]
        public bool AutoEquipItems { get; set; }

        [Setting]
        [DefaultValue(true)]
        [Category("General")]
        [DisplayName("Equip Bags")]
        [Description("Toggles if AutoEquip should equip bags.")]
        public bool AutoEquipBags { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("General")]
        [DisplayName("Include Enchants")]
        [Description("Toggles if AutoEquip should include the stats of enchants on items when scoring items.")]
        public bool IncludeEnchants { get; set; }

        [Setting]
        [DefaultValue(WeaponStyle.None)]
        [Category("General")]
        [DisplayName("Weapon Style")]
        [Description("The weapon style AutoEquip will use when equipping new items, Dual-wield, One hand and a shield or 2h weapon.")]
        public WeaponStyle WeaponStyle { get; set; }

        [Setting]
        [DefaultValue(WeaponType.All)]
        [Category("General")]
        [DisplayName("Weapon Type")]
        [Description("The weapon type AutoEquip will use when equipping new items.")]
        [Editor(typeof(FlagEnumUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public WeaponType WeaponType { get; set; }

        #endregion

        #region Category: Loot Rolling

        [Setting]
        [DefaultValue(true)]
        [Category("Loot Rolling")]
        [DisplayName("Roll on Loot")]
        [Description("Toggles if AutoEquip will roll for loot when doing dungeons, raiding or any other form of activity that requires you to be in a party with other players.")]
        public bool RollForLootInDungeons { get; set; }

        [Setting]
        [DefaultValue(false)]
        [Category("Loot Rolling")]
        [DisplayName("Roll for Disenchant if possible")]
        [Description("Toggles if AutoEquip will select disenchant if possible when rolling for loot.")]
        public bool RollForLootDE { get; set; }

        #endregion
    }

    public enum WeaponStyle
    {
        None,
        OneHanderAndShield,
        DualWield,
        TwoHander
    }

    [Flags]
    public enum WeaponType
    {
        None = 0x0,
        Axe = 0x1,
        Dagger = 0x2,
        Fist = 0x4,
        Mace = 0x8,
        Polearm = 0x10,
        Spear = 0x20,
        Staff = 0x40,
        Sword = 0x80,
        All = Axe | Mace | Sword | Dagger | Polearm | Spear | Staff | Fist

    }
}
