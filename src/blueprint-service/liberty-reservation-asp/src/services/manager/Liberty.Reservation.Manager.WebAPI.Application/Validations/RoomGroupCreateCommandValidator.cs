using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class RoomGroupCreateCommandValidator : AbstractValidator<RoomGroupCreateCommand>
{
    public RoomGroupCreateCommandValidator()
    {
        RuleFor(x => x.Payload.Name)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.BaseNumber)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0003);

        RuleFor(x => x.Payload.CapacityMin)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0003);

        RuleFor(x => x.Payload.CapacityMax)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThanOrEqualTo(x => x.Payload.CapacityMin)
            .WithErrorCode(ErrorCode.E1021);
    }
}
