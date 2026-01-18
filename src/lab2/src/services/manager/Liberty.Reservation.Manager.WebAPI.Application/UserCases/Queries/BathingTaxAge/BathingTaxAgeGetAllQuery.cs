using Liberty.ApplicationShared.Cqrs.BaseQuery;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BathingTaxAge;

public record BathingTaxAgeGetAllQuery(
    IPageable Pageable
) : IQuerySingleBase<BathingTaxAgeResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
};
