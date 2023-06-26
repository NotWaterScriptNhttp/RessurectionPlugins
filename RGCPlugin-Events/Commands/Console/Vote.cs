using System;

using CommandSystem;

using RGCPlugin.Commands;

namespace RGCPlugin_Events.Commands.Console
{
    [RGCCommand]
    internal class Vote : IRGCCommand
    {
        public RGCCommandType CommandType { get; set; } = RGCCommandType.Command;
        public string Command { get; set; } = "vote";
        public string[] Aliases { get; set; } = new string[1] { "vt" };
        public string Description { get; set; } = "Votes for a given event";
        public string[] Usage { get; set; } = new string[0];

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = null;
            return true;
        }
    }
}
