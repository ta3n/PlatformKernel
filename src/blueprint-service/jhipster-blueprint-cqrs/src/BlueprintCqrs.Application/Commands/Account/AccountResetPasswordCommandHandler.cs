using System.Threading;
using System.Threading.Tasks;
using BlueprintCqrs.Crosscutting.Exceptions;
using BlueprintCqrs.Domain.Services.Interfaces;
using MediatR;

namespace BlueprintCqrs.Application.Commands.Account;

public class AccountResetPasswordCommandHandler(
    IUserService userService,
    IMailService mailService
) : IRequestHandler<AccountResetPasswordCommand, Unit>
{
    private readonly IUserService _userService = userService;
    private readonly IMailService _mailService = mailService;

    public async Task<Unit> Handle(
        AccountResetPasswordCommand command,
        CancellationToken cancellationToken
    )
    {
        var user = await _userService.RequestPasswordReset(command.Mail);
        if (user == null)
        {
            throw new EmailNotFoundException();
        }

        await _mailService.SendPasswordResetMail(user.Email, user.FirstName, user.ActivationKey);
        return Unit.Value;
    }
}
