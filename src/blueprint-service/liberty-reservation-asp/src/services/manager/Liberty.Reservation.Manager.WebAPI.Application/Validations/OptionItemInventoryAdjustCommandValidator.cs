using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.OptionItemInventory;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class OptionItemInventoryAdjustCommandValidator : AbstractValidator<OptionItemInventoryAdjustCommand>
{
    public OptionItemInventoryAdjustCommandValidator()
    {
        RuleFor(x => x.Payload)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .Must(x => x.Any())
            .WithErrorCode(ErrorCode.E0001);

        RuleForEach(x => x.Payload)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new OptionItemChangeRemainRequestValidator());
    }
}

public class OptionItemChangeRemainRequestValidator : AbstractValidator<OptionItemChangeRemainRequest>
{
    public OptionItemChangeRemainRequestValidator()
    {
        RuleFor(x => x.AppDateId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.OptionItemId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);
    }
}
