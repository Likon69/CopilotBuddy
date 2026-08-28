using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;

namespace Styx.Helpers
{
    /// <summary>
    /// Value converter to support enum display names and localization.
    /// </summary>
    public class EnumTypeConverter : EnumConverter
    {
        private static IDictionary<Type, EnumCacheEntry>? _enumMap;
        private static IDictionary<Type, ResourceManager>? _resourceManagers;
        [CompilerGenerated]
        private static LocalizerDelegate? _defaultLocalizer;

        public EnumTypeConverter(Type type)
            : base(type)
        {
        }

        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string text)
            {
                EnsureCache(context!.PropertyDescriptor!.PropertyType, culture ?? CultureInfo.CurrentCulture);
                EnumCacheEntry cacheEntry = _enumMap![context.PropertyDescriptor.PropertyType];
                CultureCacheEntry cultureCache = cacheEntry.CultureEntries[culture ?? CultureInfo.CurrentCulture];

                if (cultureCache.HasDisplayNames)
                {
                    if (text.IndexOf(',') < 0)
                    {
                        if (!cultureCache.DisplayToValue.ContainsKey(text))
                            throw base.GetConvertFromException(text);
                        return cultureCache.DisplayToValue[text];
                    }

                    string[] parts = text.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                    string first = parts[0];
                    if (!cultureCache.DisplayToValue.ContainsKey(first))
                        throw base.GetConvertFromException(text);

                    int result = (int)cultureCache.DisplayToValue[first];
                    for (int i = 1; i < parts.Length; i++)
                    {
                        string part = parts[i];
                        if (!cultureCache.DisplayToValue.ContainsKey(part))
                            throw base.GetConvertFromException(text);
                        result |= (int)cultureCache.DisplayToValue[part];
                    }
                    return Enum.ToObject(context!.PropertyDescriptor!.PropertyType, result);
                }

                return Enum.Parse(context!.PropertyDescriptor!.PropertyType, text);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (value != null && destinationType == typeof(string) && value.GetType().IsEnum)
            {
                EnsureCache(value.GetType(), culture ?? CultureInfo.CurrentCulture);
                EnumCacheEntry cacheEntry = _enumMap![value.GetType()];
                CultureCacheEntry cultureCache = cacheEntry.CultureEntries[culture ?? CultureInfo.CurrentCulture];

                string? text = value.ToString();
                if (cultureCache.HasDisplayNames && text != null)
                {
                    if (text.IndexOf(',') < 0)
                    {
                        if (!cultureCache.ValueToDisplay!.ContainsKey(text))
                            throw base.GetConvertToException(text, destinationType);
                        return cultureCache.ValueToDisplay[text];
                    }

                    string[] parts = text.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                    var builder = new StringBuilder();
                    foreach (string part in parts)
                    {
                        if (!cultureCache.ValueToDisplay!.ContainsKey(part))
                            throw base.GetConvertToException(text, destinationType);
                        builder.Append(cultureCache.ValueToDisplay[part]);
                        builder.Append(", ");
                    }
                    builder.Length -= 2;
                    return builder.ToString();
                }

                return value.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext? context) => true;

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
        {
            EnsureCache(base.EnumType, CultureInfo.CurrentCulture);
            EnumCacheEntry cacheEntry = _enumMap![base.EnumType];
            return new TypeConverter.StandardValuesCollection(cacheEntry.StandardValues);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext? context) => true;

        public override bool IsValid(ITypeDescriptorContext? context, object? value) => base.IsValid(context, value);

        public override bool GetPropertiesSupported(ITypeDescriptorContext? context) => false;

        public override bool GetCreateInstanceSupported(ITypeDescriptorContext? context) => false;

        private static void EnsureCache(Type type, CultureInfo culture)
        {
            LocalizerDelegate? localizer = null;
            EnumCacheEntry cacheEntry;
            CultureCacheEntry cultureCache;
            if (_enumMap == null)
                _enumMap = new Dictionary<Type, EnumCacheEntry>();

            if (_enumMap.ContainsKey(type))
            {
                cacheEntry = _enumMap[type];
                if (cacheEntry.CultureEntries.ContainsKey(culture))
                    return;
            }
            else
            {
                cacheEntry = new EnumCacheEntry { CultureEntries = new Dictionary<CultureInfo, CultureCacheEntry>() };
            }

            cultureCache = new CultureCacheEntry();
            IDictionary<object, string> lookup = new Dictionary<object, string>();
            IDictionary<string, object> reverse = new Dictionary<string, object>();
            List<object> list = new List<object>();

            if (_resourceManagers != null && _resourceManagers.ContainsKey(type))
            {
                ResourceManager rm = _resourceManagers[type];
                localizer = new LocalizerDelegate(new ResourceLocalizer { Manager = rm, Culture = culture }.Localize);
            }
            else
            {
                if (_defaultLocalizer == null)
                    _defaultLocalizer = new LocalizerDelegate(DefaultLocalizer);
                localizer = _defaultLocalizer;
            }

            string? prefix = type.FullName?.Replace('.', '_');
            FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo fieldInfo in fields)
            {
                if (fieldInfo.FieldType == type)
                {
                    object enumValue = Enum.Parse(type, fieldInfo.Name);
                    string resourceKey = prefix + "_" + fieldInfo.Name;
                    string localized = localizer(resourceKey);

                    if (string.Compare(resourceKey, localized) == 0)
                    {
                        object[] attrs = fieldInfo.GetCustomAttributes(typeof(FieldDisplayNameAttribute), false);
                        if (attrs.Length > 0)
                        {
                            localized = ((FieldDisplayNameAttribute)attrs[0]).DisplayName;
                            cultureCache.HasDisplayNames = true;
                        }
                        else
                        {
                            localized = fieldInfo.Name;
                        }
                    }
                    else
                    {
                        cultureCache.HasDisplayNames = true;
                    }

                    if (!lookup.ContainsKey(fieldInfo.Name))
                        lookup.Add(fieldInfo.Name, localized);

                    reverse.Add(localized, enumValue);
                    list.Add(enumValue);
                }
            }

            cultureCache.DisplayToValue = reverse;
            if (cultureCache.HasDisplayNames)
                cultureCache.ValueToDisplay = lookup;

            lock (cacheEntry.CultureEntries)
            {
                if (!cacheEntry.CultureEntries.ContainsKey(culture))
                    cacheEntry.CultureEntries.Add(culture, cultureCache);
            }

            if (cacheEntry.StandardValues == null)
            {
                lock (_enumMap)
                {
                    cacheEntry.StandardValues = list;
                    if (!_enumMap.ContainsKey(type))
                        _enumMap.Add(type, cacheEntry);
                    else
                    {
                        cacheEntry = _enumMap[type];
                        if (!cacheEntry.CultureEntries.ContainsKey(culture))
                        {
                            lock (cacheEntry.CultureEntries)
                            {
                                if (!cacheEntry.CultureEntries.ContainsKey(culture))
                                    cacheEntry.CultureEntries.Add(culture, cultureCache);
                            }
                        }
                    }
                }
            }
        }

