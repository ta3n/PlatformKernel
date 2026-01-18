using FluentValidation;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Employee;

namespace Liberty.Reservation.Employee.WebAPI.Application.Validations;

public class EmployeeCreateCommandValidator : AbstractValidator<EmployeeCreateCommand>
{
    public EmployeeCreateCommandValidator()
    {
        RuleFor(x => x.Payload.Name)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Payload.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();
    }
}
