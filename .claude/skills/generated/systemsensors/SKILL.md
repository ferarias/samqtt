---
name: systemsensors
description: "Skill for the SystemSensors area of samqtt. 5 symbols across 4 files."
---

# SystemSensors

5 symbols | 4 files | Cohesion: 100%

## When to Use

- Working with code in `src/`
- Understanding how DriveMultiSensor, SystemMultiSensorBase, CollectAsync work
- Modifying systemsensors-related functionality

## Key Files

| File | Symbols |
|------|---------|
| `src/Samqtt.Common/SystemSensors/SystemSensor.cs` | CollectInternalAsync, CollectAsync |
| `src/Samqtt.SystemSensors/MultiSensors/DriveMultiSensor.cs` | DriveMultiSensor |
| `src/Samqtt.Common/SystemSensors/SystemMultiSensorBase.cs` | SystemMultiSensorBase |
| `src/Samqtt.Common/SystemSensors/ISystemMultiSensor.cs` | ISystemMultiSensor |

## Entry Points

Start here when exploring this area:

- **`DriveMultiSensor`** (Class) — `src/Samqtt.SystemSensors/MultiSensors/DriveMultiSensor.cs:2`
- **`SystemMultiSensorBase`** (Class) — `src/Samqtt.Common/SystemSensors/SystemMultiSensorBase.cs:4`
- **`CollectAsync`** (Method) — `src/Samqtt.Common/SystemSensors/SystemSensor.cs:16`
- **`ISystemMultiSensor`** (Interface) — `src/Samqtt.Common/SystemSensors/ISystemMultiSensor.cs:3`

## Key Symbols

| Symbol | Type | File | Line |
|--------|------|------|------|
| `DriveMultiSensor` | Class | `src/Samqtt.SystemSensors/MultiSensors/DriveMultiSensor.cs` | 2 |
| `SystemMultiSensorBase` | Class | `src/Samqtt.Common/SystemSensors/SystemMultiSensorBase.cs` | 4 |
| `ISystemMultiSensor` | Interface | `src/Samqtt.Common/SystemSensors/ISystemMultiSensor.cs` | 3 |
| `CollectAsync` | Method | `src/Samqtt.Common/SystemSensors/SystemSensor.cs` | 16 |
| `CollectInternalAsync` | Method | `src/Samqtt.Common/SystemSensors/SystemSensor.cs` | 11 |

## How to Explore

1. `gitnexus_context({name: "DriveMultiSensor"})` — see callers and callees
2. `gitnexus_query({query: "systemsensors"})` — find related execution flows
3. Read key files listed above for implementation details
