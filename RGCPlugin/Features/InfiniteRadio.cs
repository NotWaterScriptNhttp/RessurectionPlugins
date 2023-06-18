using InventorySystem.Items.Radio;

using PluginAPI.Core;
using PluginAPI.Enums;
using PluginAPI.Core.Attributes;

namespace RGCPlugin.Features
{
    internal class InfiniteRadio
    {
        [PluginEvent(ServerEventType.PlayerUsingRadio)]
        private void OnPlayerUseRadio(Player player, RadioItem radio, float drain)
        {
            if (Plugin.Instance == null)
                return;

            if (Plugin.Instance.Config.InfiniteRadio)
                radio.BatteryPercent = 100;
        }
    }
}
