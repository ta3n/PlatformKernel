using Microsoft.AspNetCore.Http;

namespace Liberty.ApplicationShared.Cqrs.BaseQuery;

/// <summary>
/// Defines the base contract for all query types in the CQRS pattern.
/// Queries are responsible for retrieving data without modifying the system state.
/// </summary>
/// <typeparam name="TResponse">
/// The type of the data returned by the query.
/// It can represent a single object, a collection, or a combination of response and headers.
/// </typeparam>
public interface IQueryBase<TResponse> : IRequestBase<(IHeaderDictionary, TResponse)>;

/// <summary>
/// Represents a base interface for queries that are intended to retrieve a single response of a specified type.
/// </summary>
/// <typeparam name="TResponse">
/// The type of the response that the query is expected to return.
/// </typeparam>
public interface IQuerySingleBase<TResponse> : IQueryBase<TResponse>;

/// <summary>
/// Represents a base interface for queries that retrieve a collection of responses.
/// </summary>
/// <typeparam name="TResponse">The type of the individual response elements in the collection.</typeparam>
public interface IQueryListBase<TResponse> : IQueryBase<IEnumerable<TResponse>>;
