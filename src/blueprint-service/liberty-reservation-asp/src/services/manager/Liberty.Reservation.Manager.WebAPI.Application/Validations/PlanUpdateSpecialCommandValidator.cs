using Liberty.Reservation.Application.Utils;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class PlanUpdateSpecialCommandValidator : AbstractValidator<PlanUpdateSpecialCommand>
{
    public PlanUpdateSpecialCommandValidator()
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.SecretWord)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .When(x => x.Payload.IsSecret ?? false);

        RuleFor(x => x.Payload.SecretWord)
            .Cascade(CascadeMode.Stop)
            .Must(x => !string.IsNullOrWhiteSpace(x?.Trim()))
            .WithErrorCode(ErrorCode.E0001)
            .When(x => x.Payload.IsSecret ?? false);

        RuleFor(x => x.Payload.SecretWord)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(1000)
            .WithErrorCode(ErrorCode.E0002);
    }
}
