using FluentValidation;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Settings;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Kakusan;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Validations;

public class KakusanGetAllRoomQueryValidator : AbstractValidator<KakusanGetAllRoomQuery>
{
    private readonly C002Setting _setting;

    public KakusanGetAllRoomQueryValidator(
        C002Setting setting
    )
    {
        _setting = setting;

        RuleFor(x => x.Request.HotelIds)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Request)
            .Must(IsValidatedParamsOfGetRooms)
            .WithMessage("Invalid parameters for GetRoomsRequest");
    }

    private bool IsValidatedParamsOfGetRooms(
        GetRoomsRequest? request
    )
    {
        if (request is null)
        {
            return false;
        }

        var today = AppDate.GetId(DateTime.Now.Date);
        var nextYearDay = AppDate.GetId(DateTime.Now.Date.AddYears(1));
        var diffDays = (request.ToDay - request.FromDay).TotalDays + 1;

        return request.HotelIds.Count >= _setting.HotelCountMin
            && request.HotelIds.Count <= _setting.HotelCountMax
            && today <= request.FromAppDateId
            && diffDays >= _setting.RangeDaysMin
            && diffDays <= _setting.RangeDaysMax
            && request.ToAppDateId <= nextYearDay;
    }
}
