using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Utils;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class PlanUpdatePaymentMethodCommandValidator : AbstractValidator<PlanUpdatePaymentMethodCommand>
{
    public PlanUpdatePaymentMethodCommandValidator()
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x)
            .Must(
                x =>
                    (x.Payload.IsOnLinePayment ?? false) || (x.Payload.IsOnSidePayment ?? false)
            )
            .WithMessage(ErrorCode.E0001.GetEnumDescriptions())
            .WithErrorCode(ErrorCode.E0001);
    }
}
