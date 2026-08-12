# âš™ï¸ Aurestruct â€” *Design Faster. Verify Smarter. Build Safer.*

**Aurestruct Structural Design Tools** is a modular desktop engineering platform for structural design workflows, combining FEM model interaction, code-based design checks, and report-friendly calculation pipelines in one environment.

---

## What this application does

Aurestruct is built to support practical structural engineering design by connecting:

- **FEM model access and extraction** (via Strand7 integration)
- **Design-code checks** (including SANS and AS workflows)
- **Ultimate Limit State (ULS) result review**
- **Mathcad-based calculation interoperability**
- **A WPF desktop UI** with modular Prism regions and tool panels

At runtime, the app authenticates the user, loads persisted runtime settings, and presents a shell UI where users can:

1. Open and interact with structural model data
2. Review beam and element properties
3. Generate and inspect design combinations and utilization outputs
4. Work with specialist tools (e.g., wind loading, buckling analysis, tank design)
5. Export or interoperate with external calculation workflows (Mathcad)

---

## Solution overview

The solution is organized into clear layers/projects:

### `SD` (main desktop host)
- WPF + Prism application bootstrapper
- Dependency injection/container registration
- App lifecycle (startup, splash, shutdown, restart)
- Authentication and token handling
- High-level shell and navigation composition

### `Core` projects
- Shared contracts, models, events, and infrastructure abstractions
- Logging and foundational cross-cutting services

### `Data` project
- Data access services, repository/unit-of-work patterns, and mappers
- User/runtime preference persistence support

### `Domain` projects
- Engineering logic and domain services:
  - Design engines
  - Strand7 integration services
  - Mathcad automation/interoperability
  - Code-specific design services (SANS / AS)

### `UserInterface` projects
- Modular WPF UI components split by functional area:
  - `SD.UI.Main` â€” shell regions and main interaction views
  - `SD.UI.Tools` â€” specialist engineering tool panels
  - `SD.UI.UltimateLimitState` â€” ULS-focused views and view models
  - Shared styling/resources in `SD.UI`

### `Tests` projects
- Unit and Reqnroll-driven behavior tests across domains and integrations

---

## Key runtime flow (high-level)

1. App starts and displays splash screen
2. Core services, domain services, settings, and models are registered
3. User authentication and license validation run
4. Shell regions are composed with modular views
5. FEM/design workflows become available in-browser and design panes
6. On exit/restart, runtime settings and connected resources are safely handled

---

## Technology stack

- **.NET 8**
- **WPF** desktop UI
- **Prism** for modular composition and regions
- **CommunityToolkit.Mvvm** for MVVM patterns and commands
- **Microsoft Identity Client (MSAL)** for authentication
- **Reqnroll + test projects** for behavior and integration validation

---

## Conservative cleanup completed

To avoid breaking behavior, cleanup was limited to code that was demonstrably unused by solution references.

- Removed unused file: `SD/Constants/OurUsers.cs`

No functional behavior was changed.

---

## Notes

The project appears to be under active evolution (UI modules, code-specific tooling, and integration-heavy services). For larger dead-code elimination, the safest approach is staged cleanup backed by strict test coverage and feature-level validation.

---

## Installer command cheat sheet

Run all commands from the repository root (`D:\mine\SD`).

### 0) Preferred: run the single helper script

The script automatically names the output EXE as `AurestructSetup<version>.exe` based on the version you provide.

```powershell
pwsh -File infra/scripts/build-installer.ps1 -MsiVersion 1.2.3
```

Optional parameters:

```powershell
# Set the version (controls both MSI version and bundle/EXE name)
pwsh -File infra/scripts/build-installer.ps1 -MsiVersion 1.2.3

# Override just the bundle/EXE version (MSI version remains as in SD.WiX.wixproj)
pwsh -File infra/scripts/build-installer.ps1 -BundleVersion 1.2.3

# Custom publish directory
pwsh -File infra/scripts/build-installer.ps1 -PublishDir "D:\build\publish\"

# Skip tool restore
pwsh -File infra/scripts/build-installer.ps1 -SkipToolRestore

# Combine options
pwsh -File infra/scripts/build-installer.ps1 -MsiVersion 2.0.0 -SkipClean
```

**Version behavior:**
- If no `-MsiVersion` is provided, the script uses the version hardcoded in `Installer/SD.WiX/SD.WiX.wixproj` (currently `1.0.0`).
- If no `-BundleVersion` is provided, the script uses the same version as the MSI.
- Both the EXE filename and the installer UI version are automatically synchronized.

### 1) Publish the desktop app (required before MSI build)

```powershell
dotnet publish SD/SD.csproj -c Release -r win-x64 --self-contained true
```

### 2) Build MSI (WiX package project)

```powershell
dotnet build Installer/SD.WiX/SD.WiX.wixproj -c Release
```

### 3) Build bootstrapper EXE (WiX bundle project)

```powershell
dotnet build Installer/SD.Bundle/SD.Bundle.wixproj -c Release
```

### 4) Rebuild everything in one go

```powershell
dotnet publish SD/SD.csproj -c Release -r win-x64 --self-contained true
dotnet build Installer/SD.WiX/SD.WiX.wixproj -c Release
dotnet build Installer/SD.Bundle/SD.Bundle.wixproj -c Release
```

### 5) Override publish path if needed (advanced)

```powershell
dotnet build Installer/SD.WiX/SD.WiX.wixproj -c Release -p:AppPublishDir="D:\custom\publish\path\"
```

### 6) Override MSI path when building bundle (advanced)

```powershell
dotnet build Installer/SD.Bundle/SD.Bundle.wixproj -c Release -p:MsiPath="D:\custom\Aurestruct.msi"
```

### 7) Outputs

```text
Installer/SD.WiX/bin/Release/Aurestruct.msi
Installer/SD.Bundle/bin/Release/AurestructSetup*.exe
```

### 8) Verify install/uninstall manually (silent)

```powershell
msiexec /i Installer/SD.WiX/bin/Release/Aurestruct.msi /qn /l*v install.log
msiexec /x Installer/SD.WiX/bin/Release/Aurestruct.msi /qn /l*v uninstall.log
```

### 9) If MSI builds but contains no app files

This means the publish folder was missing when harvesting files.
Run Step 1 first, then rebuild Steps 2 and 3.

