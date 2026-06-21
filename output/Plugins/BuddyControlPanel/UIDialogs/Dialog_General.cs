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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

using BuddyControlPanel.Resources.Localization;
using Button = System.Windows.Controls.Button;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using Image = System.Windows.Controls.Image;
using VerticalAlignment = System.Windows.VerticalAlignment;

// ReSharper disable InconsistentNaming
#endregion


namespace BuddyControlPanel
{
    // Similar to the standard dialogs, but differs in the following ways:
    // * Themed
    // * Due to the distinct ShowDialog() call usage, you can adjust parameters such as start location, and size as needed.
    internal class Dialog_General : Assets.ThemedWindow
    {
        public Dialog_General(System.Drawing.Icon icon, string title, string message, MessageBoxButton messageBoxButton)
        {
            Height = 200;
            Width = 350;
            MaxHeight = 200;
            MaxWidth = 350;
            ResizeMode = ResizeMode.NoResize;
            SizeToContent = SizeToContent.WidthAndHeight;
            Title = string.IsNullOrEmpty(title) ? "" : title;
            Topmost = true;

            var grid = new Grid()
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition() {Width = GridLength.Auto},
					//new ColumnDefinition() {Width = GridLength.Auto},
					new ColumnDefinition() {Width = new GridLength(1.0, GridUnitType.Star)},
                },
                RowDefinitions =
                {
					//new RowDefinition() {Height = GridLength.Auto},
					new RowDefinition() {Height = new GridLength(1.0, GridUnitType.Star)},
                    new RowDefinition() {Height = GridLength.Auto},
                },
            };
            Content = grid;

            var buddyBotLogo = new Image()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(25, 15, 0, 5),
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

            var alertTypeImage = new Image()
            {
                Effect = new DropShadowEffect(),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(40, 40, 30, 50),
                Source = icon.ToImageSource(),
                Stretch = Stretch.None,
                VerticalAlignment = VerticalAlignment.Top,
            };
            grid.Children.Add(alertTypeImage);
            Grid.SetColumn(alertTypeImage, 0);
            Grid.SetRow(alertTypeImage, 0);
            Grid.SetZIndex(alertTypeImage, 100);

            var textBlock = new TextBlock()
            {
                Effect = new DropShadowEffect(),
                Foreground = Assets.BrushText,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 5, 20, 5),
                MaxHeight = 250,
                MaxWidth = 300,
                Text = string.IsNullOrEmpty(message) ? "" : message,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Center,
            };
            grid.Children.Add(textBlock);
            Grid.SetColumn(textBlock, 1);
            Grid.SetRow(textBlock, 0);
            Grid.SetZIndex(textBlock, 100);

            var decisionButtons = Build_DecisionButtons(messageBoxButton);
            grid.Children.Add(decisionButtons);
            Grid.SetColumn(decisionButtons, 0);
            Grid.SetColumnSpan(decisionButtons, 2);
            Grid.SetRow(decisionButtons, 1);
        }


        private Panel Build_DecisionButtons(MessageBoxButton messageBoxButton)
        {
            var stackPanel = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(5, 5, 5, 10),
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Bottom,
            };

            var button_OK = new Button()
            {
                Background = Assets.BrushTransparent,
                Content = ((messageBoxButton == MessageBoxButton.OK) || (messageBoxButton == MessageBoxButton.OKCancel))
                            ? BCPGlobalization.ButtonText_OK    // "OK"
                            : BCPGlobalization.ButtonText_Yes,  // "Yes"
                HorizontalAlignment = HorizontalAlignment.Right,
                IsCancel = false,
                IsDefault = (messageBoxButton == MessageBoxButton.OK),
                Margin = new Thickness(5, 0, 5, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 75,
            };
            stackPanel.Children.Add(button_OK);

            button_OK.Click += (sender, evt) =>
            {
                DialogResult = true;
            };

            if (messageBoxButton == MessageBoxButton.YesNo)
            {
                var button_No = new Button()
                {
                    Background = Assets.BrushTransparent,
                    Content = BCPGlobalization.ButtonText_No,       // "No"
                    HorizontalAlignment = HorizontalAlignment.Right,
                    IsCancel = true,
                    IsDefault = (messageBoxButton == MessageBoxButton.YesNo),
                    Margin = new Thickness(5, 0, 5, 0),
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = 75,
                };
                stackPanel.Children.Add(button_No);

                button_No.Click += (sender, evt) =>
                {
                    DialogResult = false;
                };
            }

            if ((messageBoxButton == MessageBoxButton.OKCancel) || (messageBoxButton == MessageBoxButton.YesNoCancel))
            {
                var button_Cancel = new Button()
                {
                    Background = Assets.BrushTransparent,
                    Content = BCPGlobalization.ButtonText_Cancel,   // "Cancel"
                    HorizontalAlignment = HorizontalAlignment.Right,
                    IsCancel = true,
                    IsDefault = (messageBoxButton == MessageBoxButton.OKCancel) || (messageBoxButton == MessageBoxButton.YesNoCancel),
                    Margin = new Thickness(5, 0, 5, 0),
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = 75,
                };
                stackPanel.Children.Add(button_Cancel);

                button_Cancel.Click += (sender, evt) =>
                {
                    // empty
                };
            }

            return stackPanel;
        }
    }
}
