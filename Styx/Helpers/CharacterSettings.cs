using Styx.WoWInternals.WoWObjects;
using System;
using System.ComponentModel;
using System.IO;

#nullable disable
namespace Styx.Helpers
{
    /// <summary>
    /// Character-specific settings.
    /// Ported from HB 3.3.5a CharacterSettings.cs.
    /// Settings stored in: Settings/CharacterSettings_{Name}.xml
    /// Instance is created via Initialize() after game attachment.
    /// </summary>
    public class CharacterSettings : Settings, INotifyPropertyChanged
    {
        /// <summary>
        /// Singleton instance. Set via Initialize() after game attachment.
        /// </summary>
        public static CharacterSettings Instance { get; private set; }

        private int _foodAmount;
        private int _drinkAmount;
        private string _drinkName;
        private bool _findMountAutomatically;
        private bool _findVendorsAutomatically;
        private string _foodName;
        private bool _harvestHerbs;
        private bool _harvestMinerals;
        private bool _learnFlightPaths;
        private bool _lootChests;
        private bool _lootMobs;
        private bool _ninjaSkin;
        private int _lootRadius;
        private string _mailRecipient;
        private int _mountDistance;
        private string _mountName;
        private string _flyingMountName;
        private bool _ressAtSpiritHealers;
        private int _pullDistance;
        private int _selectedBotIndex;
        private bool _skinMobs;

        // backing fields for frame/look and tick settings
        private bool _useFreeLook;
        private int _freeLook;
        private byte _ticksPerSecond;
        private bool _trainNewSkills;
        private bool _useFlightPaths;
        private bool _useMount;
        private bool _useRandomMount;
        private string _lastUsedPath;
        private PropertyChangedEventHandler _propertyChangedHandler;

        /// <summary>
        /// Constructor. Path: Settings/CharacterSettings_{Name}.xml
        /// Exact pattern from HB 3.3.5a.
        /// </summary>
        public CharacterSettings()
            : base(Path.Combine(Logging.ApplicationPath,
                   string.Format("Settings\\CharacterSettings_{0}.xml",
                   (StyxWoW.Me != null) ? StyxWoW.Me.Name : "")))
        {
        }

        /// <summary>
        /// Creates the singleton instance. Must be called after game attachment
        /// when StyxWoW.Me is available.
        /// </summary>
        public static void Initialize()
        {
            Instance = new CharacterSettings();
        }

        [DefaultValue(0)]
        [Setting]
        public int FoodAmount
        {
            get => _foodAmount;
            set
            {
                _foodAmount = value;
                OnPropertyChanged(nameof(FoodAmount));
            }
        }

        [Setting]
        [DefaultValue(0)]
        public int DrinkAmount
        {
            get => _drinkAmount;
            set
            {
                _drinkAmount = value;
                OnPropertyChanged(nameof(DrinkAmount));
            }
        }

        [Setting]
        public string[] EnabledPlugins { get; set; }

        [DefaultValue(2)]
        [Setting(Explanation = "The last selected index used by the Combobox in the form.")]
        public int SelectedBotIndex
        {
            get => _selectedBotIndex;
            set
            {
                _selectedBotIndex = value;
                OnPropertyChanged(nameof(SelectedBotIndex));
            }
        }

        [Setting(Explanation = "Allow the bot to use flight paths. (Note: this will also allow the bot to interact with new flightmasters to learn new connections.")]
        [DefaultValue(false)]
        public bool UseFlightPaths
        {
            get => _useFlightPaths;
            set
            {
                _useFlightPaths = value;
                OnPropertyChanged(nameof(UseFlightPaths));
            }
        }

        [DefaultValue(true)]
        [Setting]
        public bool FindMountAutomatically
        {
            get => _findMountAutomatically;
            set
            {
                _findMountAutomatically = value;
                OnPropertyChanged(nameof(FindMountAutomatically));
            }
        }

        [DefaultValue(true)]
        [Setting]
        public bool UseRandomMount
        {
            get => _useRandomMount;
            set
            {
                _useRandomMount = value;
                OnPropertyChanged(nameof(UseRandomMount));
            }
        }

        [Setting]
        [DefaultValue("")]
        public string MailRecipient
        {
            get => _mailRecipient;
            set
            {
                _mailRecipient = value;
                OnPropertyChanged(nameof(MailRecipient));
            }
        }

        [Setting]
        [DefaultValue("")]
        public string FoodName
        {
            get => _foodName;
            set
            {
                _foodName = value;
                OnPropertyChanged(nameof(FoodName));
            }
        }

        [Setting]
        [DefaultValue("")]
        public string DrinkName
        {
            get => _drinkName;
            set
            {
                _drinkName = value;
                OnPropertyChanged(nameof(DrinkName));
            }
        }

        [Setting]
        [DefaultValue("")]
        public string MountName
        {
            get => _mountName;
            set
            {
                _mountName = value;
                OnPropertyChanged(nameof(MountName));
            }
        }

