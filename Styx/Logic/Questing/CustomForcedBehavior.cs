// Styx.Logic.Questing.CustomForcedBehavior
// Ported from Honorbuddy 4.3.4 (Cata) - Cleaned

using Styx.Helpers;
using Styx.Logic.BehaviorTree;
using Styx.Logic.Pathing;
using Styx.WoWInternals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using TreeSharp;

#nullable disable
namespace Styx.Logic.Questing;

public abstract class CustomForcedBehavior
{
  private bool _hasAttributeProblem;
  private List<string> _processedAttributes = new List<string>();
  private readonly Dictionary<string, string> _args;
  private Composite _behaviorTreeHook;
  private readonly string _elementName;
  private static Regex VariableSubstitutionRegex = new Regex("^\\$[^:]+:[:]?[ \t]*([^$]+)[ \t]*\\$$");

  private sealed class AttributeNamePair<T1, T2>
  {
    public T1 aliasBaseName;
    public T2 attributeName;

    public AttributeNamePair(T1 aliasBaseName, T2 attributeName)
    {
      this.aliasBaseName = aliasBaseName;
      this.attributeName = attributeName;
    }
  }

  public T GetAttributeAs<T>(
    string attributeName,
    bool isAttributeRequired,
    CustomForcedBehavior.IConstraintChecker<T> constraints,
    string[] attributeNameAliases)
    where T : class
  {
    return (T) this.GetAttributeValueAsObject<T>(attributeName, isAttributeRequired, constraints, attributeNameAliases);
  }

  public T[] GetAttributeAsArray<T>(
    string attributeName,
    bool isAttributeRequired,
    CustomForcedBehavior.IConstraintChecker<T> constraints,
    string[] attributeNameAliases,
    char[] separatorCharacters)
  {
    if (typeof (T) == typeof (WoWPoint))
      return (T[]) this.ParseWoWPointArray(attributeName, isAttributeRequired, attributeNameAliases);
    constraints = constraints ?? (CustomForcedBehavior.IConstraintChecker<T>) new CustomForcedBehavior.ConstrainTo.Anything<T>();
    char[] chArray = separatorCharacters;
    if (chArray == null)
      chArray = new char[3]{ ' ', ',', ';' };
    separatorCharacters = chArray;
    bool flag = false;
    string str = this.FindAttributeKeyOrAlias(isAttributeRequired, attributeName, attributeNameAliases);
    List<T> objList = new List<T>();
    if (str != null && this.Args.ContainsKey(str))
    {
      foreach (string segment in this.Args[str].Split(separatorCharacters, StringSplitOptions.RemoveEmptyEntries))
      {
        T convertedValue;
        try
        {
          convertedValue = this.ConvertStringToType<T>(str, segment);
        }
        catch (Exception ex)
        {
          flag = true;
          continue;
        }
        string format = constraints.Check(str, convertedValue);
        if (format != null)
        {
          this.LogMessage("error", format);
          flag = true;
        }
        else
          objList.Add(convertedValue);
      }
      if (flag)
      {
        objList.Clear();
        this.IsAttributeProblem = true;
      }
      return objList.ToArray();
    }
    objList.Clear();
    return objList.ToArray();
  }

  public T? GetAttributeAsNullable<T>(
    string attributeName,
    bool isAttributeRequired,
    CustomForcedBehavior.IConstraintChecker<T> constraints,
    string[] attributeNameAliases)
    where T : struct
  {
    return (T?) this.GetAttributeValueAsObject<T>(attributeName, isAttributeRequired, constraints, attributeNameAliases);
  }

  public T[] GetNumberedAttributesAsArray<T>(
    string baseName,
    int countRequired,
    CustomForcedBehavior.IConstraintChecker<T> constraints,
    string[] aliasBaseNames)
  {
    string capturedBaseName = baseName;
    CustomForcedBehavior capturedThis = this;
    bool isWoWPoint = typeof(T) == typeof(WoWPoint);

    bool flag = false;
    List<T> resultList = new List<T>();

    // Filter keys that match the base name pattern
    foreach (string matchingKey in this.Args.Keys.Where<string>(key => capturedThis.IsKeyMatchingBasePattern(capturedBaseName, key, isWoWPoint)))
      flag |= this.TryProcessSingleNumberedAttribute<T>(matchingKey, constraints, resultList);

    if (aliasBaseNames != null)
    {
      // Process alias base names
      foreach (string matchingKey in ((IEnumerable<string>) aliasBaseNames)
        .SelectMany<string, string, AttributeNamePair<string, string>>(
          (Func<string, IEnumerable<string>>) (alias => (IEnumerable<string>) this.Args.Keys),
          (Func<string, string, AttributeNamePair<string, string>>) ((alias, key) => new AttributeNamePair<string, string>(alias, key)))
        .Where<AttributeNamePair<string, string>>(pair => capturedThis.IsKeyMatchingBasePattern(pair.aliasBaseName, pair.attributeName, isWoWPoint))
        .Select<AttributeNamePair<string, string>, string>((Func<AttributeNamePair<string, string>, string>) (pair => pair.attributeName)))
        flag |= this.TryProcessSingleNumberedAttribute<T>(matchingKey, constraints, resultList);
    }

    if (resultList.Count < countRequired)
    {
      this.LogMessage("error", "The attribute '{0}N' must be provided at least {1} times (saw it '{2}' times).\n(E.g., ButtonText1, ButtonText2, ButtonText3, ...)\nPlease modify the profile to supply {1} attributes with a base name of '{0}'.", (object) capturedBaseName, (object) countRequired, (object) resultList.Count);
      flag = true;
    }

    if (flag)
    {
      resultList.Clear();
      this.IsAttributeProblem = true;
    }
    return resultList.ToArray();
  }

  private bool TryProcessSingleNumberedAttribute<T>(
    string attributeName,
    CustomForcedBehavior.IConstraintChecker<T> constraintChecker,
    List<T> resultList)
  {
    string baseAttributeName = attributeName;
    if (typeof (T) == typeof (WoWPoint))
    {
      if (attributeName.EndsWith("X") || attributeName.EndsWith("Y") || attributeName.EndsWith("Z"))
        baseAttributeName = attributeName.Substring(0, attributeName.Length - 1);
      if (!attributeName.EndsWith("X") && this.Args.ContainsKey(baseAttributeName + "X"))
        return false;
    }
    object obj = this.GetAttributeValueAsObject<T>(baseAttributeName, false, constraintChecker, (string[]) null);
    if (obj == null)
      return true;
    resultList.Add((T) obj);
    return false;
  }

  private object GetAttributeValueAsObject<T>(
    string attributeName,
    bool isRequired,
    CustomForcedBehavior.IConstraintChecker<T> constraintChecker,
    string[] aliases)
  {
    if (typeof (T) == typeof (WoWPoint))
      return this.ParseSingleWoWPoint(attributeName, isRequired, aliases);
    constraintChecker = constraintChecker ?? (CustomForcedBehavior.IConstraintChecker<T>) new CustomForcedBehavior.ConstrainTo.Anything<T>();
    string str = this.FindAttributeKeyOrAlias(isRequired, attributeName, aliases);
    if (str == null || !this.Args.ContainsKey(str))
      return (object) null;
    string attributeValue = this.Args[str];
    T convertedValue;
    try
    {
      convertedValue = this.ConvertStringToType<T>(str, attributeValue);
    }
    catch (Exception ex)
    {
      this.IsAttributeProblem = true;
      return (object) null;
    }
    string format = constraintChecker.Check(str, convertedValue);
    if (format == null)
      return (object) convertedValue;
    this.LogMessage("error", format);
    this.IsAttributeProblem = true;
    return (object) null;
  }

  private object ParseWoWPointArray(string attributeName, bool isRequired, string[] aliases)
  {
    bool flag = false;
    string attributeKey = this.FindAttributeKeyOrAlias(isRequired, attributeName, aliases);
    List<WoWPoint> woWpointList = new List<WoWPoint>();
    char[] separator1 = new char[2]{ ' ', ',' };
    char[] separator2 = new char[2]{ '|', ';' };
    if (attributeKey != null && this.Args.ContainsKey(attributeKey))
    {
      foreach (string coordinateGroup in this.Args[attributeKey].Split(separator2, StringSplitOptions.RemoveEmptyEntries))
      {
        string[] strArray = coordinateGroup.Split(separator1, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length != 3)
        {
          this.LogMessage("error", "The '{0}' attribute's value contribution (saw '{1}') doesn't have three coordinates (counted {2}).\nExpect entries of the form \"x1,y1,z1 | x2,y2,z2 | x3,...\", or \"x1,y1,z1; x2,y2,z2; x3,...\"", (object) attributeKey, (object) coordinateGroup, (object) strArray.Length);
          flag = true;
        }
        else
        {
          double? xCoord = new double?();
          try
          {
            xCoord = new double?(this.ConvertStringToType<double>(attributeKey, strArray[0]));
          }
          catch (Exception ex)
          {
            flag = true;
          }
          double? yCoord = new double?();
          try
          {
            yCoord = new double?(this.ConvertStringToType<double>(attributeKey, strArray[1]));
          }
          catch (Exception ex)
          {
            flag = true;
          }
          double? zCoord = new double?();
          try
          {
            zCoord = new double?(this.ConvertStringToType<double>(attributeKey, strArray[2]));
          }
          catch (Exception ex)
          {
            flag = true;
          }
          if (xCoord.HasValue && yCoord.HasValue && zCoord.HasValue)
            woWpointList.Add(new WoWPoint(xCoord.Value, yCoord.Value, zCoord.Value));
        }
      }
      if (flag)
      {
        woWpointList.Clear();
        this.IsAttributeProblem = true;
      }
      return (object) woWpointList.ToArray();
    }
    woWpointList.Clear();
    return (object) woWpointList.ToArray();
  }

