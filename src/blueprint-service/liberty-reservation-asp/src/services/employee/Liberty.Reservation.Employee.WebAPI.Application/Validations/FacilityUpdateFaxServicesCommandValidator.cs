using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Facility;
using Liberty.SysException;

namespace Liberty.Reservation.Employee.WebAPI.Application.Validations;

public class FacilityUpdateFaxServicesCommandValidator : AbstractValidator<FacilityUpdateFaxServiceCommand>
{
    public FacilityUpdateFaxServicesCommandValidator()
    {
        RuleFor(x => x.Payload.Id)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);
    }
}
