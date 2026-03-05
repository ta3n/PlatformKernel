using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.Facility;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class FacilityUpdateBasicSettingCommandValidator : AbstractValidator<FacilityUpdateBasicSettingCommand>
{
    public FacilityUpdateBasicSettingCommandValidator()
    {
        RuleFor(x => x.Payload.Description)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(10000)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.Fax)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(250)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.Name)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(250)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.Kana)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(250)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.Address1)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(250)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.Address2)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(250)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.Address3)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(250)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.Postcode)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(250)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(user => user.Payload.Phone)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(250)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.Url)
            .Cascade(CascadeMode.Stop)
            .Must(ValidUrl.BeValidUrl)
            .When(x => !string.IsNullOrEmpty(x.Payload.Url))
            .WithErrorCode(ErrorCode.E0005)
            .WithMessage(ErrorCode.E0005.GetEnumDescriptions());

        RuleFor(x => x.Payload.AreaId)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.FacilityTypeId)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.RoomNumberWesternStyle)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0011)
            .LessThanOrEqualTo(999)
            .WithErrorCode(ErrorCode.E0004);

        RuleFor(x => x.Payload.RoomNumberJapaneseStyle)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0011)
            .LessThanOrEqualTo(999)
            .WithErrorCode(ErrorCode.E0004);

        RuleFor(x => x.Payload.RoomNumberJapaneseWesternStyle)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0011)
            .LessThanOrEqualTo(999)
            .WithErrorCode(ErrorCode.E0004);

        RuleFor(x => x.Payload.RoomNumberOtherStyle)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0011)
            .LessThanOrEqualTo(999)
            .WithErrorCode(ErrorCode.E0004);
    }
}
