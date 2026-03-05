using System.Text.RegularExpressions;
using FluentValidation;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Settings;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Kakusan;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Validations;

public partial class KakusanGetAllBookingQueryValidator : AbstractValidator<KakusanGetAllBookingQuery>
{
    private readonly C003Setting _setting;

    public KakusanGetAllBookingQueryValidator(
        C003Setting setting
    )
    {
        _setting = setting;

        RuleFor(x => x.Request.ConvertOnlyFromDay)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(CheckFormatDateTime);

        RuleFor(x => x.Request.ConvertOnlyToDay)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(CheckFormatDateTime);

        RuleFor(x => x.Request.ConvertOnlyFromArriveDay)
            .Cascade(CascadeMode.Stop)
            .Must(BeValidDateTimeFormat);

        RuleFor(x => x.Request.ConvertOnlyToArriveDay)
            .Cascade(CascadeMode.Stop)
            .Must(BeValidDateTimeFormat);

        RuleFor(x => x.Request)
            .Cascade(CascadeMode.Stop)
            .Must(CheckFromDateAndToDateSameValue);

        RuleFor(x => x.Request.HotelIds)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Request.DateType)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Request)
            .Cascade(CascadeMode.Stop)
            .Must(IsValidatedParamsOfGetBooking)
            .WithMessage("Invalid parameters for GetBookingRequest");
    }

    private bool IsValidatedParamsOfGetBooking(
        GetBookingRequest? request
    )
    {
        try
        {
            if (request is null)
            {
                return false;
            }

            if (!(request.HotelIds.Count >= _setting.HotelCountMin && request.HotelIds.Count <= _setting.HotelCountMax))
            {
                return false;
            }

            var fromDay = request.GetFromDay();

            if (DateTime.Now.Date.AddYears(-1) > fromDay)
            {
                return false;
            }

            if (request.GetFromDay() > request.GetToDay())
            {
                return false;
            }

            var rangeDays = Math.Round((request.GetToDay() - request.GetFromDay()).TotalDays) + 1;
            if (rangeDays > _setting.RangeDaysMax || rangeDays < _setting.RangeDaysMin)
            {
                return false;
            }

            if (request.GetFromArriveDay() > request.GetToArriveDay())
            {
                return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool CheckFormatDateTime(
        string input
    )
    {
        return DateTimeRegex().IsMatch(input);
    }

    [GeneratedRegex(@"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}$")]
    private static partial Regex DateTimeRegex();

    private static bool BeValidDateTimeFormat(
        string? input
    )
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return true;
        }

        return DateTimeRegex().IsMatch(input);
    }

    private static bool CheckFromDateAndToDateSameValue(
        GetBookingRequest? request
    )
    {
        if (request is null)
        {
            return false;
        }

        return string.IsNullOrEmpty(request.ConvertOnlyFromArriveDay) == string.IsNullOrEmpty(request.ConvertOnlyToArriveDay);
    }
}
