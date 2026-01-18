using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Application.Utils;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class PlanUpdateSaleCommandValidator : AbstractValidator<PlanUpdateSaleCommand>
{
    public PlanUpdateSaleCommandValidator()
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.CheckInStart)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .Must(
                (
                    _,
                    time
                ) => ValidDate.BeAValidWithinLimit(time, 29)
            )
            .WithErrorCode(ErrorCode.E0012)
            .WithMessage(ErrorCode.E0012.GetEnumDescriptions());

        RuleFor(x => x.Payload.CheckInEnd)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .Must(
                (
                    _,
                    time
                ) => ValidDate.BeAValidWithinLimit(time, 29)
            )
            .WithErrorCode(ErrorCode.E0012)
            .WithMessage(ErrorCode.E0012.GetEnumDescriptions())
            .GreaterThanOrEqualTo(x => x.Payload.CheckInStart)
            .WithErrorCode(ErrorCode.E1020);

        RuleFor(x => x.Payload.CheckOut)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .Must(ValidDate.BeAValidTimeSpan)
            .WithErrorCode(ErrorCode.E0012)
            .WithMessage(ErrorCode.E0012.GetEnumDescriptions());

        RuleFor(x => x.Payload.TimeIntervalMinutes)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.PlanDaySaleLimitType)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .Must(types => types is PlanDaySaleLimitTypes.RoomGroup)
            .WithErrorCode(ErrorCode.E0100)
            .WithMessage(ErrorCode.E0100.GetEnumDescriptions());

        RuleFor(x => x.Payload.UseAcceptPersonNumber)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .Must(types => types is false)
            .WithErrorCode(ErrorCode.E0100)
            .WithMessage(ErrorCode.E0100.GetEnumDescriptions());

        RuleFor(x => x.Payload.AcceptPersonNumberMin)
            .Cascade(CascadeMode.Stop)
            .Must(
                (
                        model,
                        value
                    ) =>
                    model.Payload.UseAcceptPersonNumber is false && value is null
            )
            .WithErrorCode(ErrorCode.E0100)
            .WithMessage(ErrorCode.E0100.GetEnumDescriptions());

        RuleFor(x => x.Payload.AcceptPersonNumberMax)
            .Cascade(CascadeMode.Stop)
            .Must(
                (
                        model,
                        value
                    ) =>
                    model.Payload.UseAcceptPersonNumber is false && value is null
            )
            .WithErrorCode(ErrorCode.E0100)
            .WithMessage(ErrorCode.E0100.GetEnumDescriptions());

        RuleFor(x => x.Payload.GroupNumberDaySaleLimit)
            .Cascade(CascadeMode.Stop)
            .Must(
                (
                        model,
                        value
                    ) =>
                    model.Payload.PlanDaySaleLimitType is PlanDaySaleLimitTypes.RoomGroup && value is null
            )
            .WithErrorCode(ErrorCode.E0100)
            .WithMessage(ErrorCode.E0100.GetEnumDescriptions());
    }
}
