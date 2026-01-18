using Liberty.Pagination;
using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.MasterCalendar;

public class MasterCalendarGetAllQuery(
    IPageable pageable,
    long startDate,
    long endDate
) : IQueryPagedBase<DateOfMasterCalendarResponse>
{
    public IPageable Pageable { get; set; } = pageable;
    public long StartDate { get; } = startDate;
    public long EndDate { get; } = endDate;
}
