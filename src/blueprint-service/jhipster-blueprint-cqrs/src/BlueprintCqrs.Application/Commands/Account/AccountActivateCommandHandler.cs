using System.Threading;
using System.Threading.Tasks;
using BlueprintCqrs.Domain.Services.Interfaces;
using MediatR;

namespace BlueprintCqrs.Application.Commands.Account;

public class AccountActivateCommandHandler(
    IUserService userService
) : IRequestHandler<AccountActivateCommand, Domain.Entities.User>
{
    private readonly IUserService _userService = userService;

    public Task<Domain.Entities.User> Handle(
        AccountActivateCommand command,
        CancellationToken cancellationToken
    )
    {
        return _userService.ActivateRegistration(command.Key);
    }
}
