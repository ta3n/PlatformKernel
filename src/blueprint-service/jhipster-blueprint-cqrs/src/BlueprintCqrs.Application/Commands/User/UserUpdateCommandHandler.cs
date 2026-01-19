using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BlueprintCqrs.Crosscutting.Exceptions;
using BlueprintCqrs.Domain.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlueprintCqrs.Application.Commands.User;

public class UserUpdateCommandHandler(
    UserManager<Domain.Entities.User> userManager,
    IUserService userService,
    IMapper mapper
) : IRequestHandler<UserUpdateCommand, Domain.Entities.User>
{
    public async Task<Domain.Entities.User> Handle(
        UserUpdateCommand command,
        CancellationToken cancellationToken
    )
    {
        var existingUser = await userManager.FindByEmailAsync(command.UserDto.Email);
        if (existingUser != null && !existingUser.Id.Equals(command.UserDto.Id))
        {
            throw new EmailAlreadyUsedException();
        }

        existingUser = await userManager.FindByNameAsync(command.UserDto.Login);
        if (existingUser != null && !existingUser.Id.Equals(command.UserDto.Id))
        {
            throw new LoginAlreadyUsedException();
        }

        return await userService.UpdateUser(mapper.Map<Domain.Entities.User>(command.UserDto));
    }
}
