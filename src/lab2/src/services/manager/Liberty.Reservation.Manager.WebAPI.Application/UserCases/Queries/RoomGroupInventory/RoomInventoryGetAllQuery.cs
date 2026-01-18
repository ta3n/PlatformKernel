using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroupInventory;

public record RoomInventoryGetAllQuery(
    IPageable Pageable,
    long StartAppDate,
    long EndAppDate
) : IQueryPagedBase<RoomGroupWithAppDatesResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
