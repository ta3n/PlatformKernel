using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace Liberty.Pagination.Extensions;

public static class QueryableExtension
{
    public static async Task<IPage<TDto>> UsePageableAsDtoAsync<TEntity, TDto>(
        this IQueryable<TEntity> query,
        IPageable pageable,
        IMapper mapper,
        bool isApplySort = false,
        CancellationToken cancellationToken = default
    ) where TEntity : class where TDto : class
    {
        var queryable = query
            .Skip(pageable.Offset)
            .Take(Math.Min(pageable.PageSize, 2000))
            .AsNoTracking()
            .Select(entity => mapper.Map<TDto>(entity));

        if (isApplySort)
        {
            queryable = queryable.ApplySort(pageable.Sort);
        }

        return new Page<TDto>(
            await queryable.ToListAsync(cancellationToken: cancellationToken),
            pageable,
            query.Count()
        );
    }

    public static async Task<IPage<TEntity>> UsePageableAsync<TEntity>(
        this IQueryable<TEntity> query,
        IPageable pageable,
        bool isApplySort = false,
        CancellationToken cancellationToken = default
    ) where TEntity : class
    {
        var queryable = query
            .Skip(pageable.Offset)
            .Take(Math.Min(pageable.PageSize, 2000))
            .AsNoTracking();

        if (isApplySort)
        {
            queryable = queryable.ApplySort(pageable.Sort);
        }

        return new Page<TEntity>(
            await queryable.ToListAsync(cancellationToken: cancellationToken),
            pageable,
            await query.CountAsync(cancellationToken: cancellationToken)
        );
    }

    public static IPage<TEntity> UsePageable<TEntity>(
        this IQueryable<TEntity> query,
        IPageable pageable,
        bool isApplySort = false
    ) where TEntity : class
    {
        var queryable = query
            .Skip(pageable.Offset)
            .Take(Math.Min(pageable.PageSize, 2000));

        if (isApplySort)
        {
            queryable = queryable.ApplySort(pageable.Sort);
        }

        return new Page<TEntity>(
            queryable.ToList(),
            pageable,
            query.Count()
        );
    }

    private static IQueryable<TEntity> ApplySort<TEntity>(
        this IQueryable<TEntity> query,
        Sort? sort
    )
    {
        if (!query.Any() || sort is null || sort.IsUnsorted())
        {
            return query;
        }

        var sortExpressions = new SortExpressions<TEntity, object>();
        var properties = typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var order in sort.Orders)
        {
            if (order is not { Property: not null })
            {
                continue;
            }

            var isDescending = order.Direction.IsDescending();
            var propertyInfo = properties.FirstOrDefault(
                pi => pi.Name.Equals(order.Property, StringComparison.InvariantCultureIgnoreCase)
            );
            if (propertyInfo == null)
            {
                continue;
            }

            var expression = GetExpression<TEntity, object>(propertyInfo);
            sortExpressions.Add(expression, isDescending);
        }

        return SortExpressions<TEntity, object>.ApplySorts(query, sortExpressions);
    }

    private static Expression<Func<TEntity, TKey>> GetExpression<TEntity, TKey>(
        PropertyInfo propertyInfo
    )
    {
        var parameterExpression = Expression.Parameter(typeof(TEntity), "x");
        return Expression.Lambda<Func<TEntity, TKey>>(
            Expression.Convert(Expression.Property(parameterExpression, propertyInfo), typeof(object)),
            [
                parameterExpression
            ]
        );
    }
}
