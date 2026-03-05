using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.RoomGroup;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class RoomGroupGetBasicConfigurationQueryValidator : AbstractValidator<RoomGroupGetBasicConfigurationQuery>
{
    public RoomGroupGetBasicConfigurationQueryValidator()
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);
    }
}
