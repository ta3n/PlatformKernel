using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class FacilityUpdateAccessCommandValidator : AbstractValidator<FacilityUpdateAccessCommand>
{
    public FacilityUpdateAccessCommandValidator()
    {
        RuleFor(x => x.Payload.AccessInfoComment)
            .MaximumLength(1000)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.ParkingInfoComment)
            .MaximumLength(1000)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.TransferComment)
            .MaximumLength(1000)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.NearStationInfoComment)
            .MaximumLength(1000)
            .WithErrorCode(ErrorCode.E0002);
    }
}
