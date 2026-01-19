using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BlueprintCqrs.Crosscutting.Exceptions;
using BlueprintCqrs.Domain.Services.Interfaces;
using BlueprintCqrs.Dto;
using MediatR;

namespace BlueprintCqrs.Application.Queries.Account;

public class AccountGetQueryHandler(
    IUserService userService,
    IMapper mapper
) : IRequestHandler<AccountGetQuery, UserDto>
{
    private readonly IUserService _userService = userService;
    private readonly IMapper _userMapper = mapper;

    public async Task<UserDto> Handle(
        AccountGetQuery command,
        CancellationToken cancellationToken
    )
    {
        var user = await _userService.GetUserWithUserRoles();
        if (user == null)
        {
            throw new InternalServerErrorException("User could not be found");
        }

        return _userMapper.Map<UserDto>(user);
    }
}
