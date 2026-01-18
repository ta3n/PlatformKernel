using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Commands.Permission;
using Liberty.SysException;

namespace Liberty.Reservation.Employee.WebAPI.Application.Validations;

public class PermissionCreateRequestValidator : AbstractValidator<PermissionCreateCommand>
{
    public PermissionCreateRequestValidator()
    {
        RuleFor(x => x.Payload.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .WithMessage(ErrorCode.E0001.GetEnumDescriptions())
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .WithMessage(ErrorCode.E0001.GetEnumDescriptions());

        RuleFor(x => x.Payload.GroupName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .WithMessage(ErrorCode.E0001.GetEnumDescriptions())
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .WithMessage(ErrorCode.E0001.GetEnumDescriptions());

        RuleFor(x => x.Payload.ItemType)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .WithMessage(ErrorCode.E0001.GetEnumDescriptions())
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .WithMessage(ErrorCode.E0001.GetEnumDescriptions());
    }
}
