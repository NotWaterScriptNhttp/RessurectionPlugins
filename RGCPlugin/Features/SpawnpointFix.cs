using MEC;
using UnityEngine;

using PlayerRoles;
using PlayerRoles.FirstPersonControl;

using PluginAPI.Core;
using PluginAPI.Enums;
using PluginAPI.Core.Attributes;

namespace RGCPlugin.Features
{
    internal class SpawnpointFix
    {
        private static (bool, Vector3, float) GetSpawnpoint(RoleTypeId id)
        {
            if (!PlayerRoleLoader.TryGetRoleTemplate<PlayerRoleBase>(id, out PlayerRoleBase role))
                return (false, Vector3.zero, 0f);

            IFpcRole fpc = role as IFpcRole;
            if (fpc == null || fpc.SpawnpointHandler == null)
                return (false, Vector3.zero, 0f);

            fpc.SpawnpointHandler.TryGetSpawnpoint(out Vector3 pos, out float rot);
            return (true, pos, rot);
        }

        [PluginEvent(ServerEventType.PlayerSpawn)]
        private void OnPlayerSpawn(Player plr, RoleTypeId id)
        {
            if (!Plugin.GetConfigValue("FixSpawnpoints", false))
                return;

            if (id.HasFlag(RoleTypeId.None) || id.HasFlag(RoleTypeId.Spectator) || id.HasFlag(RoleTypeId.Scp079) || id.HasFlag(RoleTypeId.Scp0492) || id.HasFlag(RoleTypeId.Tutorial))
                return;

            (bool succ, Vector3 pos, float rot) = GetSpawnpoint(id);
            if (!succ)
            {
                Log.Error($"Failed to get random spawnpoint for RID: {id}, Plr: {plr.Nickname}");
                return;
            }

            PlayerRoleBase role = plr.RoleBase;
            if (!role.ServerSpawnFlags.HasFlag(RoleSpawnFlags.UseSpawnpoint))
                return;

            ReferenceHub hub = plr.ReferenceHub;
            hub.transform.position = pos;

            IFpcRole fpc;
            if ((fpc = (IFpcRole)role) != null)
                fpc.FpcModule.MouseLook.CurrentHorizontal = rot;

            FpcExtensionMethods.TryOverridePosition(hub, pos, Vector3.zero);

            Timing.CallDelayed(Time.fixedDeltaTime * 5, () =>
            {
                plr.Rotation = Vector3.up * rot;
            });
        }
    }
}
