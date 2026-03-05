using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Application.Utils;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class PlanUpdateCancelCommandValidator : AbstractValidator<PlanUpdateCancelCommand>
{
    public PlanUpdateCancelCommandValidator()
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.CancellationId)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.CancelLimit)
            .Cascade(CascadeMode.Stop)
            .Must(ValidDate.BeAValidTimeSpan)
            .WithErrorCode(ErrorCode.E0012)
            .WithMessage(ErrorCode.E0012.GetEnumDescriptions());
    }
}
