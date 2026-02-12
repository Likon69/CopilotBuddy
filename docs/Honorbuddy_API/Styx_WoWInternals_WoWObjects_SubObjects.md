# Styx.WoWInternals.WoWObjects.SubObjects

## Contents

- [GarrisonShipmentState Enumeration](#garrisonshipmentstate-enumeration)
- [WoWGarrisonShipment Class](#wowgarrisonshipment-class)

---

### GarrisonShipmentState Enumeration

```csharp
public enum GarrisonShipmentState
```

---

### WoWGarrisonShipment Class

**Inheritance:** System.Object → Styx.WoWInternals.WoWObjects.WoWSubObject → Styx.WoWInternals.WoWObjects.WoWAnimatedSubObject → Styx.WoWInternals.WoWObjects.SubObjects.WoWGarrisonShipment

```csharp
public class WoWGarrisonShipment : WoWAnimatedSubObject
```

Gets the state of the animation.

#### WoWGarrisonShipment Properties

- **`ShipmentState Property`**
  ```csharp
  public GarrisonShipmentState ShipmentState { get; }
  ```
  Returns the current state of this garrison shipment object.

---
