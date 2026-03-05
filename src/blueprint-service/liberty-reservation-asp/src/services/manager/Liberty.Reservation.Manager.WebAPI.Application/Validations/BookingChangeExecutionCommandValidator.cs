using Liberty.ApplicationShared.Extensions;
using Liberty.ApplicationShared.FluentValidationUtilities;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BookingReservation;

namespace Liberty.Reservation.Manager.WebAPI.Application.Validations;

public class BookingChangeExecutionCommandValidator : AbstractValidator<BookingChangeExecutionCommand>
{
    public BookingChangeExecutionCommandValidator()
    {
        RuleFor(x => x.Payload.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.IsAgree)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .Must(x => x)
            .WithErrorCode(ErrorCode.E1028)
            .WithMessage(ErrorCode.E1028.GetEnumDescriptions());

        RuleFor(x => x.Payload.CheckInTime)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode(ErrorCode.E0001)
            .Must(ValidDate.BeAValidTimeSpan)
            .WithErrorCode(ErrorCode.E0012)
            .WithMessage(ErrorCode.E0012.GetEnumDescriptions());

        RuleFor(x => x.Payload.NumberOfNights)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThanOrEqualTo(1)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.NumberOfRooms)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThanOrEqualTo(1)
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.FreeInput)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(1000)
            .WithErrorCode(ErrorCode.E0002);

        RuleFor(x => x.Payload.Reserver)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithErrorCode(ErrorCode.E0001);

        RuleFor(x => x.Payload.Reserver)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new ReserverOfReservationAdjustRequestValidator());

        RuleFor(x => x.Payload.MainUser!)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new GuestOfReservationAdjustRequestValidator())
            .When(x => x.Payload.MainUser is not null);

        RuleFor(x => x.Payload)
            .Must(
                request =>
                {
                    if (request.NightPeoples is null)
                    {
                        return true;
                    }

                    var availableAppDateCount = request.NightPeoples
                        .Select(x => x.AppDateId)
                        .Distinct()
                        .Count();

                    return availableAppDateCount == request.NumberOfNights;
                }
            );

        RuleFor(x => x.Payload)
            .Must(
                request =>
                {
                    if (request.NightPeoples is null)
                    {
                        return true;
                    }

                    return request.NightPeoples
                        .Select(
                            item => item.Rooms
                                .Select(x => x.RoomIndex)
                                .Distinct()
                                .Count()
                        )
                        .All(availableRoomCount => availableRoomCount == request.NumberOfRooms);
                }
            );

        RuleForEach(x => x.Payload.NightPeoples)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new RoomPeopleOfReservationAdjustRequestValidator());

        RuleFor(x => x.Payload)
            .Must(
                request =>
                {
                    if (request.NightOptions is null)
                    {
                        return true;
                    }

                    var availableAppDateCount = request.NightOptions
                        .Select(x => x.AppDateId)
                        .Distinct()
                        .Count();

                    return availableAppDateCount == request.NumberOfNights;
                }
            );

        RuleFor(x => x.Payload)
            .Must(
                request =>
                {
                    if (request.NightOptions is null)
                    {
                        return true;
                    }

                    return request.NightOptions
                        .Select(
                            item => item.Rooms
                                .Select(x => x.RoomIndex)
                                .Distinct()
                                .Count()
                        )
                        .All(availableRoomCount => availableRoomCount == request.NumberOfRooms);
                }
            );

        RuleForEach(x => x.Payload.NightOptions)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new NightOptionOfReservationAdjustRequestValidator());

        RuleFor(x => x.Payload)
            .Must(
                request =>
                {
                    if (request.RoomRepresentatives is null)
                    {
                        return true;
                    }

                    var availableCount = request.RoomRepresentatives
                        .Select(x => x.RoomIndex)
                        .Distinct()
                        .Count();

                    return availableCount == request.NumberOfRooms;
                }
            );

        RuleForEach(x => x.Payload.RoomRepresentatives)
            .Cascade(CascadeMode.Stop)
            .SetValidator(new RoomRepresentativeOfReservationAdjustRequestValidator());
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
            .WithErrorCode(ErrorCode.E0003);

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
            .NotNull()
            .WithErrorCode(ErrorCode.E0001)
            .GreaterThan(0)
            .WithErrorCode(ErrorCode.E0001)
            .Must(ValidDate.BeValidDate)
            .WithErrorCode(ErrorCode.E0008)
            .WithErrorCode(ErrorCode.E0008.GetEnumDescriptions());

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
            .WithErrorCode(ErrorCode.E0008.GetEnumDescriptions());

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
