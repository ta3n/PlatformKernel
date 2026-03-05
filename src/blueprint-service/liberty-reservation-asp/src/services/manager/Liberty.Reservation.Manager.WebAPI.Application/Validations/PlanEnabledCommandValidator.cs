using Liberty.Reservation.Application.Utils;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class PlanEnabledCommandValidator : AbstractValidator<PlanEnabledCommand>
{
    public PlanEnabledCommandValidator()
    {
        RuleFor(x => x.Payload.IsEnabled)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001);
    }
}
