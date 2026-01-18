using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class FacilityUpdatePaymentMethodCommandValidator : AbstractValidator<FacilityUpdatePaymentMethodCommand>
{
    public FacilityUpdatePaymentMethodCommandValidator()
    {
        RuleFor(x => x.Payload.OnSidePaymentComment)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(10000)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.OnLinePaymentComment)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(10000)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.PaymentComment)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(10000)
            .WithErrorCode(ErrorCode.E0002);
    }
}
