using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlueprintCqrs.Application.Queries.Account;

public class AccountGetAuthenticatedQueryHandler(
    UserManager<Domain.Entities.User> userManager
) : IRequestHandler<AccountGetAuthenticatedQuery, string>
{
    private readonly UserManager<Domain.Entities.User> _userManager = userManager;

    public Task<string> Handle(
        AccountGetAuthenticatedQuery command,
        CancellationToken cancellationToken
    )
    {
        return Task.FromResult(_userManager.GetUserName(command.User));
    }
}
