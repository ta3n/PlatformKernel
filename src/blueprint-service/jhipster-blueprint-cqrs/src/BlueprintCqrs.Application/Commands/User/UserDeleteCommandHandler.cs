using System.Threading;
using System.Threading.Tasks;
using BlueprintCqrs.Domain.Services.Interfaces;
using MediatR;

namespace BlueprintCqrs.Application.Commands.User;

public class UserDeleteCommandHandler(
    IUserService userService
) : IRequestHandler<UserDeleteCommand, Unit>
{
    private readonly IUserService _userService = userService;

    public async Task<Unit> Handle(
        UserDeleteCommand request,
        CancellationToken cancellationToken
    )
    {
        await _userService.DeleteUser(request.Login);
        return Unit.Value;
    }
}
