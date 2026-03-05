using FluentValidation;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.Settings;
using Liberty.Reservation.Manager.Distribution.WebAPI.Application.UserCases.Queries.Kakusan;

namespace Liberty.Reservation.Manager.Distribution.WebAPI.Application.Validations;

public class KakusanGetAllRoomTypeQueryValidator : AbstractValidator<KakusanGetAllRoomTypeQuery>
{
    private readonly C001Setting _setting;

    public KakusanGetAllRoomTypeQueryValidator(
        C001Setting setting
    )
    {
        _setting = setting;

        RuleFor(x => x.Request.HotelIds)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Request)
            .Must(IsValidatedParamsOfGetRoomType)
            .WithMessage("Invalid parameters for GetRoomTypeRequest");
    }

    private bool IsValidatedParamsOfGetRoomType(
        RoomTypeRequest? request
    )
    {
        if (request is null)
        {
            return false;
        }

        return request.HotelIds.Count >= _setting.HotelCountMin && request.HotelIds.Count <= _setting.HotelCountMax;
    }
}
