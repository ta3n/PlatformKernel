using BlueprintCqrs.Dto;
using MediatR;

namespace BlueprintCqrs.Application.Commands.Account;

public class AccountCreateCommand : IRequest<Domain.Entities.User>
{
    public ManagedUserDto ManagedUserDto { get; set; }
}
