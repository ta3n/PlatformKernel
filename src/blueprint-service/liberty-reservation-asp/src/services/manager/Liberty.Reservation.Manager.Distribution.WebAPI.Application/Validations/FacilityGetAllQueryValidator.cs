using FluentValidation;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Rss;
using Liberty.SysException;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Validations;

public class FacilityGetAllQueryValidator : AbstractValidator<FacilityGetAllQuery>
{
    public FacilityGetAllQueryValidator()
    {
        RuleFor(x => x.Request.FacilityIds)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .Must(facilityIds => facilityIds!.Length > 0)
            .WithMessage("FacilityIds must contain at least one item.");
    }
}
