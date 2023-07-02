using System.IO;
using System.Reflection;

using Serialization;

using PluginAPI.Core;
using PluginAPI.Enums;
using PluginAPI.Events;
using PluginAPI.Core.Attributes;

using RGCPlugin.Configs;

namespace RGCPlugin
{
    public class Plugin
    {
        private static Translation defaultTranslation = new Translation();
        private static PropertyInfo[] ConfigProperties = null;
        private static PropertyInfo[] TranslationProperties = null;

        [PluginConfig]
        public Config Config; // Some user settings for the plugin

        [PluginConfig("default_lang.yml")]
        public Translation Translation; // Translations

        public static Plugin Instance { get; private set; } = null; // A singleton of this class

        #region Helper functions

        private void LoadTranslation(string myPath)
        {
            string defaultLang = Path.Combine(myPath, "default_lang.yml");
            if (!File.Exists(defaultLang))
                File.WriteAllText(defaultLang, YamlParser.Serializer.Serialize(Translation)); // Writes the default 

            string translationDir = Path.Combine(myPath, "locale"); // Path to the translations
            if (!Directory.Exists(translationDir))
                Directory.CreateDirectory(translationDir); // Creates the directory if it doesn't exist

            // Check if the selected translation is empty, to select default (English or what the user replaced it with)
            if (string.IsNullOrEmpty(Config.Translation))
                return;

            string[] translations = Directory.GetFiles(translationDir, "*.yml"); // List all translations that are in locale
            if (translations.Length <= 0)
            {
                Log.Warning("Didn't find any translations in 'locale'");
                return;
            }

            bool foundTranslation = false;
            foreach (string tran in translations)
            {
                if (Path.GetFileNameWithoutExtension(tran).ToLower() == Config.Translation.ToLower())
                {
                    foundTranslation = true;
                    // Loads the translation
                    Translation = YamlParser.Deserializer.Deserialize<Translation>(File.ReadAllText(Path.Combine(translationDir, tran)));

                    break;
                }
            }

            if (!foundTranslation)
                Log.Warning($"Failed to find the translation: {Config.Translation}, using the default instead!");
        }

        #endregion

        #region Plugin Callbacks

        [PluginPriority(LoadPriority.Highest)]
        [PluginEntryPoint("RGC Plugin", "1.0.0", "A plugin with features that a server wants", "Noobslayer")]
        // Called when the plugin is run
        private void OnPluginLoad()
        {
            Log.Info("Loading...");

            EventManager.RegisterAllEvents(this); // Registers all of the plugins's event handlers

            PluginHandler plugin = PluginHandler.Get(this); // Gets the data about this plugin
            string myPath = plugin.PluginDirectoryPath; // Path to the plugin directory

            plugin.LoadConfig(this, "m_cConfig"); // Loads the default config

            // Find the target translation
            LoadTranslation(myPath);

            Log.Info("Successfully loaded");
        }

        [PluginReload]
        // Called when a reload is run
        private void OnPluginReload()
        {
            ConfigProperties = null;
            TranslationProperties = null;
        }

        [PluginUnload]
        // Called when the plugin is unloaded
        private void OnPluginUnload() 
        {
            EventManager.UnregisterAllEvents(this); // Unregisters all events from this plugin
        }

        #endregion

        #region Config Methods

        public static T GetConfigValue<T>(string key, T def)
        {
            if (Instance == null)
                return def;

            if (ConfigProperties == null)
                ConfigProperties = typeof(Config).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (PropertyInfo prop in ConfigProperties)
            {
                if (prop.Name.ToLower() == key.ToLower())
                    return (T)prop.GetValue(Instance.Config);
            }

            return def;
        }
        public static void SetConfigValue(string key, object val)
        {
            if (Instance == null)
                return;

            if (ConfigProperties == null)
                ConfigProperties = typeof(Config).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (PropertyInfo prop in ConfigProperties)
            {
                if (prop.Name.ToLower() == key.ToLower())
                {
                    prop.SetValue(Instance.Config, val);
                    break;
                }
            }
        }

        public static string GetTranslation(string key, string def = "")
        {
            if (TranslationProperties == null)
                TranslationProperties = typeof(Translation).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            Translation curr = Instance == null ? defaultTranslation : Instance.Translation;

            foreach (PropertyInfo prop in TranslationProperties)
                if (prop.Name.ToLower() == key.ToLower())
                    return (string)prop.GetValue(curr);

            // If no key was found return the default one
            return def;
        }

        #endregion

        public Plugin() => Instance = this; // Set the instance
    }
}
