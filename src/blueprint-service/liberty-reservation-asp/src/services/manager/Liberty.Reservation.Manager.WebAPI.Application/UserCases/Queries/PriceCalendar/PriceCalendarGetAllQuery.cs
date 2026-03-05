using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.PriceCalendar;

public record PriceCalendarGetAllQuery(
    long StartDate,
    long EndDate,
    IPageable Pageable
) : IQueryPagedBase<PriceCalendarResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
