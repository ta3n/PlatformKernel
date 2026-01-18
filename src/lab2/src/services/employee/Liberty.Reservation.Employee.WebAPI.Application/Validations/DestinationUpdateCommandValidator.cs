using Liberty.Reservation.Application.Utils;
using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Destination;
using Liberty.SysException;
using Liberty.ApplicationShared.Extensions;

namespace Liberty.Reservation.Employee.WebAPI.Application.Validations;

public class DestinationUpdateCommandValidator : AbstractValidator<DestinationUpdateCommand>
{
    public DestinationUpdateCommandValidator()
    {
        RuleFor(x => x.Payload.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.Name)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(250)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.Code)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(50)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.ShortName)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(100)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.PrefixName)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(2)
            .WithErrorCode(ErrorCode.E0002)
            .Must(
                x => x is { Length: 2 } && x.All(char.IsUpper)
            )
            .WithErrorCode(ErrorCode.E1075);

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
