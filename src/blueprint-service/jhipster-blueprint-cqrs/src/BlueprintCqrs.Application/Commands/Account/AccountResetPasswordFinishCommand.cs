using BlueprintCqrs.Dto.Authentication;
using MediatR;

namespace BlueprintCqrs.Application.Commands.Account;

public class AccountResetPasswordFinishCommand : IRequest<Domain.Entities.User>
{
    public KeyAndPasswordDto KeyAndPasswordDto { get; set; }
}
