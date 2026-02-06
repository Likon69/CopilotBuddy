using System;
using System.Collections.Generic;
using System.Threading;
using Styx.Helpers;
using Styx.Logic.BehaviorTree;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace Styx
{
	public static class BotEvents
	{
		private static List<Action> _eventCheckers;
		private static OnBotStartDelegate _onBotStart;
		private static OnBotStartDelegate _onBotStarted;
		private static OnBotStopDelegate _onBotStop;
		private static OnBotStopDelegate _onBotStopped;
		private static EventHandler _onPulse;
		private static OnBotStartDelegate _onBotStartComplete;
		private static OnBotStopDelegate _onBotStopping;
		private static OnBotChangedDelegate _onBotChanged;

		static BotEvents()
		{
			_eventCheckers = new List<Action>();
			// HB 4.3.4 pattern: register event checkers that run each pulse
			_eventCheckers.Add(Player.CheckLevelChange);
			_eventCheckers.Add(Player.CheckMapChange);
		}

		/// <summary>
		/// Fires the OnBotChanged event.
		/// </summary>
		internal static void RaiseBotChanged(BotChangedEventArgs args)
		{
			_onBotChanged?.Invoke(args);
		}

		public static void PulseEvents()
		{
			foreach (Action action in _eventCheckers)
			{
				try
				{
					action();
				}
				catch (Exception ex)
				{
					Logging.WriteDebug("Event checker {0} threw exception: {1}", action.Method.Name, ex.Message);
				}
			}
		}

		public static event OnBotStartDelegate OnBotStart
		{
			add
			{
				OnBotStartDelegate handler = _onBotStart;
				OnBotStartDelegate compare;
				do
				{
					compare = handler;
					OnBotStartDelegate combined = (OnBotStartDelegate)Delegate.Combine(compare, value);
					handler = Interlocked.CompareExchange(ref _onBotStart, combined, compare);
				}
				while (handler != compare);
			}
			remove
			{
				OnBotStartDelegate handler = _onBotStart;
				OnBotStartDelegate compare;
				do
				{
					compare = handler;
					OnBotStartDelegate removed = (OnBotStartDelegate)Delegate.Remove(compare, value);
					handler = Interlocked.CompareExchange(ref _onBotStart, removed, compare);
				}
				while (handler != compare);
			}
		}

		public static event OnBotStartDelegate OnBotStarted
		{
			add
			{
				OnBotStartDelegate handler = _onBotStarted;
				OnBotStartDelegate compare;
				do
				{
					compare = handler;
					OnBotStartDelegate combined = (OnBotStartDelegate)Delegate.Combine(compare, value);
					handler = Interlocked.CompareExchange(ref _onBotStarted, combined, compare);
				}
				while (handler != compare);
			}
			remove
			{
				OnBotStartDelegate handler = _onBotStarted;
				OnBotStartDelegate compare;
				do
				{
					compare = handler;
					OnBotStartDelegate removed = (OnBotStartDelegate)Delegate.Remove(compare, value);
					handler = Interlocked.CompareExchange(ref _onBotStarted, removed, compare);
				}
				while (handler != compare);
			}
		}

		public static event OnBotStopDelegate OnBotStop
		{
			add
			{
				OnBotStopDelegate handler = _onBotStop;
				OnBotStopDelegate compare;
				do
				{
					compare = handler;
					OnBotStopDelegate combined = (OnBotStopDelegate)Delegate.Combine(compare, value);
					handler = Interlocked.CompareExchange(ref _onBotStop, combined, compare);
				}
				while (handler != compare);
			}
			remove
			{
				OnBotStopDelegate handler = _onBotStop;
				OnBotStopDelegate compare;
				do
				{
					compare = handler;
					OnBotStopDelegate removed = (OnBotStopDelegate)Delegate.Remove(compare, value);
					handler = Interlocked.CompareExchange(ref _onBotStop, removed, compare);
				}
				while (handler != compare);
			}
		}

		public static event OnBotStopDelegate OnBotStopped
		{
			add
			{
				OnBotStopDelegate handler = _onBotStopped;
				OnBotStopDelegate compare;
				do
				{
					compare = handler;
					OnBotStopDelegate combined = (OnBotStopDelegate)Delegate.Combine(compare, value);
					handler = Interlocked.CompareExchange(ref _onBotStopped, combined, compare);
				}
				while (handler != compare);
			}
			remove
			{
				OnBotStopDelegate handler = _onBotStopped;
				OnBotStopDelegate compare;
				do
				{
					compare = handler;
					OnBotStopDelegate removed = (OnBotStopDelegate)Delegate.Remove(compare, value);
					handler = Interlocked.CompareExchange(ref _onBotStopped, removed, compare);
				}
				while (handler != compare);
			}
		}

		public static event OnBotChangedDelegate OnBotChanged
		{
			add
			{
				OnBotChangedDelegate handler = _onBotChanged;
				OnBotChangedDelegate compare;
				do
				{
					compare = handler;
					OnBotChangedDelegate combined = (OnBotChangedDelegate)Delegate.Combine(compare, value);
					handler = Interlocked.CompareExchange(ref _onBotChanged, combined, compare);
				}
				while (handler != compare);
			}
			remove
			{
				OnBotChangedDelegate handler = _onBotChanged;
				OnBotChangedDelegate compare;
				do
				{
					compare = handler;
					OnBotChangedDelegate removed = (OnBotChangedDelegate)Delegate.Remove(compare, value);
					handler = Interlocked.CompareExchange(ref _onBotChanged, removed, compare);
				}
				while (handler != compare);
			}
		}

		internal static void OnBotStartComplete()
		{
			_onBotStartComplete?.Invoke(EventArgs.Empty);
		}

		internal static void OnBotStopping()
		{
			_onBotStop?.Invoke(EventArgs.Empty);
		}

		/// <summary>
		/// Raises OnBotStart event - called BEFORE bot starts running.
		/// This is where SpellManager.Refresh() gets called.
		/// Equivalent to HB 3.3.5a smethod_3()
		/// </summary>
		internal static void RaiseBotStart()
		{
			_onBotStart?.Invoke(EventArgs.Empty);
		}

		/// <summary>
		/// Raises OnBotStarted event - called AFTER bot has started.
		/// </summary>
		internal static void RaiseBotStarted()
		{
			_onBotStarted?.Invoke(EventArgs.Empty);
		}

		internal static void RaiseBotStopped()
		{
			_onBotStopped?.Invoke(EventArgs.Empty);
		}

		internal static void RaisePulse(EventArgs args)
		{
			_onPulse?.Invoke(null, args);
		}

		internal static event EventHandler OnPulse
		{
			add
			{
				EventHandler handler = _onPulse;
				EventHandler compare;
				do
				{
					compare = handler;
					EventHandler combined = (EventHandler)Delegate.Combine(compare, value);
					handler = Interlocked.CompareExchange(ref _onPulse, combined, compare);
				}
				while (handler != compare);
			}
			remove
			{
				EventHandler handler = _onPulse;
				EventHandler compare;
				do
				{
					compare = handler;
					EventHandler removed = (EventHandler)Delegate.Remove(compare, value);
					handler = Interlocked.CompareExchange(ref _onPulse, removed, compare);
				}
				while (handler != compare);
			}
		}

		public delegate void OnBotStartDelegate(EventArgs args);

		public delegate void OnBotStopDelegate(EventArgs args);

		public delegate void OnBotChangedDelegate(BotChangedEventArgs args);

		public class BotChangedEventArgs : EventArgs
		{
			public string OldBot;
			public string NewBot;
		}

		public static class Profile
		{
			private static NewProfileLoadedDelegate _onNewProfileLoaded;
			private static NewProfileLoadedDelegate _onNewOuterProfileLoaded;

			public static event NewProfileLoadedDelegate OnNewProfileLoaded
			{
				add
				{
					NewProfileLoadedDelegate handler = _onNewProfileLoaded;
					NewProfileLoadedDelegate compare;
					do
					{
						compare = handler;
						NewProfileLoadedDelegate combined = (NewProfileLoadedDelegate)Delegate.Combine(compare, value);
						handler = Interlocked.CompareExchange(ref _onNewProfileLoaded, combined, compare);
					}
					while (handler != compare);
				}
				remove
				{
					NewProfileLoadedDelegate handler = _onNewProfileLoaded;
					NewProfileLoadedDelegate compare;
					do
					{
						compare = handler;
						NewProfileLoadedDelegate removed = (NewProfileLoadedDelegate)Delegate.Remove(compare, value);
						handler = Interlocked.CompareExchange(ref _onNewProfileLoaded, removed, compare);
					}
					while (handler != compare);
				}
			}

			public static event NewProfileLoadedDelegate OnNewOuterProfileLoaded
			{
				add
				{
					NewProfileLoadedDelegate handler = _onNewOuterProfileLoaded;
					NewProfileLoadedDelegate compare;
					do
					{
						compare = handler;
						NewProfileLoadedDelegate combined = (NewProfileLoadedDelegate)Delegate.Combine(compare, value);
						handler = Interlocked.CompareExchange(ref _onNewOuterProfileLoaded, combined, compare);
					}
					while (handler != compare);
				}
				remove
				{
					NewProfileLoadedDelegate handler = _onNewOuterProfileLoaded;
					NewProfileLoadedDelegate compare;
					do
					{
						compare = handler;
						NewProfileLoadedDelegate removed = (NewProfileLoadedDelegate)Delegate.Remove(compare, value);
						handler = Interlocked.CompareExchange(ref _onNewOuterProfileLoaded, removed, compare);
					}
					while (handler != compare);
				}
			}

			internal static void RaiseNewProfileLoaded(Logic.Profiles.Profile oldProf, Logic.Profiles.Profile newProf)
			{
				NewProfileLoadedEventArgs args = new NewProfileLoadedEventArgs
				{
					OldProfile = oldProf,
					NewProfile = newProf
				};
				_onNewProfileLoaded?.Invoke(args);
			}

			internal static void RaiseOuterProfileLoaded(Logic.Profiles.Profile oldProf, Logic.Profiles.Profile newProf)
			{
				NewProfileLoadedEventArgs args = new NewProfileLoadedEventArgs
				{
					OldProfile = oldProf,
					NewProfile = newProf
				};
				_onNewOuterProfileLoaded?.Invoke(args);
			}

			public class NewProfileLoadedEventArgs : EventArgs
			{
				public Logic.Profiles.Profile OldProfile;
				public Logic.Profiles.Profile NewProfile;
			}

			public delegate void NewProfileLoadedDelegate(NewProfileLoadedEventArgs args);
		}

		public static class Player
		{
			private static LevelUpDelegate _onLevelUp;
			private static MobKilledDelegate _onMobKilled;
			private static MobLootedDelegate _onMobLooted;
			private static MapChangedDelegate _onMapChanged;
			private static PlayerDiedDelegate _onPlayerDied;

			// HB 4.3.4 pattern: track last known level/map to detect changes
			private static int _lastKnownLevel;
			private static uint? _lastKnownMapId;

			/// <summary>
			/// HB 4.3.4 pattern: Check if player level changed since last pulse.
			/// Called every pulse by BotEvents.PulseEvents().
			/// </summary>
			internal static void CheckLevelChange()
			{
				if (ObjectManager.Me == null || !StyxWoW.IsInGame)
					return;

				int currentLevel = ObjectManager.Me.Level;

				// Initialize on first check
				if (_lastKnownLevel == 0)
				{
					_lastKnownLevel = currentLevel;
					return;
				}

				// Level increased
				if (currentLevel > _lastKnownLevel)
				{
					int oldLevel = _lastKnownLevel;
					_lastKnownLevel = currentLevel;
					RaiseLevelUp(oldLevel, currentLevel);
				}
			}

			/// <summary>
			/// HB 4.3.4 pattern: Check if map changed since last pulse.
			/// Called every pulse by BotEvents.PulseEvents().
			/// </summary>
			internal static void CheckMapChange()
			{
				if (ObjectManager.Me == null || !StyxWoW.IsInGame)
					return;

				uint currentMapId = ObjectManager.Me.MapId;

				// Initialize on first check
				if (!_lastKnownMapId.HasValue)
				{
					_lastKnownMapId = currentMapId;
					return;
				}

				// Map changed
				if (currentMapId != _lastKnownMapId.Value)
				{
					uint oldMapId = _lastKnownMapId.Value;
					_lastKnownMapId = currentMapId;
					RaiseMapChanged(oldMapId, currentMapId);
				}
			}

			public static event LevelUpDelegate OnLevelUp
			{
				add
				{
					LevelUpDelegate handler = _onLevelUp;
					LevelUpDelegate compare;
					do
					{
						compare = handler;
						LevelUpDelegate combined = (LevelUpDelegate)Delegate.Combine(compare, value);
						handler = Interlocked.CompareExchange(ref _onLevelUp, combined, compare);
					}
					while (handler != compare);
				}
				remove
				{
					LevelUpDelegate handler = _onLevelUp;
					LevelUpDelegate compare;
					do
					{
						compare = handler;
						LevelUpDelegate removed = (LevelUpDelegate)Delegate.Remove(compare, value);
						handler = Interlocked.CompareExchange(ref _onLevelUp, removed, compare);
					}
					while (handler != compare);
				}
			}

			public static event MobKilledDelegate OnMobKilled
			{
				add
				{
					MobKilledDelegate handler = _onMobKilled;
					MobKilledDelegate compare;
					do
					{
						compare = handler;
						MobKilledDelegate combined = (MobKilledDelegate)Delegate.Combine(compare, value);
						handler = Interlocked.CompareExchange(ref _onMobKilled, combined, compare);
					}
					while (handler != compare);
				}
				remove
				{
					MobKilledDelegate handler = _onMobKilled;
					MobKilledDelegate compare;
					do
					{
						compare = handler;
						MobKilledDelegate removed = (MobKilledDelegate)Delegate.Remove(compare, value);
						handler = Interlocked.CompareExchange(ref _onMobKilled, removed, compare);
					}
					while (handler != compare);
				}
			}

			public static event MobLootedDelegate OnMobLooted
			{
				add
				{
					MobLootedDelegate handler = _onMobLooted;
					MobLootedDelegate compare;
					do
					{
						compare = handler;
						MobLootedDelegate combined = (MobLootedDelegate)Delegate.Combine(compare, value);
						handler = Interlocked.CompareExchange(ref _onMobLooted, combined, compare);
					}
					while (handler != compare);
				}
				remove
				{
					MobLootedDelegate handler = _onMobLooted;
					MobLootedDelegate compare;
					do
					{
						compare = handler;
						MobLootedDelegate removed = (MobLootedDelegate)Delegate.Remove(compare, value);
						handler = Interlocked.CompareExchange(ref _onMobLooted, removed, compare);
					}
					while (handler != compare);
				}
			}

			public static event MapChangedDelegate OnMapChanged
			{
				add
				{
					MapChangedDelegate handler = _onMapChanged;
					MapChangedDelegate compare;
					do
					{
						compare = handler;
						MapChangedDelegate combined = (MapChangedDelegate)Delegate.Combine(compare, value);
						handler = Interlocked.CompareExchange(ref _onMapChanged, combined, compare);
					}
					while (handler != compare);
				}
				remove
				{
					MapChangedDelegate handler = _onMapChanged;
					MapChangedDelegate compare;
					do
					{
						compare = handler;
						MapChangedDelegate removed = (MapChangedDelegate)Delegate.Remove(compare, value);
						handler = Interlocked.CompareExchange(ref _onMapChanged, removed, compare);
					}
					while (handler != compare);
				}
			}

			public static event PlayerDiedDelegate OnPlayerDied
			{
				add
				{
					PlayerDiedDelegate handler = _onPlayerDied;
					PlayerDiedDelegate compare;
					do
					{
						compare = handler;
						PlayerDiedDelegate combined = (PlayerDiedDelegate)Delegate.Combine(compare, value);
						handler = Interlocked.CompareExchange(ref _onPlayerDied, combined, compare);
					}
					while (handler != compare);
				}
				remove
				{
					PlayerDiedDelegate handler = _onPlayerDied;
					PlayerDiedDelegate compare;
					do
					{
						compare = handler;
						PlayerDiedDelegate removed = (PlayerDiedDelegate)Delegate.Remove(compare, value);
						handler = Interlocked.CompareExchange(ref _onPlayerDied, removed, compare);
					}
					while (handler != compare);
				}
			}

			internal static void RaiseLevelUp(int oldLevel, int newLevel)
			{
				LevelUpEventArgs args = new LevelUpEventArgs
				{
					OldLevel = oldLevel,
					NewLevel = newLevel
				};
				_onLevelUp?.Invoke(args);
			}

			internal static void RaiseMobKilled(WoWUnit killedMob)
			{
				MobKilledEventArgs args = new MobKilledEventArgs
				{
					KilledMob = killedMob
				};
				_onMobKilled?.Invoke(args);
			}

			internal static void RaiseMobLooted(WoWUnit lootedMob)
			{
				MobLootedEventArgs args = new MobLootedEventArgs
				{
					LootedMob = lootedMob
				};
				_onMobLooted?.Invoke(args);
			}

			internal static void RaiseMapChanged(uint oldMap, uint newMap)
			{
				MapChangedEventArgs args = new MapChangedEventArgs
				{
					OldMapId = oldMap,
					NewMapId = newMap
				};
				_onMapChanged?.Invoke(args);
			}

			internal static void RaisePlayerDied()
			{
				_onPlayerDied?.Invoke();
			}

			public class LevelUpEventArgs : EventArgs
			{
				public int NewLevel;
				public int OldLevel;
			}

			public class MobKilledEventArgs : EventArgs
			{
				public WoWUnit KilledMob;
			}

			public class MobLootedEventArgs : EventArgs
			{
				public WoWUnit LootedMob;
			}

			public class MapChangedEventArgs : EventArgs
			{
				public uint OldMapId;
				public uint NewMapId;
			}

			public delegate void LevelUpDelegate(LevelUpEventArgs args);

			public delegate void MobKilledDelegate(MobKilledEventArgs args);

			public delegate void MobLootedDelegate(MobLootedEventArgs args);

			public delegate void MapChangedDelegate(MapChangedEventArgs args);

			public delegate void PlayerDiedDelegate();
		}

		public static class Battleground
		{
			private static BattlegroundEnterDelegate _onBattlegroundEntered;
			private static BattlegroundLeftDelegate _onBattlegroundLeft;

			public static event BattlegroundEnterDelegate OnBattlegroundEntered
			{
				add
				{
					BattlegroundEnterDelegate handler = _onBattlegroundEntered;
					BattlegroundEnterDelegate compare;
					do
					{
						compare = handler;
						BattlegroundEnterDelegate combined = (BattlegroundEnterDelegate)Delegate.Combine(compare, value);
						handler = Interlocked.CompareExchange(ref _onBattlegroundEntered, combined, compare);
					}
					while (handler != compare);
				}
				remove
				{
					BattlegroundEnterDelegate handler = _onBattlegroundEntered;
					BattlegroundEnterDelegate compare;
					do
					{
						compare = handler;
						BattlegroundEnterDelegate removed = (BattlegroundEnterDelegate)Delegate.Remove(compare, value);
						handler = Interlocked.CompareExchange(ref _onBattlegroundEntered, removed, compare);
					}
					while (handler != compare);
				}
			}

			public static event BattlegroundLeftDelegate OnBattlegroundLeft
			{
				add
				{
					BattlegroundLeftDelegate handler = _onBattlegroundLeft;
					BattlegroundLeftDelegate compare;
					do
					{
						compare = handler;
						BattlegroundLeftDelegate combined = (BattlegroundLeftDelegate)Delegate.Combine(compare, value);
						handler = Interlocked.CompareExchange(ref _onBattlegroundLeft, combined, compare);
					}
					while (handler != compare);
				}
				remove
				{
					BattlegroundLeftDelegate handler = _onBattlegroundLeft;
					BattlegroundLeftDelegate compare;
					do
					{
						compare = handler;
						BattlegroundLeftDelegate removed = (BattlegroundLeftDelegate)Delegate.Remove(compare, value);
						handler = Interlocked.CompareExchange(ref _onBattlegroundLeft, removed, compare);
					}
					while (handler != compare);
				}
			}

			public static class SotA
			{
				private static GateDestroyedDelegate _onGateDestroyed;
				private static SwitchedSideDelegate _onSwitchedSide;

				public static event GateDestroyedDelegate OnGateDestroyed
				{
					add
					{
						GateDestroyedDelegate handler = _onGateDestroyed;
						GateDestroyedDelegate compare;
						do
						{
							compare = handler;
							GateDestroyedDelegate combined = (GateDestroyedDelegate)Delegate.Combine(compare, value);
							handler = Interlocked.CompareExchange(ref _onGateDestroyed, combined, compare);
						}
						while (handler != compare);
					}
					remove
					{
						GateDestroyedDelegate handler = _onGateDestroyed;
						GateDestroyedDelegate compare;
						do
						{
							compare = handler;
							GateDestroyedDelegate removed = (GateDestroyedDelegate)Delegate.Remove(compare, value);
							handler = Interlocked.CompareExchange(ref _onGateDestroyed, removed, compare);
						}
						while (handler != compare);
					}
				}

				public static event SwitchedSideDelegate OnSwitchedSide
				{
					add
					{
						SwitchedSideDelegate handler = _onSwitchedSide;
						SwitchedSideDelegate compare;
						do
						{
							compare = handler;
							SwitchedSideDelegate combined = (SwitchedSideDelegate)Delegate.Combine(compare, value);
							handler = Interlocked.CompareExchange(ref _onSwitchedSide, combined, compare);
						}
						while (handler != compare);
					}
					remove
					{
						SwitchedSideDelegate handler = _onSwitchedSide;
						SwitchedSideDelegate compare;
						do
						{
							compare = handler;
							SwitchedSideDelegate removed = (SwitchedSideDelegate)Delegate.Remove(compare, value);
							handler = Interlocked.CompareExchange(ref _onSwitchedSide, removed, compare);
						}
						while (handler != compare);
					}
				}

				public delegate void GateDestroyedDelegate(SotAGateType type);

				public delegate void SwitchedSideDelegate(SotAObjective objective);
			}

			public delegate void BattlegroundEnterDelegate(BattlegroundType type);

			public delegate void BattlegroundLeftDelegate(EventArgs args);
		}

		public static class Questing
		{
			private static QuestAcceptedDelegate _onQuestAccepted;

			public static event QuestAcceptedDelegate OnQuestAccepted
			{
				add
				{
					QuestAcceptedDelegate handler = _onQuestAccepted;
					QuestAcceptedDelegate compare;
					do
					{
						compare = handler;
						QuestAcceptedDelegate combined = (QuestAcceptedDelegate)Delegate.Combine(compare, value);
						handler = Interlocked.CompareExchange(ref _onQuestAccepted, combined, compare);
					}
					while (handler != compare);
				}
				remove
				{
					QuestAcceptedDelegate handler = _onQuestAccepted;
					QuestAcceptedDelegate compare;
					do
					{
						compare = handler;
						QuestAcceptedDelegate removed = (QuestAcceptedDelegate)Delegate.Remove(compare, value);
						handler = Interlocked.CompareExchange(ref _onQuestAccepted, removed, compare);
					}
					while (handler != compare);
				}
			}

			public delegate void QuestAcceptedDelegate(Logic.Questing.Quest quest);
		}
	}

	// Enums pour BotEvents.Battleground
	public enum BattlegroundType
	{
		None,
		AlteracValley,
		WarsongGulch,
		ArathiBasin,
		EyeOfTheStorm,
		StrandOfTheAncients,
		IsleOfConquest,
		Unknown
	}

	public enum SotAGateType
	{
		None,
		Green,
		Blue,
		Red,
		Purple,
		Yellow
	}

	public enum SotAObjective
	{
		None,
		Attacking,
		Defending
	}
}
