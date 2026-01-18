using Liberty.Reservation.Application.Utils;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class PlanEnabledRoomTypeCommandValidator : AbstractValidator<PlanEnabledRoomTypeCommand>
{
    public PlanEnabledRoomTypeCommandValidator()
    {
        RuleFor(x => x.Payload.IsEnabled)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001);
    }
}
