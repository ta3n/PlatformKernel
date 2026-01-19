using MediatR;

namespace BlueprintCqrs.Application.Commands.User;

public class UserDeleteCommand : IRequest<Unit>
{
    public string Login { get; set; }
}
