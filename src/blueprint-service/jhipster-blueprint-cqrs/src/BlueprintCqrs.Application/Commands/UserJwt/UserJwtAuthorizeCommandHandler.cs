using BlueprintCqrs.Domain.Services.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Principal;

namespace BlueprintCqrs.Application.Commands.UserJwt;

public class UserJwtAuthorizeCommandHandler(
    IAuthenticationService authenticationService
) : IRequestHandler<UserJwtAuthorizeCommand, IPrincipal>
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    public Task<IPrincipal> Handle(
        UserJwtAuthorizeCommand command,
        CancellationToken cancellationToken
    )
    {
        return _authenticationService.Authenticate(command.LoginDto.Username, command.LoginDto.Password);
    }
}
