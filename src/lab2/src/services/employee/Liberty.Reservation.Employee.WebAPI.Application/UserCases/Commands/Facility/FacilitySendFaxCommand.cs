using Liberty.Reservation.Application.Cqrs.BaseCommands;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Facility;

public record FacilitySendFaxCommand : UpdateCommandBase<FacilitySendFaxRequest, (IHeaderDictionary header, SentFaxResponse response)>;
