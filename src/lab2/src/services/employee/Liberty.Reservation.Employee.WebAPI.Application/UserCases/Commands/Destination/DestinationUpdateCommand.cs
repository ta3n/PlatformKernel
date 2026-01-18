using Liberty.Reservation.Application.Cqrs.BaseCommands;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Destination;

public record DestinationUpdateCommand : UpdateCommandBase<DestinationUpdateRequest, DestinationResponse>;
