using System;
using System.Collections.Generic;
using System.Linq;
using Tomlet.Attributes;

namespace SmallWhitelister4Noskin
{
    public class Character
    {
        public Character(string name = "", bool fullyWhitelist = false, int[] skinIds = null)
        {
            Name = name;
            SkinIds = skinIds ?? Array.Empty<int>();
        }
        public string Name { get; }
        
        public bool FullyWhitelist { get; }

        [TomlPrecedingComment("Refer to https://martynasxs.dev/skindb for skin ids. Leave empty if you want to whitelist the entire character")]
        public int[] SkinIds { get; }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Character)obj);
        }

        protected bool Equals(Character other)
        {
            return Name == other.Name && FullyWhitelist == other.FullyWhitelist && Equals(SkinIds, other.SkinIds);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ FullyWhitelist.GetHashCode();
                if (SkinIds != null)
                {
                    hashCode = SkinIds.Aggregate(hashCode, (current, id) => (current * 397) ^ id.GetHashCode());
                }
                return hashCode;
            }
        }
    }
    
    class CharacterComparer : IEqualityComparer<Character>
    {
        public bool Equals(Character x, Character y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.Name == y.Name;
        }

        public int GetHashCode(Character obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}