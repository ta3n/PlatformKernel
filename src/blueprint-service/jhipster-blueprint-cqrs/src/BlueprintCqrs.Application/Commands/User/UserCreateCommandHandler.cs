using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BlueprintCqrs.Crosscutting.Exceptions;
using BlueprintCqrs.Domain.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlueprintCqrs.Application.Commands.User;

public class UserCreateCommandHandler(
    UserManager<Domain.Entities.User> userManager,
    IUserService userService,
    IMapper mapper,
    IMailService mailService
) : IRequestHandler<UserCreateCommand, Domain.Entities.User>
{
    private readonly IMailService _mailService = mailService;
    private readonly UserManager<Domain.Entities.User> _userManager = userManager;
    private readonly IUserService _userService = userService;
    private readonly IMapper _mapper = mapper;

    public async Task<Domain.Entities.User> Handle(
        UserCreateCommand command,
        CancellationToken cancellationToken
    )
    {
        // Lowercase the user login before comparing with database
        if (await _userManager.FindByNameAsync(command.UserDto.Login.ToLowerInvariant()) != null)
        {
            throw new LoginAlreadyUsedException();
        }

        if (await _userManager.FindByEmailAsync(command.UserDto.Email.ToLowerInvariant()) != null)
        {
            throw new EmailAlreadyUsedException();
        }

        var newUser = await _userService.CreateUser(_mapper.Map<Domain.Entities.User>(command.UserDto));
        await _mailService.SendCreationEmail(newUser.Email, newUser.FirstName);
        return newUser;
    }
}
