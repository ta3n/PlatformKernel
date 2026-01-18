using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.MasterCalendar;
using Liberty.SysException;

namespace Liberty.Reservation.Employee.WebAPI.Application.Validations;

public class MasterCalendarUpdateDataCommandValidator : AbstractValidator<MasterCalendarUpdateDataCommand>
{
    public MasterCalendarUpdateDataCommandValidator()
    {
        RuleFor(x => x.Payload.AppDate)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0003)
            .Must(ValidDate.BeValidDate)
            .WithErrorCode(ErrorCode.E0008)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions());

        RuleFor(x => x.Payload.Name)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(250)
            .WithErrorCode(ErrorCode.E0002);
    }
}
