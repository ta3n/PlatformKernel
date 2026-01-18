using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BathingTaxAge;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class BathingTaxAgeVisibleCommandValidator : AbstractValidator<BathingTaxAgeVisibleCommand>
{
    public BathingTaxAgeVisibleCommandValidator()
    {
        RuleFor(x => x.Payload.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);
    }
}
