using Liberty.ApplicationShared.Extensions;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Commands.Booking;
using Liberty.SysException;

namespace Liberty.Reservation.Site.WebAPI.Application.Validations;

public class BookingCreateCommandValidator : AbstractValidator<BookingCreateCommand>
{
    public BookingCreateCommandValidator()
    {
        RuleFor(x => x.PlanId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.RoomGroupId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.CheckInDate)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .Must(ValidDate.BeValidDate)
            .WithErrorCode(ErrorCode.E0008)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions())
            .Must(
                checkInDate =>
                {
                    var dateNowId = AppDate.GetId(DateTime.Now);
                    return checkInDate >= dateNowId;
                }
            );

        RuleFor(x => x.Payload.Adjust.IsAgree)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .Must(x => x)
            .WithErrorCode(ErrorCode.E2041);

        RuleFor(x => x.Payload.CheckInTime)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.Adjust.NumberOfNights)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThanOrEqualTo(1)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.Adjust.NumberOfRooms)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThanOrEqualTo(1)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.Adjust.FreeInput)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(1000)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.Adjust.Reserver)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.Adjust.Reserver)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new ReserverOfReservationAdjustRequestValidator());

        RuleFor(x => x.Payload.Adjust.MainUser!)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new GuestOfReservationAdjustRequestValidator())
            .When(x => x.Payload.Adjust.MainUser is not null);

        RuleFor(x => x.Payload)
            .Must(
                request =>
                {
                    if (request.Adjust.NightPeoples is null)
                    {
                        return true;
                    }

                    var availableAppDateCount = request.Adjust
                        .NightPeoples
                        .Select(x => x.AppDateId)
                        .Distinct()
                        .Count();

                    return availableAppDateCount == request.Adjust.NumberOfNights;
                }
            )
            .WithErrorCode(ErrorCode.E2053)
            .WithMessage(ErrorCode.E2053.GetEnumDescriptions());

        RuleFor(x => x.Payload)
            .Must(
                request =>
                {
                    if (request.Adjust.NightPeoples is null)
                    {
                        return true;
                    }

                    return request.Adjust
                        .NightPeoples
                        .Select(
                            item => item.Rooms
                                .Select(x => x.RoomIndex)
                                .Distinct()
                                .Count()
                        )
                        .All(availableRoomCount => availableRoomCount == request.Adjust.NumberOfRooms);
                }
            )
            .WithErrorCode(ErrorCode.E2054)
            .WithMessage(ErrorCode.E2054.GetEnumDescriptions());

        RuleForEach(x => x.Payload.Adjust.NightPeoples)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new RoomPeopleOfReservationAdjustRequestValidator());

        RuleForEach(x => x.Payload.Adjust.NightOptions)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new NightOptionOfReservationAdjustRequestValidator());

        RuleFor(x => x.Payload)
            .Must(
                request =>
                {
                    if (request.Adjust.RoomRepresentatives is null)
                    {
                        return true;
                    }

                    var availableCount = request.Adjust
                        .RoomRepresentatives
                        .Select(x => x.RoomIndex)
                        .Distinct()
                        .Count();

                    return availableCount == request.Adjust.NumberOfRooms;
                }
            )
            .WithErrorCode(ErrorCode.E2055)
            .WithMessage(ErrorCode.E2055.GetEnumDescriptions());

        RuleForEach(x => x.Payload.Adjust.RoomRepresentatives)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new RoomRepresentativeOfReservationAdjustRequestValidator());

        RuleFor(x => (int)x.Payload.PaymentType)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload)
            .Must(
                request =>
                {
                    if (request.PaymentType is PaymentTypes.OnLinePayment)
                    {
                        return BeWithinOnlinePaymentLimit(request.CheckInDate);
                    }

                    return true;
                }
            )
            .WithErrorCode(ErrorCode.E2069)
            .WithMessage(
                _ => string.Format(
                    ErrorCode.E2069.GetEnumDescriptions(),
                    DefaultValues.OnlinePaymentDayLimit
                )
            );
    }

    private static bool BeWithinOnlinePaymentLimit(
        long checkInDateId
    )
    {
        var checkInDate = AppDate.GetDateTime(checkInDateId).Date;
        return BookingCheckAvailableService.IsWithinOnlinePaymentLimit(checkInDate);
    }
}

