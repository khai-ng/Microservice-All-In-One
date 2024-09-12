﻿using System.Diagnostics;
using System.Text;
using Confluent.Kafka;

namespace Core.Kafka.OpenTelemetry;

internal static class ActivitySourceAccessor
{
    internal const string ActivitySourceName = "Confluent.Kafka.Diagnostics";
    private const string TraceParentHeaderName = "traceparent";
    private const string TraceStateHeaderName = "tracestate";

    private static ActivitySource ActivitySource { get; } = new(ActivitySourceName);

    internal static Activity? StartProduceActivity<TKey, TValue>(TopicPartition partition,
        Message<TKey, TValue> message)
        => StartProduceActivity(partition.Topic, message);

    internal static Activity? StartProduceActivity<TKey, TValue>(string topic,
        Message<TKey, TValue> message)
    {
        try
        {
            Activity? activity = ActivitySource.StartActivity(
                $"{topic} publish", ActivityKind.Producer,
                default(ActivityContext), ProducerActivityTags(topic));

            if (activity == null)
                return null;

            if (activity.IsAllDataRequested)
                SetKeyActivityTags(activity, message);

            if (message.Headers == null)
                message.Headers = [];

            if (activity.Id != null)
                message.Headers.Add(TraceParentHeaderName, Encoding.UTF8.GetBytes(activity.Id));

            var tracestateStr = activity.Context.TraceState;
            if (tracestateStr?.Length > 0)
                message.Headers.Add(TraceStateHeaderName, Encoding.UTF8.GetBytes(tracestateStr));

            return activity;
        }
        catch
        {
            return null;
        }
    }

    internal static Activity? StartConsumeActivity<TKey, TValue>(ConsumeResult<TKey, TValue> consumerResult,
        string memberId)
    {
        try
        {
            var message = consumerResult.Message;
            var activity = ActivitySource.CreateActivity(
                $"{consumerResult.Topic} process", ActivityKind.Consumer,
                default(ActivityContext), ConsumerActivityTags(consumerResult, memberId));

            if (activity != null)
            {
                var traceParentHeader = message.Headers?.FirstOrDefault(x => x.Key == TraceParentHeaderName);
                var traceStateHeader = message.Headers?.FirstOrDefault(x => x.Key == TraceStateHeaderName);

                var traceParent = traceParentHeader != null
                    ? Encoding.UTF8.GetString(traceParentHeader.GetValueBytes())
                    : null;
                var traceState = traceStateHeader != null
                    ? Encoding.UTF8.GetString(traceStateHeader.GetValueBytes())
                    : null;

                if (ActivityContext.TryParse(traceParent, traceState, out var activityContext))
                {
                    activity.SetParentId(activityContext.TraceId, activityContext.SpanId, activityContext.TraceFlags);
                    activity.TraceStateString = activityContext.TraceState;
                }

                if (activity.IsAllDataRequested)
                    SetKeyActivityTags(activity, message);

                activity.Start();
            }

            return activity;
        }
        catch
        {
            return null;
        }
    }

    internal static void UpdateActivityTags<TKey, TValue>(DeliveryResult<TKey, TValue> deliveryResult, Activity activity)
    {
        try
        {
            var activityStatus = deliveryResult.Status switch
            {
                PersistenceStatus.Persisted => ActivityStatusCode.Ok,
                PersistenceStatus.PossiblyPersisted => ActivityStatusCode.Ok,
                PersistenceStatus.NotPersisted => ActivityStatusCode.Error,
                _ => ActivityStatusCode.Unset
            };

            activity.SetStatus(activityStatus);
            if (activityStatus == ActivityStatusCode.Ok)
            {
                activity.SetTag("messaging.kafka.destination.partition", deliveryResult.Partition.Value.ToString());
                activity.SetTag("messaging.kafka.message.offset", deliveryResult.Offset.Value.ToString());
            }
        }
        catch
        {
        }
    }

    private static void SetKeyActivityTags<TKey, TValue>(Activity activity, Message<TKey, TValue> message)
    {
        if (message.Key != null)
            activity.SetTag("messaging.kafka.message.key", message.Key.ToString());
    }

    private static IEnumerable<KeyValuePair<string, object?>> ProducerActivityTags(string topic)
    {
        var list = OperationActivityTags("publish");

        list.Add(new KeyValuePair<string, object?>("messaging.destination.kind", "topic"));
        list.Add(new KeyValuePair<string, object?>("messaging.destination.name", topic));

        return list;
    }

    private static IEnumerable<KeyValuePair<string, object?>> ConsumerActivityTags<TKey, TValue>(
        ConsumeResult<TKey, TValue> consumerResult, string memberId)
    {
        var list = OperationActivityTags("process");

        // messaging.consumer.id - For Kafka, set it to {messaging.kafka.consumer.group} - {messaging.kafka.client_id},
        // if both are present, or only messaging.kafka.consumer.group
        list.Add(new("messaging.source.kind", "topic"));
        list.Add(new("messaging.source.name", consumerResult.Topic));
        list.Add(new("messaging.kafka.source.partition", consumerResult.Partition.Value.ToString()));
        list.Add(new("messaging.kafka.message.offset", consumerResult.Offset.Value.ToString()));
        list.Add(new("messaging.kafka.client_id", memberId));

        // messaging.kafka.consumer.group - there is no way to access this information from the consumer

        return list;
    }

    private static IList<KeyValuePair<string, object?>> OperationActivityTags(string operation)
    {
        return new List<KeyValuePair<string, object?>>()
        {
            new("messaging.system", "kafka"), new("messaging.operation", operation)
        };
    }
}