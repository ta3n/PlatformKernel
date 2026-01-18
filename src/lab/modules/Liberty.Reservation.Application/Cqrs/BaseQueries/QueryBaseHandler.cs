using AutoMapper;
using Liberty.ApplicationShared.Cqrs.BaseQuery;
using Microsoft.AspNetCore.Http;

namespace Liberty.Reservation.Application.Cqrs.BaseQueries;

public abstract class QueryBaseHandler<TQuery, TResponse>(
    IMapper mapper
) : IQueryBaseHandler<TQuery, TResponse>
    where TQuery : IQueryBase<TResponse>
{
    protected IMapper Mapper { get; } = mapper;

    public virtual Task<(IHeaderDictionary, TResponse)> Handle(
        TQuery request,
        CancellationToken cancellationToken
    )
    {
        return HandleAsync(request, cancellationToken);
    }

    protected abstract Task<(IHeaderDictionary, TResponse)> HandleAsync(
        TQuery request,
        CancellationToken cancellationToken
    );
}

public abstract class QuerySingleBaseHandler<TQuery, TResponse>(
    IMapper mapper
) : QueryBaseHandler<TQuery, TResponse>(
        mapper
    ),
    IQuerySingBaseHandler<TQuery, TResponse>
    where TQuery : IQuerySingleBase<TResponse>, IQueryBase<TResponse>;

public abstract class QueryListBaseHandler<TQuery, TResponse>(
    IMapper mapper
) : QueryBaseHandler<TQuery, IEnumerable<TResponse>>(
        mapper
    ),
    IQueryListBaseHandler<TQuery, TResponse>
    where TQuery : IQueryListBase<TResponse>;

public abstract class QueryPageBaseHandler<TQuery, TResponse>(
    IMapper mapper
) : QueryBaseHandler<TQuery, IEnumerable<TResponse>>(
        mapper
    ),
    IQueryPagedBaseHandler<TQuery, TResponse>
    where TQuery : IQueryPagedBase<TResponse>;
