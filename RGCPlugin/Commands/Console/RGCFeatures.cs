using System;
using System.Text;

using CommandSystem;

using RGCPlugin.Configs;

namespace RGCPlugin.Commands.Console
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    internal class RGCFeatures : IRGCCommand
    {
        public string Command { get; } = "rgcfeatures";
        public string[] Aliases { get; } = new string[] { "rf", "rgcf", "rfeatures" };
        public string Description { get; } = "";
        public string[] Usage { get; } = null;

        public RGCFeatures() => RGCCommandHandler.AddCommand(this);

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Config cfg = Plugin.Instance != null ? Plugin.Instance.Config : null;
            if (cfg == null)
            {
                response = "Failed to get the config";
                return false;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Features:");
            sb.AppendLine($" - Infinite Radio: {cfg.InfiniteRadio}");
            sb.AppendLine($" - Remote Keycard: {cfg.RemoteKeycard}");
            sb.AppendLine($" - Respawn Timer: {cfg.RespawnTimer}");

            response = sb.ToString();
            return true;
        }
    }
}
