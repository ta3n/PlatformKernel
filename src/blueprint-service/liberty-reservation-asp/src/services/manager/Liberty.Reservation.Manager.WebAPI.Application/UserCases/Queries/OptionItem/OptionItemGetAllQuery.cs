using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.OptionItem;

public record OptionItemGetAllQuery(
    IPageable Pageable
) : IQueryPagedBase<OptionItemResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
