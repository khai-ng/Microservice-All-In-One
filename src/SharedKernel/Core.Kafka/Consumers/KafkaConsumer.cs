﻿using Confluent.Kafka;
using Core.AspNet.Extensions;
using Core.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Core.Kafka.Consumers
{
    public class KafkaConsumer : IntegrationConsumer
    {
        private readonly ConsumerConfig _consumerConfig;
        private readonly IMediator _publisher;
        private readonly ILogger _logger;

        public KafkaConsumer(IConfiguration configuration, IMediator publisher, ILogger logger)
        {
            _consumerConfig = configuration.GetRequiredConfig<ConsumerConfig>("Kafka:ConsumerConfig");
            _publisher = publisher;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var consumer = new ConsumerBuilder<string, string>(_consumerConfig).Build();
            consumer.Subscribe("my-topic");
            var cancelToken = new CancellationTokenSource();
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    //GH issue: https://github.com/dotnet/extensions/issues/2149#issuecomment-518709751
                    await Task.Yield();
                    var cr = consumer.Consume(cancelToken.Token);
                    var evnentMsg = cr.ToEvent();
                    ArgumentNullException.ThrowIfNull(evnentMsg);

                    await _publisher.Publish(evnentMsg, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.Warning("OperationCanceledException");
                    break;
                }
                catch (ConsumeException e)
                {
                    // Consumer errors should generally be ignored (or logged) unless fatal.
                    _logger.Error($"Consume error: {e.Error.Reason}");

                    if (e.Error.IsFatal)
                    {
                        // https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
                        _logger.Fatal($"Consume fatal: {e.Error.Reason}");
                        break;
                    }
                }
                catch (Exception e)
                {
                    _logger.Error($"Unexpected error: {e}");
                    break;
                }

            }
        }

    }

}
