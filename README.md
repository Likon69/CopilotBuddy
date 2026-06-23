# CopilotBuddy

Public World of Warcraft 3.3.5a (Wrath of the Lich King, build 12340) bot written in C# (.NET 10, WPF, x86). The API surface and behavior are ported from Honorbuddy. 

## ⬇️ Downloads (ready-to-play build)

For players who just want to launch the bot without compiling anything, a complete prebuilt package is provided. It contains everything needed to run CopilotBuddy on a WotLK 3.3.5a (build 12340) client:

- Precompiled `CopilotBuddy.exe` + `Navigation.dll`
- All runtime content: `Bots/`, `Plugins/`, `Routines/`, `Default Profiles/`, `Dungeon Scripts/`, `Languages/`, `Data/`
- `data.bin`, `item_loot.db`, `Spells.bin`
- The matching **`mmaps/` folder** (Trinity 4x4 / MMAP v5) so pathfinding works out of the box — no extractor to run, no tiles to generate
- A `Default Profiles/` folder with profiles ready to load

Just extract the archive next to your WoW 3.3.5a client, double-click `CopilotBuddy.exe`, attach to the game, and you are ready to play.

| Mirror | Download |
| --- | --- |
| **Mega.nz** | https://mega.nz/file/HEQQTJ7S#SYoV2KF1TmJYRSxpD-7glqjbSmGWG18QtvnPsknB8Q0 |
| **Google Drive** | https://drive.google.com/file/d/1KtNk-fCmAHaMVOHJeO65jmfsF-ct94VN/view |

> Both mirrors host the same package. If one is down or full, try the other. Checksums are listed in the Discord.

For more builds, mirror updates, 1x1 / 4x4 mmap variants, additional profiles and community content, join the Discord:

👉 **https://discord.com/invite/ep5TcGMCcB**

## Ecosystem

CopilotBuddy is split across a few sibling repositories. Each one covers one slice of the stack; together they make a complete, build-it-yourself bot:

