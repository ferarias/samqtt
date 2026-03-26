namespace Samqtt.Broker.Tcp;

internal sealed record ActionResultPayload(int Count, IReadOnlyList<string> Items);
