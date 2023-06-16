using System;

using PluginAPI.Core.Attributes;

namespace RGCPlugin
{
    public class Plugin
    {
        [PluginEntryPoint("RGC Plugin", "1.0.0", "A plugin made for the RGC", "Noobslayer")]
        private void OnPluginLoad()
        {

        }

        [PluginReload]
        private void OnPluginReload() {}

        [PluginUnload]
        private void OnPluginUnload() {}
    }
}
