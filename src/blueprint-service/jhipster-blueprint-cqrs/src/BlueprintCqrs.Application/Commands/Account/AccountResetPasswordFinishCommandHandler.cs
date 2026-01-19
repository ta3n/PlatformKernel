using System.Threading;
using System.Threading.Tasks;
using BlueprintCqrs.Domain.Services.Interfaces;
using MediatR;

namespace BlueprintCqrs.Application.Commands.Account;

public class AccountResetPasswordFinishCommandHandler(
    IUserService userService
) : IRequestHandler<AccountResetPasswordFinishCommand, Domain.Entities.User>
{
    private readonly IUserService _userService = userService;

    public Task<Domain.Entities.User> Handle(
        AccountResetPasswordFinishCommand command,
        CancellationToken cancellationToken
    )
    {
        return _userService.CompletePasswordReset(command.KeyAndPasswordDto.NewPassword, command.KeyAndPasswordDto.Key);
    }
}
