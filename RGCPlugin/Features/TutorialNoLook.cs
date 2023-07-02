using PluginAPI.Core;
using PluginAPI.Enums;
using PluginAPI.Core.Attributes;

namespace RGCPlugin.Features
{
    internal class TutorialNoLook
    {
        private bool IsAllowed(Player plr)
        {
            if (!Plugin.GetConfigValue("TutorialNoLook", true))
                return false;

            switch (plr.Role)
            {
                case PlayerRoles.RoleTypeId.Tutorial:
                    return false;
            }

            return true;
        }

        [PluginEvent(ServerEventType.Scp173NewObserver)]
        private bool OnPlayerObserve173(Player plr, Player target) => IsAllowed(target);
        [PluginEvent(ServerEventType.Scp096AddingTarget)]
        private bool OnPlayerObserve096(Player plr, Player target, bool look) => IsAllowed(target);
    }
}
