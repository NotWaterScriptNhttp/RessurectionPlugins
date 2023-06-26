using System;

using CommandSystem;

using RGCPlugin.Utils;

namespace RGCPlugin.Commands
{
    [RGCCommand]
    internal class TestCmd : IRGCCommand
    {
        public RGCCommandType CommandType { get; set; } = RGCCommandType.Command | RGCCommandType.RemoteAdminCommand;
        public string Command { get; set; } = "test_cmd";
        public string[] Aliases { get; set; } = new string[0];
        public string Description { get; set; } = "";
        public string[] Usage { get; set; } = new string[0];

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = new TextColor(240, 50, 30).GetColoredText("A test message that can be handled with a dot command and RA also the server console");
            return true;
        }
    }
}
