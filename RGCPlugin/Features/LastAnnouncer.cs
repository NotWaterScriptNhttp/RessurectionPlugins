using System;
using System.Collections.Generic;

using MEC;
using UnityEngine;

using PlayerRoles;
using MapGeneration;
using PlayerStatsSystem;

using PluginAPI.Core;
using PluginAPI.Enums;
using PluginAPI.Core.Attributes;

namespace RGCPlugin.Features
{
    internal class LastAnnouncer
    {
        // Commented code to not waste server resources
        /*const float TIME_FOR_NEXT_ANNOUNCE = 1.5f * (60); // 1,5 minutes
        private float lastAnnounce = 0;
        private FacilityZone lastZone = FacilityZone.None;
        private CoroutineHandle updater;

        private void Handle()
        {
            if (!Plugin.GetConfigValue("LastAnnouncer", false))
                return;

            int countedPlayers = 0;
            int chaos = 0;
            Player lastPlayer = null;
            Player lastChaos = null;

            foreach (Player plr in Player.GetPlayers())
            {
                if (plr.Team != Team.ChaosInsurgency && plr.Team != Team.Dead && plr.Role != RoleTypeId.Tutorial)
                {
                    countedPlayers++;
                    lastPlayer = plr;
                    continue;
                }
                if (plr.Team == Team.ChaosInsurgency)
                {
                    chaos++;
                    lastChaos = plr;
                }
            }
        }
        private IEnumerator<float> Update()
        {
            float waitTime = Math.Max(0, TIME_FOR_NEXT_ANNOUNCE - (Time.realtimeSinceStartup - lastAnnounce));
            while (true)
            {
                if (!Round.IsRoundStarted)
                    yield return Timing.WaitForSeconds(1);

                if ((lastAnnounce + TIME_FOR_NEXT_ANNOUNCE) <= Time.realtimeSinceStartup)
                    Handle();

                waitTime = Math.Max(0, TIME_FOR_NEXT_ANNOUNCE - (Time.realtimeSinceStartup - lastAnnounce));
                yield return Timing.WaitForSeconds(waitTime);
            }
        }

        [PluginEvent(ServerEventType.PlayerDeath)]
        private void OnPlayerDied(Player plr, Player attacker, DamageHandlerBase handler) => Handle();

        [PluginEvent(ServerEventType.PlayerChangeRole)]
        private void OnPlayerRoleChange(Player plr, PlayerRoleBase role, RoleTypeId id, RoleChangeReason reason)
        {
            if (reason.HasFlag(RoleChangeReason.None) || reason.HasFlag(RoleChangeReason.RemoteAdmin))
                Handle();
        }

        [PluginEvent(ServerEventType.PlayerLeft)]
        private void OnPlayerLeft(Player plr)
        {
            if (plr.IsAlive)
                Handle();
        }

        public LastAnnouncer() => updater = Timing.RunCoroutine(Update());*/
    }
}
