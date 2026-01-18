using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using FluentValidation;
using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Commands.Plan;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Web.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Validations;

public partial class PlanUpdatePublishAcceptCommandValidator : AbstractValidator<PlanUpdatePublishAcceptCommand>
{
    private const string DateFormat = "yyyyMMdd";

    public PlanUpdatePublishAcceptCommandValidator()
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .WithBlankAwareMessage(nameof(UpdatePublishAcceptPlanRequest.ScAgtPlanCode));

        RuleFor(x => x.FacilityId)
            .Cascade(CascadeMode.Stop)
            .WithBlankAwareMessage(nameof(UpdatePublishAcceptPlanRequest.ScAgtFacilityCode));

        RuleFor(x => x.Payload.ReceptionLimit)
            .Cascade(CascadeMode.Stop)
            .Must(BeAValidReceptionLimit)
            .WithMessage($"{nameof(UpdatePublishAcceptPlanRequest.ReceptionLimit)} は00:00～23:59の範囲で指定してください。")
            .Must(BeAValidFormatReceptionLimit)
            .WithMessage(
                value => string.Format(
                    ErrorCode.E5003.GetEnumDescriptions(),
                    nameof(UpdatePublishAcceptPlanRequest.ReceptionLimit),
                    value.Payload.ReceptionLimit
                )
            );

        RuleFor(x => x.Payload.UseAcceptDate)
            .Cascade(CascadeMode.Stop)
            .WithBlankAwareMessage(nameof(UpdatePublishAcceptPlanRequest.UseAcceptDate))
            .Must(value => bool.TryParse(value, out _))
            .WithMessage(
                value => string.Format(
                    ErrorCode.E5003.GetEnumDescriptions(),
                    nameof(UpdatePublishAcceptPlanRequest.UseAcceptDate),
                    value.Payload.UseAcceptDate
                )
            );

        RuleFor(x => x.Payload.UseBookingReception)
            .Cascade(CascadeMode.Stop)
            .WithBlankAwareMessage(nameof(UpdatePublishAcceptPlanRequest.UseBookingReception))
            .Must(value => bool.TryParse(value, out _))
            .WithMessage(
                value => string.Format(
                    ErrorCode.E5003.GetEnumDescriptions(),
                    nameof(UpdatePublishAcceptPlanRequest.UseBookingReception),
                    value.Payload.UseBookingReception
                )
            );

        RuleFor(x => x.Payload.UseDisplayDate)
            .Cascade(CascadeMode.Stop)
            .WithBlankAwareMessage(nameof(UpdatePublishAcceptPlanRequest.UseDisplayDate))
            .Must(value => bool.TryParse(value, out _))
            .WithMessage(
                value => string.Format(
                    ErrorCode.E5003.GetEnumDescriptions(),
                    nameof(UpdatePublishAcceptPlanRequest.UseDisplayDate),
                    value.Payload.UseDisplayDate
                )
            );

        RuleFor(x => x.Payload.DisplayDateStart)
            .Cascade(CascadeMode.Stop)
            .WithBlankAwareMessage(nameof(UpdatePublishAcceptPlanRequest.DisplayDateStart))
            .Must(
                date => DateTime.TryParseExact(
                    date,
                    DateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out _
                )
            )
            .When(x => bool.TryParse(x.Payload.UseDisplayDate, out var use) && use)
            .WithMessage(
                value => string.Format(
                    ErrorCode.E5003.GetEnumDescriptions(),
                    nameof(UpdatePublishAcceptPlanRequest.DisplayDateStart),
                    value.Payload.DisplayDateStart
                )
            );

        RuleFor(x => x.Payload.AcceptDateStart)
            .Cascade(CascadeMode.Stop)
            .WithBlankAwareMessage(nameof(UpdatePublishAcceptPlanRequest.AcceptDateStart))
            .Must(
                date => DateTime.TryParseExact(
                    date,
                    DateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out _
                )
            )
            .When(x => bool.TryParse(x.Payload.UseAcceptDate, out var use) && use)
            .WithMessage(
                x =>
                    string.Format(
                        ErrorCode.E5003.GetEnumDescriptions(),
                        nameof(UpdatePublishAcceptPlanRequest.AcceptDateStart),
                        x.Payload.AcceptDateStart
                    )
            );

        RuleFor(x => x.Payload.BookingReceptionStart)
            .Cascade(CascadeMode.Stop)
            .WithBlankAwareMessage(nameof(UpdatePublishAcceptPlanRequest.BookingReceptionStart))
            .Must(
                date => DateTime.TryParseExact(
                    date,
                    DateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out _
                )
            )
            .When(x => bool.TryParse(x.Payload.UseBookingReception, out var use) && use)
            .WithMessage(
                x =>
                    string.Format(
                        ErrorCode.E5003.GetEnumDescriptions(),
                        nameof(UpdatePublishAcceptPlanRequest.BookingReceptionStart),
                        x.Payload.BookingReceptionStart
                    )
            );

