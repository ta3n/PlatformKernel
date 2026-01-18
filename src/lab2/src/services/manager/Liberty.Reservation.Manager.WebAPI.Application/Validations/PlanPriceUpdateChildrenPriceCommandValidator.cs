using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.PlanPrice;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class PlanPriceUpdateChildrenPriceCommandValidator : AbstractValidator<PlanPriceUpdateChildrenPriceCommand>
{
    public PlanPriceUpdateChildrenPriceCommandValidator()
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

        RuleFor(x => x.Payload.PersonAgeTypes)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001);

        RuleForEach(p => p.Payload.PersonAgeTypes)
            .ChildRules(
                child =>
                {
                    child.RuleFor(x => x.PersonAgeTypeId)
                        .Cascade(CascadeMode.Stop)
                        .NotNull()
                        .WithErrorCode(ErrorCode.E0001)
                        .GreaterThan(0)
                        .WithErrorCode(ErrorCode.E0003);

                    child.RuleFor(x => x.IsEnabled)
                        .Cascade(CascadeMode.Stop)
                        .NotNull()
                        .WithErrorCode(ErrorCode.E0001);

                    child.RuleFor(x => x.IsRegardAdult)
                        .Cascade(CascadeMode.Stop)
                        .NotNull()
                        .WithErrorCode(ErrorCode.E0001);

                    child.RuleFor(x => x.PriceSettingType)
                        .Cascade(CascadeMode.Stop)
                        .NotNull()
                        .WithErrorCode(ErrorCode.E0001);

                    child.RuleFor(x => x.Value)
                        .Cascade(CascadeMode.Stop)
                        .GreaterThanOrEqualTo(0)
                        .When(x => !x.IsRegardAdult)
                        .WithErrorCode(ErrorCode.E0010);

                    child.RuleFor(x => x.Value)
                        .Cascade(CascadeMode.Stop)
                        .Must(v => v is 0f or > 100)
                        .WithErrorCode(ErrorCode.E2072)
                        .When(x => x.PriceSettingType is PriceSettingTypes.Price && !x.IsRegardAdult);

                    child.RuleFor(x => x.Value)
                        .Cascade(CascadeMode.Stop)
                        .InclusiveBetween(1, 100)
                        .When(x => x.PriceSettingType is PriceSettingTypes.Percent && !x.IsRegardAdult)
                        .WithErrorCode(ErrorCode.E1069);
                }
            );
    }
}
