# AGENTS.md — CopilotBuddy

## Build

```powershell
dotnet build CopilotBuddy.csproj -c Debug
```

Single project, no solution file. Output: `bin/Debug/net10.0-windows7.0/`

## Project Layout

```
CopilotBuddy.csproj          # net10.0-windows7.0, x86, WPF+WinForms, WinExe
                              # Requires administrator privileges (memory access to WoW process)
GreenMagic/                  # EndScene hook, memory read/write, ASM injection
Styx/                        # Core engine
  ├── WoWInternals/          # ObjectManager, WoWObjects, Lua, LuaEvents
  ├── Logic/                 # Combat, Pathing, Profiles, Inventory, Questing
  ├── Offsets/               # 3.3.5a memory offsets (UnitFlags, NpcFlags, etc.)
  ├── Loaders/               # DynamicLoader (Roslyn), AssemblyFactory, SourceCompiler
  ├── CommonBot/             # Inventory, Frames, Coroutines, CharacterManagement
  └── ...
TreeSharp/                  # Behavior tree library (namespace: TreeSharp, not Styx.TreeSharp)
Tripper/Navigation/         # Navigator, pathfinding via Navigation.dll (P/Invoke)
CommonBehaviors/            # Reusable Actions/Decorators
Bots/                       # DungeonBuddy, GatherBuddy, Grind, Quest
Lib/                        # Native DLLs: Navigation.dll, fasmdll_managed.dll,
                            # BlackMagic.dll, System.Data.SQLite.dll, RecastManaged.dll
UI/                         # WPF windows (MainWindow, PluginsWindow, etc.)
datadb/                     # Python scripts + SQLite DBs (CreatureSpawns.db, item_loot.db)
Offsets335.txt             # 6184-line 3.3.5a memory offset reference
```

## Reference hb decompile dans tripper.recastmanager 

Default Profiles C:\Users\Texy6\Desktop\newhcb\CopilotBuddy\bin\Debug\net10.0-windows7.0\Default Profiles

Singular C:\Users\Texy6\Desktop\newhcb\CopilotBuddy\bin\Debug\net10.0-windows7.0\Routines\Singular wotlk

Quest Behaviors C:\Users\Texy6\Desktop\newhcb\CopilotBuddy\bin\Debug\net10.0-windows7.0\Quest Behaviors

Plugins C:\Users\Texy6\Desktop\newhcb\CopilotBuddy\bin\Debug\net10.0-windows7.0\Plugins

Dungeon Scripts C:\Users\Texy6\Desktop\newhcb\CopilotBuddy\bin\Debug\net10.0-windows7.0\Dungeon Scripts

Botbase C:\Users\Texy6\Desktop\newhcb\CopilotBuddy\bin\Debug\net10.0-windows7.0\Bots

C:\Users\Texy6\Desktop\newhcb\hb decompile\.hb 4.3.4\Honorbuddy\Honorbuddy

C:\Users\Texy6\Desktop\newhcb\hb decompile\.hb 6.2.3\Honorbuddy

C:\Users\Texy6\Desktop\newhcb\hb decompile\.hb 6.2.3\Tripper.RecastManaged



## Runtime Structure

External code is loaded at runtime into `bin/Debug/net10.0-windows7.0/`:
- `Bots/` — bot base implementations
- `Plugins/` — HB plugin assemblies
- `Quest Behaviors/` — quest behavior assemblies
- `Routines/` — combat rotations (e.g. `Singular wotlk`)
- `Default Profiles/` — XML quest/gather profiles
- `Dungeon Scripts/` — dungeon script assemblies

Plugins/QuestBehaviors/Routines are **compiled at runtime** via Roslyn (`DynamicLoader`). They are not statically linked.

`Navigation.dll` is loaded via `[DllImport]` and must be placed next to the exe.

## Key Conventions

- **No async/await in bot logic** — synchronous pulse loop only
- **WoWGuid is `ulong`** — 64-bit, WotLK
- **Logging: `[HH:mm:ss.fff]`** format via `Logging.Write()` (Styx/Helpers/Logging.cs)
- **Deobfuscate always** — `smethod_0` → descriptive name, `field_0` → `_meaningfulName`, no raw goto
- **Never invent code** — must exist in HB 3.3.5a, 4.3.4, or 6.2.3 reference
- **One file per commit**, descriptive messages

## Data Files (datadb/)

Python scripts manage the SQLite DBs:
- `check_db_integrity.py` — validates spawn/loot DB integrity (points at `bin/Debug/` output)
- `check_spawns.py`, `rebuild_spawns.py` — spawn DB utilities

DBs copied to output on build: `CreatureSpawns.db`, `item_loot.db`.

## VS Code XML Profile Support

`.vscode/settings.json` configures XML validation and auto-close against `Tools/ProfileGeneratorV2/schemas/HBProfile.xsd` (schema not present in repo).

## Testing

No automated tests. Manual in-game testing only.