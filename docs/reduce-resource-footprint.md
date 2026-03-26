# Plan: Reduce Resource Footprint

## Goal

Make SAMQTT suitable as a lightweight background daemon with minimal CPU, memory, and binary size impact —
while keeping full functionality on both Linux and Windows.

---

## Current State

| Metric | Estimated Value |
|--------|----------------|
| RSS memory | ~60–80 MB |
| Startup time | ~400–600 ms |
| MQTT publishes/day | ~8,640+ (5s interval, ~10 sensors) |
| QoS level | 1 (AtLeastOnce — requires ACK) |
| Polling mechanism | `Task.Delay` loop + `SemaphoreSlim` |
| DI registration | Scrutor assembly scanning (reflection) |
| Logging | Serilog + Console + File + EventLog sinks |

---

## Proposed Changes

Changes are grouped by risk and ordered within each group by impact.

---

### Phase 1 — Low Risk, High Impact (do first)

#### 1.1 Change sensor publishes to QoS 0 ✅ DONE

**File:** `src/Samqtt.Broker.Mqtt2Net/MqttPublisher.cs`

Change `QualityOfServiceLevel.AtLeastOnce` → `QualityOfServiceLevel.AtMostOnce` for sensor value
publishes. Keep QoS 1 for Home Assistant discovery messages and LWT (online/offline), since those
are control-plane messages worth guaranteeing.

**Why:** Sensor readings are naturally lossy. QoS 1 adds per-message ACK state on both client and
broker for data where losing an occasional reading is acceptable.

**Risk:** None. Home Assistant handles missing sensor updates gracefully.

---

#### 1.2 Replace `Task.Delay` loop with `PeriodicTimer` ✅ DONE

**File:** `src/Samqtt.Application/Win2MqttBackgroundService.cs`

Replace:
```csharp
await Task.Delay(TimeSpan.FromSeconds(options.CurrentValue.TimerInterval), stoppingToken);
```
With a `PeriodicTimer` initialized once before the loop:
```csharp
using var timer = new PeriodicTimer(TimeSpan.FromSeconds(options.CurrentValue.TimerInterval));
while (await timer.WaitForNextTickAsync(stoppingToken)) { ... }
```

**Why:** `PeriodicTimer` is the .NET 6+ idiomatic way to write background loops. It avoids timer
drift, has lower allocations than `Task.Delay`, and integrates cleanly with cancellation.

**Risk:** None. Behavioral change is negligible.

---

#### 1.3 Increase default polling interval ✅ DONE

**File:** `src/Samqtt.Common/Options/SamqttOptions.cs` (or `appsettings.json` default)

Change default `TimerInterval` from `5` seconds to `30` seconds.

**Why:** For Home Assistant dashboards, 30-second resolution is perfectly adequate. This reduces
MQTT publishes by 6x (~1,440/day instead of ~8,640) and cuts CPU wakeups proportionally.

**Risk:** Users relying on 5s granularity will need to explicitly set their interval. Update
documentation accordingly.

---

### Phase 2 — Medium Risk, High Impact

#### 2.1 Remove Scrutor — replace with explicit DI registrations ✅ DONE

**Files changed:**
- `src/Samqtt.Common/SystemSensors/SystemSensorsServiceCollectionExtensions.cs` — replaced `.Scan()` with `AddSystemSensor<T>()` / `AddSystemMultiSensor<T>()` helpers
- `src/Samqtt.Common/SystemActions/SystemActionsServiceCollectionExtensions.cs` — replaced `.Scan()` with `AddSystemAction<T>()` helper
- `src/Samqtt.SystemSensors/ServiceCollectionExtensions.cs` — explicit list of cross-platform sensors
- `src/Samqtt.SystemSensors.Windows/ServiceCollectionExtensions.cs` — explicit list of Windows-only sensors
- `src/Samqtt.SystemActions/ServiceCollectionExtensions.cs` — explicit list of cross-platform actions
- `src/Samqtt.SystemActions.Windows/ServiceCollectionExtensions.cs` — explicit list of Windows-only actions
- Removed `Scrutor` from 5 `.csproj` files (`Samqtt.Common`, both SystemSensors projects, both SystemActions projects)

**Note:** The keyed child-sensor registration for multi-sensors (e.g. per-drive) still uses
reflection at startup to discover child types by naming convention. This is intentional — drive
letters are only known at runtime. New child sensor types added to the same assembly are still
picked up automatically. See `CONTRIBUTING.md` for details.

---

#### 2.2 Remove reflection from `SystemSensorFactory` ✅ DONE

**File:** `src/Samqtt.Application/SystemSensorFactory.cs`

The factory currently uses reflection to match config key strings (e.g., `"CpuProcessorTime"`) to
concrete sensor classes. Replace with an explicit dictionary or switch expression.

**Why:** Reflection blocks trimming and adds startup overhead. An explicit map is also easier to
reason about.

