---
name: sensors
description: "Skill for the Sensors area of samqtt. 12 symbols across 9 files."
---

# Sensors

12 symbols | 9 files | Cohesion: 100%

## When to Use

- Working with code in `src/`
- Understanding how FreeMemorySensor, CpuProcessorTimeSensor, TimestampSensor work
- Modifying sensors-related functionality

## Key Files

| File | Symbols |
|------|---------|
| `src/Samqtt.SystemSensors.Windows/Sensors/FreeMemorySensor.cs` | FreeMemorySensor, CollectInternalAsync, GetFreeMemoryAsync, GlobalMemoryStatusEx |
| `src/Samqtt.SystemSensors.Windows/Sensors/CpuProcessorTimeSensor.cs` | CpuProcessorTimeSensor |
| `src/Samqtt.SystemSensors/Sensors/TimestampSensor.cs` | TimestampSensor |
| `src/Samqtt.SystemSensors/Sensors/NetworkAvailabilitySensor.cs` | NetworkAvailabilitySensor |
| `src/Samqtt.Common/SystemSensors/SystemSensor.cs` | SystemSensor |
| `src/Samqtt.Common/SystemSensors/ISystemSensor.cs` | ISystemSensor |
| `src/Samqtt.SystemSensors/MultiSensors/Drive/DriveTotalSizeSensor.cs` | DriveTotalSizeSensor |
| `src/Samqtt.SystemSensors/MultiSensors/Drive/DrivePercentFreeSizeSensor.cs` | DrivePercentFreeSizeSensor |
| `src/Samqtt.SystemSensors/MultiSensors/Drive/DriveFreeSizeSensor.cs` | DriveFreeSizeSensor |

## Entry Points

Start here when exploring this area:

- **`FreeMemorySensor`** (Class) — `src/Samqtt.SystemSensors.Windows/Sensors/FreeMemorySensor.cs:5`
- **`CpuProcessorTimeSensor`** (Class) — `src/Samqtt.SystemSensors.Windows/Sensors/CpuProcessorTimeSensor.cs:5`
- **`TimestampSensor`** (Class) — `src/Samqtt.SystemSensors/Sensors/TimestampSensor.cs:4`
- **`NetworkAvailabilitySensor`** (Class) — `src/Samqtt.SystemSensors/Sensors/NetworkAvailabilitySensor.cs:5`
- **`SystemSensor`** (Class) — `src/Samqtt.Common/SystemSensors/SystemSensor.cs:4`

## Key Symbols

| Symbol | Type | File | Line |
|--------|------|------|------|
| `FreeMemorySensor` | Class | `src/Samqtt.SystemSensors.Windows/Sensors/FreeMemorySensor.cs` | 5 |
| `CpuProcessorTimeSensor` | Class | `src/Samqtt.SystemSensors.Windows/Sensors/CpuProcessorTimeSensor.cs` | 5 |
| `TimestampSensor` | Class | `src/Samqtt.SystemSensors/Sensors/TimestampSensor.cs` | 4 |
| `NetworkAvailabilitySensor` | Class | `src/Samqtt.SystemSensors/Sensors/NetworkAvailabilitySensor.cs` | 5 |
| `SystemSensor` | Class | `src/Samqtt.Common/SystemSensors/SystemSensor.cs` | 4 |
| `DriveTotalSizeSensor` | Class | `src/Samqtt.SystemSensors/MultiSensors/Drive/DriveTotalSizeSensor.cs` | 4 |
| `DrivePercentFreeSizeSensor` | Class | `src/Samqtt.SystemSensors/MultiSensors/Drive/DrivePercentFreeSizeSensor.cs` | 4 |
| `DriveFreeSizeSensor` | Class | `src/Samqtt.SystemSensors/MultiSensors/Drive/DriveFreeSizeSensor.cs` | 4 |
| `ISystemSensor` | Interface | `src/Samqtt.Common/SystemSensors/ISystemSensor.cs` | 4 |
| `CollectInternalAsync` | Method | `src/Samqtt.SystemSensors.Windows/Sensors/FreeMemorySensor.cs` | 8 |
| `GetFreeMemoryAsync` | Method | `src/Samqtt.SystemSensors.Windows/Sensors/FreeMemorySensor.cs` | 16 |
| `GlobalMemoryStatusEx` | Method | `src/Samqtt.SystemSensors.Windows/Sensors/FreeMemorySensor.cs` | 43 |

## Execution Flows

| Flow | Type | Steps |
|------|------|-------|
| `CollectInternalAsync → MEMORYSTATUSEX` | intra_community | 3 |
| `CollectInternalAsync → GlobalMemoryStatusEx` | intra_community | 3 |

## How to Explore

1. `gitnexus_context({name: "FreeMemorySensor"})` — see callers and callees
2. `gitnexus_query({query: "sensors"})` — find related execution flows
3. Read key files listed above for implementation details
