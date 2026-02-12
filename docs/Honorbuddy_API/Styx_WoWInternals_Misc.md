# Styx.WoWInternals.Misc

Contains miscellaneous WoW wrapper classes.

## Contents

- [NetStats Structure](#netstats-structure)
- [Stable Class](#stable-class)
- [StabledPet Class](#stabledpet-class)
- [WoWAuction Class](#wowauction-class)
- [WoWClient Class](#wowclient-class)

---

### NetStats Structure

```csharp
public struct NetStats
```

A net stats.

#### NetStats Fields

- **`BytesReceived Field`**
  ```csharp
  public uint BytesReceived
  ```
  The bytes received.

- **`BytesSent Field`**
  ```csharp
  public uint BytesSent
  ```
  The bytes sent.

- **`StartTime Field`**
  ```csharp
  public uint StartTime
  ```
  The start time.

---

### Stable Class

**Inheritance:** System.Object → Styx.WoWInternals.Misc.Stable

```csharp
public class Stable
```

A stable.

#### Stable Methods

- **`GetCarriedPets Method`**
  ```csharp
  public List&lt;StabledPet&gt; GetCarriedPets()
  ```
  Gets all pets the player is carrying and that therefore can be summoned.
  - **Returns:** See Also

- **`GetNonCarriedPets Method`**
  ```csharp
  public List&lt;StabledPet&gt; GetNonCarriedPets()
  ```
  Gets all stabled pets the player is not carrying. None of these pets can be summoned.
  - **Returns:** See Also

- **`GetPet Method`**
  ```csharp
  public StabledPet GetPet(
int index
)
  ```
  Gets a stabled pet. Returns null if there is no pet in the slot.
  - *index*: Type: SystemAddLanguageSpecificTextSet("LST9B5C137C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32.
  - **Returns:** ReferenceStable Class

- **`GetPets Method`**
  ```csharp
  public List&lt;StabledPet&gt; GetPets()
  ```
  Gets a list of all stabled pets. This includes carried pets.
  - **Returns:** See Also

---

### StabledPet Class

**Inheritance:** System.Object → Styx.WoWInternals.Misc.StabledPet

```csharp
public class StabledPet
```

A stabled pet.

#### StabledPet Properties

- **`CanSummon Property`**
  ```csharp
  public bool CanSummon { get; }
  ```
  Returns a boolean indicating whether this pet can be summoned (if it's in one of the
            first 5 slots).

- **`CreatureId Property`**
  ```csharp
  public uint CreatureId { get; }
  ```
  Gets the identifier of the creature.

- **`Index Property`**
  ```csharp
  public int Index { get; }
  ```
  Gets the zero-based index of this object.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

- **`PetLevel Property`**
  ```csharp
  public int PetLevel { get; }
  ```
  Gets the pet level.

- **`PetNumber Property`**
  ```csharp
  public uint PetNumber { get; }
  ```
  A unique number for this pet.

#### StabledPet Methods

- **`GetSummonSpellId Method`**
  ```csharp
  public int GetSummonSpellId()
  ```
  Gets the spell ID of the spell used to summon this pet if it's summonable. Returns 0
            on failure.
  - **Returns:** ReferenceStabledPet Class

- **`Summon Method`**
  ```csharp
  public bool Summon()
  ```
  Summons this object.
  - **Returns:** ReferenceStabledPet Class

---

### WoWAuction Class

**Inheritance:** System.Object → Styx.WoWInternals.Misc.WoWAuction

```csharp
public class WoWAuction
```

Class representing an item on the auction house.

#### WoWAuction Properties

- **`AuctionId Property`**
  ```csharp
  public uint AuctionId { get; }
  ```
  Gets the ID of this auction.

- **`BattlePetSpecies Property`**
  ```csharp
  public BattlePetSpecies BattlePetSpecies { get; }
  ```
  Gets the battle pet species info, or null if this auction is not for a battle pet.

- **`BattlePetSpeciesId Property`**
  ```csharp
  public uint BattlePetSpeciesId { get; }
  ```
  Gets the ID fo the battle pet species that this auction represents.

- **`BuyoutPrice Property`**
  ```csharp
  public long BuyoutPrice { get; }
  ```
  Gets the buyout price of this auction.

- **`CanUse Property`**
  ```csharp
  public bool CanUse { get; }
  ```
  Returns true if you can use this item.

- **`CurrentBid Property`**
  ```csharp
  public long CurrentBid { get; }
  ```
  Gets the current bid of this auction.

- **`CurrentBidderGuid Property`**
  ```csharp
  public WoWGuid CurrentBidderGuid { get; }
  ```
  Gets the current bidder GUID.

- **`EndTime Property`**
  ```csharp
  public uint EndTime { get; }
  ```
  Gets the end time of this auction.

- **`ExpireTime Property`**
  ```csharp
  public DateTime ExpireTime { get; }
  ```
  Returns the exact time as a DateTime when this auction ends.

- **`Index Property`**
  ```csharp
  public int Index { get; }
  ```
  Gets the zero based index of this auction in the batch it was retrieved.

- **`IsSold Property`**
  ```csharp
  public bool IsSold { get; }
  ```
  Returns true if this auction is sold.

- **`ItemId Property`**
  ```csharp
  public uint ItemId { get; }
  ```
  Gets the ID of the item that this auction has posted.

- **`ItemInfo Property`**
  ```csharp
  public ItemInfo ItemInfo { get; }
  ```
  Returns detailed info about this item.

- **`ItemLink Property`**
  ```csharp
  public string ItemLink { get; }
  ```
  Gets the item link for this auction item.

- **`ItemName Property`**
  ```csharp
  public string ItemName { get; }
  ```
  The name of this auction.

- **`MinBid Property`**
  ```csharp
  public long MinBid { get; }
  ```
  Gets the minimum bid of this auction.

- **`MinIncrement Property`**
  ```csharp
  public long MinIncrement { get; }
  ```
  Gets the minimum increment of bids on this auction.

- **`PosterGuid Property`**
  ```csharp
  public WoWGuid PosterGuid { get; }
  ```
  Gets the poster GUID of this auction.

- **`StackCount Property`**
  ```csharp
  public int StackCount { get; }
  ```
  Gets the stack count of this auction.

- **`TimeLeft Property`**
  ```csharp
  public TimeSpan TimeLeft { get; }
  ```
  Time as a TimeSpan when this auction ends.

---

### WoWClient Class

**Inheritance:** System.Object → Styx.WoWInternals.Misc.WoWClient

```csharp
public class WoWClient
```

A wow client.

#### WoWClient Properties

- **`Fps Property`**
  ```csharp
  public uint Fps { get; }
  ```
  Gets the FPS. Cached for one second

- **`IsCinematicPlaying Property`**
  ```csharp
  public bool IsCinematicPlaying { get; }
  ```
  Gets a value indicating whether a cinematic is playing.

- **`Latency Property`**
  ```csharp
  public uint Latency { get; }
  ```
  Returns the current world latency of WoW's network traffic.

#### WoWClient Methods

- **`GetNetStats Method`**
  ```csharp
  public void GetNetStats(
out float downKBs,
out float upKBs,
out uint latency
)
  ```
  Gets the current net stats.
  - *downKBs*: Type: SystemAddLanguageSpecificTextSet("LSTB6DA77BD_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleAddLanguageSpecificTextSet("LSTB6DA77BD_2?cpp=%");Down KBs. The value is cached for 10 seconds
  - *upKBs*: Type: SystemAddLanguageSpecificTextSet("LSTB6DA77BD_3?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleAddLanguageSpecificTextSet("LSTB6DA77BD_4?cpp=%");Up KBs. The value is cached for 10 seconds
  - *latency*: Type: SystemAddLanguageSpecificTextSet("LSTB6DA77BD_5?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32AddLanguageSpecificTextSet("LSTB6DA77BD_6?cpp=%");The latency. The value is cached for 10 seconds

- **`PerformanceCounter Method`**
  ```csharp
  public ulong PerformanceCounter()
  ```
  Performance counter.
  - **Returns:** ReferenceWoWClient Class

- **`PerformanceCounterToDateTime Method`**
  ```csharp
  public DateTime PerformanceCounterToDateTime(
ulong performanceCounter
)
  ```
  Converts a value retrieved by PerformanceCounter into a DateTime.
  - *performanceCounter*: Type: SystemAddLanguageSpecificTextSet("LST37E9F9E_2?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt64
  - **Returns:** ReferenceWoWClient Class

---
