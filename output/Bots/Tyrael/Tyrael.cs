using CommonBehaviors.Actions;
using Styx;
using Styx.CommonBot;
using Styx.Logic.Profiles;
using Styx.Logic.Combat;
using Styx.Helpers;
using Styx.Logic.BehaviorTree;
using TreeSharp;
using Styx.WoWInternals.WoWObjects;
using System;
using System.Windows.Forms;
using Tyrael.Shared;

namespace Tyrael
{
    public class Tyrael : BotBase
    {
		public static bool IsPaused;
		
        internal static readonly Version Revision = new Version(5, 6, 4);
        
        private static Composite _root;
        private static PulseFlags _pulseFlags;

        /// <summary>
        /// Returns Me as StyxWoW.Me.
        /// </summary>
        internal static LocalPlayer Me
        {
            get { return StyxWoW.Me; }
        }

        #region Overrides
        /// <summary>
        /// Returns the Name of the BotBase.
        /// </summary>
        public override string Name
        {
            get { return "Tyrael"; }
        }

        /// <summary>
        /// Returns the requires PulseFlags for the BotBase.
        /// </summary>
        public override PulseFlags PulseFlags
        {
            get { return !TyraelUtilities.IsTyraelPaused ? _pulseFlags : PulseFlags.Objects | PulseFlags.Lua | PulseFlags.InfoPanel; }
        }

        /// <summary>
        /// Returns the GUI of the BotBase on button press.
        /// </summary>
        public override Form ConfigurationForm
        {
            get { return new TyraelInterface(); }
        }
		
        /// <summary>
        /// Forcefully disable the profile requirements.
        /// </summary>
		public override bool RequiresProfile
        {
            get { return false; }
        }

        /// <summary>
        /// Creating the Root Composite - The magic happens within this composite.
        /// </summary>
        public override Composite Root
        {
            get { return _root ?? (_root = CreateRoot()); }
        }

        /// <summary>
        /// Runs when the BotBase is started - Initializes several basic functions.
        /// </summary>
        public override void Start()
        {
            try
            {
                InitializeComponents();
                InitializePlugins();

                if (!StyxSettings.Instance.UseFrameLock && !TyraelSettings.Instance.UseSoftLock)
                {
                    TyraelUtilities.PrintLog("------------------------------------------");
                    TyraelUtilities.PrintLog("HardLock and SoftLock are both disabled - For optimal DPS/HPS I suggest enabling ONE of them.");
                }

                TyraelUtilities.PrintLog("------------------------------------------");
                TyraelUtilities.PrintLog("{0} {1} has been started.", Name, Revision);
                TyraelUtilities.PrintLog("{0} is loaded.\r\n", RoutineManager.Current.Name);
                TyraelUtilities.PrintLog("Special thanks to the following persons:");
                TyraelUtilities.PrintLog("PureRotation Team");
                TyraelUtilities.PrintLog("-------------------------------------------\r\n");
            }
            catch (Exception startexception)
            {
                TyraelUtilities.DiagnosticLog("Start() Error: {0}", startexception);
            }
        }

        /// <summary>
        /// Runs when the BotBase is stopped - Stops several basic functions.
        /// </summary>
        public override void Stop()
        {
            try
            {
                TyraelUtilities.PrintLog("------------------------------------------");
                TyraelUtilities.PrintLog("Shutdown - Performing required actions.");
                StopComponents();
                TyraelUtilities.PrintLog("-------------------------------------------\r\n");
            }
            catch (Exception stopexception)
            {
                TyraelUtilities.DiagnosticLog("Stop() Error: {0}", stopexception);
            }
        }
        #endregion

        #region Privates & Internals
        /// <summary>
        /// SanityCheck - Checks if we are actually ingame and able to control the character.
        /// </summary>
        private static bool SanityCheckCombat()
        {
            try
            {
                return TyraelUtilities.IsViable(Me) && (StyxWoW.Me.Combat || TyraelSettings.Instance.ContinuesHealingMode) && !Me.IsDead;
            }
            catch (Exception sanitycheckexception)
            {
                TyraelUtilities.DiagnosticLog("SanityCheckCombat() Error: {0}", sanitycheckexception); return false;
            }
        }

        /// <summary>
        /// Actual Root Composite - Within this the RoutineManager runs the routines behaviors.
        /// </summary>
        private static Composite CreateRoot()
        {
            return new PrioritySelector(
                new Decorator(ret => IsPaused, new ActionAlwaysSucceed()),
                new Decorator(ret => SanityCheckCombat(),
                    SelectLockMethod(
                        RoutineManager.Current.HealBehavior,
                        RoutineManager.Current.CombatBuffBehavior ?? new ActionAlwaysFail(),
                        RoutineManager.Current.CombatBehavior)),
                    RoutineManager.Current.PreCombatBuffBehavior,
                    RoutineManager.Current.RestBehavior);
        }

        /// <summary>
        /// Initializes all the required components for Tyrael to run.
        /// </summary>
        private static void InitializeComponents()
        {
            try
            {
                TyraelSettings.Instance.Load();
                TyraelUtilities.RegisterHotkeys();
                TyraelUtilities.PrintInformation();
                TyraelUtilities.ClickToMove(1000);

                ProfileManager.LoadEmpty();

                StyxSettings.Instance.LogoutForInactivity = false;
                TreeRoot.TicksPerSecond = CharacterSettings.Instance.TicksPerSecond;
            }
            catch (Exception initializecomponentsexception)
            {
                TyraelUtilities.DiagnosticLog("InitializeComponents() Error: {0}", initializecomponentsexception);
            }
        }

        /// <summary>
        /// Stops components for Tyrael.
        /// </summary>
        private static void StopComponents()
        {
            try
            {
                TyraelUtilities.RemoveHotkeys();
                StyxSettings.Instance.LogoutForInactivity = true;
                TreeRoot.TicksPerSecond = CharacterSettings.Instance.TicksPerSecond;
            }
            catch (Exception stopcomponentsexception)
            {
                TyraelUtilities.DiagnosticLog("StopComponents() Error: {0}", stopcomponentsexception);
            }
        }

        /// <summary>
        /// Initializes the proper plugins based on the Setting CheckPluginPulsing.
        /// </summary>
        internal static void InitializePlugins()
        {
            try
            {
                if (TyraelSettings.Instance.PluginPulsing)
                {
                    _pulseFlags = PulseFlags.InfoPanel | PulseFlags.Lua | PulseFlags.Objects | PulseFlags.Plugins;
                }
                else
                {
                    _pulseFlags = PulseFlags.InfoPanel | PulseFlags.Lua | PulseFlags.Objects;
                }
            }
            catch (Exception initializepluginsexception)
            {
                TyraelUtilities.DiagnosticLog("InitializePlugins() Error: {0}", initializepluginsexception);
            }
        }
        #endregion

        #region Softlock
        /// <summary>
        /// Used for Locking only the BotBase and Routine - Not HonorBuddy itself (Softlock). Only kicks in when HardLock isn't enabled.
        /// </summary>
        private static Composite SelectLockMethod(params Composite[] children)
        {
            return TyraelSettings.Instance.UseSoftLock && !StyxSettings.Instance.UseFrameLock ? new FrameLockSelector(children) : new PrioritySelector(children);
        }

        private class FrameLockSelector : PrioritySelector
        {
            public FrameLockSelector(params Composite[] children)
                : base(children)
            {
            }

            public override RunStatus Tick(object context)
            {
                using (StyxWoW.Memory.AcquireFrame())
                {
                    return base.Tick(context);
                }
            }
        }
        #endregion
    }
}
