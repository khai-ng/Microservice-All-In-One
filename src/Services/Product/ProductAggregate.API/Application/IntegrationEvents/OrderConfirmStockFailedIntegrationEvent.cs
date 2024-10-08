﻿using Core.IntegrationEvents.IntegrationEvents;

namespace ProductAggregate.API.Application.IntegrationEvents
{
    public class OrderConfirmStockFailedIntegrationEvent(Guid orderId, string reason) : IntegrationEvent
    {
        public Guid OrderId { get; set; } = orderId;
        public string Reason { get; set; } = reason;
    }
}
