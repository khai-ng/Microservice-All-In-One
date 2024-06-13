﻿using Core.SharedKernel;

namespace Core.EntityFramework.Repository
{
    public interface IRepository<TModel> : IRepository<TModel, Ulid>
        where TModel : AggregateRoot<Ulid>
    { }

    public interface IRepository<TModel, TKey> :
        IQueryRepository<TModel, TKey>,
        ICommandRepository<TModel, TKey>
        where TModel : AggregateRoot<TKey>
    { }
}