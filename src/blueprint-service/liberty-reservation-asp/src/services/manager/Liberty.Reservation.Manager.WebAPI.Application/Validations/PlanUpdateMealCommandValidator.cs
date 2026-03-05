using Liberty.Reservation.Application.Utils;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class PlanUpdateMealCommandValidator : AbstractValidator<PlanUpdateMealCommand>
{
    public PlanUpdateMealCommandValidator()
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleForEach(p => p.Payload.MealTypes)
            .Cascade(CascadeMode.Stop)
            .ChildRules(
                child =>
                {
                    child.RuleFor(x => x.Id)
                        .Cascade(CascadeMode.Stop)
                        .NotNull()
                        .WithErrorCode(ErrorCode.E0001);

                    child.RuleFor(x => x.MealTypeEatType)
                        .Cascade(CascadeMode.Stop)
                        .NotNull()
                        .WithErrorCode(ErrorCode.E0001);
                }
            );
    }
}
