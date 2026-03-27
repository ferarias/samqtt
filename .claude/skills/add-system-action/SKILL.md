---
name: add-system-action
description: "Step-by-step recipe for adding a new SystemAction (fire-and-forget or value-returning) to samqtt, covering file location, base class, payload handling, DI registration, options config, and documentation."
---

# Adding a New SystemAction to samqtt

Follow every numbered step. Each step maps to a concrete file edit.

---

## How actions work

An action is triggered by an inbound MQTT message. The flow is:

```
MQTT message → CommandTopic
    ↓
MqttSubscriber invokes action.HandleAsync(payload, ct)
    ↓
Action.HandleCoreAsync() runs your logic
    ↓
If ReturnsState == true:
    result → StateTopic (scalar string)
    result → JsonAttributesTopic (full JSON, useful for collections)
```

The `payload` parameter in `HandleCoreAsync` is the raw MQTT message body (a string). Parse it however your action needs — plain string, integer, JSON object.

---

## Decision: which project?

| Condition | Project |
|-----------|---------|
| Works on all platforms | `src/Samqtt.SystemActions/` |
| Windows-only API (P/Invoke, WMI, etc.) | `src/Samqtt.SystemActions.Windows/` |
| Linux-only but code compiles everywhere | `src/Samqtt.SystemActions/` + `OperatingSystem.IsLinux()` guard in registration |

---

## Decision: which return type?

| Scenario | Base class | `ReturnsState` |
|----------|------------|----------------|
| Fire-and-forget (no result needed) | `SystemAction<Unit>` | `false` — no state/attributes topics created |
| Returns a single value | `SystemAction<bool>`, `SystemAction<string>`, etc. | `true` |
| Returns a collection | `SystemAction<YourRecord[]>` | `true` — count published to StateTopic, full JSON to JsonAttributesTopic |

Use `Unit` (defined in `Samqtt.Common`) for fire-and-forget. Return `Unit.Default` at the end.

---

## Step 1 — Create the action class

### 1a. Fire-and-forget action (no return value)

Put the file in `src/<TargetProject>/Actions/YourNameAction.cs`.

```csharp
namespace Samqtt.SystemActions.Actions   // adjust namespace for Windows project
{
    public class YourNameAction : SystemAction<Unit>
    {
        // Must match the key in appsettings.json exactly (case-insensitive).
        public override string ConfigKey => "YourName";

        public override Task<Unit> HandleCoreAsync(string payload, CancellationToken cancellationToken)
        {
            // payload is the raw MQTT message body
            // Do your work here
            return Task.FromResult(Unit.Default);
        }
    }
}
```

### 1b. Value-returning action (scalar)

```csharp
namespace Samqtt.SystemActions.Actions
{
    public class YourNameAction : SystemAction<bool>
    {
        public override string ConfigKey => "YourName";

        public override Task<bool> HandleCoreAsync(string payload, CancellationToken cancellationToken)
        {
            // payload is the process name, a JSON string, etc.
            var success = /* ... your logic ... */;
            return Task.FromResult(success);
        }
    }
}
```

### 1c. Collection-returning action

Define a record for the item type in the same file, then return an array:

```csharp
namespace Samqtt.SystemActions.Actions
{
    public class YourNameAction : SystemAction<YourItem[]>
    {
        public override string ConfigKey => "YourName";

        public override Task<YourItem[]> HandleCoreAsync(string payload, CancellationToken cancellationToken)
        {
            var items = /* ... build your collection ... */;
            return Task.FromResult(items.ToArray());
        }
    }

    public record YourItem(string Name, int Value);
}
```

The subscriber automatically publishes the count to `StateTopic` and the full JSON array to `JsonAttributesTopic`.

### 1d. Action with a JSON payload

When the caller sends a JSON object as the MQTT payload, deserialize it using `System.Text.Json`:

