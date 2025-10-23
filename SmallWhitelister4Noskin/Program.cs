using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Tomlet;

namespace SmallWhitelister4Noskin
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            const string tomlTemplate = "# Path to Noskin mod by Moga\nNoskinPath = ''\n\n[[Characters]]\nName = \"Alistar\"\nFullyWhitelist = false\n# Refer to https://martynasxs.dev/skindb for skin ids. If FullyWhitelist is true this field is ignored\nSkinIds = [ 1,2,3 ]";
            var configString = string.Empty;
            
            var config = new Config();
            var data = new Data();

            if (File.Exists(Config.Path))
            {
                configString = File.ReadAllText(Config.Path);
                config = TomletMain.To<Config>(configString);
            }

            if (File.Exists(Data.Path))
            {
                data = TomletMain.To<Data>(File.ReadAllText(Data.Path));
            }

            var arr1 = tomlTemplate.ToCharArray();
            var arr2 = configString.ToCharArray();

            if (config.Characters.Length == 0 || configString == tomlTemplate)
            {
                Console.WriteLine("Set up the config.toml to start whitelisting");
                Console.ReadKey();
                return;
            }
            
            if (config.NoskinPath == string.Empty)
            {
                Console.WriteLine("Uh oh. Seems like you forgot something.");
                while (config.NoskinPath == string.Empty)
                {
                    Console.WriteLine(@"Paste the full path to Noskin mod (it's inside cslol-manager\installed)");
                    var input = Console.ReadLine();
                    if (input is null)
                    {
                        Console.WriteLine("I said paste the full path to Noskin mod");
                        continue;
                    }

                    if (!Directory.Exists(input))
                    {
                        Console.WriteLine("This path does not exist");
                        continue;
                    }

                    if (!input.Contains("noskin"))
                    {
                        Console.WriteLine("Are you sure? (Y/N)");
                        var key = Console.ReadKey();
                        if (key.Key != ConsoleKey.Y)
                        {
                            Console.WriteLine('\r');
                            continue;
                        }
                    }
                    config.NoskinPath = input;
                }
            }

            if (!Directory.Exists(config.NoskinPath))
            {
                Console.WriteLine("Your noskin path is outdated and doesn't exist");
                while (config.NoskinPath == string.Empty)
                {
                    Console.WriteLine(@"Paste the full path to Noskin mod (it's inside cslol-manager\installed)");
                    var input = Console.ReadLine();
                    if (input is null)
                    {
                        Console.WriteLine("I said");
                        continue;
                    }

                    if (!Directory.Exists(input))
                    {
                        Console.WriteLine("This path does not exist");
                        continue;
                    }

                    if (!config.NoskinPath.Contains("noskin"))
                    {
                        Console.WriteLine("Are you sure? (Y/N)");
                        var key = Console.ReadKey();
                        if (key.Key != ConsoleKey.Y)
                        {
                            continue;
                        }
                    }
                    config.NoskinPath = input;
                }
            }
            
            var cslolPath = config.NoskinPath.Split(new[] { @"\installed\" }, StringSplitOptions.None)[0];
            var installedPath = cslolPath + @"\installed";
            var noskinName = config.NoskinPath.Split(new[] { @"\installed\" }, StringSplitOptions.None)[1];

            string noskinWorkingPath;
            if (!noskinName.StartsWith("1"))
            {
                try
                {
                    Directory.Move(config.NoskinPath, $@"{installedPath}\1{noskinName}");
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("CsLoL is being used by another program");
                    Console.ReadKey();
                    Environment.Exit(5);
                }
                config.NoskinPath = $@"{installedPath}\1{noskinName}";
                noskinWorkingPath = $@"{installedPath}\1{noskinName}\WAD";
            }
            else
            {
                noskinWorkingPath = $@"{installedPath}\{noskinName}\WAD";
            }
            
            File.WriteAllText(Config.Path, TomletMain.TomlStringFrom(config));

            var clone = new Character [data.WhitelistedCharacters.Count];
            data.WhitelistedCharacters.CopyTo(clone);
            foreach (var wlCharacter in clone)
            {
                var match = config.Characters.FirstOrDefault(character => character.Name == wlCharacter.Name);
                if (match is null)
                {
                    if (wlCharacter.FullyWhitelist)
                    {
                        RestoreFromFullyWhitelisted(noskinWorkingPath, wlCharacter, data);
                    }
                    else
                    {
                        RestoreSkins(wlCharacter, noskinWorkingPath, data);
                    }
                    continue;
                }

                if (wlCharacter.FullyWhitelist && match.FullyWhitelist)
                {
                    continue;
                }

                if (wlCharacter.FullyWhitelist && !match.FullyWhitelist)
                {
                    RestoreFromFullyWhitelisted(noskinWorkingPath, wlCharacter, data);
                }

                if (!wlCharacter.FullyWhitelist && match.FullyWhitelist)
                {
                    RestoreSkins(wlCharacter, noskinWorkingPath, data);
                }

                if (!wlCharacter.FullyWhitelist && !match.FullyWhitelist)
                {
                    if (wlCharacter.SkinIds.Length == match.SkinIds.Length && !wlCharacter.SkinIds.Except(match.SkinIds).Any()) //equal
                    {
                        continue;
                    }
                    RestoreSkins(wlCharacter, noskinWorkingPath, data);
                }
            }
            
            var fullWhitelist = config.Characters.Where(character => character.FullyWhitelist).ToArray();
            var skinsToWhitelist = config.Characters.Except(fullWhitelist);
            foreach (var character in fullWhitelist)
            {
                var path = $@"{noskinWorkingPath}\{character.Name}.wad.client";
                var altPath = $@"{noskinWorkingPath}\{character.Name}.wad";
                var wlPath = $@"{noskinWorkingPath}\{character.Name}.wad.client.whitelisted";
                var wlAltPath = $@"{noskinWorkingPath}\{character.Name}.wad.whitelisted";
                if (File.Exists(path))
                {
                    try
                    {
                        File.Move(path, $"{path}.whitelisted");
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine("CsLoL is being used by another program");
                        Console.ReadKey();
                        Environment.Exit(5);
                    }
                }
                else if (Directory.Exists(altPath))
                {
                    try
                    {
                        Directory.Move(altPath, $"{altPath}.whitelisted");
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine("CsLoL is being used by another program");
                        Console.ReadKey();
                        Environment.Exit(5);
                    }
                }
                else if (File.Exists(wlPath) || Directory.Exists(wlAltPath))
                {
                    Console.WriteLine($"{character.Name} already whitelisted");
                    continue;
                }
                else 
                {
                    Console.WriteLine($"No {character.Name} found. Skipping.");
                    continue;
                }

                data.WhitelistedCharacters.Add(character);
                Console.WriteLine($"Whitelisted {character.Name}");
            }
            
            var continuu = false;
            foreach (var character in skinsToWhitelist)
            {
                if(character.SkinIds.Length == 0) continue;
                
                var path = $@"{noskinWorkingPath}\{character.Name}.wad.client";
                if (File.Exists(path))
                {
                    using(var pProcess = new Process())
                    {
                        pProcess.StartInfo.FileName = $@"{cslolPath}\cslol-tools\wad-extract.exe";
                        pProcess.StartInfo.Arguments = $@"{noskinWorkingPath}\{character.Name}.wad.client";
                        pProcess.StartInfo.UseShellExecute = false;
                        pProcess.StartInfo.CreateNoWindow = false;
                        pProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                        pProcess.Start();
                        pProcess.WaitForExit();
                    }
                    File.Delete(path);
                }
                else if (!Directory.Exists($@"{noskinWorkingPath}\{character.Name}.wad"))
                {
                    Console.WriteLine($"No {character.Name} found. Skipping.");
                    continue;
                }

                foreach (var skin in character.SkinIds)
                {
                    foreach (var characterPath in Directory.GetDirectories(
                                 $@"{noskinWorkingPath}\{character.Name}.wad\data\characters"))
                    {
                        var path1 = $@"{characterPath}\skins\skin{skin}.bin";
                        var wlPath1 = $@"{characterPath}\skins\skin{skin}.bin.whitelisted";
                        if (File.Exists(wlPath1))
                        {
                            Console.WriteLine($"{character.Name} already whitelisted");
                            continuu = true;
                            break;
                        }
                        if (!File.Exists(path1))
                        {
                            Console.WriteLine($"No skin{skin}.bin for {character.Name} found. Skipping.");
                            continue;
                        }

                        try
                        {
                            File.Move(path1, $"{path1}.whitelisted");
                        }
                        catch (UnauthorizedAccessException)
                        {
                            Console.WriteLine("CsLoL is being used by another program");
                            Console.ReadKey();
                            Environment.Exit(5);
                        }
                        Console.WriteLine($"Whitelisted skin{skin}.bin for {character.Name}");
                    }
                    if(continuu) break;
                }
                if(continuu) continue;
                data.WhitelistedCharacters.Add(character);
            }
            File.WriteAllText(Data.Path, TomletMain.TomlStringFrom(data));

            Console.WriteLine("Done");

            Console.ReadKey();
        }

        private static void RestoreSkins(Character wlCharacter, string noskinWorkingPath, Data data)
        {
            foreach (var skin in wlCharacter.SkinIds)
            {
                foreach (var characterPath in Directory.GetDirectories(
                             $@"{noskinWorkingPath}\{wlCharacter.Name}.wad\data\characters"))
                {
                    var path = $@"{characterPath}\skins\skin{skin}.bin.whitelisted";
                    if (!File.Exists(path))
                    {
                        Console.WriteLine($"No skin{skin}.bin for {wlCharacter.Name} found for restore. Skipping.");
                        continue;
                    }

                    try
                    {
                        File.Move(path, path.Replace(".whitelisted", string.Empty));
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine("CsLoL is being used by another program");
                        Console.ReadKey();
                        Environment.Exit(5);
                    }
                    
                    Console.WriteLine($"Restored skin{skin}.bin for {wlCharacter.Name}");
                }
            }

            data.WhitelistedCharacters.Remove(wlCharacter);
        }

        private static void RestoreFromFullyWhitelisted(string noskinWorkingPath, Character wlCharacter, Data data)
        {
            var path = $@"{noskinWorkingPath}\{wlCharacter.Name}.wad.whitelisted";
            var altPath = $@"{noskinWorkingPath}\{wlCharacter.Name}.wad.client.whitelisted";
            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Move(path, path.Replace(".whitelisted", string.Empty));
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("CsLoL is being used by another program");
                    Console.ReadKey();
                    Environment.Exit(5);
                }
            }
            else if (File.Exists(altPath))
            {
                try
                {
                    File.Move(altPath, altPath.Replace(".whitelisted", string.Empty));
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("CsLoL is being used by another program");
                    Console.ReadKey();
                    Environment.Exit(5);
                }
            }
            else
            {
                Console.WriteLine($"No {wlCharacter.Name} found for restore. Skipping.");
                return;
            }

            data.WhitelistedCharacters.Remove(wlCharacter);
            Console.WriteLine($"Restored {wlCharacter.Name}");
        }
    }
}