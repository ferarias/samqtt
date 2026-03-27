---
name: add-system-sensor
description: "Step-by-step recipe for adding a new SystemSensor (simple or multi) to samqtt, covering file location, base class, HA metadata, DI registration, options config, and documentation."
---

# Adding a New SystemSensor to samqtt

Follow every numbered step. Each step maps to a concrete file edit.

---

## Decision: which project?

| Condition | Project |
|-----------|---------|
| Works on all platforms (uses only .NET BCL) | `src/Samqtt.SystemSensors/` |
| Windows-only API (PerformanceCounter, P/Invoke to win32) | `src/Samqtt.SystemSensors.Windows/` |
| Linux-only (reads `/proc/*`, etc.) but code compiles everywhere | `src/Samqtt.SystemSensors/` + `OperatingSystem.IsLinux()` guard in registration |

---

## Step 1 — Create the sensor class

### 1a. Simple sensor

Put the file in `src/<TargetProject>/Sensors/YourNameSensor.cs`.

```csharp
using Microsoft.Extensions.Logging;

namespace Samqtt.SystemSensors.Sensors   // adjust namespace for Windows project
{
    [HomeAssistantSensor(unitOfMeasurement: "°C", deviceClass: "temperature", stateClass: "measurement")]
    public class YourNameSensor(ILogger<YourNameSensor> logger) : SystemSensor<double>
    {
        // Must match the key used in appsettings.json exactly (case-insensitive).
        public override string ConfigKey => "YourName";

        // Mirror the attribute values here — the factory reads this, not the attribute.
        public override SensorAttributeInfo GetSensorAttributes() => new()
        {
            UnitOfMeasurement = "°C",
            DeviceClass = "temperature",
            StateClass = "measurement",
        };

        protected override Task<double> CollectInternalAsync()
        {
            var value = /* ... your measurement logic ... */ 42.0;
            logger.LogDebug("Collect {Key}: {Value}", Metadata.Key, value);
            return Task.FromResult(value);
        }
    }
}
```

**Rules:**
- Extend `SystemSensor<T>` where `T` is the return type (`double`, `long`, `bool`, `DateTime`, …).
- `ConfigKey` is matched case-insensitively against the `Sensors` dictionary in `appsettings.json`.
- `GetSensorAttributes()` must return values that match the attribute — the attribute itself is currently unused at runtime.
- Use `Metadata.Key` in log messages (it is the sanitized config key, set by the factory before `CollectAsync` is called).
- `Metadata` is set by the factory; never set it yourself.

### 1b. Binary sensor

Use `[HomeAssistantBinarySensor]` and return `bool`:

```csharp
[HomeAssistantBinarySensor(deviceClass: "connectivity")]
public class YourBinarySensor(ILogger<YourBinarySensor> logger) : SystemSensor<bool>
{
    public override string ConfigKey => "YourBinary";
    public override SensorAttributeInfo GetSensorAttributes() => new()
    {
        IsBinary = true,
        DeviceClass = "connectivity",
        PayloadOn = "on",
        PayloadOff = "off",
    };

    protected override Task<bool> CollectInternalAsync()
    {
        var value = /* ... */ true;
        logger.LogDebug("Collect {Key}: {Value}", Metadata.Key, value);
        return Task.FromResult(value);
    }
}
```

---

## Step 2 — Register in DI

### 2a. Cross-platform sensor

Edit `src/Samqtt.SystemSensors/ServiceCollectionExtensions.cs`:

```csharp
public static IServiceCollection AddSystemSensors(this IServiceCollection services)
{
    services
        .AddSystemSensor<Sensors.TimestampSensor>()
        .AddSystemSensor<Sensors.NetworkAvailabilitySensor>()
        .AddSystemSensor<Sensors.YourNameSensor>()   // ← add this line
        ...
    return services;
}
```

### 2b. Linux-only sensor (inside the cross-platform project)

```csharp
if (OperatingSystem.IsLinux())
    services.AddSystemSensor<Sensors.YourNameSensor>();
```

### 2c. Windows-only sensor

Edit `src/Samqtt.SystemSensors.Windows/ServiceCollectionExtensions.cs`:

```csharp
public static IServiceCollection AddWindowsSpecificSystemSensors(this IServiceCollection services) =>
    services
        .AddSystemSensor<Sensors.CpuProcessorTimeSensor>()
        .AddSystemSensor<Sensors.YourNameSensor>();   // ← add this line
```

`AddSystemSensor<T>()` is defined in `Samqtt.Common` (`SystemSensorsServiceCollectionExtensions.cs`). It registers `T` as both its concrete type and as `ISystemSensor` (singleton). Never call `services.AddSingleton<ISystemSensor, T>()` directly.

---

## Step 3 — Options / appsettings configuration

Sensors are enabled/disabled through `SamqttOptions.Sensors`, a `Dictionary<string, SystemSensorOptions>`. The key is the `ConfigKey` value; the only property is `Enabled`.

**`src/Samqtt/appsettings.json`** — development/test defaults:

```json
"Sensors": {
  "Timestamp":          { "enabled": true },
  "CpuProcessorTime":   { "enabled": true },
  "YourName":           { "enabled": true }   // ← add this entry
}
```

