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
using System.Windows.Media;

#endregion


namespace BuddyControlPanel
{
    public class Assets
    {
        public const string PluginAuthor = "chinajade";
        public static readonly Version PluginVersion = new Version(1, 3, 3, 68);

        // Many constructs need a unique name for the plugin.  We don't want to use the localized
        // plugin name for various reasons (e.g., a file system may not support certain characters, etc).
        // Also, Honorbuddy will ship this plugin in English, and we expect to find the information by
        // its English name.
        public const string PluginInternalName = "BuddyControlPanel";

        static Assets()
        {
            // To prevent exceptions being thrown on 'unfrozen' assets like brushes, we freeze them here...
            // The brushes are highly reusable, and freezing them has the beneficial side effect of increasing
            // performance (10x-ish) also.s perf by 10x)
            Freeze(BrushConfirmation);
            Freeze(BrushOpaqueBlack);
            Freeze(BrushSemiTransparent);
            Freeze(BrushText);
            Freeze(BrushTransparent);
            Freeze(BrushWarning);
        }

        public static readonly Brush BrushConfirmation = new SolidColorBrush(Colors.DarkSeaGreen);

        public static readonly Brush BrushOpaqueBlack = new SolidColorBrush(Colors.Black);

        public static readonly Brush BrushSemiTransparent = new SolidColorBrush(Colors.Black)
        {
            Opacity = 0.2,
        };

        public static readonly Brush BrushText = new SolidColorBrush(Colors.White);

        public static readonly Brush BrushTransparent = new SolidColorBrush(Colors.Black)
        {
            Opacity = 0.0,
        };

        public static readonly Brush BrushWarning = Brushes.Goldenrod;

        public static readonly Color ColorConfirmation = Colors.DarkSeaGreen;

        public static readonly Color ColorInformation = Colors.CornflowerBlue;

        public static readonly Color ColorProblem = Colors.Firebrick;

        public static readonly Color ColorWarning = Colors.Goldenrod;

        public static readonly FontFamily OverlayFontFamily = new FontFamily("Consolas");

        public class ThemedButton : Button
        {
            public ThemedButton()
            {
                // WPF (or the Honorbuddy style) doesn't propagate styles to derived classes
                // correctly.  So, we explicit set it here.
                Style = new Style()
                {
                    BasedOn = (Style)FindResource(typeof(Button)),
                    TargetType = typeof(Button),
                };
            }
        }


        public class ThemedCheckBox : CheckBox
        {
            public ThemedCheckBox()
            {
                // WPF (or the Honorbuddy style) doesn't propagate styles to derived classes
                // correctly.  So, we explicit set it here.
                Style = new Style()
                {
                    BasedOn = (Style)FindResource(typeof(CheckBox)),
                    TargetType = typeof(CheckBox),
                };
            }
        }


        public class ThemedComboBox : ComboBox
        {
            public ThemedComboBox()
            {
                // WPF (or the Honorbuddy style) doesn't propagate styles to derived classes
                // correctly.  So, we explicit set it here.
                Style = new Style()
                {
                    BasedOn = (Style)FindResource(typeof(ComboBox)),
                    TargetType = typeof(ComboBox),
                };
            }
        }

        public class ThemedContextMenu : ContextMenu
        {
            public ThemedContextMenu()
            {
                // WPF (or the Honorbuddy style) doesn't propagate styles to derived classes
                // correctly.  So, we explicit set it here.
                Style = new Style()
                {
                    BasedOn = (Style)FindResource(typeof(ContextMenu)),
                    TargetType = typeof(ContextMenu),
                };
            }
        }

        public class ThemedGroupBox : GroupBox
        {
            public ThemedGroupBox()
            {
                // WPF (or the Honorbuddy style) doesn't propagate styles to derived classes
                // correctly.  So, we explicit set it here.
                Style = new Style()
                {
                    BasedOn = (Style)FindResource(typeof(GroupBox)),
                    TargetType = typeof(GroupBox),
                };
            }
        }

        public class ThemedMenuItem : MenuItem
        {
            public ThemedMenuItem()
            {
                // WPF (or the Honorbuddy style) doesn't propagate styles to derived classes
                // correctly.  So, we explicit set it here.
                Style = new Style()
                {
                    BasedOn = (Style)FindResource(typeof(MenuItem)),
                    TargetType = typeof(MenuItem),
                };

                BorderThickness = new Thickness(0);
                Margin = new Thickness(0, 0, 0, 0);
                Padding = new Thickness(0, 0, 0, 0);
            }
        }


        public class ThemedSeparator : Separator
        {
            public ThemedSeparator()
            {
                // WPF (or the Honorbuddy style) doesn't propagate styles to derived classes
                // correctly.  So, we explicit set it here.
                Style = new Style()
                {
                    BasedOn = (Style)FindResource(typeof(Separator)),
                    TargetType = typeof(Separator),
                };

                Margin = new Thickness(15, 3, 15, 3);
            }
        }


        public class ThemedTextBlock : TextBlock
        {
            public ThemedTextBlock()
            {
                // WPF (or the Honorbuddy style) doesn't propagate styles to derived classes
                // correctly.  So, we explicit set it here.
                Style = new Style()
                {
                    BasedOn = (Style)FindResource(typeof(TextBlock)),
                    TargetType = typeof(TextBlock),
                };
            }
        }


        public class ThemedWindow : Window
        {
            public ThemedWindow()
            {
                // WPF (or the Honorbuddy style) doesn't propagate styles to derived classes
                // correctly.  So, we explicit set it here.
                Style = new Style()
                {
                    BasedOn = (Style)FindResource(typeof(Window)),
                    TargetType = typeof(Window),
                };
            }
        }


        private static void Freeze(Freezable brush)
        {
            if (brush.CanFreeze)
                brush.Freeze();
        }
    }
}