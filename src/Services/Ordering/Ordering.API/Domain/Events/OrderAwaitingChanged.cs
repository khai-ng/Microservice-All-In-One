﻿using Core.EntityFramework.ServiceDefault;
using System.Text.Json.Serialization;

namespace Ordering.API.Domain.Events
{
    public class OrderAwaitingChanged : DomainEvent
    {
        [JsonConstructor]
        private OrderAwaitingChanged() { }
        public OrderAwaitingChanged(Guid orderId) : base(orderId) { }

    }
}
