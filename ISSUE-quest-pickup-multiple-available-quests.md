# [Questing] NPC 同时出现两个可接任务时无法接取并重复交互

## 问题描述

使用 `Questing`（任务机器人）执行任务流程时，如果 NPC 对话菜单中同时出现两个可接任务，机器人无法正确选择并接取目标任务。

表现为：

1. 机器人移动到任务 NPC 附近并打开对话。
2. NPC 的任务列表中出现两个或多个可接任务。
3. 机器人没有正确进入目标任务的接受界面，或者选择了错误的任务条目。
4. 任务没有被接取，机器人关闭对话窗口。
5. 机器人再次与 NPC 对话，重复上述过程，形成无限循环。

## 复现条件

- 客户端：WoW 3.3.5a，Build 12340
- BotBase：`Questing`
- 使用 XML 任务 Profile
- 任务 NPC 同时提供至少两个可接任务
- 目标任务不是 NPC 列表中的第一个任务时更容易复现

## 期望结果

机器人应根据当前 Profile 中的任务 ID 或任务名称，准确选择目标任务，进入任务详情界面并点击接受；如果 NPC 还有其他任务，也不应影响当前目标任务的接取。

## 实际结果

机器人无法接取目标任务，随后关闭对话并立即重新交互 NPC，持续重复，任务流程无法继续。

## 相关源码线索

问题疑似位于：

- `Bots/Quest/QuestOrder/ForcedQuestPickUp.cs`
  - `SelectAvailableQuest()` 负责从 NPC 的可接任务列表中选择任务。
  - 同时读取 `GossipFrame.AvailableQuests` 和 `QuestFrame.AvailableQuests`。
  - Gossip 列表数量来自 Lua，但任务 ID/索引部分来自 WoW 内存结构。
  - 最终使用 `GossipFrame.Instance.SelectAvailableQuest(questIndex)` 选择任务。
- `Styx/Logic/Inventory/Frames/Gossip/GossipFrame.cs`
  - `SelectAvailableQuest(int index)` 同时调用：
    - `SelectAvailableQuest(index + 1)`
    - `SelectGossipAvailableQuest(index + 1)`
- `Bots/Quest/QuestOrder/ForcedQuestPickUp.cs`
  - `HandleQuestFrame()` 在任务窗口没有出现可操作按钮时会关闭窗口并返回外层行为树。
  - 外层流程随后再次与 NPC 交互，因此产生重复对话。

## 初步怀疑

当 NPC 同时提供多个可接任务时，内存读取到的 `GossipQuestEntry.Id`/索引与 Lua 的任务列表顺序可能不一致。当前逻辑在列表来源之间切换后仍直接复用同一个索引，可能导致选择了错误的任务条目。

另外，选择任务后仅等待固定时间，未确认：

- 目标任务是否已经成为 `CurrentShownQuestId`；
- `QuestFrameAcceptButton` 是否出现；
- 当前显示的任务是否与 Profile 中的 `QuestId` 一致。

## 建议排查方向

1. 在选择任务前后记录以下信息：
   - 目标任务 ID 和名称；
   - `GetNumGossipAvailableQuests()` 返回值；
   - Lua 任务名称列表及其索引；
   - `GossipQuestEntry.Id`、`GossipQuestEntry.Index`；
   - `QuestFrame.CurrentShownQuestId`；
   - `QuestFrameAcceptButton.IsVisible`。
2. 对两个可接任务分别验证 Lua 索引与内存索引是否一致。
3. 选择任务后等待任务窗口状态变化，而不是固定等待后直接进入下一步。
4. 如果目标任务没有进入接受界面，应重新读取列表并按任务 ID/名称重新匹配，避免无条件关闭并重新交互。
5. 增加单次 NPC 交互的最大重试次数，并在达到次数后输出明确错误日志，避免静默无限循环。

## 附件

用户提供了 NPC 多任务菜单下任务机器人重复交互的截图，另附有任务机器人和插件界面截图作为运行环境参考。
