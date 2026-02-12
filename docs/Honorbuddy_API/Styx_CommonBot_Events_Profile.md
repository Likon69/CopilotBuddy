# Styx.CommonBot.Events.Profile

## Contents

- [CodeCompositionEventArgs Class](#codecompositioneventargs-class)

---

### CodeCompositionEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.CommonBot.Events.Profile.CodeCompositionEventArgs

```csharp
public class CodeCompositionEventArgs : EventArgs
```

Adds the specified xmlObject for code composition.All public properties of type DelayCompiledExpression with a CompileExpressionAttribute attached and all public properties of type Stringwith a CompileStringAttribute attached in xmlObjectare added to a CompileBatch and compiled into one assembly.All public properties of type IXmlObject or collections of IXmlObjectin xmlObject are also added recursively.

#### CodeCompositionEventArgs Properties

- **`Profile Property`**
  ```csharp
  public Profile Profile { get; }
  ```

#### CodeCompositionEventArgs Methods

- **`Add Method`**
  ```csharp
  public void Add(
IXmlObject xmlObject
)
  ```
  Adds the specified xmlObject for code composition.All public properties of type DelayCompiledExpression with a CompileExpressionAttribute attached and all public properties of type Stringwith a CompileStringAttribute attached in xmlObjectare added to a CompileBatch and compiled into one assembly.All public properties of type IXmlObject or collections of IXmlObjectin xmlObject are also added recursively.
  - *xmlObject*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LST5089A2C8_1?cs=.|vb=.|cpp=::|nu=.|fs=.");IXmlObjectThe XML object.

---
