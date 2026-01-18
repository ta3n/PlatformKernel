using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Utils;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class PlanUpdateDisplayCommandValidator : AbstractValidator<PlanUpdateDisplayCommand>
{
    public PlanUpdateDisplayCommandValidator()
    {
        RuleForEach(x => x.Payload.Tags)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .When(x => x.Payload.Tags is not null)
            .WithErrorCode(ErrorCode.E0001);

        RuleForEach(x => x.Payload.PlanCategoryIds)
            .Cascade(CascadeMode.Stop)
            .Must(x => x > 0)
            .WithErrorCode(ErrorCode.E0010)
            .WithMessage(ErrorCode.E0010.GetEnumDescriptions());

        RuleForEach(x => x.Payload.MasterCategoryIds)
            .Cascade(CascadeMode.Stop)
            .Must(x => x > 0)
            .WithErrorCode(ErrorCode.E0010)
            .WithMessage(ErrorCode.E0010.GetEnumDescriptions());
    }
}
