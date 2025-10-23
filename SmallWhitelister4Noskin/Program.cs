using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Tomlet;

namespace SmallWhitelister4Noskin
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            const string tomlTemplate = "# Path to Noskin mod by Moga\r\nNoskinPath = ''\r\n\r\n[[Characters]]\r\nName = \"Yunara\"\r\n# Refer to https://martynasxs.dev/skindb for skin ids. Leave empty if you want to whitelist the entire character\r\nSkinsIds = [ 2, 3, 4, 5, 6, 7, ]";
            var configString = string.Empty;
            
            var config = new Config();
            Console.WriteLine(Environment.CurrentDirectory);

            if (File.Exists(Config.Path))
            {
                configString = File.ReadAllText(Config.Path);
                config = TomletMain.To<Config>(configString);
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
                    return;
                }
                config.NoskinPath = $@"{installedPath}\1{noskinName}";
                noskinWorkingPath = $@"{installedPath}\1{noskinName}\WAD";
            }
            else
            {
                noskinWorkingPath = $@"{installedPath}\{noskinName}\WAD";
            }
            
            File.WriteAllText(Config.Path, TomletMain.TomlStringFrom(config));

            var fullWhitelist = config.Characters.Where(character => character.SkinIds.Length == 0).ToArray();
            var skinsToWhitelist = config.Characters.Except(fullWhitelist);
            foreach (var character in fullWhitelist)
            {
                var path = $@"{noskinWorkingPath}\{character.Name}.wad.client";
                if (!File.Exists(path))
                {
                    Console.WriteLine($"No {character.Name} found. Skipping.");
                    continue;
                }

                File.Delete(path);
                Console.WriteLine($"Whitelisted {character}");
            }
            
            foreach (var character in skinsToWhitelist)
            {
                if (!File.Exists($@"{noskinWorkingPath}\{character.Name}.wad.client"))
                {
                    Console.WriteLine($"No {character.Name} found. Skipping.");
                    continue;
                }

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
                var path = $@"{noskinWorkingPath}\{character.Name}.wad.client";
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                foreach (var skin in character.SkinIds)
                {
                    foreach (var characterPath in Directory.GetDirectories(
                                 $@"{noskinWorkingPath}\{character.Name}.wad\data\characters"))
                    {
                        var path1 = $@"{characterPath}\skins\skin{skin}.bin";
                        if (!Directory.Exists(path1))
                        {
                            Console.WriteLine($"No skin{skin}.bin for {character.Name} found. Skipping.");
                            continue;
                        }

                        File.Delete(path1);
                        Console.WriteLine($"Whitelisted skin{skin}.bin for {character.Name}");
                    }
                }
            }

            Console.WriteLine("Done");

            Console.ReadKey();
        }
    }
}