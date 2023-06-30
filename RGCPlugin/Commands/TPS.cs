using System;

using CommandSystem;

using PluginAPI.Core;

namespace RGCPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class TPS : ICommand, IUsageProvider
    {
        public string Command { get; } = "tps";
        public string[] Aliases { get; } = new string[0];
        public string Description { get; } = "Gets the server tps";
        public string[] Usage { get; } = new string[0];

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = Server.TPS.ToString();
            return true;
        }
    }
}
