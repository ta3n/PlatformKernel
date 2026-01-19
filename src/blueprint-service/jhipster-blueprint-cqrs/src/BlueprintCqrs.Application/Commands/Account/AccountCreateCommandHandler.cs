using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BlueprintCqrs.Domain.Services.Interfaces;
using MediatR;

namespace BlueprintCqrs.Application.Commands.Account;

public class AccountCreateCommandHandler(
    IUserService userService,
    IMapper mapper,
    IMailService mailService
) : IRequestHandler<AccountCreateCommand, Domain.Entities.User>
{
    private readonly IMailService _mailService = mailService;
    private readonly IUserService _userService = userService;
    private readonly IMapper _userMapper = mapper;

    public async Task<Domain.Entities.User> Handle(
        AccountCreateCommand command,
        CancellationToken cancellationToken
    )
    {
        var user = await _userService.RegisterUser(_userMapper.Map<Domain.Entities.User>(command.ManagedUserDto), command.ManagedUserDto.Password);
        await _mailService.SendActivationEmail(user.Email, user.FirstName, user.ActivationKey);
        return user;
    }
}