**Risk:** Low. Must ensure all sensor/action names remain consistent with config keys.

---

#### 2.3 Pool MQTT payload buffers ✅ DONE

**File:** `src/Samqtt.Broker.Mqtt2Net/MqttPublisher.cs`

Use `ArrayPool<byte>.Shared` to rent/return byte arrays for UTF-8 payload encoding instead of
allocating a new `byte[]` per publish.

**Why:** With 10+ sensors at 5–30s intervals, repeated small allocations add steady GC pressure.
Pooling eliminates these allocations from the hot path entirely.

**Risk:** Low, but requires careful `try/finally` to always return rented buffers.

---

#### 2.4 Slim down Serilog sinks

**File:** `src/Samqtt/Program.cs`, `appsettings.json`

- Remove the `Serilog.Sinks.File` dependency; redirect file logging to stdout (let the OS/service
  manager handle log rotation via `journald` on Linux or redirected stdout on Windows).
- On Windows, remove `Serilog.Sinks.EventLog` from default configuration (keep as opt-in via config).
- Raise minimum level default from `Information` to `Warning` for production.

**Why:** Each active sink adds background threads and memory buffers. File rolling keeps handles
open. The EventLog sink on Windows is particularly heavy.

**Risk:** Medium. Users who rely on file logs will need to adjust. Should be opt-in via config.

---

### Phase 3 — Higher Risk, Highest Impact

#### 3.1 Enable Native AOT

**Files:** `src/Samqtt/Samqtt.csproj`, all dependent projects

Add to entry point `.csproj`:
```xml
<PublishAot>true</PublishAot>
<TrimmerRootDescriptor>TrimmerRoots.xml</TrimmerRootDescriptor>
```

Prerequisites (must complete Phase 2 first):
- No reflection-based DI registration (2.1)
- No reflection in factories (2.2)
- All serialization annotated with source generators (`[JsonSerializable]`)
- MQTTnet 5.x AOT compatibility verified (may require downgrade to 4.x or switch library)
- `Scrutor` removed (2.1)

**Expected outcome:**
- Memory: ~60 MB → ~10–15 MB
- Startup: ~500 ms → ~30–50 ms
- Binary size: self-contained single file, no JIT warmup

**Risk:** High. AOT is a significant constraint. Requires:
- Testing both Linux and Windows targets
- Replacing any `dynamic`, reflection, or `Activator.CreateInstance` usage
- Validating that MQTTnet works under trimming (check for `[RequiresUnreferencedCode]` warnings)
- P/Invoke calls in Windows sensors must be declared correctly

---

#### 3.2 Replace Microsoft.Extensions.Hosting with a minimal host (optional)

Replace the full generic host (`IHostBuilder`, `BackgroundService`, etc.) with a plain `CancellationTokenSource` + a hand-rolled run loop.

**Why:** The generic host drags in ~10 transitive packages. A minimal host halves this.

**Note:** Only pursue this if AOT alone does not meet memory targets. The generic host is well-tested
and its removal increases maintenance burden. Evaluate after Phase 3.1.

---

## Summary Table

| # | Change | Risk | Memory Δ | CPU/Network Δ | AOT Prereq |
|---|--------|------|----------|---------------|------------|
| 1.1 | QoS 0 for sensors | None | — | ↓ broker state | No |
| 1.2 | PeriodicTimer | None | ↓ minor | ↓ minor | No |
| 1.3 | Default interval 30s | None | — | ↓↓ 6x fewer publishes | No |
| 2.1 | Remove Scrutor | Low | ↓ minor | ↓ startup | Yes |
| 2.2 | Explicit sensor factory | Low | ↓ minor | ↓ startup | Yes |
| 2.3 | Pool MQTT buffers | Low | ↓ GC pressure | ↓ minor | No |
| 2.4 | Slim Serilog sinks | Medium | ↓ moderate | — | No |
| 3.1 | Native AOT | High | ↓↓ ~50–70 MB | ↓↓ startup | Required |
| 3.2 | Minimal host (optional) | High | ↓ moderate | — | No |

---

## What to Keep As-Is

- MQTTnet 5.x — keep unless AOT compatibility proves problematic
- DI container for all platform-specific registrations — clean pattern, low overhead
- `SemaphoreSlim` in polling loop — correct and simple; no need to parallelize
- Home Assistant discovery format — no changes needed
- Configuration file locations and structure — no user-facing changes

---

## Notes

- Phases 1 and 2 can be done incrementally on the current branch without breaking changes.
- Phase 3 (AOT) should be done on a separate branch and validated against both `net8.0` and
  `net8.0-windows8.0` targets before merging.
- If AOT proves too complex for MQTTnet or Windows P/Invoke sensors, the gains from Phases 1–2
  alone (QoS 0 + longer interval + Serilog reduction) still meaningfully reduce the operational
  footprint.
