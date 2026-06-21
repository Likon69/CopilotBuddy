using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace Styx.Localization
{
    public static partial class Globalization
    {
        private static ResourceManager _resourceManager;
        private static CultureInfo _culture;
        // Cache of manually-loaded satellite ResourceSets by culture name.
        // Needed because .NET 10's ResourceManager does not honor App.config's
        // <probing privatePath="Languages"/> for satellite assembly lookup.
        // The HB 6.2.3 pattern puts satellites in Languages/{culture}/ next to exe.
        private static readonly Dictionary<string, ResourceSet> _satelliteSets = new Dictionary<string, ResourceSet>();
        private static readonly object _satelliteLock = new object();

        public static ResourceManager ResourceManager
        {
            get
            {
                if (_resourceManager == null)
                {
                    _resourceManager = new ResourceManager(
                        "CopilotBuddy.Styx.Localization.Strings",
                        typeof(Globalization).Assembly);
                }
                return _resourceManager;
            }
        }

        public static CultureInfo Culture
        {
            get { return _culture ?? Thread.CurrentThread.CurrentUICulture; }
            set
            {
                _culture = value;
                Thread.CurrentThread.CurrentUICulture = value;
                // Propagate to all future threads (.NET 4.6+/Core+ equivalent of
                // setting the default UI culture for any thread created later).
                CultureInfo.DefaultThreadCurrentUICulture = value;
                if (value != null && !value.IsNeutralCulture)
                    CultureInfo.DefaultThreadCurrentCulture = value;
            }
        }

        public static string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            var culture = Culture;
            // 1) Try the Languages/{culture}/CopilotBuddy.resources.dll satellite we
            //    ship via the PostBuild target. This is the HB 6.2.3 folder pattern.
            string customValue = TryGetFromSatellite(key, culture);
            if (customValue != null)
                return customValue;

            // 2) Fallback to the standard ResourceManager (handles neutral + invariant).
            try
            {
                string stdValue = ResourceManager.GetString(key, culture);
                if (stdValue != null)
                    return stdValue;
            }
            catch
            {
            }

            return key;
        }

        private static string TryGetFromSatellite(string key, CultureInfo culture)
        {
            if (culture == null)
                return null;

            string cultureName = culture.Name;
            if (string.IsNullOrEmpty(cultureName))
                return null;

            ResourceSet set;
            lock (_satelliteLock)
            {
                if (!_satelliteSets.TryGetValue(cultureName, out set))
                {
                    set = LoadSatelliteSet(cultureName);
                    _satelliteSets[cultureName] = set;
                }
            }

            if (set == null)
                return null;

            try
            {
                return set.GetString(key);
            }
            catch
            {
                return null;
            }
        }

        private static ResourceSet LoadSatelliteSet(string cultureName)
        {
            try
            {
                string assemblyDir = Path.GetDirectoryName(typeof(Globalization).Assembly.Location);
                if (string.IsNullOrEmpty(assemblyDir))
                    return null;

                string dllPath = Path.Combine(assemblyDir, "Languages", cultureName, "CopilotBuddy.resources.dll");
                if (!File.Exists(dllPath))
                    return null;

                var satelliteAssembly = Assembly.LoadFrom(dllPath);

                // Manifest resource name pattern: <rootnamespace>.<resourcename>.<culture>.resources
                string expectedName = "Styx.Localization.Strings." + cultureName + ".resources";
                Stream stream = satelliteAssembly.GetManifestResourceStream(expectedName);
                if (stream == null)
                {
                    // Fallback: scan manifest names for any *.resources entry.
                    foreach (var name in satelliteAssembly.GetManifestResourceNames())
                    {
                        if (name.EndsWith(".resources", System.StringComparison.OrdinalIgnoreCase))
                        {
                            stream = satelliteAssembly.GetManifestResourceStream(name);
                            if (stream != null)
                                break;
                        }
                    }
                }

                if (stream == null)
                    return null;

                return new ResourceSet(stream);
            }
            catch
            {
                return null;
            }
        }

        public static void ApplyLanguage(string language)
        {
            if (string.IsNullOrEmpty(language))
            {
                Culture = CultureInfo.CurrentUICulture;
                return;
            }

            try
            {
                Culture = CultureInfo.GetCultureInfo(language);
            }
            catch (CultureNotFoundException)
            {
                Culture = CultureInfo.InvariantCulture;
            }
        }
    }
}