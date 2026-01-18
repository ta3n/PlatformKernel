using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class PlanPriceUpdatePriceCalendarCommandValidator
    : AbstractValidator<PlanPriceUpdatePriceCalendarCommand>
{
    public PlanPriceUpdatePriceCalendarCommandValidator()
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

        RuleFor(x => x.Payload.PriceDatas)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .Must(x => x is { Count: <= 160 })
            .WithErrorCode(ErrorCode.E1053)
            .WithMessage(ErrorCode.E1053.GetEnumDescriptions());

        RuleForEach(p => p.Payload.PriceDatas)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .ChildRules(
                child =>
                {
                    child.RuleFor(x => x.DateCalendar)
                        .GreaterThan(0)
                        .WithErrorCode(ErrorCode.E0003)
                        .Must(ValidDate.BeValidDate)
                        .WithErrorCode(ErrorCode.E0008)
                        .WithMessage(ErrorCode.E0008.GetEnumDescriptions());

                    child.RuleFor(x => x.PersonMin)
                        .GreaterThan(0)
                        .WithErrorCode(ErrorCode.E0003);

                    child.RuleFor(x => x.PersonMax)
                        .GreaterThan(0)
                        .WithErrorCode(ErrorCode.E0003)
                        .GreaterThanOrEqualTo(x => x.PersonMin)
                        .WithErrorCode(ErrorCode.E1017);
                }
            )
            .When(x => x.Payload.PriceDatas!.Count > 0);
    }
}
