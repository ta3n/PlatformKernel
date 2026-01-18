using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.CancellationPolicy;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class CancellationPolicyDeleteCommandValidator : AbstractValidator<CancellationPolicyDeleteCommand>
{
    public CancellationPolicyDeleteCommandValidator()
    {
        RuleFor(x => x.Payload.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);
    }
}
