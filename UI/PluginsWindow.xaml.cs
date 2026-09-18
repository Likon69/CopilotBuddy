using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using Styx.Helpers;
using Styx.Plugins;
using Styx.Localization;

namespace CopilotBuddy.UI
{
	public partial class PluginsWindow : MetroWindow
	{
		public PluginsWindow()
		{
			InitializeComponent();
			UiLocalization.LocalizeWpf(this);
			LoadPlugins();
		}

		private void LoadPlugins()
		{
			lstPlugins.ItemsSource = PluginManager.Plugins;
			
			if (PluginManager.Plugins.Count == 0)
			{
				Logging.Write("No plugins found. Place plugins in the Plugins directory.");
			}
		}

		private void LstPlugins_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (lstPlugins.SelectedItem is PluginContainer container)
			{
				lblAuthor.Content = container.Author;
				lblVersion.Content = container.Version.ToString();
				btnSettings.IsEnabled = container.WantButton;
			}
			else
			{
				lblAuthor.Content = "";
				lblVersion.Content = "";
				btnSettings.IsEnabled = false;
			}
		}

		private void Plugin_CheckedChanged(object sender, RoutedEventArgs e)
		{
			// The TwoWay binding already handles the state change
			// This method can be used for additional actions
			if (sender is System.Windows.Controls.CheckBox checkBox && checkBox.DataContext is PluginContainer container)
			{
				if (container.Enabled)
				{
					Logging.Write($"Plugin '{container.Name}' enabled.");
				}
				else
				{
					Logging.Write($"Plugin '{container.Name}' disabled.");
				}
			}
		}

		private void BtnSettings_Click(object sender, RoutedEventArgs e)
		{
			if (lstPlugins.SelectedItem is PluginContainer container && container.WantButton)
			{
				try
				{
					UiLocalization.RunWithWinFormsLocalization(container.Plugin.OnButtonPress);
				}
				catch (Exception ex)
				{
					Logging.WriteException(ex);
					System.Windows.MessageBox.Show(
						$"Error opening plugin settings:\n{ex.Message}", 
						"Plugin Error", 
						MessageBoxButton.OK, 
						MessageBoxImage.Error);
				}
			}
		}

		private void BtnRefresh_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// Save currently enabled plugins
				var enabledPlugins = PluginManager.Plugins
					.Where(p => p.Enabled)
					.Select(p => p.Name)
					.ToArray();

				// Refresh plugins
				PluginManager.RefreshPlugins(enabledPlugins);

				// Reload UI
				LoadPlugins();

				Logging.Write("Plugins refreshed successfully.");
			}
			catch (Exception ex)
			{
				Logging.WriteException(ex);
				System.Windows.MessageBox.Show(
					$"Error refreshing plugins:\n{ex.Message}", 
					"Plugin Error", 
					MessageBoxButton.OK, 
					MessageBoxImage.Error);
			}
		}

		private void BtnClose_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Saves enabled plugins to CharacterSettings when window closes.
		/// Pattern from HB 4.3.4 PluginsWindow.OnClosing()
		/// </summary>
		protected override void OnClosing(CancelEventArgs e)
		{
			try
			{
				// Save enabled plugins list - exactly like HB 4.3.4
				CharacterSettings.Instance.EnabledPlugins = PluginManager.Plugins
					.Where(p => p.Enabled)
					.Select(p => p.Name)
					.ToArray();
				CharacterSettings.Instance.Save();
				Logging.Write("Plugin settings saved.");
			}
			catch (Exception ex)
			{
				Logging.WriteException(ex);
			}

			base.OnClosing(e);
		}
	}
}
