using System;
using Tomlet.Attributes;

namespace SmallWhitelister4Noskin
{
    public class Config
    {
        [TomlNonSerialized]
        public static string Path { get; set; } = "config.toml";
        [TomlPrecedingComment("Refer to https://martynasxs.dev/skindb for skin ids.\nIf FullyWhitelist is true SkinIds is ignored\n\nPath to Noskin mod by Moga")]
        public string NoskinPath { get; set; } = string.Empty;
        public Character[] Characters { get; set; } = Array.Empty<Character>();
    }
}