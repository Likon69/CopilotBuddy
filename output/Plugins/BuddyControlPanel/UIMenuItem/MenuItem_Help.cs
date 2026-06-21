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
using System.Windows;
using System.Windows.Media;

using BuddyControlPanel.Resources.Localization;

// ReSharper disable InconsistentNaming
#endregion


namespace BuddyControlPanel
{
    internal class MenuItem_Help : MenuItemBase
    {
        /// <summary>
        /// Creates a MenuItem of entries that can provide helpful information for the user.
        /// These entries will cause a page to be rendered in the user's Web Browser.
        /// We've broken the entries into groupings such that the information is easy for
        /// a user to locate.
        /// </summary>
        public MenuItem_Help()
            : base("Help", null)
        {
            Background = Brushes.Transparent;

            // Create the sub-menu entries...
            Items.Add(new MenuItem_HelpResource("{BOTNAME} " + BCPGlobalization.Item_Help_StartHere_Label,
                BCPGlobalization.Item_Help_StartHere_Uri));

            Items.Add(new MenuItem_HelpResource("{BOTNAME} " + BCPGlobalization.Item_Help_HelpDesk_Label,
                BCPGlobalization.Item_Help_HelpDesk_Uri));

            Items.Add(new MenuItem_HelpResource("{BOTNAME} " + BCPGlobalization.Item_Help_SupportForum_Label,
                BCPGlobalization.Item_Help_SupportForum_Uri));

            Items.Add(new Assets.ThemedSeparator());

            Items.Add(new MenuItem_HelpResource(BCPGlobalization.Item_Help_TheBuddyForums_Label,
                BCPGlobalization.Item_Help_TheBuddyForums_Uri));

            Items.Add(new MenuItem_HelpResource("{BOTNAME} " + BCPGlobalization.Item_Help_Wiki_Label,
                BCPGlobalization.Item_Help_Wiki_Uri));

            Items.Add(new MenuItem_HelpResource("{BOTNAME} " + BCPGlobalization.Item_Help_GuidesForum_Label,
                BCPGlobalization.Item_Help_GuidesForum_Uri));

            Items.Add(new Assets.ThemedSeparator());

            Items.Add(new MenuItem_HelpResource(BCPGlobalization.Item_Help_BuddyStore_Label,
                BCPGlobalization.Item_Help_BuddyStore_Uri));

            Items.Add(new MenuItem_HelpResource(BCPGlobalization.Item_Help_BuddyAuthenticationPortal_Label,
                BCPGlobalization.Item_Help_BuddyAuthenticationPortal_Uri));

            Items.Add(new Assets.ThemedSeparator());

            Items.Add(new MenuItem_HelpResource(BCPGlobalization.Item_Help_ReleaseNotes_Label,
                BCPGlobalization.Item_Help_ReleaseNotes_Uri));

            Items.Add(new MenuItem_HelpResource("{BOTNAME} " + BCPGlobalization.Item_Help_ReleaseForum_Label,
                BCPGlobalization.Item_Help_ReleaseForum_Uri));

            Items.Add(new MenuItem_HelpResource(BCPGlobalization.Item_Help_Updates_Label,
                BCPGlobalization.Item_Help_Updates_Uri));

            Items.Add(new Assets.ThemedSeparator());
            Items.Add(new MenuItem_About());
        }
    }


    internal class MenuItem_About : MenuItemBase
    {
        public MenuItem_About()
            : base("About...", null)
        {
            // empty
        }

        protected override void OnClick()
        {
            Dialog_About.Instance.ShowAtFront(Utility.GetStartLocation((MenuItemBase)this.Parent));
            base.OnClick();
        }
    }


    internal class MenuItem_HelpResource : MenuItemBase
    {
        public MenuItem_HelpResource(string name, string uri)
            : base(BuildMenuItemName(name), BuildDefaultToolTip(uri))
        {
            Contract.Requires(!string.IsNullOrEmpty(name), () => "HelpLocation Name may not be null or empty");
            Contract.Requires(!string.IsNullOrEmpty(uri), () => "HelpLocation URI may not be null or empty");

            _normalizedUri = uri
                            .Replace("{APPDIR}", AppDomain.CurrentDomain.BaseDirectory)
                            .Replace(@"\", "/")
                            .Replace("//", "/");
        }

        private static string BuildMenuItemName(string name)
        {
            return name.Replace("{BOTNAME}", BCPGlobalization.BuddyBotName);
        }

        private static string BuildDefaultToolTip(string uri)
        {
            var normalizedUri = uri
                .Replace("{APPDIR}", AppDomain.CurrentDomain.BaseDirectory)
                .Replace(@"\", "/")
                .Replace("//", "/");
            return string.Format("Navigate to \"{0}\" in your Web Browser", normalizedUri);
        }

        private readonly string _normalizedUri;

        protected override void OnClick()
        {
            System.Diagnostics.Process.Start(_normalizedUri);
            base.OnClick();
        }
    }
}