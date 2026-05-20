using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using CommonBehaviors.Actions;
using Styx.Common;
using Styx.CommonBot.Coroutines;
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
		private static Thread? _minimizeGuardThread;
		private static string _statusText = "";
		private static string _goalText = "";
		private static readonly object _stateLock = new object();
		private static WindowPlacement? _lastNonMinimizedPlacement;

		// HB 6.2.3: Coroutine pre-tick checks (ns47.Class678 → HookCoroutineTask)
		private static HookCoroutineTask? _inGameCheck;
		private static HookCoroutineTask? _taxiCheck;
		private static ActionRunCoroutine? _composite0;
		private static bool _onTaxi; // HB 6.2.3 bool_2: tracks taxi state across ticks
		private static bool _paused; // HB 6.2.3 bool_3: tracks whether pause event has fired

		// HB 6.2.3: Win32 imports for minimization guard
		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetWindowPlacement(IntPtr hWnd, ref WindowPlacement lpwndpl);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WindowPlacement lpwndpl);

		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		// HB 6.2.3: WINDOWPLACEMENT struct
		[StructLayout(LayoutKind.Sequential)]
		private struct WindowPlacement
		{
			public int length;
			public int flags;
			public int showCmd;
			public System.Drawing.Point ptMinPosition;
			public System.Drawing.Point ptMaxPosition;
			public RECT rcNormalPosition;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct RECT
		{
			public int Left, Top, Right, Bottom;
		}

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
		/// Returns true when the bot thread is alive and not stopping.
		/// HB 6.2.3: thread != null && thread.IsAlive && State != Stopping
		/// </summary>
		public static bool IsRunning
		{
			get { return _workerThread != null && _workerThread.IsAlive && State != TreeRootState.Stopping && State != TreeRootState.Stopped; }
		}

		/// <summary>HB 5.4.8: True when the calling thread is the bot worker thread.</summary>
		public static bool CurrentThreadIsBotThread => Thread.CurrentThread == _workerThread;

		/// <summary>HB 6.2.3: true when State == Paused.</summary>
		public static bool IsPaused => State == TreeRootState.Paused;

		/// <summary>HB 6.2.3: Transition Running → Paused.</summary>
		public static void Pause()
		{
			if (State != TreeRootState.Running) return;
			State = TreeRootState.Paused;
		}

		/// <summary>HB 6.2.3: Transition Paused → Running.</summary>
		public static void Resume()
		{
			if (State != TreeRootState.Paused) return;
			State = TreeRootState.Running;
		}

		static TreeRoot()
		{
			BotEvents.OnBotStart += OnBotStart;
			TicksPerSecond = 13; // ARCH-02: Matches HB 3.3.5a's 13 TPS (~77ms per tick)
		}

		/// <summary>
		/// HB 6.2.3 smethod_3: Called once during initialization to set up
		/// event handlers and start the minimize guard thread.
		/// </summary>
		internal static void Initialize()
		{
			StartMinimizeGuard();
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
			SpellManager.Initialize();
			if (RoutineManager.Current == null)
			{
				throw new Exception("Unable to start. No Combat Routine loaded.");
			}
		}

		// ARCH-03: Track fall state
		private static bool _wasFalling;
		private static int _fallStartTick;

		/// <summary>
		/// HB 6.2.3 smethod_1: Check if WoW is minimized — restore if so.
		/// Called every 50ms by the minimize guard thread.
		/// </summary>
		private static void CheckAndRestoreMinimized()
		{
			var mem = StyxWoW.Memory;
			if (mem == null) return;

			IntPtr hWnd = mem.WindowHandle;
			if (hWnd == IntPtr.Zero) return;

			var placement = new WindowPlacement();
			placement.length = Marshal.SizeOf<WindowPlacement>();
			if (!GetWindowPlacement(hWnd, ref placement))
				return;

			// showCmd: 2=SW_SHOWMINIMIZED, 6=SW_MINIMIZE, 7=SW_SHOWMINNOACTIVE
			bool isMinimized = placement.showCmd == 2 || placement.showCmd == 6 || placement.showCmd == 7;

			if (IsRunning && isMinimized)
			{
				Logging.Write(Colors.Red, "WoW cannot be minimized while the bot is running — restoring window.");

				if (_lastNonMinimizedPlacement != null)
				{
					var restore = _lastNonMinimizedPlacement.Value;
					restore.showCmd = 9; // SW_RESTORE
					SetWindowPlacement(hWnd, ref restore);
				}
				else
				{
					ShowWindow(hWnd, 9); // SW_RESTORE
				}

				Thread.Sleep(500);
			}

			// Cache last known non-minimized placement
			if (!isMinimized)
			{
				_lastNonMinimizedPlacement = placement;
			}
		}

		/// <summary>
		/// HB 6.2.3 smethod_2: Polling loop for minimization guard.
		/// Runs on a background thread for the entire process lifetime.
		/// </summary>
		private static void MinimizeGuardLoop()
		{
			try
			{
				while (StyxWoW.Memory != null && !StyxWoW.Memory.Process.HasExited)
				{
					CheckAndRestoreMinimized();
					Thread.Sleep(50);
				}
			}
			catch
			{
				// Process exited or Memory disposed — silently exit
			}
		}

		/// <summary>
		/// HB 6.2.3 smethod_3: Start the minimization guard thread.
		/// Called once during initialization.
		/// </summary>
		internal static void StartMinimizeGuard()
		{
			if (_minimizeGuardThread != null && _minimizeGuardThread.IsAlive)
				return;

			_minimizeGuardThread = new Thread(MinimizeGuardLoop)
			{
				IsBackground = true,
				Name = "No minimizing WoW"
			};
			_minimizeGuardThread.Start();
		}

		/// <summary>
		/// HB 6.2.3 Composite_0: coroutine composite that runs InGame + Taxi
		/// pre-tick checks via ActionRunCoroutine before the main bot root.
		/// </summary>
		private static ActionRunCoroutine Composite0
		{
			get
			{
				if (_composite0 == null)
				{
					_inGameCheck = new HookCoroutineTask(
						"InGame_Check",
						"IsInGame check location. Warning: nothing is pulsed before this. Only hook this location for relogging purposes.",
						InGameCheckAsync);
					_taxiCheck = new HookCoroutineTask(
						"Taxi_Check",
						"Taxi check",
						TaxiCheckAsync);
					_composite0 = new ActionRunCoroutine(PreTickCoroutineAsync);
				}
				return _composite0;
			}
		}

		/// <summary>
		/// HB 6.2.3 Class1150.method_0: InGame check.
		/// Returns true to SKIP the bot tick (not in game).
		/// </summary>
		private static async Task<bool> InGameCheckAsync()
		{
			if (StyxWoW.IsInWorld)
				return false; // in game → don't skip
			Logging.Write("Not in game");
			return true; // not in game → skip tick
		}

		/// <summary>
		/// HB 6.2.3 Class1150.method_1: Taxi check.
		/// Returns true to SKIP the bot tick (on taxi).
		/// </summary>
		private static async Task<bool> TaxiCheckAsync()
		{
			if (StyxWoW.Me != null && StyxWoW.Me.OnTaxi)
			{
				_onTaxi = true;
				StatusText = "Waiting on taxi...";
				StyxWoW.ResetAfk();
				return true; // on taxi → skip tick
			}
			if (_onTaxi)
			{
				_onTaxi = false;
				BotPoi.Clear("Reached final destination with taxi");
				return true; // just got off taxi → skip one tick to settle
			}
			return false; // not on taxi → don't skip
		}

		/// <summary>
		/// HB 6.2.3 Class1150.method_2: Main pre-tick coroutine.
		/// Awaits InGame check then Taxi check; returns true to skip bot tick.
		/// </summary>
		private static async Task<bool> PreTickCoroutineAsync(object? context)
		{
			if (await _inGameCheck!)
				return true;

			if (await _taxiCheck!)
				return true;

			return false;
		}

		/// <summary>
		/// HB 6.2.3 smethod_7: Safe action executor with exception handling.
		/// Returns true if action succeeded, false if exception was thrown.
		/// </summary>
		private static bool SafeAction(System.Action action, string name, bool stopOnError)
		{
			try
			{
				action();
				return true;
			}
			catch (Exception ex)
			{
				Logging.WriteDiagnostic("Exception was thrown in {0}", name);
				Logging.WriteDiagnostic(ex.ToString());
				if (stopOnError)
					Stop("Exception thrown");
				return false;
			}
		}

		private static void Tick()
		{
			if (State != TreeRootState.Running && State != TreeRootState.Paused)
				return;

			// sync ticks-per-second slider value (per-character)
			TicksPerSecond = CharacterSettings.Instance.TicksPerSecond;
			var sw = Stopwatch.StartNew();
			if (StyxSettings.Instance.UseFrameLock)
			{
				using (StyxWoW.Memory.AcquireFrame(true))
				using (StyxWoW.Memory.TemporaryCacheState(true))
				{
					StyxWoW.Memory.ClearCache();
					RunTickBody();
				}
			}
			else
			{
				// Soft frame mode: keep continuous execution for the tick without hard frame grab.
				// This avoids per-call EndScene waits when the game is throttled (background/low FPS)
				// while reducing the visual tearing seen with hard frame lock.
				using (StyxWoW.Memory.AcquireFrame(false))
				using (StyxWoW.Memory.TemporaryCacheState(true))
				{
					StyxWoW.Memory.ClearCache();
					RunTickBody();
				}
			}

			// HB 6.2.3: Force-release leaked frame lock (runs for BOTH branches)
			var executor = ObjectManager.Executor;
			if (executor != null && executor.IsExecutingContinuously)
			{
				lock (executor.AssemblyLock)
				{
					if (executor.IsExecutingContinuously)
					{
						Logging.WriteDiagnostic("Frame lock was forcibly released after tick completed");
						executor.EndExecute();
					}
				}
			}

			// HB 6.2.3 pattern: sleep OUTSIDE AcquireFrame so WoW can render
			int remainingMs = (int)Math.Ceiling(1000.0 / TicksPerSecond - sw.Elapsed.TotalMilliseconds);
			if (remainingMs <= 0)
			{
				Logging.WriteDebug("[PERF] Tick took {0:F0}ms (budget {1:F0}ms) - over budget by {2:F0}ms",
					sw.Elapsed.TotalMilliseconds,
					1000.0 / TicksPerSecond,
					-remainingMs);
			}
			if (remainingMs > 0)
			{
				Thread.Sleep(remainingMs);
			}
		}

		/// <summary>
		/// HB 6.2.3 smethod_8: Main tick body using coroutine pre-checks.
		/// 1. Pulse if in game
		/// 2. Run Composite_0 (InGame + Taxi coroutine checks)
		/// 3. If checks pass (Failure = both false), pulse BotBase and tick Root
		/// </summary>
		private static void RunTickBody()
		{
			// HB 6.2.3: Pause/resume handling — must be FIRST in tick body
			if (IsPaused)
			{
				if (!_paused)
				{
					Logging.Write(Colors.Lime, "Bot paused");
					SafeAction(() => Current!.OnPaused(), "BotBase.Pause", true);
					BotEvents.RaiseBotPaused();
					_paused = true;
				}
				return;
			}
			if (_paused)
			{
				Logging.Write(Colors.Lime, "Bot resumed");
				SafeAction(() => Current!.OnResumed(), "BotBase.Resume", true);
				BotEvents.RaiseBotResumed();
				_paused = false;
			}

			// ARCH-03: Check if WoW process has exited
			if (ObjectManager.WoWProcess != null && ObjectManager.WoWProcess.HasExited)
			{
				Logging.Write("WoW process has exited. Stopping bot.");
				Stop("WoW process exited");
				return;
			}

			if (StyxWoW.IsInGame)
			{
				PulseFlags flags = Current?.PulseFlags ?? PulseFlags.All;

				WoWPulsator.Pulse(flags);
				BotEvents.RaisePulse(EventArgs.Empty);
			}

			// HB 6.2.3: Run Composite_0 (InGame + Taxi pre-checks via coroutine)
			if (Composite0.LastStatus != RunStatus.Running)
			{
				Composite0.Start(null);
			}
			if (!SafeAction(() => Composite0.Tick(null), "MainRoot.Tick", true))
			{
				Composite0.Stop(null);
				return;
			}

			RunStatus? lastStatus = Composite0.LastStatus;
			if (lastStatus != RunStatus.Running)
			{
				Composite0.Stop(null);
			}

			// HB 6.2.3: If Composite_0 returned Success (true) or is still Running,
			// the pre-checks want to skip the bot tick
			if (lastStatus != RunStatus.Failure)
				return;

			// HB 6.2.3: Pre-checks returned false → game is running normally → tick bot

			// ARCH-03: Fall tracking — clear navigator during long free-falls
			bool isFalling = StyxWoW.Me != null && StyxWoW.Me.IsFalling;
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

			Current?.Pulse();

			if (Current?.Root == null)
				return;

			if (Current.Root.LastStatus != RunStatus.Running)
			{
				Current.Root.Start(null);
			}
			if (!SafeAction(() => Current!.Root.Tick(null), "BotBase.Root.Tick", false))
			{
				BotPoi.Clear("Exception in Root.Tick");
				Current.Root.Stop(null);
				return;
			}
			if (Current.Root.LastStatus != RunStatus.Running)
			{
				Current.Root.Stop(null);
			}
		}

		public static void Start()
		{
			// HB 6.2.3: entire Start() body under lock to prevent double-click race
			lock (_stateLock)
			{
				if (Current == null)
					return;

				// HB 6.2.3 pattern: if a previous thread is still stopping, wait for it to exit.
				// IMPORTANT: do NOT wait while holding the lock if the caller is the worker thread
				// itself (e.g. ChangeBotAction via Dispatcher.Invoke) — that would deadlock.
				// Use a timed wait outside the lock instead.
				if (State == TreeRootState.Stopping)
				{
					if (_workerThread != null && _workerThread.IsAlive &&
					    Thread.CurrentThread != _workerThread)
					{
						// Release the lock while waiting so the worker thread can acquire it to set Stopped
						Monitor.Exit(_stateLock);
						bool exited = _workerThread.Join(5000); // 5s max
						Monitor.Enter(_stateLock);
						if (!exited)
							Logging.WriteDebug("[TreeRoot] Worker thread did not stop in time — forcing Stopped state");
					}
					State = TreeRootState.Stopped;
				}

				if (State != TreeRootState.Stopped)
					return;

				try
				{
					State = TreeRootState.Starting;

					// HB 6.2.3: ObjectManager.Update() directly, no GrabFrame
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
					throw; // HB 4.3.4 pattern: propagate to caller (MainWindow) so UI can recover
				}
				catch (Exception ex)
				{
					Logging.WriteException(ex);
					State = TreeRootState.Stopped;
					throw; // propagate so UI can recover
				}
			}
		}

		/// <summary>
		/// Stop the bot. HB 6.2.3 pattern: ONLY sets State = Stopping.
		/// The worker thread detects the state change, finishes the current tick,
		/// then does the actual cleanup on its own thread — no race condition.
		/// NO Join() — HB 6.2.3 never joins the worker thread from Stop().
		/// </summary>
		public static void Stop(string? reason = null)
		{
			lock (_stateLock)
			{
				if (State != TreeRootState.Running && State != TreeRootState.Starting && State != TreeRootState.Paused)
					return;

				Logging.Write(Colors.DeepSkyBlue, "Bot stopping! Reason: {0}", reason ?? "User request");
				State = TreeRootState.Stopping;
			}
			// HB 6.2.3: Stop() returns immediately. The worker thread sees
			// State == Stopping, exits its while loop, and does cleanup in
			// its own finally block. No Join(), no blocking the UI thread.
		}

		private static void WorkerThread()
		{
			// HB 4.3.4/6.2.3: Set invariant culture on bot thread so float.ToString()
			// always produces "1.5" (not "1,5" on European locales). Lua DoString
			// embeds numbers — wrong decimal separator breaks WoW API calls.
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

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
				// HB 6.2.3: Loop must continue for Paused state (tick body handles it)
				while (State == TreeRootState.Running || State == TreeRootState.Paused)
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
				// HB 6.2.3 pattern: cleanup on worker thread, NO GrabFrame.
				try
				{
					// HB 6.2.3 cleanup order: Current.Stop → Root.Stop → Navigator.Clear → BotPoi.Clear → RaiseBotStopped
					try { Current?.Stop(); } catch { }
					try { Current?.Root?.Stop(null); } catch { }
					try { Navigator.Clear(); } catch { }
					try { BotPoi.Clear(); } catch { }
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
