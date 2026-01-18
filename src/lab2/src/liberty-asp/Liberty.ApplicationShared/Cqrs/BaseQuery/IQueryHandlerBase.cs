using MediatR;
using Microsoft.AspNetCore.Http;

namespace Liberty.ApplicationShared.Cqrs.BaseQuery;

/// <summary>
/// Interface representing a base handler for queries in a CQRS pattern.
/// This handler is responsible for processing a query and returning a response
/// along with any associated headers.
/// </summary>
/// <typeparam name="TQuery">
/// The type of the query being handled. It must implement <see cref="IQueryBase{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response returned by the handled query.
/// </typeparam>
public interface IQueryBaseHandler<in TQuery, TResponse>
    : IRequestHandler<TQuery, (IHeaderDictionary, TResponse)>
    where TQuery : IQueryBase<TResponse>;

/// <summary>
/// Defines a query handler interface for processing single-result queries and responding
/// with both the response data and associated headers.
/// </summary>
/// <typeparam name="TQuery">
/// Represents the type of the query object. It must implement both
/// IQuerySingleBase<TResponse> and IQueryBase<TResponse>.
/// </typeparam>
/// <typeparam name="TResponse">
/// Represents the type of the response associated with the query.
/// </typeparam>
public interface IQuerySingBaseHandler<in TQuery, TResponse>
    : IQueryBaseHandler<TQuery, TResponse>
    where TQuery : IQuerySingleBase<TResponse>, IQueryBase<TResponse>;

/// <summary>
/// Interface for handling query operations that retrieve a collection of responses with associated headers.
/// </summary>
/// <typeparam name="TQuery">The type of the query being handled. Must implement <see cref="IQueryListBase{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the individual response elements in the collection.</typeparam>
public interface IQueryListBaseHandler<in TQuery, TResponse>
    : IQueryBaseHandler<TQuery, IEnumerable<TResponse>>
    where TQuery : IQueryListBase<TResponse>;
