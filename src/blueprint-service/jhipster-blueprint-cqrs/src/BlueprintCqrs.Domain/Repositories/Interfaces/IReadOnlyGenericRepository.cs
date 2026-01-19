using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JHipsterNet.Core.Pagination;
using BlueprintCqrs.Domain.Entities;

namespace BlueprintCqrs.Domain.Repositories.Interfaces;

public interface IReadOnlyGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
{
    Task<TEntity> GetOneAsync(
        TKey id
    );

    Task<IEnumerable<TEntity>> GetAllAsync();

    Task<IPage<TEntity>> GetPageAsync(
        IPageable pageable
    );

    Task<bool> Exists(
        Expression<Func<TEntity, bool>> predicate
    );

    Task<int> CountAsync();
    IFluentRepository<TEntity> QueryHelper();
}