```csharp
using System.Text.Json;

namespace Samqtt.SystemActions.Actions
{
    public class YourNameAction : SystemAction<Unit>
    {
        public override string ConfigKey => "YourName";

        public override async Task<Unit> HandleCoreAsync(string payload, CancellationToken cancellationToken)
        {
            var parameters = JsonSerializer.Deserialize<YourParameters>(payload);
            if (parameters != null)
            {
                // use parameters.SomeField, etc.
            }
            return Unit.Default;
        }
    }

    public record YourParameters(string SomeField, int AnotherField);
}
```

**Rules:**
- `ConfigKey` is matched case-insensitively against the `Actions` dictionary in `appsettings.json`.
- `Metadata` is set by the factory before the first invocation — never set it yourself.
- Do not override `ReturnsState` unless you have a special reason; the default (`typeof(T) != typeof(Unit)`) is always correct.
- `cancellationToken` is passed through; honour it for async operations.

---

## Step 2 — Register in DI

### 2a. Cross-platform action

Edit `src/Samqtt.SystemActions/ServiceCollectionExtensions.cs`:

```csharp
public static IServiceCollection AddSystemActions(this IServiceCollection services) =>
    services
        .AddSystemAction<GetProcessAction>()
        .AddSystemAction<GetProcessesAction>()
        .AddSystemAction<KillProcessAction>()
        .AddSystemAction<StartProcessAction>()
        .AddSystemAction<YourNameAction>();   // ← add this line
```

### 2b. Linux-only action (inside the cross-platform project)

```csharp
public static IServiceCollection AddSystemActions(this IServiceCollection services)
{
    services
        .AddSystemAction<GetProcessAction>()
        // ... other actions ...

    if (OperatingSystem.IsLinux())
        services.AddSystemAction<YourNameAction>();

    return services;
}
```

Note: the method must change from an expression body (`=>`) to a block body when adding a conditional.

### 2c. Windows-only action

Edit `src/Samqtt.SystemActions.Windows/ServiceCollectionExtensions.cs`:

```csharp
public static IServiceCollection AddWindowsSpecificSystemActions(this IServiceCollection services) =>
    services
        .AddSystemAction<HibernateAction>()
        .AddSystemAction<RebootAction>()
        .AddSystemAction<ShutdownAction>()
        .AddSystemAction<SuspendAction>()
        .AddSystemAction<YourNameAction>();   // ← add this line
```

`AddSystemAction<T>()` is defined in `Samqtt.Common` (`SystemActionsServiceCollectionExtensions.cs`). It registers `T` as both its concrete type and as `ISystemAction` (singleton). Never call `services.AddSingleton<ISystemAction, T>()` directly.

---

## Step 3 — Options / appsettings configuration

Actions are enabled/disabled through `SamqttOptions.Actions`, a `Dictionary<string, SystemActionOptions>`. The key is the `ConfigKey` value; the only property is `Enabled`.

**`src/Samqtt/appsettings.json`** — development/test defaults:

```json
"Actions": {
  "Reboot":          { "Enabled": true },
  "StartProcess":    { "Enabled": true },
  "YourName":        { "Enabled": true }   // ← add this entry
}
```

**`setup/linux/samqtt.appsettings.template.json`** — Linux install template:

Add the same entry. Set `Enabled: false` for Windows-only actions; `true` for Linux or cross-platform ones.

**`setup/windows/samqtt.appsettings.template.json`** — Windows install template:

Same pattern, reversed logic for platform-specific actions.

> The factory (`SystemActionFactory.GetEnabledActions`) iterates `_options.Actions`, skips disabled entries, then resolves the matching `ISystemAction` by `ConfigKey`. If the key is in config but no implementation is registered (e.g. Windows action on Linux), a warning is logged and the action is silently skipped.

---

## Step 4 — MQTT topic schema

Topics are generated by `TopicProvider`. For an action with `ConfigKey = "YourName"` on device `mymachine`:

