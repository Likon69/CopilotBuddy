// Originally contributed by Chinajade
//
// LICENSE:
// This work is licensed under the
//     Creative Commons Attribution-NonCommercial-ShareAlike 4.0 Unported License.
// also known as CC-BY-NC-SA.  To view a copy of this license, visit
//      http://creativecommons.org/licenses/by-nc-sa/4.0/
// or send a letter to
//      Creative Commons // 171 Second Street, Suite 300 // San Francisco, California, 94105, USA.
//

#region Usings

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using BuddyControlPanel.Resources.Localization;
using Styx.Helpers;
using Styx.Plugins;

// ReSharper disable InconsistentNaming
#endregion


namespace BuddyControlPanel
{
    internal class MenuItem_Plugins : MenuItemBase
    {
        public MenuItem_Plugins()
            : base(BCPGlobalization.Item_Plugins_Label,
                   string.Format(BCPGlobalization.Item_Plugins_ToolTipFormat, BCPGlobalization.BuddyBotName))
        {
            // Create the sub-menu entries...
            foreach (var pluginContainer in PluginManager.Plugins.OrderBy(p => p.Name))
                Items.Add(new MenuItem_PluginContainer(pluginContainer));
        }


        protected override void OnSubmenuOpened(RoutedEventArgs evt)
        {
            // Since other windows and entities may alter whether or not a plugin is enabled,
            // we must make certain that the current 'enable' values are reflected any time the
            // submenu is opened.
            Utility.InvokeOnSpecificDispatcher(Dispatcher, () =>
           {
               foreach (var menuItemPluginContainer in Items.OfType<MenuItem_PluginContainer>())
                   menuItemPluginContainer.SynchronizeCheckedState();
           });

            base.OnSubmenuOpened(evt);
        }
    }


    internal class MenuItem_PluginContainer : MenuItemBase
    {
        public MenuItem_PluginContainer(PluginContainer pluginContainer)
            : base(BuildMenuItemName(pluginContainer), BuildDefaultToolTip(pluginContainer))
        {
            _pluginContainer = pluginContainer;

            // All plugins are enabled except the one that controls the BuddyControlPanel itself...
            IsCheckable = true;
            IsEnabled = (_pluginContainer.Name != Assets.PluginInternalName);
            SynchronizeCheckedState();

            ToolTipService.SetShowOnDisabled(this, true);
        }

        private static string BuildMenuItemName(PluginContainer pluginContainer)
        {
            // We use an asterisk following the plugin name to indicate when a plugin has settings
            return pluginContainer.Name + (pluginContainer.WantButton ? "*" : "");
        }

        private static string BuildDefaultToolTip(PluginContainer pluginContainer)
        {
            string toolTip;
            if (pluginContainer.Name == Assets.PluginInternalName)
                toolTip = string.Format(BCPGlobalization.GeneralTextFormat_ThisPluginCanOnlyBeEnabledOrDisabledFromTheMainHonorbuddyUI,
                                        BCPGlobalization.BuddyBotName);
            else
            {
                toolTip = BCPGlobalization.GeneralText_MouseLeftButtonToEnableOrDisable;
                if (pluginContainer.WantButton)
                    toolTip += Environment.NewLine + "* " + BCPGlobalization.GeneralText_MouseRightButtonForPluginSettings;
            }

            return string.Format("{1}{0}{2}{0}{3}{0}{4}",
                Environment.NewLine,
                pluginContainer.Name,
                pluginContainer.Version,
                pluginContainer.Author,
                toolTip);
        }

        private readonly PluginContainer _pluginContainer;


        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _pluginContainer.Enabled = !_pluginContainer.Enabled;   // Toggle state
            SynchronizeCheckedState();

            // Update the settings...
            CharacterSettings.Instance.EnabledPlugins =
                (from pluginContainer in PluginManager.Plugins
                 where pluginContainer.Enabled
                 select pluginContainer.Name).ToArray();
            CharacterSettings.Instance.Save();

            // Notify user of new state...
            Utility.OverlayNotification(string.Format("{0}: {1}",
                (_pluginContainer.Enabled
                    ? BCPGlobalization.GeneralText_PluginIsNowEnabled
                    : BCPGlobalization.GeneralText_PluginIsNowDisabled),
                _pluginContainer.Name),
                Assets.ColorInformation);
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            if ((_pluginContainer.Plugin != null) && _pluginContainer.WantButton)
                _pluginContainer.Plugin.OnButtonPress();
            else
                Utility.OverlayNotification(string.Format(BCPGlobalization.GeneralText_PluginHasNoSettings + ": {0}",
                                                        _pluginContainer.Name), Assets.ColorProblem);
        }

        public void SynchronizeCheckedState()
        {
            Utility.InvokeOnSpecificDispatcher(Dispatcher, () =>
                IsChecked = _pluginContainer.Enabled);
        }
    }
}