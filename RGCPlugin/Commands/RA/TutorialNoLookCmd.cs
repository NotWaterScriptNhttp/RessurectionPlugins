﻿using System;

using CommandSystem;

namespace RGCPlugin.Commands.RA
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class TutorialNoLookCmd : ICommand, IUsageProvider
    {
        public string Command { get; } = "";
        public string[] Aliases { get; } = new string[0];
        public string Description { get; } = "";
        public string[] Usage { get; } = new string[0];

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count <= 0)
            {
                response = Plugin.GetConfigValue("TutorialNoLook", true).ToString();
                return true;
            }

            string[] arr = new string[1] { arguments.At(0) };
            Misc.CommandOperationMode mode;
            if (!Misc.TryCommandModeFromArgs(ref arr, out mode))
            {
                response = $"Invalid option {arr[0]} use 0/1, false/true, disable/enable or off/on";
                return false;
            }

            switch (mode)
            {
                case Misc.CommandOperationMode.Disable:
                    Plugin.SetConfigValue("TutorialNoLook", false);
                    break;
                case Misc.CommandOperationMode.Enable:
                    Plugin.SetConfigValue("TutorialNoLook", true);
                    break;
                case Misc.CommandOperationMode.Toggle:
                    Plugin.SetConfigValue("TutorialNoLook", !Plugin.GetConfigValue("TutorialNoLook", true));
                    break;
            }

            response = $"New value: {Plugin.GetConfigValue("TutorialNoLook", true)}";
            return true;
        }
    }
}