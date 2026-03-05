using Liberty.Reservation.Application.Utils;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class PlanCreateCommandValidator : AbstractValidator<PlanCreateCommand>
{
    public PlanCreateCommandValidator()
    {
        RuleFor(x => x.Payload.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(200)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.Summary)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(10000)
            .WithErrorCode(ErrorCode.E0002);
    }
}
