# Styx.CommonBot.Profiles

Contains profile related classes.

## Contents

- [Blackspot Structure](#blackspot-structure)
- [CompileExpressionAttribute Class](#compileexpressionattribute-class)
- [CompileStringAttribute Class](#compilestringattribute-class)
- [CustomBehaviorFileNameAttribute Class](#custombehaviorfilenameattribute-class)
- [CustomForcedBehavior Class](#customforcedbehavior-class)
- [CustomForcedBehavior.ConstrainAs Class](#customforcedbehavior.constrainas-class)
- [CustomForcedBehavior.ConstrainTo Class](#customforcedbehavior.constrainto-class)
- [CustomForcedBehavior.ConstrainTo.Anything(T) Class](#customforcedbehavior.constrainto.anythingt-class)
- [CustomForcedBehavior.ConstrainTo.Domain(T) Class](#customforcedbehavior.constrainto.domaint-class)
- [CustomForcedBehavior.ConstrainTo.NonEmptyString(T) Class](#customforcedbehavior.constrainto.nonemptystringt-class)
- [CustomForcedBehavior.ConstrainTo.NonEmptyVector3(T) Class](#customforcedbehavior.constrainto.nonemptyvector3t-class)
- [CustomForcedBehavior.ConstrainTo.QuestId(T) Class](#customforcedbehavior.constrainto.questidt-class)
- [CustomForcedBehavior.ConstrainTo.SpecificValues(T) Class](#customforcedbehavior.constrainto.specificvaluest-class)
- [CustomForcedBehavior.IConstraintChecker(T) Class](#customforcedbehavior.iconstraintcheckert-class)
- [CustomForcedBehavior.QuestCompleteRequirement Enumeration](#customforcedbehavior.questcompleterequirement-enumeration)
- [CustomForcedBehavior.QuestInLogRequirement Enumeration](#customforcedbehavior.questinlogrequirement-enumeration)
- [ForceMailManager Class](#forcemailmanager-class)
- [HotspotCollection Class](#hotspotcollection-class)
- [IXmlObject Interface](#ixmlobject-interface)
- [Mailbox Class](#mailbox-class)
- [MailboxManager Class](#mailboxmanager-class)
- [Profile Class](#profile-class)
- [ProfileAttributeExpectedException Class](#profileattributeexpectedexception-class)
- [ProfileAttributeExpectedException(T) Class](#profileattributeexpectedexceptiont-class)
- [ProfileException Class](#profileexception-class)
- [ProfileManager Class](#profilemanager-class)
- [ProfileMissingAttributeException Class](#profilemissingattributeexception-class)
- [ProfileMissingAttributeException(T) Class](#profilemissingattributeexceptiont-class)
- [ProfileMissingElementException Class](#profilemissingelementexception-class)
- [ProfileNotFoundException Class](#profilenotfoundexception-class)
- [ProfileTagExpectedException Class](#profiletagexpectedexception-class)
- [ProfileUnknownAttributeException Class](#profileunknownattributeexception-class)
- [ProfileUnknownElementException Class](#profileunknownelementexception-class)
- [ProtectedItemsManager Class](#protecteditemsmanager-class)
- [UnknownProfileElementEventArgs Class](#unknownprofileelementeventargs-class)
- [Vendor Class](#vendor-class)
- [Vendor.VendorType Enumeration](#vendor.vendortype-enumeration)
- [VendorManager Class](#vendormanager-class)
- [VendorTypeExtensions Class](#vendortypeextensions-class)

---

### Blackspot Structure

```csharp
public struct Blackspot : IEquatable&lt;Blackspot&gt;
```

A blackspot.

#### Blackspot Constructor

- **`Blackspot Constructor (XElement)`**
  ```csharp
  public Blackspot(
XElement element
)
  ```
  Constructor.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTAE0721FA_0?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.

- **`Blackspot Constructor (Vector3, Single, Single, String, Int32)`**
  ```csharp
  public Blackspot(
Vector3 location,
float radius,
float height,
string name = "",
int mapID = -1
)
  ```
  Constructor.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST8A6013C0_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.
  - *radius*: Type: SystemAddLanguageSpecificTextSet("LST8A6013C0_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe radius.
  - *height*: Type: SystemAddLanguageSpecificTextSet("LST8A6013C0_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SingleThe height.

#### Blackspot Properties

- **`Height Property`**
  ```csharp
  public float Height { get; }
  ```
  The height.

- **`Location Property`**
  ```csharp
  public Vector3 Location { get; }
  ```
  The location.

- **`MapID Property`**
  ```csharp
  public int MapID { get; }
  ```
  The map ID where this blackspot should be applied, or -1, for all maps.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  The name.

- **`Radius Property`**
  ```csharp
  public float Radius { get; }
  ```
  The radius.

#### Blackspot Methods

- **`Equals Method`**

- **`Equals Method (Object)`**
  ```csharp
  public override bool Equals(
Object obj
)
  ```
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LSTDE35B618_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceBlackspot Structure

- **`Equals Method (Blackspot)`**
  ```csharp
  public bool Equals(
Blackspot other
)
  ```
  - *other*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LSTF1CCE77B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Blackspot
  - **Returns:** See Also

- **`GetHashCode Method`**
  ```csharp
  public override int GetHashCode()
  ```
  - **Returns:** ReferenceBlackspot Structure

---

### CompileExpressionAttribute Class

**Inheritance:** System.Object → System.Attribute → Styx.CommonBot.Profiles.CompileExpressionAttribute

```csharp
public class CompileExpressionAttribute : Attribute
```

Instructs the Profile code compilation engine to compile an expression contained in a DelayCompiledExpression property

---

### CompileStringAttribute Class

**Inheritance:** System.Object → System.Attribute → Styx.CommonBot.Profiles.CompileStringAttribute

```csharp
public class CompileStringAttribute : Attribute
```

Instructs the Profile code compilation engine to compile an expression contained in a String property

---

### CustomBehaviorFileNameAttribute Class

**Inheritance:** System.Object → System.Attribute → Styx.CommonBot.Profiles.CustomBehaviorFileNameAttribute

```csharp
public sealed class CustomBehaviorFileNameAttribute : Attribute
```

Attribute for custom behavior file name.

#### CustomBehaviorFileNameAttribute Properties

- **`FileName Property`**
  ```csharp
  public string FileName { get; }
  ```
  Gets the filename of the file.

---

### CustomForcedBehavior Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.CustomForcedBehavior

```csharp
public abstract class CustomForcedBehavior : IXmlObject
```

Custom forced behavior.

#### CustomForcedBehavior Properties

- **`Args Property`**
  ```csharp
  public Dictionary&lt;string, string&gt; Args { get; }
  ```
  Gets the arguments.

- **`Branch Property`**
  ```csharp
  public Composite Branch { get; }
  ```
  Gets the branch.

- **`Element Property`**
  ```csharp
  public XElement Element { get; set; }
  ```
  Gets or sets the element.

- **`IsAttributeProblem Property`**
  ```csharp
  public bool IsAttributeProblem { get; protected set; }
  ```
  Gets a value indicating whether this object is attribute problem.

- **`IsDone Property`**
  ```csharp
  public abstract bool IsDone { get; }
  ```
  Gets a value indicating whether this object is done.

- **`NavType Property`**
  ```csharp
  public virtual Nullable&lt;NavType&gt; NavType { get; }
  ```
  Gets or sets the type of the navigation.

- **`SubversionId Property`**
  ```csharp
  [ObsoleteAttribute("Use VersionId")]
public virtual string SubversionId { get; }
  ```
  DON'T EDIT THESE--they are auto-populated by Subversion.

- **`SubversionRevision Property`**
  ```csharp
  [ObsoleteAttribute("Use VersionId")]
public virtual string SubversionRevision { get; }
  ```
  Gets the subversion revision.

- **`VersionId Property`**
  ```csharp
  public virtual string VersionId { get; }
  ```
  Gets the version ID. This version is displayed in the log
            as a way to identify the version of the custom behavior.

#### CustomForcedBehavior Methods

- **`CreateBehavior Method`**
  ```csharp
  protected virtual Composite CreateBehavior()
  ```
  - **Returns:** ReferenceCustomForcedBehavior Class

- **`Dispose Method`**
  ```csharp
  [ObsoleteAttribute("Use OnFinished instead.")]
public virtual void Dispose()
  ```
  Called right before this behavior is removed as the current behavior.

- **`Execute Method`**
  ```csharp
  protected virtual bool Execute()
  ```
  - **Returns:** ReferenceCustomForcedBehavior Class

- **`GetAttributeAs(T) Method`**
  ```csharp
  public T GetAttributeAs&lt;T&gt;(
string attributeName,
bool isAttributeRequired,
CustomForcedBehavior.IConstraintChecker&lt;T&gt; constraints,
string[] attributeNameAliases
)
where T : class
  ```
  This class is for use only with reference-types (e.g., string).
            GetAttributeAsNullable.T(String, Boolean, CustomForcedBehavior.IConstraintChecker.T, .String)
            is the corresponding method for use with value-types (e.g, double, int, Vector3).
            -----Examines the argument list looking for the specified attributeName,
            and returns the parsed representation of the attribute's value.  The attribute may also be
            found as one of the aliases provided in attributeNameAliases.
  - *attributeName*: Type: SystemAddLanguageSpecificTextSet("LSTF65871B5_11?cs=.|vb=.|cpp=::|nu=.|fs=.");String       the name of the attribute for which we look in the
            argument list. This value may not be null, or the empty string ("").
  - *isAttributeRequired*: Type: SystemAddLanguageSpecificTextSet("LSTF65871B5_12?cs=.|vb=.|cpp=::|nu=.|fs=.");Boolean true, if the attribute is required; false, if the
            attribute is optional.
  - *constraints*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LSTF65871B5_13?cs=.|vb=.|cpp=::|nu=.|fs=.");CustomForcedBehaviorAddLanguageSpecificTextSet("LSTF65871B5_14?cs=.|vb=.|cpp=::|nu=.|fs=.");IConstraintCheckerAddLanguageSpecificTextSet("LSTF65871B5_15?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LSTF65871B5_16?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");         A method that further constrains the input beyond its
            native type. If the input should not be constrained beyond its native type, supply
            null for this parameter.
            There are a number of preset
            constraints provided by the CustomForcedBehaviorAddLanguageSpecificTextSet("LSTF65871B5_17?cs=.|vb=.|cpp=::|nu=.|fs=.");ConstrainAs class.  There are also a number of
            prebuilt constraint classes that implement the CustomForcedBehaviorAddLanguageSpecificTextSet("LSTF65871B5_18?cs=.|vb=.|cpp=::|nu=.|fs=.");IConstraintCheckerAddLanguageSpecificTextSet("LSTF65871B5_19?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LSTF65871B5_20?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); interface--
            these prebuilt classes begin with the name "ConstrainTo".
  - *attributeNameAliases*: Type: AddLanguageSpecificTextSet("LSTF65871B5_21?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTF65871B5_22?cs=.|vb=.|cpp=::|nu=.|fs=.");StringAddLanguageSpecificTextSet("LSTF65871B5_23?cpp=&gt;|vb=()|nu=[]");an array of alternate names that the attribute is known
            as.  The primary purpose of this field is for backward-compatibility when attributes get
            renamed.  If there are no other names by which the attribute is known, supply null for
            this parameter.
  - *T*: Generic type parameter.
  - **Returns:** Remarks

- **`GetAttributeAsArray(T) Method`**
  ```csharp
  public T[] GetAttributeAsArray&lt;T&gt;(
string attributeName,
bool isAttributeRequired,
CustomForcedBehavior.IConstraintChecker&lt;T&gt; constraints,
string[] attributeNameAliases,
char[] separatorCharacters
)
  ```
  This method is primarily to support backward-compatibility.  Constructing data
            that requires the use of this method is un-XML-like, and its use is discouraged.-----Examines the argument list looking for the specified attributeName,
            and returns the parsed representation of the attribute's value.  The attribute may also be
            found as one of the aliases provided in attributeNameAliases.The value of the located argument is expected to be in the form of a delineated
            composite.
            If the separatorCharacters parameter is provided as null, the separator
            characters default to space, comma, and semicolon.For example, an integer list could be formatted as follows:
            "1,6,3,1,2,-1", "1 6 3 1 2 -1", or "1; 6; 3; 1; 2; -1".  Since the method is a generic, you
            may also use for other data types such as boolean (e.g., "true,false,true,true"),
            enumerations (e.g., "Black; Blue; DarkRed;
            White"), and other types.Each argument in the composite must adhere to any constraint that is provided by the
            caller.Although this method can also be used for strings, the comma/space/semicolon as
            separation characters,
            typically makes the returned results difficult to use.  When string are employed, the caller
            may need to supply a non-null value for separatorCharacters that is chosen
            judiciously.If &lt;T&gt; is a Vector3, the the separatorCharacters are ignored,
            and the attribute
            value must follow a rigid format.  Vector3s are formatted in one of the following ways: "X1,
            Y1,Z1|X2,Y2,Z2|X3." or "X1,Y1,Z1;X2,Y2,Z2;X3.".
  - *attributeName*: Type: SystemAddLanguageSpecificTextSet("LSTFBA754D1_4?cs=.|vb=.|cpp=::|nu=.|fs=.");String       the name of the attribute for which we look in the
            argument list. This value may not be null, or the empty string ("").
  - *isAttributeRequired*: Type: SystemAddLanguageSpecificTextSet("LSTFBA754D1_5?cs=.|vb=.|cpp=::|nu=.|fs=.");Boolean true, if the attribute is required; false, if the
            attribute is optional.
  - *constraints*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LSTFBA754D1_6?cs=.|vb=.|cpp=::|nu=.|fs=.");CustomForcedBehaviorAddLanguageSpecificTextSet("LSTFBA754D1_7?cs=.|vb=.|cpp=::|nu=.|fs=.");IConstraintCheckerAddLanguageSpecificTextSet("LSTFBA754D1_8?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LSTFBA754D1_9?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");         A method that further constrains the input beyond its
            native type. If the input should not be constrained beyond its native type, supply
            null for this parameter.
            There are a number of preset
            constraints provided by the CustomForcedBehaviorAddLanguageSpecificTextSet("LSTFBA754D1_10?cs=.|vb=.|cpp=::|nu=.|fs=.");ConstrainAs class.  There are also a number of
            prebuilt constraint classes that implement the CustomForcedBehaviorAddLanguageSpecificTextSet("LSTFBA754D1_11?cs=.|vb=.|cpp=::|nu=.|fs=.");IConstraintCheckerAddLanguageSpecificTextSet("LSTFBA754D1_12?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LSTFBA754D1_13?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); interface--
            these prebuilt classes begin with the name "ConstrainTo".
  - *attributeNameAliases*: Type: AddLanguageSpecificTextSet("LSTFBA754D1_14?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTFBA754D1_15?cs=.|vb=.|cpp=::|nu=.|fs=.");StringAddLanguageSpecificTextSet("LSTFBA754D1_16?cpp=&gt;|vb=()|nu=[]");an array of alternate names that the attribute is known
            as.  The primary purpose of this field is for backward-compatibility when attributes get
            renamed.  If there are no other names by which the attribute is known, supply null for
            this parameter.
  - *separatorCharacters*: Type: AddLanguageSpecificTextSet("LSTFBA754D1_17?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTFBA754D1_18?cs=.|vb=.|cpp=::|nu=.|fs=.");CharAddLanguageSpecificTextSet("LSTFBA754D1_19?cpp=&gt;|vb=()|nu=[]"); an array of characters that will be used as separators to
            delineate the attribute's value into array members. Note: if &lt;T&gt; is Vector3,
            then the separator characters are ignored, and the argument list must adhere to a specific
            format.
  - *T*: Generic type parameter.
  - **Returns:** Remarks

- **`GetAttributeAsNullable(T) Method`**
  ```csharp
  public Nullable&lt;T&gt; GetAttributeAsNullable&lt;T&gt;(
string attributeName,
bool isAttributeRequired,
CustomForcedBehavior.IConstraintChecker&lt;T&gt; constraints,
string[] attributeNameAliases
)
where T : struct, new()
  ```
  This class is for use only with value-types (e.g., double, int, Vector3).
            GetAttributeAs.T(String, Boolean, CustomForcedBehavior.IConstraintChecker.T, .String)
            is the corresponding method for use with reference-types (e.g, string).
            -----Examines the argument list looking for the specified attributeName,
            and returns the parsed representation of the attribute's value.  The attribute may also be
            found as one of the aliases provided in attributeNameAliases.
  - *attributeName*: Type: SystemAddLanguageSpecificTextSet("LST44211096_11?cs=.|vb=.|cpp=::|nu=.|fs=.");String       the name of the attribute for which we look in the
            argument list. This value may not be null, or the empty string ("").
  - *isAttributeRequired*: Type: SystemAddLanguageSpecificTextSet("LST44211096_12?cs=.|vb=.|cpp=::|nu=.|fs=.");Boolean true, if the attribute is required; false, if the
            attribute is optional.
  - *constraints*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LST44211096_13?cs=.|vb=.|cpp=::|nu=.|fs=.");CustomForcedBehaviorAddLanguageSpecificTextSet("LST44211096_14?cs=.|vb=.|cpp=::|nu=.|fs=.");IConstraintCheckerAddLanguageSpecificTextSet("LST44211096_15?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST44211096_16?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");         A method that further constrains the input beyond its
            native type. If the input should not be constrained beyond its native type, supply
            null for this parameter.
            There are a number of preset
            constraints provided by the CustomForcedBehaviorAddLanguageSpecificTextSet("LST44211096_17?cs=.|vb=.|cpp=::|nu=.|fs=.");ConstrainAs class.  There are also a number of
            prebuilt constraint classes that implement the CustomForcedBehaviorAddLanguageSpecificTextSet("LST44211096_18?cs=.|vb=.|cpp=::|nu=.|fs=.");IConstraintCheckerAddLanguageSpecificTextSet("LST44211096_19?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LST44211096_20?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); interface--
            these prebuilt classes begin with the name "ConstrainTo".
  - *attributeNameAliases*: Type: AddLanguageSpecificTextSet("LST44211096_21?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST44211096_22?cs=.|vb=.|cpp=::|nu=.|fs=.");StringAddLanguageSpecificTextSet("LST44211096_23?cpp=&gt;|vb=()|nu=[]");an array of alternate names that the attribute is known
            as.  The primary purpose of this field is for backward-compatibility when attributes get
            renamed.  If there are no other names by which the attribute is known, supply null for
            this parameter.
  - *T*: Generic type parameter.
  - **Returns:** Remarks

- **`GetNumberedAttributesAsArray(T) Method`**
  ```csharp
  public T[] GetNumberedAttributesAsArray&lt;T&gt;(
string baseName,
int countRequired,
CustomForcedBehavior.IConstraintChecker&lt;T&gt; constraints,
string[] aliasBaseNames
)
  ```
  This method looks for attributes with a prefix specified as
            baseName
            followed by an integer value (e.g., ButtonPhrase1, ButtonPhrase2, ButtonPhrase3). Such
            located parameters are collected and returned as an array.  An argument consisting of just
            the prefix and no following number (e.g., ButtonPhrase) is considered as part of the
            acceptable set of parameters found.  An argument that does not fit the form of prefix-
            followed-by-number will be ignored (e.g., ButtonPhrase3n, ButtonPhrases, etc). Attributes are
            also looked for under prefixes specified by aliasBaseNamesAny found arguments must adhere to the constraints specified by the
            caller.The countRequired argument determines how many of the similarly-
            named attributes
            must be found; otherwise, an error is reported and an empty array is returned.
            If &lt;T&gt; is a Vector3, then three parameters will be sought, one parameter each that
            identifies the X, Y, and Z contribution.  If any of these three parameters doesn't exist,
            then this is considered an error in the argument list.
  - *baseName*: Type: SystemAddLanguageSpecificTextSet("LST997DDAB7_4?cs=.|vb=.|cpp=::|nu=.|fs=.");String      a prefix used for the primary name of the arguments sought.
  - *countRequired*: Type: SystemAddLanguageSpecificTextSet("LST997DDAB7_5?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32 the number of similarly-named arguments that must be encountered
            for success.
  - *constraints*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LST997DDAB7_6?cs=.|vb=.|cpp=::|nu=.|fs=.");CustomForcedBehaviorAddLanguageSpecificTextSet("LST997DDAB7_7?cs=.|vb=.|cpp=::|nu=.|fs=.");IConstraintCheckerAddLanguageSpecificTextSet("LST997DDAB7_8?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");TAddLanguageSpecificTextSet("LST997DDAB7_9?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");   A method that further constrains the input beyond its native
            type. If the input should not be constrained beyond its native type, supply null for
            this parameter.
            There are a number of preset
            constraints provided by the CustomForcedBehaviorAddLanguageSpecificTextSet("LST997DDAB7_10?cs=.|vb=.|cpp=::|nu=.|fs=.");ConstrainAs class.  There are also a number of
            prebuilt constraint classes that implement the CustomForcedBehaviorAddLanguageSpecificTextSet("LST997DDAB7_11?cs=.|vb=.|cpp=::|nu=.|fs=.");IConstraintCheckerAddLanguageSpecificTextSet("LST997DDAB7_12?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LST997DDAB7_13?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); interface--
            these prebuilt classes begin with the name "ConstrainTo".
  - *aliasBaseNames*: Type: AddLanguageSpecificTextSet("LST997DDAB7_14?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST997DDAB7_15?cs=.|vb=.|cpp=::|nu=.|fs=.");StringAddLanguageSpecificTextSet("LST997DDAB7_16?cpp=&gt;|vb=()|nu=[]");alternative prefixes under which to look for acceptable
            attributes.
  - *T*: Generic type parameter.
  - **Returns:** Remarks

- **`LogMessage Method`**
  Convenience wrapper around
            LogMessage(String, Nullable.Color, String, .Object)

- **`LogMessage Method (String, String, Object[])`**
  ```csharp
  public void LogMessage(
string messageType,
string format,
params Object[] args
)
  ```
  Convenience wrapper around
            LogMessage(String, Nullable.Color, String, .Object)
  - *messageType*: Type: SystemAddLanguageSpecificTextSet("LSTF7F7AAFD_7?cs=.|vb=.|cpp=::|nu=.|fs=.");String.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LSTF7F7AAFD_8?cs=.|vb=.|cpp=::|nu=.|fs=.");String     .
  - *args*: Type: AddLanguageSpecificTextSet("LSTF7F7AAFD_9?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTF7F7AAFD_10?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LSTF7F7AAFD_11?cpp=&gt;|vb=()|nu=[]");       .

- **`LogMessage Method (String, Nullable(Color), String, Object[])`**
  ```csharp
  public void LogMessage(
string messageType,
Nullable&lt;Color&gt; messageColor,
string format,
params Object[] args
)
  ```
  This utility method issues a message (comprised of FORMAT and ARGS) to the Styx logging facilities.The message will be prefixed with information clearly identifying the behavior in the following form:
            [BEHAVIOR_NAME-vVERNUM(MESSAGETYPE) @line 647]: MESSAGEIf MESSAGECOLOR is provided, the message will be rendered in the requested color.
            If MESSAGECOLOR is null, the color is selected based on MESSAGETYPE:* MESSAGETYPE="fatal": (Red) goes to normal log and HONORBUDDY is
            STOPPED* MESSAGETYPE="error": (Red) goes to normal
            log* MESSAGETYPE="warning": (DarkOrange) goes to normal
            log* MESSAGETYPE="info": (CornflowerBlue) goes to normal
            log* MESSAGETYPE="debug": (LimeGreen) goes to DEBUG
            log* MESSAGETYPE=anything else: (DarkGray) goes to normal
            log
  - *messageType*: Type: SystemAddLanguageSpecificTextSet("LSTCD044D78_5?cs=.|vb=.|cpp=::|nu=.|fs=.");String assumes one of the following values: "fatal", "error", "warning",
            "info", "debug". Any other value is allowed, however the message will be not be highlighted
            for the user's attention. A messageType of "fatal" will also stop the profile. A messageType
            of "debug" will be emitted to the Debug Log instead of the 'normal' log.
  - *messageColor*: Type: SystemAddLanguageSpecificTextSet("LSTCD044D78_6?cs=.|vb=.|cpp=::|nu=.|fs=.");NullableAddLanguageSpecificTextSet("LSTCD044D78_7?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");ColorAddLanguageSpecificTextSet("LSTCD044D78_8?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The message color.
  - *format*: Type: SystemAddLanguageSpecificTextSet("LSTCD044D78_9?cs=.|vb=.|cpp=::|nu=.|fs=.");String      format spec of the message that should be logged.  This works the
            same as the format spec provided to string.Format().
  - *args*: Type: AddLanguageSpecificTextSet("LSTCD044D78_10?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LSTCD044D78_11?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectAddLanguageSpecificTextSet("LSTCD044D78_12?cpp=&gt;|vb=()|nu=[]");        arguments to be given to the format spec to build complete
            message.

- **`OnFinished Method`**
  ```csharp
  public virtual void OnFinished()
  ```
  Called when the behavior is finished, that is when moving to the next behavior. Also
            called when the bot stops.

- **`OnStart Method`**
  ```csharp
  public virtual void OnStart()
  ```
  Called right after this behavior has been set as the current behavior.

- **`OnStart_HandleAttributeProblem Method`**
  ```csharp
  public void OnStart_HandleAttributeProblem()
  ```
  This should be the first method called in a concrete CustomForcedBehavior's
            OnStart() method. When this is called, it will emit error messages that have occurred due to
            argument processing. The messages sent to the Styx log indicate which line of the profile is
            generating the problem.If there was a problem with argument processing, this method arranges for Honorbudy to
            cease
            executing the profile.This method also informs the user if the behavior will be skipped because the completion
            criteria were met when the behavior was called.

- **`OnTick Method`**
  ```csharp
  public virtual void OnTick()
  ```
  Called when the behavior tree is ticked.

- **`UtilIsProgressRequirementsMet Method`**
  ```csharp
  public bool UtilIsProgressRequirementsMet(
int questId,
CustomForcedBehavior.QuestInLogRequirement questInLogRequirement,
CustomForcedBehavior.QuestCompleteRequirement questCompleteRequirement
)
  ```
  Determine whether a behavior should start or continue based on the QuestId, and its
            required presence and completion criteria.
  - *questId*: Type: SystemAddLanguageSpecificTextSet("LSTBD9CB480_3?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32                 provides the reference for which the specified
            qualifies should be applied.
  - *questInLogRequirement*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LSTBD9CB480_4?cs=.|vb=.|cpp=::|nu=.|fs=.");CustomForcedBehaviorAddLanguageSpecificTextSet("LSTBD9CB480_5?cs=.|vb=.|cpp=::|nu=.|fs=.");QuestInLogRequirement   the QuestId must meet this specified qualifier for
            the behavior to proceed.
  - *questCompleteRequirement*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LSTBD9CB480_6?cs=.|vb=.|cpp=::|nu=.|fs=.");CustomForcedBehaviorAddLanguageSpecificTextSet("LSTBD9CB480_7?cs=.|vb=.|cpp=::|nu=.|fs=.");QuestCompleteRequirementthe QuestId must mee this specified qualifier for the
            behavior to proceed.
  - **Returns:** ReferenceCustomForcedBehavior Class

---

### CustomForcedBehavior.ConstrainAs Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.CustomForcedBehavior.ConstrainAs

```csharp
public static class ConstrainAs
```

Constraints can be applied to &lt;see cref="GetAttributeAs&lt;T&gt;(.)"/&gt;,
            &lt;see cref="GetAttributeAsArray&lt;T&gt;(.)"/&gt;, &lt;see
            cref="GetAttributeAsNullable&lt;T&gt;(.)"/&gt;, or &lt;see
            cref="GetNumberedAttributesAsArray&lt;T&gt;(.)"/&gt; methods to constrain the input to
            ranges, or sets of selected values.

#### ConstrainAs Methods

- **`QuestId Method`**
  ```csharp
  public static CustomForcedBehavior.IConstraintChecker&lt;int&gt; QuestId(
CustomForcedBehavior cfb
)
  ```
  Domain: [1.int.MaxValue]
  - *cfb*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LSTA5AA5514_3?cs=.|vb=.|cpp=::|nu=.|fs=.");CustomForcedBehaviorThe cfb.
  - **Returns:** See Also

#### ConstrainAs Fields

- **`AuraId Field`**
  ```csharp
  public static CustomForcedBehavior.IConstraintChecker&lt;int&gt; AuraId
  ```
  Domain: [1.int.MaxValue]

- **`CollectionCount Field`**
  ```csharp
  public static CustomForcedBehavior.IConstraintChecker&lt;int&gt; CollectionCount
  ```
  Domain: [1.1000]

- **`FactionId Field`**
  ```csharp
  public static CustomForcedBehavior.IConstraintChecker&lt;int&gt; FactionId
  ```
  Domain: [1.int.MaxValue]

- **`HotbarButton Field`**
  ```csharp
  public static CustomForcedBehavior.IConstraintChecker&lt;int&gt; HotbarButton
  ```
  Domain: [1.12]

- **`ItemId Field`**
  ```csharp
  public static CustomForcedBehavior.IConstraintChecker&lt;int&gt; ItemId
  ```
  Domain: [1.int.MaxValue]

- **`Milliseconds Field`**
  ```csharp
  public static CustomForcedBehavior.IConstraintChecker&lt;int&gt; Milliseconds
  ```
  Domain: [0.int.MaxValue]

- **`MobId Field`**
  ```csharp
  public static CustomForcedBehavior.IConstraintChecker&lt;int&gt; MobId
  ```
  Domain: [1.int.MaxValue]

- **`ObjectId Field`**
  ```csharp
  public static CustomForcedBehavior.IConstraintChecker&lt;int&gt; ObjectId
  ```
  Domain: [1.int.MaxValue]

- **`Percent Field`**
  ```csharp
  public static CustomForcedBehavior.IConstraintChecker&lt;double&gt; Percent
  ```
  Domain: [0.0.100.0]

- **`Range Field`**
  ```csharp
  public static CustomForcedBehavior.IConstraintChecker&lt;double&gt; Range
  ```
  Domain: [1.0.10000.0]

- **`RepeatCount Field`**
  ```csharp
  public static CustomForcedBehavior.IConstraintChecker&lt;int&gt; RepeatCount
  ```
  Domain: [1.1000]

- **`SpellId Field`**
  ```csharp
  public static CustomForcedBehavior.IConstraintChecker&lt;int&gt; SpellId
  ```
  Domain: [1.int.MaxValue]

- **`StringNonEmpty Field`**
  ```csharp
  public static CustomForcedBehavior.IConstraintChecker&lt;string&gt; StringNonEmpty
  ```
  string must not be null or string.Empty

- **`Vector3NonEmpty Field`**
  ```csharp
  public static CustomForcedBehavior.IConstraintChecker&lt;Vector3&gt; Vector3NonEmpty
  ```
  Vector3 must not be equal to Vector3.Zero

- **`VehicleId Field`**
  ```csharp
  public static CustomForcedBehavior.IConstraintChecker&lt;int&gt; VehicleId
  ```
  Domain: [1.int.MaxValue]

---

### CustomForcedBehavior.ConstrainTo Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.CustomForcedBehavior.ConstrainTo

```csharp
public static class ConstrainTo
```

This class contains pre-built constraint checking classes for use with the &lt;
            see cref="GetAttributeAs&lt;T&gt;(.)"/&gt;, &lt;see cref="GetAttributeAsArray&lt;T&gt;
            (.)"/&gt;, &lt;see cref="GetAttributeAsNullable&lt;T&gt;(.)"/&gt;, or &lt;see
            cref="GetNumberedAttributesAsArray&lt;T&gt;(.)"/&gt; methods.

---

### CustomForcedBehavior.ConstrainTo.Anything(T) Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.CustomForcedBehavior.IConstraintChecker.T → Styx.CommonBot.Profiles.CustomForcedBehavior.ConstrainTo.Anything.T

```csharp
public class Anything&lt;T&gt; : CustomForcedBehavior.IConstraintChecker&lt;T&gt;
```

An anything.

#### Anything(T) Methods

- **`Check Method`**
  ```csharp
  public override string Check(
string attributeName,
T value
)
  ```
  Checks.
  - *attributeName*: Type: SystemAddLanguageSpecificTextSet("LST2FB9D5BB_5?cs=.|vb=.|cpp=::|nu=.|fs=.");StringName of the attribute.
  - *value*: Type: T        The value.
  - **Returns:** ReferenceCustomForcedBehaviorAddLanguageSpecificTextSet("LST2FB9D5BB_6?cs=.|vb=.|cpp=::|nu=.|fs=.");ConstrainToAddLanguageSpecificTextSet("LST2FB9D5BB_7?cs=.|vb=.|cpp=::|nu=.|fs=.");AnythingAddLanguageSpecificTextSet("LST2FB9D5BB_8?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LST2FB9D5BB_9?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

---

### CustomForcedBehavior.ConstrainTo.Domain(T) Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.CustomForcedBehavior.IConstraintChecker.T → Styx.CommonBot.Profiles.CustomForcedBehavior.ConstrainTo.Domain.T → Styx.CommonBot.Profiles.CustomForcedBehavior.ConstrainTo.QuestId.T

```csharp
public class Domain&lt;T&gt; : CustomForcedBehavior.IConstraintChecker&lt;T&gt;
```

A domain.

#### Domain(T) Methods

- **`Check Method`**
  ```csharp
  public override string Check(
string attributeName,
T value
)
  ```
  Checks.
  - *attributeName*: Type: SystemAddLanguageSpecificTextSet("LSTA087A599_5?cs=.|vb=.|cpp=::|nu=.|fs=.");StringName of the attribute.
  - *value*: Type: T        The value.
  - **Returns:** ReferenceCustomForcedBehaviorAddLanguageSpecificTextSet("LSTA087A599_6?cs=.|vb=.|cpp=::|nu=.|fs=.");ConstrainToAddLanguageSpecificTextSet("LSTA087A599_7?cs=.|vb=.|cpp=::|nu=.|fs=.");DomainAddLanguageSpecificTextSet("LSTA087A599_8?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LSTA087A599_9?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

---

### CustomForcedBehavior.ConstrainTo.NonEmptyString(T) Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.CustomForcedBehavior.IConstraintChecker.String → Styx.CommonBot.Profiles.CustomForcedBehavior.ConstrainTo.NonEmptyString.T

```csharp
public class NonEmptyString&lt;T&gt; : CustomForcedBehavior.IConstraintChecker&lt;string&gt;
```

A non empty string.

#### NonEmptyString(T) Methods

- **`Check Method`**
  ```csharp
  public override string Check(
string attributeName,
string value
)
  ```
  Checks.
  - *attributeName*: Type: SystemAddLanguageSpecificTextSet("LST3A15181A_5?cs=.|vb=.|cpp=::|nu=.|fs=.");StringName of the attribute.
  - *value*: Type: SystemAddLanguageSpecificTextSet("LST3A15181A_6?cs=.|vb=.|cpp=::|nu=.|fs=.");String        The value.
  - **Returns:** ReferenceCustomForcedBehaviorAddLanguageSpecificTextSet("LST3A15181A_7?cs=.|vb=.|cpp=::|nu=.|fs=.");ConstrainToAddLanguageSpecificTextSet("LST3A15181A_8?cs=.|vb=.|cpp=::|nu=.|fs=.");NonEmptyStringAddLanguageSpecificTextSet("LST3A15181A_9?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LST3A15181A_10?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

---

### CustomForcedBehavior.ConstrainTo.NonEmptyVector3(T) Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.CustomForcedBehavior.IConstraintChecker.Vector3 → Styx.CommonBot.Profiles.CustomForcedBehavior.ConstrainTo.NonEmptyVector3.T

```csharp
public class NonEmptyVector3&lt;T&gt; : CustomForcedBehavior.IConstraintChecker&lt;Vector3&gt;
```

A non empty wow point.

#### NonEmptyVector3(T) Methods

- **`Check Method`**
  ```csharp
  public override string Check(
string attributeName,
Vector3 value
)
  ```
  Checks.
  - *attributeName*: Type: SystemAddLanguageSpecificTextSet("LST11C93BC2_5?cs=.|vb=.|cpp=::|nu=.|fs=.");StringName of the attribute.
  - *value*: Type: System.NumericsAddLanguageSpecificTextSet("LST11C93BC2_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3        The value.
  - **Returns:** ReferenceCustomForcedBehaviorAddLanguageSpecificTextSet("LST11C93BC2_7?cs=.|vb=.|cpp=::|nu=.|fs=.");ConstrainToAddLanguageSpecificTextSet("LST11C93BC2_8?cs=.|vb=.|cpp=::|nu=.|fs=.");NonEmptyVector3AddLanguageSpecificTextSet("LST11C93BC2_9?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LST11C93BC2_10?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

---

### CustomForcedBehavior.ConstrainTo.QuestId(T) Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.CustomForcedBehavior.IConstraintChecker.Int32 → Styx.CommonBot.Profiles.CustomForcedBehavior.ConstrainTo.Domain.Int32 → Styx.CommonBot.Profiles.CustomForcedBehavior.ConstrainTo.QuestId.T

```csharp
public class QuestId&lt;T&gt; : CustomForcedBehavior.ConstrainTo.Domain&lt;int&gt;
```

A question identifier.

#### QuestId(T) Methods

- **`Check Method`**
  ```csharp
  public override string Check(
string attributeName,
int value
)
  ```
  Checks.
  - *attributeName*: Type: SystemAddLanguageSpecificTextSet("LSTBEBB5A2B_5?cs=.|vb=.|cpp=::|nu=.|fs=.");StringName of the attribute.
  - *value*: Type: SystemAddLanguageSpecificTextSet("LSTBEBB5A2B_6?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32        The value.
  - **Returns:** ReferenceCustomForcedBehaviorAddLanguageSpecificTextSet("LSTBEBB5A2B_7?cs=.|vb=.|cpp=::|nu=.|fs=.");ConstrainToAddLanguageSpecificTextSet("LSTBEBB5A2B_8?cs=.|vb=.|cpp=::|nu=.|fs=.");QuestIdAddLanguageSpecificTextSet("LSTBEBB5A2B_9?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LSTBEBB5A2B_10?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

---

### CustomForcedBehavior.ConstrainTo.SpecificValues(T) Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.CustomForcedBehavior.IConstraintChecker.T → Styx.CommonBot.Profiles.CustomForcedBehavior.ConstrainTo.SpecificValues.T

```csharp
public class SpecificValues&lt;T&gt; : CustomForcedBehavior.IConstraintChecker&lt;T&gt;
```

A specific values.

#### SpecificValues(T) Methods

- **`Check Method`**
  ```csharp
  public override string Check(
string attributeName,
T value
)
  ```
  Checks.
  - *attributeName*: Type: SystemAddLanguageSpecificTextSet("LSTD83F4FAD_5?cs=.|vb=.|cpp=::|nu=.|fs=.");StringName of the attribute.
  - *value*: Type: T        The value.
  - **Returns:** ReferenceCustomForcedBehaviorAddLanguageSpecificTextSet("LSTD83F4FAD_6?cs=.|vb=.|cpp=::|nu=.|fs=.");ConstrainToAddLanguageSpecificTextSet("LSTD83F4FAD_7?cs=.|vb=.|cpp=::|nu=.|fs=.");SpecificValuesAddLanguageSpecificTextSet("LSTD83F4FAD_8?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LSTD83F4FAD_9?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

---

### CustomForcedBehavior.IConstraintChecker(T) Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.CustomForcedBehavior.IConstraintChecker.T → Styx.CommonBot.Profiles.CustomForcedBehavior.ConstrainTo.Anything.T → Styx.CommonBot.Profiles.CustomForcedBehavior.ConstrainTo.Domain.T → Styx.CommonBot.Profiles.CustomForcedBehavior.ConstrainTo.NonEmptyString.T → Styx.CommonBot.Profiles.CustomForcedBehavior.ConstrainTo.NonEmptyVector3.T → Styx.CommonBot.Profiles.CustomForcedBehavior.ConstrainTo.SpecificValues.T

```csharp
public abstract class IConstraintChecker&lt;T&gt;
```

This defines the interface specification for custom-written constraint checkers
            that are accepted by the &lt;see cref="GetAttributeAs&lt;T&gt;(.)"/&gt;, &lt;see
            cref="GetAttributeAsArray&lt;T&gt;(.)"/&gt;, &lt;see cref="GetAttributeAsNullable&lt;T&gt;
            (.)"/&gt;, or &lt;see cref="GetNumberedAttributesAsArray&lt;T&gt;(.)"/&gt; methods.

#### IConstraintChecker(T) Methods

- **`Check Method`**
  ```csharp
  public virtual string Check(
string attributeName,
T value
)
  ```
  Checks.
  - *attributeName*: Type: SystemAddLanguageSpecificTextSet("LSTF943891C_4?cs=.|vb=.|cpp=::|nu=.|fs=.");StringName of the attribute.
  - *value*: Type: T        The value.
  - **Returns:** ReferenceCustomForcedBehaviorAddLanguageSpecificTextSet("LSTF943891C_5?cs=.|vb=.|cpp=::|nu=.|fs=.");IConstraintCheckerAddLanguageSpecificTextSet("LSTF943891C_6?cs=&lt;|vb=(Of |cpp=&lt;|nu=(|fs=&lt;'");TAddLanguageSpecificTextSet("LSTF943891C_7?cs=&gt;|vb=)|cpp=&gt;|nu=)|fs=&gt;"); Class

---

### CustomForcedBehavior.QuestCompleteRequirement Enumeration

```csharp
public enum QuestCompleteRequirement
```

Values that represent QuestCompleteRequirement.

---

### CustomForcedBehavior.QuestInLogRequirement Enumeration

```csharp
public enum QuestInLogRequirement
```

Values that represent QuestInLogRequirement.

---

### ForceMailManager Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.ForceMailManager

```csharp
public class ForceMailManager
```

Manager for force mails.

#### ForceMailManager Methods

- **`Add Method`**
  Adds item.

- **`Add Method (String)`**
  ```csharp
  public static bool Add(
string item
)
  ```
  Adds item.
  - *item*: Type: SystemAddLanguageSpecificTextSet("LST9BDD4476_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe item to remove.
  - **Returns:** ReferenceForceMailManager Class

- **`Add Method (UInt32)`**
  ```csharp
  public static bool Add(
uint item
)
  ```
  Adds item.
  - *item*: Type: SystemAddLanguageSpecificTextSet("LST697BEAEE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The item to remove.
  - **Returns:** ReferenceForceMailManager Class

- **`Contains Method`**
  Query if this object contains the given item.

- **`Contains Method (String)`**
  ```csharp
  public static bool Contains(
string item
)
  ```
  Query if this object contains the given item.
  - *item*: Type: SystemAddLanguageSpecificTextSet("LSTE6918F24_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe item to remove.
  - **Returns:** ReferenceForceMailManager Class

- **`Contains Method (UInt32)`**
  ```csharp
  public static bool Contains(
uint item
)
  ```
  Query if this object contains the given item.
  - *item*: Type: SystemAddLanguageSpecificTextSet("LST26FE6AF4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The item to remove.
  - **Returns:** ReferenceForceMailManager Class

- **`GetAllItemIds Method`**
  ```csharp
  public static List&lt;uint&gt; GetAllItemIds()
  ```
  Gets all item identifiers.
  - **Returns:** See Also

- **`GetAllItemNames Method`**
  ```csharp
  public static List&lt;string&gt; GetAllItemNames()
  ```
  Gets all item names.
  - **Returns:** See Also

- **`ReloadProtectedItems Method`**
  ```csharp
  public static void ReloadProtectedItems()
  ```
  Reload protected items.

- **`Remove Method`**
  Removes the given item.

- **`Remove Method (String)`**
  ```csharp
  public static bool Remove(
string item
)
  ```
  Removes the given item.
  - *item*: Type: SystemAddLanguageSpecificTextSet("LST224172C3_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe item to remove.
  - **Returns:** ReferenceForceMailManager Class

- **`Remove Method (UInt32)`**
  ```csharp
  public static bool Remove(
uint item
)
  ```
  Removes the given item.
  - *item*: Type: SystemAddLanguageSpecificTextSet("LSTB00FAFF9_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The item to remove.
  - **Returns:** ReferenceForceMailManager Class

---

### HotspotCollection Class

**Inheritance:** System.Object → System.Collections.Generic.List.Vector3 → Styx.CommonBot.Profiles.HotspotCollection

```csharp
public class HotspotCollection : List&lt;Vector3&gt;
```

Collection of hotspots.

#### HotspotCollection Constructor

- **`HotspotCollection Constructor`**
  ```csharp
  public HotspotCollection()
  ```
  Default constructor.

- **`HotspotCollection Constructor (IEnumerable(Vector3))`**
  ```csharp
  public HotspotCollection(
IEnumerable&lt;Vector3&gt; collection
)
  ```
  Constructor.
  - *collection*: Type: System.Collections.GenericAddLanguageSpecificTextSet("LSTCC1E995D_2?cs=.|vb=.|cpp=::|nu=.|fs=.");IEnumerableAddLanguageSpecificTextSet("LSTCC1E995D_3?cs=&lt;|vb=(Of |cpp=&lt;|fs=&lt;'|nu=(");Vector3AddLanguageSpecificTextSet("LSTCC1E995D_4?cs=&gt;|vb=)|cpp=&gt;|fs=&gt;|nu=)");The collection.

- **`HotspotCollection Constructor (Int32)`**
  ```csharp
  public HotspotCollection(
int capacity
)
  ```
  Constructor.
  - *capacity*: Type: SystemAddLanguageSpecificTextSet("LST83ABAE65_0?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32The capacity.

#### HotspotCollection Methods

- **`FindClosestTo Method`**
  ```csharp
  public Vector3 FindClosestTo(
Vector3 loc
)
  ```
  Searches for the nearest to.
  - *loc*: Type: System.NumericsAddLanguageSpecificTextSet("LST91521DDD_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.
  - **Returns:** ExceptionConditionExceptionThrown when an exception error condition occurs.

---

### IXmlObject Interface

```csharp
public interface IXmlObject
```

An object that is associated with an XML element

#### IXmlObject Properties

- **`Element Property`**
  ```csharp
  XElement Element { get; }
  ```
  Gets the element.

---

### Mailbox Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Mailbox

```csharp
public class Mailbox : IEquatable&lt;Mailbox&gt;,
IXmlObject
```

A mailbox.

#### Mailbox Constructor

- **`Mailbox Constructor (XElement)`**
  ```csharp
  public Mailbox(
XElement element
)
  ```
  Constructor.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST3454DE37_0?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.

- **`Mailbox Constructor (Vector3, String, Nullable(NavType))`**
  ```csharp
  public Mailbox(
Vector3 location,
string name = null,
Nullable&lt;NavType&gt; navType = null
)
  ```
  Initializes a new instance of the Mailbox class.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST1A0FC5E5_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.

#### Mailbox Properties

- **`Element Property`**
  ```csharp
  public XElement Element { get; }
  ```

- **`Location Property`**
  ```csharp
  public Vector3 Location { get; }
  ```
  Gets the location.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

- **`NavType Property`**
  ```csharp
  public Nullable&lt;NavType&gt; NavType { get; }
  ```
  Gets the type of the navigation.

- **`UsableWhen Property`**
  ```csharp
  public DelayCompiledExpression&lt;Func&lt;bool&gt;&gt; UsableWhen { get; }
  ```
  Gets the usable when.

#### Mailbox Methods

- **`Equals Method`**
  Determines whether the specified object is equal to the current object.

- **`Equals Method (Mailbox)`**
  ```csharp
  public bool Equals(
Mailbox other
)
  ```
  Tests if this Mailbox is considered equal to another.
  - *other*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LSTAE4C027F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");MailboxThe mailbox to compare to this object.
  - **Returns:** See Also

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceMailbox Class

---

### MailboxManager Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.MailboxManager

```csharp
public class MailboxManager
```

Manager for mailboxes.

#### MailboxManager Constructor

- **`MailboxManager Constructor`**
  ```csharp
  public MailboxManager()
  ```
  Default constructor.

- **`MailboxManager Constructor (XElement)`**
  ```csharp
  public MailboxManager(
XElement xml
)
  ```
  Constructor.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST321D1CF0_0?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

#### MailboxManager Properties

- **`AllMailboxes Property`**
  ```csharp
  public List&lt;Mailbox&gt; AllMailboxes { get; }
  ```
  Gets all mailboxes.

- **`Blacklist Property`**
  ```csharp
  public List&lt;Mailbox&gt; Blacklist { get; }
  ```
  Gets the blacklist.

- **`ForcedMailboxes Property`**
  ```csharp
  public List&lt;Mailbox&gt; ForcedMailboxes { get; set; }
  ```
  List of mailboxes you can set to 'override' the current mailboxes defined by the
            profile.

- **`Mailboxes Property`**
  ```csharp
  public List&lt;Mailbox&gt; Mailboxes { get; }
  ```
  Gets the mailboxes.

#### MailboxManager Methods

- **`GetClosestMailbox Method`**
  Gets closest mailbox.

- **`GetClosestMailbox Method`**
  ```csharp
  public Mailbox GetClosestMailbox()
  ```
  Gets closest mailbox.
  - **Returns:** ReferenceMailboxManager Class

- **`GetClosestMailbox Method (Vector3)`**
  ```csharp
  public Mailbox GetClosestMailbox(
Vector3 location
)
  ```
  Gets closest mailbox.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST8B5ED8F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.
  - **Returns:** ReferenceMailboxManager Class

---

### Profile Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Profile

```csharp
public class Profile : IEquatable&lt;Profile&gt;
```

A profile.

#### Profile Constructor

- **`Profile Constructor`**
  ```csharp
  public Profile()
  ```
  Default constructor.

- **`Profile Constructor (String, Profile)`**
  ```csharp
  public Profile(
string path,
Profile parent
)
  ```
  Constructor.
  - *path*: Type: SystemAddLanguageSpecificTextSet("LST36FC2717_0?cs=.|vb=.|cpp=::|nu=.|fs=.");String  Full pathname of the file.
  - *parent*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LST36FC2717_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ProfileThe parent.

- **`Profile Constructor (XElement, Profile)`**
  ```csharp
  public Profile(
XElement xml,
Profile parent
)
  ```
  Constructor.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTCEEC63F_0?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.
  - *parent*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LSTCEEC63F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ProfileThe parent.

#### Profile Properties

- **`AerialBlackspots Property`**
  ```csharp
  public Dictionary&lt;Tuple&lt;uint, WoWFactionGroup&gt;, List&lt;Vector2[]&gt;&gt; AerialBlackspots { get; }
  ```
  Gets the aerial blackspots.

- **`AvoidMobs Property`**
  ```csharp
  public DualHashSet&lt;uint, string&gt; AvoidMobs { get; }
  ```
  Gets the avoid mobs.

- **`Blacklist Property`**
  ```csharp
  public Dictionary&lt;uint, BlacklistFlags&gt; Blacklist { get; }
  ```
  Gets the blacklist.

- **`BlacklistedQuestgivers Property`**
  ```csharp
  public DualHashSet&lt;string, uint&gt; BlacklistedQuestgivers { get; }
  ```
  Gets the blacklisted questgivers.

- **`BlacklistedQuests Property`**
  ```csharp
  public DualHashSet&lt;string, uint&gt; BlacklistedQuests { get; }
  ```
  Gets the blacklisted quests.

- **`Blackspots Property`**
  ```csharp
  public List&lt;Blackspot&gt; Blackspots { get; }
  ```
  Gets the blackspots.

- **`ContinentId Property`**
  ```csharp
  public int ContinentId { get; }
  ```
  Gets the identifier of the continent.

- **`Factions Property`**
  ```csharp
  public HashSet&lt;uint&gt; Factions { get; }
  ```
  Gets the factions.

- **`ForceMail Property`**
  ```csharp
  public DualHashSet&lt;uint, string&gt; ForceMail { get; }
  ```
  Gets the force mail.

- **`GrindArea Property`**
  ```csharp
  public GrindArea GrindArea { get; }
  ```
  Gets the grind area.

- **`HotspotManager Property`**
  ```csharp
  public HotspotManager HotspotManager { get; }
  ```
  Gets the manager for hotspot.

- **`LootMobs Property`**
  ```csharp
  public Nullable&lt;bool&gt; LootMobs { get; set; }
  ```
  Gets or sets the loot mobs.

- **`LootRadius Property`**
  ```csharp
  public Nullable&lt;double&gt; LootRadius { get; set; }
  ```
  Gets or sets the loot radius.

- **`MailBlue Property`**
  ```csharp
  public bool MailBlue { get; set; }
  ```
  Gets a value indicating whether the mail blue.

- **`MailboxManager Property`**
  ```csharp
  public MailboxManager MailboxManager { get; }
  ```
  Gets the manager for mailbox.

- **`MailGreen Property`**
  ```csharp
  public bool MailGreen { get; set; }
  ```
  Gets a value indicating whether the mail green.

- **`MailGrey Property`**
  ```csharp
  public bool MailGrey { get; set; }
  ```
  Gets a value indicating whether the mail grey.

- **`MailPurple Property`**
  ```csharp
  public bool MailPurple { get; set; }
  ```
  Gets a value indicating whether the mail purple.

- **`MailQualities Property`**
  ```csharp
  public List&lt;WoWItemQuality&gt; MailQualities { get; }
  ```
  Gets the mail qualities.

- **`MailWhite Property`**
  ```csharp
  public bool MailWhite { get; set; }
  ```
  Gets a value indicating whether the mail white.

- **`MaxLevel Property`**
  ```csharp
  public int MaxLevel { get; }
  ```
  Gets the maximum level.

- **`MinDurability Property`**
  ```csharp
  public float MinDurability { get; }
  ```
  Gets the minimum durability.

- **`MinFreeBagSlots Property`**
  ```csharp
  public int MinFreeBagSlots { get; }
  ```
  Gets the minimum free bag slots.

- **`MinLevel Property`**
  ```csharp
  public int MinLevel { get; }
  ```
  Gets the minimum level.

- **`MinMailLevel Property`**
  ```csharp
  public int MinMailLevel { get; }
  ```
  Gets the minimum mail level.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

- **`NavType Property`**
  ```csharp
  public NavType NavType { get; set; }
  ```
  Gets or sets the type of the navigation.

- **`Parent Property`**
  ```csharp
  public Profile Parent { get; }
  ```
  Gets the parent.

- **`Path Property`**
  ```csharp
  public string Path { get; }
  ```
  Gets the path that was used to create the profile, or null, if not created from a path.

- **`ProtectedItems Property`**
  ```csharp
  public DualHashSet&lt;uint, string&gt; ProtectedItems { get; }
  ```
  Gets the protected items.

- **`QuestOrder Property`**
  ```csharp
  public OrderNodeCollection QuestOrder { get; }
  ```
  Gets the question order.

- **`Quests Property`**
  ```csharp
  public List&lt;QuestInfo&gt; Quests { get; }
  ```
  Gets the quests.

- **`SellBlue Property`**
  ```csharp
  public bool SellBlue { get; set; }
  ```
  Gets a value indicating whether the sell blue.

- **`SellGreen Property`**
  ```csharp
  public bool SellGreen { get; set; }
  ```
  Gets a value indicating whether the sell green.

- **`SellGrey Property`**
  ```csharp
  public bool SellGrey { get; set; }
  ```
  Gets a value indicating whether the sell grey.

- **`SellPurple Property`**
  ```csharp
  public bool SellPurple { get; set; }
  ```
  Gets a value indicating whether the sell purple.

- **`SellWhite Property`**
  ```csharp
  public bool SellWhite { get; set; }
  ```
  Gets a value indicating whether the sell white.

- **`SubProfiles Property`**
  ```csharp
  public List&lt;Profile&gt; SubProfiles { get; }
  ```
  Gets the sub profiles.

- **`TargetElites Property`**
  ```csharp
  public bool TargetElites { get; }
  ```
  Gets a value indicating whether the target elites.

- **`TargetingDistance Property`**
  ```csharp
  public Nullable&lt;double&gt; TargetingDistance { get; set; }
  ```
  Gets or sets the targeting distance.

- **`TargetMaxLevel Property`**
  ```csharp
  public int TargetMaxLevel { get; }
  ```
  Gets target maximum level.

- **`TargetMinLevel Property`**
  ```csharp
  public int TargetMinLevel { get; }
  ```
  Gets target minimum level.

- **`UseMount Property`**
  ```csharp
  public Nullable&lt;bool&gt; UseMount { get; set; }
  ```
  Gets or sets the use mount.

- **`VendorManager Property`**
  ```csharp
  public VendorManager VendorManager { get; }
  ```
  Gets the manager for vendor.

#### Profile Methods

- **`ApplyExpressions Method`**
  ```csharp
  public void ApplyExpressions(
IXmlObject xmlObject
)
  ```
  Restores the CompiledExpression values of all public properties of type DelayCompiledExpression with a CompileExpressionAttribute attached, to the values from an IXmlObject instance that was previously added to Add(IXmlObject)and is constructed of the same Element instance.All public properties of type IXmlObject or collections of IXmlObjectin xmlObject are also processed recursively.
  - *xmlObject*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LST4860623F_1?cs=.|vb=.|cpp=::|nu=.|fs=.");IXmlObject

- **`Equals Method`**
  Determines whether the specified Object is equal to the
            current Object.

- **`Equals Method (Object)`**
  ```csharp
  public override bool Equals(
Object obj
)
  ```
  Determines whether the specified Object is equal to the
            current Object.
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LSTEDA6A926_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ObjectThe Object to compare with the current
            Object.
  - **Returns:** ReferenceProfile Class

- **`Equals Method (Profile)`**
  ```csharp
  public bool Equals(
Profile other
)
  ```
  Tests if this Profile is considered equal to another.
  - *other*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LST1627938B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ProfileThe profile to compare to this object.
  - **Returns:** See Also

- **`FindQuest Method`**
  ```csharp
  public QuestInfo FindQuest(
uint id
)
  ```
  Searches for the first question.
  - *id*: Type: SystemAddLanguageSpecificTextSet("LST44EFE316_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The quest identifier.
  - **Returns:** ReferenceProfile Class

- **`GetHashCode Method`**
  ```csharp
  public override int GetHashCode()
  ```
  Serves as a hash function for a particular type.
  - **Returns:** ReferenceProfile Class

- **`GetRootProfile Method`**
  ```csharp
  public Profile GetRootProfile()
  ```
  Returns the root profile.
  - **Returns:** ReferenceProfile Class

- **`GetScopeSortedProfiles Method`**
  ```csharp
  public List&lt;Profile&gt; GetScopeSortedProfiles()
  ```
  Gets all children profiles and children of children profiles, returning a list where
            the most inner profiles are first.
  - **Returns:** See Also

- **`HasChanged Method`**
  ```csharp
  public bool HasChanged()
  ```
  Checks if this profile has changed on disk since it was loaded.
  - **Returns:** This function returns false if the file referred to by Path does not exist, or if Path is null.
            This function should only be used for profiles constructed with the Profile(String, Profile) constructor.This function opens the file referred to by the Path property; exceptions from opening the file are not caught. For exceptions thrown, see Open(String, FileMode, FileAccess).

#### Profile Events

- **`OnUnknownProfileElement Event`**
  ```csharp
  public static event EventHandler&lt;UnknownProfileElementEventArgs&gt; OnUnknownProfileElement
  ```
  Event queue for all listeners interested in OnUnknownProfileElement events.

#### Profile Operators

- **`Equality Operator`**
  ```csharp
  public static bool operator ==(
Profile left,
Profile right
)
  ```
  Equality operator.
  - *left*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LST9AA11E0B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Profile The left.
  - *right*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LST9AA11E0B_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ProfileThe right.
  - **Returns:** ReferenceProfile Class

- **`Inequality Operator`**
  ```csharp
  public static bool operator !=(
Profile left,
Profile right
)
  ```
  Inequality operator.
  - *left*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LST3FC1D126_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Profile The left.
  - *right*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LST3FC1D126_2?cs=.|vb=.|cpp=::|nu=.|fs=.");ProfileThe right.
  - **Returns:** ReferenceProfile Class

---

### ProfileAttributeExpectedException Class

**Inheritance:** System.Object → System.Exception → Styx.CommonBot.Profiles.ProfileException → Styx.CommonBot.Profiles.ProfileAttributeExpectedException

```csharp
public class ProfileAttributeExpectedException : ProfileException
```

Exception for signalling profile attribute expected errors.

---

### ProfileAttributeExpectedException(T) Class

**Inheritance:** System.Object → System.Exception → Styx.CommonBot.Profiles.ProfileException → Styx.CommonBot.Profiles.ProfileAttributeExpectedException.T

```csharp
public class ProfileAttributeExpectedException&lt;T&gt; : ProfileException
```

Exception for signalling profile attribute expected errors.

---

### ProfileException Class

**Inheritance:** System.Object → System.Exception → Styx.CommonBot.Profiles.ProfileException → More.

```csharp
public class ProfileException : Exception
```

Exception for signalling profile errors.

#### ProfileException Constructor

- **`ProfileException Constructor`**
  ```csharp
  public ProfileException()
  ```
  Default constructor.

- **`ProfileException Constructor (String)`**
  ```csharp
  public ProfileException(
string message
)
  ```
  Constructor.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTE8A81367_0?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe message.

- **`ProfileException Constructor (String, Exception)`**
  ```csharp
  public ProfileException(
string message,
Exception innerException
)
  ```
  Constructor.
  - *message*: Type: SystemAddLanguageSpecificTextSet("LSTF0525DC7_0?cs=.|vb=.|cpp=::|nu=.|fs=.");String       The message.
  - *innerException*: Type: SystemAddLanguageSpecificTextSet("LSTF0525DC7_1?cs=.|vb=.|cpp=::|nu=.|fs=.");ExceptionThe inner exception.

---

### ProfileManager Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.ProfileManager

```csharp
public static class ProfileManager
```

Manager for profiles.

#### ProfileManager Properties

- **`CurrentOuterProfile Property`**
  ```csharp
  public static Profile CurrentOuterProfile { get; }
  ```
  Gets the current outer profile.

- **`CurrentProfile Property`**
  ```csharp
  public static Profile CurrentProfile { get; set; }
  ```
  Gets or sets the current profile.

- **`XmlLocation Property`**
  ```csharp
  public static string XmlLocation { get; }
  ```
  Gets the XML location.

#### ProfileManager Methods

- **`LoadEmpty Method`**
  ```csharp
  public static void LoadEmpty()
  ```
  Loads an 'empty' profile. The empty profile will be valid for all level ranges, and
            continents.

- **`LoadNew Method`**
  Loads a new profile from a stream resource. This function will not save the profile
            as a "recent" profile as it does not have a file path. Use LoadNew(path) instead.

- **`LoadNew Method (Stream)`**
  ```csharp
  public static void LoadNew(
Stream stream
)
  ```
  Loads a new profile from a stream resource. This function will not save the profile
            as a "recent" profile as it does not have a file path. Use LoadNew(path) instead.
  - *stream*: Type: System.IOAddLanguageSpecificTextSet("LSTD96394B5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StreamA stream object containing the profile data. This must be parseable as
            XML or this function will throw exceptions.

- **`LoadNew Method (String, Boolean)`**
  ```csharp
  public static void LoadNew(
string path,
bool rememberMe = true
)
  ```
  Loads a new profile.
  - *path*: Type: SystemAddLanguageSpecificTextSet("LSTC72AB2EB_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe path of the profile.

- **`RefreshBestProfile Method`**
  ```csharp
  public static void RefreshBestProfile()
  ```
  Refresh best profile.

---

### ProfileMissingAttributeException Class

**Inheritance:** System.Object → System.Exception → Styx.CommonBot.Profiles.ProfileException → Styx.CommonBot.Profiles.ProfileMissingAttributeException

```csharp
public class ProfileMissingAttributeException : ProfileException
```

Exception for signalling profile missing attribute errors.

---

### ProfileMissingAttributeException(T) Class

**Inheritance:** System.Object → System.Exception → Styx.CommonBot.Profiles.ProfileException → Styx.CommonBot.Profiles.ProfileMissingAttributeException.T

```csharp
public class ProfileMissingAttributeException&lt;T&gt; : ProfileException
```

Exception for signalling profile missing attribute errors.

---

### ProfileMissingElementException Class

**Inheritance:** System.Object → System.Exception → Styx.CommonBot.Profiles.ProfileException → Styx.CommonBot.Profiles.ProfileMissingElementException

```csharp
public class ProfileMissingElementException : ProfileException
```

Exception for signalling profile missing element errors.

---

### ProfileNotFoundException Class

**Inheritance:** System.Object → System.Exception → Styx.CommonBot.Profiles.ProfileException → Styx.CommonBot.Profiles.ProfileNotFoundException

```csharp
public class ProfileNotFoundException : ProfileException
```

Exception for signalling profile is not found.

---

### ProfileTagExpectedException Class

**Inheritance:** System.Object → System.Exception → Styx.CommonBot.Profiles.ProfileException → Styx.CommonBot.Profiles.ProfileTagExpectedException

```csharp
public class ProfileTagExpectedException : ProfileException
```

Exception for signalling profile tag expected errors.

#### ProfileTagExpectedException Methods

- **`Create(T) Method`**
  ```csharp
  public static ProfileTagExpectedException Create&lt;T&gt;(
XElement tag,
params string[] validValues
)
  ```
  Creates a ProfileTagExpectedException for the specified basic type.
  - *tag*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST3B2771CB_3?cs=.|vb=.|cpp=::|nu=.|fs=.");XElement
  - *validValues*: Type: AddLanguageSpecificTextSet("LST3B2771CB_4?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST3B2771CB_5?cs=.|vb=.|cpp=::|nu=.|fs=.");StringAddLanguageSpecificTextSet("LST3B2771CB_6?cpp=&gt;|vb=()|nu=[]");
  - **Returns:** ReferenceProfileTagExpectedException Class

---

### ProfileUnknownAttributeException Class

**Inheritance:** System.Object → System.Exception → Styx.CommonBot.Profiles.ProfileException → Styx.CommonBot.Profiles.ProfileUnknownAttributeException

```csharp
public class ProfileUnknownAttributeException : ProfileException
```

Exception for signalling profile unknown attribute errors.

#### ProfileUnknownAttributeException Constructor

- **`ProfileUnknownAttributeException Constructor (XAttribute)`**
  ```csharp
  public ProfileUnknownAttributeException(
XAttribute attribute
)
  ```
  Constructor.
  - *attribute*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTB76D47B1_0?cs=.|vb=.|cpp=::|nu=.|fs=.");XAttributeThe attribute.

- **`ProfileUnknownAttributeException Constructor (XAttribute, String[])`**
  ```csharp
  public ProfileUnknownAttributeException(
XAttribute attribute,
params string[] validAttributes
)
  ```
  Constructor.
  - *attribute*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST5FE44895_2?cs=.|vb=.|cpp=::|nu=.|fs=.");XAttribute      The attribute.
  - *validAttributes*: Type: AddLanguageSpecificTextSet("LST5FE44895_3?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST5FE44895_4?cs=.|vb=.|cpp=::|nu=.|fs=.");StringAddLanguageSpecificTextSet("LST5FE44895_5?cpp=&gt;|vb=()|nu=[]");A variable-length parameters list containing valid attributes.

---

### ProfileUnknownElementException Class

**Inheritance:** System.Object → System.Exception → Styx.CommonBot.Profiles.ProfileException → Styx.CommonBot.Profiles.ProfileUnknownElementException

```csharp
public class ProfileUnknownElementException : ProfileException
```

Exception for signalling profile unknown element errors.

#### ProfileUnknownElementException Constructor

- **`ProfileUnknownElementException Constructor (XElement)`**
  ```csharp
  public ProfileUnknownElementException(
XElement element
)
  ```
  Constructor.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTB0AA2D31_0?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.

- **`ProfileUnknownElementException Constructor (XElement, String[])`**
  ```csharp
  public ProfileUnknownElementException(
XElement element,
params string[] validTags
)
  ```
  Constructor.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LST646EAE15_2?cs=.|vb=.|cpp=::|nu=.|fs=.");XElement  The element.
  - *validTags*: Type: AddLanguageSpecificTextSet("LST646EAE15_3?cpp=array&lt;");SystemAddLanguageSpecificTextSet("LST646EAE15_4?cs=.|vb=.|cpp=::|nu=.|fs=.");StringAddLanguageSpecificTextSet("LST646EAE15_5?cpp=&gt;|vb=()|nu=[]");A variable-length parameters list containing valid tags.

---

### ProtectedItemsManager Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.ProtectedItemsManager

```csharp
public static class ProtectedItemsManager
```

Manager for protected items.

#### ProtectedItemsManager Methods

- **`Add Method`**
  Adds item.

- **`Add Method (String)`**
  ```csharp
  public static bool Add(
string item
)
  ```
  Adds item.
  - *item*: Type: SystemAddLanguageSpecificTextSet("LST93303FFE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe item to add.
  - **Returns:** ReferenceProtectedItemsManager Class

- **`Add Method (UInt32)`**
  ```csharp
  public static bool Add(
uint item
)
  ```
  Adds item.
  - *item*: Type: SystemAddLanguageSpecificTextSet("LST813307CE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The item to add.
  - **Returns:** ReferenceProtectedItemsManager Class

- **`Contains Method`**
  Query if this object contains the given item.

- **`Contains Method (String)`**
  ```csharp
  public static bool Contains(
string item
)
  ```
  Query if this object contains the given item.
  - *item*: Type: SystemAddLanguageSpecificTextSet("LST975B7974_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe item to remove.
  - **Returns:** ReferenceProtectedItemsManager Class

- **`Contains Method (UInt32)`**
  ```csharp
  public static bool Contains(
uint item
)
  ```
  Query if this object contains the given item.
  - *item*: Type: SystemAddLanguageSpecificTextSet("LST6C9A398C_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The item to remove.
  - **Returns:** ReferenceProtectedItemsManager Class

- **`GetAllItemIds Method`**
  ```csharp
  public static List&lt;uint&gt; GetAllItemIds()
  ```
  Gets all item identifiers.
  - **Returns:** See Also

- **`GetAllItemNames Method`**
  ```csharp
  public static List&lt;string&gt; GetAllItemNames()
  ```
  Gets all item names.
  - **Returns:** See Also

- **`ReloadProtectedItems Method`**
  ```csharp
  public static void ReloadProtectedItems()
  ```
  Reload protected items.

- **`Remove Method`**
  Removes the given item.

- **`Remove Method (String)`**
  ```csharp
  public static bool Remove(
string item
)
  ```
  Removes the given item.
  - *item*: Type: SystemAddLanguageSpecificTextSet("LST4F282907_1?cs=.|vb=.|cpp=::|nu=.|fs=.");StringThe item to remove.
  - **Returns:** ReferenceProtectedItemsManager Class

- **`Remove Method (UInt32)`**
  ```csharp
  public static bool Remove(
uint item
)
  ```
  Removes the given item.
  - *item*: Type: SystemAddLanguageSpecificTextSet("LST1CC4E14D_1?cs=.|vb=.|cpp=::|nu=.|fs=.");UInt32The item to remove.
  - **Returns:** ReferenceProtectedItemsManager Class

---

### UnknownProfileElementEventArgs Class

**Inheritance:** System.Object → System.EventArgs → Styx.CommonBot.Profiles.UnknownProfileElementEventArgs

```csharp
public class UnknownProfileElementEventArgs : EventArgs
```

Additional information for unknown profile element events.

#### UnknownProfileElementEventArgs Properties

- **`Element Property`**
  ```csharp
  public XElement Element { get; set; }
  ```
  Gets or sets the element.

- **`Handled Property`**
  ```csharp
  public bool Handled { get; set; }
  ```
  Gets or sets a value indicating whether the handled.

---

### Vendor Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.Vendor

```csharp
public class Vendor : IEquatable&lt;Vendor&gt;,
IXmlObject
```

A vendor.

#### Vendor Constructor

- **`Vendor Constructor (XElement)`**
  ```csharp
  public Vendor(
XElement xml
)
  ```
  Constructor.
  - *xml*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTB297C9E3_0?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe XML.

- **`Vendor Constructor (WoWObject, Vendor.VendorType)`**
  ```csharp
  public Vendor(
WoWObject unit,
Vendor.VendorType type
)
  ```
  Constructor.
  - *unit*: Type: Styx.WoWInternals.WoWObjectsAddLanguageSpecificTextSet("LSTDDD63AE7_2?cs=.|vb=.|cpp=::|nu=.|fs=.");WoWObjectThe unit.
  - *type*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LSTDDD63AE7_3?cs=.|vb=.|cpp=::|nu=.|fs=.");VendorAddLanguageSpecificTextSet("LSTDDD63AE7_4?cs=.|vb=.|cpp=::|nu=.|fs=.");VendorTypeThe type.

- **`Vendor Constructor (Int32, String, Vendor.VendorType, Vector3, Nullable(NavType))`**
  ```csharp
  public Vendor(
int entry,
string name,
Vendor.VendorType type,
Vector3 location,
Nullable&lt;NavType&gt; navType = null
)
  ```
  Constructor.
  - *entry*: Type: SystemAddLanguageSpecificTextSet("LST92F90B9E_4?cs=.|vb=.|cpp=::|nu=.|fs=.");Int32   The entry.
  - *name*: Type: SystemAddLanguageSpecificTextSet("LST92F90B9E_5?cs=.|vb=.|cpp=::|nu=.|fs=.");String    The name.
  - *type*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LST92F90B9E_6?cs=.|vb=.|cpp=::|nu=.|fs=.");VendorAddLanguageSpecificTextSet("LST92F90B9E_7?cs=.|vb=.|cpp=::|nu=.|fs=.");VendorType    The type.
  - *location*: Type: System.NumericsAddLanguageSpecificTextSet("LST92F90B9E_8?cs=.|vb=.|cpp=::|nu=.|fs=.");Vector3The location.

#### Vendor Properties

- **`Element Property`**
  ```csharp
  public XElement Element { get; }
  ```

- **`Entry Property`**
  ```csharp
  public int Entry { get; }
  ```
  Gets the entry.

- **`Entry2 Property`**
  ```csharp
  public Nullable&lt;int&gt; Entry2 { get; }
  ```
  Gets the entry 2.

- **`Location Property`**
  ```csharp
  public Vector3 Location { get; set; }
  ```
  Gets or sets the location.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Gets the name.

- **`NavType Property`**
  ```csharp
  public Nullable&lt;NavType&gt; NavType { get; set; }
  ```
  Gets or sets the type of the navigaiton.

- **`TrainClass Property`**
  ```csharp
  public WoWClass TrainClass { get; set; }
  ```
  Gets or sets the train class.

- **`Type Property`**
  ```csharp
  public Vendor.VendorType Type { get; }
  ```
  Gets the type.

- **`UsableWhen Property`**
  ```csharp
  public DelayCompiledExpression&lt;Func&lt;bool&gt;&gt; UsableWhen { get; }
  ```
  Gets the usable when.

#### Vendor Methods

- **`Equals Method`**
  Determines whether the specified object is equal to the current object.

- **`Equals Method (Vendor)`**
  ```csharp
  public bool Equals(
Vendor other
)
  ```
  Tests if this Vendor is considered equal to another.
  - *other*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LST2EA2AC03_1?cs=.|vb=.|cpp=::|nu=.|fs=.");VendorThe vendor to compare to this object.
  - **Returns:** See Also

- **`ToString Method`**
  ```csharp
  public override string ToString()
  ```
  Returns a String that represents the current
            Object.
  - **Returns:** ReferenceVendor Class

---

### Vendor.VendorType Enumeration

```csharp
public enum VendorType
```

Values that represent VendorType.

---

### VendorManager Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.VendorManager

```csharp
public class VendorManager
```

Manager for vendors.

#### VendorManager Constructor

- **`VendorManager Constructor`**
  ```csharp
  public VendorManager()
  ```
  Default constructor.

- **`VendorManager Constructor (XElement)`**
  ```csharp
  public VendorManager(
XElement element
)
  ```
  Constructor.
  - *element*: Type: System.Xml.LinqAddLanguageSpecificTextSet("LSTA62270BC_0?cs=.|vb=.|cpp=::|nu=.|fs=.");XElementThe element.

#### VendorManager Properties

- **`AllVendors Property`**
  ```csharp
  public List&lt;Vendor&gt; AllVendors { get; }
  ```
  All the vendors.

- **`ForcedVendors Property`**
  ```csharp
  public List&lt;Vendor&gt; ForcedVendors { get; set; }
  ```
  List of vendors you can set to 'override' the current vendors defined by the profile.

- **`Vendors Property`**
  ```csharp
  public Lookup&lt;Vendor.VendorType, Vendor&gt; Vendors { get; }
  ```
  Gets the vendors.

#### VendorManager Methods

- **`GetClosestVendor Method`**
  Gets closest vendor.

- **`GetClosestVendor Method`**
  ```csharp
  public Vendor GetClosestVendor()
  ```
  Gets closest vendor.
  - **Returns:** ReferenceVendorManager Class

- **`GetClosestVendor Method (Vendor.VendorType)`**
  ```csharp
  public Vendor GetClosestVendor(
Vendor.VendorType type
)
  ```
  Gets closest vendor.
  - *type*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LSTEF0EC13A_3?cs=.|vb=.|cpp=::|nu=.|fs=.");VendorAddLanguageSpecificTextSet("LSTEF0EC13A_4?cs=.|vb=.|cpp=::|nu=.|fs=.");VendorTypeThe type.
  - **Returns:** ReferenceVendorManager Class

---

### VendorTypeExtensions Class

**Inheritance:** System.Object → Styx.CommonBot.Profiles.VendorTypeExtensions

```csharp
public static class VendorTypeExtensions
```

A vendor type extensions.

#### VendorTypeExtensions Methods

- **`AsNpcFlag Method`**
  ```csharp
  public static UnitNPCFlags AsNpcFlag(
this Vendor.VendorType vt
)
  ```
  A Vendor.VendorType extension method that converts a vt to a npc flag.
  - *vt*: Type: Styx.CommonBot.ProfilesAddLanguageSpecificTextSet("LSTFF9799F_2?cs=.|vb=.|cpp=::|nu=.|fs=.");VendorAddLanguageSpecificTextSet("LSTFF9799F_3?cs=.|vb=.|cpp=::|nu=.|fs=.");VendorTypeThe vt.
  - **Returns:** See Also

---