  private object ParseSingleWoWPoint(string attributeBaseName, bool isRequired, string[] aliases)
  {
    if (attributeBaseName == null)
      attributeBaseName = "";
    string[] xAliases = (string[]) null;
    string[] yAliases = (string[]) null;
    string[] zAliases = (string[]) null;
    if (aliases != null)
    {
      xAliases = ((IEnumerable<string>) aliases).Select<string, string>((Func<string, string>) (alias => alias + "X")).ToArray<string>();
      yAliases = ((IEnumerable<string>) aliases).Select<string, string>((Func<string, string>) (alias => alias + "Y")).ToArray<string>();
      zAliases = ((IEnumerable<string>) aliases).Select<string, string>((Func<string, string>) (alias => alias + "Z")).ToArray<string>();
    }
    string xAttributeKey = this.FindAttributeKeyOrAlias(false, attributeBaseName + "X", xAliases);
    string yAttributeKey = this.FindAttributeKeyOrAlias(false, attributeBaseName + "Y", yAliases);
    string zAttributeKey = this.FindAttributeKeyOrAlias(false, attributeBaseName + "Z", zAliases);
    string foundAttributeKey = xAttributeKey ?? yAttributeKey ?? zAttributeKey;
    string finalBaseName;
    if (foundAttributeKey != null)
    {
      finalBaseName = foundAttributeKey.Substring(0, foundAttributeKey.Length - 1);
      isRequired = true;
    }
    else
      finalBaseName = attributeBaseName;
    double? nullable1 = (double?) this.GetAttributeValueAsObject<double>(finalBaseName + "X", isRequired, (CustomForcedBehavior.IConstraintChecker<double>) null, (string[]) null);
    double? nullable2 = (double?) this.GetAttributeValueAsObject<double>(finalBaseName + "Y", isRequired, (CustomForcedBehavior.IConstraintChecker<double>) null, (string[]) null);
    double? nullable3 = (double?) this.GetAttributeValueAsObject<double>(finalBaseName + "Z", isRequired, (CustomForcedBehavior.IConstraintChecker<double>) null, (string[]) null);
    return nullable1.HasValue && nullable2.HasValue && nullable3.HasValue ? (object) new WoWPoint(nullable1.Value, nullable2.Value, nullable3.Value) : (object) null;
  }

  private bool IsKeyMatchingBasePattern(string baseName, string key, bool isWoWPoint)
  {
    if (!key.StartsWith(baseName))
      return false;
    string s = key.Substring(baseName.Length);
    if (isWoWPoint && (s.EndsWith("X") || s.EndsWith("Y") || s.EndsWith("Z")))
      s = s.Substring(0, s.Length - 1);
    return s.Length == 0 || int.TryParse(s, out int _);
  }

  private T ConvertStringToType<T>(string attributeName, string stringValue)
  {
    Type type = typeof (T);
    if (type == typeof (bool))
    {
      int result;
      if (int.TryParse(stringValue, out result))
      {
        stringValue = result != 0 ? "true" : "false";
        this.LogMessage("warning", "Attribute's '{0}' value was provided as an integer (saw '{1}')--a boolean was expected.\nThe integral value '{1}' was converted to Boolean({2}).\nPlease update the profile to provide '{2}' for this value.", (object) attributeName, (object) result, (object) stringValue);
      }
    }
    else if (type.IsEnum)
    {
      bool flag = true;
      T enumValue = default (T);
      try
      {
        enumValue = (T) Enum.Parse(type, stringValue, true);
      }
      catch (Exception ex)
      {
        flag = false;
      }
      if (flag && Enum.IsDefined(type, (object) enumValue))
      {
        int result;
        if (int.TryParse(stringValue, out result))
          this.LogMessage("warning", "The '{0}' attribute's value '{1}' has been implicitly converted to the corresponding enumeration '{2}'.\nPlease use the enumeration name '{2}' instead of a number.", (object) attributeName, (object) result, (object) enumValue.ToString());
        return enumValue;
      }
      this.LogMessage("error", "The value '{0}' is not a member of the {1} enumeration.", (object) stringValue, (object) type.Name);
      return default (T);
    }
    T convertedValue;
    try
    {
      convertedValue = (T) Convert.ChangeType((object) stringValue, type, System.Globalization.CultureInfo.InvariantCulture);
    }
    catch (Exception ex)
    {
      this.LogMessage("error", "The '{0}' attribute's value (saw '{1}') is malformed. ({2})", (object) attributeName, (object) stringValue, (object) ex.GetType().Name);
      throw;
    }
    return convertedValue;
  }

  public bool IsAttributeProblem
  {
    get => this._hasAttributeProblem;
    protected set
    {
      if (!value)
        return;
      this._hasAttributeProblem = true;
    }
  }

  public void OnStart_HandleAttributeProblem()
  {
    this.WarnUnusedAttributes();
    if (this.IsAttributeProblem)
    {
      this.LogMessage("error", "Stopping CopilotBuddy.  Please repair the profile!");
      TreeRoot.Stop();
    }
    else
    {
      if (!this.IsDone)
        return;
      this.LogMessage("debug", "Behavior sees 'done'.  Skipping behavior.");
    }
  }

  public void LogMessage(
    string messageType,
    Color? messageColor,
    string format,
    params object[] args)
  {
    string str = "";
    if (this.Element != null && ((IXmlLineInfo) this.Element).HasLineInfo())
      str = " @line " + ((IXmlLineInfo) this.Element).LineNumber.ToString();
    if (!string.IsNullOrEmpty(messageType))
      messageType = $"({messageType.Trim().ToLower()})";
    if (!messageColor.HasValue)
    {
      switch (messageType)
      {
        case "(debug)":
          messageColor = new Color?(Color.LimeGreen);
          break;
        case "(error)":
          messageColor = new Color?(Color.Red);
          break;
        case "(fatal)":
          messageColor = new Color?(Color.Red);
          break;
        case "(warning)":
          messageColor = new Color?(Color.DarkOrange);
          break;
        case "(info)":
          messageColor = new Color?(Color.CornflowerBlue);
          break;
      }
    }
    messageColor = new Color?(messageColor ?? Color.DarkGray);
    string message = $"[{this._elementName}{messageType}{str}]: " + string.Format(format, args);
    if (messageType == "(debug)")
      Logging.WriteDebug(messageColor.Value, message);
    else
      Logging.Write(messageColor.Value, message);
    if (!(messageType == "(fatal)"))
      return;
    Logging.Write(Color.Red, "Fatal error. Stopping CopilotBuddy.");
    TreeRoot.Stop();
  }

  public void LogMessage(string messageType, string format, params object[] args)
  {
    this.LogMessage(messageType, new Color?(), format, args);
  }

  public bool UtilIsProgressRequirementsMet(
    int questId,
    CustomForcedBehavior.QuestInLogRequirement questInLogRequirement,
    CustomForcedBehavior.QuestCompleteRequirement questCompleteRequirement)
  {
    if (this.IsAttributeProblem)
      return false;
    if (questId == 0)
      return true;
    PlayerQuest questById = StyxWoW.Me.QuestLog.GetQuestById((uint) questId);
    if (questInLogRequirement == CustomForcedBehavior.QuestInLogRequirement.InLog && questById == null || questInLogRequirement == CustomForcedBehavior.QuestInLogRequirement.NotInLog && questById != null)
      return false;
    bool flag = questById != null && questById.IsCompleted || ObjectManager.Me.QuestLog.GetCompletedQuests().Contains((uint) questId);
    return (questCompleteRequirement != CustomForcedBehavior.QuestCompleteRequirement.Complete || flag) && (questCompleteRequirement != CustomForcedBehavior.QuestCompleteRequirement.NotComplete || !flag);
  }

  private int CountAttributeAliasesFound(string attributeName, string[] aliases)
  {
    int num = 0;
    if (!string.IsNullOrEmpty(attributeName))
      num += this.Args.ContainsKey(attributeName) ? 1 : 0;
    if (aliases != null)
      num += ((IEnumerable<string>) aliases).Where<string>((Func<string, bool>) (alias => this.Args.ContainsKey(alias))).Count<string>();
    return num;
  }

