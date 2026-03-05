using AutoMapper;
using AutoMapper.QueryableExtensions;
using Liberty.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace Liberty.Pagination.Extensions;

/// <summary>
/// Provides extension methods for working with queryable data sources to enable paginated queries and projection to DTOs.
/// </summary>
public static class QueryableExtension
{
    /// <summary>
    /// Applies pagination, optional sorting, and projection to a DTO for the given queryable entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity source.</typeparam>
    /// <typeparam name="TDto">The type of the DTO projection.</typeparam>
    /// <param name="query">The queryable entity source.</param>
    /// <param name="pageable">Pagination and sorting configuration.</param>
    /// <param name="mapper">The object mapper used to project the entity to the DTO.</param>
    /// <param name="isApplySort">Indicates if sorting should be applied.</param>
    /// <param name="cancellationToken">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation, which returns a page object containing the projected DTOs.</returns>
    public static async Task<IPage<TDto>> UsePageableAsDtoAsync<TEntity, TDto>(
        this IQueryable<TEntity> query,
        IPageable pageable,
        IMapper mapper,
        bool isApplySort = false,
        CancellationToken cancellationToken = default
    ) where TEntity : class where TDto : class
    {
        if (typeof(TEntity).IsAssignableTo(typeof(EntityData)) && pageable.IsEnabled is not null)
        {
            var entityDataQueryable = query.Cast<EntityData>()
                .Where(x => x.IsEnabled == pageable.IsEnabled);

            query = entityDataQueryable.Cast<TEntity>();
        }

        var queryable = query
            .Skip(pageable.Offset)
            .Take(Math.Min(pageable.PageSize, 2000))
            .AsNoTracking()
            .ProjectTo<TDto>(mapper.ConfigurationProvider);

        if (isApplySort)
        {
            queryable = queryable.ApplySort(pageable.Sort);
        }

        return new Page<TDto>(
            await queryable.ToListAsync(cancellationToken),
            pageable,
            await query.CountAsync(cancellationToken)
        );
    }

    /// <summary>
    /// Asynchronously applies pagination, sorting, and optional filtering to an <see cref="IQueryable{T}"/> sequence based on the provided pageable configuration.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of elements in the queryable sequence. Must be a reference type.
    /// </typeparam>
    /// <param name="query">
    /// The queryable sequence to paginate.
    /// </param>
    /// <param name="pageable">
    /// The object containing the pagination and sorting configuration.
    /// </param>
    /// <param name="isApplySort">
    /// A boolean value indicating whether sorting should be applied based on the pageable configuration. Default is false.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the operation to complete. Default is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a page of items of type <typeparamref name="TEntity"/> that matches the specified pageable configuration.
    /// </returns>
    public static async Task<IPage<TEntity>> UsePageableAsync<TEntity>(
        this IQueryable<TEntity> query,
        IPageable pageable,
        bool isApplySort = false,
        CancellationToken cancellationToken = default
    ) where TEntity : class
    {
        var queryable = query.AsNoTracking();

        if (typeof(TEntity).IsAssignableTo(typeof(EntityData)) && pageable.IsEnabled is not null)
        {
            var entityDataQueryable = queryable.Cast<EntityData>()
                .Where(x => x.IsEnabled == pageable.IsEnabled);

            queryable = entityDataQueryable.Cast<TEntity>();
        }

        if (pageable.IsPaged)
        {
            queryable = queryable
                .Skip(pageable.Offset)
                .Take(Math.Min(pageable.PageSize, 2000));
        }

        if (isApplySort)
        {
            queryable = queryable.ApplySort(pageable.Sort);
        }

        var content = await queryable.ToListAsync(cancellationToken);
        var total = await query.CountAsync(cancellationToken);

        return new Page<TEntity>(
            content,
            pageable,
            total
        );
    }

    /// <summary>
    /// Paginates a queryable collection of entities based on the specified pageable options
    /// and optionally applies sorting.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity in the queryable collection.</typeparam>
    /// <param name="query">The queryable collection of entities to paginate.</param>
    /// <param name="pageable">
    /// An object containing pagination details such as page number, page size, sorting,
    /// and filtering options.
    /// </param>
    /// <param name="isApplySort">
    /// Determines whether sorting is applied to the queryable collection.
    /// The default value is <c>false</c>.
    /// </param>
    /// <returns>
    /// A paginated result set of the type <typeparamref name="TEntity"/> that
    /// implements <see cref="IPage{TEntity}"/>, containing the data, pagination metadata,
    /// and total item count.
    /// </returns>
    public static IPage<TEntity> UsePageable<TEntity>(
        this IQueryable<TEntity> query,
        IPageable pageable,
        bool isApplySort = false
    ) where TEntity : class
    {
        if (typeof(TEntity).IsAssignableTo(typeof(EntityData)) && pageable.IsEnabled is not null)
        {
            var entityDataQueryable = query.Cast<EntityData>()
                .Where(x => x.IsEnabled == pageable.IsEnabled);

            query = entityDataQueryable.Cast<TEntity>();
        }

        var queryable = query
            .Skip(pageable.Offset)
            .Take(Math.Min(pageable.PageSize, 2000));

        if (isApplySort)
        {
            queryable = queryable.ApplySort(pageable.Sort);
        }

        return new Page<TEntity>(
            [.. queryable],
            pageable,
            query.Count()
        );
    }

    /// <summary>
    /// Applies sorting to the given query based on the specified sort criteria.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entities in the query.</typeparam>
    /// <param name="query">The queryable data collection to which sorting will be applied.</param>
    /// <param name="sort">The sorting configuration, containing property names and their respective sort directions.</param>
    /// <returns>A new queryable collection with the applied sort order, or the original collection if no sorting is specified.</returns>
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
            var propertyInfo = Array.Find(
                properties,
                pi => pi.Name.Equals(
                    order.Property,
                    StringComparison.InvariantCultureIgnoreCase
                )
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

    /// <summary>
    /// Creates a lambda expression required to access a property of type <typeparamref name="TEntity"/>
    /// with the specified property information.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity containing the property.</typeparam>
    /// <typeparam name="TKey">The type of the property to access.</typeparam>
    /// <param name="propertyInfo">The metadata of the property for which the expression is needed.</param>
    /// <returns>A strongly typed lambda expression that accesses the specified property of the entity.</returns>
    private static Expression<Func<TEntity, TKey>> GetExpression<TEntity, TKey>(
        PropertyInfo propertyInfo
    )
    {
        var parameterExpression = Expression.Parameter(typeof(TEntity), "x");
        return Expression.Lambda<Func<TEntity, TKey>>(
            Expression.Convert(Expression.Property(parameterExpression, propertyInfo), typeof(object)),
            parameterExpression
        );
    }
}
