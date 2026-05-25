using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Styx.Combat.CombatRoutine;
using Styx.Helpers;
using Styx.Loaders;
using Styx.WoWInternals;

namespace Styx.Logic.Combat
{
	/// <summary>
	/// Manages combat routines - loads, compiles, and selects the appropriate routine.
	/// Follows HB pattern: Init() is called after WoW attachment when ObjectManager.Me is available.
	/// FEAT-37: Added CLI parsing, locking, improved selection.
	/// </summary>
	public static class RoutineManager
	{
		private static readonly List<CombatRoutine> _routines = new List<CombatRoutine>();
		private static CombatRoutine _current;
		private static bool _initialized;
		private static readonly object _initLock = new object();

		/// <summary>
		/// Initializes the RoutineManager. Called after WoW is attached and ObjectManager.Me is available.
		/// This is where routines are compiled and loaded, exactly like HB.
		/// </summary>
		public static void Init()
		{
			lock (_initLock)
			{
				if (_initialized)
					return;

				_initialized = true;
			}
			
			Logging.Write("Initializing Combat Routines...");
			
			// Load and compile routines
			LoadCombatRoutines();
			
			// FEAT-37: Check CLI arg /customclass=ClassName
			string? cliRoutine = GetCliRoutineName();
			if (cliRoutine != null)
			{
				Logging.Write("CLI: /customclass={0}", cliRoutine);
				if (SetCurrent(cliRoutine))
					return;
				Logging.Write("CLI routine '{0}' not found, falling back to auto-select.", cliRoutine);
			}

			// Auto-select routine for current class
			SelectRoutineForCurrentClass();
		}

		/// <summary>
		/// FEAT-37: Parses /customclass=ClassName from command line arguments.
		/// </summary>
		private static string? GetCliRoutineName()
		{
			string[] args = Environment.GetCommandLineArgs();
			foreach (string arg in args)
			{
				if (arg.StartsWith("/customclass=", StringComparison.OrdinalIgnoreCase))
					return arg.Substring("/customclass=".Length).Trim();
			}
			return null;
		}

		private static void LoadCombatRoutines()
		{
			string routinesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Routines");
			
			Logging.WriteDebug("Looking for routines in: {0}", routinesPath);
			
			if (!Directory.Exists(routinesPath))
			{
				Logging.WriteDebug("Routines folder not found, creating it...");
				Directory.CreateDirectory(routinesPath);
				return;
			}

			try
			{
				// HB 4.3.4 pattern: compile each root .cs file and each subdirectory separately
				// This prevents cross-routine namespace conflicts.
				var classCollection = new ClassCollection<CombatRoutine>();
				var entries = new List<string>();
				entries.AddRange(Directory.GetFiles(routinesPath, "*.cs", SearchOption.TopDirectoryOnly));
				entries.AddRange(Directory.GetDirectories(routinesPath, "*", SearchOption.TopDirectoryOnly));

				foreach (string entry in entries)
				{
					string entryName = Path.GetFileName(entry);
					Logging.WriteDebug("Compiling {0}", entryName);
					CompilerResults compilerResults;
					classCollection.CompileAndLoadFrom(entry, out compilerResults);

					if (compilerResults != null && compilerResults.Errors.HasErrors)
					{
						Logging.Write("Could not compile routine from: {0}", entryName);
						Logging.Write(Utilities.FormatCompilerErrors(compilerResults));
					}
				}

				foreach (CombatRoutine routine in classCollection)
				{
					_routines.Add(routine);
					Logging.Write("Loaded routine: {0} for class {1}", 
						routine.Name ?? "Unknown", routine.Class);
				}

				Logging.Write("Loaded {0} combat routine(s)", _routines.Count);
			}
			catch (Exception ex)
			{
				Logging.WriteException(ex);
			}
		}

		private static void SelectRoutineForCurrentClass()
		{
			if (ObjectManager.Me == null)
			{
				Logging.Write("Cannot select routine - player not available");
				_current = new DefaultCombatRoutine();
				return;
			}

			WoWClass playerClass = ObjectManager.Me.Class;
			var matching = _routines.Where(r => r.Class == playerClass).ToList();

			if (matching.Count == 0)
			{
				Logging.Write("Could not find a routine fitting for your class. Please make sure you have a proper combat class routine in your Routines folder, and restart CopilotBuddy. Using default.");
				_current = new DefaultCombatRoutine();
				return;
			}

			if (matching.Count == 1)
			{
				_current = matching[0];
			}
			else
			{
				// Multiple routines for this class — let the user pick (HB 4.3.4 pattern)
				var form = new RoutineSelectionForm(matching);
				if (form.ShowDialog() == DialogResult.OK && form.SelectedRoutine != null)
				{
					_current = form.SelectedRoutine;
				}
				else
				{
					// User cancelled — pick the first one
					_current = matching[0];
				}
			}

			Logging.Write("Chose {0} as your combat class.", _current.Name);
			Logging.Write("Confirmed {0} as your class.", playerClass);

			try
			{
				_current.Initialize();
				Logging.WriteDebug("Routine initialized successfully");
			}
			catch (Exception ex)
			{
				Logging.Write("Routine Initialize() failed: {0}", ex.Message);
				Logging.WriteException(ex);
			}
		}

		/// <summary>
		/// Gets or sets the currently selected combat routine.
		/// HB 4.3.4: RoutineManager.Current setter allows CCs to assign themselves directly.
		/// </summary>
		public static CombatRoutine Current
		{
			get
			{
				if (_current == null)
				{
					_current = new DefaultCombatRoutine();
					Logging.Write("No Combat Routine loaded. Using default.");
				}
				return _current;
			}
			set
			{
				_current = value;
			}
		}

		/// <summary>
		/// Gets all loaded routines.
		/// </summary>
		public static IReadOnlyList<CombatRoutine> Routines => _routines;

		/// <summary>
		/// Sets the current routine by name. Returns true if found.
		/// FEAT-37: Added return value and LegacySpellManager refresh.
		/// </summary>
		public static bool SetCurrent(string routineName)
		{
			foreach (var routine in _routines)
			{
				if (string.Equals(routine.Name, routineName, StringComparison.OrdinalIgnoreCase))
				{
					_current = routine;
					routine.Initialize();
					// Refresh spell manager when routine changes (HB 3.3.5a + 4.3.4 pattern)
					try { LegacySpellManager.Refresh(); } catch { }
					Logging.Write("Combat Routine changed to: {0}", routineName);
					return true;
				}
			}
			return false;
		}

		private sealed class DefaultCombatRoutine : CombatRoutine
		{
			public override string Name => "Default";
			public override WoWClass Class => StyxWoW.Me?.Class ?? WoWClass.None;
			public override void Initialize() { }
		}
	}
}
