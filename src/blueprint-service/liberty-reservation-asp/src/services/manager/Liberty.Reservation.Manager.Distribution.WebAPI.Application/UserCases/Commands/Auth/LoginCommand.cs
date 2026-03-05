using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Responses;
using Microsoft.AspNetCore.Identity.Data;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Commands.Auth;

public record LoginCommand : ActionCommandBase<LoginRequest, IdentityResponse>;
