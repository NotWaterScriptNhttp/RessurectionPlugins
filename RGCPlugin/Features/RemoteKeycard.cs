//TODO: Support Alpha Warhead when NW adds it

using System.Reflection;

using InventorySystem.Items;
using InventorySystem.Items.Keycards;

// We can group all of these as they are from the same assembly
using Respawning;
using PlayerRoles;
using Footprinting;
using MapGeneration.Distributors;
using Interactables.Interobjects.DoorUtils;

using PluginAPI.Core;
using PluginAPI.Enums;
using PluginAPI.Events;
using PluginAPI.Core.Attributes;

namespace RGCPlugin.Features
{
    internal class RemoteKeycard
    {
        [PluginEvent(ServerEventType.PlayerInteractLocker)]
        private bool OnPlayerLockerInteract(Player player, Locker locker, LockerChamber chamber, bool canOpen)
        {
            if (!Plugin.GetConfigValue("RemoteKeycard", false))
                return true;

            if (canOpen)
                return true;

            foreach (ItemBase item in player.Items)
            {
                if (item.Category != ItemCategory.Keycard)
                    continue;

                if (((KeycardItem)item).Permissions.HasFlagFast(chamber.RequiredPermissions))
                {
                    chamber.SetDoor(!chamber.IsOpen, null);

                    int num = 1;
                    int num2 = 0;
                    LockerChamber[] chambers = locker.Chambers;
                    for (int i = 0; i < chambers.Length; i++)
                    {
                        if (chambers[i].IsOpen)
                            num2 += num;

                        num *= 2;
                    }
                    if (num2 != locker.OpenedChambers)
                        locker.NetworkOpenedChambers = (ushort)num2;

                    return false;
                }
            }

            return true;
        }

        [PluginEvent(ServerEventType.PlayerInteractDoor)]
        private bool OnPlayerDoorInteract(Player player, DoorVariant door, bool canOpen)
        {
            if (!Plugin.GetConfigValue("RemoteKeycard", false))
                return true;

            // Check if the default logic has executed
            if (canOpen)
                return true;
            // Check if the player is a living and not disarmed
            if (!player.IsHuman || player.IsDisarmed)
                return true;
            // Check if the door is not locked
            if (door.ActiveLocks != 0 && !player.IsBypassEnabled)
                return true;

            ReferenceHub rh = player.ReferenceHub;
            // Check if the door can be interacted with
            if (!door.AllowInteracting(rh, 0))
                return true;

            foreach (ItemBase item in player.Items)
            {
                if (item.Category != ItemCategory.Keycard)
                    continue;

                if (door.RequiredPermissions.CheckPermissions(item, rh))
                {
                    DoorEvents.TriggerAction(door, door.IsConsideredOpen() ? DoorAction.Closed : DoorAction.Opened, rh);
                    door.TargetState = door.NetworkTargetState = !door.IsConsideredOpen();

                    return false;
                }
            }

            return true;
        }

        [PluginEvent(ServerEventType.PlayerInteractGenerator)]
        private bool OnPlayerGeneratorInteract(Player player, Scp079Generator generator, Scp079Generator.GeneratorColliderId id)
        {
            bool HasFlag(Scp079Generator.GeneratorFlags flag)
            {
                return (generator.Network_flags & (byte)flag) == (byte)flag;
            }

            if (!Plugin.GetConfigValue("RemoteKeycard", false))
                return true;

            // Check if the interaction is with the door 
            if (id != Scp079Generator.GeneratorColliderId.Door)
                return true;

            // Check if the generator is unlocked
            if (HasFlag(Scp079Generator.GeneratorFlags.Unlocked))
                return true;

            FieldInfo field = generator.GetType().GetField("_requiredPermission", BindingFlags.Instance | BindingFlags.NonPublic);
            KeycardPermissions permission = field != null ? (KeycardPermissions)field.GetValue(generator) : KeycardPermissions.ArmoryLevelTwo;

            foreach (ItemBase item in player.Items)
            {
                if (item.Category != ItemCategory.Keycard)
                    continue;

                if (((KeycardItem)item).Permissions.HasFlagFast(permission))
                {
                    // This code is pretty much what the server normally does
                    
                    if (EventManager.ExecuteEvent(new PlayerUnlockGeneratorEvent(player.ReferenceHub, generator)))
                    {
                        generator.Network_flags |= (byte)Scp079Generator.GeneratorFlags.Unlocked;

                        Footprint fp = new Footprint(player.ReferenceHub);
                        if (fp.IsSet && fp.Role.GetFaction() != Faction.FoundationStaff)
                            RespawnTokensManager.GrantTokens(SpawnableTeamType.NineTailedFox, 0.5f);
                    }

                    return false; // Return false so we reset the internal timer for the interactions
                }
            }

            return true;
        }
    }
}
