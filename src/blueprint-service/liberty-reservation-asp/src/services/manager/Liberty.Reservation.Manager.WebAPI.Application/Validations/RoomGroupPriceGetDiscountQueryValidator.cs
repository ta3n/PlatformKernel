using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroupPrice;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class RoomGroupPriceGetDiscountQueryValidator : AbstractValidator<RoomGroupPriceGetDiscountQuery>
{
    public RoomGroupPriceGetDiscountQueryValidator()
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
