﻿using Core.IntegrationEvents.IntegrationEvents;
using Ordering.API.Application.Dto.Order;

namespace Ordering.API.Application.IntegrationEvents
{
    public class OrderConfirmStockIntegrationEvent(
        Guid orderId,
        IEnumerable<ProductUnit> productUnits) : IntegrationEvent
    {
        public Guid OrderId { get; } = orderId;
        public IEnumerable<ProductUnit> ProductUnits { get; } = productUnits;
    }
}
