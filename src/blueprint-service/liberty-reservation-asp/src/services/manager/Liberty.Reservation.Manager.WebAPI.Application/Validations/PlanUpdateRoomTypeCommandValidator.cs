using Liberty.Reservation.Application.Utils;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class PlanUpdateRoomTypeCommandValidator : AbstractValidator<PlanUpdateRoomTypeCommand>
{
    public PlanUpdateRoomTypeCommandValidator()
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);
    }
}
