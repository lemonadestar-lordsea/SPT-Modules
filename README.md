# Modules

Client-side modifications to alter Escape From Tarkov's behaviour

**Project**        | **Function**
------------------ | --------------------------------------------
Aki.Loader         | Loads custom assemblies into the game
Aki.Common         | Common utilities used across projects
Aki.Reflection     | Reflection utilities used across the project
Aki.Core           | Required patches to start the game
Aki.Bundles        | External bundle loader
Aki.SinglePlayer   | Simulating online game offline
Aki.Custom         | SPT-AKI enhancements to EFT
Aki.Debugging      | Debug utilities
Aki.Build          | Build script

## Requirements

- Escape From Tarkov 0.12.12.0.16440
- Visual Studio Build Tools (.NET desktop workload)
- .NET Framework 4.7.2
- VSCodium

## Build

1. Copy your tarkov DLLs from live\EscapeFromTarkov_Data\Managed to Modules\project\Shared
2. Deobfuscate and remap your live\EscapeFromTarkov_Data\Managed\Assembly-CSharp.dll, copy to Modules\project\Shared
3. VSCodium > File > Open Workspace > Modules.code-workspace
4. VSCodium > Terminal > Run Build Task...
5. Copy-paste content inside `Build` into `%gamedir%`, overwrite when prompted.

## Authors

- Senko-san
- Craink
- PoloYolo
- Ginja
- waffle.lord
- stx09
- KruncyBite
- BALIST0N
