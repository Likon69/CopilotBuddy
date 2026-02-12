# Styx.CommonBot.Frames

Contains classes for Lua frame wrappers.

## Contents

- [AuctionFrame Class](#auctionframe-class)
- [AuctionListType Enumeration](#auctionlisttype-enumeration)
- [FlightMapFrame Class](#flightmapframe-class)
- [Frame Class](#frame-class)
- [GarrisonMissionFrame Class](#garrisonmissionframe-class)
- [GossipEntry Structure](#gossipentry-structure)
- [GossipEntry.GossipEntryType Enumeration](#gossipentry.gossipentrytype-enumeration)
- [GossipFrame Class](#gossipframe-class)
- [GossipQuestEntry Class](#gossipquestentry-class)
- [GuildBankFrame Class](#guildbankframe-class)
- [GuildBankTab Class](#guildbanktab-class)
- [ItemQuality Enumeration](#itemquality-enumeration)
- [LootFrame Class](#lootframe-class)
- [LootRarity Enumeration](#lootrarity-enumeration)
- [LootSlotInfo Class](#lootslotinfo-class)
- [MailFrame Class](#mailframe-class)
- [MailFrame.InboxMailItem Class](#mailframe.inboxmailitem-class)
- [MerchantFrame Class](#merchantframe-class)
- [MerchantItem Class](#merchantitem-class)
- [QuestFrame Class](#questframe-class)
- [TaxiFrame Class](#taxiframe-class)
- [TrainerFrame Class](#trainerframe-class)
- [TrainerServiceFilter Enumeration](#trainerservicefilter-enumeration)

---

### AuctionFrame Class

**Inheritance:** System.Object → Styx.CommonBot.Frames.Frame → Styx.CommonBot.Frames.AuctionFrame

```csharp
public class AuctionFrame : Frame
```

An auction frame.

#### AuctionFrame Properties

- **`CanPerformGetAllSearch Property`**
  ```csharp
  public bool CanPerformGetAllSearch { get; }
  ```
  Returns true if auction search button would be active, false otherwise.

- **`CanPerformSearch Property`**
  ```csharp
  public bool CanPerformSearch { get; }
  ```
  Returns true if auction search button would be active, false otherwise.

- **`FullNumBidderAuctions Property`**
  ```csharp
  public int FullNumBidderAuctions { get; }
  ```
  Full number of 'bidder' auctions (all pages)

- **`FullNumListAuctions Property`**
  ```csharp
  public int FullNumListAuctions { get; }
  ```
  Full number of 'list' auctions (all pages)

- **`NumBidderAuctions Property`**
  ```csharp
  public int NumBidderAuctions { get; }
  ```
  Number of 'bidder' auctions.

- **`NumListAuctionPages Property`**
  ```csharp
  public int NumListAuctionPages { get; }
  ```
  Returns the number of pages for the current search result.

- **`NumListAuctions Property`**
  ```csharp
  public int NumListAuctions { get; }
  ```
  Number of 'list' auctions.

- **`NumOwnerAuctions Property`**
  ```csharp
  public int NumOwnerAuctions { get; }
  ```
  Number of 'owner' auctions.

#### AuctionFrame Methods

- **`CalculateAuctionDeposit Method`**
  ```csharp
  public ulong CalculateAuctionDeposit(
int runTime,
int stackSize,
int numStacks
)
  ```
  Returns the required deposit for the current selling item given the specified
            duration.
  - *runTime*: Type: SystemAddLanguageSpecificTextSet("LST5C27D0B8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32  1=12h, 2=24h, 3=48h.
  - *stackSize*: Type: SystemAddLanguageSpecificTextSet("LST5C27D0B8_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32size of the stack.
  - *numStacks*: Type: SystemAddLanguageSpecificTextSet("LST5C27D0B8_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32number of stacks.
  - **Returns:** ReferenceAuctionFrame Class

- **`CanCancelAuction Method`**
  ```csharp
  public bool CanCancelAuction(
int index
)
  ```
  Returns true if auction can be canceled, false otherwise.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LSTDE37CF90_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.
  - **Returns:** ReferenceAuctionFrame Class

- **`CancelAuction Method`**
  ```csharp
  public void CancelAuction(
int index
)
  ```
  Cancel the specified auction (on the "owner" list).
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST41537CCC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32index of the auction.

- **`CancelSell Method`**
  ```csharp
  public void CancelSell()
  ```
  Clears the auction house listing queue, not creating any additional auctions.

- **`ClickAuctionSellItemButton Method`**
  ```csharp
  public void ClickAuctionSellItemButton()
  ```
  Puts the currently 'picked up' item into the 'create auction' slot.

- **`Close Method`**
  ```csharp
  public override void Close()
  ```
  Closes this object.

- **`GetAllBatchAuctions Method`**
  ```csharp
  public WoWAuction[] GetAllBatchAuctions(
AuctionListType list
)
  ```
  Gets all batch auctions in the specified list. For the 'browse' tab, this is the
            auctions on the current page of the current search.
  - *list*: Type: Styx.CommonBot.FramesAddLanguageSpecificTextSet("LST9A649D2_1?cs=.|vb=.|cpp=::|nu=.|fs=.");AuctionListType.
  - **Returns:** See Also

- **`GetAuction Method`**
  ```csharp
  public bool GetAuction(
AuctionListType list,
int index,
out WoWAuction auction
)
  ```
  Gets an auction by index.
  - *list*: Type: Styx.CommonBot.FramesAddLanguageSpecificTextSet("LST582BD0F6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");AuctionListType   .
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST582BD0F6_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32  .
  - *auction*: Type: Styx.WoWInternals.MiscAddLanguageSpecificTextSet("LST582BD0F6_3?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWAuctionAddLanguageSpecificTextSet("LST582BD0F6_4?cpp=%");[out].
  - **Returns:** ReferenceAuctionFrame Class

- **`GetAuctionHouseDepositRate Method`**
  ```csharp
  public ulong GetAuctionHouseDepositRate()
  ```
  Returns the deposit rate (percentage) for the currently open auction house.
  - **Returns:** ReferenceAuctionFrame Class

- **`GetBidderAuctions Method`**
  ```csharp
  public WoWAuction[] GetBidderAuctions()
  ```
  Get's all bidder auctions.
  - **Returns:** See Also

- **`GetListAuctions Method`**
  ```csharp
  public WoWAuction[] GetListAuctions()
  ```
  Get's all auctions listed on the current page in the 'browse' tab.
  - **Returns:** See Also

- **`GetNumAuctionItems Method`**
  ```csharp
  public void GetNumAuctionItems(
AuctionListType type,
out int batch,
out int totalCount
)
  ```
  Gets the number of auction items in the specified list. For the 'browse' tab, this is
            the current search.
  - *type*: Type: Styx.CommonBot.FramesAddLanguageSpecificTextSet("LST6C16BBE0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");AuctionListType      .
  - *batch*: Type: SystemAddLanguageSpecificTextSet("LST6C16BBE0_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32AddLanguageSpecificTextSet("LST6C16BBE0_3?cpp=%");     [out].
  - *totalCount*: Type: SystemAddLanguageSpecificTextSet("LST6C16BBE0_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32AddLanguageSpecificTextSet("LST6C16BBE0_5?cpp=%");[out].

- **`GetOwnedAuctions Method`**
  ```csharp
  public WoWAuction[] GetOwnedAuctions()
  ```
  Get's all auctions owned by the player.
  - **Returns:** See Also

- **`Hide Method`**
  ```csharp
  public override void Hide()
  ```
  Hides this object.

#### AuctionFrame Fields

- **`Instance Field`**
  ```csharp
  public static readonly AuctionFrame Instance
  ```
  The instance.

---

### AuctionListType Enumeration

```csharp
public enum AuctionListType
```

Values that represent AuctionListType.

---

### FlightMapFrame Class

**Inheritance:** System.Object → Styx.CommonBot.Frames.Frame → Styx.CommonBot.Frames.FlightMapFrame

```csharp
public class FlightMapFrame : Frame
```

Default constructor.

#### FlightMapFrame Methods

- **`Close Method`**
  ```csharp
  public override void Close()
  ```
  Closes the Taxi window.

- **`Hide Method`**
  ```csharp
  public override void Hide()
  ```

#### FlightMapFrame Fields

- **`Instance Field`**
  ```csharp
  public static readonly FlightMapFrame Instance
  ```
  The instance.

---

### Frame Class

**Inheritance:** System.Object → Styx.CommonBot.Frames.Frame → More.

```csharp
public class Frame
```

A frame.

#### Frame Properties

- **`FrameName Property`**
  ```csharp
  public string FrameName { get; }
  ```
  Gets the name of the frame.

- **`IsVisible Property`**
  ```csharp
  public bool IsVisible { get; }
  ```
  Gets a value indicating whether this object is visible.

#### Frame Methods

- **`Close Method`**
  ```csharp
  public virtual void Close()
  ```

- **`Hide Method`**
  ```csharp
  public virtual void Hide()
  ```
  Hides this object.

- **`Show Method`**
  ```csharp
  public virtual void Show()
  ```
  Shows this object.

---

### GarrisonMissionFrame Class

**Inheritance:** System.Object → Styx.CommonBot.Frames.Frame → Styx.CommonBot.Frames.GarrisonMissionFrame

```csharp
public class GarrisonMissionFrame : Frame
```

Initializes a new instance of the GarrisonMissionFrame class

#### GarrisonMissionFrame Properties

- **`MissionNpcGuid Property`**
  ```csharp
  public WoWGuid MissionNpcGuid { get; }
  ```

#### GarrisonMissionFrame Methods

- **`AddFollower Method`**

- **`AddFollower Method (Int32, GarrisonFollower)`**
  ```csharp
  public void AddFollower(
int missionId,
GarrisonFollower follower
)
  ```
  - *missionId*: Type: SystemAddLanguageSpecificTextSet("LST42FA5E09_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32
  - *follower*: Type: Styx.WoWInternals.GarrisonAddLanguageSpecificTextSet("LST42FA5E09_2?cs=.|vb=.|cpp=::|nu=.|fs=.");GarrisonFollower

- **`AddFollower Method (Int32, Int32)`**
  ```csharp
  public void AddFollower(
int missionId,
int followerId
)
  ```
  - *missionId*: Type: SystemAddLanguageSpecificTextSet("LSTBAE1EBAD_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32
  - *followerId*: Type: SystemAddLanguageSpecificTextSet("LSTBAE1EBAD_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32

- **`CompleteDialogIsVisible Method`**
  ```csharp
  public bool CompleteDialogIsVisible()
  ```
  - **Returns:** ReferenceGarrisonMissionFrame Class

- **`HideCompleteDialog Method`**
  ```csharp
  public void HideCompleteDialog()
  ```

- **`ShowMission Method`**
  ```csharp
  public void ShowMission(
int missionId
)
  ```
  - *missionId*: Type: SystemAddLanguageSpecificTextSet("LSTD7BB6BF2_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32

- **`StartMission Method`**
  ```csharp
  public void StartMission(
int missionId
)
  ```
  - *missionId*: Type: SystemAddLanguageSpecificTextSet("LST57CEA2AF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32

- **`UpdateFollowersAndMissionParty Method`**
  ```csharp
  public void UpdateFollowersAndMissionParty()
  ```

#### GarrisonMissionFrame Fields

- **`Instance Field`**
  ```csharp
  public static readonly GarrisonMissionFrame Instance
  ```

---

### GossipEntry Structure

```csharp
public struct GossipEntry
```

A gossip entry.

#### GossipEntry Properties

- **`ConfirmationText Property`**
  ```csharp
  public string ConfirmationText { get; }
  ```
  The confirmation text.

- **`Cost Property`**
  ```csharp
  public int Cost { get; }
  ```
  Gets the cost.

- **`Flags Property`**
  ```csharp
  public uint Flags { get; }
  ```
  Gets the flags.

- **`Index Property`**
  ```csharp
  public int Index { get; }
  ```
  Zero-based index of this instance.

- **`ServerIndex Property`**
  ```csharp
  public int ServerIndex { get; }
  ```
  Gets the index used by server.

- **`Text Property`**
  ```csharp
  public string Text { get; }
  ```
  The text.

- **`Type Property`**
  ```csharp
  public GossipEntry.GossipEntryType Type { get; }
  ```
  The type.

#### GossipEntry Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  - **Returns:** ReferenceGossipEntry Structure

---

### GossipEntry.GossipEntryType Enumeration

```csharp
public enum GossipEntryType
```

Values that represent GossipEntryType.

---

### GossipFrame Class

**Inheritance:** System.Object → Styx.CommonBot.Frames.Frame → Styx.CommonBot.Frames.GossipFrame

```csharp
public class GossipFrame : Frame
```

A gossip frame.

#### GossipFrame Properties

- **`ActiveQuests Property`**
  ```csharp
  public List&lt;GossipQuestEntry&gt; ActiveQuests { get; }
  ```
  Retrieves a list of the active (?) quests on the NPC you are talking to.

- **`AvailableQuests Property`**
  ```csharp
  public List&lt;GossipQuestEntry&gt; AvailableQuests { get; }
  ```
  Retrieves a list of the available (!) quests on the NPC you are talking to.

- **`GossipOptionEntries Property`**
  ```csharp
  public List&lt;GossipEntry&gt; GossipOptionEntries { get; }
  ```
  Retrieves a list of the available gossip items on the NPC you are talking to.

- **`GossipText Property`**
  ```csharp
  public string GossipText { get; }
  ```
  Retrieves the gossip text.

#### GossipFrame Methods

- **`Close Method`**
  ```csharp
  public override void Close()
  ```
  Dismiss the gossip window.

- **`Hide Method`**
  ```csharp
  public void Hide()
  ```
  Hides this frame.

- **`SelectActiveGossipQuest Method`**
  ```csharp
  public void SelectActiveGossipQuest(
int index
)
  ```
  Selects a zero based active quest.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST63FFD9BE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.

- **`SelectActiveQuest Method`**
  ```csharp
  public void SelectActiveQuest(
int index
)
  ```
  Selects a zero based active quest.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LSTC115E4BD_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.

- **`SelectAvailableQuest Method`**
  ```csharp
  public void SelectAvailableQuest(
int index
)
  ```
  Selects a zero based available quest.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST4C8CA680_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.

- **`SelectGossipOption Method`**
  ```csharp
  public void SelectGossipOption(
int index
)
  ```
  Selects a zero based gossip option.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LSTC6430779_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.

#### GossipFrame Fields

- **`Instance Field`**
  ```csharp
  public static readonly GossipFrame Instance
  ```
  The instance.

---

### GossipQuestEntry Class

**Inheritance:** System.Object → Styx.CommonBot.Frames.GossipQuestEntry

```csharp
public class GossipQuestEntry
```

A gossip question entry.

#### GossipQuestEntry Properties

- **`Id Property`**
  ```csharp
  public int Id { get; }
  ```
  Gets the identifier.

- **`IsValid Property`**
  ```csharp
  public bool IsValid { get; }
  ```
  Gets a value indicating whether this object is valid.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

- **`RequiredLevel Property`**
  ```csharp
  public int RequiredLevel { get; }
  ```
  Gets the required level.

#### GossipQuestEntry Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceGossipQuestEntry Class

#### GossipQuestEntry Fields

- **`Index Field`**
  ```csharp
  public int Index
  ```
  Zero-based index of the.

---

### GuildBankFrame Class

**Inheritance:** System.Object → Styx.CommonBot.Frames.GuildBankFrame

```csharp
public class GuildBankFrame : IDisposable
```

A thin wrapper around the guild bank frame. Please note that this is meant to be used within a using statement, as it requires Lua events to track certain states.

#### GuildBankFrame Properties

- **`CanWithdrawMoney Property`**
  ```csharp
  public bool CanWithdrawMoney { get; }
  ```
  Returns whether or not the player can withdraw money from the guild bank.

- **`CurrentTab Property`**
  ```csharp
  public int CurrentTab { get; }
  ```
  Returns the zero-based index of the current tab in the guild bank.

- **`IsVisible Property`**
  ```csharp
  public bool IsVisible { get; }
  ```
  Whether or not the guild bank frame is currently shown.

- **`Money Property`**
  ```csharp
  public long Money { get; }
  ```
  Returns the amount of money currently in the guild bank, in copper.

- **`NumTabs Property`**
  ```csharp
  public int NumTabs { get; }
  ```
  Returns the number of tabs in the guild bank.

#### GuildBankFrame Methods

- **`Close Method`**
  ```csharp
  public void Close()
  ```
  Closes the guild bank frame.

- **`DepositItemCoroutine Method`**
  ```csharp
  public Task&lt;bool&gt; DepositItemCoroutine(
WoWItem item,
int tab
)
  ```
  A simple logic mechanism to help deposit an item from the player's bahs, to the guild bank.
  - *item*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST2592E660_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWItemThe  to deposit into the bank.
  - *tab*: Type: SystemAddLanguageSpecificTextSet("LST2592E660_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The zero-based guild bank tab.
  - **Returns:** See Also

- **`DepositMoney Method`**
  ```csharp
  public void DepositMoney(
int amount
)
  ```
  Deposits the specified amount of money (in copper) into the guild bank.
  - *amount*: Type: SystemAddLanguageSpecificTextSet("LST536A937F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The amount of money (in copper) to deposit.

- **`Dispose Method`**
  ```csharp
  public void Dispose()
  ```
  Releases all resources used by the GuildBankFrame

- **`GetItem Method`**
  ```csharp
  public ItemInfo GetItem(
int tab,
int slot
)
  ```
  Gets the  at the specified tab, and slot of the guild bank.
  - *tab*: Type: SystemAddLanguageSpecificTextSet("LST888326ED_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The zero-based guild bank tab.
  - *slot*: Type: SystemAddLanguageSpecificTextSet("LST888326ED_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The zero-based item slot in the guild bank.
  - **Returns:** ReferenceGuildBankFrame Class

- **`GetItemAndCount Method`**
  ```csharp
  public bool GetItemAndCount(
int tab,
int slot,
out ItemInfo itemInfo,
out int count
)
  ```
  Gets the  and count at the specified tab, and slot of the guild bank.
  - *tab*: Type: SystemAddLanguageSpecificTextSet("LST1CE2E11_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The zero-based guild bank tab.
  - *slot*: Type: SystemAddLanguageSpecificTextSet("LST1CE2E11_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The zero-based item slot in the guild bank.
  - *itemInfo*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST1CE2E11_3?cs=.|vb=.|cpp=::|nu=.|fs=.");ItemInfoAddLanguageSpecificTextSet("LST1CE2E11_4?cpp=%");The  for the item requested.
  - *count*: Type: SystemAddLanguageSpecificTextSet("LST1CE2E11_5?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32AddLanguageSpecificTextSet("LST1CE2E11_6?cpp=%");The count of the item requested.
  - **Returns:** ReferenceGuildBankFrame Class

- **`GetItemCount Method`**
  ```csharp
  public int GetItemCount(
int tab,
int slot
)
  ```
  Gets the item count at the specified tab, and slot of the guild bank.
  - *tab*: Type: SystemAddLanguageSpecificTextSet("LST5946B5FE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The zero-based guild bank tab.
  - *slot*: Type: SystemAddLanguageSpecificTextSet("LST5946B5FE_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The zero-based item slot in the guild bank.
  - **Returns:** ReferenceGuildBankFrame Class

- **`GetTabInfo Method`**
  ```csharp
  public GuildBankTab GetTabInfo(
int tab
)
  ```
  Returns an instance of  containing information about the specified guild bank tab.
  - *tab*: Type: SystemAddLanguageSpecificTextSet("LSTF0C6E04A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The zero-based index of the tab to retrieve info for.
  - **Returns:** ReferenceGuildBankFrame Class

- **`PickupItem Method`**
  ```csharp
  public void PickupItem(
int tab,
int slot
)
  ```
  Picks up an item from or puts an item into the guild bank. If the cursor is empty and the referenced guild bank
                slot contains an item, that item is put onto the cursor. If the cursor contains an item and the slot is empty, the
                item is placed into the slot. If both the cursor and the slot contain items, the contents of the cursor and the
                guild bank slot are exchanged.
  - *tab*: Type: SystemAddLanguageSpecificTextSet("LSTDE4147E3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The zero-based guild bank tab.
  - *slot*: Type: SystemAddLanguageSpecificTextSet("LSTDE4147E3_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The zero-based item slot in the guild bank.

- **`QueryTabs Method`**
  ```csharp
  public Task QueryTabs()
  ```
  Requests information about the contents of all guild bank tabs. This method waits for the GUILDBANKBAGSLOTS_CHANGED Lua event to be fired.
  - **Returns:** ReferenceGuildBankFrame Class

- **`SetTab Method`**
  ```csharp
  public void SetTab(
int tab
)
  ```
  Selects the current tab in the GuildBank frame
  - *tab*: Type: SystemAddLanguageSpecificTextSet("LSTC6DC67E6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The zero-based tab to select.

- **`SplitItem Method`**
  ```csharp
  public void SplitItem(
int tab,
int slot,
int amount
)
  ```
  Picks up only part of a stack of items from the guild bank. Has no effect if the given amount
                is greater than the number of items stacked in the slot.
  - *tab*: Type: SystemAddLanguageSpecificTextSet("LSTFF4C009E_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The zero-based guild bank tab.
  - *slot*: Type: SystemAddLanguageSpecificTextSet("LSTFF4C009E_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The zero-based item slot in the guild bank.
  - *amount*: Type: SystemAddLanguageSpecificTextSet("LSTFF4C009E_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The number of items to pick up.

- **`TakeItem Method`**
  ```csharp
  public void TakeItem(
int tab,
int slot
)
  ```
  Takes an item from the guild bank, and puts it in the player's bag.
  - *tab*: Type: SystemAddLanguageSpecificTextSet("LSTAD5F1F38_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The zero-based guild bank tab.
  - *slot*: Type: SystemAddLanguageSpecificTextSet("LSTAD5F1F38_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The zero-based item slot in the guild bank.

- **`WithdrawItemCoroutine Method`**
  ```csharp
  public Task&lt;bool&gt; WithdrawItemCoroutine(
int tab,
int slot
)
  ```
  A simple logic mechanism to help withdraw an item from the guild bank, to the player's bags.
  - *tab*: Type: SystemAddLanguageSpecificTextSet("LST4763EFEB_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The zero-based guild bank tab.
  - *slot*: Type: SystemAddLanguageSpecificTextSet("LST4763EFEB_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The zero-based item slot in the guild bank.
  - **Returns:** See Also

- **`WithdrawMoney Method`**
  ```csharp
  public void WithdrawMoney(
int amount
)
  ```
  Withdraws the specified amount of money (in copper) from the guild bank.
  - *amount*: Type: SystemAddLanguageSpecificTextSet("LSTB2E59901_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The amount of money (in copper) to withdraw.

---

### GuildBankTab Class

**Inheritance:** System.Object → Styx.CommonBot.Frames.GuildBankTab

```csharp
public class GuildBankTab
```

Whether the player can deposit items into the tab.

#### GuildBankTab Properties

- **`CanDeposit Property`**
  ```csharp
  public bool CanDeposit { get; }
  ```
  Whether the player can deposit items into the tab.

- **`Icon Property`**
  ```csharp
  public string Icon { get; }
  ```
  The current icon of the tab.

- **`IsFiltered Property`**
  ```csharp
  public bool IsFiltered { get; }
  ```
  Whether this tab is filtered.

- **`IsViewable Property`**
  ```csharp
  public bool IsViewable { get; }
  ```
  Whether the player can view the tab.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  The name of the tab.

- **`NumWithdrawls Property`**
  ```csharp
  public int NumWithdrawls { get; }
  ```
  How many withdrawals the player can do from the tab.

- **`RemainingWithdrawals Property`**
  ```csharp
  public int RemainingWithdrawals { get; }
  ```
  The remaining withdrawals of the _currently_open_ tab. (This is a bug/feature in the WoW client)

---

### ItemQuality Enumeration

```csharp
[FlagsAttribute]
public enum ItemQuality
```

Bitfield of flags for specifying ItemQuality.

---

### LootFrame Class

**Inheritance:** System.Object → Styx.CommonBot.Frames.Frame → Styx.CommonBot.Frames.LootFrame

```csharp
public class LootFrame : Frame
```

A loot frame.

#### LootFrame Properties

- **`IsVisible Property`**
  ```csharp
  public bool IsVisible { get; }
  ```
  Gets a value indicating whether this object is visible.

- **`LootingObjectGuid Property`**
  ```csharp
  public WoWGuid LootingObjectGuid { get; }
  ```
  Gets a unique identifier of the looting object.

- **`LootItems Property`**
  ```csharp
  public int LootItems { get; }
  ```
  Gets the loot items.

#### LootFrame Methods

- **`Close Method`**
  ```csharp
  public override void Close()
  ```
  Closes this object.

- **`GetItemId Method`**
  ```csharp
  public uint GetItemId(
int slot
)
  ```
  Gets item identifier.
  - *slot*: Type: SystemAddLanguageSpecificTextSet("LST72AB2FD6_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The slot.
  - **Returns:** ReferenceLootFrame Class

- **`Loot Method`**
  ```csharp
  public void Loot(
int slot
)
  ```
  Loots.
  - *slot*: Type: SystemAddLanguageSpecificTextSet("LST997E9106_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The slot.

- **`LootAll Method`**
  ```csharp
  public void LootAll()
  ```
  Loot all.

- **`LootInfo Method`**
  ```csharp
  public LootSlotInfo LootInfo(
int slot
)
  ```
  Loot information.
  - *slot*: Type: SystemAddLanguageSpecificTextSet("LST2A28826A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The slot.
  - **Returns:** ReferenceLootFrame Class

#### LootFrame Fields

- **`Instance Field`**
  ```csharp
  public static readonly LootFrame Instance
  ```
  The instance.

---

### LootRarity Enumeration

```csharp
public enum LootRarity
```

Values that represent LootRarity.

---

### LootSlotInfo Class

**Inheritance:** System.Object → Styx.CommonBot.Frames.LootSlotInfo

```csharp
public class LootSlotInfo
```

Class used internally by the LootFrame class to get more information about loot slot
            items.

#### LootSlotInfo Properties

- **`Locked Property`**
  ```csharp
  public bool Locked { get; }
  ```
  Whether you are eligible to loot this item or not. Locked items are by default shown
            tinted red.

- **`LootIcon Property`**
  ```csharp
  public string LootIcon { get; }
  ```
  The path of the graphical icon for the item.

- **`LootName Property`**
  ```csharp
  public string LootName { get; }
  ```
  The name of the item.

- **`LootQuantity Property`**
  ```csharp
  public int LootQuantity { get; }
  ```
  The quantity of the item in a stack. Note: Quantity for coin is always 0.

- **`LootRarity Property`**
  ```csharp
  public LootRarity LootRarity { get; }
  ```
  The rarity of this item, Grey, white, green epic etc.

---

### MailFrame Class

**Inheritance:** System.Object → Styx.CommonBot.Frames.Frame → Styx.CommonBot.Frames.MailFrame

```csharp
public class MailFrame : Frame
```

A mail frame.

#### MailFrame Properties

- **`GetCodMail Property`**
  ```csharp
  public bool GetCodMail { get; set; }
  ```
  Gets or sets a value indicating whether or not to get the cod mail.

- **`GetCodMailMax Property`**
  ```csharp
  public int GetCodMailMax { get; set; }
  ```
  Gets or sets the get cod mail maximum.

- **`HasNewMail Property`**
  ```csharp
  public bool HasNewMail { get; }
  ```
  Gets a value indicating whether this object has new mail.

- **`MailCount Property`**
  ```csharp
  public int MailCount { get; }
  ```
  Gets the number of mails.

- **`SendMailItemGuids Property`**
  ```csharp
  public WoWGuid[] SendMailItemGuids { get; }
  ```
  Gets the send mail item guids.

- **`SendMailItems Property`**
  ```csharp
  public WoWItem[] SendMailItems { get; }
  ```
  Gets the send mail items.

#### MailFrame Methods

- **`Close Method`**
  ```csharp
  public override void Close()
  ```
  Closes the mailbox frame.

- **`DeleteMail Method`**
  ```csharp
  public void DeleteMail(
int mailIndex
)
  ```
  Deletes the mail described by mailIndex.
  - *mailIndex*: Type: SystemAddLanguageSpecificTextSet("LST85D9C683_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.

- **`GetAllMails Method`**
  ```csharp
  public IEnumerable&lt;MailFrame.InboxMailItem&gt; GetAllMails()
  ```
  Gets all mails in this collection.
  - **Returns:** See Also

- **`GetMailAttachmentsCoroutine Method`**
  ```csharp
  public Task GetMailAttachmentsCoroutine(
int mailIndex
)
  ```
  Gets the mail attachments.
  - *mailIndex*: Type: SystemAddLanguageSpecificTextSet("LSTBB79E334_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32Index of the mail.
  - **Returns:** ReferenceMailFrame Class

- **`Hide Method`**
  ```csharp
  public override void Hide()
  ```
  Closes the mailbox frame.

- **`OpenAllMailCoroutine Method`**
  ```csharp
  public Task&lt;bool&gt; OpenAllMailCoroutine()
  ```
  Opens all mail .
  - **Returns:** See Also

- **`OpenAllMailFromCoroutine Method`**
  ```csharp
  public Task&lt;bool&gt; OpenAllMailFromCoroutine(
string senderName
)
  ```
  Opens all mail from senderName.
  - *senderName*: Type: SystemAddLanguageSpecificTextSet("LSTFD8964E4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringName of the sender.
  - **Returns:** See Also

- **`ReturnMail Method`**
  ```csharp
  public void ReturnMail(
int mailIndex
)
  ```
  Returns a mail.
  - *mailIndex*: Type: SystemAddLanguageSpecificTextSet("LSTA02F8C74_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.

- **`SendMail Method`**
  ```csharp
  public void SendMail(
string recipient,
string subject,
string body,
int copper,
params WoWItem[] attachmentItems
)
  ```
  Sends a mail.
  - *recipient*: Type: SystemAddLanguageSpecificTextSet("LSTBB9AE46A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String      The recipient.
  - *subject*: Type: SystemAddLanguageSpecificTextSet("LSTBB9AE46A_2?cs=.|vb=.|cpp=::|nu=.|fs=.");String        The subject.
  - *body*: Type: SystemAddLanguageSpecificTextSet("LSTBB9AE46A_3?cs=.|vb=.|cpp=::|nu=.|fs=.");String           The body.
  - *copper*: Type: SystemAddLanguageSpecificTextSet("LSTBB9AE46A_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32         The copper.
  - *attachmentItems*: Type: AddLanguageSpecificTextSet("LSTBB9AE46A_5?cpp=array&lt;");Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTBB9AE46A_6?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWItemAddLanguageSpecificTextSet("LSTBB9AE46A_7?cpp=&gt;|vb=()|nu=[]");A variable-length parameters list containing attachment items.

- **`SendMailWithManyAttachmentsCoroutine Method`**
  ```csharp
  public Task&lt;bool&gt; SendMailWithManyAttachmentsCoroutine(
string recipient,
WoWItem[] attachments
)
  ```
  - *recipient*: Type: SystemAddLanguageSpecificTextSet("LSTCEBF0476_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String
  - *attachments*: Type: AddLanguageSpecificTextSet("LSTCEBF0476_2?cpp=array&lt;");Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTCEBF0476_3?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWItemAddLanguageSpecificTextSet("LSTCEBF0476_4?cpp=&gt;|vb=()|nu=[]");
  - **Returns:** See Also

- **`SwitchToSendMailTab Method`**
  ```csharp
  public void SwitchToSendMailTab()
  ```
  ReSharper disable MemberCanBeMadeStatic.Global ReSharper disable
            MemberCanBePrivate.Global.

#### MailFrame Fields

- **`Instance Field`**
  ```csharp
  public static readonly MailFrame Instance
  ```
  The instance.

---

### MailFrame.InboxMailItem Class

**Inheritance:** System.Object → Styx.CommonBot.Frames.MailFrame.InboxMailItem

```csharp
public class InboxMailItem
```

An inbox mail item.

#### InboxMailItem Properties

- **`CanReply Property`**
  ```csharp
  public bool CanReply { get; set; }
  ```
  Gets or sets a value indicating whether we can reply.

- **`CODAmount Property`**
  ```csharp
  public int CODAmount { get; set; }
  ```
  Gets or sets the cod amount.

- **`Copper Property`**
  ```csharp
  public int Copper { get; set; }
  ```
  Gets or sets the copper.

- **`CopyCreated Property`**
  ```csharp
  public bool CopyCreated { get; set; }
  ```
  Gets or sets a value indicating whether the copy created.

- **`DaysLeft Property`**
  ```csharp
  public float DaysLeft { get; set; }
  ```
  Gets or sets the days left.

- **`Index Property`**
  ```csharp
  public int Index { get; set; }
  ```
  Gets or sets the one-based index of this object.

- **`IsGameMaster Property`**
  ```csharp
  public bool IsGameMaster { get; set; }
  ```
  Gets or sets a value indicating whether this object is game master.

- **`ItemCount Property`**
  ```csharp
  public int ItemCount { get; set; }
  ```
  Gets or sets the number of items.

- **`ItemQuantity Property`**
  ```csharp
  public int ItemQuantity { get; set; }
  ```
  Gets or sets the item quantity.

- **`PackageIcon Property`**
  ```csharp
  public string PackageIcon { get; set; }
  ```
  Gets or sets the package icon.

- **`Sender Property`**
  ```csharp
  public string Sender { get; set; }
  ```
  Gets or sets the sender.

- **`StationaryIcon Property`**
  ```csharp
  public string StationaryIcon { get; set; }
  ```
  Gets or sets the stationary icon.

- **`Subject Property`**
  ```csharp
  public string Subject { get; set; }
  ```
  Gets or sets the subject.

- **`WasRead Property`**
  ```csharp
  public bool WasRead { get; set; }
  ```
  Gets or sets a value indicating whether the was was read.

- **`WasReturned Property`**
  ```csharp
  public bool WasReturned { get; set; }
  ```
  Gets or sets a value indicating whether the was returned.

#### InboxMailItem Methods

- **`TakeAttachedItems Method`**
  ```csharp
  [ObsoleteAttribute("Use TakeAttachedItemsCoroutine")]
public void TakeAttachedItems()
  ```
  Take attached items.

- **`TakeAttachedItemsCoroutine Method`**
  ```csharp
  public Task TakeAttachedItemsCoroutine()
  ```
  Take attached items.
  - **Returns:** ReferenceMailFrameAddLanguageSpecificTextSet("LST67677EC0_2?cs=.|vb=.|cpp=::|nu=.|fs=.");InboxMailItem Class

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceMailFrameAddLanguageSpecificTextSet("LST113B1395_2?cs=.|vb=.|cpp=::|nu=.|fs=.");InboxMailItem Class

---

### MerchantFrame Class

**Inheritance:** System.Object → Styx.CommonBot.Frames.Frame → Styx.CommonBot.Frames.MerchantFrame

```csharp
public class MerchantFrame : Frame
```

Class for functions fields properites etc associated with the MerchantFrame.

#### MerchantFrame Properties

- **`Merchant Property`**
  ```csharp
  public WoWUnit Merchant { get; }
  ```
  Returns the merchant unit.

- **`MerchantNumItems Property`**
  ```csharp
  public int MerchantNumItems { get; }
  ```
  Number of items this merchant has avaible for sale.

- **`NumBuybackItems Property`**
  ```csharp
  public int NumBuybackItems { get; }
  ```
  Returns the number of buyback items.

#### MerchantFrame Methods

- **`BuyItem Method`**
  Buys an item from the merchant. 
            This will always buy "at least" the amount specified. 
            In the case of items that have an extended cost (such as a non-gold requirement), you must purchase items in stacks of whatever the vendor sells them in.
            This method does not check for reputation requirements, or arena/rated battleground rating requirements.

- **`BuyItem Method (Int32, Int32)`**
  ```csharp
  public bool BuyItem(
int index,
int amount
)
  ```
  Buys an item from the merchant. 
            This will always buy "at least" the amount specified. 
            In the case of items that have an extended cost (such as a non-gold requirement), you must purchase items in stacks of whatever the vendor sells them in.
            This method does not check for reputation requirements, or arena/rated battleground rating requirements.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LSTC8FC1CB8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32 The zero based index of the item.
  - *amount*: Type: SystemAddLanguageSpecificTextSet("LSTC8FC1CB8_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The amount to buy.
  - **Returns:** This method will purchase at least the amount specified.
            If a vendor sells an item for gold, then it will purchase the exact amount.
            However, if a vendor sells an item for a "currency" (such as Honor, or Garrison Resources), you can only buy increments of what the vendor sells.
            In this case, this method will buy "at least" the amount specified.
            
            For example:
            
            If the vendor sells 5x Herbs for 3 Garrison Resources, and you pass a value of 2 to amount, this method will buy 5 herbs, instead of 3.

- **`BuyItem Method (String, Int32)`**
  ```csharp
  public bool BuyItem(
string name,
int amount
)
  ```
  Buys an item from the merchant.
  - *name*: Type: SystemAddLanguageSpecificTextSet("LST72CDA94D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");String  name of the item.
  - *amount*: Type: SystemAddLanguageSpecificTextSet("LST72CDA94D_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The amount to buy.
  - **Returns:** ExceptionConditionExceptionThrown when an exception error condition occurs.

- **`Close Method`**
  ```csharp
  public override void Close()
  ```
  Close this frame (same as Hide())

- **`GetAllMerchantItems Method`**
  ```csharp
  public MerchantItem[] GetAllMerchantItems()
  ```
  Returns an array with MerchantItem that this merchant offers to sell.
  - **Returns:** See Also

- **`GetBestDrinkFromVendor Method`**
  ```csharp
  public int GetBestDrinkFromVendor()
  ```
  Returns the best usable drink available from a vendor. -1 if none are found.
  - **Returns:** ReferenceMerchantFrame Class

- **`GetBestFoodFromVendor Method`**
  ```csharp
  public int GetBestFoodFromVendor()
  ```
  Returns the best usable food available from a vendor. -1 if none are found.
  - **Returns:** ReferenceMerchantFrame Class

- **`GetBuybackItem Method`**
  ```csharp
  public WoWItem GetBuybackItem(
int index
)
  ```
  Returns an item from the buyback frame.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LSTC8C4E592_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The index or slot of the item in the buyback frame.
  - **Returns:** ExceptionConditionArgumentOutOfRangeExceptionThrown when one or more arguments are outside the
            required range.

- **`GetMerchantItemByIndex Method`**
  ```csharp
  public MerchantItem GetMerchantItemByIndex(
int index
)
  ```
  Gets a merchant item by index.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LSTB241A0B4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32index in the array.
  - **Returns:** ReferenceMerchantFrame Class

- **`Hide Method`**
  ```csharp
  public void Hide()
  ```
  Hide the frame (same as Close())

- **`RepairAllItems Method`**
  Reparis all your items.

- **`RepairAllItems Method`**
  ```csharp
  public void RepairAllItems()
  ```
  Reparis all your items.

- **`RepairAllItems Method (Boolean)`**
  ```csharp
  public void RepairAllItems(
bool useGuildBankFunds
)
  ```
  Reparis all your items.
  - *useGuildBankFunds*: Type: SystemAddLanguageSpecificTextSet("LSTA7147BB2_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Booleantrue if you wish to use the guildbank as payment.

- **`SellItem Method`**
  ```csharp
  public void SellItem(
WoWItem item
)
  ```
  Sell this item.
  - *item*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTBDD4844C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWItemitem to sell.

- **`SellItemQualities Method`**
  ```csharp
  public void SellItemQualities(
ItemQuality qualities,
IEnumerable&lt;string&gt; nameExceptions,
IEnumerable&lt;uint&gt; idExceptions
)
  ```
  Sells all items of the specified qualities (bitwise OR them together). Pass null for
            exceptions if you don't have any.
  - *qualities*: Type: Styx.CommonBot.FramesAddLanguageSpecificTextSet("LSTE8A94AC1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ItemQuality     The qualities.
  - *nameExceptions*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTE8A94AC1_2?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTE8A94AC1_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");StringAddLanguageSpecificTextSet("LSTE8A94AC1_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The name exceptions.
  - *idExceptions*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTE8A94AC1_5?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTE8A94AC1_6?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");UInt32AddLanguageSpecificTextSet("LSTE8A94AC1_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");  The identifier exceptions.

#### MerchantFrame Fields

- **`Instance Field`**
  ```csharp
  public static readonly MerchantFrame Instance
  ```
  The instance.

---

### MerchantItem Class

**Inheritance:** System.Object → Styx.CommonBot.Frames.MerchantItem

```csharp
public class MerchantItem
```

A merchant item.

#### MerchantItem Properties

- **`BuyPrice Property`**
  ```csharp
  public ulong BuyPrice { get; }
  ```
  Returns the buy price in copper of this MerchantItem
            compare to Me.Coinage to check if you afford it.

- **`CurrencyInfo Property`**
  ```csharp
  public CurrencyType CurrencyInfo { get; }
  ```
  Gets the currency information.

- **`ExtendedCost Property`**
  ```csharp
  public ItemExtendedCost ExtendedCost { get; }
  ```

- **`ExtendedCostId Property`**
  ```csharp
  public uint ExtendedCostId { get; }
  ```
  Returns the 'ExtendedCostId' used to check if the item requires honor, badges or
            something else to be bought requires a dbc lookup.

- **`Index Property`**
  ```csharp
  public int Index { get; }
  ```
  Returns the 1 based index of the item in the merchant's inventory.

- **`IsCurrency Property`**
  ```csharp
  public bool IsCurrency { get; }
  ```
  Gets a value indicating whether this instance is a currency.

- **`ItemId Property`**
  ```csharp
  public uint ItemId { get; }
  ```
  Returns the itemid of this MerchantItem

- **`ItemInfo Property`**
  ```csharp
  public ItemInfo ItemInfo { get; }
  ```
  Returns an ItemInfo reference of this MerchantItem.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Returns the name of this item.

- **`NumAvailable Property`**
  ```csharp
  public int NumAvailable { get; }
  ```
  Returns the number of how many times you can buy this item
            -1 = Infinite.

- **`PlayerConditionId Property`**
  ```csharp
  public uint PlayerConditionId { get; }
  ```
  Gets the player condition identifier; MatchesCondition(UInt32).

- **`Quantity Property`**
  ```csharp
  public int Quantity { get; }
  ```
  The quantity in which you buy the item.

#### MerchantItem Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceMerchantItem Class

---

### QuestFrame Class

**Inheritance:** System.Object → Styx.CommonBot.Frames.Frame → Styx.CommonBot.Frames.QuestFrame

```csharp
public class QuestFrame : Frame
```

A question frame.

#### QuestFrame Properties

- **`ActiveQuests Property`**
  ```csharp
  public List&lt;GossipQuestEntry&gt; ActiveQuests { get; }
  ```
  Retrieves a list of the active (?) quests on the NPC you are talking to.

- **`AvailableQuests Property`**
  ```csharp
  public List&lt;GossipQuestEntry&gt; AvailableQuests { get; }
  ```
  Retrieves a list of the available (!) quests on the NPC you are talking to.

- **`CurrentShownQuest Property`**
  ```csharp
  public Quest CurrentShownQuest { get; }
  ```
  Gets the current shown question.

- **`CurrentShownQuestId Property`**
  ```csharp
  public uint CurrentShownQuestId { get; }
  ```
  Gets the current shown question identifier.

#### QuestFrame Methods

- **`AcceptQuest Method`**
  ```csharp
  public void AcceptQuest()
  ```
  Accepts the current quest available in the quest frame.

- **`ClickContinue Method`**
  ```csharp
  public void ClickContinue()
  ```
  Click continue.

- **`Close Method`**
  ```csharp
  public override void Close()
  ```
  Closes the quest frame.

- **`CompleteQuest Method`**
  ```csharp
  public void CompleteQuest()
  ```
  Completes the current quest open in the quest frame.

- **`DeclineQuest Method`**
  ```csharp
  public void DeclineQuest()
  ```
  Declines the current quest available in the quest frame.

- **`Hide Method`**
  ```csharp
  public override void Hide()
  ```
  Closes the quest frame.

- **`SelectActiveGossipQuest Method`**
  ```csharp
  public void SelectActiveGossipQuest(
int index
)
  ```
  Selects a zero based active quest.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LSTE30EB3F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.

- **`SelectActiveQuest Method`**
  ```csharp
  public void SelectActiveQuest(
int index
)
  ```
  Selects a zero based active quest.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LSTEB007FA2_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.

- **`SelectAvailableQuest Method`**
  ```csharp
  public void SelectAvailableQuest(
int index
)
  ```
  Selects a zero based available quest.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LSTC64C58A7_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.

- **`SelectQuestReward Method`**
  ```csharp
  public void SelectQuestReward(
int index
)
  ```
  Selects a quest reward by a zero based index. This does not complete the quest
            immediately, but is like pressing a reward choice item manually.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST25145C49_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.

#### QuestFrame Fields

- **`Instance Field`**
  ```csharp
  public static readonly QuestFrame Instance
  ```
  The instance.

---

### TaxiFrame Class

**Inheritance:** System.Object → Styx.CommonBot.Frames.Frame → Styx.CommonBot.Frames.TaxiFrame

```csharp
public class TaxiFrame : Frame
```

A taxi frame.

#### TaxiFrame Methods

- **`Close Method`**
  ```csharp
  public override void Close()
  ```
  Closes the Taxi window.

- **`Hide Method`**
  ```csharp
  public override void Hide()
  ```

#### TaxiFrame Fields

- **`Instance Field`**
  ```csharp
  public static readonly TaxiFrame Instance
  ```
  The instance.

---

### TrainerFrame Class

**Inheritance:** System.Object → Styx.CommonBot.Frames.Frame → Styx.CommonBot.Frames.TrainerFrame

```csharp
public class TrainerFrame : Frame
```

A trainer frame.

#### TrainerFrame Properties

- **`IsTradeskillTrainer Property`**
  ```csharp
  public bool IsTradeskillTrainer { get; }
  ```
  Returns whether or not this trainer is a tradeskill trainer. If false, this is a
            class trainer.

- **`IsVisible Property`**
  ```csharp
  public bool IsVisible { get; }
  ```
  Gets a value indicating whether this object is visible.

- **`NumTrainerServices Property`**
  ```csharp
  public int NumTrainerServices { get; }
  ```
  Returns the total number of available trainer services shown.

- **`Selected Property`**
  ```csharp
  public int Selected { get; }
  ```
  Returns the currently selected trainer service index.

#### TrainerFrame Methods

- **`Buy Method`**
  ```csharp
  public void Buy(
int index
)
  ```
  Buys.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST792F5877_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.

- **`BuyAll Method`**
  ```csharp
  public bool BuyAll()
  ```
  Attempts to purchase all the available services. If it cannot, due to not having
            enough money, this function returns false.
  - **Returns:** ReferenceTrainerFrame Class

- **`Close Method`**
  ```csharp
  public override void Close()
  ```
  Closes the trainer window.

- **`GetServiceCost Method`**
  ```csharp
  public int GetServiceCost(
int index
)
  ```
  Returns the cost (in copper) to purchase a trainer service. (Spell)
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST140B2DD9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.
  - **Returns:** ReferenceTrainerFrame Class

- **`SetServiceFilter Method`**
  ```csharp
  public void SetServiceFilter(
TrainerServiceFilter filter,
bool show
)
  ```
  Sets the trainer filter shown in the UI.
  - *filter*: Type: Styx.CommonBot.FramesAddLanguageSpecificTextSet("LSTEC315756_1?cs=.|vb=.|cpp=::|nu=.|fs=.");TrainerServiceFilter.
  - *show*: Type: SystemAddLanguageSpecificTextSet("LSTEC315756_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Boolean  .

#### TrainerFrame Fields

- **`Instance Field`**
  ```csharp
  public static readonly TrainerFrame Instance
  ```
  The instance.

---

### TrainerServiceFilter Enumeration

```csharp
public enum TrainerServiceFilter
```

Values that represent TrainerServiceFilter.

---
