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
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Serialization;

// ReSharper disable InconsistentNaming
#endregion

namespace BuddyControlPanel
{
    [XmlRoot("BuddyControlPanelSettings", Namespace = null)]
    public class BuddyControlPanelSettings : XmlSettings<BuddyControlPanelSettings>, IDisposable
    {
        #region Creation & Destruction
        private static string s_dataFileName = Path.Combine(CharacterSettingsDirectory, Assets.PluginInternalName + ".xml");

        public BuddyControlPanelSettings()
            : base(s_dataFileName)
        {
            //SetDefaultValues();
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

            // Reclaim unmanaged resources --
            s_instance = null;
        }

        [XmlIgnore]
        private static BuddyControlPanelSettings s_instance;

        [XmlIgnore]
        public static BuddyControlPanelSettings Instance
        {
            get { return s_instance ?? (s_instance = Load(s_dataFileName)); }
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


        #region Overrides of XmlSettings<BuddyControlPanelSettings>
        public override void SetDefaultValues()
        {
            OverlayControlBarConfiguration = Overlay_ControlBar.Configuration_GetDefaults();
            KeybindConfiguration = KeybindManager.Configuration_GetDefaults();
            OverlayStatusDisplayConfiguration = Overlay_StatusDisplay.Configuration_GetDefaults();
        }
        #endregion


        // Actual settings...
        public ControlBarSettings OverlayControlBarConfiguration { get; set; }
        public KeybindSettings KeybindConfiguration { get; set; }
        public StatusDisplaySettings OverlayStatusDisplayConfiguration { get; set; }


        #region Types
        public class ControlBarSettings
        {
            [XmlElement]
            public Overlay_ControlBar.ControlSizeEnum ControlSize { get; set; }

            // N.B.: ControlHeight and ControlWidth are missing from this list.  These dimensions
            // are determined when the user selects the icon size for the control.

            [XmlElement]
            public double ControlLeft { get; set; }

            [XmlElement]
            public double ControlTop { get; set; }

            [XmlElement]
            public bool DragLocked { get; set; }
        };


        public class KeybindSettings
        {
            [XmlElement]
            public WpfKeyWithModifiers KeyPauseResume { get; set; }

            [XmlElement]
            public WpfKeyWithModifiers KeyStartStop { get; set; }
        }


        public class StatusDisplaySettings
        {
            // N.B.: ControlHeight is missing from this list.  The height is determined when the
            // user selects the font size for the control.

            [XmlElement]
            public double ControlLeft { get; set; }

            [XmlElement]
            public double ControlTop { get; set; }

            [XmlElement]
            public double ControlWidth { get; set; }

            [XmlElement]
            public bool DragLocked { get; set; }

            [XmlElement]
            public Overlay_StatusDisplay.FontSizeChoiceEnum FontSizeChoice { get; set; }
        }


        public struct WpfKeyWithModifiers
        {
            [XmlElement]
            public Key Key { get; set; }

            [XmlIgnore]
            public Keys FormsKey
            {
                get
                {
                    var formsKey = (Keys)KeyInterop.VirtualKeyFromKey(Key);
                    var wpfKey = Key;   // Needed for lamba closure
                    Contract.Provides((wpfKey == Key.None) || (formsKey != Keys.None),
                        () => string.Format("Wpf Key({0}) does not map to a Forms key ({1})", wpfKey, formsKey));
                    return formsKey;
                }
                set
                {
                    var formsKey = value;
                    var wpfKey = KeyInterop.KeyFromVirtualKey((int)value);
                    Contract.Provides((formsKey == Keys.None) || (wpfKey != Key.None),
                        () => string.Format("Forms Key({0}) does not map to a Wpf key ({1})", formsKey, wpfKey));
                    Key = wpfKey;
                }
            }

            [XmlIgnore]
            public bool IsModifiedShiftOnly { get { return (Modifiers & ~ModifierKeys.Shift) == 0; } }

            [XmlIgnore]
            public bool IsModifiedNone { get { return Modifiers == 0; } }

            [XmlElement]
            public ModifierKeys Modifiers { get; set; }

            [XmlIgnore]
            public bool ModifierAlt
            {
                get { return Modifiers.HasFlag(ModifierKeys.Alt); }
                set
                {
                    if (value)
                        Modifiers |= ModifierKeys.Alt;
                    else
                        Modifiers &= ~ModifierKeys.Alt;
                }
            }

            [XmlIgnore]
            public bool ModifierControl
            {
                get { return Modifiers.HasFlag(ModifierKeys.Control); }
                set
                {
                    if (value)
                        Modifiers |= ModifierKeys.Control;
                    else
                        Modifiers &= ~ModifierKeys.Control;
                }
            }

            [XmlIgnore]
            public bool ModifierShift
            {
                get { return Modifiers.HasFlag(ModifierKeys.Shift); }
                set
                {
                    if (value)
                        Modifiers |= ModifierKeys.Shift;
                    else
                        Modifiers &= ~ModifierKeys.Shift;
                }
            }

            [XmlIgnore]
            public bool ModifierWindows
            {
                get { return Modifiers.HasFlag(ModifierKeys.Windows); }
                set
                {
                    if (value)
                        Modifiers |= ModifierKeys.Windows;
                    else
                        Modifiers &= ~ModifierKeys.Windows;
                }
            }


            /// <summary>
            /// Returns true, if a Keybinding with the configuration would not yield any fundamental problems
            /// </summary>
            /// <returns></returns>
            public bool IsViableKeybind()
            {
                var isBadKey = IsModifiedNone && DisallowedKeyBinds_Unadorned.Contains(Key);
                isBadKey |= IsModifiedShiftOnly && DisallowedKeyBinds_Shifted.Contains(Key);

                return !isBadKey;
            }

            public override string ToString()
            {
                string keyDescription = string.Empty;

                if (Key != Key.None)
                {
                    if (ModifierControl)
                        keyDescription += "Ctrl+";

                    if (ModifierShift)
                        keyDescription += "Shift+";

                    if (ModifierAlt)
                        keyDescription += "Alt+";
                }

                keyDescription += Key.ToString();
                return keyDescription;
            }


            public static readonly Key[] DisallowedKeyBinds_Unadorned =
            {
                Key.A, Key.B, Key.C, Key.D, Key.E, Key.F, Key.G, Key.H, Key.I, Key.J, Key.K, Key.L, Key.M,
                Key.N, Key.O, Key.P, Key.Q, Key.R, Key.S, Key.T, Key.U, Key.V, Key.W, Key.X, Key.Y, Key.Z,
                Key.Back, Key.Delete, Key.Escape, Key.Return, Key.Space, Key.Tab,
            };

            public static readonly Key[] DisallowedKeyBinds_Shifted =
            {
                Key.A, Key.B, Key.C, Key.D, Key.E, Key.F, Key.G, Key.H, Key.I, Key.J, Key.K, Key.L, Key.M,
                Key.N, Key.O, Key.P, Key.Q, Key.R, Key.S, Key.T, Key.U, Key.V, Key.W, Key.X, Key.Y, Key.Z,
            };
        };

        #endregion
    }
}
