using Liberty.Pagination;
using Liberty.Reservation.Application.Cqrs.BaseQueries;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.PersonAgeType;

public class PersonAgeTypeGetAllQuery(
    IPageable pageable
) : IQueryPagedBase<PersonAgeTypeResponse>
{
    public IPageable Pageable { get; set; } = pageable;
}
