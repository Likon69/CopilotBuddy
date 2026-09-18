using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace Styx.Localization
{
    /// <summary>
    /// Localizes display text supplied by dynamically compiled bots, routines and plugins.
    /// Internal bot/plugin names and enum values remain unchanged for profile and settings compatibility.
    /// </summary>
    public static class UiLocalization
    {
        private static readonly Dictionary<string, string> Simplified =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Party Bot"] = "队伍机器人",
                ["Questing"] = "任务机器人",
                ["Grind"] = "打怪机器人",
                ["GatherBuddy"] = "采集机器人",
                ["DungeonBuddy"] = "副本机器人",
                ["BGBuddy"] = "战场机器人",
                ["Combat Bot"] = "战斗机器人",
                ["AutoAngler"] = "钓鱼机器人",
                ["LazyRaider"] = "懒人助手",
                ["ProfessionBuddy"] = "专业助手",
                ["Templar"] = "圣殿骑士",
                ["Tyrael"] = "泰瑞尔",
                ["LeaderPlugin"] = "队长插件",
                ["NoCombatLoot"] = "非战斗拾取",
                ["AutoEquip2"] = "自动装备",
                ["BuddyControlPanel"] = "控制面板",
                ["DrinkPotions"] = "自动喝药",
                ["MIR2"] = "MIR2",
                ["Talented"] = "天赋助手",
                ["Tidy Bags 3 Reloaded"] = "整理背包 3 重制版",
                ["General"] = "常规",
                ["Common"] = "通用",
                ["Movement"] = "移动",
                ["Targeting"] = "目标选择",
                ["Healing"] = "治疗",
                ["Items"] = "物品",
                ["Racials"] = "种族技能",
                ["Herbalism-Heal"] = "草药治疗",
                ["Tanking"] = "坦克",
                ["Pet"] = "宠物",
                ["Misc"] = "其他",
                ["Hunter"] = "猎人",
                ["Warrior"] = "战士",
                ["Paladin"] = "圣骑士",
                ["Priest"] = "牧师",
                ["Rogue"] = "盗贼",
                ["DeathKnight"] = "死亡骑士",
                ["Shaman"] = "萨满祭司",
                ["Mage"] = "法师",
                ["Warlock"] = "术士",
                ["Druid"] = "德鲁伊",
                ["Cancel"] = "取消",
                ["Save"] = "保存",
                ["Save and Close"] = "保存并关闭",
                ["Settings"] = "设置",
                ["Never"] = "从不",
                ["On Cooldown"] = "冷却完成时",
                ["On Cooldown In Combat"] = "战斗中冷却完成时",
                ["Use First Trinket"] = "使用第一个饰品",
                ["Use Second Trinket"] = "使用第二个饰品",
                ["Trinket 1 Usage"] = "饰品 1 使用时机",
                ["Trinket 2 Usage"] = "饰品 2 使用时机",
                ["Disable Movement"] = "禁用移动",
                ["Disable Targeting"] = "禁用目标选择",
                ["Use Instance Rotation (Needs a restart !)"] = "使用副本循环（需要重启）",
                ["Wait For Res Sickness"] = "等待复活虚弱结束",
                ["Min Health"] = "最低生命值",
                ["Min Mana"] = "最低法力值",
                ["Potion Health"] = "生命药水使用阈值",
                ["Potion Mana"] = "法力药水使用阈值",
                ["Ignore Targets Health"] = "忽略高于此生命值的目标",
                ["Lifeblood HP"] = "生命之血生命值",
                ["Use Flasks"] = "使用合剂",
                ["Use Racials"] = "使用种族技能",
                ["Debug Logging"] = "调试日志",
                ["Disable Non Combat Behaviors"] = "禁用非战斗行为",
                ["Disable Pet usage"] = "禁用宠物使用",
                ["Enable Taunting for tanks"] = "坦克启用嘲讽",
                ["This section has no configurable settings."] = "此部分没有可配置的设置。",
                ["SINGULAR"] = "Singular",
                ["Singular Settings"] = "Singular 设置",
                ["Community Driven"] = "社区驱动",
                ["Minimum Value Before Removing Grays"] = "删除灰色物品的最低价值",
                ["Protected"] = "保护列表",
                ["Miscellaneous"] = "其他",
                ["Save and close"] = "保存并关闭",
                ["Close"] = "关闭",
                ["Refresh"] = "刷新",
                ["Run"] = "运行",
                ["Enable"] = "启用",
                ["Disable"] = "禁用"
            };

        private static readonly Dictionary<string, string> Words =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Use"] = "使用", ["Disable"] = "禁用", ["Enable"] = "启用", ["Wait"] = "等待",
                ["Health"] = "生命值", ["HP"] = "生命值", ["Mana"] = "法力值", ["Percent"] = "百分比",
                ["Potion"] = "药水", ["Min"] = "最低", ["Minimum"] = "最低", ["Ignore"] = "忽略",
                ["Target"] = "目标", ["Targets"] = "目标", ["Targeting"] = "目标选择", ["Movement"] = "移动",
                ["Healing"] = "治疗", ["Heal"] = "治疗", ["Items"] = "物品", ["Item"] = "物品",
                ["Trinket"] = "饰品", ["First"] = "第一个", ["Second"] = "第二个", ["Usage"] = "使用时机",
                ["General"] = "常规", ["Common"] = "通用", ["Misc"] = "其他", ["Pet"] = "宠物",
                ["Slot"] = "插槽", ["Debug"] = "调试", ["Logging"] = "日志", ["Flasks"] = "合剂",
                ["Racials"] = "种族技能", ["Hunter"] = "猎人", ["Warrior"] = "战士", ["Paladin"] = "圣骑士",
                ["Priest"] = "牧师", ["Rogue"] = "盗贼", ["Shaman"] = "萨满祭司", ["Mage"] = "法师",
                ["Warlock"] = "术士", ["Druid"] = "德鲁伊", ["DeathKnight"] = "死亡骑士",
                ["Res"] = "复活", ["Sickness"] = "虚弱", ["Pet"] = "宠物", ["Class"] = "职业",
                ["Common"] = "通用", ["Cooldown"] = "冷却", ["Combat"] = "战斗", ["Non"] = "非",
                ["Behavior"] = "行为", ["Behaviors"] = "行为", ["Above"] = "高于", ["Below"] = "低于",
                ["Count"] = "数量", ["Health"] = "生命值", ["Power"] = "能量", ["Shield"] = "护盾",
                ["Aura"] = "光环", ["Buff"] = "增益", ["Interrupt"] = "打断", ["Spells"] = "法术",
                ["Spell"] = "法术", ["Form"] = "形态", ["Stance"] = "姿态", ["Poison"] = "毒药",
                ["Main"] = "主手", ["Off"] = "副手", ["Hand"] = "手", ["Raid"] = "团队",
                ["Party"] = "队伍", ["Threat"] = "威胁", ["Drop"] = "降低", ["In"] = "在",
                ["On"] = "在", ["Only"] = "仅", ["Auto"] = "自动", ["Rotation"] = "循环",
                ["Instance"] = "副本", ["Resurrection"] = "复活", ["Summon"] = "召唤", ["Table"] = "桌子",
                ["If"] = "如果", ["Behind"] = "背后", ["Boss"] = "首领", ["Fleeing"] = "逃跑",
                ["Charge"] = "冲锋", ["Stealth"] = "潜行", ["Pull"] = "拉怪", ["Damage"] = "伤害",
                ["Cooldowns"] = "冷却技能", ["Taunting"] = "嘲讽", ["Tanks"] = "坦克", ["Tank"] = "坦克",
                ["Save"] = "保存", ["Close"] = "关闭", ["Cancel"] = "取消", ["Refresh"] = "刷新",
                ["Protected"] = "保护", ["Miscellaneous"] = "其他", ["Run"] = "运行", ["Open"] = "打开",
                ["Remove"] = "删除", ["Sell"] = "出售", ["Food"] = "食物", ["Drinks"] = "饮料",
                ["Gray"] = "灰色", ["Grey"] = "灰色", ["White"] = "白色", ["Green"] = "绿色",
                ["Blue"] = "蓝色", ["Purple"] = "紫色", ["Quality"] = "品质", ["Value"] = "价值"
            };

        private static readonly HashSet<Type> RegisteredTypes = new HashSet<Type>();
        private static readonly object ProviderLock = new object();

        public static bool IsSimplifiedChinese =>
            Globalization.Culture.Name.Equals("zh-Hans", StringComparison.OrdinalIgnoreCase) ||
            Globalization.Culture.Name.Equals("zh-CN", StringComparison.OrdinalIgnoreCase);

        public static string Translate(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || !IsSimplifiedChinese)
                return text;

            string trimmed = text.Trim();
            if (Simplified.TryGetValue(trimmed, out string value))
                return value;

            return Regex.Replace(text, @"[A-Za-z][A-Za-z']*", match =>
                Words.TryGetValue(match.Value, out string word) ? word : match.Value);
        }

        public static void LocalizeWpf(DependencyObject root)
        {
            if (!IsSimplifiedChinese || root == null)
                return;

            if (root is Window window)
                window.Title = Translate(window.Title);
            if (root is TextBlock textBlock)
                textBlock.Text = Translate(textBlock.Text);
            if (root is ContentControl contentControl && contentControl.Content is string content)
                contentControl.Content = Translate(content);
            if (root is HeaderedContentControl headered && headered.Header is string header)
                headered.Header = Translate(header);

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
                LocalizeWpf(VisualTreeHelper.GetChild(root, i));
        }

        public static void LocalizeWinForms(Form form)
        {
            if (!IsSimplifiedChinese || form == null)
                return;

            LocalizeControl(form);
        }

        public static void RunWithWinFormsLocalization(Action action)
        {
            if (action == null)
                return;

            using var timer = new System.Windows.Forms.Timer { Interval = 60 };
            timer.Tick += (sender, args) =>
            {
                foreach (Form form in System.Windows.Forms.Application.OpenForms.Cast<Form>().ToArray())
                    LocalizeControl(form);
            };
            timer.Start();
            try
            {
                action();
            }
            finally
            {
                timer.Stop();
                foreach (Form form in System.Windows.Forms.Application.OpenForms.Cast<Form>().ToArray())
                    LocalizeControl(form);
            }
        }

        private static void LocalizeControl(System.Windows.Forms.Control control)
        {
            control.Text = Translate(control.Text);

            if (control is PropertyGrid propertyGrid && propertyGrid.SelectedObject != null)
            {
                RegisterTypeDescriptionProvider(propertyGrid.SelectedObject);
                propertyGrid.Refresh();
            }

            foreach (System.Windows.Forms.Control child in control.Controls)
                LocalizeControl(child);

            if (control is ToolStrip toolStrip)
            {
                foreach (ToolStripItem item in toolStrip.Items)
                    item.Text = Translate(item.Text);
            }
        }

        private static void RegisterTypeDescriptionProvider(object settings)
        {
            Type type = settings.GetType();
            lock (ProviderLock)
            {
                if (RegisteredTypes.Add(type))
                    TypeDescriptor.AddProvider(new LocalizedTypeDescriptionProvider(type), type);
            }
        }

        private sealed class LocalizedTypeDescriptionProvider : TypeDescriptionProvider
        {
            private readonly TypeDescriptionProvider _baseProvider;

            public LocalizedTypeDescriptionProvider(Type type)
            {
                _baseProvider = TypeDescriptor.GetProvider(type);
            }

            public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
            {
                return new LocalizedTypeDescriptor(_baseProvider.GetTypeDescriptor(objectType, instance));
            }
        }

        private sealed class LocalizedTypeDescriptor : CustomTypeDescriptor
        {
            public LocalizedTypeDescriptor(ICustomTypeDescriptor parent) : base(parent) { }

            public override PropertyDescriptorCollection GetProperties()
            {
                return GetProperties(null);
            }

            public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                return new PropertyDescriptorCollection(
                    base.GetProperties(attributes).Cast<PropertyDescriptor>()
                        .Select(property => new LocalizedPropertyDescriptor(property)).ToArray(), true);
            }
        }

        private sealed class LocalizedPropertyDescriptor : PropertyDescriptor
        {
            private readonly PropertyDescriptor _baseProperty;

            public LocalizedPropertyDescriptor(PropertyDescriptor baseProperty) : base(baseProperty)
            {
                _baseProperty = baseProperty;
            }

            public override string DisplayName => Translate(_baseProperty.DisplayName);
            public override string Category => Translate(_baseProperty.Category);
            public override string Description => Translate(_baseProperty.Description);
            public override Type ComponentType => _baseProperty.ComponentType;
            public override Type PropertyType => _baseProperty.PropertyType;
            public override bool IsReadOnly => _baseProperty.IsReadOnly;
            public override bool SupportsChangeEvents => _baseProperty.SupportsChangeEvents;
            public override bool CanResetValue(object component) => _baseProperty.CanResetValue(component);
            public override object GetValue(object component) => _baseProperty.GetValue(component);
            public override void ResetValue(object component) => _baseProperty.ResetValue(component);
            public override void SetValue(object component, object value) => _baseProperty.SetValue(component, value);
            public override bool ShouldSerializeValue(object component) => _baseProperty.ShouldSerializeValue(component);
        }
    }
}
