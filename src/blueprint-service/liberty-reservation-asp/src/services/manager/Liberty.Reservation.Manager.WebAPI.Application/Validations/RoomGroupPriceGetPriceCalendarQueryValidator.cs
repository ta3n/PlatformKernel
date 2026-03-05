using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroupPrice;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class RoomGroupPriceGetPriceCalendarQueryValidator : AbstractValidator<RoomGroupPriceGetPriceCalendarQuery>
{
    public RoomGroupPriceGetPriceCalendarQueryValidator()
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

        RuleFor(x => x.StartDate)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.StartDate)
            .Cascade(CascadeMode.Stop)
            .Must(ValidDate.BeValidDate)
            .WithErrorCode(ErrorCode.E0008)
            .WithErrorCode(ErrorCode.E0008.GetEnumDescriptions());

        RuleFor(x => x.EndDate)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(x => x.StartDate)
            .WithErrorCode(ErrorCode.E0009);

        RuleFor(x => x.EndDate)
            .Cascade(CascadeMode.Stop)
            .Must(ValidDate.BeValidDate)
            .WithErrorCode(ErrorCode.E0008)
            .WithErrorCode(ErrorCode.E0008.GetEnumDescriptions());
    }
}
