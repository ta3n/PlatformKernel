using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.File.WebAPI.Application.UserCases.Queries.Image;

public record ImageGetAllQuery(
    IPageable Pageable
) : IQueryPagedBase<ImageResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
