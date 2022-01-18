# PearsAndGrayWitch Mod

A mod for the game [PearsAndGrayWitch] to remove various bugs and annoyances, as well
as assist with speedrun practice.

[PearsAndGrayWitch]: https://store.steampowered.com/app/766700/PearsAndGrayWitch/

## Install

Download the [latest release] and extract into the game folder. `doorstop_config.ini`, `version.dll` and the `mod`
sub-folder must end up in the same folder as `PearsAndGrayWitch.exe`.

The mod remains disabled by default. To enable it, either
- edit `doorstop_config.ini` and replace `enabled=false` with `enabled=true` or
- add `--doorstop-enable true` to the game's launch options in Steam or
- create a shortcut to `PearsAndGrayWitch.exe` with the `--doorstop-enable true` argument

Edit `mod.config` to enable or disable the features you want.

The mod uses the [Doorstop] and [HarmonyX] libraries to inject code into the game without modifying game files.

[latest release]: https://github.com/kalimag/PAGW-Mod/releases
[Doorstop]: https://github.com/NeighTools/UnityDoorstop/
[HarmonyX]: https://github.com/BepInEx/HarmonyX/

## Features

- Lock mouse cursor to the game window
- Significantly reduce loading screens
- Visualize the location/shape of death planes and other triggers and collisions
- Menu for quickly loading any level
- Flying and teleporting
- Change mission 7 to not require restarting the game after each attempt
- Change rotating platforms to move at a constant speed independent of framerate

All can be individually enabled/disabled in the `mod.config` file

## Hotkeys

| Hotkey         | Function                                       | Required `mod.config` entry |
|----------------|------------------------------------------------|-----------------------------|
| Ctrl+R         | Restart level                                  | LevelMenu                   |
| F2             | Level select                                   | LevelMenu                   |
| F3             | Trigger visualization menu                     | Visualizations              |
| F4             | Trigger visualization toggle                   | Visualizations              |
| Ctrl+0-9       | Save current location as teleport              | Teleport                    |
| 0-9            | Teleport to saved location                     | Teleport                    |
| E              | Toggle flight                                  | Flying                      |
