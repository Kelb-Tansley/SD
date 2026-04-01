# ⚙️ Aurestruct — *Design Faster. Verify Smarter. Build Safer.*

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
  - `SD.UI.Main` — shell regions and main interaction views
  - `SD.UI.Tools` — specialist engineering tool panels
  - `SD.UI.UltimateLimitState` — ULS-focused views and view models
  - Shared styling/resources in `SD.UI`

### `Tests` projects
- Unit and SpecFlow-driven behavior tests across domains and integrations

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
- **SpecFlow + test projects** for behavior and integration validation

---

## Conservative cleanup completed

To avoid breaking behavior, cleanup was limited to code that was demonstrably unused by solution references.

- Removed unused file: `SD/Constants/OurUsers.cs`

No functional behavior was changed.

---

## Notes

The project appears to be under active evolution (UI modules, code-specific tooling, and integration-heavy services). For larger dead-code elimination, the safest approach is staged cleanup backed by strict test coverage and feature-level validation.
