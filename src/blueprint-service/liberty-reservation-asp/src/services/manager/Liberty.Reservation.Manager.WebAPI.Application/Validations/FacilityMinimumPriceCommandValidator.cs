using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class FacilityMinimumPriceCommandValidator : AbstractValidator<FacilityUpdateMinimumPriceCommand>
{
    public FacilityMinimumPriceCommandValidator()
    {
        RuleFor(x => x.Payload.IsEnabledMinimumPrice)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001);
    }
}
