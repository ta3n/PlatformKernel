using FluentValidation;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Plan;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Web.Extensions;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Validations;

public class PlanQueryValidator : AbstractValidator<PlanGetQuery>
{
    public PlanQueryValidator()
    {
        RuleFor(x => x.Request.ScAgtFacilityCode)
            .Cascade(CascadeMode.Stop)
            .WithBlankAwareMessage(nameof(GetPlanRequest.ScAgtFacilityCode));
    }
}
