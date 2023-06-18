using YamlDotNet.Serialization;

using RGCPlugin.Utils;
using YamlDotNet.Core;
using System;

namespace RGCPlugin.Configs
{
    public class Translation
    {
        [YamlMember(Description = "The language code of this translation")]
        public string TranslationCode { get; private set; } = "en";

        [YamlMember(Description = "{0} is minutes, {1} is seconds")]
        public string RespawnTimer { get; private set; } = "You will respawn in {0}:{1}";

        [YamlMember(Description = "{0} is the class that will spawn")]
        public string RespawnTimerAs { get; private set; } = "You will respawn as {0}";

        [YamlMember(Description = "The translation for MTF")]
        public string RespawnTimerMTF { get; private set; } = "Mobile Task Force";

        [YamlMember(Description = "The translation for CI")]
        public string RespawnTimerCI { get; private set; } = "Chaos Insurgency";
    }
}
