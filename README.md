# CopilotBuddy

World of Warcraft 3.3.5a (Wrath of the Lich King, build 12340) bot written in C# (.NET 10, WPF, x86). The API surface and architecture are ported from Honorbuddy and adapted for WotLK private servers.

## Downloads (ready-to-play build)

For players who just want to launch the bot without compiling anything, a complete prebuilt package is provided. It contains everything needed to run CopilotBuddy on a WotLK 3.3.5a (build 12340) client:

- Precompiled `CopilotBuddy.exe` + `Navigation.dll`
- All runtime content: `Bots/`, `Plugins/`, `Routines/`, `Default Profiles/`, `Dungeon Scripts/`, `Languages/`, `Data/`
- `data.bin`, `item_loot.db`, `Spells.bin`
- The matching **`mmaps/` folder** (Trinity 4x4 / MMAP v5) so pathfinding works out of the box — no extractor to run, no tiles to generate
- A `Default Profiles/` folder with profiles ready to load

Just extract the archive next to your WoW 3.3.5a client, double-click `CopilotBuddy.exe`, attach to the game, and you are ready to play.

| Mirror | Download |
| --- | --- |
| **Mega.nz** | https://mega.nz/file/iN4FVISI#s41tkwqg-7rQRWap8BrnnE9fCCtWHG4nilflzyJLuH0 |
| **Google Drive** | https://drive.google.com/file/d/1_JsG9NNmTxUV-3PA_38eaDptDn6nn-Jg/view |

> Both mirrors host the same package. If one is down or full, try the other. Checksums are listed on the Discord.

For more builds, mirror updates, 1x1 / 4x4 mmap variants, additional profiles and community content, join the Discord:

