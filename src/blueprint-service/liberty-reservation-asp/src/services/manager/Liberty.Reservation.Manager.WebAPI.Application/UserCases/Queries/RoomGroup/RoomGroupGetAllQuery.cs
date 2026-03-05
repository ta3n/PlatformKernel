using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroup;

public record RoomGroupGetAllQuery(
    IPageable Pageable
) : IQueryPagedBase<RoomGroupResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
