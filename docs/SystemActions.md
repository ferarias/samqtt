# System Actions

SAMQTT subscribes to action command topics and executes the corresponding action when a message is received.

**Command topic:**
```
samqtt/system_action/{device}/{action_name}/request
```

**Result topic** (for actions that return a value):
```
samqtt/system_action/{device}/{action_name}/state
samqtt/system_action/{device}/{action_name}/attributes
```

where `{device}` is the `DeviceIdentifier` from configuration (defaults to machine hostname).

Actions that return `Unit` (fire-and-forget) do not publish to a state topic.

---

## Cross-Platform Actions

### GetProcess

Checks whether a process is currently running.

| | |
|---|---|
| **ConfigKey** | `GetProcess` |
| **Command topic** | `samqtt/system_action/{device}/get_process/request` |
| **State topic** | `samqtt/system_action/{device}/get_process/state` |
| **Payload** | Process name (plain text, without `.exe`) |
| **Returns** | `true` if running, `false` otherwise |
| **Platforms** | Windows, Linux |

Example: publish `notepad` → receives `true`

---

### GetProcesses

Returns a list of all currently running processes.

| | |
|---|---|
| **ConfigKey** | `GetProcesses` |
| **Command topic** | `samqtt/system_action/{device}/get_processes/request` |
| **State topic** | `samqtt/system_action/{device}/get_processes/state` |
| **Attributes topic** | `samqtt/system_action/{device}/get_processes/attributes` |
| **Payload** | Empty |
| **Returns** | JSON array of `{ "name": string, "id": number }` objects |
| **Platforms** | Windows, Linux |

Example return value:
```json
[
  { "name": "explorer", "id": 1234 },
  { "name": "notepad", "id": 5678 }
]
```

---

### KillProcess

Terminates all processes with the given name.

| | |
|---|---|
| **ConfigKey** | `KillProcess` |
| **Command topic** | `samqtt/system_action/{device}/kill_process/request` |
| **State topic** | `samqtt/system_action/{device}/kill_process/state` |
| **Payload** | Process name (plain text, without `.exe`) |
| **Returns** | `true` on success, `false` on error |
| **Platforms** | Windows, Linux |

Example: publish `notepad` → receives `true`

---

### StartProcess

Starts a process and waits for it to exit.

| | |
|---|---|
| **ConfigKey** | `StartProcess` |
| **Command topic** | `samqtt/system_action/{device}/start_process/request` |
| **Payload** | JSON object (see below) |
| **Returns** | Unit (no state topic) |
| **Platforms** | Windows, Linux |

Payload fields:

| Field | Type | Required | Description |
|---|---|---|---|
| `CommandString` | string | Yes | Executable path or command name |
| `ExecParameters` | string | No | Command-line arguments |
| `WindowStyle` | int | No | `0`=Normal, `1`=Hidden, `2`=Minimized, `3`=Maximized |

Example:
```json
{ "CommandString": "notepad.exe", "ExecParameters": "C:\\notes.txt", "WindowStyle": 0 }
```

---

## Power State Actions

Power state actions are available on both platforms but use different mechanisms.

| Action | Linux | Windows |
|---|---|---|
| Reboot | `systemctl reboot` | `WindowsPowerManagement.Reboot(delay)` |
| Shutdown | `systemctl poweroff` | `WindowsPowerManagement.Shutdown(delay)` |
| Suspend | `systemctl suspend` | `WindowsPowerManagement.SuspendSystem()` |
| Hibernate | `systemctl hibernate` | `WindowsPowerManagement.HibernateSystem()` |

### Reboot

| | |
|---|---|
| **ConfigKey** | `Reboot` |
| **Command topic** | `samqtt/system_action/{device}/reboot/request` |
| **Payload (Linux)** | Empty |
| **Payload (Windows)** | Delay in seconds as integer string (default: `10`) |
| **Returns** | Unit (no state topic) |
| **Platforms** | Windows, Linux |

---

### Shutdown

| | |
|---|---|
| **ConfigKey** | `Shutdown` |
| **Command topic** | `samqtt/system_action/{device}/shutdown/request` |
| **Payload (Linux)** | Empty |
| **Payload (Windows)** | Delay in seconds as integer string (default: `10`) |
| **Returns** | Unit (no state topic) |
| **Platforms** | Windows, Linux |

---

### Suspend

| | |
|---|---|
| **ConfigKey** | `Suspend` |
| **Command topic** | `samqtt/system_action/{device}/suspend/request` |
| **Payload** | Empty |
| **Returns** | Unit (no state topic) |
| **Platforms** | Windows, Linux |

---

### Hibernate

| | |
|---|---|
| **ConfigKey** | `Hibernate` |
| **Command topic** | `samqtt/system_action/{device}/hibernate/request` |
| **Payload** | Empty |
| **Returns** | Unit (no state topic) |
| **Platforms** | Windows, Linux |

---

## Linux-Only Actions

### SendNotification

Sends a desktop notification using `notify-send` (requires a running D-Bus session).

| | |
|---|---|
| **ConfigKey** | `SendNotification` |
| **Command topic** | `samqtt/system_action/{device}/send_notification/request` |
| **Payload** | JSON object (see below) |
| **Returns** | Unit (no state topic) |
| **Platforms** | Linux |

Payload fields:

| Field | Type | Required | Description |
|---|---|---|---|
| `Summary` | string | Yes | Notification title |
| `Body` | string | No | Notification body text |
| `Icon` | string | No | Theme icon name (e.g. `dialog-information`, `dialog-warning`) or absolute path to an image file |

Example:
```json
{ "Summary": "Hello from Home Assistant", "Body": "Motion detected in the garden", "Icon": "dialog-information" }
```
