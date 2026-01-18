using Microsoft.AspNetCore.Http;

namespace Liberty.ApplicationShared.Cqrs.BaseQuery;

public interface IQueryBase<TResponse> : IRequestBase<(IHeaderDictionary, TResponse)>;

public interface IQuerySingleBase<TResponse> : IQueryBase<TResponse>;

public interface IQueryListBase<TResponse> : IQueryBase<IEnumerable<TResponse>>;


