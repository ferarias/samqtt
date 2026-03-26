---
name: samqtt-application
description: "Skill for the Samqtt.Application area of samqtt. 19 symbols across 6 files."
---

# Samqtt.Application

19 symbols | 6 files | Cohesion: 86%

## When to Use

- Working with code in `src/`
- Understanding how SystemSensorFactory, SystemActionFactory, Sanitize work
- Modifying samqtt.application-related functionality

## Key Files

| File | Symbols |
|------|---------|
| `src/Samqtt.Application/TopicProvider.cs` | GetUniqueId, GetSensorStateTopic, GetActionStateTopic, GetActionJsonAttributesTopic, GetActionCommandTopic (+4) |
| `src/Samqtt.Application/SystemSensorFactory.cs` | GetEnabledSensors, GetEnabledSimpleSensors, GetEnabledMultiSensors, GetEnabledMultiSensorChildren, SystemSensorFactory |
| `src/Samqtt.Common/SanitizeHelpers.cs` | SanitizationRegex, Sanitize |
| `src/Samqtt.Common/SystemSensors/ISystemSensorFactory.cs` | ISystemSensorFactory |
| `src/Samqtt.Application/SystemActionFactory.cs` | SystemActionFactory |
| `src/Samqtt.Common/SystemActions/ISystemActionFactory.cs` | ISystemActionFactory |

## Entry Points

Start here when exploring this area:

- **`SystemSensorFactory`** (Class) — `src/Samqtt.Application/SystemSensorFactory.cs:9`
- **`SystemActionFactory`** (Class) — `src/Samqtt.Application/SystemActionFactory.cs:9`
- **`Sanitize`** (Method) — `src/Samqtt.Common/SanitizeHelpers.cs:9`
- **`GetUniqueId`** (Method) — `src/Samqtt.Application/TopicProvider.cs:53`
- **`GetSensorStateTopic`** (Method) — `src/Samqtt.Application/TopicProvider.cs:60`

## Key Symbols

| Symbol | Type | File | Line |
|--------|------|------|------|
| `SystemSensorFactory` | Class | `src/Samqtt.Application/SystemSensorFactory.cs` | 9 |
| `SystemActionFactory` | Class | `src/Samqtt.Application/SystemActionFactory.cs` | 9 |
| `ISystemSensorFactory` | Interface | `src/Samqtt.Common/SystemSensors/ISystemSensorFactory.cs` | 4 |
| `ISystemActionFactory` | Interface | `src/Samqtt.Common/SystemActions/ISystemActionFactory.cs` | 4 |
| `Sanitize` | Method | `src/Samqtt.Common/SanitizeHelpers.cs` | 9 |
| `GetUniqueId` | Method | `src/Samqtt.Application/TopicProvider.cs` | 53 |
| `GetSensorStateTopic` | Method | `src/Samqtt.Application/TopicProvider.cs` | 60 |
| `GetActionStateTopic` | Method | `src/Samqtt.Application/TopicProvider.cs` | 67 |
| `GetActionJsonAttributesTopic` | Method | `src/Samqtt.Application/TopicProvider.cs` | 68 |
| `GetActionCommandTopic` | Method | `src/Samqtt.Application/TopicProvider.cs` | 75 |
| `GetBinarySensorDiscoveryTopic` | Method | `src/Samqtt.Application/TopicProvider.cs` | 82 |
| `GetStandardSensorDiscoveryTopic` | Method | `src/Samqtt.Application/TopicProvider.cs` | 89 |
| `GetActionResponseDiscoveryTopic` | Method | `src/Samqtt.Application/TopicProvider.cs` | 97 |
| `GetButtonDiscoveryTopic` | Method | `src/Samqtt.Application/TopicProvider.cs` | 98 |
| `GetEnabledSensors` | Method | `src/Samqtt.Application/SystemSensorFactory.cs` | 17 |
| `SanitizationRegex` | Method | `src/Samqtt.Common/SanitizeHelpers.cs` | 6 |
| `GetEnabledSimpleSensors` | Method | `src/Samqtt.Application/SystemSensorFactory.cs` | 29 |
| `GetEnabledMultiSensors` | Method | `src/Samqtt.Application/SystemSensorFactory.cs` | 56 |
| `GetEnabledMultiSensorChildren` | Method | `src/Samqtt.Application/SystemSensorFactory.cs` | 82 |

## Execution Flows

| Flow | Type | Steps |
|------|------|-------|
| `GetEnabledSensors → SanitizationRegex` | cross_community | 5 |
| `GetEnabledSensors → SystemSensorMetadata` | cross_community | 5 |
| `GetEnabledSensors → GetUniqueId` | cross_community | 5 |
| `GetEnabledSensors → GetSensorStateTopic` | cross_community | 5 |
| `GetEnabledSensors → GetStandardSensorDiscoveryTopic` | cross_community | 5 |
| `GetUniqueId → SanitizationRegex` | intra_community | 3 |
| `GetSensorStateTopic → SanitizationRegex` | intra_community | 3 |
| `GetActionStateTopic → SanitizationRegex` | intra_community | 3 |
| `GetActionJsonAttributesTopic → SanitizationRegex` | intra_community | 3 |
| `GetActionCommandTopic → SanitizationRegex` | intra_community | 3 |

## Connected Areas

| Area | Connections |
|------|-------------|
| Samqtt.Common | 2 calls |

## How to Explore

1. `gitnexus_context({name: "SystemSensorFactory"})` — see callers and callees
2. `gitnexus_query({query: "samqtt.application"})` — find related execution flows
3. Read key files listed above for implementation details
