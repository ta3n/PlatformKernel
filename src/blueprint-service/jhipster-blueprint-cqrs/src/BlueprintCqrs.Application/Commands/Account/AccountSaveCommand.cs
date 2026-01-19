using System.Security.Claims;
using BlueprintCqrs.Dto;
using MediatR;

namespace BlueprintCqrs.Application.Commands.Account;

public class AccountSaveCommand : IRequest<Unit>
{
    public ClaimsPrincipal User { get; set; }
    public UserDto UserDto { get; set; }
}
