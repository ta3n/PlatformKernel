using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class RoomGroupDeleteCommandValidator : AbstractValidator<RoomGroupDeleteCommand>
{
    public RoomGroupDeleteCommandValidator()
    {
        RuleFor(x => x.Payload.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);
    }
}
