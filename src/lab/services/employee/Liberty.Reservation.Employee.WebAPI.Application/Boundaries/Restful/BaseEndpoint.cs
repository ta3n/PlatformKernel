using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Liberty.Reservation.Employee.WebAPI.Application.Boundaries.Restful;

[ApiController]
[ApiVersion("1.0")]
public abstract class BaseEndpoint(
    IMediator mediator
) : ControllerBase
{
    protected IMediator Mediator { get; } = mediator;
}
