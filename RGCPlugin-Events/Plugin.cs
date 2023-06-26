using PluginAPI.Core;
using PluginAPI.Enums;
using PluginAPI.Events;
using PluginAPI.Core.Attributes;

using RGCPlugin_Events.Configs;

namespace RGCPlugin_Events
{
    public class Plugin
    {
        [PluginConfig]
        public Config Config;

        public static Plugin Instance { get; private set; } = null;

        #region Plugin Callbacks

        [PluginPriority(LoadPriority.Lowest)]
        [PluginEntryPoint("RGC Event Plugin", "1.0.0", "A plugin for events", "Noobslayer")]
        private void OnPluginLoad()
        {
            Log.Info("Loading Events...");

            EventManager.RegisterAllEvents(this);

            Log.Info("Loaded Events!");
        }

        [PluginReload]
        private void OnPluginReload() { }

        [PluginUnload]
        private void OnPluginUnload()
        {
            EventManager.UnregisterAllEvents(this);
        }

        #endregion

        public Plugin() => Instance = this;
    }
}
