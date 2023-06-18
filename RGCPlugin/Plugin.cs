using System.IO;

using Serialization;

using PluginAPI.Core;
using PluginAPI.Events;
using PluginAPI.Core.Attributes;

using RGCPlugin.Configs;


namespace RGCPlugin
{
    public class Plugin
    {
        [PluginConfig]
        public Config Config; // Some user settings for the plugin

        [PluginConfig("default.yml")]
        public Translation Translation; // Translations

        public static Plugin Instance { get; private set; } = null; // A singleton of this class

        #region Helper functions

        private void LoadTranslation(string myPath)
        {
            string defaultLang = Path.Combine(myPath, "default.yml");
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

        [PluginEntryPoint("RGC Plugin", "1.0.0", "A plugin made for the RGC", "Noobslayer")]
        // Called when the plugin is run
        private void OnPluginLoad()
        {
            Log.Info("Loading...");

            EventManager.RegisterEvents<PluginEventManager>(this); // Forwards the events to PluginEventManager

            PluginHandler plugin = PluginHandler.Get(this); // Gets the data about this plugin
            string myPath = plugin.PluginDirectoryPath; // Path to the plugin directory

            plugin.LoadConfig(this, "m_cConfig"); // Loads the default config

            // Find the target translation
            LoadTranslation(myPath);

            Log.Info("Successfully loaded");
        }

        [PluginReload]
        // Called when a reload is run
        private void OnPluginReload() { }

        [PluginUnload]
        // Called when the plugin is unloaded
        private void OnPluginUnload() 
        {
            EventManager.UnregisterEvents<PluginEventManager>(this); // Unregisters the PluginEventManager
        }

        #endregion

        public Plugin() => Instance = this; // Set the instance
    }
}
