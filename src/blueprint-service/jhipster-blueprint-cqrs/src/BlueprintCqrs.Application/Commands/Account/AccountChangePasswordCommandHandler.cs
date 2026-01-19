using System.Threading;
using System.Threading.Tasks;
using BlueprintCqrs.Domain.Services.Interfaces;
using MediatR;

namespace BlueprintCqrs.Application.Commands.Account;

public class AccountChangePasswordCommandHandler(
    IUserService userService
) : IRequestHandler<AccountChangePasswordCommand, Unit>
{
    private readonly IUserService _userService = userService;

    public async Task<Unit> Handle(
        AccountChangePasswordCommand command,
        CancellationToken cancellationToken
    )
    {
        await _userService.ChangePassword(command.PasswordChangeDto.CurrentPassword, command.PasswordChangeDto.NewPassword);
        return Unit.Value;
    }
}
