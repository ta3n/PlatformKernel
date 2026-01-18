using AutoMapper;
using AutoMapper.QueryableExtensions;
using Liberty.Pagination;
using Liberty.Pagination.Extensions;
using Liberty.Reservation.Application.Cqrs.BaseQueries;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using Liberty.Specification;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Employee;

public class EmployeeGetAllPublicEmployeesQueryHandler(
    IMapper mapper,
    IEmployeeRepository employeeRepository
) : QueryPageBaseHandler<EmployeeGetAllPublicEmployeesQuery, DataGetEmployeeManyResponse>(
    mapper
)
{
    protected override async Task<(IHeaderDictionary, IEnumerable<DataGetEmployeeManyResponse>)> HandleAsync(
        EmployeeGetAllPublicEmployeesQuery request,
        CancellationToken cancellationToken
    )
    {
        var queryable = employeeRepository
            .GetQueryableWithAsNoTracking(
                new EmployeeGetAllPublicEmployeesQuerySpec(
                    request.Pageable,
                    x => x.IsEnabled
                )
            )
            .ProjectTo<DataGetEmployeeManyResponse>(Mapper.ConfigurationProvider);
        var page = await queryable.UsePageableAsync(
            request.Pageable,
            cancellationToken: cancellationToken
        );

        var data = page.Content;
        var headers = page.GeneratePaginationHttpHeaders();

        return (headers, data);
    }
}

public class EmployeeGetAllPublicEmployeesQuerySpec
    : SpecificationBase<Reservation.Employee.Application.Contexts.Entities.Employee>
{
    public EmployeeGetAllPublicEmployeesQuerySpec(
        IPageable pageable,
        Expression<Func<Reservation.Employee.Application.Contexts.Entities.Employee, bool>>? criteria = null
    )
    {
        Criteria = criteria;

        AddInclude(x => x.Address!);
        AddInclude(x => x.EmployeeMeta!);


        if (!pageable.Sort.Orders.Any())
        {
            return;
        }

        foreach (var sortOrder in pageable.Sort.Orders)
        {
            switch (sortOrder.Property)
            {
                case "kana" when sortOrder.Direction is Direction.Asc:
                    ApplyOrderBy(x => x.EmployeeMeta!.Kana!);
                    break;
                case "kana" when sortOrder.Direction is Direction.Desc:
                    ApplyOrderByDescending(x => x.EmployeeMeta!.Kana!);
                    break;
                default:
                    ApplyOrderBy(x => x.Id);
                    break;
            }
        }
    }

    public override Expression<Func<Reservation.Employee.Application.Contexts.Entities.Employee, bool>>? Criteria
    {
        get;
    }
}
