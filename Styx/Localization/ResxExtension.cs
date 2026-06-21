using System;
using System.Resources;
using System.Windows.Markup;

namespace Styx.Localization
{
    [MarkupExtensionReturnType(typeof(string))]
    public sealed class Resx : MarkupExtension
    {
        public string Key { get; set; }
        public string ResxName { get; set; }
        public string DefaultValue { get; set; }

        public Resx() { }

        public Resx(string key)
        {
            Key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(Key))
                return DefaultValue ?? string.Empty;

            try
            {
                string value = Globalization.Get(Key);
                if (value != null)
                    return value;
            }
            catch
            {
            }

            return DefaultValue ?? Key;
        }
    }
}