using System;

using PluginAPI.Core;
using PluginAPI.Events;
using PluginAPI.Core.Attributes;

namespace NwApiTest
{
    public class Config {}
    public class Plugin
    {
        [PluginConfig]
        public Config PluginConfig;

        [PluginEntryPoint("NwApiTest", "1.0.0", "A test of the NwApi", "Noobslayer")]
        public void OnLoad()
        {
            Log.Info("Loading the plugin");

            EventManager.RegisterEvents(this);
            EventManager.RegisterEvents<EventHandlers>(this);
        }

        [PluginReload]
        public void OnReload()
        {
            Log.Info("Plugin is reloading");
        }

        [PluginUnload]
        public void OnUnload()
        {
            Log.Info("Unloading the plugin");
        }
    }
}
