# Styx.WoWInternals.TradeSkills

## Contents

- [Ingredient Class](#ingredient-class)
- [Recipe Class](#recipe-class)
- [RecipeDifficulty Enumeration](#recipedifficulty-enumeration)
- [Tool Class](#tool-class)
- [TradeSkill Class](#tradeskill-class)

---

### Ingredient Class

**Inheritance:** System.Object → Styx.WoWInternals.TradeSkills.Ingredient

```csharp
public class Ingredient
```

Number of this Reagent in players possession

#### Ingredient Properties

- **`ID Property`**
  ```csharp
  public uint ID { get; }
  ```

- **`InBagItemCount Property`**
  ```csharp
  public int InBagItemCount { get; }
  ```
  Number of this Reagent in players possession

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Name of the Reagent

- **`Required Property`**
  ```csharp
  public uint Required { get; }
  ```
  The required number of this reagent needed

---

### Recipe Class

**Inheritance:** System.Object → Styx.WoWInternals.TradeSkills.Recipe

```csharp
public class Recipe
```

Returns the color that represents the recipes difficulty

#### Recipe Properties

- **`CanRepeatNum Property`**
  ```csharp
  public uint CanRepeatNum { get; }
  ```
  Returns the color that represents the recipes difficulty

- **`CraftedItemId Property`**
  ```csharp
  public uint CraftedItemId { get; }
  ```
  Returns ID of the item the recipe makes

- **`Difficulty Property`**
  ```csharp
  public RecipeDifficulty Difficulty { get; }
  ```
  Returns the difficulty of the Recipe

- **`DisplayOrder Property`**
  ```csharp
  public int DisplayOrder { get; }
  ```
  Gets the display order.

- **`GreenSkillLevel Property`**
  ```csharp
  public int GreenSkillLevel { get; }
  ```
  Gets the green skill level.

- **`GreySkillLevel Property`**
  ```csharp
  public int GreySkillLevel { get; }
  ```
  Gets the grey skill level.

- **`Header Property`**
  ```csharp
  public string Header { get; }
  ```
  Name of header this recipe belongs to.Returns null when header name is not immediately available but it can be available in a subsequent call.

- **`Ingredients Property`**
  ```csharp
  public IReadOnlyList&lt;Ingredient&gt; Ingredients { get; }
  ```
  List of ingredients required for the recipe

- **`KnowsRecipe Property`**
  ```csharp
  public bool KnowsRecipe { get; }
  ```
  Gets a value indicating whether player knows recipe.

- **`MaxRank Property`**
  ```csharp
  public int MaxRank { get; }
  ```

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Name of the Recipe

- **`OptimalSkillups Property`**
  ```csharp
  public int OptimalSkillups { get; }
  ```
  Number of skillups earned when recipe difficulty is orange (optimal)

- **`Rank Property`**
  ```csharp
  public int Rank { get; }
  ```

- **`Skill Property`**
  ```csharp
  public SkillLine Skill { get; }
  ```
  The Skill this recipe is from

- **`Skillups Property`**
  ```csharp
  public int Skillups { get; }
  ```
  Number of Skillup this recipe will grant when crafted using current tradeskill level

- **`Spell Property`**
  ```csharp
  public WoWSpell Spell { get; }
  ```
  returns the spell that's attached to the recipe

- **`SpellId Property`**
  ```csharp
  public int SpellId { get; }
  ```
  Gets the spell identifier.

- **`Tools Property`**
  ```csharp
  public IReadOnlyList&lt;Tool&gt; Tools { get; }
  ```

- **`YellowSkillLevel Property`**
  ```csharp
  public int YellowSkillLevel { get; }
  ```
  Gets the yellow skill level.

#### Recipe Methods

- **`CanCraft Method`**
  ```csharp
  public bool CanCraft(
out bool missingIngredients,
out bool missingTools
)
  ```
  Returns whether or not we can craft this recipe.
  - *missingIngredients*: Type: SystemAddLanguageSpecificTextSet("LST1342EBC5_1?cs=.|vb=.|cpp=::|nu=.|fs=.");BooleanAddLanguageSpecificTextSet("LST1342EBC5_2?cpp=%");Whether we are missing required ingredients for the recipe.
  - *missingTools*: Type: SystemAddLanguageSpecificTextSet("LST1342EBC5_3?cs=.|vb=.|cpp=::|nu=.|fs=.");BooleanAddLanguageSpecificTextSet("LST1342EBC5_4?cpp=%");Whether we are missing required tools for the recipe.
  - **Returns:** ReferenceRecipe Class

---

### RecipeDifficulty Enumeration

```csharp
public enum RecipeDifficulty
```

---

### Tool Class

**Inheritance:** System.Object → Styx.WoWInternals.TradeSkills.Tool

```csharp
public class Tool : IEquatable&lt;Tool&gt;
```

returns true if tool is in players bags

#### Tool Properties

- **`HasTool Property`**
  ```csharp
  public bool HasTool { get; }
  ```
  returns true if tool is in players bags

- **`Id Property`**
  ```csharp
  public uint Id { get; }
  ```
  Gets the identifier.

- **`ItemId Property`**
  ```csharp
  [ObsoleteAttribute("Use Id instead")]
public uint ItemId { get; }
  ```
  Gets the item identifier.

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Name of the tool

#### Tool Methods

- **`Equals Method`**

- **`Equals Method (Object)`**
  ```csharp
  public override bool Equals(
Object obj
)
  ```
  - *obj*: Type: SystemAddLanguageSpecificTextSet("LST33CA92FE_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Object
  - **Returns:** ReferenceTool Class

- **`Equals Method (Tool)`**
  ```csharp
  public bool Equals(
Tool other
)
  ```
  - *other*: Type: Styx.WoWInternals.TradeSkillsAddLanguageSpecificTextSet("LST3B975C63_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Tool
  - **Returns:** See Also

- **`GetHashCode Method`**
  ```csharp
  public override int GetHashCode()
  ```
  - **Returns:** ReferenceTool Class

#### Tool Operators

- **`Equality Operator`**
  ```csharp
  public static bool operator ==(
Tool left,
Tool right
)
  ```
  - *left*: Type: Styx.WoWInternals.TradeSkillsAddLanguageSpecificTextSet("LST4AC0104B_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Tool
  - *right*: Type: Styx.WoWInternals.TradeSkillsAddLanguageSpecificTextSet("LST4AC0104B_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Tool
  - **Returns:** ReferenceTool Class

- **`Inequality Operator`**
  ```csharp
  public static bool operator !=(
Tool left,
Tool right
)
  ```
  - *left*: Type: Styx.WoWInternals.TradeSkillsAddLanguageSpecificTextSet("LST75E235E4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");Tool
  - *right*: Type: Styx.WoWInternals.TradeSkillsAddLanguageSpecificTextSet("LST75E235E4_2?cs=.|vb=.|cpp=::|nu=.|fs=.");Tool
  - **Returns:** ReferenceTool Class

---

### TradeSkill Class

**Inheritance:** System.Object → Styx.WoWInternals.TradeSkills.TradeSkill

```csharp
public class TradeSkill
```

Amount of Bonus the player has to the Tradeskill

#### TradeSkill Properties

- **`Bonus Property`**
  ```csharp
  public uint Bonus { get; }
  ```
  Amount of Bonus the player has to the Tradeskill

- **`KnownRecipes Property`**
  ```csharp
  public IReadOnlyDictionary&lt;int, Recipe&gt; KnownRecipes { get; }
  ```
  List of known recipes

- **`Level Property`**
  ```csharp
  public int Level { get; }
  ```
  Tradeskill level

- **`MaxLevel Property`**
  ```csharp
  public int MaxLevel { get; }
  ```
  Maximum level of the Tradeskill

- **`Name Property`**
  ```csharp
  public string Name { get; }
  ```
  Name of the Tradeskill

- **`RecipeCount Property`**
  ```csharp
  public int RecipeCount { get; }
  ```
  number of Recipes

- **`Recipes Property`**
  ```csharp
  public IReadOnlyDictionary&lt;int, Recipe&gt; Recipes { get; }
  ```
  List of all recipes

- **`SkillLine Property`**
  ```csharp
  public SkillLine SkillLine { get; }
  ```

- **`Tools Property`**
  ```csharp
  public IReadOnlyList&lt;Tool&gt; Tools { get; }
  ```
  List of Tools

- **`WoWSkill Property`**
  ```csharp
  public WoWSkill WoWSkill { get; }
  ```

#### TradeSkill Methods

- **`GetTradeSkill Method`**
  ```csharp
  public static TradeSkill GetTradeSkill(
SkillLine skillLine
)
  ```
  Returns a tradeskill for the provided skillLine
  - *skillLine*: Type: StyxAddLanguageSpecificTextSet("LST632229F4_1?cs=.|vb=.|cpp=::|nu=.|fs=.");SkillLine
  - **Returns:** ReferenceTradeSkill Class

- **`GetTradeSkills Method`**
  ```csharp
  public static IEnumerable&lt;TradeSkill&gt; GetTradeSkills(
SkillLine[] skillLines
)
  ```
  Gets the tradeskills.
  - *skillLines*: Type: AddLanguageSpecificTextSet("LST91929AE3_1?cpp=array&lt;");StyxAddLanguageSpecificTextSet("LST91929AE3_2?cs=.|vb=.|cpp=::|nu=.|fs=.");SkillLineAddLanguageSpecificTextSet("LST91929AE3_3?cpp=&gt;|vb=()|nu=[]");The skill lines.
  - **Returns:** Exceptions

#### TradeSkill Fields

- **`_knownRecipes Field`**
  ```csharp
  public PerFrameCachedValue&lt;IReadOnlyDictionary&lt;int, Recipe&gt;&gt; _knownRecipes
  ```

---
