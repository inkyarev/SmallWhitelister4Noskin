using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
            const string wadExtractPath = "wad-extract.exe";
            const string tomlTemplate = "# Refer to https://martynasxs.dev/skindb for skin ids. \n# If FullyWhitelist is true SkinIds is ignored\n# \n# Path to Noskin mod by Moga\nNoskinPath = ''\n\n[[Characters]]\nName = \"Aurora\"\nFullyWhitelist = false\nSkinIds = [ 4, 5, 6, 7, ]\n\n[[Characters]]\nName = \"Viego\"\nFullyWhitelist = true\nSkinIds = [ ]\n\n";
            const string nameArt = "  _   _      ____  _    _        __        ___     _ _       _ _     _            \n | \\ | | ___/ ___|| | _(_)_ __   \\ \\      / / |__ (_) |_ ___| (_)___| |_ ___ _ __ \n |  \\| |/ _ \\___ \\| |/ / | '_ \\   \\ \\ /\\ / /| '_ \\| | __/ _ \\ | / __| __/ _ \\ '__|\n | |\\  | (_) |__) |   <| | | | |   \\ V  V / | | | | | ||  __/ | \\__ \\ ||  __/ |   \n |_| \\_|\\___/____/|_|\\_\\_|_| |_|   _\\_/\\_/  |_| |_|_|\\__\\___|_|_|___/\\__\\___|_|   \n                         | __ ) _   _  |  _ \\ _____   __                               \n                         |  _ \\| | | | | |_) / _ \\ \\ / /                               \n                         | |_) | |_| | |  _ <  __/\\ V /                                \n                         |____/ \\__, | |_| \\_\\___| \\_/                                 \n                                |___/                                                  ";
            const string girlsKissing = "         -             @#*%                                                                   @:     @@@       @@@      \n        :         .#%***%*@*****@#*@@@@#.                                                   . -    @@@@@@    *@@@*%@@@@ \n       .      %***+****+*%#**+#**%*************@                                         @@@ : .-  %%@@###%   @@@  @    \n       .  @*@*+*#****##%**#@*######*#####*####***#.                                   @  %  + *- %%@  %##%   @@  @ =**% \n       .#***#*####*#*#**###****#%****##%##########***                                : :-  :: *-@# :  .##@   #= %- --- .\n**##*#*#*#********#**#*******%#*****#######*#####*%*****                       . =     .%-@@ .@%%@%@:   %@@@+=-=   -- - \n*##**********#**#*#********#*%*******##*%##########*****                    :=: .:----%----:        ::.@@@ --:=*- ----  \n***********#***#*##*********#***********##%####*#######*                  + *:--------------= -:     @%@@=   -.=* ---:- \n*******#********#*#***********************##%##*##**#%#*              .: -:----::**-+=---::---=:----@+@@@    :.=+ -:::--\n****#**********#*************#*************##%*##**#*###            *.:-::-::::*-::::+:::::::::+=:+=-:@@@     . =:-:::--\n*##*****************************************#####****#%%#*         ::-:::::::-*:::::-+-::::::::::%%::#@@@     ::-:::::--\n#***********************************************+*####*%%##     -::::-.::::-:*-::::::*::::::::::@@-:-@@@+     :.::::::--\n**********%********************#*#***********+*+**######## *  *=.   -.:::::::-:::::::+:-:::::::@@#:%:@@@=     . .:::-:-:\n*******%**#********************%**%******+***+**##**######@ #=.:  :.:: ::.::-:::::::::=:::::::@@@=::%@@*:     . -:::::-:\n***#**%******+*****************#%%*=*+===+******##*#*####@# =:::=-:.. :-  :+*=::::::::-::::::+@@@-###@@::     ..+:::::-:\n**%***#******++=+**=***********%.*%*:%+=+********#@*#####@#---:-::::::-.. #+*-::::::::::-:::::::@@##@@:::     :.::::-::-\n%@#*****##*#++++**+==*****=*=**@..* *+=******#***#%#*###%%=--::*:::::-::-  :=:::::::::::::::::::::. @%-::     :.::::-:--\n*****@***%***++++*===*++===+===:. --  *.#*****#***#.%####%%+--+-::::::::   ==::   :::::. ::.::: :. =@::-.     :-:::-:-::\n****##**%#***++++*++***+==+=*=*.   =   :.#****#****.=*##%##+--*::::::::    *: .    :: . : :-.   :::*@-.:.    . ::::::-:-\n*******#*#*********************     :    .*********#.*##*=--=.-:::::::     *::::::::::     :::..::-%@:::.    ::::-:::-:-\n*******%*#*******************+        *+:  =*******.*@%%=*--:+:::::::   ::: ::.::::::::::::-.::::-:@@:::     .::-::::---\n##****##******************+++*     *.   -   #*****#*@@%%* #:.*::::::=.:     %::::::::::::-::= :::-+@@+::     ::-:-::-:--\n*****%*******************++++   +      =     -****%@#*#-. %:.=-::::-  -      :::::::::::::::: ::::=@@-::    ::::-:-:-:::\n****#******************+*++=*                +****:+*#  * :*.--::::    .      :-:::::::-:::::: . :+@*::*    -::-:::---::\n**********************++*==*        .. +%@+=   +*#   *#    #..::+::*%%@@%%*: : -.::::::-:::::*  .:%@-+--   :-:--:-:-:-::\n#**#*****##**********+*%==*       .=@%@@      *+@=        -  *=::-+=: **     -#%*::-:::-:::::-    @@:::+   -- -:-:---:--\n##******###*##******+*+==+    :*%#%. -#**    *+*=              -:*=  *       :  -:.=::::::::::  ..@@:-*+  ---.-:-:-:--::\n**#@%-=%#%*#********++%==  .=+.*=+- :.= .   .+                  *:- .           *::.:::::::-::  .@@@+..*  :=:     :.:--:\n@@@:*:-#@:*##******+*%=+:             .   .-            -       :*  .   ..      -:* :::::-:::=:-:@@*.-:: -=-::   ::.::::\n*#%-::#=*+##**##****%=#=#         ..                              :          == +=  ::-::=::-+ -:@@  .-. :-:--: .-:-::::\n*##%:....*#####**@*%*++==                               :                      +   -:*::::-::+::-@%+  . :-----:.:.::-:::\n##%=:.  -*#*#%*@-%#*+++==*                             -                         : -*:-::-::--*-@@:-: : -::---:::  -::::\n#*%@: -  ###%*   :%*+++==*                           # *                          :  :+:: +-=-  : =:: + -=---:---.: ::::\n##@#*@: +#*:      #**=+==.#                          :  *                            +%*+ :++= == ::  -=-:-:----:: :.:::\n#####**%*#:   --   @*++*=- %                         . .                   -          **::*-:*      @* --------::: .::::\n#**####******%* -@  #*+++*  -                       %  *               -=         .=*:----   *   *=. +--=------:- :    :\n%    +%####*#        #*+++     .                         =           :        .*       =       **     =----:---::.:.    \n #     #%%#%:         **++=      .                       %              .               :      *-=-::-=:=-----:-::::   :\n        ##%.           %**:                             .                                :@+ ..#-++**==-----::-::::::  \n#@    =%+%.             #** :      :##%*-.          .    .           =                           :=+=-*------:-::::::: -\n##   %  *   =            +*+  :   =-------*--  : . .      :                          .          - + =**---------------::\n#   %          :          :*    *---------==.               + .=#%*                      :*    :  *#------=-----------*-\n     +-           :         #  # =-=+***+-==                :    *                    . = :    :   @-----=-----------+--\n    = *               .      %=  =====+===                  :                             :        #----=----------+=-::\n#    :%                    %@@ =     -=====                 -   :                      :+*       %*---==----------==-:-.";
            
            Console.WriteLine(nameArt);
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
                Console.WriteLine("[WRN] Please configure your config.toml and restart the whitelister");
                Console.ReadKey();
                return;
            }
            #endregion

            #region Check and set paths
            if (config.NoskinPath == string.Empty)
            {
                GetPath(config);
            }

            if (!Directory.Exists(config.NoskinPath))
            {
                Console.WriteLine("[ERR] Your NoSkin path does not exist!");
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
                Console.WriteLine("[ERR] CsLoL directory is currently in use by another process and cannot be accessed");
                Console.ReadKey();
                Environment.Exit(5);
            }
            config.NoskinPath = $@"{installedPath}\{noskinName}";
            var noskinWorkingPath = $@"{installedPath}\{noskinName}\WAD";
            
            File.WriteAllText(Config.Path, TomletMain.TomlStringFrom(config));
            #endregion

            #region Restore characters
            var stopwatch = Stopwatch.StartNew();
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
                var swInd = Stopwatch.StartNew();
                Console.WriteLine($"[INF] Whitelisting: {character.Name}");
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
                        Console.WriteLine("[ERR] CsLoL directory is currently in use by another process and cannot be accessed");
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
                        Console.WriteLine("[ERR] CsLoL directory is currently in use by another process and cannot be accessed");
                        Console.ReadKey();
                        Environment.Exit(5);
                    }
                }
                else if (File.Exists(wlPath) || Directory.Exists(wlAltPath))
                {
                    Console.WriteLine($"[INF] {character.Name} is already whitelisted");
                    Console.WriteLine();
                    continue;
                }
                else 
                {
                    Console.WriteLine($"[WRN] Failed to find {character.Name}. Skipping");
                    Console.WriteLine();
                    continue;
                }

                data.WhitelistedCharacters.Add(character);
                Console.WriteLine($"[INF] Finished {character.Name}: Time {swInd.Elapsed:mm\\:ss\\.ff}");
                Console.WriteLine();
            }
            #endregion

            #region Whitelist separate skins
            foreach (var character in skinsToWhitelist)
            {
                if(character.SkinIds.Length == 0) continue;
                
                var swInd = Stopwatch.StartNew();
                var skinStr = string.Join(", ", character.SkinIds.Select(skinId => $"Skin{skinId}"));
                var path = $@"{noskinWorkingPath}\{character.Name}.wad.client";
                if (File.Exists(path))
                {
                    using(var pProcess = new Process())
                    {
                        pProcess.StartInfo.FileName = wadExtractPath;
                        pProcess.StartInfo.Arguments = $@"{noskinWorkingPath}\{character.Name}.wad.client";
                        pProcess.StartInfo.RedirectStandardOutput = false;
                        pProcess.StartInfo.UseShellExecute = false;
                        pProcess.StartInfo.CreateNoWindow = true;
                        pProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                        pProcess.Start();
                        Console.WriteLine($"[INF] Extracting: {character.Name}");
                        pProcess.WaitForExit();
                    }
                    File.Delete(path);
                }
                else if (!Directory.Exists($@"{noskinWorkingPath}\{character.Name}.wad"))
                {
                    Console.WriteLine($"[WRN] Failed to find {character.Name}. Skipping");
                    continue;
                }

                Console.WriteLine($"[INF] Whitelisting: [{skinStr}] for {character.Name}");
                foreach (var characterPath in Directory.GetDirectories(
                             $@"{noskinWorkingPath}\{character.Name}.wad\data\characters"))
                {
                    var characterName = characterPath.Replace($@"{noskinWorkingPath}\{character.Name}.wad\data\characters\", string.Empty);
                    var awlList = new List<string>();
                    var ftfList = new List<string>();
                    foreach (var skin in character.SkinIds)
                    {
                        var path1 = $@"{characterPath}\skins\skin{skin}.bin";
                        var wlPath1 = $@"{characterPath}\skins\skin{skin}.bin.whitelisted";
                        if (File.Exists(wlPath1))
                        {
                            awlList.Add($"Skin{skin}");
                            continue;
                        }

                        if (!File.Exists(path1))
                        {
                            ftfList.Add($"Skin{skin}");
                            continue;
                        }

                        try
                        {
                            File.Move(path1, $"{path1}.whitelisted");
                        }
                        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                        {
                            Console.WriteLine(
                                "[ERR] CsLoL directory is currently in use by another process and cannot be accessed");
                            Console.ReadKey();
                            Environment.Exit(5);
                        }
                    }

                    if (awlList.Count > 0)
                    {
                        Console.WriteLine(characterName.ToLower() == character.Name.ToLower()
                            ? $"[INF] Already whitelisted: [{string.Join(", ", awlList)}] for {character.Name} "
                            : $"[INF] Already whitelisted: [{string.Join(", ", awlList)}] for {characterName} ({character.Name})");
                    }
                    if (ftfList.Count > 0)
                    {
                        Console.WriteLine(characterName.ToLower() == character.Name.ToLower()
                            ? $"[INF] Failed to find: [{string.Join(", ", ftfList)}] for {character.Name}"
                            : $"[INF] Failed to find: [{string.Join(", ", ftfList)}] for {characterName} ({character.Name})");
                    }
                }

                Console.WriteLine($@"[INF] Finished whitelisting: [{skinStr}] for {character.Name}; Time [{swInd.Elapsed:mm\:ss\.ff}]");
                if (!data.WhitelistedCharacters.Contains(character))
                {
                    data.WhitelistedCharacters.Add(character);
                }
                Console.WriteLine();
            }
            #endregion
            #endregion
             
            File.WriteAllText(Data.Path, TomletMain.TomlStringFrom(data));

            Console.WriteLine();
            Console.WriteLine($"[INF] Finished Whitelist! Total time: [{stopwatch.Elapsed:mm\\:ss\\.ff}]");
            //Console.WriteLine(art2);
            Console.WriteLine($"Press Enter to exit...");
            Console.ReadKey();
        }

        private static void RestoreSkins(Character wlCharacter, string noskinWorkingPath, Data data)
        {
            var skinStr = string.Join(", ", wlCharacter.SkinIds.Select(skinId => $"Skin{skinId}"));
            foreach (var characterPath in Directory.GetDirectories(
                         $@"{noskinWorkingPath}\{wlCharacter.Name}.wad\data\characters"))
            {
                var characterName =
                    characterPath.Replace($@"{noskinWorkingPath}\{wlCharacter.Name}.wad\data\characters\", string.Empty);

                Console.WriteLine(characterName.ToLower() == wlCharacter.Name.ToLower()
                    ? $"[INF] Restoring: [{skinStr}] for {wlCharacter.Name}"
                    : $"[INF] Restoring: [{skinStr}] for {characterName} ({wlCharacter.Name})");

                var ftfList = new List<string>();
                foreach (var skin in wlCharacter.SkinIds)
                {
                    var path = $@"{characterPath}\skins\skin{skin}.bin.whitelisted";
                    if (!File.Exists(path))
                    {
                        ftfList.Add($"Skin{skin}");
                        continue;
                    }

                    try
                    {
                        File.Move(path, path.Replace(".whitelisted", string.Empty));
                    }
                    catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                    {
                        Console.WriteLine("[ERR] CsLoL directory is currently in use by another process and cannot be accessed");
                        Console.ReadKey();
                        Environment.Exit(5);
                    }
                }
                if (ftfList.Count > 0)
                {
                    Console.WriteLine(characterName.ToLower() == wlCharacter.Name.ToLower()
                        ? $"[INF] Failed to find: [{string.Join(", ", ftfList)}] for {wlCharacter.Name}"
                        : $"[INF] Failed to find: [{string.Join(", ", ftfList)}] for {characterName} ({wlCharacter.Name})");
                }
            }

            Console.WriteLine($"[INF] Finished restoring: [{skinStr}] for {wlCharacter.Name}");
            Console.WriteLine();
            data.WhitelistedCharacters.Remove(wlCharacter);
        }

        private static void RestoreFromFullyWhitelisted(string noskinWorkingPath, Character wlCharacter, Data data)
        {
            Console.WriteLine($"[INF] Restoring: {wlCharacter.Name}");
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
                    Console.WriteLine("[ERR] CsLoL directory is currently in use by another process and cannot be accessed");
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
                    Console.WriteLine("[ERR] CsLoL directory is currently in use by another process and cannot be accessed");
                    Console.ReadKey();
                    Environment.Exit(5);
                }
            }
            else
            {
                Console.WriteLine($"[WRN] Failed to find {wlCharacter.Name}. Skipping");
                return;
            }

            data.WhitelistedCharacters.Remove(wlCharacter);
            Console.WriteLine($"[INF] Finished restoring: {wlCharacter.Name}");
            Console.WriteLine();
        }

        private static void GetPath(Config config)
        {
            var end = false;
            while (!end)
            {
                Console.WriteLine(@"[INF] Paste the full path to NoSkin here and press enter, Ex: C:\mods\cslol\installed\noskin");
                var input = Console.ReadLine();
                if (input is null || !Directory.Exists(input))
                {
                    Console.WriteLine("[ERR] This path does not exist!");
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

            Console.WriteLine();
        }
    }
}