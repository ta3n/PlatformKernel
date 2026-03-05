using System.Globalization;
using FluentValidation;
using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Plan;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Web.Extensions;
using Liberty.SysException;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Validations;

public class PlanPriceQueryValidator : AbstractValidator<PlanPriceQuery>
{
    public PlanPriceQueryValidator()
    {
        RuleFor(x => x.Request.ScAgtPlanCode)
            .Cascade(CascadeMode.Stop)
            .WithBlankAwareMessage(nameof(GetPriceDataByPlanRoomRequest.ScAgtPlanCode));

        RuleFor(x => x.Request.ScAgtSiteCode)
            .Cascade(CascadeMode.Stop)
            .WithBlankAwareMessage(nameof(GetPriceDataByPlanRoomRequest.ScAgtSiteCode));

        RuleFor(x => x.Request.ScAgtRoomCode)
            .Cascade(CascadeMode.Stop)
            .WithBlankAwareMessage(nameof(GetPriceDataByPlanRoomRequest.ScAgtRoomCode));

        RuleFor(x => x.Request.AppointedDate)
            .Cascade(CascadeMode.Stop)
            .WithBlankAwareMessage(nameof(GetPriceDataByPlanRoomRequest.AppointedDate))
            .Must(
                date => DateTime.TryParseExact(
                    date,
                    "yyyyMMdd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out _
                )
            )
            .WithMessage(
                value => string.Format(
                    ErrorCode.E5003.GetEnumDescriptions(),
                    nameof(GetPriceDataByPlanRoomRequest.AppointedDate),
                    value.Request.AppointedDate
                )
            )
            .Must(
                x => ValidDate.BeAValidCheckToDay(
                    AppDate.GetDateTime(long.Parse(x!)),
                    DateTime.UtcNow.AddHours(DefaultValues.TimeZoneOffset),
                    365
                )
            )
            .WithErrorCode(ErrorCode.E5002)
            .WithMessage(
                string.Format(
                    ErrorCode.E5002.GetEnumDescriptions(),
                    AppDate.GetId(DateTime.UtcNow.AddHours(DefaultValues.TimeZoneOffset).AddDays(-365))
                )
            )
            .Must(
                x => ValidDate.BeAValidCheckToDay(
                    DateTime.UtcNow.AddHours(DefaultValues.TimeZoneOffset),
                    AppDate.GetDateTime(long.Parse(x!)),
                    365
                )
            )
            .WithErrorCode(ErrorCode.E5002)
            .WithMessage(
                string.Format(
                    ErrorCode.E5004.GetEnumDescriptions(),
                    AppDate.GetId(DateTime.UtcNow.AddHours(DefaultValues.TimeZoneOffset).AddDays(365))
                )
            );

        RuleFor(x => x.Request.AcquireDayNums)
            .Cascade(CascadeMode.Stop)
            .WithBlankAwareMessage(nameof(GetPriceDataByPlanRoomRequest.AcquireDayNums))
            .Must(value => int.TryParse(value, out _))
            .WithMessage(
                value => string.Format(
                    ErrorCode.E5003.GetEnumDescriptions(),
                    nameof(GetPriceDataByPlanRoomRequest.AcquireDayNums),
                    value.Request.AcquireDayNums
                )
            )
            .Must(value => int.TryParse(value, out var num) && num is >= 0 and <= 179)
            .WithErrorCode(ErrorCode.E5001)
            .WithMessage(x => string.Format(ErrorCode.E5001.GetEnumDescriptions(), x.Request.AcquireDayNums));
    }
}
