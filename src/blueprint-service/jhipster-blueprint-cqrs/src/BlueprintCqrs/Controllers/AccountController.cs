using System.Threading.Tasks;
using System.Collections.Generic;
using BlueprintCqrs.Dto;
using BlueprintCqrs.Web.Extensions;
using BlueprintCqrs.Crosscutting.Constants;
using BlueprintCqrs.Crosscutting.Exceptions;
using Microsoft.AspNetCore.Mvc;
using BlueprintCqrs.Application.Commands.Account;
using BlueprintCqrs.Application.Queries.Account;
using BlueprintCqrs.Application.Queries.User;
using MediatR;
using BlueprintCqrs.Dto.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace BlueprintCqrs.Controllers;

[Route("api")]
[ApiController]
public class AccountController(
    ILogger<AccountController> log,
    IMediator mediator
) : ControllerBase
{
    private readonly ILogger<AccountController> _log = log;
    private readonly IMediator _mediator = mediator;

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAccount(
        [FromBody] ManagedUserDto managedUserDto
    )
    {
        if (!CheckPasswordLength(managedUserDto.Password))
        {
            throw new InvalidPasswordException();
        }

        var user = await _mediator.Send(new AccountCreateCommand { ManagedUserDto = managedUserDto });
        return CreatedAtAction(nameof(GetAccount), user);
    }

    [HttpGet("activate")]
    public async Task ActivateAccount(
        [FromQuery(Name = "key")] string key
    )
    {
        var user = await _mediator.Send(new AccountActivateCommand { Key = key });
        if (user == null)
        {
            throw new InternalServerErrorException("Not user was found for this activation key");
        }
    }

    [HttpGet("authenticate")]
    public async Task<string> IsAuthenticated()
    {
        _log.LogDebug("REST request to check if the current user is authenticated");
        return await _mediator.Send(new AccountGetAuthenticatedQuery { User = User });
    }

    [HttpGet("authorities")]
    [Authorize(Roles = RolesConstants.Admin)]
    public async Task<ActionResult<IEnumerable<string>>> GetAuthorities()
    {
        return Ok(await _mediator.Send(new UserGetAuthoritiesQuery()));
    }

    [Authorize]
    [HttpGet("account")]
    public async Task<ActionResult<UserDto>> GetAccount()
    {
        var userDto = await _mediator.Send(new AccountGetQuery());
        return Ok(userDto);
    }

    [Authorize]
    [HttpPost("account")]
    public async Task<ActionResult> SaveAccount(
        [FromBody] UserDto userDto
    )
    {
        await _mediator.Send(
            new AccountSaveCommand
            {
                User = User,
                UserDto = userDto
            }
        );
        return Ok();
    }

    [Authorize]
    [HttpPost("account/change-password")]
    public async Task<ActionResult> ChangePassword(
        [FromBody] PasswordChangeDto passwordChangeDto
    )
    {
        if (!CheckPasswordLength(passwordChangeDto.NewPassword))
        {
            throw new InvalidPasswordException();
        }

        await _mediator.Send(new AccountChangePasswordCommand { PasswordChangeDto = passwordChangeDto });
        return Ok();
    }

    [HttpPost("account/reset-password/init")]
    public async Task<ActionResult> RequestPasswordReset()
    {
        var mail = await Request.BodyAsStringAsync();
        await _mediator.Send(new AccountResetPasswordCommand { Mail = mail });
        return Ok();
    }

    [HttpPost("account/reset-password/finish")]
    public async Task RequestPasswordReset(
        [FromBody] KeyAndPasswordDto keyAndPasswordDto
    )
    {
        if (!CheckPasswordLength(keyAndPasswordDto.NewPassword))
        {
            throw new InvalidPasswordException();
        }

        var user = await _mediator.Send(new AccountResetPasswordFinishCommand { KeyAndPasswordDto = keyAndPasswordDto });
        if (user == null)
        {
            throw new InternalServerErrorException("No user was found for this reset key");
        }
    }

    private static bool CheckPasswordLength(
        string password
    )
    {
        return !string.IsNullOrEmpty(password)
            && password.Length is >= ManagedUserDto.PasswordMinLength and <= ManagedUserDto.PasswordMaxLength;
    }
}
