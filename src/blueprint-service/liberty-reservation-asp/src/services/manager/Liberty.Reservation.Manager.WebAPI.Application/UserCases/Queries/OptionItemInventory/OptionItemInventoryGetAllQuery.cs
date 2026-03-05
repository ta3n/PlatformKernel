using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.OptionItemInventory;

public record OptionItemInventoryGetAllQuery(
    IPageable Pageable,
    long StartAppDate,
    long EndAppDate
) : IQueryPagedBase<OptionItemAppDateResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
