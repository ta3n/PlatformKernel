using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.RoomGroup;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class RoomGroupUpdateBasicConfigurationCommandValidator
    : AbstractValidator<RoomGroupUpdateBasicConfigurationCommand>
{
    public RoomGroupUpdateBasicConfigurationCommandValidator()
    {
        RuleFor(x => x.Payload.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.GroupName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.BaseNumber)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0011);

        RuleFor(x => x.Payload.CapacityMin)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0011);

        RuleFor(x => x.Payload.CapacityMax)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(x => x.Payload.CapacityMin)
            .WithErrorCode(ErrorCode.E1021);

        RuleFor(x => x.Payload.Size)
            .Cascade(CascadeMode.Stop)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0011);

        RuleFor(x => x.Payload.BedTypes)
            .Cascade(CascadeMode.Stop)
            .Must(x => x.Any())
            .WithErrorCode(ErrorCode.E0001);

        RuleForEach(x => x.Payload.BedTypes)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new BedTypeOfRoomGroupUpdateBasicConfigurationRequestValidator());
    }
}

public class BedTypeOfRoomGroupUpdateBasicConfigurationRequestValidator
    : AbstractValidator<BedTypeOfRoomGroupUpdateBasicConfigurationRequest>
{
    public BedTypeOfRoomGroupUpdateBasicConfigurationRequestValidator()
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Number)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0011);
    }
}
