using System.Collections.Generic;

namespace SmallWhitelister4Noskin
{
    public class Data
    {
        public static string Path { get; set; } = "do_not_touch";
        public List<Character> WhitelistedCharacters { get; set; } = new List<Character>();
    }
}