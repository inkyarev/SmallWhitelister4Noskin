using System;
using Tomlet.Attributes;

namespace SmallWhitelister4Noskin
{
    public class Config
    {
        public static readonly Config Default = new Config
        {
            Characters = new[]
            {
                new Character("Aurora", false, new []{ 4, 5, 6, 7 }),
                new Character("Viego", true)
            }
        };
        
        [TomlNonSerialized]
        public static string Path { get; set; } = "config.toml";
        [TomlPrecedingComment("Refer to https://martynasxs.dev/skindb for skin ids\nIf FullyWhitelist is true SkinIds is ignored\n\nPath to NoSkin mod by Moga, e.g. C:\\cslol-manager\\installed\\riot-skin-disabler-noskin\n")]
        public string NoSkinPath { get; set; } = string.Empty;
        public bool DisplaySecretArt { get; set; }
        public Character[] Characters { get; set; } = Array.Empty<Character>();
    }
}