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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

using BuddyControlPanel.Resources.Localization;

using Button = System.Windows.Controls.Button;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Image = System.Windows.Controls.Image;
using VerticalAlignment = System.Windows.VerticalAlignment;

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
#endregion


namespace BuddyControlPanel
{
    public class Dialog_LoadProfileFromWeb : Assets.ThemedWindow
    {
        #region Construcion and Destruction
        public Dialog_LoadProfileFromWeb()
        {
            Height = 200;
            ResizeMode = ResizeMode.NoResize;
            SizeToContent = SizeToContent.WidthAndHeight;
            Title = BCPGlobalization.Title_LoadAProfileFromTheWeb;
            Topmost = true;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Width = 300;

            var grid = new Grid()
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition() {Width = new GridLength(1.0, GridUnitType.Star)},
                    new ColumnDefinition() {Width = GridLength.Auto},
                },
                RowDefinitions =
                {
                    new RowDefinition() {Height = GridLength.Auto},
                    new RowDefinition() {Height = GridLength.Auto},
                },
            };

            var buddyBotLogo = new Image()
            {
                Height = 100,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(20, 10, 0, 5),
                Opacity = 0.35,
                Source = Utility.ToImageSource("logo-honorbuddy.png", 128),
                Stretch = Stretch.Uniform,
                VerticalAlignment = VerticalAlignment.Top,
            };
            grid.Children.Add(buddyBotLogo);
            Grid.SetColumn(buddyBotLogo, 0);
            Grid.SetColumnSpan(buddyBotLogo, 2);
            Grid.SetRow(buddyBotLogo, 0);
            Grid.SetRowSpan(buddyBotLogo, 2);
            Grid.SetZIndex(buddyBotLogo, 1);


            var textBlock_UriLabel = new TextBlock()
            {
                Effect = new DropShadowEffect(),
                Foreground = Assets.BrushText,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10, 10, 5, 0),
                Text = BCPGlobalization.Item_Uri_Label,     // "URI:"
                VerticalAlignment = VerticalAlignment.Center,
            };
            grid.Children.Add(textBlock_UriLabel);
            Grid.SetColumn(textBlock_UriLabel, 0);
            Grid.SetRow(textBlock_UriLabel, 0);
            Grid.SetZIndex(textBlock_UriLabel, 100);

            var textBox_Uri = new TextBox()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 10, 10, 0),
                VerticalAlignment = VerticalAlignment.Center,
                MinWidth = 200,
            };
            grid.Children.Add(textBox_Uri);
            Grid.SetColumn(textBox_Uri, 1);
            Grid.SetRow(textBox_Uri, 0);
            textBox_Uri.KeyDown += (sender, evt) =>
            {
                if ((evt.Key == Key.Enter) || (evt.Key == Key.Return))
                {
                    Input = textBox_Uri.Text;
                    DialogResult = true;
                    Close();
                }
            };

            var stackPanel_DecisionButtons = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5, 10, 5, 5),
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Bottom,
            };
            grid.Children.Add(stackPanel_DecisionButtons);
            Grid.SetColumn(stackPanel_DecisionButtons, 0);
            Grid.SetColumnSpan(stackPanel_DecisionButtons, 2);
            Grid.SetRow(stackPanel_DecisionButtons, 1);


            var buttonLoad = new Button()
            {
                Content = BCPGlobalization.ButtonText_Load,     // "Load"
                HorizontalAlignment = HorizontalAlignment.Right,
                IsDefault = true,
                Margin = new Thickness(5, 0, 5, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 75,
            };
            stackPanel_DecisionButtons.Children.Add(buttonLoad);

            buttonLoad.Click += (sender, evt) =>
            {
                Input = textBox_Uri.Text;
                DialogResult = true;
                Close();
            };


            var buttonCancel = new Button()
            {
                Content = BCPGlobalization.ButtonText_Cancel,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5, 0, 5, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 75,
            };
            stackPanel_DecisionButtons.Children.Add(buttonCancel);

            buttonCancel.Click += (sender, evt) =>
            {
                Input = string.Empty;
                Close();
            };

            Content = grid;
            FocusManager.SetFocusedElement(this, textBox_Uri);
        }

        public string Input { get; private set; }
        #endregion
    }
}
