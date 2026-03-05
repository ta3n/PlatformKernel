using System.Text.Json;
using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Utils;
using Liberty.SysException;

namespace Liberty.Reservation.Employee.WebAPI.Application.Validations;

public class MailTemplateUpdateRequestValidator : AbstractValidator<MailTemplateUpdateRequest>
{
    public MailTemplateUpdateRequestValidator()
    {
        RuleFor(x => x.IoType)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Format)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .Must(
                value =>
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        return false;
                    }

                    try
                    {
                        // Try to parse the string to a JsonDocument
                        using var doc = JsonDocument.Parse(value);
                        // Successfully parsed JSON
                        return true;
                    }
                    catch (JsonException)
                    {
                        // Parsing failed, not valid JSON
                        return false;
                    }
                }
            )
            .WithErrorCode(ErrorCode.E0007)
            .WithMessage(ErrorCode.E0007.GetEnumDescriptions())
            .MaximumLength(5000)
            .WithErrorCode(ErrorCode.E0002);
    }
}