        [DefaultValue("")]
        [Setting]
        public string FlyingMountName
        {
            get => _flyingMountName;
            set
            {
                _flyingMountName = value;
                OnPropertyChanged(nameof(FlyingMountName));
            }
        }

        [Setting]
        [DefaultValue(true)]
        public bool LootMobs
        {
            get => _lootMobs;
            set
            {
                _lootMobs = value;
                OnPropertyChanged(nameof(LootMobs));
            }
        }

        [Setting]
        [DefaultValue(false)]
        public bool SkinMobs
        {
            get => _skinMobs;
            set
            {
                _skinMobs = value;
                OnPropertyChanged(nameof(SkinMobs));
            }
        }

        [DefaultValue(false)]
        [Setting]
        public bool NinjaSkin
        {
            get => _ninjaSkin;
            set
            {
                _ninjaSkin = value;
                OnPropertyChanged(nameof(NinjaSkin));
            }
        }

        [Setting]
        [DefaultValue(true)]
        public bool LootChests
        {
            get => _lootChests;
            set
            {
                _lootChests = value;
                OnPropertyChanged(nameof(LootChests));
            }
        }

        [DefaultValue(false)]
        [Setting]
        public bool HarvestMinerals
        {
            get => _harvestMinerals;
            set
            {
                _harvestMinerals = value;
                OnPropertyChanged(nameof(HarvestMinerals));
            }
        }

        [DefaultValue(false)]
        [Setting]
        public bool HarvestHerbs
        {
            get => _harvestHerbs;
            set
            {
                _harvestHerbs = value;
                OnPropertyChanged(nameof(HarvestHerbs));
            }
        }

        [DefaultValue(true)]
        [Setting]
        public bool UseMount
        {
            get => _useMount;
            set
            {
                _useMount = value;
                OnPropertyChanged(nameof(UseMount));
            }
        }

        [Setting]
        [DefaultValue(45)]
        public int PullDistance
        {
            get => _pullDistance;
            set
            {
                _pullDistance = value;
                OnPropertyChanged(nameof(PullDistance));
            }
        }

        // frame look support (UI removed per user request, but preserve settings)
        [Setting]
        [DefaultValue(false)]
        public bool UseFreeLook
        {
            get => _useFreeLook;
            set
            {
                _useFreeLook = value;
                OnPropertyChanged(nameof(UseFreeLook));
            }
        }

        [Setting]
        [DefaultValue(0)]
        public int FreeLook
        {
            get => _freeLook;
            set
            {
                _freeLook = value;
                OnPropertyChanged(nameof(FreeLook));
            }
        }

        // ticks-per-second slider value
        [Setting]
        [DefaultValue((byte)13)]
        public byte TicksPerSecond
        {
            get => _ticksPerSecond;
            set
            {
                _ticksPerSecond = value;
                OnPropertyChanged(nameof(TicksPerSecond));
            }
        }

        [Setting]
        [DefaultValue(45)]
        public int LootRadius
        {
            get => _lootRadius;
            set
            {
                _lootRadius = value;
                OnPropertyChanged(nameof(LootRadius));
            }
        }

        [Setting]
        [DefaultValue(false)]
        public bool FindVendorsAutomatically
        {
            get => _findVendorsAutomatically;
            set
            {
                _findVendorsAutomatically = value;
                OnPropertyChanged(nameof(FindVendorsAutomatically));
            }
        }

        [Setting]
        [DefaultValue(false)]
        public bool TrainNewSkills
        {
            get => _trainNewSkills;
            set
            {
                _trainNewSkills = value;
                OnPropertyChanged(nameof(TrainNewSkills));
            }
        }

        [Setting]
        [DefaultValue(true)]
        public bool LearnFlightPaths
        {
            get => _learnFlightPaths;
            set
            {
                _learnFlightPaths = value;
                OnPropertyChanged(nameof(LearnFlightPaths));
            }
        }

        [Setting]
        [DefaultValue("")]
        public string LastUsedPath
        {
            get => _lastUsedPath;
            set
            {
                _lastUsedPath = value;
                OnPropertyChanged(nameof(LastUsedPath));
            }
        }

        [DefaultValue(75)]
        [Setting]
        public int MountDistance
        {
            get => _mountDistance;
            set
            {
                _mountDistance = value;
                OnPropertyChanged(nameof(MountDistance));
            }
        }

        [Setting]
        [DefaultValue(false)]
        public bool RessAtSpiritHealers
        {
            get => _ressAtSpiritHealers;
            set
            {
                _ressAtSpiritHealers = value;
                OnPropertyChanged(nameof(RessAtSpiritHealers));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => _propertyChangedHandler += value;
            remove => _propertyChangedHandler -= value;
        }

        private void OnPropertyChanged(string propertyName)
        {
            _propertyChangedHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
