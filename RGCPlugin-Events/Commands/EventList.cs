using System;

using CommandSystem;

using RGCPlugin.Commands;

namespace RGCPlugin_Events.Commands
{
    [RGCCommand]
    internal class EventList : IRGCCommand
    {
        public RGCCommandType CommandType { get; set; } = RGCCommandType.Command | RGCCommandType.RemoteAdminCommand;
        public string Command { get; set; } = "event_list";
        public string[] Aliases { get; set; } = new string[] { "eventlist", "list_event", "list_ev" };
        public string Description { get; set; } = "Gets all events that can be played.";
        public string[] Usage { get; set; } = new string[0];

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = null;
            return true;
        }
    }
}
