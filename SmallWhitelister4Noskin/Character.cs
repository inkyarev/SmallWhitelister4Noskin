using System;
using Tomlet.Attributes;

namespace SmallWhitelister4Noskin
{
    public class Character
    {
        public Character(string name = "", int[] skinIds = null)
        {
            Name = name;
            SkinIds = skinIds ?? Array.Empty<int>();
        }
        public string Name { get; set; }

        [TomlPrecedingComment("Refer to https://martynasxs.dev/skindb for skin ids. Leave empty if you want to whitelist the entire character")]
        public int[] SkinIds { get; set; }
    }
}