using BlueprintCqrs.Dto;
using MediatR;

namespace BlueprintCqrs.Application.Commands.User;

public class UserCreateCommand : IRequest<Domain.Entities.User>
{
    public UserDto UserDto { get; set; }
}
