using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroupInventory;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class RoomGroupInventoryAdjustCommandValidator : AbstractValidator<RoomGroupInventoryAdjustCommand>
{
    public RoomGroupInventoryAdjustCommandValidator()
    {
        RuleFor(x => x.Payload)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .Must(x => x.Any())
            .WithErrorCode(ErrorCode.E0001);

        RuleForEach(x => x.Payload)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new RoomGroupChangeRemainRequestValidator());
    }
}

public class RoomGroupChangeRemainRequestValidator : AbstractValidator<RoomGroupChangeRemainRequest>
{
    public RoomGroupChangeRemainRequestValidator()
    {
        RuleFor(x => x.AppDateId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001)
            .Must(ValidDate.BeValidDate)
            .WithErrorCode(ErrorCode.E0008)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions());

        RuleFor(x => x.RoomGroupId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);
    }
}
