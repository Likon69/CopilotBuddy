# CopilotBuddy → x64 World of Warcraft: Audit & Migration Plan

Status: **foundation layer landed** (see "What is done" below). Everything
else is planned but not started.

## 1. Why this is not "just flip a build flag"

CopilotBuddy targets WoW **3.3.5a build 12340, x86** at every layer. The
`<PlatformTarget>x86</PlatformTarget>` in the csproj is the smallest part.
A 64-bit WoW process breaks the bot in seven independent places:

| # | Layer | x86 assumption | What breaks on a 64-bit target |
|---|-------|----------------|-------------------------------|
| 1 | `GreenMagic/Native/Imports.cs` | `ReadProcessMemory(IntPtr, uint lpBaseAddress, …)`, `VirtualAllocEx → uint` | Any address above 4 GB cannot even be represented. A 32-bit bot process can also receive truncated pointers from a 64-bit target. |
| 2 | `GreenMagic/Memory.cs` | `_imageBase` is `uint` (default `0x400000`); ~400 `uint`-addressed signatures across ~38 files; pointer chains read 4-byte pointers (`ReadInternal<uint>`) | All pointer reads return garbage; `GetAbsolute` wrong; cache keys collide. |
| 3 | `GreenMagic/Native/Context.cs` | i386 `CONTEXT` (Eip/Eax/…, `FLOATING_SAVE_AREA`, 512-byte `ExtendedRegisters`), `CONTEXT_i386 = 0x10000` | `Get/SetThreadContext` fails outright against a 64-bit thread — Windows requires the 1232-byte AMD64 `CONTEXT` with `CONTEXT_AMD64 = 0x100000`. |
| 4 | `Executor.cs` / `ExecutorRand.cs` | Hook `d3d9.dll!EndScene`; emit `pushad`/`popad`/`pushfd` (**illegal in x64 long mode**); stdcall `push arg… / call / add esp,N`; thiscall `mov ecx, this`; EIP hijack; **440 hand-written asm lines in 20 files** | Every byte is wrong for x64: the instructions are undefined, the calling convention is RCX/RDX/R8/R9 + 32-byte shadow space + 16-byte stack alignment, pointers are 8 bytes, and a 64-bit client renders with DX11/DX12/Vulkan — there is no d3d9 to hook. |
| 5 | `Styx/Offsets/GlobalOffsets.cs`, `Styx/Patchables/GlobalOffsets.cs` | ~122 hardcoded absolute addresses for build 12340 (`InGame = 0xBD0792`, `LuaState = 0xD3F78C`, ~90 `FrameScript_*`/`CG*` functions) plus struct offsets that assume **4-byte pointers** (`NextObjectOffset = 0x3C`, `DescriptorMultiplier = 0x4`, `NameCacheNextOffset = 0xC`, `ObjectGuidOffset = 0x30`, …) | Every address is wrong for any other build/architecture, and every pointer-bearing struct doubles in size on x64 (all `+4` strides after the first pointer shift). |
| 6 | `Lib/*.dll` | `Navigation.dll`, `Navigation 1x1.dll`, `BlackMagic.dll`, `fasmdll_managed.dll`, `System.Data.SQLite.dll`, `RecastManaged.dll`, `Tripper.Tools.dll`, `Tripper.XNAMath.dll` are all **PE32/x86** (verified from PE headers) | A 64-bit CopilotBuddy cannot load any of them. x64 rebuilds (or replacements) are mandatory. |
| 7 | Runtime content | `data.bin`, `Spells.bin`, `datadb/`, `mmaps/` are extracted from 3.3.5a MPQ/DBC | Only valid if the x64 target still speaks 3.3.5a game data. |

## 2. Which x64 client?

Blizzard never shipped a 64-bit 3.3.5a client (first x64 client: patch
4.3.2, 2012; 32-bit retired at 8.0.1, 2018). A "64-bit WoW that plays WotLK
3.3.5a content" therefore means one of:

