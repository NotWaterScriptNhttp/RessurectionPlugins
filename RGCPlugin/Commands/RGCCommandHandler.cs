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
        private struct CommandData
        {
            public RGCCommandType type;
            public CommandExecute exec;
        }

        private delegate bool CommandExecute(ArraySegment<string> args, ICommandSender sender, out string res);

        private static List<IRGCCommand> Commands = new List<IRGCCommand>();
        private static Dictionary<Type, IRGCCommand> RegisteredCommands = new Dictionary<Type, IRGCCommand>();
        private static Dictionary<string, CommandData> CommandHandlers = new Dictionary<string, CommandData>();
        private static object CommandLock = new object();

        private static void RegisterHandlers(IRGCCommand command)
        {
            CommandData cmdData;
            lock (CommandLock)
            {
                if (CommandHandlers.TryGetValue(command.Command, out CommandData cmdData2))
                {
                    Type orig = cmdData2.exec.GetMethodInfo().DeclaringType;
                    Log.Error($"Command ({command.Command}) was already registered from '{orig.Assembly.GetName().Name + "::" + orig.FullName}'");
                }
                else CommandHandlers.Add(command.Command, cmdData = new CommandData() { type = command.CommandType, exec = command.Execute });
            }

            if (command.Aliases != null || command.Aliases.Length > 0)
            {
                foreach (string alias in command.Aliases)
                    lock (CommandLock)
                    {
                        if (CommandHandlers.TryGetValue(alias, out CommandData cmdData2))
                        {
                            Type orig = cmdData2.exec.GetMethodInfo().DeclaringType;
                            Log.Error($"Command ({alias}) was already registered from '{orig.Assembly.GetName().Name + "::" + orig.FullName}'");
                        }
                        else CommandHandlers.Add(alias, cmdData = new CommandData() { type = command.CommandType, exec = command.Execute });
                    }
            }
        }

        public static IRGCCommand AddCommand(Type command)
        {
            foreach (IRGCCommand cmd in Commands)
                if (cmd.GetType() == command)
                {
                    Log.Error($"This command ({cmd.Command}) was already added.");
                    return null;
                }


            IRGCCommand inst = (IRGCCommand)Activator.CreateInstance(command); // Creates an instance of the command

            lock (CommandLock)
            {
                Commands.Add(inst); // Adds the command
                RegisterHandlers(inst); // Registers all of its handlers
            }

            return inst;
        }
        public static void AddCommand(IRGCCommand command)
        {
            foreach (IRGCCommand cmd in Commands)
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
        public static void RegisterCommands()
        {
            Assembly asm = Assembly.GetCallingAssembly();
            foreach (Type type in asm.GetTypes())
            {
                if (type.GetCustomAttribute<RGCCommandAttribute>() == null)
                    continue;

                lock (CommandLock)
                    RegisteredCommands.Add(type, AddCommand(type));
            }
        }

        public static void ReloadHandlers()
        {
            lock (CommandLock)
            {
                CommandHandlers.Clear(); // Clears the handlers

                // Registers all of the added commands
                foreach (IRGCCommand cmd in Commands)
                    RegisterHandlers(cmd);
            }
        }

        public static void RemoveCommand(IRGCCommand command)
        {
            lock (CommandLock)
            {
                if (Commands.Contains(command))
                {
                    if (CommandHandlers.TryGetValue(command.Command, out CommandData cmdData))
                    {
                        if (cmdData.exec.GetMethodInfo().DeclaringType == command.GetType())
                            CommandHandlers.Remove(command.Command);
                    }

                    if (command.Aliases != null || command.Aliases.Length > 0)
                        foreach (string alias in command.Aliases)
                        {
                            if (!CommandHandlers.TryGetValue(alias, out CommandData cmdData2))
                                continue;

                            if (cmdData2.exec.GetMethodInfo().DeclaringType == command.GetType())
                                CommandHandlers.Remove(alias);
                        }

                    Commands.Remove(command);
                }
            }
        }
        public static void UnregisterCommands()
        {
            Assembly asm = Assembly.GetCallingAssembly();
            foreach (Type type in asm.GetTypes())
            {
                if (type.GetCustomAttributes<RGCCommandAttribute>() == null)
                    continue;

                if (RegisteredCommands.TryGetValue(type, out IRGCCommand command))
                    lock (CommandLock)
                    {
                        RemoveCommand(command);
                        RegisteredCommands.Remove(type);
                    }
            }
        }

        // The handler for the commands
        [PluginEvent(ServerEventType.PlayerGameConsoleCommand)]
        private bool OnPlayerCommand(Player player, string command, string[] args)
        {
            if (!CommandHandlers.TryGetValue(command, out CommandData cmdData))
                return true;
            if (!cmdData.type.HasFlag(RGCCommandType.Command))
                return true;

            // Create the sender
            ICommandSender sender = new PlayerCommandSender(player.ReferenceHub);

            string data = "";
            try
            {
                bool success = cmdData.exec(args.Segment(0), sender, out data);

                if (!EventManager.ExecuteEvent(new PlayerGameConsoleCommandExecutedEvent(player.ReferenceHub, command, args, success, data)))
                    return false;
            }
            catch (Exception ex)
            {
                data = "Command execution failed! Error: " + (ex != null ? ex.ToString() : null);
                Log.Error(data);

                if (!EventManager.ExecuteEvent(new PlayerGameConsoleCommandExecutedEvent(player.ReferenceHub, command, args, false, data)))
                    return false;
            }

            // This cannot be null, otherwise we wouldn't have even received the command
            QueryProcessor qp = player.GetComponent<QueryProcessor>();

            // Send the command response to the client
            qp.GCT.SendToClient(qp.connectionToClient, command.ToUpperInvariant() + "#" + data, "");

            return false;
        }

        [PluginEvent(ServerEventType.ConsoleCommand)]
        private bool OnConsoleCommand(ICommandSender sender, string command, string[] args)
        {
            if (!CommandHandlers.TryGetValue(command, out CommandData cmdData))
                return true;
            if (!cmdData.type.HasFlag(RGCCommandType.Command))
                return true;

            string data = "";
            bool success = false;
            try
            {
                success = cmdData.exec(args.Segment(0), sender, out data);

                
                if (!EventManager.ExecuteEvent(new ConsoleCommandExecutedEvent(sender, command, args, success, data)))
                    return false;
            } catch (Exception ex)
            {
                data = "Command execution failed! Error: " + (ex != null ? ex.ToString() : null);
                Log.Error(data);

                if (!EventManager.ExecuteEvent(new ConsoleCommandExecutedEvent(sender, command, args, false, data)))
                    return false;
            }

            if (sender != null)
                ((CommandSender)sender).Print(data, success ? ConsoleColor.Green : ConsoleColor.Red);

            return false;
        }

        [PluginEvent(ServerEventType.RemoteAdminCommand)]
        private bool OnRemoteAdminCommand(ICommandSender sender, string command, string[] args)
        {
            if (!CommandHandlers.TryGetValue(command, out CommandData cmdData))
                return true;
            if (!cmdData.type.HasFlag(RGCCommandType.RemoteAdminCommand))
                return true;

            string data = "";
            string overdisplay = "";
            bool success = false;
            try
            {
                success = cmdData.exec(args.Segment(0), sender, out data);
                
                if (!EventManager.ExecuteEvent(new RemoteAdminCommandExecutedEvent(sender, command, args, success, data)))
                    return false;
            } 
            catch (Exception ex)
            {
                data = "Command execution failed! Error: " + Misc.RemoveStacktraceZeroes(ex.ToString());
                overdisplay = command.ToUpperInvariant() + "#" + data;

                if (!EventManager.ExecuteEvent(new RemoteAdminCommandExecutedEvent(sender, command, args, false, data)))
                    return false;
            }

            if (sender != null)
                ((CommandSender)sender).RaReply(data, success, true, overdisplay);

            return false;
        }
    }
}
