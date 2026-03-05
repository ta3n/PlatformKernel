using Liberty.Reservation.Application.Utils;
using Liberty.SysException;

namespace Liberty.Reservation.Employee.WebAPI.Application.Validations;

public class AppDateTypeEnabledRequestValidator : AbstractValidator<AppDateTypeEnabledRequest>
{
    public AppDateTypeEnabledRequestValidator()
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);
    }
}
