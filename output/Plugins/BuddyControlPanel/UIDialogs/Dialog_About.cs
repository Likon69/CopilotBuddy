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
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;

using BuddyControlPanel.Resources.Localization;
using Styx.CommonBot;
using Styx.Plugins;

using Button = System.Windows.Controls.Button;
using DataFormats = System.Windows.DataFormats;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using RichTextBox = System.Windows.Controls.RichTextBox;
using TabControl = System.Windows.Controls.TabControl;
using VerticalAlignment = System.Windows.VerticalAlignment;

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
#endregion


namespace BuddyControlPanel
{
    internal class Dialog_About : Assets.ThemedWindow, IDisposable
    {
        #region Construction and Destruction
        private Dialog_About()
        {
            Height = 400;
            MaxHeight = 400;
            MinHeight = 400;
            ResizeMode = ResizeMode.NoResize;
            SizeToContent = SizeToContent.WidthAndHeight;
            Title = string.Format(BCPGlobalization.TitleFormat_About, BCPGlobalization.PluginName);

            var grid_Main = new Grid()
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition() {Width = GridLength.Auto},
                    new ColumnDefinition() {Width = GridLength.Auto},
                },
                RowDefinitions =
                {
                    new RowDefinition() {Height = GridLength.Auto},
                    new RowDefinition() {Height = new GridLength(1.0, GridUnitType.Star)},
                    new RowDefinition() {Height = GridLength.Auto},
                    new RowDefinition() {Height = GridLength.Auto},
                    new RowDefinition() {Height = GridLength.Auto},
                },
            };
            Content = grid_Main;

            Populate_MainPanel(grid_Main);
            Populate_TabControl(grid_Main);

