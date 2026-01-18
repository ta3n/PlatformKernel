using Liberty.Pagination;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.User.WebAPI.Application.Models.Requests;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Commands.BookingReservation;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;
using Liberty.Reservation.User.WebAPI.Application.Validations;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest;
using Liberty.SysException;

namespace Liberty.Reservation.User.WebAPI.Test.UnitTests;

public class ReservationUserValidationUnitTest : BaseUnitTest
{
    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenIdIsEmpty()
    {
        // Arrange
        var checkInDate = DateTime.Now;
        var bookingAdjustReq = new BookingAdjustRequest(
            true,
            "14:00",
            1,
            2,
            "Free input",
            new ReserverOfReservationAdjustRequest(
                "Reserver full name",
                "Reserver kana",
                Genders.None,
                "test@liberty.com",
                "014574",
                "Country",
                "Reserver address 1",
                "Reserver address 2",
                "Reserver address 3",
                "0214155455"
            ),
            new GuestOfReservationAdjustRequest(
                "Main user full name",
                "Main user kana",
                Genders.Male,
                AppDate.GetId(
                    new(
                        1993,
                        1,
                        1,
                        0,
                        0,
                        0,
                        DateTimeKind.Local
                    )
                ),
                "014574",
                "Country",
                "Main user address 1",
                "Main user address 2",
                "Main user address 3",
                "0214155455"
            ),
            [
                new NightPeopleOfReservationAdjustRequest(
                    AppDate.GetId(checkInDate),
                    [
                        new RoomNightOfReservationAdjustRequest(
                            0,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    12,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        ),
                        new RoomNightOfReservationAdjustRequest(
                            1,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        )
                    ]
                )
            ],
            null,
            [
                new RoomRepresentativeOfReservationAdjustRequest(
                    0,
                    "Full name 1",
                    "Kana 1"
                ),
                new RoomRepresentativeOfReservationAdjustRequest(
                    1,
                    "Full name 2",
                    "Kana 2"
                )
            ],
            null,
            null
        )
        {
            Id = 0,
            CheckInDateId = AppDate.GetId(checkInDate)
        };

        var bookingCreateCommand = new BookingChangeExecutionCommand { Payload = bookingAdjustReq };

        // Act
        var validation = await new BookingAdjustCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenIdIsNegative()
    {
        // Arrange
        var checkInDate = DateTime.Now;
        var bookingAdjustReq = new BookingAdjustRequest(
            true,
            "14:00",
            1,
            2,
            "Free input",
            new ReserverOfReservationAdjustRequest(
                "Reserver full name",
                "Reserver kana",
                Genders.None,
                "test@liberty.com",
                "014574",
                "Country",
                "Reserver address 1",
                "Reserver address 2",
                "Reserver address 3",
                "0214155455"
            ),
            new GuestOfReservationAdjustRequest(
                "Main user full name",
                "Main user kana",
                Genders.Male,
                AppDate.GetId(
                    new(
                        1993,
                        1,
                        1,
                        0,
                        0,
                        0,
                        DateTimeKind.Local
                    )
                ),
                "014574",
                "Country",
                "Main user address 1",
                "Main user address 2",
                "Main user address 3",
                "0214155455"
            ),
            [
                new NightPeopleOfReservationAdjustRequest(
                    AppDate.GetId(checkInDate),
                    [
                        new RoomNightOfReservationAdjustRequest(
                            0,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    12,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        ),
                        new RoomNightOfReservationAdjustRequest(
                            1,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        )
                    ]
                )
            ],
            null,
            [
                new RoomRepresentativeOfReservationAdjustRequest(
                    0,
                    "Full name 1",
                    "Kana 1"
                ),
                new RoomRepresentativeOfReservationAdjustRequest(
                    1,
                    "Full name 2",
                    "Kana 2"
                )
            ],
            null,
            null
        )
        {
            Id = -1,
            CheckInDateId = AppDate.GetId(checkInDate)
        };

        var bookingCreateCommand = new BookingChangeExecutionCommand { Payload = bookingAdjustReq };

        // Act
        var validation = await new BookingAdjustCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenCheckInTimeIsEmpty()
    {
        // Arrange
        var checkInDate = DateTime.Now;
        var bookingAdjustReq = new BookingAdjustRequest(
            true,
            string.Empty,
            1,
            2,
            "Free input",
            new ReserverOfReservationAdjustRequest(
                "Reserver full name",
                "Reserver kana",
                Genders.None,
                "test@liberty.com",
                "014574",
                "Country",
                "Reserver address 1",
                "Reserver address 2",
                "Reserver address 3",
                "0214155455"
            ),
            new GuestOfReservationAdjustRequest(
                "Main user full name",
                "Main user kana",
                Genders.Male,
                AppDate.GetId(
                    new(
                        1993,
                        1,
                        1,
                        0,
                        0,
                        0,
                        DateTimeKind.Local
                    )
                ),
                "014574",
                "Country",
                "Main user address 1",
                "Main user address 2",
                "Main user address 3",
                "0214155455"
            ),
            [
                new NightPeopleOfReservationAdjustRequest(
                    AppDate.GetId(checkInDate),
                    [
                        new RoomNightOfReservationAdjustRequest(
                            0,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    12,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        ),
                        new RoomNightOfReservationAdjustRequest(
                            1,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        )
                    ]
                )
            ],
            null,
            [
                new RoomRepresentativeOfReservationAdjustRequest(
                    0,
                    "Full name 1",
                    "Kana 1"
                ),
                new RoomRepresentativeOfReservationAdjustRequest(
                    1,
                    "Full name 2",
                    "Kana 2"
                )
            ],
            null,
            null
        )
        {
            Id = 1,
            CheckInDateId = AppDate.GetId(checkInDate)
        };

        var bookingCreateCommand = new BookingChangeExecutionCommand { Payload = bookingAdjustReq };

        // Act
        var validation = await new BookingAdjustCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("CheckInTime", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenIsAgreeIsFalse()
    {
        // Arrange
        var checkInDate = DateTime.Now;
        var bookingAdjustReq = new BookingAdjustRequest(
            false,
            "14:00",
            1,
            2,
            "Free input",
            new ReserverOfReservationAdjustRequest(
                "Reserver full name",
                "Reserver kana",
                Genders.None,
                "test@liberty.com",
                "014574",
                "Country",
                "Reserver address 1",
                "Reserver address 2",
                "Reserver address 3",
                "0214155455"
            ),
            new GuestOfReservationAdjustRequest(
                "Main user full name",
                "Main user kana",
                Genders.Male,
                AppDate.GetId(
                    new(
                        1993,
                        1,
                        1,
                        0,
                        0,
                        0,
                        DateTimeKind.Local
                    )
                ),
                "014574",
                "Country",
                "Main user address 1",
                "Main user address 2",
                "Main user address 3",
                "0214155455"
            ),
            [
                new NightPeopleOfReservationAdjustRequest(
                    AppDate.GetId(checkInDate),
                    [
                        new RoomNightOfReservationAdjustRequest(
                            0,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    12,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        ),
                        new RoomNightOfReservationAdjustRequest(
                            1,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        )
                    ]
                )
            ],
            null,
            [
                new RoomRepresentativeOfReservationAdjustRequest(
                    0,
                    "Full name 1",
                    "Kana 1"
                ),
                new RoomRepresentativeOfReservationAdjustRequest(
                    1,
                    "Full name 2",
                    "Kana 2"
                )
            ],
            null,
            null
        )
        {
            Id = 1,
            CheckInDateId = AppDate.GetId(checkInDate)
        };

        var bookingCreateCommand = new BookingChangeExecutionCommand { Payload = bookingAdjustReq };

        // Act
        var validation = await new BookingAdjustCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("IsAgree", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenNumberOfNightsIsInvalid()
    {
        // Arrange
        var checkInDate = DateTime.Now;
        var bookingAdjustReq = new BookingAdjustRequest(
            true,
            "14:00",
            0,
            2,
            "Free input",
            new ReserverOfReservationAdjustRequest(
                "Reserver full name",
                "Reserver kana",
                Genders.None,
                "test@liberty.com",
                "014574",
                "Country",
                "Reserver address 1",
                "Reserver address 2",
                "Reserver address 3",
                "0214155455"
            ),
            new GuestOfReservationAdjustRequest(
                "Main user full name",
                "Main user kana",
                Genders.Male,
                AppDate.GetId(
                    new(
                        1993,
                        1,
                        1,
                        0,
                        0,
                        0,
                        DateTimeKind.Local
                    )
                ),
                "014574",
                "Country",
                "Main user address 1",
                "Main user address 2",
                "Main user address 3",
                "0214155455"
            ),
            [
                new NightPeopleOfReservationAdjustRequest(
                    AppDate.GetId(checkInDate),
                    [
                        new RoomNightOfReservationAdjustRequest(
                            0,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    12,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        ),
                        new RoomNightOfReservationAdjustRequest(
                            1,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        )
                    ]
                )
            ],
            null,
            [
                new RoomRepresentativeOfReservationAdjustRequest(
                    0,
                    "Full name 1",
                    "Kana 1"
                ),
                new RoomRepresentativeOfReservationAdjustRequest(
                    1,
                    "Full name 2",
                    "Kana 2"
                )
            ],
            null,
            null
        )
        {
            Id = 1,
            CheckInDateId = AppDate.GetId(checkInDate)
        };

        var bookingCreateCommand = new BookingChangeExecutionCommand { Payload = bookingAdjustReq };

        // Act
        var validation = await new BookingAdjustCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("NumberOfNights", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenNumberOfRoomsIsInvalid()
    {
        // Arrange
        var checkInDate = DateTime.Now;
        var bookingAdjustReq = new BookingAdjustRequest(
            true,
            "14:00",
            1,
            0,
            "Free input",
            new ReserverOfReservationAdjustRequest(
                "Reserver full name",
                "Reserver kana",
                Genders.None,
                "test@liberty.com",
                "014574",
                "Country",
                "Reserver address 1",
                "Reserver address 2",
                "Reserver address 3",
                "0214155455"
            ),
            new GuestOfReservationAdjustRequest(
                "Main user full name",
                "Main user kana",
                Genders.Male,
                AppDate.GetId(
                    new(
                        1993,
                        1,
                        1,
                        0,
                        0,
                        0,
                        DateTimeKind.Local
                    )
                ),
                "014574",
                "Country",
                "Main user address 1",
                "Main user address 2",
                "Main user address 3",
                "0214155455"
            ),
            [
                new NightPeopleOfReservationAdjustRequest(
                    AppDate.GetId(checkInDate),
                    [
                        new RoomNightOfReservationAdjustRequest(
                            0,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    12,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        ),
                        new RoomNightOfReservationAdjustRequest(
                            1,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        )
                    ]
                )
            ],
            null,
            [
                new RoomRepresentativeOfReservationAdjustRequest(
                    0,
                    "Full name 1",
                    "Kana 1"
                ),
                new RoomRepresentativeOfReservationAdjustRequest(
                    1,
                    "Full name 2",
                    "Kana 2"
                )
            ],
            null,
            null
        )
        {
            Id = 1,
            CheckInDateId = AppDate.GetId(checkInDate)
        };

        var bookingCreateCommand = new BookingChangeExecutionCommand { Payload = bookingAdjustReq };

        // Act
        var validation = await new BookingAdjustCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("NumberOfRooms", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenFreeInputOverMaxLength()
    {
        // Arrange
        var checkInDate = DateTime.Now;
        var bookingAdjustReq = new BookingAdjustRequest(
            true,
            "14:00",
            1,
            2,
            new string('a', 1001),
            new ReserverOfReservationAdjustRequest(
                "Reserver full name",
                "Reserver kana",
                Genders.None,
                "test@liberty.com",
                "014574",
                "Country",
                "Reserver address 1",
                "Reserver address 2",
                "Reserver address 3",
                "0214155455"
            ),
            new GuestOfReservationAdjustRequest(
                "Main user full name",
                "Main user kana",
                Genders.Male,
                AppDate.GetId(
                    new(
                        1993,
                        1,
                        1,
                        0,
                        0,
                        0,
                        DateTimeKind.Local
                    )
                ),
                "014574",
                "Country",
                "Main user address 1",
                "Main user address 2",
                "Main user address 3",
                "0214155455"
            ),
            [
                new NightPeopleOfReservationAdjustRequest(
                    AppDate.GetId(checkInDate),
                    [
                        new RoomNightOfReservationAdjustRequest(
                            0,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    12,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        ),
                        new RoomNightOfReservationAdjustRequest(
                            1,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        )
                    ]
                )
            ],
            null,
            [
                new RoomRepresentativeOfReservationAdjustRequest(
                    0,
                    "Full name 1",
                    "Kana 1"
                ),
                new RoomRepresentativeOfReservationAdjustRequest(
                    1,
                    "Full name 2",
                    "Kana 2"
                )
            ],
            null,
            null
        )
        {
            Id = 1,
            CheckInDateId = AppDate.GetId(checkInDate)
        };

        var bookingCreateCommand = new BookingChangeExecutionCommand { Payload = bookingAdjustReq };

        // Act
        var validation = await new BookingAdjustCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("FreeInput", validation.GetErrorField());
    }

    [Fact]
    public async Task ReserverOfReservationAdjustRequestValidator_ShouldThrowError_WhenFullNameIsEmpty()
    {
        // Arrange
        var reserverRequest = new ReserverOfReservationAdjustRequest(
            string.Empty,
            "Reserver kana",
            Genders.None,
            "test@liberty.com",
            "014574",
            "Country",
            "Reserver address 1",
            "Reserver address 2",
            "Reserver address 3",
            "0214155455"
        );

        var validator = new ReserverOfReservationAdjustRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(reserverRequest);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("FullName", validation.GetErrorField());
    }

    [Fact]
    public async Task ReserverOfReservationAdjustRequestValidator_ShouldThrowError_WhenFullNameOverMaxLength()
    {
        // Arrange
        var reserverRequest = new ReserverOfReservationAdjustRequest(
            new string('a', 256),
            "Reserver kana",
            Genders.None,
            "test@liberty.com",
            "014574",
            "Country",
            "Reserver address 1",
            "Reserver address 2",
            "Reserver address 3",
            "0214155455"
        );

        var validator = new ReserverOfReservationAdjustRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(reserverRequest);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("FullName", validation.GetErrorField());
    }

    [Fact]
    public async Task ReserverOfReservationAdjustRequestValidator_ShouldThrowError_WhenKanaOverMaxLength()
    {
        // Arrange
        var reserverRequest = new ReserverOfReservationAdjustRequest(
            "Reserver full name",
            new string('a', 256),
            Genders.None,
            "test@liberty.com",
            "014574",
            "Country",
            "Reserver address 1",
            "Reserver address 2",
            "Reserver address 3",
            "0214155455"
        );

        var validator = new ReserverOfReservationAdjustRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(reserverRequest);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Kana", validation.GetErrorField());
    }

    [Fact]
    public async Task ReserverOfReservationAdjustRequestValidator_ShouldThrowError_WhenEmailIsEmpty()
    {
        // Arrange
        var reserverRequest = new ReserverOfReservationAdjustRequest(
            "Reserver full name",
            "Reserver kana",
            Genders.None,
            string.Empty,
            "014574",
            "Country",
            "Reserver address 1",
            "Reserver address 2",
            "Reserver address 3",
            "0214155455"
        );

        var validator = new ReserverOfReservationAdjustRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(reserverRequest);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Email", validation.GetErrorField());
    }

    [Fact]
    public async Task ReserverOfReservationAdjustRequestValidator_ShouldThrowError_WhenEmailIsInvalid()
    {
        // Arrange
        var reserverRequest = new ReserverOfReservationAdjustRequest(
            "Reserver full name",
            "Reserver kana",
            Genders.None,
            "Invalid Email",
            "014574",
            "Country",
            "Reserver address 1",
            "Reserver address 2",
            "Reserver address 3",
            "0214155455"
        );

        var validator = new ReserverOfReservationAdjustRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(reserverRequest);

        // Assert
        Assert.Equal(ErrorCode.E0006, validation.Errors.GetErrorCode());
        Assert.Contains("Email", validation.GetErrorField());
    }

    [Fact]
    public async Task ReserverOfReservationAdjustRequestValidator_ShouldThrowError_WhenPostCodeIsEmpty()
    {
        // Arrange
        var reserverRequest = new ReserverOfReservationAdjustRequest(
            "Reserver full name",
            "Reserver kana",
            Genders.None,
            "test@liberty.com",
            string.Empty,
            "Country",
            "Reserver address 1",
            "Reserver address 2",
            "Reserver address 3",
            "0214155455"
        );

        var validator = new ReserverOfReservationAdjustRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(reserverRequest);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("PostCode", validation.GetErrorField());
    }

    [Fact]
    public async Task ReserverOfReservationAdjustRequestValidator_ShouldThrowError_WhenAddress1IsEmpty()
    {
        // Arrange
        var reserverRequest = new ReserverOfReservationAdjustRequest(
            "Reserver full name",
            "Reserver kana",
            Genders.None,
            "test@liberty.com",
            "014574",
            "Country",
            string.Empty,
            "Reserver address 2",
            "Reserver address 3",
            "0214155455"
        );

        var validator = new ReserverOfReservationAdjustRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(reserverRequest);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Address1", validation.GetErrorField());
    }

    [Fact]
    public async Task ReserverOfReservationAdjustRequestValidator_ShouldThrowError_WhenAddress2IsEmpty()
    {
        // Arrange
        var reserverRequest = new ReserverOfReservationAdjustRequest(
            "Reserver full name",
            "Reserver kana",
            Genders.None,
            "test@liberty.com",
            "014574",
            "Country",
            "Reserver address 1",
            string.Empty,
            "Reserver address 3",
            "0214155455"
        );

        var validator = new ReserverOfReservationAdjustRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(reserverRequest);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Address2", validation.GetErrorField());
    }

    [Fact]
    public async Task ReserverOfReservationAdjustRequestValidator_ShouldThrowError_WhenAddress3IsEmpty()
    {
        // Arrange
        var reserverRequest = new ReserverOfReservationAdjustRequest(
            "Reserver full name",
            "Reserver kana",
            Genders.None,
            "test@liberty.com",
            "014574",
            "Country",
            "Reserver address 1",
            string.Empty,
            string.Empty,
            "0214155455"
        );

        var validator = new ReserverOfReservationAdjustRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(reserverRequest);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Address2", validation.GetErrorField());
    }

    [Fact]
    public async Task ReserverOfReservationAdjustRequestValidator_ShouldThrowError_WhenPhoneNumberIsInvalid()
    {
        // Arrange
        var reserverRequest = new ReserverOfReservationAdjustRequest(
            "Reserver full name",
            "Reserver kana",
            Genders.None,
            "test@liberty.com",
            "014574",
            "Country",
            "Reserver address 1",
            "Reserver address 2",
            "Reserver address 2",
            "1234567891234567899999"
        );

        var validator = new ReserverOfReservationAdjustRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(reserverRequest);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("PhoneNumber", validation.GetErrorField());
    }

    [Fact]
    public async Task GuestOfReservationAdjustRequestValidator_ShouldThrowError_WhenFullNameExceedsMaxLength()
    {
        // Arrange
        var guestRequest = new GuestOfReservationAdjustRequest(
            new string('a', 256), // Exceed maximum length of 255
            "Kana",
            Genders.Male,
            20250101,
            "PostCode",
            "Country",
            "Address1",
            "Address2",
            "Address3",
            "0214155455"
        );

        var validation = await new GuestOfReservationAdjustRequestValidator().ValidateAsync(guestRequest);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("FullName", validation.GetErrorField());
    }

    [Fact]
    public async Task GuestOfReservationAdjustRequestValidator_ShouldThrowError_WhenKanaExceedsMaxLength()
    {
        // Arrange
        var guestRequest = new GuestOfReservationAdjustRequest(
            "Full Name",
            new string('a', 256), // Exceed maximum length of 255
            Genders.Male,
            20250101,
            "PostCode",
            "Country",
            "Address1",
            "Address2",
            "Address3",
            "0214155455"
        );

        var validation = await new GuestOfReservationAdjustRequestValidator().ValidateAsync(guestRequest);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Kana", validation.GetErrorField());
    }

    [Fact]
    public async Task GuestOfReservationAdjustRequestValidator_ShouldThrowError_WhenBirthdayIsInvalid()
    {
        // Arrange
        var guestRequest = new GuestOfReservationAdjustRequest(
            "Full Name",
            "Kana",
            Genders.Male,
            null,
            "PostCode",
            "Country",
            "Address1",
            "Address2",
            "Address3",
            "0214155455"
        );

        var validation = await new GuestOfReservationAdjustRequestValidator().ValidateAsync(guestRequest);

        // Assert
        Assert.Equal(ErrorCode.E0100, validation.Errors.GetErrorCode());
    }

    [Fact]
    public async Task GuestOfReservationAdjustRequestValidator_ShouldThrowError_WhenPostCodeExceedsMaxLength()
    {
        // Arrange
        var guestRequest = new GuestOfReservationAdjustRequest(
            "Full Name",
            "Kana",
            Genders.Male,
            20250101,
            new string('a', 256),
            "Country",
            "Address1",
            "Address2",
            "Address3",
            "0214155455"
        );

        var validation = await new GuestOfReservationAdjustRequestValidator().ValidateAsync(guestRequest);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("PostCode", validation.GetErrorField());
    }

    [Fact]
    public async Task GuestOfReservationAdjustRequestValidator_ShouldThrowError_WhenAddressExceedsMaxLength()
    {
        // Arrange
        var guestRequest = new GuestOfReservationAdjustRequest(
            "Full Name",
            "Kana",
            Genders.Male,
            20250101,
            "PostCode",
            "Country",
            new string('a', 256),
            "address 2",
            "address 3",
            "0214155455"
        );

        var validation = await new GuestOfReservationAdjustRequestValidator().ValidateAsync(guestRequest);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Address1", validation.GetErrorField());
    }

    [Fact]
    public async Task GuestOfReservationAdjustRequestValidator_ShouldThrowError_WhenPhoneNumberExceedsMaxLength()
    {
        // Arrange
        var guestRequest = new GuestOfReservationAdjustRequest(
            "Full Name",
            "Kana",
            Genders.Male,
            20250101,
            "PostCode",
            "Country",
            "Address1",
            "Address2",
            "Address3",
            "123456789123456789999"
        );

        var validation = await new GuestOfReservationAdjustRequestValidator().ValidateAsync(guestRequest);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("PhoneNumber", validation.GetErrorField());
    }

    [Fact]
    public async Task RoomPeopleOfReservationAdjustRequestValidator_ShouldPass_WhenValidData()
    {
        // Arrange
        var checkInDate = DateTime.Now;
        var request = new NightPeopleOfReservationAdjustRequest(
            AppDate.GetId(checkInDate),
            [
                new RoomNightOfReservationAdjustRequest(
                    0,
                    [
                        new PeopleOfReservationAdjustRequest(1, 12, Genders.Male),
                        new PeopleOfReservationAdjustRequest(1, 1, Genders.Female)
                    ]
                ),
                new RoomNightOfReservationAdjustRequest(
                    1,
                    [
                        new PeopleOfReservationAdjustRequest(1, 12, Genders.Male),
                        new PeopleOfReservationAdjustRequest(1, 1, Genders.Female)
                    ]
                )
            ]
        );

        // Act
        var validation = await new RoomPeopleOfReservationAdjustRequestValidator().ValidateAsync(request);

        // Assert
        Assert.True(validation.IsValid);
    }

    [Fact]
    public async Task RoomPeopleOfReservationAdjustRequestValidator_ShouldThrowError_WhenAppDateIdIsInvalid()
    {
        // Arrange
        var request = new NightPeopleOfReservationAdjustRequest(
            0,
            [
                new RoomNightOfReservationAdjustRequest(0, [new PeopleOfReservationAdjustRequest(1, 1, Genders.Male)])
            ]
        );

        // Act
        var validation = await new RoomPeopleOfReservationAdjustRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("AppDateId", validation.GetErrorField());
    }

    [Fact]
    public async Task RoomPeopleOfReservationAdjustRequestValidator_ShouldThrowError_WhenPeopleInRoomIsInvalid()
    {
        // Arrange
        var request = new NightPeopleOfReservationAdjustRequest(
            AppDate.GetId(DateTime.Now),
            [
                new RoomNightOfReservationAdjustRequest(
                    0,
                    [
                        new(
                            -1,
                            1,
                            Genders.Male
                        ),
                        new(
                            1,
                            1,
                            Genders.Female
                        )
                    ]
                )
            ]
        );

        // Act
        var validation = await new RoomPeopleOfReservationAdjustRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
    }

    [Fact]
    public async Task RoomRepresentativeOfReservationAdjustRequestValidator_ShouldThrowError_WhenRoomIndexIsNegative()
    {
        // Arrange
        var request = new RoomRepresentativeOfReservationAdjustRequest(
            -1, // Invalid RoomIndex
            "Full Name",
            "Kana"
        );

        var validation = await new RoomRepresentativeOfReservationAdjustRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomIndex", validation.GetErrorField());
    }

    [Fact]
    public async Task RoomRepresentativeOfReservationAdjustRequestValidator_ShouldThrowError_WhenFullNameIsEmpty()
    {
        // Arrange
        var request = new RoomRepresentativeOfReservationAdjustRequest(
            1,
            string.Empty, // FullName is empty
            "Kana"
        );

        var validation = await new RoomRepresentativeOfReservationAdjustRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("FullName", validation.GetErrorField());
    }

    [Fact]
    public async Task RoomRepresentativeOfReservationAdjustRequestValidator_ShouldThrowError_WhenFullNameExceedsMaxLength()
    {
        // Arrange
        var request = new RoomRepresentativeOfReservationAdjustRequest(
            1,
            new string('a', 256), // Exceeds max length of 255 for FullName
            "Kana"
        );

        var validation = await new RoomRepresentativeOfReservationAdjustRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("FullName", validation.GetErrorField());
    }

    [Fact]
    public async Task RoomRepresentativeOfReservationAdjustRequestValidator_ShouldThrowError_WhenKanaExceedsMaxLength()
    {
        // Arrange
        var request = new RoomRepresentativeOfReservationAdjustRequest(
            1,
            "Full Name",
            new string('a', 256) // Exceeds max length of 255 for Kana
        );

        var validation = await new RoomRepresentativeOfReservationAdjustRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Kana", validation.GetErrorField());
    }

    [Fact]
    public async Task RoomRepresentativeOfReservationAdjustRequestValidator_ShouldPass_WhenValidRequest()
    {
        // Arrange
        var request = new RoomRepresentativeOfReservationAdjustRequest(
            1, // Valid RoomIndex
            "Valid Full Name",
            "Valid Kana"
        );

        var validation = await new RoomRepresentativeOfReservationAdjustRequestValidator().ValidateAsync(request);

        // Assert
        Assert.True(validation.IsValid);
    }

    [Fact]
    public async Task OptionOfReservationAdjustRequestValidator_ShouldThrowError_WhenOptionItemIdIsNegative()
    {
        // Arrange
        var request = new OptionOfReservationAdjustRequest(
            -1,
            1
        );

        // Act
        var validation = await new OptionOfReservationAdjustRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("OptionItemId", validation.GetErrorField());
    }

    [Fact]
    public async Task OptionOfReservationAdjustRequestValidator_ShouldThrowError_WhenNumberIsNull()
    {
        // Arrange
        var request = new OptionOfReservationAdjustRequest(
            1,
            0
        );

        // Act
        var validation = await new OptionOfReservationAdjustRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Number", validation.GetErrorField());
    }

    [Fact]
    public async Task OptionOfReservationAdjustRequestValidator_ShouldThrowError_WhenNumberIsLessThanOne()
    {
        // Arrange
        var request = new OptionOfReservationAdjustRequest(
            1,
            -5
        );

        // Act
        var validation = await new OptionOfReservationAdjustRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Number", validation.GetErrorField());
    }

    [Fact]
    public async Task RoomOptionOfReservationAdjustRequestValidator_ShouldPass_WhenValidData()
    {
        // Arrange
        var request = new RoomOptionOfReservationAdjustRequest(
            1,
            [
                new OptionOfReservationAdjustRequest(1, 2),
                new OptionOfReservationAdjustRequest(2, 3)
            ]
        );

        // Act
        var validation = await new RoomOptionOfReservationAdjustRequestValidator().ValidateAsync(request);

        // Assert
        Assert.True(validation.IsValid);
    }

    [Fact]
    public async Task RoomOptionOfReservationAdjustRequestValidator_ShouldThrowError_WhenRoomIndexIsNegative()
    {
        // Arrange
        var request = new RoomOptionOfReservationAdjustRequest(
            -1,
            [
                new OptionOfReservationAdjustRequest(1, 2)
            ]
        );

        // Act
        var validation = await new RoomOptionOfReservationAdjustRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomIndex", validation.GetErrorField());
    }

    [Fact]
    public async Task RoomOptionOfReservationAdjustRequestValidator_ShouldThrowError_WhenOptionItemsAreInvalid()
    {
        // Arrange
        var request = new RoomOptionOfReservationAdjustRequest(
            1,
            [
                new OptionOfReservationAdjustRequest(1, -1),
                new OptionOfReservationAdjustRequest(-1, 2)
            ]
        );

        // Act
        var validation = await new RoomOptionOfReservationAdjustRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("OptionItem", validation.GetErrorField());
    }

    [Fact]
    public async Task PeopleOfReservationAdjustRequestValidator_ShouldThrowError_WhenPersonAgeTypeIdIsInvalid()
    {
        // Arrange
        var request = new PeopleOfReservationAdjustRequest(0, 5, Genders.Male);

        // Act
        var validation = await new PeopleOfReservationAdjustRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("PersonAgeTypeId", validation.GetErrorField());
    }

    [Fact]
    public async Task PeopleOfReservationAdjustRequestValidator_ShouldThrowError_WhenNumberOfPeoplesIsInvalid()
    {
        // Arrange
        var request = new PeopleOfReservationAdjustRequest(1, 0, Genders.Female);

        // Act
        var validation = await new PeopleOfReservationAdjustRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("NumberOfPeoples", validation.GetErrorField());
    }

    [Fact]
    public async Task NightOptionOfReservationAdjustRequestValidator_ShouldThrowError_WhenAppDateIdIsInvalid()
    {
        // Arrange
        var request = new NightOptionOfReservationAdjustRequest(
            0,
            new List<RoomOptionOfReservationAdjustRequest>()
        );

        // Act
        var validation = await new NightOptionOfReservationAdjustRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("AppDateId", validation.GetErrorField());
    }

    [Fact]
    public async Task NightOptionOfReservationAdjustRequestValidator_ShouldThrowError_WhenAppDateIdIsNotValidDate()
    {
        // Arrange
        var invalidAppDate = 123456;
        var request = new NightOptionOfReservationAdjustRequest(
            invalidAppDate,
            []
        );

        // Act
        var validation = await new NightOptionOfReservationAdjustRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0008, validation.Errors.GetErrorCode());
        Assert.Contains("AppDateId", validation.GetErrorField());
    }

    [Fact]
    public async Task RoomOfReservationAdjustRequestValidator_ShouldThrowError_WhenRoomIndexIsInvalid()
    {
        // Arrange
        var request = new RoomNightOfReservationAdjustRequest(
            -1,
            []
        );

        var validator = new RoomOfReservationAdjustRequestValidator();

        // Act
        var validation = await validator.ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomIndex", validation.GetErrorField());
    }

    [Fact]
    public async Task NightOptionOfReservationAdjustRequestValidator_ShouldThrowError_WhenRoomsContainInvalidRoomIndex()
    {
        // Arrange
        var request = new NightOptionOfReservationAdjustRequest(
            AppDate.GetId(DateTime.Now),
            new List<RoomOptionOfReservationAdjustRequest> { new(-1, new List<OptionOfReservationAdjustRequest> { new(1, 2) }) }
        );

        // Act
        var validation = await new NightOptionOfReservationAdjustRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomIndex", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingConfirmCommandValidator_ShouldThrowError_WhenIdIsInvalid()
    {
        // Arrange
        var request = new BookingConfirmRequest { Id = -1 };

        // Act
        var validation = await new BookingConfirmCommandValidator().ValidateAsync(
            new BookingConfirmCommand { Payload = request }
        );

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingConfirmCommandValidator_ShouldThrowError_WhenIdIsNull()
    {
        // Arrange
        var request = new BookingConfirmRequest();

        // Act
        var validation = await new BookingConfirmCommandValidator().ValidateAsync(
            new BookingConfirmCommand { Payload = request }
        );

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task ChangePersonsBookingCommandValidator_ShouldThrowError_WhenGuestsPerRoomIsEmpty()
    {
        // Arrange
        var checkInDate = AppDate.GetId(DateTime.UtcNow);
        List<PersonOfBookingPriceRequest> guestsPerRoom = [];
        var bookingPrinceRequest = new BookingPriceRequest
        {
            RestNumber = 1,
            RoomNumber = 1,
            CheckInDate = checkInDate,
            GuestsPerRoom = guestsPerRoom,
            OptionItems = null
        };

        // Act
        var validation = await new ChangePersonsBookingCommandValidator().ValidateAsync(
            new ChangePersonsBookingCommand(1) { Payload = bookingPrinceRequest }
        );

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("GuestsPerRoom", validation.GetErrorField());
    }

    [Fact]
    public async Task ChangePersonsBookingCommandValidator_ShouldThrowError_WhenRestNumberIsInvalid()
    {
        // Arrange
        var checkInDate = AppDate.GetId(DateTime.UtcNow);
        List<PersonOfBookingPriceRequest> guestsPerRoom =
        [
            new()
            {
                AppDateId = checkInDate,
                RestIndex = 0,
                RoomGroupIndex = 0,
                PersonAgeTypeId = 1,
                Persons = 1,
                MalePersons = 1,
                FemalePersons = 1
            }
        ];
        var bookingPrinceRequest = new BookingPriceRequest
        {
            RestNumber = 0,
            RoomNumber = 1,
            CheckInDate = checkInDate,
            GuestsPerRoom = guestsPerRoom,
            OptionItems = null
        };

        // Act
        var validation = await new ChangePersonsBookingCommandValidator().ValidateAsync(
            new ChangePersonsBookingCommand(1) { Payload = bookingPrinceRequest }
        );

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RestNumber", validation.GetErrorField());
    }

    [Fact]
    public async Task ChangePersonsBookingCommandValidator_ShouldThrowError_WhenRoomNumberIsInvalid()
    {
        // Arrange
        var checkInDate = AppDate.GetId(DateTime.UtcNow);
        List<PersonOfBookingPriceRequest> guestsPerRoom =
        [
            new()
            {
                AppDateId = checkInDate,
                RestIndex = 0,
                RoomGroupIndex = 0,
                PersonAgeTypeId = 1,
                Persons = 1,
                MalePersons = 1,
                FemalePersons = 1
            }
        ];
        var bookingPrinceRequest = new BookingPriceRequest
        {
            RestNumber = 1,
            RoomNumber = 0,
            CheckInDate = checkInDate,
            GuestsPerRoom = guestsPerRoom,
            OptionItems = null
        };

        // Act
        var validation = await new ChangePersonsBookingCommandValidator().ValidateAsync(
            new ChangePersonsBookingCommand(1) { Payload = bookingPrinceRequest }
        );

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomNumber", validation.GetErrorField());
    }

    [Fact]
    public async Task ChangePersonsBookingCommandValidator_ShouldThrowError_WhenCheckInDateIsInvalid()
    {
        // Arrange
        var checkInDate = AppDate.GetId(DateTime.UtcNow);
        List<PersonOfBookingPriceRequest> guestsPerRoom =
        [
            new()
            {
                AppDateId = checkInDate,
                RestIndex = 0,
                RoomGroupIndex = 0,
                PersonAgeTypeId = 1,
                Persons = 1,
                MalePersons = 1,
                FemalePersons = 1
            }
        ];
        var bookingPrinceRequest = new BookingPriceRequest
        {
            RestNumber = 1,
            RoomNumber = 1,
            CheckInDate = 1000,
            GuestsPerRoom = guestsPerRoom,
            OptionItems = null
        };

        // Act
        var validation = await new ChangePersonsBookingCommandValidator().ValidateAsync(
            new ChangePersonsBookingCommand(1) { Payload = bookingPrinceRequest }
        );

        // Assert
        Assert.Equal(ErrorCode.E0008, validation.Errors.GetErrorCode());
        Assert.Contains("CheckInDate", validation.GetErrorField());
    }

    [Fact]
    public async Task ReservationCheckNumberOfNightsQueryValidator_ShouldThrowError_WhenIdIsInvalid()
    {
        // Arrange + Act
        var validation = await new ReservationCheckNumberOfNightsQueryValidator().ValidateAsync(
            new ReservationCheckNumberOfNightsQuery(-1)
        );

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task ReservationCheckNumberOfRoomsQueryValidator_ShouldThrowError_WhenIdIsInvalid()
    {
        // Arrange + Act
        var validation = await new ReservationCheckNumberOfRoomsQueryValidator().ValidateAsync(
            new ReservationCheckNumberOfRoomsQuery(-1)
        );

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task ReservationGetAllOptionItemsQueryValidator_ShouldThrowError_WhenIdIsInvalid()
    {
        // Arrange
        var mockPageable = PageableBinderConfig.DefaultPageable;

        // Act
        var validation = await new ReservationGetAllOptionItemsQueryValidator().ValidateAsync(
            new ReservationGetAllOptionItemsQuery(-1, 1, 1, mockPageable)
        );

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task ReservationGetDetailsQueryValidator_ShouldThrowError_WhenIdIsInvalid()
    {
        // Act
        var validation = await new ReservationGetDetailsQueryValidator().ValidateAsync(
            new ReservationGetDetailsQuery(-1)
        );

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.GetErrorField());
    }
}