| Component | Repo | Role |
| --- | --- | --- |
| **Navigation** (C++ Detour runtime) | [Navigation-C-](https://github.com/Likon69/Navigation-C-) | The C++/Detour wrapper that performs pathfinding. Ships `Lib/Navigation.dll` (4x4, Trinity / MMAP v5) and `Lib/Navigation 1x1.dll` (1x1, MaNGOS / MMAP v4). |
| **Extractor (4x4)** — C#, native | [extractor-csharp](https://github.com/Likon69/extractor-csharp) | The MaNGOS-style extractor rewritten in C# / WPF. Produces 4x4 sub-tile `.mmtile` files in HonorBuddy format (PAMM, `mmapVer = 5`). |
| **Extractor (1x1)** — MaNGOS C++ | [Extractor_projects](https://github.com/Likon69/Extractor_projects) | The original MaNGOS extractor. Produces 1x1 `.mmap` / `.mmtile` files for the MaNGOS / MMAP v4 path. |
| **MeshViewer 3D** | [MeshViewer3D](https://github.com/Likon69/MeshViewer3D) | Standalone 3D viewer for the produced navmesh tiles — useful for debugging pathing without launching the bot. |

Use the 4x4 extractor + `Navigation.dll` for Trinity mmaps; use the MaNGOS extractor + `Navigation 1x1.dll` for the older 1x1 layout. The bot auto-detects which one to load from the file header.

## Why this project

CopilotBuddy is a public port of the Honorbuddy API adapted for the WotLK 3.3.5a client and for custom servers. The goal is a bot that anyone can read, modify and contribut.

The porting work started in January 2026. About five months later, the bot is functional on our server: botbases, navigation, questing, dungeons, battlegrounds, gathering and combat routines all run in-game.

## Maintainer's note

CopilotBuddy is a personal project I care about deeply. I first started digging into the Honorbuddy internals back in July 2025, investing serious time on the reverse-engineering side. I eventually hit a wall and stopped for a while, then came back to it in October 2025 and decided to throw everything away and restart from a clean slate. What you see in this repository today is the result of that second attempt, made public in January 2026.

This bot is not a one-shot release. Updates are pushed on a rolling basis as the project matures, and the Discord is where most of the day-to-day news, patch notes and roadmap discussions happen. If you are using CopilotBuddy on your own server, the Discord is the place to follow along.

## Community and contributing

All appropriate contributions are welcome. That includes code, documentation, profile XML, dungeon scripts, translations, bug reports and reproduction steps.

- Discord: join the CopilotBuddy server for updates, help, profile / mmap sharing and to coordinate contributions — **https://discord.com/invite/ep5TcGMCcB**. This is also where additional prebuilt builds, mirrors, alternative mmap variants and other community content are posted.
- Bug reports: open an issue on this repository with the client build, the botbase or plugin involved, the map, and a log excerpt. The community has been extremely valuable in surfacing edge cases I would never have hit on a single server.
- Code contributions: open a pull request. For new botbases or plugins, please follow the patterns described in *Developing a botbase or plugin* below so the project stays consistent.

A sincere thank you to the community around this bot. The bug reports, the test sessions, the profile sharing, the patience during the early unstable builds and the steady stream of feedback are what keep this project moving. CopilotBuddy would not be where it is today without that support, and it is very much appreciated.

## Tech stack

- Language: C# 10 / .NET 10, WPF for the UI, x86 to match the WoW 3.3.5a client
- Injection and memory: custom EndScene hook, direct read/write into the WoW process
- Pathfinding: Detour through a separate C++ wrapper (see the `Navigation C++` project in my github)
- Lua: executed in the game thread through a custom ASM/FASM layer
- Profiles: XML, Honorbuddy-compatible format

## Branches

Two mmap variants are kept side-by-side. Pick the one that matches the mmaps your server ships.

- **`master`** *(default branch on GitHub)* — 4x4 sub-tile navigation (Trinity / MMAP v5). Each ADT is split into 16 Detour sub-tiles of 133 yards, converted through `Tripper.Navigation.MeshMapCalculator`. This is where active development happens.
- **`1x1`** — 1x1 navigation (MaNGOS / MMAP v4). One ADT = one Detour tile of 533 yards. Ships `Lib/Navigation 1x1.dll` (prebuilt MaNGOS runtime) and stays on the conservative 1x1 mmap path.

Both branches share the same bot UI, behaviors, profiles and plugins. The only differences are the navigation stack (Detour tile geometry) and the runtime `Navigation*.dll` shipped under `Lib/`.

## Release

A pre-packaged runtime drop is attached at the root of each branch as **`output.zip`**. Extract it next to `CopilotBuddy.exe` (typically under `bin/Release/net10.0-windows7.0/`) and the bot has everything it needs to run: `Bots/`, `Plugins/`, `Routines/`, `Default Profiles/`, `Dungeon Scripts/`, `Languages/`, `Data/`, `Navigation.dll`, `data.bin`, `item_loot.db`, `Spells.bin`, etc.

The `output/` folder itself is gitignored — only `output.zip` is tracked. Rebuild it whenever runtime content changes:

```powershell
# from the repo root
Compress-Archive -Path .\output\* -DestinationPath .\output.zip -Force
```

## Included botbases

All under `Bots/`. Every botbase inherits from `BotBase` and runs as a synchronous behavior tree.

- `Bots/BGBuddy` - Battlegrounds. Warsong Gulch, Arathi Basin, Eye of the Storm, Alterac Valley, Strand of the Ancients and Isle of Conquest. Battle for Gilneas and Twin Peaks (Cataclysm) classes are present for parity but inactive on 3.3.5a. Handles Isle of Conquest and Strand of the Ancients gates by re-flagging navmesh polygons when their state changes.
- `Bots/DungeonBuddy` - Dungeons. Supports the Dungeon Finder (LFG, added in patch 3.3), SoloFarm mode, automatic role detection (Tank / Healer / DPS) from talents, random or specific queuing, boss handlers and dynamic avoidance. 32 WotLK dungeon scripts and 32 Burning Crusade dungeon scripts are included under `Dungeon Scripts/`.
- `Bots/Quest` - Questing. Full system with `QuestOrder`, `ForcedBehavior` (PickUp, TurnIn, MoveTo, GrindTo, UseItem, If, While, Singleton, Nothing), `QuestObjective` (CollectItem, UseGameObject, Grind, MoveToGrindArea).
- `Bots/Grind` - LevelBot. Combat, loot, vendor, rest, roam, pull, flight, death and resurrection. 3.3.5a offsets only.
- `Bots/Gatherbuddy` - Gathering. Hardcoded lists of every herb and mineral in the game (up to the WotLK 450 skill cap).
- `Bots/DiscoBot` - Party bot / follower with an associated `LeaderPlugin` for IPC coordination between leader and followers.

## Navigation

The navigation code lives in `Tripper/`. It calls `Navigation.dll`, a C++ wrapper around Detour (Recast). See the `Navigation C++` project in the workspace for details on the wrapper itself.

Two mmap formats are supported:
- 1x1 (MaNGOS, MMAP v4): one ADT = one Detour tile of 533 yards
- 4x4 (Trinity, MMAP v5): one ADT = 16 Detour sub-tiles of 133 yards

The format is auto-detected from the file header. The ADT to sub-tile conversion is handled by `Tripper.Navigation.MeshMapCalculator`, with the ADT grid origin at [32, 32] and `detourX = (adt.X - 32) * 4 + subX`.

## Combat routines

Under `bin/.../Routines/`, several combat routines are loaded as plugins. Singular is ported for WotLK. Routines implement `ICombatRoutine` and are called by `RoutineManager` every tick.

## Plugins

Under `bin/.../Plugins/`: AutoEquip2, BuddyControlPanel, BuddyHelper, BuddyManager, DrinkPotions, MrItemRemover2, RareKiller, Talented, TidyBags. They inherit from `Styx.Plugins.PluginClass` and are loaded by `PluginManager` at startup.

## Developing a botbase or plugin

The API is exposed by `CopilotBuddy.dll` (in `bin/Debug/net10.0-windows7.0/` or `bin/Release/...` after publish).

- For a botbase: reference `CopilotBuddy.dll` and implement `Styx.BotBase`. See `Bots/BGBuddy/BGBuddy.cs` as an example.
- For a plugin: reference `CopilotBuddy.dll` and implement `Styx.Plugins.PluginClass`. See any plugin under `bin/.../Plugins/`.
- For a combat routine: implement `Styx.Combat.CombatRoutine.ICombatRoutine` and drop it into `bin/.../Routines/`.

The built-in `SourceCompiler` can also compile C# plugins at runtime from the bot UI.

## Localization

The UI is translated into 15 languages (English, French, German, Spanish, Italian, Portuguese, Russian, Simplified and Traditional Chinese, Japanese, Korean, Turkish, Polish, Dutch, Czech). Strings are generated by `Tools/gen_resx.py` and exposed through `Styx.Localization.Globalization`.

## Build

```
dotnet build CopilotBuddy.csproj -c Release
```

The executable lands in `bin/Release/net10.0-windows7.0/CopilotBuddy.exe` with `Navigation.dll`, `data.bin`, and the `Bots/`, `Plugins/`, `Routines/`, `Profiles/`, `Settings/` and `Dungeon Scripts/` folders.

The C++ navigation wrapper is built separately with Visual Studio 2022 (see `Navigation C++/README.md`).

## Limitations

- Targets the WoW 3.3.5a client build 12340 only. No other clients or expansions are supported.
- Features specific to Cataclysm and later (Mastery, Reforge, Transmogrification, LFR, the currency system, scenarios) are stubs that return neutral values. This is intentional; the bot targets WotLK.

## Acknowledgements

- The community around CopilotBuddy, for the steady flow of bug reports, log dumps and reproduction steps. This project would be a fraction of what it is without that feedback.
- Everyone who has contributed dungeon scripts, profiles, translations and code.
- The Honorbuddy team, whose work made this public port possible.

## Credits and provenance
- HB Team
- API surface and architecture: ported from Honorbuddy 4.3.4 (Cataclysm), decompiled for reference only
- Offsets, memory layout, Lua calls: ported from Honorbuddy 3.3.5a (WotLK)
- Navigation and UI: ported from Honorbuddy 6.2.3 (WoD) and later
- Third-party routines under `bin/.../Routines/` (Singular, etc.) keep their original licenses
- Third-party plugins under `bin/.../Plugins/` keep their original licenses
