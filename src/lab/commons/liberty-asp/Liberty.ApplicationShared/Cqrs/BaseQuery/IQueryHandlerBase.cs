using MediatR;
using Microsoft.AspNetCore.Http;

namespace Liberty.ApplicationShared.Cqrs.BaseQuery;

public interface IQueryBaseHandler<in TQuery, TResponse>
    : IRequestHandler<TQuery, (IHeaderDictionary, TResponse)>
    where TQuery : IQueryBase<TResponse>;

public interface IQuerySingBaseHandler<in TQuery, TResponse>
    : IQueryBaseHandler<TQuery, TResponse>
    where TQuery : IQuerySingleBase<TResponse>, IQueryBase<TResponse>;

public interface IQueryListBaseHandler<in TQuery, TResponse>
    : IQueryBaseHandler<TQuery, IEnumerable<TResponse>>
    where TQuery : IQueryListBase<TResponse>;

