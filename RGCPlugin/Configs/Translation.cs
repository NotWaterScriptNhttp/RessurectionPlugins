using YamlDotNet.Serialization;

namespace RGCPlugin.Configs
{
    public class Translation
    {
        [YamlMember(Description = "The language code of this translation")]
        public string TranslationCode { get; private set; } = "en";

        [YamlMember(Description = "{0} is the time until respawn")]
        public string RespawnTimer { get; private set; } = "You will respawn in {0}";

        [YamlMember(Description = "{0} is the class that will spawn, {1} is time until respawn")]
        public string RespawnTimerAs { get; private set; } = "You will respawn as <b>{0}</b> in {1}";

        [YamlMember(Description = "The translation for MTF")]
        public string RespawnTimerMTF { get; private set; } = "Mobile Task Force";

        [YamlMember(Description = "The translation for CI")]
        public string RespawnTimerCI { get; private set; } = "Chaos Insurgency";

        [YamlMember(Description = "{0} is the zone/room where the last player is")]
        public string AnnouncerLastIn { get; private set; } = "Last player in {0}";
        public string AnnouncerLastOn { get; private set; } = "Last player on {0}";
        public string AnnouncerYouAreLast { get; private set; } = "You are the last player";
    }
}
