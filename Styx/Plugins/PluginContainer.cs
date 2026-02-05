using System;
using System.ComponentModel;
using Styx.Plugins.PluginClass;

namespace Styx.Plugins
{
	public class PluginContainer : INotifyPropertyChanged
	{
		private bool _enabled;

		public HBPlugin Plugin { get; private set; }

		public bool Enabled
		{
			get => _enabled;
			set
			{
				if (_enabled != value)
				{
					_enabled = value;
					OnPropertyChanged(nameof(Enabled));
					
					if (_enabled)
					{
						try
						{
							Plugin.Initialize();
						}
						catch (Exception ex)
						{
							Helpers.Logging.WriteException(ex);
						}
					}
					else
					{
						try
						{
							Plugin.Dispose();
						}
						catch (Exception ex)
						{
							Helpers.Logging.WriteException(ex);
						}
					}
					
					// Update enabled plugins list (no immediate save - HB 4.3.4 pattern)
					PluginManager.UpdateEnabledPlugins();
				}
			}
		}

		public string Name => Plugin.Name;
		public string Author => Plugin.Author;
		public Version Version => Plugin.Version;
		public bool WantButton => Plugin.WantButton;
		public string ButtonText => Plugin.ButtonText;

		public PluginContainer(HBPlugin plugin, bool enabled)
		{
			Plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
			_enabled = false; // Don't trigger initialization yet
			Enabled = enabled; // Now trigger if needed
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