  private string FindAttributeKeyOrAlias(bool isRequired, string attributeName, string[] aliases)
  {
    this.MarkAttributeAsProcessed(attributeName, aliases);
    if (this.CountAttributeAliasesFound(attributeName, aliases) > 1)
    {
      List<string> stringList = new List<string>();
      stringList.Add(attributeName);
      stringList.AddRange((IEnumerable<string>) aliases);
      stringList.Sort();
      this.LogMessage("error", "The attributes [{0}] are aliases for each other, and thus mutually exclusive.\nPlease specify the attribute by its preferred name '{1}'.", (object) $"'{string.Join("', '", stringList.ToArray())}'", (object) attributeName);
      this.IsAttributeProblem = true;
      return (string) null;
    }
    if (!string.IsNullOrEmpty(attributeName) && this.Args.ContainsKey(attributeName))
      return attributeName;
    if (aliases != null)
    {
      string str = ((IEnumerable<string>) aliases).Where<string>((Func<string, bool>) (alias => !string.IsNullOrEmpty(alias) && this.Args.ContainsKey(alias))).FirstOrDefault<string>();
      if (!string.IsNullOrEmpty(str))
      {
        this.LogMessage("warning", "Found attribute via its alias name '{0}'.\nPlease update the profile to use its primary name '{1}', instead.", (object) str, (object) attributeName);
        return str;
      }
    }
    if (isRequired)
    {
      this.LogMessage("error", "Attribute '{0}' is required, but was not provided.", (object) attributeName);
      this.IsAttributeProblem = true;
    }
    return (string) null;
  }

  private void MarkAttributeAsProcessed(string attributeName, string[] aliases)
  {
    if (!string.IsNullOrEmpty(attributeName) && !this._processedAttributes.Contains(attributeName))
      this._processedAttributes.Add(attributeName);
    if (aliases == null)
      return;
    foreach (string str in aliases)
    {
      if (!this._processedAttributes.Contains(str))
        this._processedAttributes.Add(str);
    }
  }

  private void WarnUnusedAttributes()
  {
    foreach (object obj in (IEnumerable<string>) this.Args.Keys.Where<string>((Func<string, bool>) (key => !this._processedAttributes.Contains(key))).OrderBy<string, string>((Func<string, string>) (key => key)))
      this.LogMessage("warning", "Attribute '{0}' is not recognized by this behavior--ignoring it.", obj);
  }

  public virtual string SubversionId => "Unknown";

  public virtual string SubversionRevision => "Unknown";

  // Mirrors HB xelement_1: set by ForcedCodeBehavior before Activator.CreateInstance so the
  // behavior constructor can access Element immediately (e.g. to read CDATA).
  [ThreadStatic]
  internal static XElement PendingElement;

  protected CustomForcedBehavior(Dictionary<string, string> args)
  {
    this._args = args;
    this._elementName = $"{this.GetType().Name}-v{this.ExtractVersionFromSubversionTag(this.SubversionRevision)}";
    this.Element = PendingElement;
  }

  public XElement Element { get; set; }

  public Dictionary<string, string> Args => this._args;

  public Composite Branch
  {
    get
    {
      Composite branch = this._behaviorTreeHook;
      if ((object) branch == null)
        branch = this._behaviorTreeHook = this.CreateBehavior();
      return branch;
    }
  }

  public abstract bool IsDone { get; }

  protected virtual Composite CreateBehavior()
  {
    return (Composite) new Decorator((CanRunDecoratorDelegate) (_ => this.Execute()), (Composite) new TreeSharp.Action((ActionSucceedDelegate) (_ => { })));
  }

  protected virtual bool Execute() => true;

  public virtual void OnStart()
  {
  }

  public virtual void OnTick()
  {
  }

  public virtual void Dispose()
  {
  }

  private string ExtractVersionFromSubversionTag(string subversionTag)
  {
    return CustomForcedBehavior.VariableSubstitutionRegex.Replace(subversionTag, "$1").Trim();
  }

  [Obsolete("Use GetAttributeAsNullable<boolean>(attributeName, isAttributeRequired, null, attributeNameAliases), instead")]
  public bool? GetAttributeAsBoolean(
    string attributeName,
    bool isAttributeRequired,
    string[] attributeNameAliases)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAsNullable<boolean>(\"{attributeName}\", {isAttributeRequired}, null, {(attributeNameAliases == null ? (object) "null" : (object) "ATTRIBUTE_ALIASES")})");
    string attributeKey = this.FindAttributeKeyOrAlias(isAttributeRequired, attributeName, attributeNameAliases);
    if (attributeKey == null)
      return new bool?();
    string attributeValue = this.Args[attributeKey];
    int intParseResult;
    bool boolParseResult;
    if (int.TryParse(attributeValue, out intParseResult))
    {
      boolParseResult = intParseResult != 0;
      this.LogMessage("warning", "Attribute's '{0}' value was provided as an integer (saw '{1}')--a boolean was expected.\nThe integral value '{1}' was converted to Boolean({2}).\nPlease update the profile to provide 'true' or 'false' for this value", (object) attributeKey, (object) attributeValue, (object) boolParseResult);
    }
    else if (!bool.TryParse(attributeValue, out boolParseResult))
    {
      this.UtilReportMalformed(attributeKey, attributeValue);
      return new bool?();
    }
    return new bool?(boolParseResult);
  }

  [Obsolete("Use GetAttributeAsNullable<Color>(attributeName, isAttributeRequired, null, attributeNameAliases), instead")]
  public Color? GetAttributeAsColor(
    string attributeName,
    bool isAttributeRequired,
    string[] attributeNameAliases)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAsNullable<Color>(\"{attributeName}\", {isAttributeRequired}, null, {(attributeNameAliases == null ? (object) "null" : (object) "ATTRIBUTE_ALIASES")})");
    KnownColor? attributeAsEnum = this.GetAttributeAsEnum<KnownColor>(attributeName, isAttributeRequired, attributeNameAliases);
    return attributeAsEnum.HasValue ? new Color?(Color.FromKnownColor(attributeAsEnum.Value)) : new Color?();
  }

  [Obsolete("Use GetAttributeAsNullable<double>(attributeName, isAttributeRequired, new ConstrainTo.Domain<double>(lowerBound, upperBound), attributeNameAliases), instead")]
  public double? GetAttributeAsDouble(
    string attributeName,
    bool isAttributeRequired,
    double minValue,
    double maxValue,
    string[] attributeNameAliases)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAsNullable<double>(\"{attributeName}\", {isAttributeRequired}, {(minValue != double.MinValue || maxValue != double.MaxValue ? (object) $"new ConstrainTo.Domain<double>({minValue}, {maxValue})" : (object) "null")}, {(attributeNameAliases == null ? (object) "null" : (object) "ATTRIBUTE_ALIASES")})");
    string attributeKey = this.FindAttributeKeyOrAlias(isAttributeRequired, attributeName, attributeNameAliases);
    return attributeKey != null ? this.ParseDoubleWithRange(attributeKey, this.Args[attributeKey], minValue, maxValue) : new double?();
  }

  [Obsolete("Use GetAttributeAsArray<double>(attributeName, isAttributeRequired, new ConstrainTo.Domain<double>(lowerBound, upperBound), attributeNameAliases, null), instead")]
  public double[] GetAttributeAsDoubleArray(
    string attributeName,
    bool isAttributeRequired,
    double minValue,
    double maxValue,
    string[] attributeNameAliases)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAsArray<double>(\"{attributeName}\", {isAttributeRequired}, {(minValue != double.MinValue || maxValue != double.MaxValue ? (object) $"new ConstrainTo.Domain<double>({minValue}, {maxValue})" : (object) "null")}, {(attributeNameAliases == null ? (object) "null" : (object) "ATTRIBUTE_ALIASES")}, null)");
    string attributeKey = this.FindAttributeKeyOrAlias(isAttributeRequired, attributeName, attributeNameAliases);
    if (attributeKey == null)
      return (double[]) null;
    List<double> doubleList = new List<double>();
    bool flag = false;
    string attributeValue = this.Args[attributeKey];
    char[] separator = new char[2]{ ' ', ',' };
    foreach (string segment in attributeValue.Split(separator, StringSplitOptions.RemoveEmptyEntries))
    {
      double? nullable = this.ParseDoubleWithRange(attributeKey, segment, minValue, maxValue);
      if (nullable.HasValue)
        doubleList.Add(nullable.Value);
      else
        flag = true;
    }
    return !flag ? doubleList.ToArray() : (double[]) null;
  }

  [Obsolete("Use GetAttributeAsNullable<T>(attributeName, isAttributeRequired, null, attributeNameAliases), instead")]
  public T? GetAttributeAsEnum<T>(
    string attributeName,
    bool isAttributeRequired,
    string[] attributeNameAliases)
    where T : struct
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAsNullable<{typeof (T).GetType().Name}>(\"{attributeName}\", {isAttributeRequired}, null, {(attributeNameAliases == null ? (object) "null" : (object) "ATTRIBUTE_ALIASES")})");
    Type enumType = typeof (T);
    if (!enumType.IsEnum)
      throw new ArgumentException("Type parameter must be an enum.");
    string attributeKey = this.FindAttributeKeyOrAlias(isAttributeRequired, attributeName, attributeNameAliases);
    if (attributeKey == null)
      return new T?();
    T? attributeAsEnum;
    try
    {
      string s = this.Args[attributeKey];
      T obj = (T) Enum.Parse(enumType, s, true);
      if (Enum.IsDefined(enumType, (object) obj))
      {
        int result;
        if (int.TryParse(s, out result))
          this.LogMessage("warning", "The '{0}' attribute's value (saw '{1}') has been implicitly converted to the corresponding enumeration '{2}'.\nPlease repair the profile to use the enumeration name '{2}' instead of a number.", (object) attributeKey, (object) result, (object) obj.ToString());
        attributeAsEnum = new T?(obj);
        goto label_10;
      }
    }
    catch (ArgumentException ex)
    {
    }
    this.UtilReportValueFail(attributeKey, this.Args[attributeKey], $"'{string.Join("', '", Enum.GetNames(enumType))}'");
    return new T?();
