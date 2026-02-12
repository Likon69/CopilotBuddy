using System;
using System.Threading;
using System.Windows.Media;
using Styx.Helpers;
using Styx.Logic.Combat;
using Styx.Logic.Pathing;
using Styx.Logic.POI;
using Styx.Logic.Profiles;
using Styx.WoWInternals;
using TreeSharp;

namespace Styx.Logic.BehaviorTree
{
	public static class TreeRoot
	{
		private static Thread? _workerThread;
		private static string _statusText = "";
		private static string _goalText = "";
		private static readonly object _stateLock = new object();

		public static byte TicksPerSecond { get; set; }

		public static BotBase? Current
		{
			get { return BotManager.Current; }
		}

		/// <summary>
		/// State machine for TreeRoot lifecycle — matches HB 6.2.3.
		/// Stop() sets Stopping; worker thread does cleanup then sets Stopped.
		/// </summary>
		public static TreeRootState State { get; private set; } = TreeRootState.Stopped;

		/// <summary>
		/// Returns true when the bot is Running (excludes Stopping/Stopped/Starting).
		/// Matches HB 6.2.3 computed property.
		/// </summary>
		public static bool IsRunning
		{
			get { return _workerThread != null && _workerThread.IsAlive && State == TreeRootState.Running; }
		}

		static TreeRoot()
		{
			BotEvents.OnBotStart += OnBotStart;
			TicksPerSecond = 13; // ARCH-02: Matches HB 3.3.5a's 13 TPS (~77ms per tick)
		}

		private static void OnBotStart(EventArgs args)
		{
			// Initialize spell database from Spells.bin before refreshing spells
			SpellDb.Initialize();
			Lua.DoString("ClearTarget();");
			Lua.DoString("MoveForwardStop();MoveBackwardStop();StrafeLeftStop();StrafeRightStop();");

			// ARCH-03: Set CVars for bot safety (HB 3.3.5a sets autoSelfCast + autoLootDefault)
			Lua.DoString("SetCVar('autoSelfCast', 1)");
			Lua.DoString("SetCVar('autoLootDefault', 1)");

			BotPoi.Clear();
			SpellManager.Refresh();
			if (RoutineManager.Current == null)
			{
				throw new Exception("Unable to start. No Combat Routine loaded.");
			}
		}

		// ARCH-03: Track fall state
		private static bool _wasFalling;
		private static int _fallStartTick;

		private static void Tick()
		{
			if (State != TreeRootState.Running)
				return;

			if (StyxWoW.Me == null || !StyxWoW.IsInGame)
			{
				Logging.Write("Not in game");
				Thread.Sleep(1000);
				return;
			}

			// ARCH-03: Check if WoW process has exited
			if (ObjectManager.WoWProcess != null && ObjectManager.WoWProcess.HasExited)
			{
				Logging.Write("WoW process has exited. Stopping bot.");
				Stop("WoW process exited");
				return;
			}

			// ARCH-03: Skip tick while on taxi (flight path)
			if (StyxWoW.Me.OnTaxi)
			{
				Thread.Sleep(1000 / (int)TicksPerSecond);
				return;
			}

			// ARCH-03: Fall tracking — clear navigator during long free-falls
			bool isFalling = StyxWoW.Me.IsFalling;
			if (isFalling && !_wasFalling)
			{
				_fallStartTick = Environment.TickCount;
			}
			else if (isFalling && _wasFalling)
			{
				int fallDuration = Environment.TickCount - _fallStartTick;
				if (fallDuration > 3000) // 3+ seconds of falling
				{
					Navigator.Clear();
				}
			}
			_wasFalling = isFalling;

			WoWPulsator.Pulse(Current?.PulseFlags ?? PulseFlags.All);
			BotEvents.RaisePulse(EventArgs.Empty);

			// BUG-03 fix: Removed mid-air dismount that killed the player.
			// If mounted and flying, let the bot run normally — dismount is
			// handled by individual bot behaviors when appropriate.
			Current?.Pulse();
			try
			{
				Current?.Root?.Tick(null);
				RunStatus? lastStatus = Current?.Root?.LastStatus;
				if (lastStatus != RunStatus.Running)
				{
					Current?.Root?.Stop(null);
					Current?.Root?.Start(null);
				}
			}
			catch (Exception ex)
			{
				Logging.WriteException(ex);
				Current?.Root?.Stop(null);
				Current?.Root?.Start(null);
				BotPoi.Clear();
			}
			Thread.Sleep(1000 / (int)TicksPerSecond);
		}

