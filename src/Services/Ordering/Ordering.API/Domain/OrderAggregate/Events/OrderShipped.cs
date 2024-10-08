﻿using Core.EntityFramework.ServiceDefault;
using System.Text.Json.Serialization;

namespace Ordering.API.Domain.OrderAggregate.Events
{
    public class OrderShipped : DomainEvent
    {
        [JsonConstructor]
        private OrderShipped() { }
        public OrderShipped(Guid orderId) : base(orderId) { }
    }
}
