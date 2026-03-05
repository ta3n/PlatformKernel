using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroup;

public record RoomGroupGetAllPublishedInQuery(
    long Id,
    IPageable Pageable
) : IQueryPagedBase<SiteOfRoomGroupDetailPublicationSettingResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