		public static void Start()
		{
			if (State != TreeRootState.Stopped || Current == null)
				return;

			try
			{
				State = TreeRootState.Starting;

				// Grab frame first to sync with WoW's main thread (like HB 3.3.5a)
				if (ObjectManager.Executor != null)
				{
					ObjectManager.Executor.GrabFrame();
				}
				ObjectManager.Update();
				string lastUsedPath = LevelbotSettings.Instance.LastUsedPath;
				if (!string.IsNullOrEmpty(lastUsedPath) && System.IO.File.Exists(lastUsedPath))
				{
				ProfileManager.LoadNew(lastUsedPath);
				}
				// BUG-20 fix: Use DoInitialize() which guards against double-init
				Current.DoInitialize();
				Current.Start();
				Current.Root?.Start(null);

				// Initialize Navigator BEFORE RaiseBotStart to ensure mesh loading happens
				// Navigator's static constructor subscribes to OnBotStart, so we need to
				// force it to initialize first by touching it
				_ = Navigator.PathPrecision; // Forces static constructor to run

				// RaiseBotStart triggers OnBotStart which calls SpellManager.Refresh()
				// This must be called BEFORE the worker thread starts (like HB 3.3.5a smethod_3)
				BotEvents.RaiseBotStart();

				// Worker thread transitions to Running once it starts ticking
				_workerThread = new Thread(WorkerThread);
				_workerThread.IsBackground = true;
				_workerThread.Name = "TreeRoot Worker";
				_workerThread.Start();

				BotEvents.OnBotStartComplete();
			}
			catch (HonorbuddyUnableToStartException ex)
			{
				Logging.Write(Colors.Red, "Unable to start: {0}", ex.Message);
				State = TreeRootState.Stopped;
			}
			catch (Exception ex)
			{
				Logging.WriteException(ex);
				State = TreeRootState.Stopped;
			}
		}

		/// <summary>
		/// Stop the bot. HB 6.2.3 pattern: only sets State = Stopping.
		/// The worker thread detects the state change, finishes the current tick,
		/// then does the actual cleanup on its own thread — no race condition.
		/// </summary>
		public static void Stop(string? reason = null)
		{
			lock (_stateLock)
			{
				if (State != TreeRootState.Running && State != TreeRootState.Starting)
					return;

				Logging.Write(Colors.DeepSkyBlue, "Bot stopping! Reason: {0}", reason ?? "User request");
				State = TreeRootState.Stopping;
			}

			// Only join if we're NOT on the worker thread.
			// Bot behaviors (ForcedBehaviorExecutor, QuestBot, etc.) call Stop()
			// from the worker thread — joining ourselves would deadlock.
			if (Thread.CurrentThread != _workerThread && _workerThread != null)
			{
				if (!_workerThread.Join(TimeSpan.FromSeconds(5)))
				{
					Logging.WriteDebug("Worker thread did not exit gracefully within 5s");
				}
				_workerThread = null;
			}
			// If called from worker thread, the while loop will exit naturally
			// and cleanup happens in the finally block.
		}

		private static void WorkerThread()
		{
			// Use lock to prevent race with Stop() called right after Start()
			lock (_stateLock)
			{
				if (State != TreeRootState.Starting)
				{
					// Stop() was called before we could start — bail out
					State = TreeRootState.Stopped;
					return;
				}
				State = TreeRootState.Running;
			}

			try
			{
				// Main tick loop — exits when Stop() sets State to Stopping
				while (State == TreeRootState.Running)
				{
					Tick();
				}
			}
			catch (Exception ex)
			{
				Logging.WriteException(ex);
			}
			finally
			{
				// HB 6.2.3 pattern: cleanup happens on the WORKER THREAD
				// — same thread that was calling Tick(), so no race condition
				try
				{
					try { ObjectManager.Executor?.GrabFrame(); } catch { }
					try { Navigator.Clear(); } catch { }
					try { BotEvents.OnBotStopping(); } catch { }
					try { Current?.Stop(); } catch { }
					try { Current?.Root?.Stop(null); } catch { }
					try { BotEvents.RaiseBotStopped(); } catch { }
				}
				catch (Exception ex)
				{
					Logging.WriteException(ex);
				}

				State = TreeRootState.Stopped;
				Logging.WriteDebug("Worker thread exited cleanly");
			}
		}

		public static void Restart()
		{
			Stop();
			Start();
		}

		/// <summary>
		/// Current activity text — displayed in the StatusBar at the bottom of the UI.
		/// Fires OnStatusTextChanged event (same as HB 4.3.4).
		/// </summary>
		public static string StatusText
		{
			get { return _statusText; }
			set
			{
				if (!string.IsNullOrEmpty(value) && _statusText != value)
				{
					Logging.WriteDebug("Activity: {0}", value);
				}
				string oldStatus = _statusText;
				_statusText = value;
				OnStatusTextChanged?.Invoke(null, new StatusTextChangedEventArgs(oldStatus, value));
			}
		}

		/// <summary>
		/// Event fired when StatusText changes — UI subscribes to update StatusBar.
		/// Same as HB 4.3.4's TreeRoot.OnStatusTextChanged.
		/// </summary>
		public static event EventHandler<StatusTextChangedEventArgs> OnStatusTextChanged;

		/// <summary>
		/// High-level goal text — displayed in the Info panel.
		/// Same as HB 4.3.4 (no event, polled by UpdateInfoPanel timer).
		/// </summary>
		public static string GoalText
		{
			get { return _goalText; }
			set
			{
				if (!string.IsNullOrEmpty(value) && _goalText != value)
				{
					Logging.WriteDebug("Goal: {0}", value);
				}
				_goalText = value;
			}
		}
	}
}
