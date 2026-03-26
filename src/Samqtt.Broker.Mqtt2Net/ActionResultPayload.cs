using System.Collections.Generic;

namespace Samqtt.Broker.MQTTNet
{
    internal sealed record ActionResultPayload(int Count, IReadOnlyList<string> Items);
}