        RuleFor(x => x.Payload.DisplayDateEnd)
            .Cascade(CascadeMode.Stop)
            .WithBlankAwareMessage(nameof(UpdatePublishAcceptPlanRequest.DisplayDateEnd))
            .Must(
                date => DateTime.TryParseExact(
                    date,
                    DateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out _
                )
            )
            .When(x => bool.TryParse(x.Payload.UseDisplayDate, out var use) && use)
            .WithMessage(
                value => string.Format(
                    ErrorCode.E5003.GetEnumDescriptions(),
                    nameof(UpdatePublishAcceptPlanRequest.DisplayDateEnd),
                    value.Payload.DisplayDateEnd
                )
            );

        RuleFor(x => x.Payload.AcceptDateEnd)
            .Cascade(CascadeMode.Stop)
            .WithBlankAwareMessage(nameof(UpdatePublishAcceptPlanRequest.AcceptDateEnd))
            .Must(
                date => DateTime.TryParseExact(
                    date,
                    DateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out _
                )
            )
            .When(x => bool.TryParse(x.Payload.UseAcceptDate, out var use) && use)
            .WithMessage(
                x =>
                    string.Format(
                        ErrorCode.E5003.GetEnumDescriptions(),
                        nameof(UpdatePublishAcceptPlanRequest.AcceptDateEnd),
                        x.Payload.AcceptDateEnd
                    )
            );

        RuleFor(x => x.Payload.BookingReceptionEnd)
            .Cascade(CascadeMode.Stop)
            .WithBlankAwareMessage(nameof(UpdatePublishAcceptPlanRequest.BookingReceptionEnd))
            .Must(
                date => DateTime.TryParseExact(
                    date,
                    DateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out _
                )
            )
            .When(x => bool.TryParse(x.Payload.UseBookingReception, out var use) && use)
            .WithMessage(
                x =>
                    string.Format(
                        ErrorCode.E5003.GetEnumDescriptions(),
                        nameof(UpdatePublishAcceptPlanRequest.BookingReceptionEnd),
                        x.Payload.BookingReceptionEnd
                    )
            );

        ValidateDateRange(
            x => x.Payload,
            p => p.UseDisplayDate,
            p => p.DisplayDateStart,
            p => p.DisplayDateEnd,
            nameof(UpdatePublishAcceptPlanRequest.DisplayDateEnd),
            nameof(UpdatePublishAcceptPlanRequest.DisplayDateStart)
        );

        ValidateDateRange(
            x => x.Payload,
            p => p.UseAcceptDate,
            p => p.AcceptDateStart,
            p => p.AcceptDateEnd,
            nameof(UpdatePublishAcceptPlanRequest.AcceptDateEnd),
            nameof(UpdatePublishAcceptPlanRequest.AcceptDateStart)
        );

        ValidateDateRange(
            x => x.Payload,
            p => p.UseBookingReception,
            p => p.BookingReceptionStart,
            p => p.BookingReceptionEnd,
            nameof(UpdatePublishAcceptPlanRequest.BookingReceptionEnd),
            nameof(UpdatePublishAcceptPlanRequest.BookingReceptionStart)
        );

        RuleFor(x => x.Payload.ReceptionDayLimit)
            .Cascade(CascadeMode.Stop)
            .Must(IsValidReceptionDayLimit)
            .WithErrorCode(ErrorCode.E5005)
            .WithMessage(string.Format(ErrorCode.E5005.GetEnumDescriptions(), nameof(UpdatePublishAcceptPlanRequest.ReceptionDayLimit)));
    }

    private void ValidateDateRange<T>(
        Expression<Func<PlanUpdatePublishAcceptCommand, T>> payloadExpr,
        Func<T, string?> useFlag,
        Func<T, string?> startDate,
        Func<T, string?> endDate,
        string displayName,
        string displayStartDate
    )
    {
        RuleFor(payloadExpr)
            .Cascade(CascadeMode.Stop)
            .Must(
                payload =>
                {
                    _ = bool.TryParse(useFlag(payload), out var useFlagBool);
                    if (!useFlagBool
                        || string.IsNullOrEmpty(startDate(payload))
                        || string.IsNullOrEmpty(endDate(payload)))
                    {
                        return true;
                    }

                    return long.TryParse(startDate(payload), out var start)
                        && long.TryParse(endDate(payload), out var end)
                        && end >= start;
                }
            )
            .WithName(displayName)
            .WithErrorCode(ErrorCode.E5006)
            .WithMessage(string.Format(ErrorCode.E5006.GetEnumDescriptions(), displayName, displayStartDate));
    }

    private static bool IsValidReceptionDayLimit(
        string? value
    )
    {
        if (string.IsNullOrEmpty(value))
        {
            return true;
        }

        return int.TryParse(value, out var day) && day is >= 0 and <= 60;
    }

    private static bool BeAValidReceptionLimit(
        string? value
    )
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        return TimeSpan.TryParse(
                value,
                CultureInfo.InvariantCulture,
                out var time
            )
            && time >= TimeSpan.Zero
            && time < TimeSpan.FromDays(1);
    }

    private static bool BeAValidFormatReceptionLimit(
        string? value
    )
    {
        return string.IsNullOrWhiteSpace(value) || RegexReceptionLimit().IsMatch(value);
    }

    [GeneratedRegex(@"^(?:[01]\d|2[0-3]):[0-5]\d$")]
    private static partial Regex RegexReceptionLimit();
}