| Target | Reality | Effort for CopilotBuddy |
|--------|---------|-------------------------|
| **WoWee** — [github.com/Kelsidavis/WoWee](https://github.com/Kelsidavis/WoWee) | Open-source C++20/SDL3/Vulkan reimplementation; x86-64 + ARM64; supports Vanilla/TBC/WotLK 3.3.5a; tested vs TrinityCore/AzerothCore/MaNGOS; very active | **Best option.** Source is available → struct layouts and call sites can be read directly, and a small in-process bridge (named pipe / shared memory export layer) can replace memory scraping entirely. No offsets to RE, no FASM needed. |
| **whoa** — [github.com/whoahq/whoa](https://github.com/whoahq/whoa) | Open-source effort rebuilding the 3.3.5a client | Same approach as WoWee; maturity varies. |
| Custom/private x64 3.3.5a build | Some server teams ship rebuilt clients | Needs the binary; offsets must be RE'd per build. Full faithful-port path below. |
| Wrath Classic / modern retail 64-bit | 128-bit GUIDs, CASC/DB2, Warden, DX11/12 | Effectively a **new bot**, not a port. Out of scope for this codebase. |

The foundation work in §4 is required by **all** paths; nothing here is
client-specific.

## 3. Work packages (ordered)

- **WP0 — Foundation (done, see §4).** Bitness-agnostic memory layer,
  x64 CONTEXT, PE32+ parsing, dual-platform project.
- **WP1 — Target client decision + binary acquisition.** Pick the client,
  get it running, capture a reproducible attach scenario. Everything below
  depends on this.
- **WP2 — Object model abstraction.** `ObjectManager`, `WoWObject` and
  friends read through `uint` chains with 4-byte strides. Introduce a
  small adapter (`IProcessMemory` with `ReadPointer`/pointer-size-aware
  chain walks) so the same Styx code can run against either target
  bitness. Affected: ~38 files.
- **WP3 — Offset layer.** Split `Styx/Offsets/GlobalOffsets.cs` +
  `Styx/Patchables/GlobalOffsets.cs` into per-target offset tables
  (e.g. `Offsets335x86`, `Offsets<Target>x64`). Struct offsets that encode
  pointer size (`NextObjectOffset`, `NameCacheNextOffset`,
  `DescriptorMultiplier`…) become computed values. For WoWee/whoa these
  come from reading their source; for a closed binary they must be
  reverse-engineered per build.
- **WP4 — Execution engine.** `Executor`/`ExecutorRand` need an x64 twin:
  no `pushad`/`popfd`, x64 fastcall ABI, 8-byte pointers, RIP hijack with
  the 1232-byte CONTEXT, and a new hook point (DX11/DX12 `Present`, Vulkan
  submit, or — for open-source targets — a cooperative callback instead of
  a hook at all). The 440 inline x86 asm lines in 20 files each need an
  x64 variant or (preferably) replacement by a cooperative bridge.
- **WP5 — Native libraries.** x64 builds of `Navigation.dll` (Detour
  runtime — rebuild from the Navigation-C- sources), `fasmdll_managed.dll`
  (or drop FASM and emit machine code directly), `System.Data.SQLite.dll`
  (official x64 build exists), `Tripper.Tools/XNAMath` (pure C++ wrappers,
  rebuild), `BlackMagic.dll` (audit — may be unused).
- **WP6 — Data layer.** Re-extract/convert `data.bin`, `Spells.bin`,
  `datadb/`, mmaps if the target's data formats differ from 3.3.5a MPQ/DBC.
- **WP7 — Attach/UI flow.** Process picker (`Styx/CommonBot` +
  `UI/`) currently filters for the classic client; extend for x64,
  including `IsWow64Process` sanity checks and clearer error messages.
- **WP8 — Verification.** No Windows/WoW in the current dev sandbox: the
  x64 platform has **not been compiled or run** anywhere yet. First
  milestone is `dotnet build -p:Platform=x64` on Windows, then attach to
  the chosen client.

## 4. What is done (this branch)

All additive — the classic x86/3.3.5a path is byte-for-byte untouched:

- `CopilotBuddy.csproj` / `CopilotBuddy.slnx` — new **x64 platform**;
  `PlatformTarget` is now conditional (x86 stays the default).
- `GreenMagic/Native/Context64.cs` — the 1232-byte AMD64 `CONTEXT`
  (`Context64`), `M128A`, and `ContextFlags64` (`CONTEXT_AMD64 | …`).
- `GreenMagic/Native/MEMORY_BASIC_INFORMATION64.cs` — 64-bit
  `MEMORY_BASIC_INFORMATION` (required whenever a 64-bit bot process calls
  `VirtualQueryEx`).
- `GreenMagic/Native/Imports.cs` — 64-bit-capable P/Invokes
  (`ReadProcessMemory64`, `WriteProcessMemory64`, `VirtualAllocEx64`,
  `VirtualFreeEx64`, `VirtualProtectEx64`, `VirtualQueryEx64`,
  `Get/SetThreadContext64`, `IsWow64Process`), all pointing at the real
  kernel32 exports via `EntryPoint`.
- `GreenMagic/Native/PeHeaderParser.cs` — PE32+ support
  (`ImageNtHeader64`/`ImageOptionalHeader64`, `Is64BitImage`,
  `NtHeader64`); parsing no longer crashes on modules above 4 GB.
- `GreenMagic/Memory.cs` —
  auto-detects a 64-bit target from the PE machine field
  (`Is64BitTarget`), full 64-bit image base (`ImageBase64`,
  `GetAbsolute64`), and a complete ulong-addressed API:
  `ReadBytes64`, `Read64<T>`, `ReadPointer64`, `ReadChain64<T>`,
  `ReadChainRelative64<T>`, `ReadStruct64<T>`, `ReadString64`,
  `WriteBytes64`, `Write64<T>`, `WritePointer64`, `WriteObject64`,
  `AllocateMemory64`, `FreeMemory64`, `QueryMemory64`,
  `Get/SetThreadContext64`, `CreateRemoteThread64`.
  `ReadChain64` walks 8-byte pointer chains exactly like the existing
  `Read<T>(params uint[])` walks 4-byte ones.

## 5. Known gaps / risks

- **Nothing here has been compiled or run** — no .NET SDK or Windows in
  this environment. Treat WP8 as the first task.
- Native Lib DLLs are still x86; the x64 platform will fail at load time
  for `fasmdll_managed`/`System.Data.SQLite` until WP5 lands.
- `PatternManager` (AOB scanning) still assumes 32-bit modules
  (`BaseOfCode`/`BaseOfData` PE32 fields); it needs a PE32+ variant for
  x64 targets.
- `ExecutorRand`'s SEH/VEH gate and `Styx/Offsets` struct strides are the
  two hardest WP4 items — plan real time for them.
- An x64 WoW above 4 GB also invalidates the cached `uint` addresses in
  any plugin/botbase that bypasses `Memory` — audit `Bots/` during WP2.
