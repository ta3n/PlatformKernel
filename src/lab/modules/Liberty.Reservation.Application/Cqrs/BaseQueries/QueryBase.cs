using Liberty.ApplicationShared.Cqrs;
using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Application.Cqrs.BaseQueries;

public abstract record QueryBase<TResponse> : RequestBase<TResponse>, IQueryBase<TResponse>;
