using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Styx.Helpers;
using Binding = System.Windows.Data.Binding;
using CheckBox = System.Windows.Controls.CheckBox;
using ComboBox = System.Windows.Controls.ComboBox;
using GroupBox = System.Windows.Controls.GroupBox;
using TextBox = System.Windows.Controls.TextBox;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using VerticalAlignment = System.Windows.VerticalAlignment;
using Application = System.Windows.Application;

namespace CopilotBuddy.UI
{
    public partial class RoutineSettingsWindow : Window
    {
        private static readonly System.Windows.Media.Brush SectionForeground =
            new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xE0, 0xE0, 0xE0));
        private static readonly System.Windows.Media.Brush HintForeground =
            new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x9A, 0x9A, 0x9A));

        private readonly List<object> _pages = new List<object>();
        private readonly List<KeyValuePair<PropertyInfo, KeyValuePair<object, object>>> _originalValues =
            new List<KeyValuePair<PropertyInfo, KeyValuePair<object, object>>>();

        public RoutineSettingsWindow(string title, string brand, string version)
        {
            InitializeComponent();
            Title = title;
            txtBrand.Text = brand;
            txtVersion.Text = version;
            SectionForeground.Freeze();
            HintForeground.Freeze();

            if (Application.Current != null && !ReferenceEquals(Application.Current.MainWindow, this))
                Owner = Application.Current.MainWindow;
        }

        public void AddPage(string header, object settings)
        {
            if (settings == null)
                return;

            _pages.Add(settings);
            Snapshot(settings);
            tabs.Items.Add(new TabItem
            {
                Header = header,
                Content = new ScrollViewer
                {
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                    Padding = new Thickness(2),
                    Content = BuildPage(settings)
                }
            });

            if (tabs.SelectedIndex < 0)
                tabs.SelectedIndex = 0;
        }

        public void AddCustomPage(string header, UIElement content)
        {
            tabs.Items.Add(new TabItem { Header = header, Content = content });
            if (tabs.SelectedIndex < 0)
                tabs.SelectedIndex = 0;
        }

        private static IEnumerable<PropertyInfo> EditableProperties(object settings)
        {
            return settings.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0)
                .Where(p => p.GetCustomAttribute<SettingAttribute>() != null);
        }

        private void Snapshot(object settings)
        {
            foreach (var prop in EditableProperties(settings))
            {
                try
                {
                    _originalValues.Add(new KeyValuePair<PropertyInfo, KeyValuePair<object, object>>(
                        prop, new KeyValuePair<object, object>(settings, prop.GetValue(settings))));
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                }
            }
        }

        private UIElement BuildPage(object settings)
        {
            var root = new StackPanel { Margin = new Thickness(4, 8, 4, 4) };

            var groups = EditableProperties(settings)
                .GroupBy(p => p.GetCustomAttribute<CategoryAttribute>()?.Category ?? "General")
                .OrderBy(g => string.Equals(g.Key, "General", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .ThenBy(g => g.Key, StringComparer.OrdinalIgnoreCase);

            foreach (var group in groups)
            {
                var rows = new Grid();
                rows.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(240) });
                rows.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                int row = 0;
                foreach (var prop in group.OrderBy(p => Label(p), StringComparer.OrdinalIgnoreCase))
                {
                    var editor = BuildEditor(prop, settings);
                    if (editor == null)
                        continue;

                    rows.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                    var label = new TextBlock
                    {
                        Text = Label(prop),
                        Foreground = SectionForeground,
                        Margin = new Thickness(0, 6, 8, 6),
                        VerticalAlignment = VerticalAlignment.Center,
                        TextWrapping = TextWrapping.Wrap
                    };

                    var description = prop.GetCustomAttribute<DescriptionAttribute>()?.Description;
                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        label.ToolTip = description;
                        editor.ToolTip = description;
                    }

                    Grid.SetRow(label, row);
                    Grid.SetColumn(label, 0);
                    Grid.SetRow(editor, row);
                    Grid.SetColumn(editor, 1);
                    rows.Children.Add(label);
                    rows.Children.Add(editor);
                    row++;
                }

                if (row == 0)
                    continue;

                root.Children.Add(new GroupBox { Header = group.Key, Content = rows });
            }

            if (root.Children.Count == 0)
            {
                root.Children.Add(new TextBlock
                {
                    Text = "This section has no configurable settings.",
                    Foreground = HintForeground,
                    Margin = new Thickness(8)
                });
            }

            return root;
        }

        private static string Label(PropertyInfo prop)
        {
            var display = prop.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
            return string.IsNullOrWhiteSpace(display) ? Humanize(prop.Name) : display;
        }

        private static string Humanize(string name)
        {
            var text = new System.Text.StringBuilder(name.Length + 8);
            for (int i = 0; i < name.Length; i++)
            {
                if (i > 0 && char.IsUpper(name[i]) && !char.IsUpper(name[i - 1]))
                    text.Append(' ');
                text.Append(name[i]);
            }
            return text.ToString();
        }

        private FrameworkElement BuildEditor(PropertyInfo prop, object settings)
        {
            var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            var margin = new Thickness(0, 4, 0, 4);

            var binding = new Binding(prop.Name)
            {
                Source = settings,
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            if (type == typeof(bool))
            {
                var box = new CheckBox { Margin = margin, HorizontalAlignment = HorizontalAlignment.Left };
                box.SetBinding(ToggleButton_IsCheckedProperty, binding);
                return box;
            }

            if (type.IsEnum)
            {
                var combo = new ComboBox { Margin = margin, ItemsSource = Enum.GetValues(type) };
                combo.SetBinding(Selector_SelectedItemProperty, binding);
                return combo;
            }

            if (type == typeof(string))
            {
                var text = new TextBox { Margin = margin };
                text.SetBinding(TextBox.TextProperty, binding);
                return text;
            }

            if (type.IsPrimitive || type == typeof(decimal))
            {
                var number = new TextBox
                {
                    Margin = margin,
                    Width = 90,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    TextAlignment = TextAlignment.Right
                };
                number.SetBinding(TextBox.TextProperty, binding);
                return number;
            }

            return null;
        }

        private static readonly DependencyProperty ToggleButton_IsCheckedProperty =
            System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty;

        private static readonly DependencyProperty Selector_SelectedItemProperty =
            System.Windows.Controls.Primitives.Selector.SelectedItemProperty;

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            foreach (var page in _pages)
            {
                try
                {
                    (page as Settings)?.Save();
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                }
            }

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            foreach (var entry in _originalValues)
            {
                try
                {
                    entry.Key.SetValue(entry.Value.Key, entry.Value.Value);
                }
                catch (Exception ex)
                {
                    Logging.WriteException(ex);
                }
            }

            DialogResult = false;
            Close();
        }
    }
}
