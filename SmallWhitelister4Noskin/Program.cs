using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tomlet;

namespace SmallWhitelister4Noskin
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            const string constNoskinName = "$noskin";
            const string tomlTemplate = "# Refer to https://martynasxs.dev/skindb for skin ids. \n# If FullyWhitelist is true SkinIds is ignored\n# \n# Path to Noskin mod by Moga\nNoskinPath = ''\n\n[[Characters]]\nName = \"Aurora\"\nFullyWhitelist = true\nSkinIds = [ 4, 5, 6, 7, ]\n\n[[Characters]]\nName = \"Viego\"\nFullyWhitelist = true\nSkinIds = [ ]\n\n";

            #region Process files i.e. config
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

            //var arr1 = tomlTemplate.ToCharArray();
            //var arr2 = configString.ToCharArray();
            //Console.WriteLine("template "+arr1.Length);
            //Console.WriteLine("config "+arr2.Length);

            if (config.Characters.Length == 0 || configString == tomlTemplate)
            {
                Console.WriteLine("Set up the config.toml to start whitelisting");
                Console.ReadKey();
                return;
            }
            #endregion

            #region Check and set paths
            if (config.NoskinPath == string.Empty)
            {
                Console.WriteLine("Uh oh. Seems like you forgot something.");
                GetPath(config);
            }

            if (!Directory.Exists(config.NoskinPath))
            {
                Console.WriteLine("Your noskin path is outdated and doesn't exist");
                GetPath(config);
            }
            
            
            var cslolPath = config.NoskinPath.Split(new[] { @"\installed\" }, StringSplitOptions.None)[0];
            var installedPath = cslolPath + @"\installed";
            var noskinVer = config.NoskinPath.Split(new[] { @"\installed\" }, StringSplitOptions.None)[1]
                .Split(new []{ '_' }, StringSplitOptions.RemoveEmptyEntries);
            var noskinName = noskinVer.Length == 1 ? constNoskinName : $"{constNoskinName}_{noskinVer[1]}";

            try
            {
                if (config.NoskinPath != $@"{installedPath}\{noskinName}")
                {
                    Directory.Move(config.NoskinPath, $@"{installedPath}\{noskinName}");
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                Console.WriteLine("CsLoL directory is being used by another program");
                Console.ReadKey();
                Environment.Exit(5);
            }
            config.NoskinPath = $@"{installedPath}\{noskinName}";
            var noskinWorkingPath = $@"{installedPath}\{noskinName}\WAD";
            
            File.WriteAllText(Config.Path, TomletMain.TomlStringFrom(config));
            #endregion

            #region Restore characters
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
            #endregion

            #region Whitelist characters
            var fullWhitelist = config.Characters.Where(character => character.FullyWhitelist).ToArray();
            var skinsToWhitelist = config.Characters.Except(fullWhitelist);

            #region Fully whitelisted
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
                    catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                    {
                        Console.WriteLine("CsLoL directory is being used by another program");
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
                    catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                    {
                        Console.WriteLine("CsLoL directory is being used by another program");
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
            #endregion

            #region Whitelist separate skins
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
                        pProcess.StartInfo.RedirectStandardOutput = true;
                        pProcess.StartInfo.UseShellExecute = false;
                        pProcess.StartInfo.CreateNoWindow = true;
                        pProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                        pProcess.Start();
                        Console.WriteLine("Extracting wad");
                        var cts = new CancellationTokenSource();
                        Task.Run(() =>
                        {
                            var sw = Stopwatch.StartNew();
                            while (!cts.Token.IsCancellationRequested)
                            {
                                Console.Write($"\rElapsed: {sw.Elapsed:mm\\:ss}");
                                Thread.Sleep(100);
                            }
                        }, cts.Token);
                        pProcess.WaitForExit();
                        cts.Cancel();
                        Console.WriteLine("\nSuccess");
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
                        var characterName = characterPath.Replace($@"{noskinWorkingPath}\{character.Name}.wad\data\characters\", string.Empty);
                        var path1 = $@"{characterPath}\skins\skin{skin}.bin";
                        var wlPath1 = $@"{characterPath}\skins\skin{skin}.bin.whitelisted";
                        if (File.Exists(wlPath1))
                        {
                            if (characterName.ToLower() == character.Name.ToLower())
                            {
                                Console.WriteLine($"skin{skin}.bin for {character.Name} already whitelisted");
                                continue;
                            }
                            Console.WriteLine($"skin{skin}.bin for{characterName} ({character.Name}) already whitelisted");
                            continue;
                        }
                        if (!File.Exists(path1))
                        {
                            if (characterName.ToLower() == character.Name.ToLower())
                            {
                                Console.WriteLine($"No skin{skin}.bin for {character.Name} found. Skipping.");
                                continue;
                            }
                            Console.WriteLine($"No skin{skin}.bin for {characterName} ({character.Name}) found. Skipping.");
                            continue;
                        }

                        try
                        {
                            File.Move(path1, $"{path1}.whitelisted");
                        }
                        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                        {
                            Console.WriteLine("CsLoL directory is being used by another program");
                            Console.ReadKey();
                            Environment.Exit(5);
                        }
                        if (characterName.ToLower() == character.Name.ToLower())
                        {
                            Console.WriteLine($"Whitelisted skin{skin}.bin for {character.Name}");
                            continue;
                        }
                        Console.WriteLine($"Whitelisted skin{skin}.bin for {characterName} ({character.Name})");
                    }
                }
                data.WhitelistedCharacters.Add(character);
            }
            #endregion
            #endregion
             
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
                    var characterName = characterPath.Replace($@"{noskinWorkingPath}\{wlCharacter.Name}.wad\data\characters\", string.Empty);
                    var path = $@"{characterPath}\skins\skin{skin}.bin.whitelisted";
                    if (!File.Exists(path))
                    {
                        if (characterName.ToLower() == wlCharacter.Name.ToLower())
                        {
                            Console.WriteLine($"No skin{skin}.bin for {wlCharacter.Name} found for restore. Skipping.");
                            continue;
                        }
                        Console.WriteLine($"No skin{skin}.bin for {characterName} ({wlCharacter.Name}) found for restore. Skipping.");
                        continue;
                    }

                    try
                    {
                        File.Move(path, path.Replace(".whitelisted", string.Empty));
                    }
                    catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                    {
                        Console.WriteLine("CsLoL directory is being used by another program");
                        Console.ReadKey();
                        Environment.Exit(5);
                    }
                    
                    if (characterName.ToLower() == wlCharacter.Name.ToLower())
                    {
                        Console.WriteLine($"Restored skin{skin}.bin for {wlCharacter.Name}");
                        continue;
                    }
                    Console.WriteLine($"Restored skin{skin}.bin for {characterName} ({wlCharacter.Name})");
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
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    Console.WriteLine("CsLoL directory is being used by another program");
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
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    Console.WriteLine("CsLoL directory is being used by another program");
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

        private static void GetPath(Config config)
        {
            var end = false;
            while (!end)
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

                if (!input.Contains("noskin"))
                {
                    Console.WriteLine("Are you sure? (Y/N)");
                    var key = Console.ReadKey();
                    if (key.Key != ConsoleKey.Y)
                    {
                        Console.WriteLine('\r');
                        continue;
                    }
                    Console.WriteLine('\r');
                }
                config.NoskinPath = input;
                end = true;
            }
        }
    }
}