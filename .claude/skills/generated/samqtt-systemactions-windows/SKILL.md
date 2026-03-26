---
name: samqtt-systemactions-windows
description: "Skill for the Samqtt.SystemActions.Windows area of samqtt. 9 symbols across 5 files."
---

# Samqtt.SystemActions.Windows

9 symbols | 5 files | Cohesion: 100%

## When to Use

- Working with code in `src/`
- Understanding how HibernateSystem, SuspendSystem, HandleCoreAsync work
- Modifying samqtt.systemactions.windows-related functionality

## Key Files

| File | Symbols |
|------|---------|
| `src/Samqtt.SystemActions.Windows/WindowsPowerManagement.cs` | SetSuspendState, HibernateSystem, SuspendSystem, Shutdown, Reboot |
| `src/Samqtt.SystemActions.Windows/Actions/SuspendAction.cs` | HandleCoreAsync |
| `src/Samqtt.SystemActions.Windows/Actions/HibernateAction.cs` | HandleCoreAsync |
| `src/Samqtt.SystemActions.Windows/Actions/ShutdownAction.cs` | HandleCoreAsync |
| `src/Samqtt.SystemActions.Windows/Actions/RebootAction.cs` | HandleCoreAsync |

## Entry Points

Start here when exploring this area:

- **`HibernateSystem`** (Method) — `src/Samqtt.SystemActions.Windows/WindowsPowerManagement.cs:14`
- **`SuspendSystem`** (Method) — `src/Samqtt.SystemActions.Windows/WindowsPowerManagement.cs:15`
- **`HandleCoreAsync`** (Method) — `src/Samqtt.SystemActions.Windows/Actions/SuspendAction.cs:6`
- **`HandleCoreAsync`** (Method) — `src/Samqtt.SystemActions.Windows/Actions/HibernateAction.cs:6`
- **`Shutdown`** (Method) — `src/Samqtt.SystemActions.Windows/WindowsPowerManagement.cs:17`

## Key Symbols

| Symbol | Type | File | Line |
|--------|------|------|------|
| `HibernateSystem` | Method | `src/Samqtt.SystemActions.Windows/WindowsPowerManagement.cs` | 14 |
| `SuspendSystem` | Method | `src/Samqtt.SystemActions.Windows/WindowsPowerManagement.cs` | 15 |
| `HandleCoreAsync` | Method | `src/Samqtt.SystemActions.Windows/Actions/SuspendAction.cs` | 6 |
| `HandleCoreAsync` | Method | `src/Samqtt.SystemActions.Windows/Actions/HibernateAction.cs` | 6 |
| `Shutdown` | Method | `src/Samqtt.SystemActions.Windows/WindowsPowerManagement.cs` | 17 |
| `HandleCoreAsync` | Method | `src/Samqtt.SystemActions.Windows/Actions/ShutdownAction.cs` | 8 |
| `Reboot` | Method | `src/Samqtt.SystemActions.Windows/WindowsPowerManagement.cs` | 21 |
| `HandleCoreAsync` | Method | `src/Samqtt.SystemActions.Windows/Actions/RebootAction.cs` | 8 |
| `SetSuspendState` | Method | `src/Samqtt.SystemActions.Windows/WindowsPowerManagement.cs` | 7 |

## Execution Flows

| Flow | Type | Steps |
|------|------|-------|
| `HandleCoreAsync → SetSuspendState` | intra_community | 3 |
| `HandleCoreAsync → SetSuspendState` | intra_community | 3 |

## How to Explore

1. `gitnexus_context({name: "HibernateSystem"})` — see callers and callees
2. `gitnexus_query({query: "samqtt.systemactions.windows"})` — find related execution flows
3. Read key files listed above for implementation details
