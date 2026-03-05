using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class PlanPriceUpdateStandardPriceCommandValidator
    : AbstractValidator<PlanPriceUpdateStandardPriceCommand>
{
    public PlanPriceUpdateStandardPriceCommandValidator()
    {
        RuleFor(x => x.PlanId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.RoomTypeId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.SiteId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(p => p.Payload.PriceDatas)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E1062)
            .WithMessage(ErrorCode.E1062.GetEnumDescriptions());

        RuleForEach(p => p.Payload.PriceDatas)
            .Cascade(CascadeMode.Stop)
            .ChildRules(
                child =>
                {
                    child.RuleFor(x => x.DateTypeId)
                        .NotNull()
                        .WithErrorCode(ErrorCode.E1062)
                        .GreaterThan(0)
                        .WithErrorCode(ErrorCode.E1062);

                    child.RuleFor(x => x.PersonMin)
                        .GreaterThan(0)
                        .WithErrorCode(ErrorCode.E0003);

                    child.RuleFor(x => x.PersonMax)
                        .GreaterThan(0)
                        .WithErrorCode(ErrorCode.E0003)
                        .GreaterThanOrEqualTo(x => x.PersonMin)
                        .WithErrorCode(ErrorCode.E1017);
                }
            );
    }
}
