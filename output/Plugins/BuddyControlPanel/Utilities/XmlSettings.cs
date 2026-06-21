// Originally contributed by Apoc.
//
// LICENSE:
// This work is licensed under the
//     Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
// also known as CC-BY-NC-SA.  To view a copy of this license, visit
//      http://creativecommons.org/licenses/by-nc-sa/3.0/
// or send a letter to
//      Creative Commons // 171 Second Street, Suite 300 // San Francisco, California, 94105, USA.

#region Usings

using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml.Serialization;

using Styx;
using Styx.Common;
using Styx.Helpers;
#endregion


namespace BuddyControlPanel
{
    public class XmlSettings<T> where T : XmlSettings<T>, new()
    {
        protected XmlSettings(string filePath)
        {
            FilePath = filePath;
            //Load(filePath);
        }

        [XmlIgnore]
        protected string FilePath { get; set; }

        /// <summary>Gets the pathname of the character settings directory.</summary>
        /// <value>The pathname of the character settings directory.</value>
        [XmlIgnore]
        public static string CharacterSettingsDirectory
        {
            get
            {
                var assemblyLocation = Utilities.AssemblyDirectory;

                try
                {
                    Contract.Requires(assemblyLocation != null, () => "assemblyLocation may not be null.");

                    string settingsDirectoryName = Path.Combine(assemblyLocation, "Settings");
                    string characterSettingsDirectoryName = Path.Combine(settingsDirectoryName, StyxWoW.Me.RealmName, StyxWoW.Me.Name);
                    if (!Directory.Exists(characterSettingsDirectoryName))
                        Directory.CreateDirectory(characterSettingsDirectoryName);

                    return characterSettingsDirectoryName;
                }
                catch (Exception ex)
                {
                    PluginLog.Exception(ex);
                    return Path.Combine(assemblyLocation, "Settings");
                }
            }
        }

        /// <summary>Gets the pathname of the character settings directory.</summary>
        /// <value>The pathname of the character settings directory.</value>
        [XmlIgnore]
        public static string SettingsDirectory
        {
            get
            {
                var assemblyLocation = Utilities.AssemblyDirectory;

                try
                {
                    Contract.Requires(assemblyLocation != null, () => "assemblyLocation may not be null.");

                    string settingsDirectoryName = Path.Combine(assemblyLocation, "Settings");
                    if (!Directory.Exists(settingsDirectoryName))
                        Directory.CreateDirectory(settingsDirectoryName);

                    return settingsDirectoryName;
                }
                catch (Exception ex)
                {
                    PluginLog.Exception(ex);
                    return Path.Combine(assemblyLocation, "Settings");
                }
            }
        }

        public virtual void Save()
        {
            Utility.InvokeOnSpecificDispatcher(Application.Current.Dispatcher, () =>
            {
                Save(FilePath, this);
            });
        }

        public virtual void SetDefaultValues()
        {
        }

        protected internal static T Load(string path, bool saveAfterLoad = true)
        {
            var result = default(T);
            Utility.InvokeOnSpecificDispatcher(Application.Current.Dispatcher, () =>
            {
                try
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    T deserializedContent;
                    using (var fileStream = File.OpenRead(path))
                    {
                        deserializedContent = xmlSerializer.Deserialize(fileStream) as T;
                    }
                    if (saveAfterLoad)
                        Save(path, deserializedContent);

                    result = deserializedContent;
                }
                catch (Exception)
                {
                    //PluginLog.Exception(ex);
                    T r = new T();
                    r.SetDefaultValues();
                    Save(path, r);
                    result = Load(path, saveAfterLoad);
                }
            });

            return result;
        }

        protected internal static void Save(string path, object objectToSerialize)
        {
            Utility.InvokeOnSpecificDispatcher(Application.Current.Dispatcher, () =>
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                using (var fileStream = File.Create(path))
                {
                    xmlSerializer.Serialize(fileStream, objectToSerialize, null);
                }
            });
        }
    }
}