=> **[Discord](https://discord.com/invite/ep5TcGMCcB)** <=

## Ecosystem

CopilotBuddy is split across a few sibling repositories. Each one covers one slice of the stack; together they make a complete, build-it-yourself bot:

| Component | Repo | Role |
| --- | --- | --- |
| **Navigation** (C++ Detour runtime) | [Navigation-C-](https://github.com/Likon69/Navigation-C-) | C++ wrapper around Recast/Detour. Ships `Navigation.dll` (4x4, Trinity / MMAP v5) and `Navigation 1x1.dll` (1x1, MaNGOS / MMAP v4). Based on Honorbuddy's `Tripper.RecastManager` from the WoD / Legion client. |
| **Extractor (4x4)** — C#, native | [extractor-csharp](https://github.com/Likon69/extractor-csharp) | Navmesh extractor written in C# / WPF, ported and heavily extended from the MaNGOS extractor. Produces 4x4 sub-tile `.mmtile` files in HonorBuddy format (PAMM, `mmapVer = 5`). |
| **Extractor (1x1)** — MaNGOS C++ | [Extractor_projects](https://github.com/Likon69/Extractor_projects) | The original MaNGOS extractor. Produces 1x1 `.mmap` / `.mmtile` files for the MaNGOS / MMAP v4 path. |
| **MeshViewer 3D** | [MeshViewer3D](https://github.com/Likon69/MeshViewer3D) | Standalone 3D viewer for the produced navmesh tiles. Useful for debugging pathfinding without launching the bot. |

Use the 4x4 extractor + `Navigation.dll` for Trinity mmaps; use the MaNGOS extractor + `Navigation 1x1.dll` for the older 1x1 layout. The bot auto-detects which format to load from the file header.

## How this project started

Back in 2020 I created a group called **Robot reVolt** to help people run Honorbuddy on private servers — sharing builds, giving support, distributing server-specific content. Over the years the community grew, and one request kept coming back: a bot like HB, but for WotLK 3.3.5a.

I started looking into it in July 2025. The biggest challenge was navigation: I had no idea how WoW tile-based pathfinding worked at a low level. Reading through [https://drewkestell.us/Article/6/Chapter/20](https://drewkestell.us/Article/6/Chapter/20) was what finally made the pieces click — how ADT tiles map to Detour tiles, how the query mesh is built, and why the coordinate system flips the way it does. From there I was able to move forward.

After a first attempt that I scrapped, I restarted from scratch in October 2025. What you see in this repository is the result of that second attempt, made public in January 2026. About five months after the restart, the bot is functional: botbases, navigation, questing, dungeons, battlegrounds, gathering and combat routines all run in-game on our server.

## Credits and provenance

- **API surface and architecture** — ported from Honorbuddy 4.3.4 (Cataclysm), decompiled for reference
- **Offsets, memory layout, Lua calls** — ported from Honorbuddy 3.3.5a (WotLK)
- **Navigation wrapper and UI** — ported from Honorbuddy 6.2.3 (WoD), specifically `Tripper.RecastManager.dll` decompiled
- Third-party routines under `bin/.../Routines/` (Singular, etc.) keep their original licenses
- Third-party plugins under `bin/.../Plugins/` keep their original licenses

## Community and contributing

CopilotBuddy grew out of the **Robot reVolt** community. All appropriate contributions are welcome — code, documentation, profile XML, dungeon scripts, translations, bug reports and reproduction steps.

- Discord: => **[Discord](https://discord.com/invite/ep5TcGMCcB)** <= — updates, help, profile and mmap sharing, patch notes, roadmap discussions, prebuilt builds and alternative mmap variants.
- Bug reports: open an issue with the client build, the botbase or plugin involved, the map, and a log excerpt.
- Code contributions: open a pull request. For new botbases or plugins, follow the patterns described in *Developing a botbase or plugin* below.

## Tech stack

- Language: C# 10 / .NET 10, WPF for the UI, x86 to match the WoW 3.3.5a client
- Injection and memory: custom EndScene hook, direct read/write into the WoW process
- Pathfinding: Detour through a separate C++ wrapper (see `Navigation C++`)
- Lua: executed in the game thread through a custom ASM/FASM layer
- Profiles: XML, Honorbuddy-compatible format

## Branches

Two mmap variants are kept side by side. Pick the one that matches the mmaps your server ships.

- **`master`** — 4x4 sub-tile navigation (Trinity / MMAP v5). Each ADT is split into 16 Detour sub-tiles of 133 yards, converted through `Tripper.Navigation.MeshMapCalculator`. Active development happens here.
- **`1x1`** — 1x1 navigation (MaNGOS / MMAP v4). One ADT = one Detour tile of 533 yards. Ships `Lib/Navigation 1x1.dll`.

Both branches share the same bot UI, behaviors, profiles and plugins. The only differences are the navigation stack (Detour tile geometry) and the `Navigation*.dll` under `Lib/`.

## Release

A pre-packaged runtime drop is attached at the root of each branch as **`output.zip`**. Extract it next to `CopilotBuddy.exe` and the bot has everything it needs to run.

The `output/` folder itself is gitignored — only `output.zip` is tracked. Rebuild it when runtime content changes:

```powershell
# from the repo root
Compress-Archive -Path .\output\* -DestinationPath .\output.zip -Force
```

## Included botbases

All under `Bots/`. Every botbase inherits from `BotBase` and runs as a synchronous behavior tree.

- `Bots/BGBuddy` — Battlegrounds: Warsong Gulch, Arathi Basin, Eye of the Storm, Alterac Valley, Strand of the Ancients, Isle of Conquest. Handles gates and vehicles by re-flagging navmesh polygons when their state changes.
- `Bots/DungeonBuddy` — Dungeons: Dungeon Finder (LFG), SoloFarm mode, automatic role detection, random or specific queuing, boss handlers and dynamic avoidance. 32 WotLK and 32 Burning Crusade dungeon scripts included.
- `Bots/Quest` — Questing: full system with `QuestOrder`, `ForcedBehavior`, `QuestObjective`.
- `Bots/Grind` — LevelBot: combat, loot, vendor, rest, roam, pull, flight, death and resurrection.
- `Bots/Gatherbuddy` — Gathering: hardcoded lists of every herb and mineral up to the WotLK 450 skill cap.
- `Bots/DiscoBot` — Party bot / follower with an associated `LeaderPlugin` for IPC coordination.

## Navigation

The navigation code lives in `Tripper/`. It calls `Navigation.dll`, a C++ wrapper around Detour (Recast) ported from HB 6.2.3. See the `Navigation C++` repository for details.

Two mmap formats are supported:
- 1x1 (MaNGOS, MMAP v4): one ADT = one Detour tile of 533 yards
- 4x4 (Trinity, MMAP v5): one ADT = 16 Detour sub-tiles of 133 yards

The format is auto-detected from the file header. The ADT-to-sub-tile conversion is handled by `Tripper.Navigation.MeshMapCalculator`, with the ADT grid origin at [32, 32] and `detourX = (adt.X - 32) * 4 + subX`.

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
- Features specific to Cataclysm and later are stubs that return neutral values. This is intentional; the bot targets WotLK.
