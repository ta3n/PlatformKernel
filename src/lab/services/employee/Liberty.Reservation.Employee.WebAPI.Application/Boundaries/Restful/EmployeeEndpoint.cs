using Asp.Versioning;
using Liberty.Pagination;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Employee;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Employee;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Employee.WebAPI.Application.Web.Rest.Utilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;

[Route("api/employees")]
[ApiVersion("1.0")]
public class EmployeeEndpoint(
    ILogger<EmployeeEndpoint> logger,
    IMediator mediator
) : BaseEndpoint(mediator)
{
    [HttpPost]
    public async Task<IActionResult> CreateEmployee(
        [FromBody] CreateEmployeeRequest request
    )
    {
        logger.LogDebug("REST request to save Employee : {Request}", request);

        var newEmployee = await Mediator.Send(
            new EmployeeCreateCommand { Payload = request }
        );

        return Ok(newEmployee)
            .WithHeaders(
                HeaderUtil.CreateEntityCreationAlert(
                    "employee",
                    newEmployee
                )
            );
    }

    [HttpGet]
    public async Task<IActionResult> GetAllEmployees(
        IPageable pageable
    )
    {
        logger.LogDebug("REST request to get a page of Employees");

        var (headers, response) = await Mediator.Send(
            new EmployeeGetAllQuery(
                pageable
            )
        );
        return Ok(response).WithHeaders(headers);
    }

    [HttpGet("public")]
    public async Task<IActionResult> GetAllPublicEmployees(
        IPageable pageable
    )
    {
        logger.LogDebug("REST request to get a page of Public Employees");

        var (headers, response) = await Mediator.Send(
            new EmployeeGetAllPublicEmployeesQuery(
                pageable
            )
        );
        return Ok(response).WithHeaders(headers);
    }
}
