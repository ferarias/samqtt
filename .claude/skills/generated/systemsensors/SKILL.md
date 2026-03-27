---
name: systemsensors
description: "Skill for the SystemSensors area of samqtt. 9 symbols across 8 files."
---

# SystemSensors

9 symbols | 8 files | Cohesion: 100%

## When to Use

- Working with code in `src/`
- Understanding how DriveMultiSensor, SystemMultiSensorBase, FormatObject work
- Modifying systemsensors-related functionality

## Key Files

| File | Symbols |
|------|---------|
| `src/Samqtt.Common/SystemSensors/SystemSensor.cs` | CollectInternalAsync, CollectAsync |
| `src/Samqtt.Common/IMessagePublisher.cs` | PublishSensorValue |
| `src/Samqtt.Application/Win2MqttBackgroundService.cs` | ExecuteAsync |
| `src/Samqtt.Common/SystemSensors/ISystemSensorValueFormatter.cs` | FormatObject |
| `src/Samqtt.Common/SystemSensors/ISystemSensor.cs` | CollectAsync |
| `src/Samqtt.SystemSensors/MultiSensors/DriveMultiSensor.cs` | DriveMultiSensor |
| `src/Samqtt.Common/SystemSensors/SystemMultiSensorBase.cs` | SystemMultiSensorBase |
| `src/Samqtt.Common/SystemSensors/ISystemMultiSensor.cs` | ISystemMultiSensor |

## Entry Points

Start here when exploring this area:

- **`DriveMultiSensor`** (Class) — `src/Samqtt.SystemSensors/MultiSensors/DriveMultiSensor.cs:2`
- **`SystemMultiSensorBase`** (Class) — `src/Samqtt.Common/SystemSensors/SystemMultiSensorBase.cs:4`
- **`FormatObject`** (Method) — `src/Samqtt.Common/SystemSensors/ISystemSensorValueFormatter.cs:11`
- **`CollectAsync`** (Method) — `src/Samqtt.Common/SystemSensors/SystemSensor.cs:31`
- **`ISystemMultiSensor`** (Interface) — `src/Samqtt.Common/SystemSensors/ISystemMultiSensor.cs:3`

## Key Symbols

| Symbol | Type | File | Line |
|--------|------|------|------|
| `DriveMultiSensor` | Class | `src/Samqtt.SystemSensors/MultiSensors/DriveMultiSensor.cs` | 2 |
| `SystemMultiSensorBase` | Class | `src/Samqtt.Common/SystemSensors/SystemMultiSensorBase.cs` | 4 |
| `ISystemMultiSensor` | Interface | `src/Samqtt.Common/SystemSensors/ISystemMultiSensor.cs` | 3 |
| `FormatObject` | Method | `src/Samqtt.Common/SystemSensors/ISystemSensorValueFormatter.cs` | 11 |
| `CollectAsync` | Method | `src/Samqtt.Common/SystemSensors/SystemSensor.cs` | 31 |
| `PublishSensorValue` | Method | `src/Samqtt.Common/IMessagePublisher.cs` | 17 |
| `ExecuteAsync` | Method | `src/Samqtt.Application/Win2MqttBackgroundService.cs` | 54 |
| `CollectAsync` | Method | `src/Samqtt.Common/SystemSensors/ISystemSensor.cs` | 24 |
| `CollectInternalAsync` | Method | `src/Samqtt.Common/SystemSensors/SystemSensor.cs` | 26 |

## How to Explore

1. `gitnexus_context({name: "DriveMultiSensor"})` — see callers and callees
2. `gitnexus_query({query: "systemsensors"})` — find related execution flows
3. Read key files listed above for implementation details
