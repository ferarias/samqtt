---
name: actions
description: "Skill for the Actions area of samqtt. 10 symbols across 10 files."
---

# Actions

10 symbols | 10 files | Cohesion: 100%

## When to Use

- Working with code in `src/`
- Understanding how SuspendAction, ShutdownAction, RebootAction work
- Modifying actions-related functionality

## Key Files

| File | Symbols |
|------|---------|
| `src/Samqtt.SystemActions.Windows/Actions/SuspendAction.cs` | SuspendAction |
| `src/Samqtt.SystemActions.Windows/Actions/ShutdownAction.cs` | ShutdownAction |
| `src/Samqtt.SystemActions.Windows/Actions/RebootAction.cs` | RebootAction |
| `src/Samqtt.SystemActions.Windows/Actions/HibernateAction.cs` | HibernateAction |
| `src/Samqtt.SystemActions/Actions/StartProcessAction.cs` | StartProcessAction |
| `src/Samqtt.SystemActions/Actions/KillProcessAction.cs` | KillProcessAction |
| `src/Samqtt.SystemActions/Actions/GetProcessesAction.cs` | GetProcessesAction |
| `src/Samqtt.SystemActions/Actions/GetProcessAction.cs` | GetProcessAction |
| `src/Samqtt.Common/SystemActions/SystemAction.cs` | SystemAction |
| `src/Samqtt.Common/SystemActions/ISystemAction.cs` | ISystemAction |

## Entry Points

Start here when exploring this area:

- **`SuspendAction`** (Class) — `src/Samqtt.SystemActions.Windows/Actions/SuspendAction.cs:2`
- **`ShutdownAction`** (Class) — `src/Samqtt.SystemActions.Windows/Actions/ShutdownAction.cs:2`
- **`RebootAction`** (Class) — `src/Samqtt.SystemActions.Windows/Actions/RebootAction.cs:2`
- **`HibernateAction`** (Class) — `src/Samqtt.SystemActions.Windows/Actions/HibernateAction.cs:2`
- **`StartProcessAction`** (Class) — `src/Samqtt.SystemActions/Actions/StartProcessAction.cs:5`

## Key Symbols

| Symbol | Type | File | Line |
|--------|------|------|------|
| `SuspendAction` | Class | `src/Samqtt.SystemActions.Windows/Actions/SuspendAction.cs` | 2 |
| `ShutdownAction` | Class | `src/Samqtt.SystemActions.Windows/Actions/ShutdownAction.cs` | 2 |
| `RebootAction` | Class | `src/Samqtt.SystemActions.Windows/Actions/RebootAction.cs` | 2 |
| `HibernateAction` | Class | `src/Samqtt.SystemActions.Windows/Actions/HibernateAction.cs` | 2 |
| `StartProcessAction` | Class | `src/Samqtt.SystemActions/Actions/StartProcessAction.cs` | 5 |
| `KillProcessAction` | Class | `src/Samqtt.SystemActions/Actions/KillProcessAction.cs` | 4 |
| `GetProcessesAction` | Class | `src/Samqtt.SystemActions/Actions/GetProcessesAction.cs` | 4 |
| `GetProcessAction` | Class | `src/Samqtt.SystemActions/Actions/GetProcessAction.cs` | 4 |
| `SystemAction` | Class | `src/Samqtt.Common/SystemActions/SystemAction.cs` | 5 |
| `ISystemAction` | Interface | `src/Samqtt.Common/SystemActions/ISystemAction.cs` | 5 |

## How to Explore

1. `gitnexus_context({name: "SuspendAction"})` — see callers and callees
2. `gitnexus_query({query: "actions"})` — find related execution flows
3. Read key files listed above for implementation details
