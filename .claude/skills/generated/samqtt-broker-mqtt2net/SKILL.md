---
name: samqtt-broker-mqtt2net
description: "Skill for the Samqtt.Broker.Mqtt2Net area of samqtt. 3 symbols across 1 files."
---

# Samqtt.Broker.Mqtt2Net

3 symbols | 1 files | Cohesion: 100%

## When to Use

- Working with code in `src/`
- Understanding how DisconnectAsync, DisposeAsync work
- Modifying samqtt.broker.mqtt2net-related functionality

## Key Files

| File | Symbols |
|------|---------|
| `src/Samqtt.Broker.Mqtt2Net/MqttConnector.cs` | DisconnectAsync, DisposeAsync, DisposeAsyncCore |

## Entry Points

Start here when exploring this area:

- **`DisconnectAsync`** (Method) — `src/Samqtt.Broker.Mqtt2Net/MqttConnector.cs:65`
- **`DisposeAsync`** (Method) — `src/Samqtt.Broker.Mqtt2Net/MqttConnector.cs:83`

## Key Symbols

| Symbol | Type | File | Line |
|--------|------|------|------|
| `DisconnectAsync` | Method | `src/Samqtt.Broker.Mqtt2Net/MqttConnector.cs` | 65 |
| `DisposeAsync` | Method | `src/Samqtt.Broker.Mqtt2Net/MqttConnector.cs` | 83 |
| `DisposeAsyncCore` | Method | `src/Samqtt.Broker.Mqtt2Net/MqttConnector.cs` | 89 |

## Execution Flows

| Flow | Type | Steps |
|------|------|-------|
| `DisposeAsync → DisconnectAsync` | intra_community | 3 |

## How to Explore

1. `gitnexus_context({name: "DisconnectAsync"})` — see callers and callees
2. `gitnexus_query({query: "samqtt.broker.mqtt2net"})` — find related execution flows
3. Read key files listed above for implementation details
