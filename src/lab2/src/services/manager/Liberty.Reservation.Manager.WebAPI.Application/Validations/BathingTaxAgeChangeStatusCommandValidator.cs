using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BathingTaxAge;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class BathingTaxAgeChangeStatusCommandValidator : AbstractValidator<BathingTaxAgeChangeStatusCommand>
{
    public BathingTaxAgeChangeStatusCommandValidator()
    {
        RuleFor(x => x.Payload)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new BathingTaxAgeChangeStatusRequestValidator());
    }
}

public class BathingTaxAgeChangeStatusRequestValidator : AbstractValidator<BathingTaxAgeChangeStatusRequest>
{
    public BathingTaxAgeChangeStatusRequestValidator()
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);
    }
}
