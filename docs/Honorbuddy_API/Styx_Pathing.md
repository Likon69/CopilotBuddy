# Styx.Pathing

Contains pathing related classes.

## Contents

- [AerialBlackspotManager Class](#aerialblackspotmanager-class)
- [BlackspotManager Class](#blackspotmanager-class)
- [BlackspotQueryFlags Enumeration](#blackspotqueryflags-enumeration)
- [ClickToMoveMover Class](#clicktomovemover-class)
- [Flightor Class](#flightor-class)
- [Flightor.MountHelper Class](#flightor.mounthelper-class)
- [Flightor.MountHelper.DisMount Class](#flightor.mounthelper.dismount-class)
- [FlyToParameters Class](#flytoparameters-class)
- [IPlayerMover Interface](#iplayermover-interface)
- [MoveResult Enumeration](#moveresult-enumeration)
- [MoveResultExtensions Class](#moveresultextensions-class)
- [MoveToParameters Class](#movetoparameters-class)
- [NavigationProvider Class](#navigationprovider-class)
- [NavigationProviderChangedEventArgs(T) Class](#navigationproviderchangedeventargst-class)
- [Navigator Class](#navigator-class)
- [PathDistanceType Enumeration](#pathdistancetype-enumeration)
- [PathGenerationFailStep Enumeration](#pathgenerationfailstep-enumeration)
- [PathInformation Class](#pathinformation-class)
- [PathNavigability Enumeration](#pathnavigability-enumeration)
- [SampleCircle Class](#samplecircle-class)
- [SamplePointsParameters Class](#samplepointsparameters-class)
- [SamplePointsType Enumeration](#samplepointstype-enumeration)
- [WrappingNavigationProvider Class](#wrappingnavigationprovider-class)

---

### AerialBlackspotManager Class

**Inheritance:** System.Object → Styx.Pathing.AerialBlackspotManager

```csharp
public static class AerialBlackspotManager
```

Gets the blackspots.

#### AerialBlackspotManager Properties

- **`Blackspots Property`**
  ```csharp
  public static IEnumerable&lt;Vector2[]&gt; Blackspots { get; }
  ```
  Gets the blackspots.

#### AerialBlackspotManager Methods

- **`AddBlackspot Method`**
  Adds a blackspot.

- **`AddBlackspot Method (UInt32, WoWFactionGroup, Vector2[])`**
  ```csharp
  public static void AddBlackspot(
uint mapId,
WoWFactionGroup faction,
Vector2[] blackspot
)
  ```
  Adds a blackspot.
  - *mapId*: Type: SystemAddLanguageSpecificTextSet("LST5A62FC4B_3?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32    Identifier for the map.
  - *faction*: Type: StyxAddLanguageSpecificTextSet("LST5A62FC4B_4?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWFactionGroup  The faction.
  - *blackspot*: Type: AddLanguageSpecificTextSet("LST5A62FC4B_5?cpp=array&lt;");System.NumericsAddLanguageSpecificTextSet("LST5A62FC4B_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector2AddLanguageSpecificTextSet("LST5A62FC4B_7?cpp=&gt;|vb=()|nu=[]");The blackspot.

- **`AddBlackspot Method (UInt32, Func(Boolean), Vector2[])`**
  ```csharp
  public static void AddBlackspot(
uint mapId,
Func&lt;bool&gt; condition,
Vector2[] blackspot
)
  ```
  Adds a blackspot.
  - *mapId*: Type: SystemAddLanguageSpecificTextSet("LST7BC256DF_5?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32    Identifier for the map.
  - *condition*: Type: SystemAddLanguageSpecificTextSet("LST7BC256DF_6?cs=.|vb=.|cpp=::|nu=.|fs=.");FuncAddLanguageSpecificTextSet("LST7BC256DF_7?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BooleanAddLanguageSpecificTextSet("LST7BC256DF_8?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The condition for applying blackspot; always applied if null.
  - *blackspot*: Type: AddLanguageSpecificTextSet("LST7BC256DF_9?cpp=array&lt;");System.NumericsAddLanguageSpecificTextSet("LST7BC256DF_10?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector2AddLanguageSpecificTextSet("LST7BC256DF_11?cpp=&gt;|vb=()|nu=[]");The blackspot.

- **`AddBlackspots Method`**
  ```csharp
  public static void AddBlackspots(
uint mapId,
WoWFactionGroup faction,
IEnumerable&lt;Vector2[]&gt; blackspots
)
  ```
  Adds the blackspots.
  - *mapId*: Type: SystemAddLanguageSpecificTextSet("LSTCEA9438_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32     Identifier for the map.
  - *faction*: Type: StyxAddLanguageSpecificTextSet("LSTCEA9438_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWFactionGroup   The faction.
  - *blackspots*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTCEA9438_3?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTCEA9438_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");AddLanguageSpecificTextSet("LSTCEA9438_5?cpp=array&lt;");Vector2AddLanguageSpecificTextSet("LSTCEA9438_6?cpp=&gt;|vb=()|nu=[]");AddLanguageSpecificTextSet("LSTCEA9438_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The blackspots.

- **`IsInBlackspot Method`**
  ```csharp
  public static bool IsInBlackspot(
Vector3 point
)
  ```
  Query if 'point' is in a blackspot.
  - *point*: Type: System.NumericsAddLanguageSpecificTextSet("LSTD7346FF0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3 The point.
  - **Returns:** raphus, 01/04/2014.

- **`RemoveBlackspot Method`**
  ```csharp
  public static void RemoveBlackspot(
uint mapId,
WoWFactionGroup faction,
Vector2[] blackspot
)
  ```
  Removes the blackspot.
  - *mapId*: Type: SystemAddLanguageSpecificTextSet("LST8A67A3CC_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32    Identifier for the map.
  - *faction*: Type: StyxAddLanguageSpecificTextSet("LST8A67A3CC_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWFactionGroup  The faction.
  - *blackspot*: Type: AddLanguageSpecificTextSet("LST8A67A3CC_3?cpp=array&lt;");System.NumericsAddLanguageSpecificTextSet("LST8A67A3CC_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector2AddLanguageSpecificTextSet("LST8A67A3CC_5?cpp=&gt;|vb=()|nu=[]");The blackspot.

- **`RemoveBlackspots Method`**
  ```csharp
  public static void RemoveBlackspots(
uint mapId,
WoWFactionGroup faction,
IEnumerable&lt;Vector2[]&gt; blackspots
)
  ```
  Removes the blackspots.
  - *mapId*: Type: SystemAddLanguageSpecificTextSet("LST2315B649_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32     Identifier for the map.
  - *faction*: Type: StyxAddLanguageSpecificTextSet("LST2315B649_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWFactionGroup   The faction.
  - *blackspots*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST2315B649_3?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST2315B649_4?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");AddLanguageSpecificTextSet("LST2315B649_5?cpp=array&lt;");Vector2AddLanguageSpecificTextSet("LST2315B649_6?cpp=&gt;|vb=()|nu=[]");AddLanguageSpecificTextSet("LST2315B649_7?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The blackspots.

---

### BlackspotManager Class

**Inheritance:** System.Object → Styx.Pathing.BlackspotManager

```csharp
public static class BlackspotManager
```

Manager for blackspots.

#### BlackspotManager Properties

- **`DynamicBlackspots Property`**
  ```csharp
  [ObsoleteAttribute("Use GetAllCurrentBlackspots")]
public static ReadOnlyCollection&lt;Blackspot&gt; DynamicBlackspots { get; }
  ```
  Gets a ReadOnlyCollection.T of the blackspots that have been dynamically added to the manager.

#### BlackspotManager Methods

- **`AddBlackspot Method`**
  Adds a dynamic blackspot. Does nothing if the blackspot is not on the current map.

- **`AddBlackspot Method (Blackspot)`**
  ```csharp
  public static void AddBlackspot(
Blackspot blackspot
)
  ```
  Adds a dynamic blackspot. Does nothing if the blackspot is not on the current map.
  - *blackspot*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LST2B2E957A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");BlackspotThe blackspot.

- **`AddBlackspot Method (Vector3, Single, Single, String)`**
  ```csharp
  public static void AddBlackspot(
Vector3 location,
float radius,
float height,
string name = ""
)
  ```
  Adds a dynamic blackspot.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LSTAA41BA96_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LSTAA41BA96_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe radius.
  - *height*: Type: SystemAddLanguageSpecificTextSet("LSTAA41BA96_3?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe height.

- **`AddBlackspots Method`**
  ```csharp
  public static void AddBlackspots(
IEnumerable&lt;Blackspot&gt; blackspots
)
  ```
  Adds multiple dynamic blackspots. This function ignores blackspots that are not on the current map.
  - *blackspots*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTA21BC1F7_1?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTA21BC1F7_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BlackspotAddLanguageSpecificTextSet("LSTA21BC1F7_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The blackspots to add.

- **`GetAllCurrentBlackspots Method`**
  ```csharp
  public static IEnumerable&lt;Blackspot&gt; GetAllCurrentBlackspots(
BlackspotQueryFlags flags = BlackspotQueryFlags.All
)
  ```
  Gets all current relevant blackspots. This is the collection of static and dynamic blackspots (according to flags), filtered by map ID.
  - **Returns:** See Also

- **`IsBlackspotted Method`**
  ```csharp
  public static bool IsBlackspotted(
Vector3 location,
float radius = 0f,
BlackspotQueryFlags flags = BlackspotQueryFlags.All
)
  ```
  Queries if a circle with given radius centered at location touches a blackspot.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST89D3FEF2_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.
  - **Returns:** To query if a point is inside a blackspot, pass 0 as radius.

- **`RemoveBlackspot Method`**
  ```csharp
  public static void RemoveBlackspot(
Blackspot blackspot
)
  ```
  Removes the specified dynamic blackspot.
  - *blackspot*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LST823B86F9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");BlackspotThe blackspot.

- **`RemoveBlackspots Method`**
  ```csharp
  public static void RemoveBlackspots(
IEnumerable&lt;Blackspot&gt; blackspots
)
  ```
  Removes the dynamic blackspots described by blackspots.
  - *blackspots*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST80B96F8A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST80B96F8A_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");BlackspotAddLanguageSpecificTextSet("LST80B96F8A_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The blackspots.

---

### BlackspotQueryFlags Enumeration

```csharp
[FlagsAttribute]
public enum BlackspotQueryFlags
```

Represents flags for querying blackspots.

---

### ClickToMoveMover Class

**Inheritance:** System.Object → Styx.Pathing.ClickToMoveMover

```csharp
public class ClickToMoveMover : IPlayerMover
```

Click to move mover. This is the default mover for Honorbuddy.

#### ClickToMoveMover Methods

- **`Move Method`**
  ```csharp
  public void Move(
WoWMovement.MovementDirection direction
)
  ```
  Moves.
  - *direction*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST1F887B8C_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWMovementAddLanguageSpecificTextSet("LST1F887B8C_3?cs=.|vb=.|cpp=::|nu=.|fs=.");MovementDirectionThe direction.

- **`MoveStop Method`**
  ```csharp
  public void MoveStop()
  ```
  Move stop.

- **`MoveTowards Method`**
  ```csharp
  public void MoveTowards(
Vector3 location
)
  ```
  Move towards.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LSTC2844F40_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3.

---

### Flightor Class

**Inheritance:** System.Object → Styx.Pathing.Flightor

```csharp
public static class Flightor
```

The flightor class.

#### Flightor Properties

- **`CanFly Property`**
  ```csharp
  public static bool CanFly { get; }
  ```
  Returns true if we can currently fly, or are in a flyable area and have a flying mount.

#### Flightor Methods

- **`Clear Method`**
  ```csharp
  public static void Clear()
  ```
  Clears the flight path and the poly nav.

- **`IsFlyableArea Method`**
  ```csharp
  public static bool IsFlyableArea()
  ```
  Gets a bool that indicates whether the current area is flyable.
  - **Returns:** ReferenceFlightor Class

- **`IsFlyableAreaOrChildOfFlyableArea Method`**
  ```csharp
  public static bool IsFlyableAreaOrChildOfFlyableArea()
  ```
  Checks whether this area or a parent area is flyable.
  - **Returns:** ReferenceFlightor Class

- **`MoveTo Method`**
  Move to by Flying Navigation.

- **`MoveTo Method (FlyToParameters)`**
  ```csharp
  public static MoveResult MoveTo(
FlyToParameters parameters
)
  ```
  Move to by Flying Navigation.
  - *parameters*: Type: Styx.PathingAddLanguageSpecificTextSet("LST995E8115_1?cs=.|vb=.|cpp=::|nu=.|fs=.");FlyToParametersThe FlyToParameters. FlyToParameters
  - **Returns:** ReferenceFlightor Class

- **`MoveTo Method (Vector3, Boolean)`**
  ```csharp
  public static void MoveTo(
Vector3 destination,
bool checkIndoors
)
  ```
  Move to.
  - *destination*: Type: System.NumericsAddLanguageSpecificTextSet("LSTBCC8214_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3 Destination for the.
  - *checkIndoors*: Type: SystemAddLanguageSpecificTextSet("LSTBCC8214_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Booleanthe check indoors.

- **`MoveTo Method (Vector3, Single, Boolean)`**
  ```csharp
  public static MoveResult MoveTo(
Vector3 destination,
float minHeight = 40f,
bool checkIndoors = false
)
  ```
  - *destination*: Type: System.NumericsAddLanguageSpecificTextSet("LSTA30267C1_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3
  - **Returns:** ReferenceFlightor Class

---

### Flightor.MountHelper Class

**Inheritance:** System.Object → Styx.Pathing.Flightor.MountHelper

```csharp
public static class MountHelper
```

A mount helper.

#### MountHelper Properties

- **`CanMount Property`**
  ```csharp
  public static bool CanMount { get; }
  ```
  Gets a value indicating whether we can mount.

- **`Mounted Property`**
  ```csharp
  public static bool Mounted { get; }
  ```
  Gets a value indicating whether we are mounted on a flying mount.

#### MountHelper Methods

- **`Dismount Method`**
  ```csharp
  [ObsoleteAttribute("Use CommonCoroutines.Dismount")]
public static void Dismount()
  ```
  Dismounts this object.

- **`MountUp Method`**
  ```csharp
  [ObsoleteAttribute("Use MountUpCoroutine. This will be removed in the future.")]
public static void MountUp()
  ```
  Mount up.

- **`MountUpCoroutine Method`**
  ```csharp
  public static Task&lt;bool&gt; MountUpCoroutine()
  ```
  Summons the flying mount.
  - **Returns:** See Also

---

### Flightor.MountHelper.DisMount Class

**Inheritance:** System.Object → Styx.TreeSharp.Composite → Styx.TreeSharp.Action → Styx.Pathing.Flightor.MountHelper.DisMount

```csharp
[ObsoleteAttribute("Use CommonCoroutines.Dismount")]
public class DisMount : Action
```

The dis mount.

#### DisMount Methods

- **`Run Method`**
  ```csharp
  protected override RunStatus Run(
Object context
)
  ```
  - *context*: Type: SystemAddLanguageSpecificTextSet("LSTFBE8719_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceFlightorAddLanguageSpecificTextSet("LSTFBE8719_4?cs=.|vb=.|cpp=::|nu=.|fs=.");MountHelperAddLanguageSpecificTextSet("LSTFBE8719_5?cs=.|vb=.|cpp=::|nu=.|fs=.");DisMount Class

---

### FlyToParameters Class

**Inheritance:** System.Object → Styx.Pathing.FlyToParameters

```csharp
public class FlyToParameters
```

Represents parameters for MoveTo(FlyToParameters).

#### FlyToParameters Constructor

- **`FlyToParameters Constructor`**
  ```csharp
  public FlyToParameters()
  ```
  Initializes a new instance of the FlyToParameters class

- **`FlyToParameters Constructor (Vector3)`**
  ```csharp
  public FlyToParameters(
Vector3 location
)
  ```
  Initializes a new instance of the FlyToParameters class
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LSTE52FCB9B_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3

#### FlyToParameters Properties

- **`CheckIndoors Property`**
  ```csharp
  public bool CheckIndoors { get; set; }
  ```
  Gets or sets a bool that indicates whether the flightor
            should check if the destination is indoors or not to land.

- **`GroundNavParameters Property`**
  ```csharp
  public MoveToParameters GroundNavParameters { get; }
  ```
  The MoveToParameters to be used 
            when Flightor falls back to ground navigation.

- **`Location Property`**
  ```csharp
  public Vector3 Location { get; set; }
  ```
  The location to move to.

- **`MinHeight Property`**
  ```csharp
  public float MinHeight { get; set; }
  ```
  The minimum height to stay from the ground.

#### FlyToParameters Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Gets a string representation of these parameters.
  - **Returns:** ReferenceFlyToParameters Class

---

### IPlayerMover Interface

```csharp
public interface IPlayerMover
```

Interface for player mover.

#### IPlayerMover Methods

- **`Move Method`**
  ```csharp
  void Move(
WoWMovement.MovementDirection direction
)
  ```
  Moves.
  - *direction*: Type: Styx.WoWInternalsAddLanguageSpecificTextSet("LST54803432_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWMovementAddLanguageSpecificTextSet("LST54803432_3?cs=.|vb=.|cpp=::|nu=.|fs=.");MovementDirectionThe direction.

- **`MoveStop Method`**
  ```csharp
  void MoveStop()
  ```
  Move stop.

- **`MoveTowards Method`**
  ```csharp
  void MoveTowards(
Vector3 location
)
  ```
  Moves towards a location.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST98BB086A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.

---

### MoveResult Enumeration

```csharp
public enum MoveResult
```

Values that represent MoveResult.

---

### MoveResultExtensions Class

**Inheritance:** System.Object → Styx.Pathing.MoveResultExtensions

```csharp
public static class MoveResultExtensions
```

Determines whether the specified move result is successful.

#### MoveResultExtensions Methods

- **`IsSuccessful Method`**
  ```csharp
  public static bool IsSuccessful(
this MoveResult moveResult
)
  ```
  Determines whether the specified move result is successful.
  - *moveResult*: Type: Styx.PathingAddLanguageSpecificTextSet("LST3F5068F0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");MoveResultThe move result.
  - **Returns:** See Also

---

### MoveToParameters Class

**Inheritance:** System.Object → Styx.Pathing.MoveToParameters

```csharp
public class MoveToParameters
```

Represents parameters for MoveTo(MoveToParameters).

#### MoveToParameters Constructor

- **`MoveToParameters Constructor`**
  ```csharp
  public MoveToParameters()
  ```
  Initializes a new instance of the MoveToParameters class

- **`MoveToParameters Constructor (Vector3)`**
  ```csharp
  public MoveToParameters(
Vector3 location
)
  ```
  Initializes a new instance of the MoveToParameters class
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LSTA2AABFAD_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3

#### MoveToParameters Properties

- **`AllowFlightPaths Property`**
  ```csharp
  public bool AllowFlightPaths { get; set; }
  ```
  Gets or sets a bool that indicates whether the navigator
            is allowed to use flight paths to get to the destination.

- **`AllowSurfaceSnappingWhileSwimming Property`**
  ```csharp
  [ObsoleteAttribute("Do not use. Will be removed in the future.")]
public bool AllowSurfaceSnappingWhileSwimming { get; set; }
  ```
  If true, the nav provider will be allowed to snap the start point upwards
            to the liquid surface if we are swimming.

- **`DistanceTolerance Property`**
  ```csharp
  public float DistanceTolerance { get; set; }
  ```
  The distance tolerance to get within Location.

- **`Location Property`**
  ```csharp
  public Vector3 Location { get; set; }
  ```
  The location to move to.

- **`MapId Property`**
  ```csharp
  public Nullable&lt;int&gt; MapId { get; set; }
  ```
  The map ID of the location.

- **`MoveImmediately Property`**
  ```csharp
  public bool MoveImmediately { get; set; }
  ```
  If true, instructs the nav provider to move immediately once the path has been generated,
            so the player will start moving at the same time as PathGenerated is returned.
            If false, instructs the nav provider to wait until the next movement request after the path generation
            has finished to begin moving. This is for example useful for inspecting the path if it succeeds
            and aborting the navigation if the path generated was too long.

#### MoveToParameters Methods

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Gets a string representation of these parameters.
  - **Returns:** ReferenceMoveToParameters Class

---

### NavigationProvider Class

**Inheritance:** System.Object → Styx.Pathing.NavigationProvider → Styx.Pathing.WrappingNavigationProvider

```csharp
public abstract class NavigationProvider
```

Initializes a new instance of the NavigationProvider class

#### NavigationProvider Properties

- **`IsCurrent Property`**
  ```csharp
  public bool IsCurrent { get; }
  ```
  Gets a bool that indicates whether this is the current navigation provider.

#### NavigationProvider Methods

- **`AtLocation Method`**
  ```csharp
  public abstract bool AtLocation(
Vector3 point1,
Vector3 point2
)
  ```
  Gets a bool that indicates whether one position is considered to be at another position.
            Can be used to check if the player has reached a destination, for example.
  - *point1*: Type: System.NumericsAddLanguageSpecificTextSet("LSTDFAE1952_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The first point.
  - *point2*: Type: System.NumericsAddLanguageSpecificTextSet("LSTDFAE1952_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The second point.
  - **Returns:** ReferenceNavigationProvider Class

- **`Clear Method`**
  ```csharp
  public virtual bool Clear()
  ```
  Clears the current path in this nav provider.
  - **Returns:** ReferenceNavigationProvider Class

- **`ClearStuckInfo Method`**
  ```csharp
  public virtual void ClearStuckInfo()
  ```
  Clears the stuck handler information.

- **`LookupPathInfo Method`**
  ```csharp
  public virtual PathInformation LookupPathInfo(
WoWObject obj,
float distanceTolerance = 3f
)
  ```
  Looks up the information of a path going to an object.
  - *obj*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST9BFF3760_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObjectThe object.
  - **Returns:** Exceptions

- **`MoveTo Method`**
  ```csharp
  public abstract MoveResult MoveTo(
MoveToParameters parameters
)
  ```
  Moves towards the specified locations.
  - *parameters*: Type: Styx.PathingAddLanguageSpecificTextSet("LSTC2462ED_1?cs=.|vb=.|cpp=::|nu=.|fs=.");MoveToParametersThe parameters.
  - **Returns:** This function should return instantly, and should be able to be called again to perform the next step of the movement.

- **`OnPulse Method`**
  ```csharp
  public abstract void OnPulse()
  ```
  Pulses this provider. Called every tick.

- **`OnRemoveAsCurrent Method`**
  ```csharp
  public virtual void OnRemoveAsCurrent()
  ```
  Called when this NavigationProvider is removed as the current by assigning a different provider to NavigationProvider.

- **`OnSetAsCurrent Method`**
  ```csharp
  public virtual void OnSetAsCurrent()
  ```
  Called when this NavigationProvider is set as the current by assigning it to NavigationProvider.

- **`SamplePointsAsync Method`**
  ```csharp
  public virtual Task&lt;List&lt;Vector3&gt;&gt; SamplePointsAsync(
SamplePointsParameters parameters
)
  ```
  Samples the navigator for points.
  - *parameters*: Type: Styx.PathingAddLanguageSpecificTextSet("LSTE0C6C817_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SamplePointsParametersThe parameters.
  - **Returns:** Exceptions

---

### NavigationProviderChangedEventArgs(T) Class

**Inheritance:** System.Object → System.EventArgs → Styx.Pathing.NavigationProviderChangedEventArgs.T

```csharp
public class NavigationProviderChangedEventArgs&lt;T&gt; : EventArgs
```

Additional information for navigator changed events.

#### NavigationProviderChangedEventArgs(T) Properties

- **`NewProvider Property`**
  ```csharp
  public T NewProvider { get; }
  ```
  Gets the new provider.

- **`OldProvider Property`**
  ```csharp
  public T OldProvider { get; }
  ```
  Gets the old provider.

---

### Navigator Class

**Inheritance:** System.Object → Styx.Pathing.Navigator

```csharp
public static class Navigator
```

Navigator.

#### Navigator Properties

- **`NavigationProvider Property`**
  ```csharp
  public static NavigationProvider NavigationProvider { get; set; }
  ```
  Gets or sets the navigation provider.

- **`PathPrecision Property`**
  ```csharp
  [ObsoleteAttribute("This property always returns 2 and cannot be set.")]
public static float PathPrecision { get; set; }
  ```
  Gets or sets the path precision.

- **`PlayerMover Property`**
  ```csharp
  public static IPlayerMover PlayerMover { get; set; }
  ```
  Gets or sets the player mover.

#### Navigator Methods

- **`AtLocation Method`**
  Checks if the local player is considered to be on top of a point.

- **`AtLocation Method (Vector3)`**
  ```csharp
  public static bool AtLocation(
Vector3 point
)
  ```
  Checks if the local player is considered to be on top of a point.
  - *point*: Type: System.NumericsAddLanguageSpecificTextSet("LSTA95ADCAF_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3.
  - **Returns:** ReferenceNavigator Class

- **`AtLocation Method (Vector3, Vector3)`**
  ```csharp
  public static bool AtLocation(
Vector3 point1,
Vector3 point2
)
  ```
  Checks if one point is considered to be at another point. This should be used to
            check if the player is near a point.
  - *point1*: Type: System.NumericsAddLanguageSpecificTextSet("LSTF263E928_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3.
  - *point2*: Type: System.NumericsAddLanguageSpecificTextSet("LSTF263E928_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3.
  - **Returns:** ExceptionConditionInvalidOperationExceptionThrown when the requested operation is invalid.

- **`Clear Method`**
  ```csharp
  public static bool Clear()
  ```
  Clears the current path to its blank/initial state.
  - **Returns:** ExceptionConditionInvalidOperationExceptionThrown when the requested operation is invalid.

- **`GetRunStatusFromMoveResult Method`**
  ```csharp
  public static RunStatus GetRunStatusFromMoveResult(
MoveResult moveResult
)
  ```
  Gets a run status from move result.
  - *moveResult*: Type: Styx.PathingAddLanguageSpecificTextSet("LST78DB80A0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");MoveResultThe move result.
  - **Returns:** ExceptionConditionArgumentOutOfRangeExceptionThrown when one or more arguments are outside the
            required range.

- **`LookupPathInfo Method`**
  ```csharp
  public static PathInformation LookupPathInfo(
WoWObject obj,
float distanceTolerance = 3f
)
  ```
  Looks up the information of a path going to an object.
  - *obj*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST51A3A05A_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObjectThe object.
  - **Returns:** Exceptions

- **`MoveTo Method`**
  Moves towards the specified location using the specified parameters.

- **`MoveTo Method (MoveToParameters)`**
  ```csharp
  public static MoveResult MoveTo(
MoveToParameters parameters
)
  ```
  Moves towards the specified location using the specified parameters.
  - *parameters*: Type: Styx.PathingAddLanguageSpecificTextSet("LSTB98EEAD9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");MoveToParametersThe parameters.
  - **Returns:** ExceptionConditionInvalidOperationExceptionThrown when the navigation provider is null.ArgumentNullExceptionparameters is null.ArgumentExceptionOne or more members of parameters is wrong.

- **`MoveTo Method (Vector3, Nullable(Int32))`**
  ```csharp
  public static MoveResult MoveTo(
Vector3 location,
Nullable&lt;int&gt; mapId = null
)
  ```
  Move towards the specified location.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LSTB441503C_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.
  - **Returns:** ExceptionConditionInvalidOperationExceptionThrown when the navigation provider is null.

- **`SamplePointsAsync Method`**
  ```csharp
  public static Task&lt;List&lt;Vector3&gt;&gt; SamplePointsAsync(
SamplePointsParameters parameters
)
  ```
  Samples the navigator for points.
  - *parameters*: Type: Styx.PathingAddLanguageSpecificTextSet("LST9914F793_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SamplePointsParametersThe parameters.
  - **Returns:** Exceptions

#### Navigator Events

- **`OnNavigationProviderChanged Event`**
  ```csharp
  public static event EventHandler&lt;NavigationProviderChangedEventArgs&lt;NavigationProvider&gt;&gt; OnNavigationProviderChanged
  ```
  Event queue for all listeners interested in OnNavigationProviderChanged events.

- **`OnPlayerMoverChanged Event`**
  ```csharp
  public static event EventHandler&lt;NavigationProviderChangedEventArgs&lt;IPlayerMover&gt;&gt; OnPlayerMoverChanged
  ```
  Event queue for all listeners interested in OnPlayerMoverChanged events.

---

### PathDistanceType Enumeration

```csharp
public enum PathDistanceType
```

Represents the different kinds of path distance values.

---

### PathGenerationFailStep Enumeration

```csharp
public enum PathGenerationFailStep
```

Values that represent PathGenerationFailStep.

---

### PathInformation Class

**Inheritance:** System.Object → Styx.Pathing.PathInformation

```csharp
public class PathInformation
```

Represents information about a path distance to an object.

#### PathInformation Properties

- **`Distance Property`**
  ```csharp
  public float Distance { get; }
  ```
  The distance value of the path. See DistanceType for how to interpret this.

- **`DistanceType Property`**
  ```csharp
  public PathDistanceType DistanceType { get; }
  ```
  The type of value the Distance propery indicates.

- **`Navigability Property`**
  ```csharp
  public PathNavigability Navigability { get; }
  ```
  Information about the navigability to the object.

---

### PathNavigability Enumeration

```csharp
public enum PathNavigability
```

Represents path information.

---

### SampleCircle Class

**Inheritance:** System.Object → Styx.Pathing.SampleCircle

```csharp
public class SampleCircle
```

Represents a circle (in 2D) to sample points within.

#### SampleCircle Properties

- **`Center Property`**
  ```csharp
  public Vector2 Center { get; }
  ```
  The center of the circle.

- **`Radius Property`**
  ```csharp
  public float Radius { get; }
  ```
  The radius of the circle.

---

### SamplePointsParameters Class

**Inheritance:** System.Object → Styx.Pathing.SamplePointsParameters

```csharp
public class SamplePointsParameters
```

Represents parameters for sampling points with a navigation provider.

#### SamplePointsParameters Properties

- **`Circles Property`**
  ```csharp
  public IReadOnlyList&lt;SampleCircle&gt; Circles { get; set; }
  ```
  The circles to sample points within.

- **`NavigabilityLocation Property`**
  ```csharp
  public Vector3 NavigabilityLocation { get; set; }
  ```
  The location that the sampled points must be navigable from
            or to, depending on Type.

- **`NavigableFromDestinationTolerance Property`**
  ```csharp
  public float NavigableFromDestinationTolerance { get; set; }
  ```
  When Type is NavigableFrom,
            specifies the distance we must be able to get within NavigabilityLocation
            from the sampled points.

- **`NumPoints Property`**
  ```csharp
  public int NumPoints { get; set; }
  ```
  The number of points to try to find.

- **`Type Property`**
  ```csharp
  public SamplePointsType Type { get; set; }
  ```
  The type of points to sample.

#### SamplePointsParameters Methods

- **`Any Method`**
  ```csharp
  public static SamplePointsParameters Any(
IEnumerable&lt;SampleCircle&gt; circles,
int numPoints
)
  ```
  Find any points in the specified circles.
  - *circles*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST89207418_1?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST89207418_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");SampleCircleAddLanguageSpecificTextSet("LST89207418_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The circles.
  - *numPoints*: Type: SystemAddLanguageSpecificTextSet("LST89207418_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The number of points to try to sample.
  - **Returns:** ReferenceSamplePointsParameters Class

- **`HighlyConnected Method`**
  ```csharp
  public static SamplePointsParameters HighlyConnected(
IEnumerable&lt;SampleCircle&gt; circles,
int numPoints
)
  ```
  Find points in the specified circles that are highly connected (i.e. with navigability
            to lots of other points).
  - *circles*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST11C9A6E4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST11C9A6E4_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");SampleCircleAddLanguageSpecificTextSet("LST11C9A6E4_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The circles to sample points in.
  - *numPoints*: Type: SystemAddLanguageSpecificTextSet("LST11C9A6E4_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The number of points to try to sample.
  - **Returns:** ReferenceSamplePointsParameters Class

- **`NavigableFrom Method`**
  ```csharp
  public static SamplePointsParameters NavigableFrom(
IEnumerable&lt;SampleCircle&gt; circles,
int numPoints,
Vector3 to,
float destinationTolerance = 3f
)
  ```
  Find points in the specified circles that can be navigated from to the specified location.
  - *circles*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST55120F05_1?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST55120F05_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");SampleCircleAddLanguageSpecificTextSet("LST55120F05_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The circles to sample points in.
  - *numPoints*: Type: SystemAddLanguageSpecificTextSet("LST55120F05_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The number of points to try to sample.
  - *to*: Type: System.NumericsAddLanguageSpecificTextSet("LST55120F05_5?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location we must be able to navigate to.
  - **Returns:** ReferenceSamplePointsParameters Class

- **`NavigableTo Method`**
  ```csharp
  public static SamplePointsParameters NavigableTo(
IEnumerable&lt;SampleCircle&gt; circles,
int numPoints,
Vector3 from
)
  ```
  Find points in the specified cirles that can be navigated to from the specified location.
  - *circles*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LST98CE1159_1?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LST98CE1159_2?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");SampleCircleAddLanguageSpecificTextSet("LST98CE1159_3?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The circles to sample points in.
  - *numPoints*: Type: SystemAddLanguageSpecificTextSet("LST98CE1159_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The number of points to try to sample.
  - *from*: Type: System.NumericsAddLanguageSpecificTextSet("LST98CE1159_5?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location we must be able to navigate from.
            Usually our own position.
  - **Returns:** ReferenceSamplePointsParameters Class

---

### SamplePointsType Enumeration

```csharp
public enum SamplePointsType
```

Represents the types of points to sample with a navigation provider.

---

### WrappingNavigationProvider Class

**Inheritance:** System.Object → Styx.Pathing.NavigationProvider → Styx.Pathing.WrappingNavigationProvider

```csharp
public class WrappingNavigationProvider : NavigationProvider
```

Provides a helper for wrapping a different navigation provider.

#### WrappingNavigationProvider Properties

- **`Original Property`**
  ```csharp
  public NavigationProvider Original { get; }
  ```
  The original nav provider, which is being wrapped.

#### WrappingNavigationProvider Methods

- **`AtLocation Method`**
  ```csharp
  public override bool AtLocation(
Vector3 point1,
Vector3 point2
)
  ```
  Gets a bool that indicates whether one position is considered to be at another position.
            Can be used to check if the player has reached a destination, for example.
  - *point1*: Type: System.NumericsAddLanguageSpecificTextSet("LST955B3796_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The first point.
  - *point2*: Type: System.NumericsAddLanguageSpecificTextSet("LST955B3796_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The second point.
  - **Returns:** ReferenceWrappingNavigationProvider Class

- **`Clear Method`**
  ```csharp
  public override bool Clear()
  ```
  Clears the current path in this nav provider.
  - **Returns:** ReferenceWrappingNavigationProvider Class

- **`ClearStuckInfo Method`**
  ```csharp
  public override void ClearStuckInfo()
  ```
  Clears the stuck handler information.

- **`LookupPathInfo Method`**
  ```csharp
  public override PathInformation LookupPathInfo(
WoWObject obj,
float distanceTolerance = 3f
)
  ```
  Looks up the information of a path going to an object.
  - *obj*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LST7CC66E4C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObjectThe object.
  - **Returns:** Exceptions

- **`MoveTo Method`**
  ```csharp
  public override MoveResult MoveTo(
MoveToParameters parameters
)
  ```
  Moves towards the specified locations.
  - *parameters*: Type: Styx.PathingAddLanguageSpecificTextSet("LSTB68BCE69_1?cs=.|vb=.|cpp=::|nu=.|fs=.");MoveToParametersThe parameters.
  - **Returns:** This function should return instantly, and should be able to be called again to perform the next step of the movement.

- **`OnPulse Method`**
  ```csharp
  public override void OnPulse()
  ```
  Pulses this provider. Called every tick.

- **`OnRemoveAsCurrent Method`**
  ```csharp
  public override void OnRemoveAsCurrent()
  ```
  Called when this NavigationProvider is removed as the current by assigning a different provider to NavigationProvider.

- **`OnSetAsCurrent Method`**
  ```csharp
  public override void OnSetAsCurrent()
  ```
  Called when this NavigationProvider is set as the current by assigning it to NavigationProvider.

- **`SamplePointsAsync Method`**
  ```csharp
  public override Task&lt;List&lt;Vector3&gt;&gt; SamplePointsAsync(
SamplePointsParameters parameters
)
  ```
  Samples the navigator for points.
  - *parameters*: Type: Styx.PathingAddLanguageSpecificTextSet("LST79BF7E93_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SamplePointsParametersThe parameters.
  - **Returns:** Exceptions

---
