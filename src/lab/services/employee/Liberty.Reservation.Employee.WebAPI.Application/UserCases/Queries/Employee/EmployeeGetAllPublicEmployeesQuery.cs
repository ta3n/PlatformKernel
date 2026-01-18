using Liberty.Pagination;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Employee;

public record EmployeeGetAllPublicEmployeesQuery(
    IPageable Pageable
)
    : IQueryPagedBase<DataGetEmployeeManyResponse>
{
    public IPageable Pageable { get; set; } = Pageable;
}