label_10:
    return attributeAsEnum;
  }

  [Obsolete("Use GetAttributeAsNullable<int>(attributeName, isAttributeRequired, ConstrainAs.FactionId, attributeNameAliases), instead")]
  public int? GetAttributeAsFactionId(
    string attributeName,
    bool isAttributeRequired,
    string[] attributeNameAliases)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAsNullable<int>(\"{attributeName}\", {isAttributeRequired}, ConstrainAs.FactionId, {(attributeNameAliases == null ? (object) "null" : (object) "ATTRIBUTE_ALIASES")})");
    return this.GetAttributeAsInteger(attributeName, isAttributeRequired, 1, int.MaxValue, attributeNameAliases);
  }

  [Obsolete("Use GetAttributeAsNullable<int>(attributeName, isAttributeRequired, ConstrainAs.HotbarButton, attributeNameAliases), instead")]
  public int? GetAttributeAsHotbarButton(
    string attributeName,
    bool isAttributeRequired,
    string[] attributeNameAliases)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAsNullable<int>(\"{attributeName}\", {isAttributeRequired}, ConstrainAs.HotbarButton, {(attributeNameAliases == null ? (object) "null" : (object) "ATTRIBUTE_ALIASES")})");
    return this.GetAttributeAsInteger(attributeName, isAttributeRequired, 1, 12, attributeNameAliases);
  }

  [Obsolete("Use GetAttributeAsNullable<int>(attributeName, isAttributeRequired, new ConstrainTo.Domain<int>(lowerBound, upperBound), attributeNameAliases), instead")]
  public int? GetAttributeAsInteger(
    string attributeName,
    bool isAttributeRequired,
    int minValue,
    int maxValue,
    string[] attributeNameAliases)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAsNullable<int>(\"{attributeName}\", {isAttributeRequired}, {(minValue != int.MinValue || maxValue != int.MaxValue ? (object) $"new ConstrainTo.Domain<int>({minValue}, {maxValue})" : (object) "null")}, {(attributeNameAliases == null ? (object) "null" : (object) "ATTRIBUTE_ALIASES")})");
    string str = this.FindAttributeKeyOrAlias(isAttributeRequired, attributeName, attributeNameAliases);
    return str != null ? this.ParseIntWithRange(str, this.Args[str], minValue, maxValue) : new int?();
  }

  [Obsolete("Use GetAttributeAsArray<int>(attributeName, isAttributeRequired, new ConstrainTo.Domain<int>(lowerBound, upperBound), attributeNameAliases, null), instead")]
  public int[] GetAttributeAsIntegerArray(
    string attributeName,
    bool isAttributeRequired,
    int minValue,
    int maxValue,
    string[] attributeNameAliases)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAsArray<int>(\"{attributeName}\", {isAttributeRequired}, {(minValue != int.MinValue || maxValue != int.MaxValue ? (object) $"new ConstrainTo.Domain<int>({minValue}, {maxValue})" : (object) "null")}, {(attributeNameAliases == null ? (object) "null" : (object) "ATTRIBUTE_ALIASES")}, null)");
    string attributeKey = this.FindAttributeKeyOrAlias(isAttributeRequired, attributeName, attributeNameAliases);
    if (attributeKey == null)
      return (int[]) null;
    bool flag = false;
    List<int> intList = new List<int>();
    string attributeValue = this.Args[attributeKey];
    char[] separator = new char[2]{ ' ', ',' };
    foreach (string segment in attributeValue.Split(separator, StringSplitOptions.RemoveEmptyEntries))
    {
      int? nullable = this.ParseIntWithRange(attributeKey, segment, minValue, maxValue);
      if (nullable.HasValue)
        intList.Add(nullable.Value);
      else
        flag = true;
    }
    return !flag ? intList.ToArray() : (int[]) null;
  }

  [Obsolete("Use GetAttributeAsNullable<int>(attributeName, isAttributeRequired, ConstrainAs.ItemId, attributeNameAliases), instead")]
  public int? GetAttributeAsItemId(
    string attributeName,
    bool isAttributeRequired,
    string[] attributeNameAliases)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAsNullable<int>(\"{attributeName}\", {isAttributeRequired}, ConstrainAs.ItemId, {(attributeNameAliases == null ? (object) "null" : (object) "ATTRIBUTE_ALIASES")})");
    return this.GetAttributeAsInteger(attributeName, isAttributeRequired, 1, int.MaxValue, attributeNameAliases);
  }

  [Obsolete("Use GetAttributeAsNullable<int>(attributeName, isAttributeRequired, ConstrainAs.MobId, attributeNameAliases), instead")]
  public int? GetAttributeAsMobId(
    string attributeName,
    bool isAttributeRequired,
    string[] attributeNameAliases)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAsNullable<int>(\"{attributeName}\", {isAttributeRequired}, ConstrainAs.MobId, {(attributeNameAliases == null ? (object) "null" : (object) "ATTRIBUTE_ALIASES")})");
    return this.GetAttributeAsInteger(attributeName, isAttributeRequired, 1, int.MaxValue, attributeNameAliases);
  }

  [Obsolete("Use GetAttributeAsNullable<int>(attributeName, isAttributeRequired, ConstrainAs.RepeatCount, attributeNameAliases), instead")]
  public int? GetAttributeAsNumOfTimes(
    string attributeName,
    bool isAttributeRequired,
    string[] attributeNameAliases)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAsNullable<int>(\"{attributeName}\", {isAttributeRequired}, ConstrainAs.RepeatCount, {(attributeNameAliases == null ? (object) "null" : (object) "ATTRIBUTE_ALIASES")})");
    return this.GetAttributeAsInteger(attributeName, isAttributeRequired, 1, 1000, attributeNameAliases);
  }

  [Obsolete("Use GetAttributeAsNullable<int>(attributeName, isAttributeRequired, ConstrainAs.QuestId, attributeNameAliases), instead")]
  public int? GetAttributeAsQuestId(
    string attributeName,
    bool isAttributeRequired,
    string[] attributeNameAliases)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAsNullable<int>(\"{attributeName}\", {isAttributeRequired}, ConstrainAs.QuestId(this), {(attributeNameAliases == null ? (object) "null" : (object) "ATTRIBUTE_ALIASES")})");
    int? attributeAsInteger = this.GetAttributeAsInteger(attributeName, isAttributeRequired, 0, int.MaxValue, attributeNameAliases);
    if (!attributeAsInteger.HasValue || attributeAsInteger.Value != 0)
      return attributeAsInteger;
    this.LogMessage("warning", new Color?(Color.Red), "The '{0}' attribute's value may not be zero.  For now, we allow it for purposes of backward compatibility; however, it will be ignored.  In a future release, a QuestId of zero will be explicitly disallowed.", (object) attributeName);
    return new int?();
  }

  [Obsolete("Use GetAttributeAsNullable<double>(attributeName, isAttributeRequired, ConstrainAs.Range, attributeNameAliases), instead")]
  public int? GetAttributeAsRange(
    string attributeName,
    bool isAttributeRequired,
    string[] attributeNameAliases)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAsNullable<double>(\"{attributeName}\", {isAttributeRequired}, ConstrainAs.Range, {(attributeNameAliases == null ? (object) "null" : (object) "ATTRIBUTE_ALIASES")})");
    return this.GetAttributeAsInteger(attributeName, isAttributeRequired, 1, 10000, attributeNameAliases);
  }

  [Obsolete("Use GetAttributeAsNullable<int>(attributeName, isAttributeRequired, ConstrainAs.SpellId, attributeNameAliases), instead")]
  public int? GetAttributeAsSpellId(
    string attributeName,
    bool isAttributeRequired,
    string[] attributeNameAliases)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAsNullable<int>(\"{attributeName}\", {isAttributeRequired}, ConstrainAs.SpellId, {(attributeNameAliases == null ? (object) "null" : (object) "ATTRIBUTE_ALIASES")})");
    return this.GetAttributeAsInteger(attributeName, isAttributeRequired, 1, int.MaxValue, attributeNameAliases);
  }

  [Obsolete("Use GetAttributeAs<string>(attributeName, isAttributeRequired, null, attributeNameAliases), instead")]
  public string GetAttributeAsString(
    string attributeName,
    bool isAttributeRequired,
    string[] attributeNameAliases)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAs<string>(\"{attributeName}\", {isAttributeRequired}, null, {(attributeNameAliases == null ? (object) "null" : (object) "ATTRIBUTE_ALIASES")})");
    string key = this.FindAttributeKeyOrAlias(isAttributeRequired, attributeName, attributeNameAliases);
    return key != null ? this.Args[key] : (string) null;
  }

  [Obsolete("Use GetAttributeAs<string>(attributeName, isAttributeRequired, ConstrainAs.StringNonEmpty, attributeNameAliases), instead")]
  public string GetAttributeAsString_NonEmpty(
    string attributeName,
    bool isAttributeRequired,
    string[] attributeNameAliases)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAs<string>(\"{attributeName}\", {isAttributeRequired}, ConstrainAs.StringNonEmpty, {(attributeNameAliases == null ? (object) "null" : (object) "ATTRIBUTE_ALIASES")})");
    string attributeAsString = this.GetAttributeAsString(attributeName, isAttributeRequired, attributeNameAliases);
    if (attributeAsString == null || !string.IsNullOrEmpty(attributeAsString))
      return attributeAsString;
    this.LogMessage("error", "The '{0}' attribute's value may not be an empty string (\"\").", (object) attributeName);
    this.IsAttributeProblem = true;
    return (string) null;
  }

  [Obsolete("Use GetAttributeAs<string>(attributeName, isAttributeRequired, new ConstrainTo.SpecificValues<string>(...), attributeNameAliases), instead")]
  public string GetAttributeAsString_SpecificValue(
    string attributeName,
    bool isAttributeRequired,
    string[] allowedValues,
    string[] attributeNameAliases)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAs<string>(\"{attributeName}\", {isAttributeRequired}, ConstrainTo.SpecificValues<string>(ALLOWED_VALUES), {(attributeNameAliases == null ? (object) "null" : (object) "ATTRIBUTE_ALIASES")})");
    string attributeAsString = this.GetAttributeAsString(attributeName, isAttributeRequired, attributeNameAliases);
    if (attributeAsString == null || ((IEnumerable<string>) allowedValues).Contains<string>(attributeAsString))
      return attributeAsString;
    this.UtilReportValueFail(attributeName, attributeAsString, $"'{string.Join("', '", allowedValues)}'");
    return (string) null;
  }

  [Obsolete("Use GetAttributeAsNullable<int>(attributeName, isAttributeRequired, ConstrainAs.Milliseconds, attributeNameAliases), instead")]
  public int? GetAttributeAsWaitTime(
    string attributeName,
    bool isAttributeRequired,
    string[] attributeNameAliases)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAsNullable<int>(\"{attributeName}\", {isAttributeRequired}, ConstrainAs.Milliseconds, {(attributeNameAliases == null ? (object) "null" : (object) "ATTRIBUTE_ALIASES")})");
    return this.GetAttributeAsInteger(attributeName, isAttributeRequired, 0, int.MaxValue, attributeNameAliases);
  }

  [Obsolete("Use GetAttributeAsNullable<WoWPoint>(attributeName, isAttributeRequired, null, attributeNameAliases), instead")]
  public WoWPoint? GetXYZAttributeAsWoWPoint(
    string attributeBaseName,
    bool isAttributeRequired,
    string[] attributeBaseNameAliases)
  {
    if (attributeBaseName == null)
      attributeBaseName = "";
    this.LogDeprecatedMethodUsage($"GetAttributeAsNullable<WoWPoint>(\"{attributeBaseName}\", {isAttributeRequired}, ConstrainAs.WoWPointNonEmpty, {(attributeBaseNameAliases == null ? (object) "null" : (object) "ATTRIBUTE_ALIASES")})");
    string[] xAliases = (string[]) null;
    string[] yAliases = (string[]) null;
    string[] zAliases = (string[]) null;
    if (attributeBaseNameAliases != null)
    {
      xAliases = ((IEnumerable<string>) attributeBaseNameAliases).Select<string, string>((Func<string, string>) (alias => alias + "X")).ToArray<string>();
      yAliases = ((IEnumerable<string>) attributeBaseNameAliases).Select<string, string>((Func<string, string>) (alias => alias + "Y")).ToArray<string>();
      zAliases = ((IEnumerable<string>) attributeBaseNameAliases).Select<string, string>((Func<string, string>) (alias => alias + "Z")).ToArray<string>();
    }
    string xAttributeKey = this.FindAttributeKeyOrAlias(false, attributeBaseName + "X", xAliases);
    string yAttributeKey = this.FindAttributeKeyOrAlias(false, attributeBaseName + "Y", yAliases);
    string zAttributeKey = this.FindAttributeKeyOrAlias(false, attributeBaseName + "Z", zAliases);
    string foundAttributeKey = xAttributeKey ?? yAttributeKey ?? zAttributeKey;
    string finalBaseName;
    if (foundAttributeKey != null)
    {
      finalBaseName = foundAttributeKey.Substring(0, foundAttributeKey.Length - 1);
      isAttributeRequired = true;
    }
    else
      finalBaseName = attributeBaseName;
    double? xCoord = this.GetAttributeAsDouble(finalBaseName + "X", isAttributeRequired, double.MinValue, double.MaxValue, (string[]) null);
    double? yCoord = this.GetAttributeAsDouble(finalBaseName + "Y", isAttributeRequired, double.MinValue, double.MaxValue, (string[]) null);
    double? zCoord = this.GetAttributeAsDouble(finalBaseName + "Z", isAttributeRequired, double.MinValue, double.MaxValue, (string[]) null);
    return xCoord.HasValue && yCoord.HasValue && zCoord.HasValue ? new WoWPoint?(new WoWPoint(xCoord.Value, yCoord.Value, zCoord.Value)) : new WoWPoint?();
  }

  [Obsolete("Use GetAttributeAsArray<WoWPoint>(attributeName, isAttributeRequired, null, attributeNameAliases, null), instead")]
  public WoWPoint[] GetAttributeAsWoWPoints(
    string attributeName,
    bool isAttributeRequired,
    string[] attributeNameAliases)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAsArray<WoWPoint>(\"{attributeName}\", {isAttributeRequired}, ConstrainAs.WoWPointNonEmpty, {(attributeNameAliases == null ? (object) "null" : (object) "ATTRIBUTE_ALIASES")}, null)");
    string attributeKey = this.FindAttributeKeyOrAlias(isAttributeRequired, attributeName, attributeNameAliases);
    if (attributeKey == null)
      return (WoWPoint[]) null;
    bool flag = false;
    List<WoWPoint> woWpointList = new List<WoWPoint>();
    char[] separator1 = new char[1]{ '|' };
    char[] separator2 = new char[2]{ ' ', ',' };
    foreach (string coordinateGroup in this.Args[attributeKey].Split(separator1, StringSplitOptions.RemoveEmptyEntries))
    {
      string[] strArray = coordinateGroup.Split(separator2, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length != 3)
      {
        this.LogMessage("error", "The '{0}' attribute's value contribution (saw '{1}') doesn't have three coordinates (counted {2}).\nExpect entries of the form \"x1,y1,z1 | x2,y2,z2  | x3,...\"", (object) attributeKey, (object) coordinateGroup, (object) strArray.Length);
        flag = true;
      }
      else
      {
        double? nullable1 = this.ParseDoubleWithRange(attributeKey, strArray[0], double.MinValue, double.MaxValue);
        double? nullable2 = this.ParseDoubleWithRange(attributeKey, strArray[1], double.MinValue, double.MaxValue);
        double? nullable3 = this.ParseDoubleWithRange(attributeKey, strArray[2], double.MinValue, double.MaxValue);
        if (nullable1.HasValue && nullable2.HasValue && nullable3.HasValue)
          woWpointList.Add(new WoWPoint(nullable1.Value, nullable2.Value, nullable3.Value));
        else
          flag = true;
      }
    }
    if (flag)
      this.IsAttributeProblem = true;
    return !flag ? woWpointList.ToArray() : (WoWPoint[]) null;
  }

  private bool IsKeyMatchingBaseName(string baseName, string key)
  {
    if (!key.StartsWith(baseName))
      return false;
    string s = key.Substring(baseName.Length);
    return s.Length == 0 || int.TryParse(s, out int _);
  }

  private void ProcessNumberedIntegerAttributes(
    IEnumerable<string> attributeKeys,
    string preferredBaseName,
    int minValue,
    int maxValue,
    ref List<int> resultList)
  {
    foreach (string attributeKey in attributeKeys)
    {
      string str = this.FindAttributeKeyOrAlias(false, attributeKey, (string[]) null);
      int? nullable = this.ParseIntWithRange(str, this.Args[str], minValue, maxValue);
      if (nullable.HasValue)
      {
        if (!attributeKey.StartsWith(preferredBaseName))
          this.LogMessage("warning", "The attribute '{0}' is an alias for '{1}N'.\nPlease modify the profile to use the preferred base name of '{1}', instead.", (object) attributeKey, (object) preferredBaseName);
        resultList.Add(nullable.Value);
      }
      else
        this.IsAttributeProblem = true;
    }
  }

  [Obsolete("Use GetNumberedAttributesAsArray<int>(attributeName, isAttributeRequired, new ConstainTo.Domain(lowerBound, upperBound), attributeNameAliases), instead")]
  public int[] GetNumberedAttributesAsIntegerArray(
    string baseName,
    int countRequired,
    int minValue,
    int maxValue,
    string[] aliasBaseNames)
  {
    string capturedBaseName = baseName;
    CustomForcedBehavior capturedThis = this;

    this.LogDeprecatedMethodUsage($"GetNumberedAttributesAsArray<int>(\"{capturedBaseName}\", {countRequired}, {(minValue != int.MinValue || maxValue != int.MaxValue ? (object) $"new ConstrainTo.Domain<int>({minValue}, {maxValue})" : (object) "null")}, {(aliasBaseNames == null ? (object) "null" : (object) "ATTRIBUTE_ALIASES")}, null)");

    List<int> resultList = new List<int>();

    // Filter keys matching base name pattern
    this.ProcessNumberedIntegerAttributes(this.Args.Keys.Where<string>(key => capturedThis.IsKeyMatchingBaseName(capturedBaseName, key)), capturedBaseName, minValue, maxValue, ref resultList);

    if (aliasBaseNames != null)
    {
      this.ProcessNumberedIntegerAttributes(((IEnumerable<string>) aliasBaseNames)
        .SelectMany<string, string, AttributeNamePair<string, string>>(
          (Func<string, IEnumerable<string>>) (alias => (IEnumerable<string>) this.Args.Keys),
          (Func<string, string, AttributeNamePair<string, string>>) ((alias, key) => new AttributeNamePair<string, string>(alias, key)))
        .Where<AttributeNamePair<string, string>>(pair => this.IsKeyMatchingBaseName(pair.aliasBaseName, pair.attributeName))
        .Select<AttributeNamePair<string, string>, string>((Func<AttributeNamePair<string, string>, string>) (pair => pair.attributeName)),
        capturedBaseName, minValue, maxValue, ref resultList);
    }

    if (resultList.Count == 0)
      return (int[]) null;
    if (resultList.Count >= countRequired)
      return resultList.ToArray();

    this.LogMessage("error", "The attribute '{0}N' must be provided at least {1} times (saw it '{2}' times).\n(E.g., MobId1, MobId2, MobId3, ...)\nPlease modify the profile to supply {1} attributes with a base name of '{0}'.", (object) capturedBaseName, (object) countRequired, (object) resultList.Count);
    return (int[]) null;
  }

  public WoWPoint? LegacyGetAttributeAsWoWPoint(
    string attributeName,
    bool isRequired,
    string[] attributeAliases,
    string preferredName)
  {
    double[] attributeAsArray = this.GetAttributeAsArray<double>(attributeName, isRequired, (CustomForcedBehavior.IConstraintChecker<double>) null, attributeAliases, (char[]) null);
    if (attributeAsArray == null || attributeAsArray.Length == 0)
      return new WoWPoint?();
    this.LogMessage("warning", "The attribute '{0}' is DEPRECATED.\nPlease modify the profile to use the new '{1}' attribute, instead.", (object) attributeName, (object) preferredName);
    if (attributeAsArray.Length == 3)
      return new WoWPoint?(new WoWPoint(attributeAsArray[0], attributeAsArray[1], attributeAsArray[2]));
    this.LogMessage("error", "The '{0}' attribute's value should have three coordinate contributions (saw '{1}')", (object) attributeName, (object) attributeAsArray.Length);
    this.IsAttributeProblem = true;
    return new WoWPoint?();
  }

  [Obsolete]
  private double? ParseDoubleWithRange(string attributeName, string stringValue, double minValue, double maxValue)
  {
    bool flag = false;
    double result;
    if (!double.TryParse(stringValue, out result))
    {
      this.UtilReportMalformed(attributeName, stringValue);
      flag = true;
    }
    else if (result < minValue || result > maxValue)
    {
      this.UtilReportValueFail(attributeName, stringValue, $"{minValue}..{maxValue}");
      flag = true;
    }
    return !flag ? new double?(result) : new double?();
  }

  [Obsolete]
  private int? ParseIntWithRange(string attributeName, string stringValue, int minValue, int maxValue)
  {
    bool flag = false;
    int result;
    if (!int.TryParse(stringValue, out result))
    {
      this.UtilReportMalformed(attributeName, stringValue);
      flag = true;
    }
    else if (result < minValue || result > maxValue)
    {
      this.UtilReportValueFail(attributeName, stringValue, $"{minValue}..{maxValue}");
      flag = true;
    }
    return !flag ? new int?(result) : new int?();
  }

  [Obsolete("This method has been renamed to 'LogMessage'")]
  public void UtilLogMessage(string messageType, string format, params object[] args)
  {
    this.LogMessage(messageType, new Color?(), format, args);
  }

  [Obsolete("This method has been renamed to 'LogMessage'")]
  public void UtilLogMessage(
    string messageType,
    Color? messageColor,
    string format,
    params object[] args)
  {
    this.LogMessage(messageType, messageColor, format, args);
  }

  [Obsolete]
  protected void UtilReportMalformed(string attributeName, string attributeValue)
  {
    this.LogMessage("error", "The '{0}' attribute's value (saw '{1}') is malformed.", (object) attributeName, (object) attributeValue);
    this.IsAttributeProblem = true;
  }

  [Obsolete]
  protected void UtilReportValueFail(
    string attributeName,
    string attributeValue,
    string validValues)
  {
    string str = $"The '{attributeName}' attribute's value (saw '{attributeValue}') is not ";
    this.LogMessage("error", !validValues.Contains("..") ? str + $"one of the allowed values...\n    [{validValues}]." : str + $"on the closed interval [{validValues}].");
    this.IsAttributeProblem = true;
  }

  [Obsolete("Use GetAttributeAsNullable<T>(attributeName, isAttributeRequired, null, attributeNameAliases), instead")]
  public bool GetAttributeAsEnum<T>(
    string attributeName,
    bool isAttributeRequired,
    T defaultValue,
    out T returnedValue)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAsNullable<{typeof (T).GetType().Name}>(\"{attributeName}\", {isAttributeRequired}, null, ATTRIBUTE_ALIASES)");
    Type enumType = typeof (T);
    if (!enumType.IsEnum)
      throw new ArgumentException("Type parameter must be an enum.");
    returnedValue = defaultValue;
    Dictionary<string, object> dictionary = Enum.GetValues(enumType).Cast<object>().ToDictionary<object, string, object>((Func<object, string>) (enumValue => enumValue.ToString()), (Func<object, object>) (enumValue => (object) null));
    string returnedValue1;
    if (!this.GetAttributeAsSpecificString(attributeName, isAttributeRequired, defaultValue.ToString(), dictionary, out returnedValue1))
      return false;
    bool attributeAsEnum;
    try
    {
      returnedValue = (T) Enum.Parse(enumType, returnedValue1);
      if (Enum.IsDefined(enumType, (object) returnedValue))
      {
        attributeAsEnum = true;
        goto label_8;
      }
    }
    catch (ArgumentException ex)
    {
    }
    this.LogMessage("error", "Problem with '{0}' attribute--unable to convert attribute's value (saw '{1}') to an enumeration of type '{2}'.", (object) attributeName, (object) returnedValue1, (object) enumType.Name);
    return false;