            Closing += (sender, evt) =>
            {
                Visibility = Visibility.Hidden;
                evt.Cancel = true;
            };
        }

        // Basic Dispose pattern (ref: https://msdn.microsoft.com/en-us/library/b1yfkh5e%28v=vs.110%29.aspx)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            // Reclaim unmanaged resources (hooked elements) --

            s_instance = null;
        }

        private static Dialog_About s_instance;
        public static Dialog_About Instance
        {
            get
            {
                return s_instance ?? (s_instance = new Dialog_About());
            }
        }

        /// <summary>
        /// De-initializes the singleton instance.
        /// </summary>
        public static void ToJunkyard()
        {
            if (s_instance != null)
            {
                s_instance.Dispose();
                s_instance = null;
            }
        }
        #endregion


        private void Populate_MainPanel(Grid parentGrid)
        {
            var textBlockProductDetails = new TextBlock()
            {
                Background = Assets.BrushTransparent,
                Effect = new DropShadowEffect(),
                Foreground = Assets.BrushText,
                Margin = new Thickness(10, 30, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                Text = string.Format("{1}{0}v{2}", Environment.NewLine, BCPGlobalization.PluginName, Assets.PluginVersion),
            };
            parentGrid.Children.Add(textBlockProductDetails);
            Grid.SetColumn(textBlockProductDetails, 0);
            Grid.SetRow(textBlockProductDetails, 0);
            Grid.SetZIndex(textBlockProductDetails, 100);

            var imageBotLogo = new Image()
            {
                Height = 200,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(23, 10, 0, 0),
                Opacity = 0.70,
                Source = Utility.ToImageSource("logo-honorbuddy.png", 128),
                Stretch = Stretch.Uniform,
                VerticalAlignment = VerticalAlignment.Top,
            };
            parentGrid.Children.Add(imageBotLogo);
            Grid.SetColumn(imageBotLogo, 0);
            Grid.SetRow(imageBotLogo, 0);
            Grid.SetRowSpan(imageBotLogo, 2);
            Grid.SetZIndex(imageBotLogo, 1);

            var textBlockProductAuthor = new TextBlock()
            {
                Background = Assets.BrushTransparent,
                Effect = new DropShadowEffect(),
                Foreground = Assets.BrushText,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(25, 10, 0, 0),
                Text = Assets.PluginAuthor,
                VerticalAlignment = VerticalAlignment.Bottom,
            };
            parentGrid.Children.Add(textBlockProductAuthor);
            Grid.SetColumn(textBlockProductAuthor, 0);
            Grid.SetRow(textBlockProductAuthor, 2);

            var authorLogoImage = new Image_RoundedCorners("TheBuddyTeam.png", 128, Brushes.Transparent)
            {
                Height = 80,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(30, 0, 0, 5),
                Opacity = 0.9,
                VerticalAlignment = VerticalAlignment.Bottom,
                Width = 80,
            };
            parentGrid.Children.Add(authorLogoImage);
            Grid.SetColumn(authorLogoImage, 0);
            Grid.SetRow(authorLogoImage, 3);

            var buttonOK = new Button()
            {
                Content = BCPGlobalization.ButtonText_OK,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5, 10, 5, 10),
                VerticalAlignment = VerticalAlignment.Bottom,
                Width = 75,
            };
            parentGrid.Children.Add(buttonOK);
            Grid.SetColumn(buttonOK, 0);
            Grid.SetColumnSpan(buttonOK, 2);
            Grid.SetRow(buttonOK, 4);

            buttonOK.Click += (sender, evt) =>
            {
                Close();
            };
        }


        private void Populate_TabControl(Grid parentGrid)
        {
            const int tabContentWidth = 500;

            var tabControl = new TabControl()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(10, 10, 5, 5),
                VerticalAlignment = VerticalAlignment.Stretch,
                MinWidth = tabContentWidth,
            };
            parentGrid.Children.Add(tabControl);
            Grid.SetColumn(tabControl, 1);
            Grid.SetRow(tabControl, 0);
            Grid.SetRowSpan(tabControl, 4);

            var tabItemChangeLog = new TabItem()
            {
                Header = BCPGlobalization.Title_Changelog,  // "Changelog"
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            tabControl.Items.Add(tabItemChangeLog);

            // N.B.: We *must* ascribe a Width to the RichTextBox.  Without a Width, the RTB will render
            // text vertically one character wide.  This is a known defect of the RTB.
            var richTextBox_Changelog = new RichTextBox()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                IsReadOnly = true,
                Margin = new Thickness(2, 2, 2, 2),
                VerticalAlignment = VerticalAlignment.Stretch,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Width = tabContentWidth,
            };
            tabItemChangeLog.Content = richTextBox_Changelog;
            LoadRtfFile(richTextBox_Changelog, "Changelog.rtf");


            var tabItemLegal = new TabItem()
            {
                Header = BCPGlobalization.Title_Legal,  // "Legal"
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            tabControl.Items.Add(tabItemLegal);

            // N.B.: We *must* ascribe a Width to the RichTextBox.  Without a Width, the RTB will render
            // text vertically one character wide.  This is a known defect of the RTB.
            var richTextBox_Legal = new RichTextBox()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                IsReadOnly = true,
                Margin = new Thickness(2, 2, 2, 2),
                VerticalAlignment = VerticalAlignment.Stretch,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Width = tabContentWidth,
            };
            tabItemLegal.Content = richTextBox_Legal;
            LoadRtfFile(richTextBox_Legal, "Legal.rtf");
        }


        private void LoadRtfFile(RichTextBox richTextBox, string fileName)
        {
            Contract.Requires(!string.IsNullOrEmpty(fileName), () => "fileName may not be null or empty");

            var fullpath = Path.Combine(PluginManager.PluginsDirectory,
                                        Assets.PluginInternalName,
                                        fileName);

            Contract.Requires(File.Exists(fullpath),
                                () => string.Format("File \"{0}\" does not exist", fullpath));

            FileStream fileStream;
            richTextBox.Document = new FlowDocument();
            var textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);

            using (fileStream = new FileStream(fullpath, FileMode.OpenOrCreate))
            {
                textRange.Load(fileStream, DataFormats.Rtf);
            }
        }
    }
}
