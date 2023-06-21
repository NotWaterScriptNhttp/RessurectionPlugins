using System;
using System.Reflection;
using System.Collections.Generic;

using RemoteAdmin;
using CommandSystem;

using PluginAPI.Core;
using PluginAPI.Enums;
using PluginAPI.Events;
using PluginAPI.Core.Attributes;

namespace RGCPlugin.Commands
{
    public class RGCCommandHandler
    {
        private delegate bool CommandExecute(ArraySegment<string> args, ICommandSender sender, out string res);

        private static List<ICommand> Commands = new List<ICommand>();
        private static Dictionary<string, CommandExecute> CommandHandlers = new Dictionary<string, CommandExecute>();
        private static object CommandLock = new object();

        private static void RegisterHandlers(ICommand command)
        {
            lock (CommandLock)
            {
                if (CommandHandlers.TryGetValue(command.Command, out CommandExecute handler))
                {
                    Type orig = handler.GetMethodInfo().DeclaringType;
                    Log.Error($"Command ({command.Command}) was already registered from '{orig.Assembly.GetName().Name + "::" + orig.FullName}'");
                }
                else CommandHandlers.Add(command.Command, command.Execute);
            }

            if (command.Aliases != null || command.Aliases.Length > 0)
            {
                foreach (string alias in command.Aliases)
                    lock (CommandLock)
                    {
                        if (CommandHandlers.TryGetValue(alias, out CommandExecute handler))
                        {
                            Type orig = handler.GetMethodInfo().DeclaringType;
                            Log.Error($"Command ({alias}) was already registered from '{orig.Assembly.GetName().Name + "::" + orig.FullName}'");
                        }
                        else CommandHandlers.Add(alias, command.Execute);
                    }
            }
        }

        public static ICommand AddCommand(Type command)
        {
            foreach (ICommand cmd in Commands)
                if (cmd.GetType() == command)
                {
                    Log.Error($"This command ({cmd.Command}) was already added.");
                    return null;
                }

            ICommand inst = (ICommand)Activator.CreateInstance(command); // Creates an instance of the command

            lock (CommandLock)
            {
                Commands.Add(inst); // Adds the command
                RegisterHandlers(inst); // Registers all of its handlers
            }

            return inst;
        }
        public static void AddCommand(ICommand command)
        {
            foreach (ICommand cmd in Commands)
                if (cmd.GetType() == command.GetType())
                {
                    Log.Error($"This command ({cmd.Command}) was already added");
                }

            lock (CommandLock)
            {
                Commands.Add(command); // Adds the command
                RegisterHandlers(command); // Registers all of its handlers
            }
        }

        public static void RemoveCommand(ICommand command)
        {
            lock (CommandLock)
            {
                if (Commands.Contains(command))
                    Commands.Remove(command);
            }
        }

        public static void ReloadHandlers()
        {
            lock (CommandLock)
            {
                CommandHandlers.Clear(); // Clears the handlers

                // Registers all of the added commands
                foreach (ICommand cmd in Commands)
                    RegisterHandlers(cmd);
            }
        }

        // The handler for the commands
        [PluginEvent(ServerEventType.PlayerGameConsoleCommand)]
        private bool OnPlayerCommand(Player player, string command, string[] args)
        {
            if (!CommandHandlers.TryGetValue(command, out CommandExecute exec))
                return true;

            // Create the sender
            ICommandSender sender = new PlayerCommandSender(player.ReferenceHub);

            string data = "";
            try
            {
                bool success = exec(args.Segment(0), sender, out data);

                if (!EventManager.ExecuteEvent(ServerEventType.PlayerGameConsoleCommandExecuted, player.ReferenceHub, command, args, success, data))
                    return false;
            }
            catch (Exception ex)
            {
                data = "Command execution failed! Error: " + (ex != null ? ex.ToString() : null);
                Log.Error(data);

                if (!EventManager.ExecuteEvent(ServerEventType.PlayerGameConsoleCommandExecuted, player.ReferenceHub, command, args, false, data))
                    return false;
            }

            // This cannot be null, otherwise we wouldn't have even received the command
            QueryProcessor qp = player.GetComponent<QueryProcessor>();

            // Send the command response to the client
            qp.GCT.SendToClient(qp.connectionToClient, command.ToUpperInvariant() + "#" + data, "");

            return false;
        }
    }
}
