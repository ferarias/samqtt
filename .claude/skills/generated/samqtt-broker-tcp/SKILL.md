---
name: samqtt-broker-tcp
description: "Skill for the Samqtt.Broker.Tcp area of samqtt. 27 symbols across 4 files."
---

# Samqtt.Broker.Tcp

27 symbols | 4 files | Cohesion: 92%

## When to Use

- Working with code in `src/`
- Understanding how PublishAsync, SubscribeAsync, UnsubscribeAsync work
- Modifying samqtt.broker.tcp-related functionality

## Key Files

| File | Symbols |
|------|---------|
| `src/Samqtt.Broker.Tcp/MqttTcpClient.cs` | PublishAsync, SubscribeAsync, UnsubscribeAsync, DisconnectAsync, SendConnectAsync (+16) |
| `src/Samqtt.Broker.Tcp/MqttConnector.cs` | DisconnectAsync, DisposeAsync, DisposeAsyncCore, ConnectAsync |
| `src/Samqtt.Broker.Tcp/MqttSubscriber.cs` | SubscribeAsync |
| `src/Samqtt.Broker.Tcp/MqttPublisher.cs` | PublishAsync |

## Entry Points

Start here when exploring this area:

- **`PublishAsync`** (Method) — `src/Samqtt.Broker.Tcp/MqttTcpClient.cs:54`
- **`SubscribeAsync`** (Method) — `src/Samqtt.Broker.Tcp/MqttTcpClient.cs:83`
- **`UnsubscribeAsync`** (Method) — `src/Samqtt.Broker.Tcp/MqttTcpClient.cs:99`
- **`DisconnectAsync`** (Method) — `src/Samqtt.Broker.Tcp/MqttTcpClient.cs:105`
- **`DisposeAsync`** (Method) — `src/Samqtt.Broker.Tcp/MqttTcpClient.cs:385`

## Key Symbols

| Symbol | Type | File | Line |
|--------|------|------|------|
| `PublishAsync` | Method | `src/Samqtt.Broker.Tcp/MqttTcpClient.cs` | 54 |
| `SubscribeAsync` | Method | `src/Samqtt.Broker.Tcp/MqttTcpClient.cs` | 83 |
| `UnsubscribeAsync` | Method | `src/Samqtt.Broker.Tcp/MqttTcpClient.cs` | 99 |
| `DisconnectAsync` | Method | `src/Samqtt.Broker.Tcp/MqttTcpClient.cs` | 105 |
| `DisposeAsync` | Method | `src/Samqtt.Broker.Tcp/MqttTcpClient.cs` | 385 |
| `SubscribeAsync` | Method | `src/Samqtt.Broker.Tcp/MqttSubscriber.cs` | 74 |
| `PublishAsync` | Method | `src/Samqtt.Broker.Tcp/MqttPublisher.cs` | 8 |
| `DisconnectAsync` | Method | `src/Samqtt.Broker.Tcp/MqttConnector.cs` | 52 |
| `DisposeAsync` | Method | `src/Samqtt.Broker.Tcp/MqttConnector.cs` | 64 |
| `ConnectAsync` | Method | `src/Samqtt.Broker.Tcp/MqttTcpClient.cs` | 23 |
| `ConnectAsync` | Method | `src/Samqtt.Broker.Tcp/MqttConnector.cs` | 17 |
| `SendConnectAsync` | Method | `src/Samqtt.Broker.Tcp/MqttTcpClient.cs` | 207 |
| `WritePacketAsync` | Method | `src/Samqtt.Broker.Tcp/MqttTcpClient.cs` | 288 |
| `BuildPublish` | Method | `src/Samqtt.Broker.Tcp/MqttTcpClient.cs` | 302 |
| `BuildSubscribe` | Method | `src/Samqtt.Broker.Tcp/MqttTcpClient.cs` | 322 |
| `BuildUnsubscribe` | Method | `src/Samqtt.Broker.Tcp/MqttTcpClient.cs` | 339 |
| `WriteString` | Method | `src/Samqtt.Broker.Tcp/MqttTcpClient.cs` | 355 |
| `WriteVarint` | Method | `src/Samqtt.Broker.Tcp/MqttTcpClient.cs` | 363 |
| `NextPacketId` | Method | `src/Samqtt.Broker.Tcp/MqttTcpClient.cs` | 377 |
| `DisposeAsyncCore` | Method | `src/Samqtt.Broker.Tcp/MqttConnector.cs` | 70 |

## Execution Flows

| Flow | Type | Steps |
|------|------|-------|
| `DisposeAsync → WriteVarint` | intra_community | 6 |
| `ConnectAsync → ReadByteAsync` | intra_community | 5 |
| `ConnectAsync → ReadUInt16` | intra_community | 5 |
| `ConnectAsync → WritePacketAsync` | cross_community | 5 |
| `DisposeAsync → WritePacketAsync` | intra_community | 5 |
| `DisposeAsync → NextPacketId` | intra_community | 5 |
| `SubscribeAsync → WriteVarint` | intra_community | 4 |
| `PublishAsync → WriteVarint` | intra_community | 4 |
| `ConnectAsync → WriteString` | cross_community | 4 |
| `ConnectAsync → WriteVarint` | cross_community | 4 |

## How to Explore

1. `gitnexus_context({name: "PublishAsync"})` — see callers and callees
2. `gitnexus_query({query: "samqtt.broker.tcp"})` — find related execution flows
3. Read key files listed above for implementation details
