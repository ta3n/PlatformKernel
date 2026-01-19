using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;
using JHipsterNet.Core.Pagination;
using BlueprintCqrs.Dto;
using BlueprintCqrs.Web.Extensions;
using BlueprintCqrs.Application.Queries.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BlueprintCqrs.Controllers;

[Route("api/users")]
[ApiController]
public class PublicUsersController(
    ILogger<UsersController> log,
    IMediator mediator
) : ControllerBase
{
    private readonly ILogger<UsersController> _log = log;
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllPublicUsers(
        IPageable pageable
    )
    {
        _log.LogDebug("REST request to get a page of Users");
        var (headers, userDtos) = await _mediator.Send(new UserGetAllPublicUsersQuery { Page = pageable });
        return Ok(userDtos).WithHeaders(headers);
    }
}
