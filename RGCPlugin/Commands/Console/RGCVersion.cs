using System;
using System.Reflection;

using CommandSystem;

namespace RGCPlugin.Commands.Console
{
    [RGCCommand]
    internal class RGCVersion : IRGCCommand
    {
        public RGCCommandType CommandType { get; set; } = RGCCommandType.Command;
        public string Command { get; } = "rgcversion";
        public string[] Aliases { get; } = new string[] { "rver", "rgcver" };
        public string Description { get; } = "Gets the running version of this plugin";
        public string[] Usage { get; } = new string[0];

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Assembly current = Assembly.GetExecutingAssembly();
            if (current.GetName() == null)
            {
                response = "<color=red>RGC ver UNKNOWN</color>";
                return false;
            }

            response = "<color=green>RGC ver " + current.GetName().Version + "</color>";

            return true;
        }
    }
}
