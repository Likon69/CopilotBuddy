using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Styx;
using Styx.Helpers;
using Styx.Logic.Pathing;
using Styx.Logic.Questing;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace CopilotBuddy.UI
{
    public partial class DeveloperToolsWindow : Window
    {
        // ObservableCollections for data binding
        private readonly ObservableCollection<WGameObject> _gameObjects = new ObservableCollection<WGameObject>();
        private readonly ObservableCollection<WUnit> _units = new ObservableCollection<WUnit>();
        private readonly ObservableCollection<WItem> _items = new ObservableCollection<WItem>();
        public ObservableCollection<WoWObject> Targets = new ObservableCollection<WoWObject>();
        public ObservableCollection<WoWObject> Loots = new ObservableCollection<WoWObject>();

        private GridViewColumnHeader? _lastHeaderClicked;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;
        private System.Windows.Threading.DispatcherTimer? _refreshTimer;
        private WoWPoint? _lastBlackspotPos;

        private static readonly Regex _startIdParser = new Regex(@"(?<=Start:\s+\[(url=/npc|url=/object|item)=)\d+(?=\])");
        private static readonly Regex _startNameParser = new Regex(@"(?<=url=/(npc|object)=\d*]).+(?=\[/url\]\[/icon\].*quest_end)");
        private static readonly Regex _endIdParser = new Regex(@"(?<=End:\s+\[url=/(npc|object)=)\d+(?=\])");
        private static readonly Regex _endNameParser = new Regex(@"(?<=End:\s+\[url=/(npc|object)=\d+\]).+(?=\[/url\]\[/icon\])");
        private static readonly string _startItemNameParserFormat = @"(?<=_\[{0}\]={{name_enus:').+?(?=',)";

        public DeveloperToolsWindow()
        {
            InitializeComponent();

            // Ensure we're attached to WoW so data (quests, objects) is available even if bot isn't started
            try { EnsureAttached(); } catch { }

            // Load objects directly after updating object manager
            try
            {
                ObjectManager.Update();
                RefreshObjectLists();
            }
            catch { }

            lvGameObjectsDump.ItemsSource = _gameObjects;
            lvUnitsDump.ItemsSource = _units;
            lvItemsDump.ItemsSource = _items;
            lvTargetsList.ItemsSource = Targets;
            lvLootList.ItemsSource = Loots;

            try { RefreshPlayerPosition(); } catch { }
            try { RefreshCurrentTargetInfo(); } catch { }
            try { EnsureAttached(); RefreshQuestInfo(); } catch { }

            // Subscribe to Targeting events
            try
            {
                Styx.Logic.Targeting.Instance.OnTargetListUpdateFinished += TargetingListUpdated;
                Styx.Logic.LootTargeting.Instance.OnTargetListUpdateFinished += LootTargetUpdated;
            }
            catch { }
        }

        #region Local Player Info

        private void btnRefreshPosition_Click(object sender, RoutedEventArgs e)
        {
            RefreshPlayerPosition();
        }

        private void RefreshPlayerPosition()
        {
            try
            {
                ObjectManager.Update();
                if (StyxWoW.IsInGame && ObjectManager.Me != null)
                    tbLocalPlayerLocation.Text = ObjectManager.Me.Location.ToString();
                else
                    tbLocalPlayerLocation.Text = "N/A";
            }
            catch
            {
                tbLocalPlayerLocation.Text = "Error";
            }
        }

        private void tbLocalPlayerLocation_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Honorbuddy behavior: only handle double-click — write Hotspot XML to the log
            if (e.ClickCount != 2)
                return;

            ObjectManager.Update();
            if (ObjectManager.Me == null)
                return;

            WoWPoint location = ObjectManager.Me.Location;
            Logging.Write($"<Hotspot X=\"{FormatCoordinate(location.X)}\" Y=\"{FormatCoordinate(location.Y)}\" Z=\"{FormatCoordinate(location.Z)}\" />");
        }

        #endregion

        #region Current Target Info

        private void btnRefreshCTInfo_Click(object sender, RoutedEventArgs e)
        {
            if (!ObjectManager.IsInGame)
                return;
            RefreshCurrentTargetInfo();
        }

        private void RefreshCurrentTargetInfo()
        {
            try
            {
                ObjectManager.Update();
                if (!StyxWoW.IsInGame || ObjectManager.Me == null)
                {
                    ClearTargetInfo();
                    return;
                }

                var currentTarget = ObjectManager.Me.CurrentTarget;
                if (currentTarget == null)
                {
                    ClearTargetInfo();
                    return;
                }

                tbCTEntry.Text = currentTarget.Entry.ToString();
                tbCTFactionId.Text = currentTarget.FactionId.ToString();
                tbCTFactionName.Text = "Unknown";

                tbCTName.Text = currentTarget.Name ?? "Unknown";
                tbCTLocation.Text = $"{currentTarget.Location.X:N2}, {currentTarget.Location.Y:N2}, {currentTarget.Location.Z:N2}";

                // Group IDs from creature cache
                try
                {
                    if (currentTarget.GetCachedInfo(out var cacheEntry))
                    {
                        tbCTGroup1.Text = cacheEntry.GroupID.ToString();
                        tbCTGroup2.Text = cacheEntry.GroupID2.ToString();
                    }
                    else
                    {
                        tbCTGroup1.Text = "0";
                        tbCTGroup2.Text = "0";
                    }
                }
                catch
                {
                    tbCTGroup1.Text = "0";
                    tbCTGroup2.Text = "0";
                }

                // Vendor type
                try
                {
                    string vendorType = !currentTarget.IsFoodVendor
                        ? (!currentTarget.IsAmmoVendor ? (currentTarget.IsVendor ? "Repair" : "None") : "Ammo")
                        : "Food";
                    tbCTVendorType.Text = vendorType;
                }
                catch
                {
                    tbCTVendorType.Text = "N/A";
                }
            }
            catch { }
        }

        private void ClearTargetInfo()
        {
            tbCTEntry.Text = tbCTFactionId.Text = tbCTFactionName.Text =
            tbCTGroup1.Text = tbCTGroup2.Text = tbCTLocation.Text =
            tbCTName.Text = tbCTVendorType.Text = "N/A";
        }

        private void btnCopyCTXmlInfo_Click(object sender, RoutedEventArgs e)
        {
            if (!ObjectManager.IsInGame)
                return;
            ObjectManager.Update();
            if (ObjectManager.Me?.CurrentTarget == null)
                return;

            WoWUnit currentTarget = ObjectManager.Me.CurrentTarget;
            string displayName = currentTarget.Name ?? "Unknown";
            WoWPoint location = currentTarget.Location;
            
            // Use InvariantCulture to ensure dots as decimal separators (not French commas)
            string xStr = location.X.ToString(CultureInfo.InvariantCulture);
            string yStr = location.Y.ToString(CultureInfo.InvariantCulture);
            string zStr = location.Z.ToString(CultureInfo.InvariantCulture);
            string posStr = $"<{xStr}, {yStr}, {zStr}>";

            Logging.Write("Name = {0}\r\nWowhead Id = {1}\r\nFaction = {2} [Unknown]\r\nLocation = {3}",
                displayName, currentTarget.Entry, currentTarget.FactionId, posStr);

            string vendorType = !currentTarget.IsFoodVendor ? (!currentTarget.IsAmmoVendor ? "Repair" : "Ammo") : "Food";
            string xml = $"<Vendor Name=\"{displayName}\" Entry=\"{currentTarget.Entry}\" Type=\"{vendorType}\" X=\"{xStr}\" Y=\"{yStr}\" Z=\"{zStr}\" />";
            Logging.Write(xml);
            System.Windows.Clipboard.SetText(xml);
        }

        private void HandleTextBlockDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2)
                return;
            System.Windows.Clipboard.SetText(((TextBlock)sender).Text);
        }

        #endregion

        #region Quests

        private void btnRefreshQuests_Click(object sender, RoutedEventArgs e)
        {
            try { EnsureAttached(); } catch { }
            RefreshQuestInfo();
        }

        private void RefreshQuestInfo()
        {
            try { EnsureAttached(); } catch { }
            ObjectManager.Update();
            tvQuests.Items.Clear();
            if (!StyxWoW.IsInGame || ObjectManager.Me == null)
                return;

            try
            {
                var allQuests = ObjectManager.Me.QuestLog.GetAllQuests();

                if (allQuests == null || !allQuests.Any())
                {
                    TreeViewItem none = new TreeViewItem() { Header = "(no quests)" };
                    tvQuests.Items.Add(none);
                    return;
                }

                foreach (PlayerQuest quest in allQuests)
                {
                    var objectives = quest.GetObjectives();

                    TreeViewItem questItem = new TreeViewItem();
                    questItem.Header = $"[{quest.Id}] {quest.Name}";

                    if (objectives.Count > 0)
                    {
                        foreach (Quest.QuestObjective objective in objectives)
                        {
                            if (!objective.IsEmpty)
                            {
                                TreeViewItem objectiveItem = new TreeViewItem();
                                string text = $"[{objective.Type}] {objective.Objective ?? $"ID:{objective.ID}"}";
                                if (objective.Count > 0)
                                    text = $"{text} ({objective.Count})";
                                objectiveItem.Header = text;
                                questItem.Items.Add(objectiveItem);
                            }
                        }
                    }

                    questItem.Tag = quest;
                    tvQuests.Items.Add(questItem);
                }
            }
            catch { }
        }

        private void CopyQuestOrderXML(object sender, RoutedEventArgs e)
        {
            if (!ObjectManager.IsInGame)
                return;
            TreeViewItem? selectedQuestItem = GetSelectedQuestItem((TreeViewItem)tvQuests.SelectedItem);
            if (selectedQuestItem == null)
                return;

            PlayerQuest? tag = selectedQuestItem.Tag as PlayerQuest;
            if (tag == null)
                return;

            StringBuilder builder = new StringBuilder();
            AppendQuestInfo(tag, builder);
            System.Windows.Clipboard.SetText(builder.ToString());
        }

        private void btnCopyAllQuests_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder builder = new StringBuilder();
            foreach (object obj in tvQuests.Items)
            {
                TreeViewItem? item = obj as TreeViewItem;
                if (item?.Tag != null)
                    AppendQuestInfo(item.Tag as Quest, builder);
            }
            string xml = builder.ToString();
            if (!string.IsNullOrEmpty(xml))
            {
                try { Logging.Write(xml); } catch { }
                try { System.Windows.Clipboard.SetText(xml); } catch { }
            }
        }

        private void btnCopyAllQuestOverrides_Click(object sender, RoutedEventArgs e)
        {
            ObjectManager.Update();
            if (ObjectManager.Me == null)
                return;

            StringBuilder stringBuilder = new StringBuilder();
            foreach (PlayerQuest allQuest in ObjectManager.Me.QuestLog.GetAllQuests())
            {
                List<Quest.QuestObjective> objectives = allQuest.GetObjectives();
                stringBuilder.AppendFormat("<Quest Id=\"{0}\" Name=\"{1}\">\n", allQuest.Id, allQuest.Name);

                for (int index = 0; index < objectives.Count; ++index)
                {
                    if (!objectives[index].IsEmpty && objectives[index].Type != Quest.QuestObjectiveType.Special)
                    {
                        string objectiveTypeName, idAttributeName, countAttributeName;
                        switch (objectives[index].Type)
                        {
                            case Quest.QuestObjectiveType.CollectIntermediateItem:
                            case Quest.QuestObjectiveType.CollectItem:
                                objectiveTypeName = "CollectItem"; idAttributeName = "ItemId"; countAttributeName = "CollectCount";
                                break;
                            case Quest.QuestObjectiveType.KillMob:
                                objectiveTypeName = "KillMob"; idAttributeName = "MobId"; countAttributeName = "KillCount";
                                break;
                            case Quest.QuestObjectiveType.UseGameObject:
                                objectiveTypeName = "UseObject"; idAttributeName = "ObjectId"; countAttributeName = "UseCount";
                                break;
                            default:
                                continue;
                        }
                        stringBuilder.AppendFormat("\t<Objective Type=\"{0}\" {1}=\"{2}\" {3}=\"{4}\">\n",
                            objectiveTypeName, idAttributeName, objectives[index].ID, countAttributeName, objectives[index].Count);
                        if (objectives[index].Type == Quest.QuestObjectiveType.CollectItem)
                            stringBuilder.AppendFormat("\t\t<CollectFrom>\n\t\t</CollectFrom>\n");
                        stringBuilder.AppendFormat("\t\t<Hotspots>\n\t\t</Hotspots>\n\t</Objective>\n");
                    }
                }
                stringBuilder.AppendFormat("</Quest>\n");
                Logging.Write(stringBuilder.ToString());
            }
            System.Windows.Clipboard.SetText(stringBuilder.ToString());
        }

        private TreeViewItem? GetSelectedQuestItem(TreeViewItem? item)
        {
            if (item == null || item.Tag == null)
                return null;
            return item.Parent == tvQuests ? item : GetSelectedQuestItem((TreeViewItem)item.Parent);
        }

        private void AppendQuestInfo(Quest? quest, StringBuilder builder)
        {
            if (quest == null)
                return;

            string giverName = "";
            string turnInName = "";
            int giverId = 0;
            int turnInId = 0;
            bool foundInfo = false;

            // Get quest giver/turnin from Wowhead
            try
            {
                string end;
                using (StreamReader streamReader = new StreamReader(
                    WebRequest.Create($"http://www.wowhead.com/quest={quest.Id}").GetResponse().GetResponseStream()!))
                    end = streamReader.ReadToEnd();
                foundInfo = ParseWowheadSource(end, out giverName!, out giverId, out turnInName!, out turnInId);
            }
            catch
            {
                foundInfo = false;
            }

            if (foundInfo)
                builder.AppendFormat("<PickUp QuestName=\"{0}\" QuestId=\"{1}\" GiverName=\"{2}\" GiverId=\"{3}\" />\n",
                    quest.Name, quest.Id, giverName, giverId);
            else
                builder.AppendFormat("<PickUp QuestName=\"{0}\" QuestId=\"{1}\" GiverName=\"\" GiverId=\"\" />\n",
                    quest.Name, quest.Id);

            List<Quest.QuestObjective> objectives = ((PlayerQuest)quest).GetObjectives();
            for (int index = 0; index < objectives.Count; ++index)
            {
                if (!objectives[index].IsEmpty && objectives[index].Type != Quest.QuestObjectiveType.Special)
                {
                    string objectiveTypeName, idAttributeName, countAttributeName;
                    switch (objectives[index].Type)
                    {
                        case Quest.QuestObjectiveType.CollectIntermediateItem:
                        case Quest.QuestObjectiveType.CollectItem:
                            objectiveTypeName = "CollectItem"; idAttributeName = "ItemId"; countAttributeName = "CollectCount";
                            break;
                        case Quest.QuestObjectiveType.KillMob:
                            objectiveTypeName = "KillMob"; idAttributeName = "MobId"; countAttributeName = "KillCount";
                            break;
                        case Quest.QuestObjectiveType.UseGameObject:
                            objectiveTypeName = "UseObject"; idAttributeName = "ObjectId"; countAttributeName = "UseCount";
                            break;
                        default:
                            continue;
                    }
                    builder.AppendFormat("<Objective QuestName=\"{0}\" QuestId=\"{1}\" Type=\"{2}\" {3}=\"{4}\" {5}=\"{6}\" />\n",
                        quest.Name, quest.Id, objectiveTypeName, idAttributeName, objectives[index].ID, countAttributeName, objectives[index].Count);
                }
            }

            if (foundInfo)
                builder.AppendFormat("<TurnIn QuestName=\"{0}\" QuestId=\"{1}\" TurnInName=\"{2}\" TurnInId=\"{3}\" />\n\n",
                    quest.Name, quest.Id, turnInName, turnInId);
            else
                builder.AppendFormat("<TurnIn QuestName=\"{0}\" QuestId=\"{1}\" TurnInName=\"\" TurnInId=\"\" />\n\n",
                    quest.Name, quest.Id);
        }

        private bool ParseWowheadSource(string source, out string? giverName, out int giverId,
            out string? turnInName, out int turnInId)
        {
            giverName = null;
            turnInName = null;
            giverId = 0;
            turnInId = 0;

            Match match1 = _startIdParser.Match(source);
            Match match2 = _endIdParser.Match(source);
            Match match3 = _startNameParser.Match(source);
            Match match4 = _endNameParser.Match(source);

            if (match1.Success && !match3.Success)
                match3 = new Regex(string.Format(_startItemNameParserFormat, match1.Value)).Match(source);

            if (!match1.Success || !match2.Success || !match3.Success || !match4.Success)
                return false;

            giverId = int.Parse(match1.Value);
            turnInId = int.Parse(match2.Value);
            giverName = match3.Value;
            turnInName = match4.Value;
            return true;
        }

        #endregion

        #region Common - Blackspot/Hotspot Generation

        private void btnGenerateBlackspot_Click(object sender, RoutedEventArgs e)
        {
            if (!ObjectManager.IsInGame)
                return;
            ObjectManager.Update();

            if (!_lastBlackspotPos.HasValue)
            {
                _lastBlackspotPos = ObjectManager.Me.Location;
            }
            else
            {
                WoWPoint location = ObjectManager.Me.Location;
                var last = _lastBlackspotPos.Value;
                WoWPoint woWpoint = new WoWPoint((last.X + location.X) / 2.0, (last.Y + location.Y) / 2.0, (last.Z + location.Z) / 2.0);
                float horizontalRadius = location.Distance2D(last) / 2f;
                float halfZDifference = Math.Abs(last.Z - location.Z) / 2f;
                string heightPart = halfZDifference > 10.0f ? $"Height=\"{(halfZDifference + 3f).ToString(CultureInfo.InvariantCulture)}\" " : string.Empty;
                Logging.Write($"<Blackspot X=\"{woWpoint.X.ToString(CultureInfo.InvariantCulture)}\" Y=\"{woWpoint.Y.ToString(CultureInfo.InvariantCulture)}\" Z=\"{woWpoint.Z.ToString(CultureInfo.InvariantCulture)}\" Radius=\"{horizontalRadius.ToString(CultureInfo.InvariantCulture)}\" {heightPart}/>");
                _lastBlackspotPos = null;
            }
        }

        private void btnGenerateHotspot_Click(object sender, RoutedEventArgs e)
        {
            ObjectManager.Update();
            if (ObjectManager.Me == null)
                return;
            WoWPoint location = ObjectManager.Me.Location;
            Logging.Write($"<Hotspot X=\"{FormatCoordinate(location.X)}\" Y=\"{FormatCoordinate(location.Y)}\" Z=\"{FormatCoordinate(location.Z)}\" />");
        }

        private static string FormatCoordinate(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region Targeting List Updates

        private void LootTargetUpdated(object context)
        {
            if (Dispatcher.Thread == System.Threading.Thread.CurrentThread)
            {
                Loots.Clear();
                foreach (WoWObject target in Styx.Logic.LootTargeting.Instance.TargetList)
                    Loots.Add(target);
            }
            else
                Dispatcher.Invoke(new Action<object>(LootTargetUpdated), context);
        }

        private void TargetingListUpdated(object context)
        {
            if (Dispatcher.Thread == System.Threading.Thread.CurrentThread)
            {
                Targets.Clear();
                foreach (WoWObject target in Styx.Logic.Targeting.Instance.TargetList)
                    Targets.Add(target);
            }
            else
                Dispatcher.Invoke(new Action<object>(TargetingListUpdated), context);
        }

        #endregion

        #region Objects Tab

        private void btnRefreshObjectList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ObjectManager.IsInGame)
                    return;

                ObjectManager.Update();
                RefreshObjectLists();
            }
            catch { }
        }

        private void RefreshObjectLists()
        {
            RefreshGameObjects(GetGameObjects());
            RefreshUnits(GetUnits());
            RefreshItems(GetItems());
        }

        private List<WGameObject> GetGameObjects()
        {
            // use cached game objects to avoid expensive queries during UI refresh
            return ObjectManager.CachedObjects
                .OrderBy(o => o.DistanceSqr)
                .Select(o => new WGameObject
                {
                    Entry = (uint)o.Entry,
                    Name = o.Name,
                    Guid = o.Guid,
                    Location = o.Location,
                    Distance = o.Distance,
                    Type = o.Type,
                    SubType = o.SubType
                })
                .ToList();
        }

        private void RefreshGameObjects(IEnumerable<WGameObject> objects)
        {
            _gameObjects.Clear();
            foreach (var gameObject in objects)
                _gameObjects.Add(gameObject);
        }

        private List<WUnit> GetUnits()
        {
            return ObjectManager.CachedUnits
                .OrderBy(o => o.DistanceSqr)
                .Select(o => new WUnit
                {
                    Entry = (uint)o.Entry,
                    Name = o.Name,
                    Guid = o.Guid,
                    Location = o.Location,
                    Distance = o.Distance,
                    Type = o.Type,
                    FactionId = (uint)o.FactionId
                })
                .ToList();
        }

        private void RefreshUnits(IEnumerable<WUnit> units)
        {
            _units.Clear();
            foreach (var unit in units)
                _units.Add(unit);
        }

        private List<WItem> GetItems()
        {
            return ObjectManager.GetObjectsOfType<WoWItem>()
                .OrderBy(o => o.Name)
                .Select(o => new WItem
                {
                    Entry = (uint)o.Entry,
                    Name = o.Name,
                    Guid = o.Guid,
                    Location = o.Location,
                    Distance = o.Distance,
                    Type = o.Type,
                    StackCount = (uint)o.StackCount
                })
                .ToList();
        }

        private void RefreshItems(IEnumerable<WItem> items)
        {
            _items.Clear();
            foreach (var item in items)
                _items.Add(item);
        }

        private void WoWObjectListViewColumn_OnClick(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is not GridViewColumnHeader header || header.Role == GridViewColumnHeaderRole.Padding)
                return;

            var direction = header != _lastHeaderClicked
                ? ListSortDirection.Ascending
                : (_lastDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending);

            var columnName = header.Column?.Header as string;
            if (string.IsNullOrEmpty(columnName))
                return;

            Sort(columnName, direction, sender as System.Windows.Controls.ListView);

            header.Column.HeaderTemplate = direction == ListSortDirection.Ascending
                ? Resources["HeaderTemplateArrowUp"] as DataTemplate
                : Resources["HeaderTemplateArrowDown"] as DataTemplate;

            if (_lastHeaderClicked != null && _lastHeaderClicked != header)
                _lastHeaderClicked.Column.HeaderTemplate = null;

            _lastHeaderClicked = header;
            _lastDirection = direction;
        }

        private static void Sort(string sortBy, ListSortDirection direction, System.Windows.Controls.ListView? listView)
        {
            if (listView?.ItemsSource == null)
                return;

            ICollectionView dataView = CollectionViewSource.GetDefaultView(listView.ItemsSource);
            dataView.SortDescriptions.Clear();
            dataView.SortDescriptions.Add(new SortDescription(sortBy, direction));
            dataView.Refresh();
        }

        private void CopyGameObjectInfo_OnClick(object sender, RoutedEventArgs e)
        {
            if (lvGameObjectsDump.SelectedItem is not WGameObject selected)
                return;

            var location = selected.Location;
            string xml = string.Format(
                CultureInfo.InvariantCulture,
                "<GameObject Name=\"{0}\" Entry=\"{1}\" X=\"{2}\" Y=\"{3}\" Z=\"{4}\" />",
                selected.Name, selected.Entry,
                FormatCoordinate(location.X), FormatCoordinate(location.Y), FormatCoordinate(location.Z));

            Logging.Write(xml);
            try { System.Windows.Clipboard.SetText(xml); } catch { }
        }

        private void CopyUnitInfo_OnClick(object sender, RoutedEventArgs e)
        {
            if (lvUnitsDump.SelectedItem is not WUnit selected)
                return;

            var location = selected.Location;
            string xml = string.Format(
                CultureInfo.InvariantCulture,
                "<WoWUnit Name=\"{0}\" Entry=\"{1}\" X=\"{2}\" Y=\"{3}\" Z=\"{4}\" />",
                selected.Name, selected.Entry,
                FormatCoordinate(location.X), FormatCoordinate(location.Y), FormatCoordinate(location.Z));

            Logging.Write(xml);
            try { System.Windows.Clipboard.SetText(xml); } catch { }
        }

        private void ViewAurasOnUnit_OnClick(object sender, RoutedEventArgs e)
        {
            if (lvUnitsDump.SelectedItem is not WUnit selected)
                return;

            var unit = ObjectManager.GetObjectByGuid<WoWUnit>(selected.Guid);
            if (unit == null)
            {
                Logging.Write("[DevTools] Unable to locate unit for aura inspection.");
                return;
            }

            var auraValues = unit.Auras?.Values;
            if (auraValues == null || auraValues.Count == 0)
            {
                Logging.Write($"[DevTools] {unit.Name} has no auras.");
                return;
            }

            Logging.Write($"[DevTools] Auras on {unit.Name}:");
            foreach (var aura in auraValues.OrderBy(a => a.Name))
                Logging.Write($"  - {aura.Name} (Id: {aura.SpellId}, Stacks: {aura.StackCount}, TimeLeft: {aura.TimeLeft.TotalSeconds:F1}s)");
        }

        private void CopyItemInfo_OnClick(object sender, RoutedEventArgs e)
        {
            if (lvItemsDump.SelectedItem is not WItem selected)
                return;

            string xml = string.Format(
                CultureInfo.InvariantCulture,
                "<WoWItem Name=\"{0}\" Entry=\"{1}\" StackCount=\"{2}\" />",
                selected.Name, selected.Entry, selected.StackCount);

            Logging.Write(xml);
            try { System.Windows.Clipboard.SetText(xml); } catch { }
        }

        #endregion

        #region Window Lifecycle

        protected override void OnClosed(EventArgs e)
        {
            _refreshTimer?.Stop();

            try
            {
                Styx.Logic.Targeting.Instance.OnTargetListUpdateFinished -= TargetingListUpdated;
                Styx.Logic.LootTargeting.Instance.OnTargetListUpdateFinished -= LootTargetUpdated;
            }
            catch { }

            try
            {
                Loots.Clear();
                Targets.Clear();
                _gameObjects.Clear();
                _units.Clear();
                _items.Clear();
                tvQuests.DataContext = null;
            }
            catch { }

            base.OnClosed(e);
        }

        #endregion

        #region Attachment Helper

        private void EnsureAttached()
        {
            // Just ensure ObjectManager is initialized if we have a WoW process
            if (ObjectManager.Wow != null)
                return;

            // ObjectManager.Initialize should be called from MainWindow when attaching
        }

        #endregion

        #region Wrapper Types

        private class WObject
        {
            public uint Entry { get; set; }
            public string Name { get; set; } = string.Empty;
            public ulong Guid { get; set; }
            public WoWPoint Location { get; set; }
            public double Distance { get; set; }
            public WoWObjectType Type { get; set; }
        }

        private class WGameObject : WObject
        {
            public Styx.WoWGameObjectType SubType { get; set; }
        }

        private class WUnit : WObject
        {
            public uint FactionId { get; set; }
        }

        private class WItem : WObject
        {
            public uint StackCount { get; set; }
        }

        #endregion
    }
}
