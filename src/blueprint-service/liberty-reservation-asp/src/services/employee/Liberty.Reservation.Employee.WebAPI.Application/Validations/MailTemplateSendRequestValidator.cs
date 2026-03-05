using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Application.Utils;
using Liberty.SysException;

namespace Liberty.Reservation.Employee.WebAPI.Application.Validations;

public class MailTemplateSendRequestValidator : AbstractValidator<MailTemplateSendRequest>
{
    public MailTemplateSendRequestValidator()
    {
        RuleFor(x => x.Subject)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(200)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Body)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(5000)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.ToEmail)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .EmailAddress()
            .WithErrorCode(ErrorCode.E0006)
            .Must(ValidMail.BeValidEmail)
            .WithErrorCode(ErrorCode.E0006);
    }
}
