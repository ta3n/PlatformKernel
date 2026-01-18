using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Employee.WebAPI.Application.Models.Requests;

namespace Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Employee;

public record EmployeeCreateCommand : CreateCommandBase<CreateEmployeeRequest, string>;
