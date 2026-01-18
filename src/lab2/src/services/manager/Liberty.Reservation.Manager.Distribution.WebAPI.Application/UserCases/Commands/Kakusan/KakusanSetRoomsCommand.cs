using Liberty.Reservation.Application.Behaviors;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Commands.Kakusan;

[IgnoreValidation]
public record KakusanSetRoomsCommand : UpdateCommandBase<SetRoomsRequest, SetRoomsResponse>;
