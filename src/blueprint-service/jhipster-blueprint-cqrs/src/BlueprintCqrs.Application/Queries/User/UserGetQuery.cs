using BlueprintCqrs.Dto;
using MediatR;

namespace BlueprintCqrs.Application.Queries.User;

public class UserGetQuery : IRequest<UserDto>
{
    public string Login { get; set; }
}
