using System;
using System.Collections.Generic;

using MEC;
using Respawning;

using PluginAPI.Core;
using PluginAPI.Enums;
using PluginAPI.Core.Attributes;

using RGCPlugin.Utils;

namespace RGCPlugin.Features
{
    internal class RespawnTimer
    {
        private Nullable<CoroutineHandle> MainCoroutine = null;
        private List<Player> ConnectedPlayers = new List<Player>();

        private IEnumerator<float> DoRespawnTimerForPlayers()
        {
            while (true)
            {
                if (!Plugin.GetConfigValue("RespawnTimer", false))
                {
                    yield return Timing.WaitForSeconds(1);
                    continue;
                }

                RespawnManager rm = RespawnManager.Singleton;
                TimeSpan ts = new TimeSpan(0, 0, rm.TimeTillRespawn);

                string text = "";
                try
                {
                    if (!Plugin.GetConfigValue("RespawnTimerShowTeam", true) || rm.NextKnownTeam == SpawnableTeamType.None)
                        text = string.Format(Plugin.GetTranslation("RespawnTimer", "You will respawn in {mm:ss}"), ts.ToString(@"mm\:ss"));
                    else
                    {
                        string teamText = "";
                        if (rm.NextKnownTeam == SpawnableTeamType.NineTailedFox)
                            teamText = Plugin.GetConfigValue<TextColor>("MTFColor", new TextColor(10, 10, 230)).GetColoredText(Plugin.GetTranslation("RespawnTimerMTF", "MTF"));
                        else teamText = Plugin.GetConfigValue<TextColor>("CIColor", new TextColor(10, 230, 10)).GetColoredText(Plugin.GetTranslation("RespawnTimerCI", "CI"));

                        text = string.Format(Plugin.GetTranslation("RespawnTimerAs"), teamText, ts.ToString(@"mm\:ss"));
                    }
                } catch (Exception e)
                {
                    Log.Error(e.StackTrace);
                    yield break;
                }

                List<Player> currentPlayers = ConnectedPlayers;
                foreach (Player plr in currentPlayers)
                {
                    if (plr.IsAlive)
                        continue;

                    plr.ReceiveHint(text, 1.15f);
                }

                yield return Timing.WaitForSeconds(1);
            }
        }

        [PluginEvent(ServerEventType.PlayerJoined)]
        private void OnPlayerJoin(Player player) => ConnectedPlayers.Add(player);
        [PluginEvent(ServerEventType.PlayerLeft)]
        private void OnPlayerLeft(Player player)
        {
            if (ConnectedPlayers.Contains(player))
                ConnectedPlayers.Remove(player);
        }

        [PluginEvent(ServerEventType.WaitingForPlayers)]
        private void OnWaiting()
        {
            if (MainCoroutine.HasValue)
                Timing.KillCoroutines(MainCoroutine.Value);

            MainCoroutine = null;
        }

        [PluginEvent(ServerEventType.RoundStart)]
        private void OnRoundStart()
        {
            if (MainCoroutine.HasValue)
                Timing.KillCoroutines(MainCoroutine.Value);

            MainCoroutine = Timing.RunCoroutine(DoRespawnTimerForPlayers());
        }

        [PluginEvent(ServerEventType.RoundEnd)]
        private void OnRoundEnd(RoundSummary.LeadingTeam team)
        {
            if (MainCoroutine.HasValue)
                Timing.KillCoroutines(MainCoroutine.Value);

            MainCoroutine = null;
        }
    }
}
