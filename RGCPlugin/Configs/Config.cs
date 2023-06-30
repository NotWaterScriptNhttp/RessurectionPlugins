using YamlDotNet.Serialization;

using RGCPlugin.Utils;

namespace RGCPlugin.Configs
{
    public class Config
    {
        [YamlMember(Description = "Leave empty to use default_lang or use the filename inside locale without .yml")]
        public string Translation { get; private set; } = ""; // The translation to be used

        [YamlMember(Description = "If set to true every color will be serialized as hex (#ffffff)")]
        public bool SerializeColorsAsHex { get; private set; } = true;

        [YamlMember(Description = "Enable to fix the role spawns, should be disabled when nw fixes it or its causing problems")]
        public bool FixSpawnpoints { get; private set; } = true;

        [YamlMember(Description = "If enabled the radios wont discharge")]
        public bool InfiniteRadio { get; private set; } = true;

        [YamlMember(Description = "If enabled you don't need to hold a keycard in your hand")]
        public bool RemoteKeycard { get; private set; } = true;

        [YamlMember(Description = "If enabled spectators will see when they will spawn")]
        public bool RespawnTimer { get; private set; } = true;

        [YamlMember(Description = "If enabled spectators will see their next role")]
        public bool RespawnTimerShowTeam { get; private set; } = true;

        [YamlMember(Description = "If set to true the last player's position will be revealed (with bc, cassie or both)")]
        public bool LastAnnouncer { get; private set; } = true;

        [YamlMember(Description = "If set to true the position will be revealed with a Broadcast")]
        public bool LastAnnouncerWithBC { get; private set; } = true;

        [YamlMember(Description = "If set to true the position will be revealed with a Cassie message")]
        public bool LastAnnouncerWithCassie { get; private set; } = true;

        [YamlMember(Description = "The color that should be used for MTF", SerializeAs = typeof(string))]
        public TextColor MTFColor { get; private set; } = new TextColor(18, 83, 204);

        [YamlMember(Description = "The color that should be used for CI", SerializeAs = typeof(string))]
        public TextColor CIColor { get; private set; } = new TextColor(31, 110, 9);

        [YamlMember(Description = "The color that should be used for last announcers's you are last message", SerializeAs = typeof(string))]
        public TextColor LastMessageColor { get; private set; } = new TextColor(225, 30, 30);
    }
}
