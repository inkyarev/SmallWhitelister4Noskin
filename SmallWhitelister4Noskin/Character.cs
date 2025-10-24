using System;
using System.Collections.Generic;
using System.Linq;
using Tomlet.Attributes;

namespace SmallWhitelister4Noskin
{
    [TomlDoNotInlineObject]
    public class Character
    {
        public Character(string name = "", bool fullyWhitelist = false, int[] skinIds = null)
        {
            Name = Capitalize(name.ToLower());
            FullyWhitelist = fullyWhitelist;
            SkinIds = skinIds ?? Array.Empty<int>();
        }
        public string Name { get; }
        public bool FullyWhitelist { get; }
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
        private static string Capitalize(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}