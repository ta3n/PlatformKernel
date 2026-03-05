using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Employee.WebAPI.Application.UserCases.Queries.BookingReservation;
using Liberty.SysException;

namespace Liberty.Reservation.Employee.WebAPI.Application.Validations;

public class ReservationGetAllByFilterRequestValidator
    : AbstractValidator<ReservationGetAllByFilterQuery>
{
    public ReservationGetAllByFilterRequestValidator()
    {
        RuleFor(x => x.Request.Code)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(50)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Request.Name)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Request.Kana)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Request.Address1)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002)
            .When(x => x.Request.Address1 is not null);

        RuleFor(x => x.Request.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(20)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Request.FreeInput)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(1000)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Request.StartCheckInDate)
            .Cascade(CascadeMode.Stop)
            .Must(date => ValidDate.BeValidDate(date!.Value))
            .When(x => x.Request.StartCheckInDate is not null)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions())
            .WithErrorCode(ErrorCode.E0008);

        RuleFor(x => x.Request.EndCheckInDate)
            .Cascade(CascadeMode.Stop)
            .Must(date => date.HasValue && ValidDate.BeValidDate(date.Value))
            .When(x => x.Request.EndCheckInDate is not null)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions())
            .WithErrorCode(ErrorCode.E0008)
            .Must(
                (
                        model,
                        endDate
                    ) =>
                    endDate!.Value >= model.Request.StartCheckInDate!.Value
            )
            .When(x => x.Request.EndCheckInDate is not null && x.Request.StartCheckInDate is not null)
            .WithMessage(ErrorCode.E0009.GetEnumDescriptions())
            .WithErrorCode(ErrorCode.E0009);

        RuleFor(x => x.Request.StartReservationAcceptanceDate)
            .Cascade(CascadeMode.Stop)
            .Must(date => date.HasValue && ValidDate.BeValidDate(date.Value))
            .When(x => x.Request.StartReservationAcceptanceDate is not null)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions())
            .WithErrorCode(ErrorCode.E0008);

        RuleFor(x => x.Request.EndReservationAcceptanceDate)
            .Cascade(CascadeMode.Stop)
            .Must(date => date.HasValue && ValidDate.BeValidDate(date.Value))
            .When(x => x.Request.EndReservationAcceptanceDate is not null)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions())
            .WithErrorCode(ErrorCode.E0008)
            .Must(
                (
                        model,
                        endDate
                    ) =>
                    endDate!.Value >= model.Request.StartReservationAcceptanceDate!.Value
            )
            .When(x => x.Request.EndReservationAcceptanceDate is not null && x.Request.StartReservationAcceptanceDate is not null)
            .WithMessage(ErrorCode.E0009.GetEnumDescriptions())
            .WithErrorCode(ErrorCode.E0009);

        RuleFor(x => x.Request.StartCancellationDate)
            .Cascade(CascadeMode.Stop)
            .Must(date => date.HasValue && ValidDate.BeValidDate(date.Value))
            .When(x => x.Request.StartCancellationDate is not null)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions())
            .WithErrorCode(ErrorCode.E0008);

        RuleFor(x => x.Request.EndCancellationDate)
            .Cascade(CascadeMode.Stop)
            .Must(date => date.HasValue && ValidDate.BeValidDate(date.Value))
            .When(x => x.Request.EndCancellationDate is not null)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions())
            .WithErrorCode(ErrorCode.E0008)
            .Must(
                (
                        model,
                        endDate
                    ) =>
                    endDate!.Value >= model.Request.StartCancellationDate!.Value
            )
            .When(x => x.Request.EndCancellationDate is not null && x.Request.StartCancellationDate is not null)
            .WithMessage(ErrorCode.E0009.GetEnumDescriptions())
            .WithErrorCode(ErrorCode.E0009);

        RuleForEach(x => x.Request.ReservationStatus)
            .Cascade(CascadeMode.Stop)
            .Must(status => ReservationStatusFilter.AllowedStatuses.Contains(status))
            .WithMessage(ErrorCode.E1052.GetEnumDescriptions())
            .WithErrorCode(ErrorCode.E1052);

        RuleFor(x => x.Request.StartNoShowDate)
            .Cascade(CascadeMode.Stop)
            .Must(date => date.HasValue && ValidDate.BeValidDate(date.Value))
            .When(x => x.Request.StartNoShowDate is not null)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions())
            .WithErrorCode(ErrorCode.E0008);

        RuleFor(x => x.Request.EndNoShowDate)
            .Cascade(CascadeMode.Stop)
            .Must(date => date.HasValue && ValidDate.BeValidDate(date.Value))
            .When(x => x.Request.EndNoShowDate is not null)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions())
            .WithErrorCode(ErrorCode.E0008)
            .Must(
                (
                        model,
                        endDate
                    ) =>
                    endDate!.Value >= model.Request.StartNoShowDate!.Value
            )
            .When(x => x.Request.EndNoShowDate is not null && x.Request.StartNoShowDate is not null)
            .WithMessage(ErrorCode.E0009.GetEnumDescriptions())
            .WithErrorCode(ErrorCode.E0009);
    }
}
