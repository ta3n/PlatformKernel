using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.MailTemplate;
using Liberty.SysException;

namespace Liberty.Reservation.Employee.WebAPI.Application.Validations;

public class MailTemplateGetQueryValidator : AbstractValidator<MailTemplateGetQuery>
{
    public MailTemplateGetQueryValidator()
    {
        RuleFor(x => x.IoType)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001);
    }
}
