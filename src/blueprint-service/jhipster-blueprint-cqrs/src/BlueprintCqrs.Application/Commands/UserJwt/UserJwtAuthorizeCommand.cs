using MediatR;
using BlueprintCqrs.Dto.Authentication;
using System.Security.Principal;

namespace BlueprintCqrs.Application.Commands.UserJwt;

public class UserJwtAuthorizeCommand : IRequest<IPrincipal>
{
    public LoginDto LoginDto { get; set; }
}
