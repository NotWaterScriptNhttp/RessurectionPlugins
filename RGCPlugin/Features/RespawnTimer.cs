using System;
using System.Collections.Generic;

using Respawning;

using MEC;

using PluginAPI.Core;
using PluginAPI.Enums;
using PluginAPI.Core.Attributes;

using RGCPlugin.Configs;

namespace RGCPlugin.Features
{
    internal class RespawnTimer
    {
        private List<Player> ConnectedPlayers = new List<Player>();

        private Config UsedConfig = Plugin.Instance != null ? Plugin.Instance.Config : null;
        private Translation UsedTranslation = Plugin.Instance != null ? Plugin.Instance.Translation : null;

        private CoroutineHandle MainCoroutine;

        private IEnumerator<float> DoRespawnTimerForPlayers()
        {
            while (true)
            {
                if (UsedConfig == null)
                {
                    Log.Error("Config is null");
                    yield break;
                }
                if (UsedTranslation == null)
                {
                    Log.Error("Translation is null");
                    yield break;
                }

                RespawnManager rm = RespawnManager.Singleton;
                TimeSpan ts = new TimeSpan(0, 0, rm.TimeTillRespawn);

                string text = "";
                try
                {
                    if (!UsedConfig.RespawnTimerShowTeam || rm.NextKnownTeam == SpawnableTeamType.None)
                        text = string.Format(UsedTranslation.RespawnTimer, ts.ToString(@"mm\:ss"));
                    else
                    {
                        string teamText = "";
                        if (rm.NextKnownTeam == SpawnableTeamType.NineTailedFox)
                            teamText = UsedConfig.MTFColor.GetColoredText(UsedTranslation.RespawnTimerMTF);
                        else teamText = UsedConfig.CIColor.GetColoredText(UsedTranslation.RespawnTimerCI);

                        text = string.Format(UsedTranslation.RespawnTimerAs, teamText, ts.ToString(@"mm\:ss"));
                    }
                } catch (Exception e)
                {
                    Log.Error(e.StackTrace);
                    yield break;
                }

                List<Player> currentPlayer = ConnectedPlayers;
                foreach (Player plr in currentPlayer)
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

        [PluginEvent(ServerEventType.RoundStart)]
        private void OnRoundStart()
        {
            MainCoroutine = Timing.RunCoroutine(DoRespawnTimerForPlayers());
        }
        [PluginEvent(ServerEventType.RoundEnd)]
        private void OnRoundEnd(RoundSummary.LeadingTeam team) => Timing.KillCoroutines(MainCoroutine);
    }
}
