﻿using Core.Autofac;
using Core.Events.EventStore;
using Core.Result.AppResults;
using Core.SharedKernel;
using Marten.Events;
using MediatR;
using Ordering.API.Application.Abstractions;
using Ordering.API.Domain.OrderAggregate;
using Ordering.API.Domain.OrderAgrregate;

namespace Ordering.API.Application.Services
{
    public class CreateOrder : IRequestHandler<CreateOrderRequest, AppResult<Guid>>, ITransient
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventStoreRepository<Order> _eventStoreRepository;

        public CreateOrder(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IEventStoreRepository<Order> eventStoreRepository)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _eventStoreRepository = eventStoreRepository;
        }

        public async Task<AppResult<Guid>> Handle(CreateOrderRequest request, CancellationToken ct)
        {
            var orderId = Guid.NewGuid();
            var address = new Address(request.Country, request.City, request.District, request.Street);
            var orderItems = request.OrderItems.Select(x => new OrderItem(orderId, x.ProductId, x.UnitPrice, x.Unit));
            var order = new Order(request.BuyerId, request.PaymentId, address, orderItems);
            _orderRepository.Add(order);

            await _eventStoreRepository.Add(order.Id, order, ct).ConfigureAwait(false);
            await _unitOfWork.SaveChangesAsync(ct).ConfigureAwait(false);

            return AppResult.Success(order.Id);
        }
    }
}