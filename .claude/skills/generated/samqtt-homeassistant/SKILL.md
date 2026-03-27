---
name: samqtt-homeassistant
description: "Skill for the Samqtt.HomeAssistant area of samqtt. 14 symbols across 6 files."
---

# Samqtt.HomeAssistant

14 symbols | 6 files | Cohesion: 96%

## When to Use

- Working with code in `src/`
- Understanding how HomeAssistantSensorValueFormatter, HomeAssistantPublisher, PublishOnlineStatus work
- Modifying samqtt.homeassistant-related functionality

## Key Files

| File | Symbols |
|------|---------|
| `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs` | PublishOnlineStatus, PublishOfflineStatus, PublishSensorValue, PublishActionStateValue, PublishSensorStateDiscoveryMessage (+2) |
| `src/Samqtt.HomeAssistant/HomeAssistantSensorValueFormatter.cs` | HomeAssistantSensorValueFormatter, Format, SerializeFallback |
| `src/Samqtt.Common/ITopicProvider.cs` | GetActionResponseDiscoveryTopic |
| `src/Samqtt.Common/IMqttPublisher.cs` | PublishAsync |
| `src/Samqtt.Common/SystemSensors/ISystemSensorValueFormatter.cs` | ISystemSensorValueFormatter |
| `src/Samqtt.Common/IMessagePublisher.cs` | IMessagePublisher |

## Entry Points

Start here when exploring this area:

- **`HomeAssistantSensorValueFormatter`** (Class) — `src/Samqtt.HomeAssistant/HomeAssistantSensorValueFormatter.cs:7`
- **`HomeAssistantPublisher`** (Class) — `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs:9`
- **`PublishOnlineStatus`** (Method) — `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs:22`
- **`PublishOfflineStatus`** (Method) — `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs:28`
- **`PublishSensorValue`** (Method) — `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs:34`

## Key Symbols

| Symbol | Type | File | Line |
|--------|------|------|------|
| `HomeAssistantSensorValueFormatter` | Class | `src/Samqtt.HomeAssistant/HomeAssistantSensorValueFormatter.cs` | 7 |
| `HomeAssistantPublisher` | Class | `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs` | 9 |
| `ISystemSensorValueFormatter` | Interface | `src/Samqtt.Common/SystemSensors/ISystemSensorValueFormatter.cs` | 4 |
| `IMessagePublisher` | Interface | `src/Samqtt.Common/IMessagePublisher.cs` | 7 |
| `PublishOnlineStatus` | Method | `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs` | 22 |
| `PublishOfflineStatus` | Method | `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs` | 28 |
| `PublishSensorValue` | Method | `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs` | 34 |
| `PublishActionStateValue` | Method | `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs` | 46 |
| `PublishSensorStateDiscoveryMessage` | Method | `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs` | 58 |
| `PublishActionStateDiscoveryMessage` | Method | `src/Samqtt.HomeAssistant/HomeAssistantPublisher.cs` | 95 |
| `GetActionResponseDiscoveryTopic` | Method | `src/Samqtt.Common/ITopicProvider.cs` | 14 |
| `Format` | Method | `src/Samqtt.HomeAssistant/HomeAssistantSensorValueFormatter.cs` | 30 |
| `PublishAsync` | Method | `src/Samqtt.Common/IMqttPublisher.cs` | 15 |
| `SerializeFallback` | Method | `src/Samqtt.HomeAssistant/HomeAssistantSensorValueFormatter.cs` | 89 |

## Execution Flows

| Flow | Type | Steps |
|------|------|-------|
| `GetEnabledActions → GetActionResponseDiscoveryTopic` | cross_community | 3 |

## How to Explore

1. `gitnexus_context({name: "HomeAssistantSensorValueFormatter"})` — see callers and callees
2. `gitnexus_query({query: "samqtt.homeassistant"})` — find related execution flows
3. Read key files listed above for implementation details
