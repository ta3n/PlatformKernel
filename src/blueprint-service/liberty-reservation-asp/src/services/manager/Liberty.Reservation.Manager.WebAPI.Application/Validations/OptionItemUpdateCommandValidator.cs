using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.OptionItem;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class OptionItemUpdateCommandValidator : AbstractValidator<OptionItemUpdateCommand>
{
    public OptionItemUpdateCommandValidator()
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
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.BaseNumber)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0011);

        RuleFor(x => x.Payload.Price)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0011);
    }
}
