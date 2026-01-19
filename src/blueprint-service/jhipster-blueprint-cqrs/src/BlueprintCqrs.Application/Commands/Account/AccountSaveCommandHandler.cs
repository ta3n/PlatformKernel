using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BlueprintCqrs.Crosscutting.Exceptions;
using BlueprintCqrs.Domain.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlueprintCqrs.Application.Commands.Account;

public class AccountSaveCommandHandler(
    IUserService userService,
    IMapper mapper,
    UserManager<Domain.Entities.User> userManager
) : IRequestHandler<AccountSaveCommand, Unit>
{
    private readonly IUserService _userService = userService;
    private readonly UserManager<Domain.Entities.User> _userManager = userManager;

    public async Task<Unit> Handle(
        AccountSaveCommand command,
        CancellationToken cancellationToken
    )
    {
        var userName = _userManager.GetUserName(command.User);
        if (userName == null)
        {
            throw new InternalServerErrorException("Current user login not found");
        }

        var existingUser = await _userManager.FindByEmailAsync(command.UserDto.Email);
        if (existingUser != null && !string.Equals(existingUser.Login, userName, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new EmailAlreadyUsedException();
        }

        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            throw new InternalServerErrorException("User could not be found");
        }

        await _userService.UpdateUser(
            command.UserDto.FirstName,
            command.UserDto.LastName,
            command.UserDto.Email,
            command.UserDto.LangKey,
            command.UserDto.ImageUrl
        );
        return Unit.Value;
    }
}
