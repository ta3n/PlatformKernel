using Liberty.Reservation.Application.Utils;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class PlanUpdateOptionCommandValidator : AbstractValidator<PlanUpdateOptionCommand>
{
    public PlanUpdateOptionCommandValidator()
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.OptionIds)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001);
        // .Must(
        //     x => x!.Count >= 1
        // )
        // .When(x => x.Payload.UseFixedOptionItem ?? false)
        // .WithErrorCode(ErrorCode.E0010)
        // .WithErrorCode(ErrorCode.E0010.GetEnumDescriptions());
    }
}
