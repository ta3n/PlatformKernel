using PlatformKernel.ApplicationShared.Cqrs.BaseQuery;

namespace PlatformKernel.Service.Base.Application.Cqrs.BaseQueries;

/// <summary>
/// Defines a query handler for paged query operations.
/// This handler processes queries that implement <see cref="IQueryPagedBase{TResponse}"/>
/// and returns a paged collection of responses.
/// </summary>
/// <typeparam name="TQuery">The type of the query being handled. Must implement <see cref="IQueryPagedBase{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the query.</typeparam>
public interface IQueryPagedBaseHandler<in TQuery, TResponse>
    : IQueryBaseHandler<TQuery, IEnumerable<TResponse>>
    where TQuery : IQueryPagedBase<TResponse>, IQueryBase<IEnumerable<TResponse>>;
