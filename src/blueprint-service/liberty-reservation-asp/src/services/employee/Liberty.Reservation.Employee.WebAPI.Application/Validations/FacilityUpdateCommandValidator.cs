using Liberty.Reservation.Application.Utils;
using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Facility;
using Liberty.SysException;
using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Employee.WebAPI.Application.Validations;

public class FacilityUpdateCommandValidator : AbstractValidator<FacilityUpdateCommand>
{
    public FacilityUpdateCommandValidator()
    {
        RuleFor(x => x.Payload.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.SystemEMail)
            .Cascade(CascadeMode.Stop)
            .EmailAddress()
            .WithErrorCode(ErrorCode.E0006)
            .Must(ValidMail.BeValidEmail)
            .WithErrorCode(ErrorCode.E0006)
            .WithMessage(ErrorCode.E0006.GetEnumDescriptions());
    }
}
