using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PriceType;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class PriceTypeUpdateCommandValidator : AbstractValidator<PriceTypeUpdateCommand>
{
    public PriceTypeUpdateCommandValidator()
    {
        RuleFor(x => x.Payload.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(250)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.ShortName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(50)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.Color)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(20)
            .WithErrorCode(ErrorCode.E0002)
            .Must(ValidDate.BeValidColor)
            .WithErrorCode(ErrorCode.E0013)
            .WithMessage(ErrorCode.E0013.GetEnumDescriptions());
    }
}
