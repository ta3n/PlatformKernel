using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.AlertMessage;
using Liberty.SysException;

namespace Liberty.Reservation.Employee.WebAPI.Application.Validations;

public class AlertMessageUpdateCommandValidator : AbstractValidator<AlertMessageUpdateCommand>
{
    public AlertMessageUpdateCommandValidator()
    {
        RuleFor(x => x.Payload.Id)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0003);

        RuleFor(x => x.Payload.Title)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.Content)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(1000)
            .WithErrorCode(ErrorCode.E0002);
    }
}
