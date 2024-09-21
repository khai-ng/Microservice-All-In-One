﻿using MediatR;

namespace Core.SharedKernel
{
    public interface IDomainEvent<TKey> : INotification
    {
        public TKey Id { get; }
        public long Version { get; }
    }
}
