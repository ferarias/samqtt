---
name: actions
description: "Skill for the Actions area of samqtt. 15 symbols across 15 files."
---

# Actions

15 symbols | 15 files | Cohesion: 100%

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
| `src/Samqtt.SystemActions/Actions/SuspendAction.cs` | SuspendAction |
| `src/Samqtt.SystemActions/Actions/StartProcessAction.cs` | StartProcessAction |
| `src/Samqtt.SystemActions/Actions/ShutdownAction.cs` | ShutdownAction |
| `src/Samqtt.SystemActions/Actions/SendNotificationAction.cs` | SendNotificationAction |
| `src/Samqtt.SystemActions/Actions/RebootAction.cs` | RebootAction |
| `src/Samqtt.SystemActions/Actions/KillProcessAction.cs` | KillProcessAction |

## Entry Points

Start here when exploring this area:

- **`SuspendAction`** (Class) — `src/Samqtt.SystemActions.Windows/Actions/SuspendAction.cs:2`
- **`ShutdownAction`** (Class) — `src/Samqtt.SystemActions.Windows/Actions/ShutdownAction.cs:2`
- **`RebootAction`** (Class) — `src/Samqtt.SystemActions.Windows/Actions/RebootAction.cs:2`
- **`HibernateAction`** (Class) — `src/Samqtt.SystemActions.Windows/Actions/HibernateAction.cs:2`
- **`SuspendAction`** (Class) — `src/Samqtt.SystemActions/Actions/SuspendAction.cs:4`

## Key Symbols

| Symbol | Type | File | Line |
|--------|------|------|------|
| `SuspendAction` | Class | `src/Samqtt.SystemActions.Windows/Actions/SuspendAction.cs` | 2 |
| `ShutdownAction` | Class | `src/Samqtt.SystemActions.Windows/Actions/ShutdownAction.cs` | 2 |
| `RebootAction` | Class | `src/Samqtt.SystemActions.Windows/Actions/RebootAction.cs` | 2 |
| `HibernateAction` | Class | `src/Samqtt.SystemActions.Windows/Actions/HibernateAction.cs` | 2 |
| `SuspendAction` | Class | `src/Samqtt.SystemActions/Actions/SuspendAction.cs` | 4 |
| `StartProcessAction` | Class | `src/Samqtt.SystemActions/Actions/StartProcessAction.cs` | 5 |
| `ShutdownAction` | Class | `src/Samqtt.SystemActions/Actions/ShutdownAction.cs` | 4 |
| `SendNotificationAction` | Class | `src/Samqtt.SystemActions/Actions/SendNotificationAction.cs` | 5 |
| `RebootAction` | Class | `src/Samqtt.SystemActions/Actions/RebootAction.cs` | 4 |
| `KillProcessAction` | Class | `src/Samqtt.SystemActions/Actions/KillProcessAction.cs` | 4 |
| `HibernateAction` | Class | `src/Samqtt.SystemActions/Actions/HibernateAction.cs` | 4 |
| `GetProcessesAction` | Class | `src/Samqtt.SystemActions/Actions/GetProcessesAction.cs` | 4 |
| `GetProcessAction` | Class | `src/Samqtt.SystemActions/Actions/GetProcessAction.cs` | 4 |
| `SystemAction` | Class | `src/Samqtt.Common/SystemActions/SystemAction.cs` | 5 |
| `ISystemAction` | Interface | `src/Samqtt.Common/SystemActions/ISystemAction.cs` | 5 |

## How to Explore

1. `gitnexus_context({name: "SuspendAction"})` — see callers and callees
2. `gitnexus_query({query: "actions"})` — find related execution flows
3. Read key files listed above for implementation details