        public static void RegisterResourceManager(Type enumType, ResourceManager resourceManager)
        {
            if (_resourceManagers == null)
                _resourceManagers = new Dictionary<Type, ResourceManager>();
            _resourceManagers[enumType] = resourceManager;
            if (_enumMap != null && _enumMap.ContainsKey(enumType))
            {
                lock (_enumMap)
                {
                    if (_enumMap.ContainsKey(enumType))
                        _enumMap.Remove(enumType);
                }
            }
        }

        [CompilerGenerated]
        private static string DefaultLocalizer(string key)
        {
            return key;
        }

        /// <summary>Per-type cache entry holding culture-specific display data.</summary>
        private sealed class EnumCacheEntry
        {
            public ICollection? StandardValues;
            public IDictionary<CultureInfo, CultureCacheEntry> CultureEntries = new Dictionary<CultureInfo, CultureCacheEntry>();
        }

        /// <summary>Per-culture cache of enum value↔display-name mappings.</summary>
        private sealed class CultureCacheEntry
        {
            public bool HasDisplayNames;
            public IDictionary<object, string>? ValueToDisplay;
            public IDictionary<string, object> DisplayToValue = new Dictionary<string, object>();
        }

        private delegate string LocalizerDelegate(string key);

        /// <summary>Wraps a ResourceManager for culture-specific localization lookups.</summary>
        [CompilerGenerated]
        private sealed class ResourceLocalizer
        {
            public ResourceManager Manager = null!;
            public CultureInfo Culture = null!;

            public string Localize(string key)
            {
                string? value = Manager.GetString(key, Culture);
                return value ?? key;
            }
        }
    }
}
