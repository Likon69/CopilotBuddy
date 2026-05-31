using System;
using System.Threading;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Styx.CommonBot
{
    /// <summary>
    /// Tracks game statistics like kills, deaths, XP/hour, etc.
    /// Provides data for the info panel in the UI.
    /// </summary>
    public static class GameStats
    {
        #region Private Fields

        private static InfoPanelUpdatedDelegate? _onInfoPanelUpdated;
        private static DateTime _startTime;
        private static uint _startXp;
        private static uint _startNextLevelXp;
        private static uint _startHonor;
        private static long _levelXpGained;
        private static long _tickCount;
        private static readonly TimedRecordKeeper<long> _ticksRecordKeeper = new(1000, 5000);

        #endregion

        #region Events

        public static event InfoPanelUpdatedDelegate OnInfoPanelUpdated
        {
            add
            {
                InfoPanelUpdatedDelegate? current = _onInfoPanelUpdated;
                InfoPanelUpdatedDelegate? updated;
                do
                {
                    updated = current;
                    InfoPanelUpdatedDelegate? combined = (InfoPanelUpdatedDelegate?)Delegate.Combine(updated, value);
                    current = Interlocked.CompareExchange(ref _onInfoPanelUpdated, combined, updated);
                }
                while (current != updated);
            }
            remove
            {
                InfoPanelUpdatedDelegate? current = _onInfoPanelUpdated;
                InfoPanelUpdatedDelegate? updated;
                do
                {
                    updated = current;
                    InfoPanelUpdatedDelegate? removed = (InfoPanelUpdatedDelegate?)Delegate.Remove(updated, value);
                    current = Interlocked.CompareExchange(ref _onInfoPanelUpdated, removed, updated);
                }
                while (current != updated);
            }
        }

        public delegate void InfoPanelUpdatedDelegate();

        #endregion

        #region Constructor

        static GameStats()
        {
            // Use OnBotStart — TreeRoot calls RaiseBotStart(), not RaiseBotStarted().
            BotEvents.OnBotStart += OnBotStarted;
            BotEvents.OnPulse += OnPulse;
            BotEvents.Player.OnMobKilled += OnMobKilled;
            BotEvents.Player.OnMobLooted += OnMobLooted;
            BotEvents.Player.OnLevelUp += OnLevelUp;
            ToastNotifier.EnsureLoaded();
        }

        #endregion

        #region Public Properties - Counts

        public static uint MobsKilled { get; private set; }

        public static uint Loots { get; private set; }

        public static uint Deaths { get; private set; }

        public static uint BGsWon { get; private set; }

        public static uint BGsLost { get; private set; }

        public static uint HonorGained { get; private set; }

        #endregion

        #region Public Properties - Rates

        public static float XPPerHour { get; private set; }

        public static float MobsPerHour { get; private set; }

        public static float LootsPerHour { get; private set; }

        public static float DeathsPerHour { get; private set; }

        public static float BGsWonPerHour { get; private set; }

        public static float BGsLostPerHour { get; private set; }

        public static float HonorPerHour { get; private set; }

        public static TimeSpan TimeToLevel { get; private set; }

        public static float TicksPerSecond { get; private set; }

        #endregion

        #region Public Properties - Computed

        public static uint BGsCompleted => BGsWon + BGsLost;

        public static float BGsPerHour => BGsWonPerHour + BGsLostPerHour;

        public static bool IsMeasuring { get; private set; }

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets the current honor points from the player.
        /// Uses LocalPlayer.Honor which reads from PLAYER_FIELD_HONOR_CURRENCY descriptor.
        /// </summary>
        private static uint HonorPoints
        {
            get
            {
                var me = ObjectManager.Me;
                if (me == null) return 0U;
                return me.Honor;
            }
        }

        #endregion

        #region Public Methods

        public static void Reset()
        {
            IsMeasuring = false;
            
            var me = ObjectManager.Me;
            if (me != null)
            {
                _startXp = me.Experience;
                _startNextLevelXp = me.NextLevelExperience;
            }
            else
            {
                _startXp = 0U;
                _startNextLevelXp = 0U;
            }
            
            _startHonor = HonorPoints;
            _levelXpGained = 0L;
            _tickCount = 0L;
            
            HonorGained = 0U;
            MobsKilled = 0U;
            Loots = 0U;
            Deaths = 0U;
            BGsWon = 0U;
            BGsLost = 0U;
            
            HonorPerHour = 0f;
            XPPerHour = 0f;
            MobsPerHour = 0f;
            LootsPerHour = 0f;
            DeathsPerHour = 0f;
            BGsWonPerHour = 0f;
            BGsLostPerHour = 0f;
        }

        public static void StartMeasuring()
        {
            _startTime = DateTime.Now;
            IsMeasuring = true;
        }

        public static void StopMeasuring()
        {
            IsMeasuring = false;
        }

        public static void Died()
        {
            Deaths += 1U;
        }

        public static void KilledMob()
        {
            MobsKilled += 1U;
        }

        public static void LootedMob()
        {
            Loots += 1U;
        }

        public static void BGWon()
        {
            BGsWon += 1U;
        }

        public static void BGLost()
        {
            BGsLost += 1U;
        }

        #endregion

        #region Internal Methods

        internal static void UpdateStats()
        {
            if (!IsMeasuring) return;
            
            var me = ObjectManager.Me;
            if (me == null) return;

            // Track honor changes
            uint currentHonor = HonorPoints;
            if (currentHonor != _startHonor)
            {
                if (currentHonor > _startHonor)
                {
                    HonorGained += currentHonor - _startHonor;
                }
                _startHonor = currentHonor;
            }

            // Calculate elapsed time
            TimeSpan elapsed = GetElapsedTime();
            
            // Calculate XP/hour
            long totalXpGained = me.Experience + _levelXpGained - _startXp;
            XPPerHour = CalculatePerHour((uint)totalXpGained, elapsed);
            
            // Calculate rates
            MobsPerHour = CalculatePerHour(MobsKilled, elapsed);
            LootsPerHour = CalculatePerHour(Loots, elapsed);
            DeathsPerHour = CalculatePerHour(Deaths, elapsed);
            BGsWonPerHour = CalculatePerHour(BGsWon, elapsed);
            BGsLostPerHour = CalculatePerHour(BGsLost, elapsed);
            HonorPerHour = CalculatePerHour(HonorGained, elapsed);
            
            // Calculate time to level
            if (XPPerHour >= 1f)
            {
                uint xpToLevel = me.NextLevelExperience - me.Experience;
                TimeToLevel = TimeSpan.FromHours(xpToLevel / XPPerHour);
            }
            else
            {
                TimeToLevel = TimeSpan.Zero;
            }
            
            // Calculate ticks per second
            _tickCount += 1L;
            TicksPerSecond = (float)(CalculateTicksPerHour(_tickCount, _ticksRecordKeeper) / 3600.0);
            
            // Fire update event
            _onInfoPanelUpdated?.Invoke();
        }

        #endregion

        #region Private Methods

        private static TimeSpan GetElapsedTime()
        {
            return DateTime.Now.Subtract(_startTime);
        }

        private static float CalculatePerHour(uint count, TimeSpan elapsed)
        {
            if (elapsed.TotalHours <= 0.0)
            {
                return 0f;
            }
            return count / (float)elapsed.TotalHours;
        }

        private static double CalculateTicksPerHour(long ticks, TimedRecordKeeper<long> keeper)
        {
            keeper.AddRecord(ticks);
            
            if (!keeper.GetRecord(TimeSpan.FromMilliseconds(keeper.StoreTimeMilliseconds), out long oldTicks, out TimeSpan span))
            {
                return 0.0;
            }
            
            return span.TotalHours > 0.0 ? (ticks - oldTicks) / span.TotalHours : 0.0;
        }

        #endregion

        #region Event Handlers

        private static void OnBotStarted(EventArgs args)
        {
            Reset();
            StartMeasuring();
        }

        private static void OnPulse(object? sender, EventArgs args)
        {
            UpdateStats();
        }

        private static void OnLevelUp(BotEvents.Player.LevelUpEventArgs args)
        {
            // Add XP from previous level that we didn't count
            _levelXpGained += _startNextLevelXp - _startXp;
            
            var me = ObjectManager.Me;
            if (me != null)
            {
                _startNextLevelXp = me.CurrentXP + me.XPToNextLevel;
            }
            _startXp = 0U;
        }

        private static void OnMobKilled(BotEvents.Player.MobKilledEventArgs args)
        {
            KilledMob();
        }

        private static void OnMobLooted(BotEvents.Player.MobLootedEventArgs args)
        {
            LootedMob();
        }

        #endregion
    }

    /// <summary>
    /// Keeps track of values over time for rate calculations.
    /// </summary>
    public class TimedRecordKeeper<T>
    {
        private readonly int _intervalMs;
        private readonly int _storeTimeMs;
        private readonly object _lock = new();
        private DateTime _lastRecordTime;
        private T? _lastValue;
        private T? _oldestValue;
        private DateTime _oldestTime;

        public TimedRecordKeeper(int intervalMilliseconds, int storeTimeMilliseconds)
        {
            _intervalMs = intervalMilliseconds;
            _storeTimeMs = storeTimeMilliseconds;
        }

        public int StoreTimeMilliseconds => _storeTimeMs;

        public void AddRecord(T value)
        {
            lock (_lock)
            {
                DateTime now = DateTime.Now;
                
                if (_lastRecordTime == default || (now - _lastRecordTime).TotalMilliseconds >= _intervalMs)
                {
                    // Shift old value
                    if (_lastRecordTime != default && (now - _oldestTime).TotalMilliseconds >= _storeTimeMs)
                    {
                        _oldestValue = _lastValue;
                        _oldestTime = _lastRecordTime;
                    }
                    else if (_oldestTime == default)
                    {
                        _oldestValue = value;
                        _oldestTime = now;
                    }
                    
                    _lastValue = value;
                    _lastRecordTime = now;
                }
            }
        }

        public bool GetRecord(TimeSpan maxAge, out T oldValue, out TimeSpan elapsed)
        {
            lock (_lock)
            {
                oldValue = default!;
                elapsed = TimeSpan.Zero;
                
                if (_oldestTime == default)
                    return false;
                
                DateTime now = DateTime.Now;
                elapsed = now - _oldestTime;
                
                if (elapsed > maxAge)
                    return false;
                
                oldValue = _oldestValue!;
                return true;
            }
        }
    }
}
