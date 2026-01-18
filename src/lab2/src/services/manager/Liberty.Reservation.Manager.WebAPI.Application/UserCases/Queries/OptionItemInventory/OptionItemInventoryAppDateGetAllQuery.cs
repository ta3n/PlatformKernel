using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.OptionItemInventory;

public record OptionItemInventoryAppDateGetAllQuery(
    IPageable Pageable,
    long StartAppDate,
    long EndAppDate
) : IQueryPagedBase<OptionItemWithAppDatesResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
