using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Application.Cqrs.BaseQueries;

public interface IQueryPagedBaseHandler<in TQuery, TResponse>
    : IQueryBaseHandler<TQuery, IEnumerable<TResponse>>
    where TQuery : IQueryPagedBase<TResponse>;
