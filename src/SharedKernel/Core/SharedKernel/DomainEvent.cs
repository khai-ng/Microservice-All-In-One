﻿namespace Core.SharedKernel
{

    public abstract class DomainEvent<TKey> : IDomainEvent<TKey>
    {
        protected DomainEvent(TKey id) => Id = id;
        public TKey Id { get; protected set; }
        public long Version { get; protected set; }
    }
}
