using System;
using Styx.Helpers;
using Styx.Logic;
using Styx.Logic.Combat;
using Styx.Plugins;
using Styx.WoWInternals;

namespace Styx
{
	public static class WoWPulsator
	{
		public static void Pulse(PulseFlags flags)
		{
			try
			{
				if ((flags & PulseFlags.Objects) != (PulseFlags)0U)
				{
					ObjectManager.Update();
				}

				if ((flags & PulseFlags.Lua) != (PulseFlags)0U)
				{
					Lua.ProcessEvents();
				}

				if ((flags & PulseFlags.WoWChat) != (PulseFlags)0U)
				{
					WoWChat.Update();
				}

				if ((flags & PulseFlags.InfoPanel) != (PulseFlags)0U)
				{
					InfoPanel.Update();
				}

				if ((flags & PulseFlags.Looting) != (PulseFlags)0U)
				{
					LootTargeting.Instance.Pulse();
				}

				if ((flags & PulseFlags.Targeting) != (PulseFlags)0U)
				{
					Targeting.Instance.Pulse();
				}

				// BUG-07 fix: Pulse movement to flush timed movement entries
				WoWMovement.Pulse();

				// BUG-07 fix: Pulse avoidance zones (was missing per audit)
				Styx.Logic.Pathing.AvoidanceManager.Pulse();

				// HB 6.2.3 AvoidanceNavigationProvider pattern:
				// Update geometric obstacle avoidance zones so Navigator.MoveTo()
				// can redirect the bot around registered world obstacles (forge, mailbox, etc.).
				// Set by WorldObstacleManager.Initialize() — no-op when no bots have registered.
				Styx.Logic.Pathing.Navigator.NavAvoidanceUpdater?.Invoke();

				if ((flags & PulseFlags.BotEvents) != (PulseFlags)0U)
				{
					BotEvents.PulseEvents();
				}

				if ((flags & PulseFlags.Plugins) != (PulseFlags)0U)
				{
					PluginManager.Pulse();
				}

				if (RoutineManager.Current != null)
				{
					RoutineManager.Current.Pulse();
				}
			}
			catch (Exception ex)
			{
				Logging.WriteException(ex);
			}
		}
	}
}