public class ReserverOfReservationAdjustRequestValidator : AbstractValidator<ReserverOfReservationAdjustRequest>
{
    public ReserverOfReservationAdjustRequestValidator()
    {
        RuleFor(x => x.FullName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Kana)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .EmailAddress()
            .WithErrorCode(ErrorCode.E0006);

        RuleFor(x => x.PostCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.CountryCode)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002)
            .When(x => x.CountryCode is not null);

        RuleFor(x => x.Address1)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Address2)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Address3)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002)
            .When(x => x.Address3 is not null);

        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(20)
            .WithErrorCode(ErrorCode.E0002);
    }
}

public class GuestOfReservationAdjustRequestValidator : AbstractValidator<GuestOfReservationAdjustRequest>
{
    public GuestOfReservationAdjustRequestValidator()
    {
        RuleFor(x => x.FullName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Kana)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.PostCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.CountryCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Address1)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Address2)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Address3)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.PhoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(20)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Birthday)
            .Cascade(CascadeMode.Stop)
            .Must(x => x is null)
            .WithErrorCode(ErrorCode.E0100);
    }
}

public class RoomRepresentativeOfReservationAdjustRequestValidator
    : AbstractValidator<RoomRepresentativeOfReservationAdjustRequest>
{
    public RoomRepresentativeOfReservationAdjustRequestValidator()
    {
        RuleFor(x => x.RoomIndex)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0010);

        RuleFor(x => x.FullName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Kana)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(255)
            .WithErrorCode(ErrorCode.E0002);
    }
}

public class RoomPeopleOfReservationAdjustRequestValidator : AbstractValidator<NightPeopleOfReservationAdjustRequest>
{
    public RoomPeopleOfReservationAdjustRequestValidator()
    {
        RuleFor(x => x.AppDateId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001)
            .Must(ValidDate.BeValidDate)
            .WithErrorCode(ErrorCode.E0008)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions());

        RuleForEach(x => x.Rooms)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new RoomOfReservationAdjustRequestValidator());
    }
}

public class RoomOfReservationAdjustRequestValidator : AbstractValidator<RoomNightOfReservationAdjustRequest>
{
    public RoomOfReservationAdjustRequestValidator()
    {
        RuleFor(x => x.RoomIndex)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0010);

        RuleForEach(x => x.Peoples)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new PeopleOfReservationAdjustRequestValidator());
    }
}

public class PeopleOfReservationAdjustRequestValidator : AbstractValidator<PeopleOfReservationAdjustRequest>
{
    public PeopleOfReservationAdjustRequestValidator()
    {
        RuleFor(x => x.PersonAgeTypeId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.NumberOfPeoples)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThanOrEqualTo(1)
            .WithErrorCode(ErrorCode.E0001);
    }
}

public class NightOptionOfReservationAdjustRequestValidator : AbstractValidator<NightOptionOfReservationAdjustRequest>
{
    public NightOptionOfReservationAdjustRequestValidator()
    {
        RuleFor(x => x.AppDateId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001)
            .Must(ValidDate.BeValidDate)
            .WithErrorCode(ErrorCode.E0008)
            .WithMessage(ErrorCode.E0008.GetEnumDescriptions());

        RuleForEach(x => x.Rooms)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new RoomOptionOfReservationAdjustRequestValidator());
    }
}

public class RoomOptionOfReservationAdjustRequestValidator : AbstractValidator<RoomOptionOfReservationAdjustRequest>
{
    public RoomOptionOfReservationAdjustRequestValidator()
    {
        RuleFor(x => x.RoomIndex)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0010);

        RuleForEach(x => x.OptionItems)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new OptionOfReservationAdjustRequestValidator());
    }
}

public class OptionOfReservationAdjustRequestValidator : AbstractValidator<OptionOfReservationAdjustRequest>
{
    public OptionOfReservationAdjustRequestValidator()
    {
        RuleFor(x => x.OptionItemId)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThanOrEqualTo(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Number)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThanOrEqualTo(1)
            .WithErrorCode(ErrorCode.E0001);
    }
}
