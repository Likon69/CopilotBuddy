# Styx.XmlEngine

Contains XML engine related classes.

## Contents

- [INamedAttribute Interface](#inamedattribute-interface)
- [XmlAttributeAttribute Class](#xmlattributeattribute-class)
- [XmlElementAttribute Class](#xmlelementattribute-class)
- [XmlEngine Class](#xmlengine-class)

---

### INamedAttribute Interface

```csharp
public interface INamedAttribute
```

Interface for named attribute.

#### INamedAttribute Properties

- **`Name Property`**
  ```csharp
  string Name { get; set; }
  ```
  Gets or sets the name.

---

### XmlAttributeAttribute Class

**Inheritance:** System.Object → System.Attribute → Styx.XmlEngine.XmlAttributeAttribute

```csharp
public sealed class XmlAttributeAttribute : Attribute,
INamedAttribute
```

Represents a property that should be loaded from an XML attribute.

#### XmlAttributeAttribute Properties

- **`Name Property`**
  ```csharp
  public string Name { get; set; }
  ```
  Gets or sets the name.

- **`ShouldSave Property`**
  ```csharp
  public bool ShouldSave { get; set; }
  ```
  Gets or sets a value indicating whether we should save.

---

### XmlElementAttribute Class

**Inheritance:** System.Object → System.Attribute → Styx.XmlEngine.XmlElementAttribute

```csharp
public sealed class XmlElementAttribute : Attribute,
INamedAttribute
```

Attribute for XML element.

#### XmlElementAttribute Properties

- **`Name Property`**
  ```csharp
  public string Name { get; set; }
  ```
  Gets or sets the name.

---

### XmlEngine Class

**Inheritance:** System.Object → Styx.XmlEngine.XmlEngine

```csharp
public class XmlEngine
```

The main engine for loading and saving XML files to .NET objects.

#### XmlEngine Properties

- **`TypeMap Property`**
  ```csharp
  public Dictionary&lt;string, Type&gt; TypeMap { get; }
  ```
  Gets the current XML element type mapping.

#### XmlEngine Methods

- **`Load Method`**
  ```csharp
  public void Load(
Object obj,
XElement element
)
  ```
  Reads the XML specified and fills the properties of the passed class with the XML
            values.
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LSTB8FFCEDA_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object    .
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTB8FFCEDA_2?cs=.|vb=.|cpp=::|nu=.|fs=.");XElement.

- **`Save Method`**
  ```csharp
  public XElement Save(
Object obj,
string namespace = null,
XElement parentElement = null
)
  ```
  Saves an object to XML. Optionally with a namespace. All properties and child
            elements will be saved as well.
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LST1B3230C5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object          .
  - **Returns:** ReferenceXmlEngine Class

#### XmlEngine Fields

- **`GENERIC_BODY Field`**
  ```csharp
  public const string GENERIC_BODY = "_XMLENGINE_BODY_"
  ```
  A generic bodied XML element name. Use this if you intend to support fairly complex
            XML elements that can hold any sub-element.

---
