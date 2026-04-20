# Professional skin whitelisting software by Rev™

## Whatawhy
Automatic whitelisting of skins for [NoSkin Mod](https://runeforge.dev/mods/278e13db-0ecd-46ef-8282-f0f8b07fc08b) (by [Moga](https://runeforge.dev/users/Moga)). Basically his mod turns skins off and my soft can turn them back on automatically (because frequent updates), though you can always do it manually.

Made this for myself then sorta remade for the general public for funzies.

## Usage
1. Drop NoSkin into CSLoL.
2. Configure `config.toml`:
** Enter the *full* unquoted path to your `cslol-manager\installed\NoSkin` inside the single quotes: `NoSkinPath = ''`.
** Fill out `config.toml` (following the provided template's formatting) with Champion names and Skin IDs for the champs and/or skins you want to see.
** Double click and run `SmallWhitelister4Noskin.exe`
** Start CSLoL. **If CSLoL was running while you ran the Whitelister, toggle the start button to refresh your profiles.**

If you update your `config.toml` afterward to add or remove more skins, simply run the Whitelister again and it'll re-enable/disable skins flawlessly.

## Building From Source
Install .NET framework 4.8, then your IDE should auto restore nuget pkg, if it doesnt run `restore nuget`.

## Attribution
Includes wad-extract.exe from [cslol manager tools](https://github.com/LeagueToolkit/cslol-manager)

## Other Info
Of course you need [NoSkin](https://runeforge.dev/mods/278e13db-0ecd-46ef-8282-f0f8b07fc08b) by [Moga](https://runeforge.dev/users/Moga).

I reside in [this discord server](https://discord.gg/HEjuFmbm6g) so come there or [open an issue](https://github.com/inkyarev/SmallWhitelister4Noskin/issues/new) here for help.

<!-- hanlo :tahi:  -->
