using System;

using CommandSystem;

using RGCPlugin.Utils;
using RGCPlugin.Commands;

namespace RGCPlugin_Events.Commands.RA
{
    [RGCCommand]
    internal class StartEvent : IRGCCommand
    {
        public RGCCommandType CommandType { get; set; } = RGCCommandType.RemoteAdminCommand;
        public string Command { get; set; } = "start_event";
        public string[] Aliases { get; set; } = new string[] { "start_ev", "s_event" }; 
        public string Description { get; set; } = "Starts the specified event";
        public string[] Usage { get; set; } = new string[] { "event_name" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count <= 0)
            {
                response = new TextColor(255).GetColoredText("Please select an event. Use event_list to show all events.");
                return false;
            }

            response = null;
            return true;
        }
    }
}
