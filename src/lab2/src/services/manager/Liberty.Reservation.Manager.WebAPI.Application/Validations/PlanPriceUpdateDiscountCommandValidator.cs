using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class PlanPriceUpdateDiscountCommandValidator
    : AbstractValidator<PlanPriceUpdateDiscountCommand>
{
    public PlanPriceUpdateDiscountCommandValidator()
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

        RuleForEach(p => p.Payload.DiscountDatas)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .ChildRules(
                child =>
                {
                    child.RuleFor(x => x.StartPrevDay)
                        .GreaterThanOrEqualTo(0)
                        .WithErrorCode(ErrorCode.E0011);

                    child.RuleFor(x => x.EndPrevDay)
                        .GreaterThanOrEqualTo(0)
                        .WithErrorCode(ErrorCode.E0003)
                        .GreaterThanOrEqualTo(x => x.StartPrevDay)
                        .WithErrorCode(ErrorCode.E0009);

                    child.RuleFor(x => x.PriceSettingType)
                        .NotNull()
                        .WithErrorCode(ErrorCode.E0001);

                    child.RuleFor(x => x.PersonMin)
                        .GreaterThan(0)
                        .WithErrorCode(ErrorCode.E0003);

                    child.RuleFor(x => x.PersonMax)
                        .GreaterThanOrEqualTo(x => x.PersonMin)
                        .WithErrorCode(ErrorCode.E1017);
                }
            );
    }
}
