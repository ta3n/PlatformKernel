using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroupInventory;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class RoomGroupInventoryGetAllQueryValidator : AbstractValidator<RoomGroupInventoryGetAllQuery>
{
    public RoomGroupInventoryGetAllQueryValidator()
    {
        RuleFor(x => x.StartAppDate)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.StartAppDate)
            .Cascade(CascadeMode.Stop)
            .Must(ValidDate.BeValidDate)
            .WithErrorCode(ErrorCode.E0008)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions());

        RuleFor(x => x.EndAppDate)
            .Cascade(CascadeMode.Stop)
            .Must(ValidDate.BeValidDate)
            .WithErrorCode(ErrorCode.E0008)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions());

        RuleFor(x => x.EndAppDate)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThanOrEqualTo(x => x.StartAppDate)
            .WithErrorCode(ErrorCode.E0009);
    }
}
