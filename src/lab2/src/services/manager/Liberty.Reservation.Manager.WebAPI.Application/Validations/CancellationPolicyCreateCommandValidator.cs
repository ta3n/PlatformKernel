using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.CancellationPolicy;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class CancellationPolicyCreateCommandValidator : AbstractValidator<CancellationPolicyCreateCommand>
{
    public CancellationPolicyCreateCommandValidator()
    {
        RuleFor(x => x.Payload.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(500)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.Description)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(500)
            .WithErrorCode(ErrorCode.E0002);
    }
}
