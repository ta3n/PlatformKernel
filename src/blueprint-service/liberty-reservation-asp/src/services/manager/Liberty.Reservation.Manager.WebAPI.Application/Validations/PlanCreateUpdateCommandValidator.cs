using Liberty.Reservation.Application.Utils;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class PlanCreateUpdateCommandValidator : AbstractValidator<PlanCreateCommand>
{
    public PlanCreateUpdateCommandValidator()
    {
        RuleFor(x => x.Payload.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(250)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.Summary)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(1000)
            .WithErrorCode(ErrorCode.E0002);
    }
}
