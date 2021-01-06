# Modules

Client-side runtime patches to alter the client's behaviour to match EFT (Live)

**Project**        | **Function**
------------------ | --------------------------------------------
Aki.Common         | Common utilities used across projects
Aki.Core           | Required patches to start the game
Aki.SinglePlayer   | Simulating online game offline
Aki.CustomBundles  | Loading custom assets into the game
Aki.Tools          | Debugging utilities

## Requirements

- Escape From Tarkov 0.12.9.10519
- Visual Studio Build Tools (.NET desktop workload)
- .NET Framework 4.6.1

## Build

1. VSCodium > File > Open Workspace > Modules.code-workspace
2. VSCodium > Terminal > Run Build Task...
3. Copy-paste content inside `Build` into `%gamedir%`, overwrite when prompted.

## Authors

- InNoHurryToCode
- Craink
- PoloYolo
- Ginja
- waffle.lord
- stx09
- KruncyBite
- BALIST0N
