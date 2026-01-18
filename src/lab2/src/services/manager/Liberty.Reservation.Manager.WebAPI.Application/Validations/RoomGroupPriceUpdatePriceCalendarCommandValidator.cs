using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroupPrice;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class RoomGroupPriceUpdatePriceCalendarCommandValidator
    : AbstractValidator<RoomGroupPriceUpdatePriceCalendarCommand>
{
    public RoomGroupPriceUpdatePriceCalendarCommandValidator()
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
    }
}
