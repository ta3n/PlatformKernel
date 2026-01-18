using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroup;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class RoomGroupGetPublicationSettingQueryValidator : AbstractValidator<RoomGroupGetDisplaySettingQuery>
{
    public RoomGroupGetPublicationSettingQueryValidator()
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);
    }
}