**`setup/linux/samqtt.appsettings.template.json`** — Linux install template (user-facing):

Add the same entry. If the sensor is Linux-only set `enabled: true`; if Windows-only set `enabled: false` so the template doesn't break on Linux.

**`setup/windows/samqtt.appsettings.template.json`** — Windows install template:

Same pattern, reversed logic for platform-specific sensors.

> The factory (`SystemSensorFactory.GetEnabledSimpleSensors`) iterates `_options.Sensors`, skips disabled entries, then resolves the matching `ISystemSensor` by `ConfigKey`. If the key is in config but no implementation is registered, a warning is logged and the sensor is skipped silently.

---

## Step 4 — Home Assistant metadata reference

### Standard sensor (`[HomeAssistantSensor]`)

| Field | Source | HA effect |
|-------|--------|-----------|
| `unitOfMeasurement` | attr + `GetSensorAttributes()` | `unit_of_measurement` in discovery |
| `deviceClass` | attr + `GetSensorAttributes()` | `device_class` in discovery |
| `stateClass` | attr + `GetSensorAttributes()` | `state_class` in discovery (`measurement` / `total_increasing`) |

### Binary sensor (`[HomeAssistantBinarySensor]`)

| Field | Source | HA effect |
|-------|--------|-----------|
| `deviceClass` | attr + `GetSensorAttributes()` | `device_class` |
| `payloadOn` | attr + `GetSensorAttributes()` | `payload_on` (default `"on"`) |
| `payloadOff` | attr + `GetSensorAttributes()` | `payload_off` (default `"off"`) |

Set `IsBinary = true` in `GetSensorAttributes()` — the factory uses this to pick the discovery topic (`binary_sensor/` prefix) and set the right payloads.

---

## Step 5 — Documentation

Update `docs/Sensors.md`. Follow the existing pattern:

```markdown
### YourName sensor

Brief description of what it measures.

`samqtt/{hostname}/sensor/yourname`

Example: `samqtt/mymachine/sensor/yourname` → `42.0`

**Platforms:** Linux / Windows / All
```

---

## Complete checklist

- [ ] Created `src/<Project>/Sensors/YourNameSensor.cs` extending `SystemSensor<T>`
- [ ] `ConfigKey` matches the appsettings key exactly
- [ ] Both attribute and `GetSensorAttributes()` return the same HA metadata
- [ ] `IsBinary = true` set if it is a binary sensor
- [ ] Registered via `AddSystemSensor<YourNameSensor>()` in the correct `ServiceCollectionExtensions.cs`
- [ ] Linux-only sensors wrapped in `if (OperatingSystem.IsLinux())`
- [ ] Entry added to `src/Samqtt/appsettings.json`
- [ ] Entry added to `setup/linux/samqtt.appsettings.template.json`
- [ ] Entry added to `setup/windows/samqtt.appsettings.template.json`
- [ ] `docs/Sensors.md` updated

---

## Key files for reference

| File | Purpose |
|------|---------|
| `src/Samqtt.Common/SystemSensors/SystemSensor.cs` | Base class — implement `CollectInternalAsync` |
| `src/Samqtt.Common/SystemSensors/SensorAttributeInfo.cs` | Struct returned by `GetSensorAttributes()` |
| `src/Samqtt.Common/SystemSensors/HomeAssistantSensorAttribute.cs` | Attribute for standard sensors |
| `src/Samqtt.Common/SystemSensors/HomeAssistantBinarySensorAttribute.cs` | Attribute for binary sensors |
| `src/Samqtt.Common/SystemSensors/SystemSensorsServiceCollectionExtensions.cs` | `AddSystemSensor<T>()` helper |
| `src/Samqtt.Application/SystemSensorFactory.cs` | How sensors are resolved, filtered, and metadata assigned |
| `src/Samqtt.Common/Options/SamqttOptions.cs` | `Sensors` dictionary definition |
| `src/Samqtt.Common/Options/SystemSensorOptions.cs` | `Enabled` flag |
| `src/Samqtt.SystemSensors/ServiceCollectionExtensions.cs` | Cross-platform registration |
| `src/Samqtt.SystemSensors.Windows/ServiceCollectionExtensions.cs` | Windows-only registration |

### Real examples to copy from

| Sensor | Platform | File |
|--------|----------|------|
| `TimestampSensor` | All | `src/Samqtt.SystemSensors/Sensors/TimestampSensor.cs` |
| `NetworkAvailabilitySensor` | All (binary) | `src/Samqtt.SystemSensors/Sensors/NetworkAvailabilitySensor.cs` |
| `CpuProcessorTimeSensor` (Linux) | Linux | `src/Samqtt.SystemSensors/Sensors/CpuProcessorTimeSensor.cs` |
| `CpuProcessorTimeSensor` (Windows) | Windows | `src/Samqtt.SystemSensors.Windows/Sensors/CpuProcessorTimeSensor.cs` |
| `FreeMemorySensor` | Windows | `src/Samqtt.SystemSensors.Windows/Sensors/FreeMemorySensor.cs` |
