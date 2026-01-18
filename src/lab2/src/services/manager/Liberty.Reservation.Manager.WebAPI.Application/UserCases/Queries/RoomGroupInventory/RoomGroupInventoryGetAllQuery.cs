using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroupInventory;

public record RoomGroupInventoryGetAllQuery(
    IPageable Pageable,
    long StartAppDate,
    long EndAppDate
) : IQueryPagedBase<RoomGroupAppDateDataResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
