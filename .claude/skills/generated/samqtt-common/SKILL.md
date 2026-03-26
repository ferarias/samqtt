---
name: samqtt-common
description: "Skill for the Samqtt.Common area of samqtt. 33 symbols across 16 files."
---

# Samqtt.Common

33 symbols | 16 files | Cohesion: 93%

## When to Use

- Working with code in `src/`
- Understanding how MetadataBase, SystemSensorMetadata, SystemActionMetadata work
- Modifying samqtt.common-related functionality

## Key Files

| File | Symbols |
|------|---------|
| `src/Samqtt.Common/ITopicProvider.cs` | GetSensorStateTopic, GetBinarySensorDiscoveryTopic, GetStandardSensorDiscoveryTopic, GetUniqueId, GetActionStateTopic (+3) |
| `src/Samqtt.Common/IMessagePublisher.cs` | PublishOnlineStatus, PublishSensorStateDiscoveryMessage, PublishActionStateDiscoveryMessage, PublishOfflineStatus, PublishSensorValue |
| `src/Samqtt.Common/IMqttConnectionManager.cs` | ConnectAsync, DisconnectAsync, IMqttConnectionManager |
| `src/Samqtt.Application/Win2MqttBackgroundService.cs` | StartAsync, StopAsync, ExecuteAsync |
| `src/Samqtt.Application/SystemActionFactory.cs` | GetEnabledActions, CreateMetadata |
| `src/Samqtt.Common/IMqttSubscriber.cs` | SubscribeAsync, IMqttSubscriber |
| `src/Samqtt.Common/MetadataBase.cs` | MetadataBase |
| `src/Samqtt.Application/SystemSensorFactory.cs` | CreateMetadata |
| `src/Samqtt.Common/SystemSensors/SystemSensorMetadata.cs` | SystemSensorMetadata |
| `src/Samqtt.Common/SystemActions/SystemActionMetadata.cs` | SystemActionMetadata |

## Entry Points

Start here when exploring this area:

- **`MetadataBase`** (Class) — `src/Samqtt.Common/MetadataBase.cs:2`
- **`SystemSensorMetadata`** (Class) — `src/Samqtt.Common/SystemSensors/SystemSensorMetadata.cs:3`
- **`SystemActionMetadata`** (Class) — `src/Samqtt.Common/SystemActions/SystemActionMetadata.cs:2`
- **`TopicProvider`** (Class) — `src/Samqtt.Application/TopicProvider.cs:7`
- **`MqttSubscriber`** (Class) — `src/Samqtt.Broker.Mqtt2Net/MqttSubscriber.cs:11`

## Key Symbols

| Symbol | Type | File | Line |
|--------|------|------|------|
| `MetadataBase` | Class | `src/Samqtt.Common/MetadataBase.cs` | 2 |
| `SystemSensorMetadata` | Class | `src/Samqtt.Common/SystemSensors/SystemSensorMetadata.cs` | 3 |
| `SystemActionMetadata` | Class | `src/Samqtt.Common/SystemActions/SystemActionMetadata.cs` | 2 |
| `TopicProvider` | Class | `src/Samqtt.Application/TopicProvider.cs` | 7 |
| `MqttSubscriber` | Class | `src/Samqtt.Broker.Mqtt2Net/MqttSubscriber.cs` | 11 |
| `MqttPublisher` | Class | `src/Samqtt.Broker.Mqtt2Net/MqttPublisher.cs` | 7 |
| `MqttConnector` | Class | `src/Samqtt.Broker.Mqtt2Net/MqttConnector.cs` | 7 |
| `ITopicProvider` | Interface | `src/Samqtt.Common/ITopicProvider.cs` | 2 |
| `IMqttSubscriber` | Interface | `src/Samqtt.Common/IMqttSubscriber.cs` | 7 |
| `IMqttPublisher` | Interface | `src/Samqtt.Common/IMqttPublisher.cs` | 5 |
| `IMqttConnectionManager` | Interface | `src/Samqtt.Common/IMqttConnectionManager.cs` | 5 |
| `GetSensorStateTopic` | Method | `src/Samqtt.Common/ITopicProvider.cs` | 8 |
| `GetBinarySensorDiscoveryTopic` | Method | `src/Samqtt.Common/ITopicProvider.cs` | 9 |
| `GetStandardSensorDiscoveryTopic` | Method | `src/Samqtt.Common/ITopicProvider.cs` | 10 |
| `GetUniqueId` | Method | `src/Samqtt.Common/ITopicProvider.cs` | 7 |
| `GetActionStateTopic` | Method | `src/Samqtt.Common/ITopicProvider.cs` | 13 |
| `GetActionCommandTopic` | Method | `src/Samqtt.Common/ITopicProvider.cs` | 15 |
| `GetActionJsonAttributesTopic` | Method | `src/Samqtt.Common/ITopicProvider.cs` | 16 |
| `GetEnabledActions` | Method | `src/Samqtt.Application/SystemActionFactory.cs` | 17 |
| `StartAsync` | Method | `src/Samqtt.Application/Win2MqttBackgroundService.cs` | 24 |

## Execution Flows

| Flow | Type | Steps |
|------|------|-------|
| `GetEnabledSensors → SystemSensorMetadata` | cross_community | 5 |
| `GetEnabledSensors → GetUniqueId` | cross_community | 5 |
| `GetEnabledSensors → GetSensorStateTopic` | cross_community | 5 |
| `GetEnabledSensors → GetStandardSensorDiscoveryTopic` | cross_community | 5 |
| `GetEnabledActions → GetUniqueId` | intra_community | 3 |
| `GetEnabledActions → GetActionResponseDiscoveryTopic` | cross_community | 3 |
| `GetEnabledActions → GetActionCommandTopic` | intra_community | 3 |
| `GetEnabledActions → GetActionStateTopic` | intra_community | 3 |

## Connected Areas

| Area | Connections |
|------|-------------|
| Samqtt.HomeAssistant | 1 calls |

## How to Explore

1. `gitnexus_context({name: "MetadataBase"})` — see callers and callees
2. `gitnexus_query({query: "samqtt.common"})` — find related execution flows
3. Read key files listed above for implementation details
