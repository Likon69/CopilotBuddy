using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Bots.Gather
{
    /// <summary>
    /// Represents a single herb/mineral checkbox item in the Node Selection tab.
    /// </summary>
    public class NodeSelectionItem : INotifyPropertyChanged
    {
        private bool _isChecked;

        public uint Entry { get; }
        public string Name { get; }
        public uint RequiredSkill { get; }
        public string DisplayName => $"{Name}  (Skill {RequiredSkill})";

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
                }
            }
        }

        public NodeSelectionItem(GatherableNode node, bool isChecked)
        {
            Entry = node.Entry;
            Name = node.Name;
            RequiredSkill = node.RequiredSkill;
            _isChecked = isChecked;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    /// <summary>
    /// Settings window for GatherBuddy — WPF dark theme matching CB style.
    /// Bound to GatherBuddySettings.Instance via a ViewModel wrapper.
    /// Tab 1: General settings. Tab 2: Node Selection (herb/mineral checkboxes).
    /// </summary>
    public partial class GatherBuddySettingsWindow : Window
    {
        private GatherBuddySettingsViewModel ViewModel => (GatherBuddySettingsViewModel)DataContext;

        public GatherBuddySettingsWindow()
        {
            InitializeComponent();
            DataContext = new GatherBuddySettingsViewModel();
        }

        // ═══ SAVE / CANCEL ═══

        private void btnSaveAndClose_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveNodeSelection();
            GatherBuddySettings.Instance.Save();
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            GatherBuddySettings.Instance.Load();
            DialogResult = false;
            Close();
        }

        // ═══ SELECT ALL / UNSELECT ALL ═══

        private void btnSelectAllHerbs_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in ViewModel.HerbItems) item.IsChecked = true;
        }

        private void btnUnselectAllHerbs_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in ViewModel.HerbItems) item.IsChecked = false;
        }

        private void btnSelectAllMinerals_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in ViewModel.MineralItems) item.IsChecked = true;
        }

        private void btnUnselectAllMinerals_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in ViewModel.MineralItems) item.IsChecked = false;
        }
    }

    /// <summary>
    /// ViewModel wrapping GatherBuddySettings for WPF binding.
    /// Slider values need double binding (WPF Slider uses double),
    /// while Settings uses int — this bridges the gap.
    /// </summary>
    public class GatherBuddySettingsViewModel : INotifyPropertyChanged
    {
        private readonly GatherBuddySettings _s = GatherBuddySettings.Instance;

        public event PropertyChangedEventHandler? PropertyChanged;

        // ═══ NODE SELECTION (bound from XAML) ═══
        public ObservableCollection<NodeSelectionItem> HerbItems { get; } = new();
        public ObservableCollection<NodeSelectionItem> MineralItems { get; } = new();

        public GatherBuddySettingsViewModel()
        {
            LoadNodeSelectionLists();
        }

        private void LoadNodeSelectionLists()
        {
            var blacklist = _s.BlacklistedEntries;

            foreach (var herb in GatherableNodes.Herbs)
                HerbItems.Add(new NodeSelectionItem(herb, !blacklist.Contains(herb.Entry)));

            foreach (var mineral in GatherableNodes.Minerals)
                MineralItems.Add(new NodeSelectionItem(mineral, !blacklist.Contains(mineral.Entry)));
        }

        /// <summary>
        /// Collect unchecked entries from both lists and save to settings.
        /// </summary>
        public void SaveNodeSelection()
        {
            var blacklist = new HashSet<uint>();

            foreach (var item in HerbItems)
            {
                if (!item.IsChecked)
                    blacklist.Add(item.Entry);
            }

            foreach (var item in MineralItems)
            {
                if (!item.IsChecked)
                    blacklist.Add(item.Entry);
            }

            _s.SetBlacklistedEntries(blacklist);
        }

        // ═══ GATHERING ═══
        public bool GatherHerbs
        {
            get => _s.GatherHerbs;
            set { _s.GatherHerbs = value; OnPropertyChanged(nameof(GatherHerbs)); }
        }

        public bool GatherMinerals
        {
            get => _s.GatherMinerals;
            set { _s.GatherMinerals = value; OnPropertyChanged(nameof(GatherMinerals)); }
        }

        public bool GatherChests
        {
            get => _s.GatherChests;
            set { _s.GatherChests = value; OnPropertyChanged(nameof(GatherChests)); }
        }

        public bool FaceNodes
        {
            get => _s.FaceNodes;
            set { _s.FaceNodes = value; OnPropertyChanged(nameof(FaceNodes)); }
        }

        // ═══ NAVIGATION ═══
        public int PathingTypeIndex
        {
            get => (int)_s.PathingType;
            set { _s.PathingType = (PathType)value; OnPropertyChanged(nameof(PathingTypeIndex)); }
        }

        public double NodeDetectionRange
        {
            get => _s.NodeDetectionRange;
            set { _s.NodeDetectionRange = (float)value; OnPropertyChanged(nameof(NodeDetectionRange)); }
        }

        public double HeightModifier
        {
            get => _s.HeightModifier;
            set { _s.HeightModifier = (float)value; OnPropertyChanged(nameof(HeightModifier)); }
        }

        public bool RandomizeHotspots
        {
            get => _s.RandomizeHotspots;
            set { _s.RandomizeHotspots = value; OnPropertyChanged(nameof(RandomizeHotspots)); }
        }

        // ═══ FLYING ═══
        public bool UseFlying
        {
            get => _s.UseFlying;
            set { _s.UseFlying = value; OnPropertyChanged(nameof(UseFlying)); }
        }

        public double FlyingAltitude
        {
            get => _s.FlyingAltitude;
            set { _s.FlyingAltitude = (float)value; OnPropertyChanged(nameof(FlyingAltitude)); }
        }

        public double FlyingDescentRange
        {
            get => _s.FlyingDescentRange;
            set { _s.FlyingDescentRange = (float)value; OnPropertyChanged(nameof(FlyingDescentRange)); }
        }

        // ═══ COMBAT / LOOT ═══
        public bool LootMobs
        {
            get => _s.LootMobs;
            set { _s.LootMobs = value; OnPropertyChanged(nameof(LootMobs)); }
        }

        public bool IgnoreElites
        {
            get => _s.IgnoreElites;
            set { _s.IgnoreElites = value; OnPropertyChanged(nameof(IgnoreElites)); }
        }

        public bool SkinMobs
        {
            get => _s.SkinMobs;
            set { _s.SkinMobs = value; OnPropertyChanged(nameof(SkinMobs)); }
        }

        public double LootRadius
        {
            get => _s.LootRadius;
            set { _s.LootRadius = (float)value; OnPropertyChanged(nameof(LootRadius)); }
        }

        // ═══ ANTI-NINJA ═══
        public bool NoNinja
        {
            get => _s.NoNinja;
            set { _s.NoNinja = value; OnPropertyChanged(nameof(NoNinja)); }
        }

        public double BlacklistTimerValue
        {
            get => _s.BlacklistTimer;
            set { _s.BlacklistTimer = (int)value; OnPropertyChanged(nameof(BlacklistTimerValue)); }
        }

        // ═══ VENDOR/REPAIR ═══
        public bool VendorWhenFull
        {
            get => _s.VendorWhenFull;
            set { _s.VendorWhenFull = value; OnPropertyChanged(nameof(VendorWhenFull)); }
        }

        public double MinFreeBagSlotsValue
        {
            get => _s.MinFreeBagSlots;
            set { _s.MinFreeBagSlots = (int)value; OnPropertyChanged(nameof(MinFreeBagSlotsValue)); }
        }

        public bool FindVendorsAutomatically
        {
            get => _s.FindVendorsAutomatically;
            set { _s.FindVendorsAutomatically = value; OnPropertyChanged(nameof(FindVendorsAutomatically)); }
        }

        public bool RepairAtVendor
        {
            get => _s.RepairAtVendor;
            set { _s.RepairAtVendor = value; OnPropertyChanged(nameof(RepairAtVendor)); }
        }

        public double RepairDurabilityPercentValue
        {
            get => _s.RepairDurabilityPercent;
            set { _s.RepairDurabilityPercent = (int)value; OnPropertyChanged(nameof(RepairDurabilityPercentValue)); }
        }

        // ═══ SELL QUALITY ═══
        public bool SellGrey
        {
            get => _s.SellGrey;
            set { _s.SellGrey = value; OnPropertyChanged(nameof(SellGrey)); }
        }

        public bool SellWhite
        {
            get => _s.SellWhite;
            set { _s.SellWhite = value; OnPropertyChanged(nameof(SellWhite)); }
        }

        public bool SellGreen
        {
            get => _s.SellGreen;
            set { _s.SellGreen = value; OnPropertyChanged(nameof(SellGreen)); }
        }

        public bool SellBlue
        {
            get => _s.SellBlue;
            set { _s.SellBlue = value; OnPropertyChanged(nameof(SellBlue)); }
        }

        public bool SellPurple
        {
            get => _s.SellPurple;
            set { _s.SellPurple = value; OnPropertyChanged(nameof(SellPurple)); }
        }

        // ═══ MAIL ═══
        public bool MailToAlt
        {
            get => _s.MailToAlt;
            set { _s.MailToAlt = value; OnPropertyChanged(nameof(MailToAlt)); }
        }

        public string MailRecipient
        {
            get => _s.MailRecipient;
            set { _s.MailRecipient = value; OnPropertyChanged(nameof(MailRecipient)); }
        }

        public bool MailGrey
        {
            get => _s.MailGrey;
            set { _s.MailGrey = value; OnPropertyChanged(nameof(MailGrey)); }
        }

        public bool MailWhite
        {
            get => _s.MailWhite;
            set { _s.MailWhite = value; OnPropertyChanged(nameof(MailWhite)); }
        }

        public bool MailGreen
        {
            get => _s.MailGreen;
            set { _s.MailGreen = value; OnPropertyChanged(nameof(MailGreen)); }
        }

        public bool MailBlue
        {
            get => _s.MailBlue;
            set { _s.MailBlue = value; OnPropertyChanged(nameof(MailBlue)); }
        }

        public bool MailPurple
        {
            get => _s.MailPurple;
            set { _s.MailPurple = value; OnPropertyChanged(nameof(MailPurple)); }
        }

        // ═══ DEATH / SAFETY ═══
        public bool UseSpiritHealer
        {
            get => _s.UseSpiritHealer;
            set { _s.UseSpiritHealer = value; OnPropertyChanged(nameof(UseSpiritHealer)); }
        }

        public bool WaitRezSickness
        {
            get => _s.WaitRezSickness;
            set { _s.WaitRezSickness = value; OnPropertyChanged(nameof(WaitRezSickness)); }
        }

        // ═══ SESSION TIMER ═══
        public double BottingHours
        {
            get => _s.BottingHours;
            set { _s.BottingHours = (float)value; OnPropertyChanged(nameof(BottingHours)); }
        }

        public bool HearthAndExit
        {
            get => _s.HearthAndExit;
            set { _s.HearthAndExit = value; OnPropertyChanged(nameof(HearthAndExit)); }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
