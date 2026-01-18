using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class RoomGroupUpdateDisplaySettingCommandValidator
    : AbstractValidator<RoomGroupUpdateDisplaySettingCommand>
{
    public RoomGroupUpdateDisplaySettingCommandValidator()
    {
        RuleFor(x => x.Payload.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.RoomGroupMasterCategoryIds)
            .Cascade(CascadeMode.Stop)
            .Must(x => x is null || x!.ToList().TrueForAll(y => y > 0))
            .WithErrorCode(ErrorCode.E0010)
            .WithMessage(ErrorCode.E0010.GetEnumDescriptions());

        RuleFor(x => x.Payload.RoomGroupCategoryIds)
            .Cascade(CascadeMode.Stop)
            .Must(x => x is null || x!.ToList().TrueForAll(y => y > 0))
            .WithErrorCode(ErrorCode.E0010)
            .WithMessage(ErrorCode.E0010.GetEnumDescriptions());

        RuleFor(x => x.Payload.RoomGroupFeatureCategoryIds)
            .Cascade(CascadeMode.Stop)
            .Must(x => x is null || x!.ToList().TrueForAll(y => y > 0))
            .WithErrorCode(ErrorCode.E0010)
            .WithMessage(ErrorCode.E0010.GetEnumDescriptions());

        RuleFor(x => x.Payload.RoomGroupEquipmentCategoryIds)
            .Cascade(CascadeMode.Stop)
            .Must(x => x is null || x!.ToList().TrueForAll(y => y > 0))
            .WithErrorCode(ErrorCode.E0010)
            .WithMessage(ErrorCode.E0010.GetEnumDescriptions());

        RuleFor(x => x.Payload.RoomAmenityCategoryIds)
            .Cascade(CascadeMode.Stop)
            .Must(x => x is null || x!.ToList().TrueForAll(y => y > 0))
            .WithErrorCode(ErrorCode.E0010)
            .WithMessage(ErrorCode.E0010.GetEnumDescriptions());

        RuleForEach(x => x.Payload.Tags)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .When(x => x.Payload.Tags is not null)
            .WithErrorCode(ErrorCode.E0001);
    }
}
