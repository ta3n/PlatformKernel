using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BlueprintCqrs.Domain.Services.Interfaces;
using MediatR;

namespace BlueprintCqrs.Application.Queries.User;

public class UserGetAuthoritiesQueryHandler(
    IUserService userService
) : IRequestHandler<UserGetAuthoritiesQuery, IEnumerable<string>>
{
    private readonly IUserService _userService = userService;

    public Task<IEnumerable<string>> Handle(
        UserGetAuthoritiesQuery request,
        CancellationToken cancellationToken
    )
    {
        return Task.FromResult(_userService.GetAuthorities());
    }
}
