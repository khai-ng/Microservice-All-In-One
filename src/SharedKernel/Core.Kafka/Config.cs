﻿using Core.IntegrationEvents.IntegrationEvents;
using Core.Kafka.Consumers;
using Core.Kafka.OpenTelemetry;
using Core.Kafka.Producers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Core.Kafka
{
    public static class Config
    {
        public static IServiceCollection AddKafkaProducer(this IServiceCollection services)
        {
            services.TryAddScoped<IKafkaProducer, KafkaProducer>();
            return services;
        }

        public static IServiceCollection AddKafkaConsumer(this IServiceCollection services) 
        {
            services.AddSingleton<IEventBus, EventBus>();
            services.AddHostedService<KafkaConsumer>();
            return services;
        }

        /// <summary>
        /// Add <see cref="AddKafkaProducer"/>, <seealso cref="AddKafkaConsumer"/>
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddKafkaCompose(this IServiceCollection services)
            => services
                .AddKafkaProducer()
                .AddKafkaConsumer();

        public static WebApplicationBuilder AddKafkaOpenTelemetry(this WebApplicationBuilder builder, string? appName = null)
        {
            builder.Services.AddOpenTelemetry()
                .WithTracing(tracing =>
                {
                    tracing.AddConfluentKafkaInstrumentation();
                });

            return builder;
        }
    }
}
