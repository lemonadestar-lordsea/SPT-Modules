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

- Escape From Tarkov 0.12.12.15.16909
- Visual Studio Code
- Dotnet 6 SDK

## Setup

Copy-paste your EFT's `EscapeFromTarkov_Data/Managed/` folder to Modules' `Project/Shared/` folder

## Build

### Visual Studio Code

1. File > Open Workspace > Modules.code-workspace
2. Terminal > Run Build Task...
3. Copy-paste content inside `Build` into `%gamedir%`, overwrite when prompted.

### Visual Studio

1. Set solution configuration to "Release"
2. Build > Rebuild Solution
3. Copy-paste content inside `Build` into `%gamedir%`, overwrite when prompted.

## Authors

- Senko-san
- Craink
- PoloYolo
- Ginja
- waffle.lord
- stx09
- KruncyBite
- BALIST0N