label_8:
    return attributeAsEnum;
  }

  [Obsolete("Use OnStart_HandleAttributeProblem(), instead")]
  public bool CheckForUnrecognizedAttributes(Dictionary<string, object> allowedAttributes)
  {
    this.LogDeprecatedMethodUsage("OnStart_HandleAttributeProblem()");
    bool flag = false;
    foreach (KeyValuePair<string, string> keyValuePair in this.Args)
    {
      if (!allowedAttributes.ContainsKey(keyValuePair.Key))
      {
        this.LogMessage("warning", "Unrecognized attribute '{0}' (with value of '{1}') will be ignored.", (object) keyValuePair.Key, (object) keyValuePair.Value);
        flag = true;
      }
    }
    return flag;
  }

  [Obsolete("Use GetAttributeAsNullable<bool>(attributeName, isAttributeRequired, null, attributeNameAliases), instead")]
  public bool GetAttributeAsBoolean(
    string attributeName,
    bool isAttributeRequired,
    string defaultValueAsString,
    out bool returnedValue)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAsNullable<bool>(\"{attributeName}\", {isAttributeRequired}, null, ATTRIBUTE_ALIASES)");
    returnedValue = false;
    string attributeValue;
    if (!this.TryGetAttributeOrDefault(attributeName, isAttributeRequired, defaultValueAsString, out attributeValue))
      return false;
    bool result;
    if (!bool.TryParse(attributeValue, out result))
    {
      this.UtilReportValueFail(attributeName, attributeValue, "true, false");
      return false;
    }
    returnedValue = result;
    return true;
  }

  [Obsolete("Use GetAttributeAsNullable<double>(attributeName, isAttributeRequired, new ConstrainTo.Domain<double>(minValueAllowed, maxValueAllowed), attributeNameAliases), instead")]
  public bool GetAttributeAsFloat(
    string attributeName,
    bool isAttributeRequired,
    string defaultValueAsString,
    float minValueAllowed,
    float maxValueAllowed,
    out float returnedValue)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAsNullable<double>(\"{attributeName}\", {isAttributeRequired}, {((double) minValueAllowed != -3.4028234663852886E+38 || (double) maxValueAllowed != 3.4028234663852886E+38 ? (object) $"new ConstrainTo.Domain<double>({minValueAllowed}, {maxValueAllowed})" : (object) "null")}, ATTRIBUTE_ALIASES)");
    returnedValue = 0.0f;
    string attributeValue;
    if (!this.TryGetAttributeOrDefault(attributeName, isAttributeRequired, defaultValueAsString, out attributeValue))
      return false;
    float result;
    if (!float.TryParse(attributeValue, out result))
    {
      this.UtilReportMalformed(attributeName, attributeValue);
      return false;
    }
    if ((double) result >= (double) minValueAllowed && (double) result <= (double) maxValueAllowed)
    {
      returnedValue = result;
      return true;
    }
    this.UtilReportValueFail(attributeName, attributeValue, $"{minValueAllowed}..{maxValueAllowed}");
    return false;
  }

  [Obsolete("Use GetAttributeAsNullable<int>(attributeName, isAttributeRequired, new ConstrainTo.Domain<int>(minValueAllowed, maxValueAllowed), attributeNameAliases), instead")]
  public bool GetAttributeAsInteger(
    string attributeName,
    bool isAttributeRequired,
    string defaultValueAsString,
    int minValueAllowed,
    int maxValueAllowed,
    out int returnedValue)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAsNullable<int>(\"{attributeName}\", {isAttributeRequired}, {(minValueAllowed != int.MinValue || maxValueAllowed != int.MaxValue ? (object) $"new ConstrainTo.Domain<int>({minValueAllowed}, {maxValueAllowed})" : (object) "null")}, ATTRIBUTE_ALIASES)");
    returnedValue = 0;
    string attributeValue;
    if (!this.TryGetAttributeOrDefault(attributeName, isAttributeRequired, defaultValueAsString, out attributeValue))
      return false;
    int result;
    if (!int.TryParse(attributeValue, out result))
    {
      this.UtilReportMalformed(attributeName, attributeValue);
      return false;
    }
    if (result >= minValueAllowed && result <= maxValueAllowed)
    {
      returnedValue = result;
      return true;
    }
    this.UtilReportValueFail(attributeName, attributeValue, $"{minValueAllowed}..{maxValueAllowed}");
    return false;
  }

  [Obsolete("Use GetAttributeAs<string>(attributeName, isAttributeRequired, null, attributeNameAliases), instead")]
  public bool GetAttributeAsString(
    string attributeName,
    bool isAttributeRequired,
    string defaultValueAsString,
    out string returnedValue)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAs<string>(\"{attributeName}\", {isAttributeRequired}, null, ATTRIBUTE_ALIASES)");
    returnedValue = (string) null;
    string attributeValue;
    if (!this.TryGetAttributeOrDefault(attributeName, isAttributeRequired, defaultValueAsString, out attributeValue))
      return false;
    returnedValue = attributeValue;
    return true;
  }

  [Obsolete("Use GetAttributeAs<string>(attributeName, isAttributeRequired, new ConstrainTo.SpecificValues<string>(allowedValues), attributeNameAliases), instead")]
  public bool GetAttributeAsSpecificString(
    string attributeName,
    bool isAttributeRequired,
    string defaultValue,
    Dictionary<string, object> allowedValues,
    out string returnedValue)
  {
    this.LogDeprecatedMethodUsage($"GetAttributeAs<string>(\"{attributeName}\", {isAttributeRequired}, {(allowedValues == null ? (object) "null" : (object) "new ConstrainTo.SpecificValues<string>(ALLOWED_VALUES)")}, ATTRIBUTE_ALIASES)");
    if (this.GetAttributeAsString(attributeName, isAttributeRequired, defaultValue, out returnedValue) && allowedValues.ContainsKey(returnedValue))
      return true;
    string validValues = string.Join(", ", allowedValues.Keys.ToArray<string>());
    this.UtilReportValueFail(attributeName, returnedValue, validValues);
    return false;
  }

  [Obsolete("Use GetAttributeAsNullable<WoWPoint>(\"\", isAttributeRequired, null, attributeNameAliases), instead")]
  public bool GetXYZAttributeAsWoWPoint(
    bool isAttributeRequired,
    WoWPoint defaultValue,
    out WoWPoint returnedValue)
  {
    return this.GetXYZAttributeAsWoWPoint("X", "Y", "Z", isAttributeRequired, defaultValue, out returnedValue);
  }

  [Obsolete("Use GetAttributeAsNullable<WoWPoint>(attributeName, isAttributeRequired, null, attributeNameAliases), instead")]
  public bool GetXYZAttributeAsWoWPoint(
    string attributeNameX,
    string attributeNameY,
    string attributeNameZ,
    bool isAttributeRequired,
    WoWPoint defaultValue,
    out WoWPoint returnedValue)
  {
    string str = attributeNameX;
    if (str.EndsWith("X"))
      str = str.Substring(str.Length - 1);
    this.LogDeprecatedMethodUsage($"GetAttributeAsNullable<WoWPoint>(\"{str}\", {isAttributeRequired}, ConstrainAs.WoWPointNonEmpty, ATTRIBUTE_ALIASES))");
    returnedValue = new WoWPoint(defaultValue.X, defaultValue.Y, defaultValue.Z);
    string xValue = (string) null;
    string yValue = (string) null;
    string zValue = (string) null;
    if (this.Args.ContainsKey(attributeNameX) || this.Args.ContainsKey(attributeNameY) || this.Args.ContainsKey(attributeNameZ))
      isAttributeRequired = true;
    bool flag;
    if (!(flag = this.TryGetAttributeOrDefault(attributeNameX, isAttributeRequired, defaultValue.X.ToString(), out xValue) & this.TryGetAttributeOrDefault(attributeNameY, isAttributeRequired, defaultValue.Y.ToString(), out yValue) & this.TryGetAttributeOrDefault(attributeNameZ, isAttributeRequired, defaultValue.Z.ToString(), out zValue)))
      return false;
    float xParsed;
    if (!float.TryParse(xValue, out xParsed))
    {
      this.UtilReportMalformed(attributeNameX, xValue);
      return false;
    }
    float yParsed;
    if (!float.TryParse(yValue, out yParsed))
    {
      this.UtilReportMalformed(attributeNameY, yValue);
      return false;
    }
    float zParsed;
    if (!float.TryParse(zValue, out zParsed))
    {
      this.UtilReportMalformed(attributeNameZ, zValue);
      return false;
    }
    returnedValue.X = xParsed;
    returnedValue.Y = yParsed;
    returnedValue.Z = zParsed;
    return true;
  }

  private bool TryGetAttributeOrDefault(string attributeName, bool isRequired, string defaultValue, out string returnedValue)
  {
    bool flag = this.Args.TryGetValue(attributeName, out returnedValue);
    if (isRequired && !flag)
    {
      this.LogMessage("error", "The '{0}' attribute is required, but missing.  (Attribute names are case-sensitive.)", (object) attributeName);
      return false;
    }
    if (!flag)
      returnedValue = defaultValue;
    return true;
  }

  private bool ValidateRequiredAttribute(string attributeName, bool isRequired)
  {
    bool flag = true;
    if (isRequired && !this.Args.ContainsKey(attributeName))
    {
      this.LogMessage("error", "The '{0}' attribute is required, but missing.  (Attribute names are case-sensitive.)", (object) attributeName);
      flag = false;
    }
    return flag;
  }

  private void LogDeprecatedMethodUsage(string replacementMethodSignature)
  {
    string name = new StackTrace().GetFrame(1).GetMethod().Name;
    string format = "This method '{0}' is deprecated.\nPlease update your behavior to use one of the replacement methods provided by the CustomForcedBehavior class.";
    if (replacementMethodSignature != null)
      format += "Your replacement line is:\n    {1}\n";
    this.LogMessage("warning", format, (object) name, (object) replacementMethodSignature);
  }

  public static class ConstrainAs
  {
    public static CustomForcedBehavior.IConstraintChecker<int> AuraId = (CustomForcedBehavior.IConstraintChecker<int>) new CustomForcedBehavior.ConstrainTo.Domain<int>(1, int.MaxValue);
    public static CustomForcedBehavior.IConstraintChecker<int> CollectionCount = (CustomForcedBehavior.IConstraintChecker<int>) new CustomForcedBehavior.ConstrainTo.Domain<int>(1, 1000);
    public static CustomForcedBehavior.IConstraintChecker<int> FactionId = (CustomForcedBehavior.IConstraintChecker<int>) new CustomForcedBehavior.ConstrainTo.Domain<int>(1, int.MaxValue);
    public static CustomForcedBehavior.IConstraintChecker<int> HotbarButton = (CustomForcedBehavior.IConstraintChecker<int>) new CustomForcedBehavior.ConstrainTo.Domain<int>(1, 12);
    public static CustomForcedBehavior.IConstraintChecker<int> ItemId = (CustomForcedBehavior.IConstraintChecker<int>) new CustomForcedBehavior.ConstrainTo.Domain<int>(1, int.MaxValue);
    public static CustomForcedBehavior.IConstraintChecker<int> Milliseconds = (CustomForcedBehavior.IConstraintChecker<int>) new CustomForcedBehavior.ConstrainTo.Domain<int>(0, int.MaxValue);
    public static CustomForcedBehavior.IConstraintChecker<int> MobId = (CustomForcedBehavior.IConstraintChecker<int>) new CustomForcedBehavior.ConstrainTo.Domain<int>(1, int.MaxValue);
    public static CustomForcedBehavior.IConstraintChecker<int> ObjectId = (CustomForcedBehavior.IConstraintChecker<int>) new CustomForcedBehavior.ConstrainTo.Domain<int>(1, int.MaxValue);
    public static CustomForcedBehavior.IConstraintChecker<double> Percent = (CustomForcedBehavior.IConstraintChecker<double>) new CustomForcedBehavior.ConstrainTo.Domain<double>(0.0, 100.0);
    public static CustomForcedBehavior.IConstraintChecker<double> Range = (CustomForcedBehavior.IConstraintChecker<double>) new CustomForcedBehavior.ConstrainTo.Domain<double>(1.0, 10000.0);
    public static CustomForcedBehavior.IConstraintChecker<int> RepeatCount = (CustomForcedBehavior.IConstraintChecker<int>) new CustomForcedBehavior.ConstrainTo.Domain<int>(1, 1000);
    public static CustomForcedBehavior.IConstraintChecker<int> SpellId = (CustomForcedBehavior.IConstraintChecker<int>) new CustomForcedBehavior.ConstrainTo.Domain<int>(1, int.MaxValue);
    public static CustomForcedBehavior.IConstraintChecker<string> StringNonEmpty = (CustomForcedBehavior.IConstraintChecker<string>) new CustomForcedBehavior.ConstrainTo.NonEmptyString<string>();
    public static CustomForcedBehavior.IConstraintChecker<int> VehicleId = (CustomForcedBehavior.IConstraintChecker<int>) new CustomForcedBehavior.ConstrainTo.Domain<int>(1, int.MaxValue);
    public static CustomForcedBehavior.IConstraintChecker<WoWPoint> WoWPointNonEmpty = (CustomForcedBehavior.IConstraintChecker<WoWPoint>) new CustomForcedBehavior.ConstrainTo.NonEmptyWoWPoint<WoWPoint>();

    public static CustomForcedBehavior.IConstraintChecker<int> QuestId(CustomForcedBehavior cfb)
    {
      return (CustomForcedBehavior.IConstraintChecker<int>) new CustomForcedBehavior.ConstrainTo.QuestId<int>(cfb);
    }
  }

  public abstract class IConstraintChecker<T>
  {
    public virtual string Check(string attributeName, T value) => (string) null;
  }

  public static class ConstrainTo
  {
    public class Anything<T> : CustomForcedBehavior.IConstraintChecker<T>
    {
      public override string Check(string attributeName, T value) => (string) null;
    }

    public class Domain<T> : CustomForcedBehavior.IConstraintChecker<T>
    {
      private T maxValue;
      private T minValue;

      public Domain(T minValue, T maxValue)
      {
        this.maxValue = maxValue;
        this.minValue = minValue;
      }

      public override string Check(string attributeName, T value)
      {
        bool isValueTooHigh = Comparer<T>.Default.Compare(value, this.maxValue) > 0;
        bool isValueTooLow;
        if (!(isValueTooLow = Comparer<T>.Default.Compare(value, this.minValue) < 0) && !isValueTooHigh)
          return (string) null;
        return $"The '{attributeName}' attribute's value (saw '{value}') is not on the closed interval [{this.minValue}..{this.maxValue}].";
      }
    }

    public class NonEmptyString<T> : CustomForcedBehavior.IConstraintChecker<string>
    {
      public override string Check(string attributeName, string value)
      {
        return !string.IsNullOrEmpty(value) ? (string) null : $"The '{attributeName}' attribute's value may not be an empty string (\"\").";
      }
    }

    public class NonEmptyWoWPoint<T> : CustomForcedBehavior.IConstraintChecker<WoWPoint>
    {
      public override string Check(string attributeName, WoWPoint value)
      {
        return value != WoWPoint.Empty ? (string) null : $"The '{attributeName}' attribute's value may not be the empty WoWPoint (e.g., {WoWPoint.Empty}).";
      }
    }

    public class QuestId<T> : CustomForcedBehavior.ConstrainTo.Domain<int>
    {
      private CustomForcedBehavior behaviorInstance;

      public QuestId(CustomForcedBehavior cfb)
        : base(1, int.MaxValue)
      {
        this.behaviorInstance = cfb;
      }

      public override string Check(string attributeName, int value)
      {
        if (value != 0)
          return base.Check(attributeName, value);
        this.behaviorInstance.LogMessage("warning", new Color?(Color.Red), "The '{0}' attribute's value may not be zero.  For now, we allow it for purposes of backward compatibility; however, it will be ignored.  In a future release, a QuestId of zero will be explicitly disallowed.", (object) attributeName);
        return (string) null;
      }
    }

    public class SpecificValues<T> : CustomForcedBehavior.IConstraintChecker<T>
    {
      private T[] allowedValues;

      public SpecificValues(T[] allowedValues) => this.allowedValues = allowedValues;

      public override string Check(string attributeName, T value)
      {
        if (((IEnumerable<T>) this.allowedValues).Contains<T>(value))
          return (string) null;
        Array.Sort<T>(this.allowedValues);
        string[] strArray = Array.ConvertAll<T, string>(this.allowedValues, (Converter<T, string>) (convertedValue => convertedValue.ToString()));
        return $"The '{attributeName}' attribute's value (saw '{value}') is not one of the allowed values...\n    [{$"'{string.Join("', '", strArray)}'"}].";
      }
    }
  }

  public sealed class ConfigMemento
  {
    private XElement _characterSettingsBackup;
    private bool _isDisposed;
    private XElement _levelbotSettingsBackup;
    private XElement _styxSettingsBackup;

    public ConfigMemento()
    {
      this._characterSettingsBackup = CharacterSettings.Instance.GetXML();
      this._levelbotSettingsBackup = LevelbotSettings.Instance.GetXML();
      this._styxSettingsBackup = StyxSettings.Instance.GetXML();
    }

    ~ConfigMemento() => this.Dispose(false);

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public void Dispose(bool isExplicitlyInitiatedDispose)
    {
      if (!this._isDisposed)
      {
        if (this._characterSettingsBackup != null)
        {
          CharacterSettings.Instance.LoadFromXML(this._characterSettingsBackup);
          CharacterSettings.Instance.Save();
        }
        if (this._levelbotSettingsBackup != null)
        {
          LevelbotSettings.Instance.LoadFromXML(this._levelbotSettingsBackup);
          LevelbotSettings.Instance.Save();
        }
        if (this._styxSettingsBackup != null)
        {
          StyxSettings.Instance.LoadFromXML(this._styxSettingsBackup);
          StyxSettings.Instance.Save();
        }
        this._characterSettingsBackup = (XElement) null;
        this._levelbotSettingsBackup = (XElement) null;
        this._styxSettingsBackup = (XElement) null;
      }
      this._isDisposed = true;
    }

    public override string ToString()
    {
      string str = "";
      if (this._isDisposed)
        throw new ObjectDisposedException(this.GetType().Name);
      if (this._characterSettingsBackup != null)
        str = $"{str}{this._characterSettingsBackup.ToString()}\n";
      if (this._levelbotSettingsBackup != null)
        str = $"{str}{this._levelbotSettingsBackup.ToString()}\n";
      if (this._styxSettingsBackup != null)
        str = $"{str}{this._styxSettingsBackup.ToString()}\n";
      return str;
    }
  }

  public enum QuestCompleteRequirement
  {
    DontCare,
    Complete,
    NotComplete,
  }

  public enum QuestInLogRequirement
  {
    DontCare,
    InLog,
    NotInLog,
  }
}


