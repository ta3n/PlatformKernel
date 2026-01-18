using AutoMapper;
using AutoMapper.QueryableExtensions;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Employee;

public class EmployeeGetAllQueryHandler(
    IMapper mapper,
    IEmployeeRepository employeeRepository
) : QueryPageBaseHandler<EmployeeGetAllQuery, DataGetEmployeeManyResponse>(
    mapper
)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<DataGetEmployeeManyResponse>)> HandleAsync(
        EmployeeGetAllQuery request,
        CancellationToken cancellationToken
    )
    {
        var page = await employeeRepository
            .GetQueryableWithAsNoTracking()
            .Include(x => x.Address)
            .Include(x => x.EmployeeMeta)
            .ProjectTo<DataGetEmployeeManyResponse>(Mapper.ConfigurationProvider)
            .UsePageableAsync(
                request.Pageable,
                isApplySort: true,
                cancellationToken: cancellationToken
            );

        var data = page.Content;
        var headers = page.GeneratePaginationHttpHeaders();

        return (headers, data);
    }
}
