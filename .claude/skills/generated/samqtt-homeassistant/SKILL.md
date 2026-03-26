---
name: samqtt-homeassistant
description: "Skill for the Samqtt.HomeAssistant area of samqtt. 12 symbols across 6 files."
---

# Samqtt.HomeAssistant

12 symbols | 6 files | Cohesion: 96%

## When to Use

- Working with code in `src/`
- Understanding how HomeAssistantSensorValueFormatter, HomeAssistantPublisher, PublishOnlineStatus work
- Modifying samqtt.homeassistant-related functionality

## Key Files

| File | Symbols |
|------|---------|
| `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs` | PublishOnlineStatus, PublishOfflineStatus, PublishSensorValue, PublishActionStateValue, PublishSensorStateDiscoveryMessage (+2) |
| `src/Samqtt.Common/ITopicProvider.cs` | GetActionResponseDiscoveryTopic |
| `src/Samqtt.Common/IMqttPublisher.cs` | PublishAsync |
| `src/Samqtt.HomeAssistant/HomeAssistantSensorValueFormatter.cs` | HomeAssistantSensorValueFormatter |
| `src/Samqtt.Common/SystemSensors/ISystemSensorValueFormatter.cs` | ISystemSensorValueFormatter |
| `src/Samqtt.Common/IMessagePublisher.cs` | IMessagePublisher |

## Entry Points

Start here when exploring this area:

- **`HomeAssistantSensorValueFormatter`** (Class) — `src/Samqtt.HomeAssistant/HomeAssistantSensorValueFormatter.cs:6`
- **`HomeAssistantPublisher`** (Class) — `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs:10`
- **`PublishOnlineStatus`** (Method) — `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs:31`
- **`PublishOfflineStatus`** (Method) — `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs:37`
- **`PublishSensorValue`** (Method) — `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs:43`

## Key Symbols

| Symbol | Type | File | Line |
|--------|------|------|------|
| `HomeAssistantSensorValueFormatter` | Class | `src/Samqtt.HomeAssistant/HomeAssistantSensorValueFormatter.cs` | 6 |
| `HomeAssistantPublisher` | Class | `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs` | 10 |
| `ISystemSensorValueFormatter` | Interface | `src/Samqtt.Common/SystemSensors/ISystemSensorValueFormatter.cs` | 2 |
| `IMessagePublisher` | Interface | `src/Samqtt.Common/IMessagePublisher.cs` | 7 |
| `PublishOnlineStatus` | Method | `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs` | 31 |
| `PublishOfflineStatus` | Method | `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs` | 37 |
| `PublishSensorValue` | Method | `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs` | 43 |
| `PublishActionStateValue` | Method | `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs` | 55 |
| `PublishSensorStateDiscoveryMessage` | Method | `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs` | 67 |
| `PublishActionStateDiscoveryMessage` | Method | `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs` | 111 |
| `GetActionResponseDiscoveryTopic` | Method | `src/Samqtt.Common/ITopicProvider.cs` | 14 |
| `PublishAsync` | Method | `src/Samqtt.Common/IMqttPublisher.cs` | 15 |

## Execution Flows

| Flow | Type | Steps |
|------|------|-------|
| `GetEnabledActions → GetActionResponseDiscoveryTopic` | cross_community | 3 |

## How to Explore

1. `gitnexus_context({name: "HomeAssistantSensorValueFormatter"})` — see callers and callees
2. `gitnexus_query({query: "samqtt.homeassistant"})` — find related execution flows
3. Read key files listed above for implementation details
