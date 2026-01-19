using BlueprintCqrs.Dto;
using MediatR;

namespace BlueprintCqrs.Application.Queries.Account;

public class AccountGetQuery : IRequest<UserDto>
{
}
