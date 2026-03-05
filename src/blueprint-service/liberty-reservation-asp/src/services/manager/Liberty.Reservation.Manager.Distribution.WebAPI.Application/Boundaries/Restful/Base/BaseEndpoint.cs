using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Boundaries.Restful.Base;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
public abstract class BaseEndpoint : ControllerBase
{
    protected IMapper Mapper { get; }
    protected IMediator? Mediator { get; }

    protected BaseEndpoint(
        IMapper mapper,
        IMediator mediator
    )
    {
        Mapper = mapper;
        Mediator = mediator;
    }

    protected BaseEndpoint(
        IMapper mapper
    )
    {
        Mapper = mapper;
    }
}
