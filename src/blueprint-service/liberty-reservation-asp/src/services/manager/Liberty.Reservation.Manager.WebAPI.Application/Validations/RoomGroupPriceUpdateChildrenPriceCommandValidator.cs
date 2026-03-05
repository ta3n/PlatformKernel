using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroupPrice;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class RoomGroupPriceUpdateChildrenPriceCommandValidator
    : AbstractValidator<RoomGroupPriceUpdateChildrenPriceCommand>
{
    public RoomGroupPriceUpdateChildrenPriceCommandValidator()
    {
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

        RuleForEach(p => p.Payload.PersonAgeTypes)
            .Cascade(CascadeMode.Stop)
            .ChildRules(
                child =>
                {
                    child.RuleFor(x => x.PersonAgeTypeId)
                        .Cascade(CascadeMode.Stop)
                        .NotNull()
                        .WithErrorCode(ErrorCode.E0001)
                        .GreaterThan(0)
                        .WithErrorCode(ErrorCode.E0001);

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
                        .WithErrorCode(ErrorCode.E0010);
                }
            );
    }
}
