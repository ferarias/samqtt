# System Sensors

Sensors are published to Home Assistant via MQTT Discovery when the MQTT integration is enabled.

State values are published to:
```
samqtt/system_sensor/{device}/{sensor_name}/state
```

where `{device}` is the `DeviceIdentifier` from configuration (defaults to machine hostname).

Enable or disable individual sensors in the `Sensors` or `MultiSensors` sections of `samqtt.appsettings.json`.

---

## Cross-Platform Sensors

### Timestamp

Current UTC timestamp.

| | |
|---|---|
| **ConfigKey** | `Timestamp` |
| **Topic** | `samqtt/system_sensor/{device}/timestamp/state` |
| **Value** | ISO 8601 UTC datetime string |
| **HA device class** | `timestamp` |
| **Platforms** | Windows, Linux |

Example: `2024-11-15T10:30:00.0000000Z`

---

### NetworkAvailability

Network connectivity status.

| | |
|---|---|
| **ConfigKey** | `NetworkAvailability` |
| **Topic** | `samqtt/system_sensor/{device}/network_availability/state` |
| **Value** | `on` / `off` |
| **HA device class** | `connectivity` (binary sensor) |
| **Platforms** | Windows, Linux |

Example: `on`

---

### Drive (multi-sensor)

Per-drive storage statistics. SAMQTT auto-discovers all ready, local drives at startup (excludes network, RAM, and overlay drives).

Each drive creates three child sensors, where `{id}` is the drive letter (Windows: `c`, `d`) or mount path suffix (Linux: root becomes `root`, `/home` becomes `home`).

**ConfigKey:** `Drive` (parent), with child sensors configured under `MultiSensors.Drive.Sensors`.

#### DriveFreeSizeSensor

| | |
|---|---|
| **ConfigKey** | `DriveFreeSize` |
| **Topic** | `samqtt/system_sensor/{device}/drive_free_size_{id}/state` |
| **Value** | Free space in GB (2 decimal places) |
| **Unit** | `GB` |
| **HA device class** | `data_size` |
| **HA state class** | `measurement` |
| **Platforms** | Windows, Linux |

Example: `samqtt/system_sensor/mypc/drive_free_size_c/state` → `123.45`

#### DriveTotalSizeSensor

| | |
|---|---|
| **ConfigKey** | `DriveTotalSize` |
| **Topic** | `samqtt/system_sensor/{device}/drive_total_size_{id}/state` |
| **Value** | Total size in GB (2 decimal places) |
| **Unit** | `GB` |
| **HA device class** | `data_size` |
| **HA state class** | `measurement` |
| **Platforms** | Windows, Linux |

Example: `samqtt/system_sensor/mypc/drive_total_size_c/state` → `476.84`

#### DrivePercentFreeSizeSensor

| | |
|---|---|
| **ConfigKey** | `DrivePercentFreeSize` |
| **Topic** | `samqtt/system_sensor/{device}/drive_percent_free_size_{id}/state` |
| **Value** | Free space as percentage of total (1 decimal place) |
| **Unit** | `%` |
| **HA state class** | `measurement` |
| **Platforms** | Windows, Linux |

Example: `samqtt/system_sensor/mypc/drive_percent_free_size_c/state` → `25.9`

---

## Linux-Only Sensors

### FreeMemory

Available physical memory, read from `/proc/meminfo` (`MemAvailable`).

| | |
|---|---|
| **ConfigKey** | `FreeMemory` |
| **Topic** | `samqtt/system_sensor/{device}/free_memory/state` |
| **Value** | Available memory in bytes |
| **Unit** | `B` |
| **HA device class** | `memory` |
| **HA state class** | `measurement` |
| **Platforms** | Linux |

Example: `samqtt/system_sensor/myserver/free_memory/state` → `4294967296`

---

### CpuProcessorTime

CPU usage percentage, calculated from `/proc/stat` deltas between polls. Returns `0.0` on the first collection.

| | |
|---|---|
| **ConfigKey** | `CpuProcessorTime` |
| **Topic** | `samqtt/system_sensor/{device}/cpu_processor_time/state` |
| **Value** | CPU usage percentage (0–100) |
| **Unit** | `%` |
| **HA state class** | `measurement` |
| **Platforms** | Linux |

Example: `samqtt/system_sensor/myserver/cpu_processor_time/state` → `42.5`

---

## Windows-Only Sensors

### FreeMemory

Available physical memory via the `GlobalMemoryStatusEx` Win32 API.

| | |
|---|---|
| **ConfigKey** | `FreeMemory` |
| **Topic** | `samqtt/system_sensor/{device}/free_memory/state` |
| **Value** | Available memory in bytes |
| **Unit** | `B` |
| **HA device class** | `memory` |
| **HA state class** | `measurement` |
| **Platforms** | Windows |

Example: `samqtt/system_sensor/mypc/free_memory/state` → `8589934592`

---

### CpuProcessorTime

CPU usage percentage, calculated from `GetSystemTimes` Win32 API deltas between polls. Returns `0.0` on the first collection.

| | |
|---|---|
| **ConfigKey** | `CpuProcessorTime` |
| **Topic** | `samqtt/system_sensor/{device}/cpu_processor_time/state` |
| **Value** | CPU usage percentage (0–100) |
| **Unit** | `%` |
| **HA state class** | `measurement` |
| **Platforms** | Windows |

Example: `samqtt/system_sensor/mypc/cpu_processor_time/state` → `18.3`

---

### ComputerInUse

Detects whether the computer has had user input in the last 30 seconds (keyboard/mouse activity via `GetLastInputInfo`).

| | |
|---|---|
| **ConfigKey** | `ComputerInUse` |
| **Topic** | `samqtt/system_sensor/{device}/computer_in_use/state` |
| **Value** | `on` / `off` |
| **HA device class** | binary sensor |
| **Platforms** | Windows |

Example: `samqtt/system_sensor/mypc/computer_in_use/state` → `on`
