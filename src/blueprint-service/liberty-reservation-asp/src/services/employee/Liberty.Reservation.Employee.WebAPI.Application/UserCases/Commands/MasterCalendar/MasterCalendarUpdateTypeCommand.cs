using Liberty.Reservation.Application.Cqrs.BaseCommands;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.MasterCalendar;

public record MasterCalendarUpdateTypeCommand
    : UpdateCommandBase<MasterCalendarEditTypeRequest, DateTypeOfMasterCalendarResponse>;