| Topic | Address | Direction |
|-------|---------|-----------|
| Command | `samqtt/system_action/mymachine/yourname/request` | Inbound (caller sends here) |
| State | `samqtt/system_action/mymachine/yourname/state` | Outbound (result scalar) |
| Attributes | `samqtt/system_action/mymachine/yourname/attributes` | Outbound (full JSON) |
| HA Discovery | `homeassistant/sensor/samqtt_mymachine_yourname/config` | Outbound (on startup) |

State and Attributes topics only exist when `ReturnsState == true`.

---

## Step 5 — Documentation

Update `docs/Listeners.md`. Follow the existing pattern:

```markdown
### YourName action

Brief description of what it does.

**Command topic:** `samqtt/{hostname}/system_action/yourname/request`
**Payload:** plain string or JSON describing the input

**State topic** (result): `samqtt/{hostname}/system_action/yourname/state`
Returns: `True` / `False` / count of items / etc.

**Platforms:** Linux / Windows / All

Example payload:
\`\`\`
some_process_name
\`\`\`
Or for JSON payloads:
\`\`\`json
{ "SomeField": "value", "AnotherField": 42 }
\`\`\`
```

---

## Complete checklist

- [ ] Created `src/<Project>/Actions/YourNameAction.cs` extending `SystemAction<T>`
- [ ] Chose the right `T`: `Unit` for fire-and-forget, concrete type for returning results
- [ ] `ConfigKey` matches the appsettings key exactly
- [ ] `HandleCoreAsync` parses `payload` and performs the action
- [ ] Registered via `AddSystemAction<YourNameAction>()` in the correct `ServiceCollectionExtensions.cs`
- [ ] Linux-only actions wrapped in `if (OperatingSystem.IsLinux())`
- [ ] Entry added to `src/Samqtt/appsettings.json`
- [ ] Entry added to `setup/linux/samqtt.appsettings.template.json`
- [ ] Entry added to `setup/windows/samqtt.appsettings.template.json`
- [ ] `docs/Listeners.md` updated

---

## Key files for reference

| File | Purpose |
|------|---------|
| `src/Samqtt.Common/SystemActions/SystemAction.cs` | Base class — implement `HandleCoreAsync` |
| `src/Samqtt.Common/SystemActions/ISystemAction.cs` | Interface (`ConfigKey`, `ReturnsState`, `HandleAsync`) |
| `src/Samqtt.Common/SystemActions/SystemActionMetadata.cs` | Metadata shape (CommandTopic, StateTopic, JsonAttributesTopic) |
| `src/Samqtt.Common/SystemActions/SystemActionsServiceCollectionExtensions.cs` | `AddSystemAction<T>()` helper |
| `src/Samqtt.Common/Unit.cs` | `Unit` struct for fire-and-forget; return `Unit.Default` |
| `src/Samqtt.Application/SystemActionFactory.cs` | How actions are resolved, filtered, and metadata assigned |
| `src/Samqtt.Common/Options/SamqttOptions.cs` | `Actions` dictionary definition |
| `src/Samqtt.Common/Options/SystemActionOptions.cs` | `Enabled` flag |
| `src/Samqtt.SystemActions/ServiceCollectionExtensions.cs` | Cross-platform registration |
| `src/Samqtt.SystemActions.Windows/ServiceCollectionExtensions.cs` | Windows-only registration |

### Real examples to copy from

| Action | Platform | Pattern | File |
|--------|----------|---------|------|
| `RebootAction` | Windows | Fire-and-forget, parses int payload | `src/Samqtt.SystemActions.Windows/Actions/RebootAction.cs` |
| `GetProcessAction` | All | Returns `bool` | `src/Samqtt.SystemActions/Actions/GetProcessAction.cs` |
| `KillProcessAction` | All | Returns `bool`, error-handled | `src/Samqtt.SystemActions/Actions/KillProcessAction.cs` |
| `GetProcessesAction` | All | Returns `ProcessInfo[]` (collection) | `src/Samqtt.SystemActions/Actions/GetProcessesAction.cs` |
| `StartProcessAction` | All | Fire-and-forget, deserializes JSON payload | `src/Samqtt.SystemActions/Actions/StartProcessAction.cs` |
