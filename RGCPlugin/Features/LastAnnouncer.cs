using System;

using PlayerStatsSystem;

using PluginAPI.Core;
using PluginAPI.Enums;
using PluginAPI.Core.Attributes;

namespace RGCPlugin.Features
{
    internal class LastAnnouncer
    {
        [PluginEvent(ServerEventType.PlayerDeath)]
        private void OnPlayerDied(Player plr, Player attacker, DamageHandlerBase handler)
        {

        }
    }
}
