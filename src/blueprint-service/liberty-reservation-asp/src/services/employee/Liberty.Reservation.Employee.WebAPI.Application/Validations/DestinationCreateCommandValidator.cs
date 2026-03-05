using Liberty.Reservation.Application.Utils;
using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Destination;
using Liberty.SysException;
using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Employee.WebAPI.Application.Validations;

public class DestinationCreateCommandValidator : AbstractValidator<DestinationCreateCommand>
{
    public DestinationCreateCommandValidator()
    {
        RuleFor(x => x.Payload.Name)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(250)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.ShortName)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(100)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.Url)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .Must(ValidUrl.BeValidUrl)
            .WithErrorCode(ErrorCode.E0005)
            .WithMessage(ErrorCode.E0005.GetEnumDescriptions())
            .MaximumLength(100)
            .WithErrorCode(ErrorCode.E0002);
    }
}
