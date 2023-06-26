using System;

using CommandSystem;

namespace RGCPlugin.Commands
{
    [Flags]
    public enum RGCCommandType
    {
        Command = 1,
        RemoteAdminCommand = 2
    }
    public interface IRGCCommand : ICommand, IUsageProvider
    {
        RGCCommandType CommandType { get; }
    }
}
