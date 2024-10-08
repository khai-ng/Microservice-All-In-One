﻿using Confluent.Kafka;
using Core.IntegrationEvents.IntegrationEvents;
using Core.Reflections;
using Newtonsoft.Json;

namespace Core.Kafka.Producers
{
    internal static class EventExtensions
    {
        internal static IntegrationEvent? ToEvent(this ConsumeResult<string, string> consumeResult)
        {
            var eventType = TypeProvider.GetTypeFromReferenceAssembly(consumeResult.Message.Key);

            if (eventType == null)
                return null;

            return JsonConvert.DeserializeObject(consumeResult.Message.Value, eventType) as IntegrationEvent;
        }
    }
}
