using System;

using CommandSystem;

namespace RGCPlugin_Events.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class EventList : ICommand, IUsageProvider
    {
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
