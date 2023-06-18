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

        [YamlMember(Description = "If enabled the radios wont discharge")]
        public bool InfiniteRadio { get; private set; } = true;

        [YamlMember(Description = "The color that should be used for MTF", SerializeAs = typeof(string))]
        public TextColor MTFColor { get; private set; } = new TextColor(18, 83, 204);

        [YamlMember(Description = "The color that should be used for CI", SerializeAs = typeof(string))]
        public TextColor CIColor { get; private set; } = new TextColor(31, 110, 9);
    }
}
