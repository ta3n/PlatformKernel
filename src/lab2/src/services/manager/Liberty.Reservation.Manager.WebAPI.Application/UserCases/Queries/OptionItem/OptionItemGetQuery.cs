using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.OptionItem;

public record OptionItemGetQuery(
    long Id
) : IQuerySingleBase<OptionItemDetailResponse>;
