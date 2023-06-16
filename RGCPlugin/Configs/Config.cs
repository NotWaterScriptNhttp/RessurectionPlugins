using YamlDotNet.Serialization;

namespace RGCPlugin.Configs
{
    public class Config
    {
        [YamlMember(Description = "Leave empty to use default")]
        public string Translation { get; private set; } = ""; // The translation to be used
    }
}
