﻿using Core.Autofac;
using Core.Contract;
using Core.IntegrationEvents;
using Serilog;
using System.Text.Json;

namespace EmployeeMgt.Application.IntergrationEvents.EventHandlers
{
    public sealed class HelloEventHandler : IIntegrationEventHandler<HelloEvent>, ITransient
    {
        private readonly ILogger _logger;
        public HelloEventHandler(ILogger logger)
        {
            _logger = logger;
        }

        public Task Handle(HelloEvent notification, CancellationToken cancellationToken)
        {
            return Task.Run(() => { _logger.Information(JsonSerializer.Serialize(notification)); }); 
        }
    }
}
