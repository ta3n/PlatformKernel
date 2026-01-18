using Liberty.Reservation.Application.Cqrs.BaseCommands;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.PersonAgeType;

public record PersonAgeTypeUpdateCommand : UpdateCommandBase<PersonAgeTypeUpdateRequest, long>;
