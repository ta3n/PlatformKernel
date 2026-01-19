using BlueprintCqrs.Dto.Authentication;
using MediatR;

namespace BlueprintCqrs.Application.Commands.Account;

public class AccountChangePasswordCommand : IRequest<Unit>
{
    public PasswordChangeDto PasswordChangeDto { get; set; }
}
