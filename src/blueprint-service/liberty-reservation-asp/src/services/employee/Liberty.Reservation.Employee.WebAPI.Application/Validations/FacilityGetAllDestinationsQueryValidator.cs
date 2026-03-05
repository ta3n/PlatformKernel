using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.Facility;
using Liberty.SysException;

namespace Liberty.Reservation.Employee.WebAPI.Application.Validations;

public class FacilityGetAllDestinationsQueryValidator : AbstractValidator<FacilityGetAllDestinationsQuery>
{
    public FacilityGetAllDestinationsQueryValidator()
    {
        RuleFor(x => x.FacilityId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0003);
    }
}
