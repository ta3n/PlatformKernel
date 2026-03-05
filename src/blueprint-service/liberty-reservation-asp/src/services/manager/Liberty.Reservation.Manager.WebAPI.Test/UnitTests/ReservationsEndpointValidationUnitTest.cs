using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BookingReservation;
using Liberty.Reservation.Manager.WebAPI.Application.Validations;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.SysException;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class ReservationsEndpointValidationUnitTest : BaseUnitTest
{
    [Fact]
    public async Task BookingChangeExecutionCommandValidator_ShouldThrowError_WhenIdIsNull()
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
                Genders.Male,
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
        ) { CheckInDateId = AppDate.GetId(checkInDate) };

        var bookingCreateCommand = new BookingChangeExecutionCommand { Payload = bookingAdjustReq };

        // Act
        var validation = await new BookingChangeExecutionCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingChangeExecutionCommandValidator_ShouldThrowError_WhenIdIsNegative()
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
                Genders.Male,
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
            CheckInDateId = AppDate.GetId(checkInDate),
            Id = -2
        };

        var bookingCreateCommand = new BookingChangeExecutionCommand { Payload = bookingAdjustReq };

        // Act
        var validation = await new BookingChangeExecutionCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingChangeExecutionCommandValidator_ShouldThrowError_WhenCheckInTimeIsInvalid()
    {
        // Arrange
        var checkInDate = DateTime.Now;
        var bookingAdjustReq = new BookingAdjustRequest(
            true,
            "a0",
            1,
            2,
            "Free input",
            new ReserverOfReservationAdjustRequest(
                "Reserver full name",
                "Reserver kana",
                Genders.Male,
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
            CheckInDateId = AppDate.GetId(checkInDate),
            Id = 2
        };

        var bookingChangeExecutionCommand = new BookingChangeExecutionCommand { Payload = bookingAdjustReq };

        // Act
        var validation = await new BookingChangeExecutionCommandValidator().ValidateAsync(bookingChangeExecutionCommand);

        // Assert
        Assert.Equal(ErrorCode.E0012, validation.Errors.GetErrorCode());
        Assert.Contains("CheckInTime", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingChangeExecutionCommandValidator_ShouldThrowError_WhenIsAgreeIsFalse()
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
                Genders.Male,
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
            CheckInDateId = AppDate.GetId(checkInDate),
            Id = 2
        };

        var bookingChangeExecutionCommand = new BookingChangeExecutionCommand { Payload = bookingAdjustReq };

        // Act
        var validation = await new BookingChangeExecutionCommandValidator().ValidateAsync(bookingChangeExecutionCommand);

        // Assert
        Assert.Equal(ErrorCode.E1028, validation.Errors.GetErrorCode());
        Assert.Contains("IsAgree", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingChangeExecutionCommandValidator_ShouldThrowError_WhenNumberOfNightsIsEmpty()
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
                Genders.Male,
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
            CheckInDateId = AppDate.GetId(checkInDate),
            Id = 2
        };

        var bookingChangeExecutionCommand = new BookingChangeExecutionCommand { Payload = bookingAdjustReq };

        // Act
        var validation = await new BookingChangeExecutionCommandValidator().ValidateAsync(bookingChangeExecutionCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("NumberOfNights", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingChangeExecutionCommandValidator_ShouldThrowError_WhenNumberOfNightsIsLessThanOne()
    {
        // Arrange
        var checkInDate = DateTime.Now;
        var bookingAdjustReq = new BookingAdjustRequest(
            true,
            "14:00",
            -1,
            2,
            "Free input",
            new ReserverOfReservationAdjustRequest(
                "Reserver full name",
                "Reserver kana",
                Genders.Male,
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
            CheckInDateId = AppDate.GetId(checkInDate),
            Id = 2
        };

        var bookingChangeExecutionCommand = new BookingChangeExecutionCommand { Payload = bookingAdjustReq };

        // Act
        var validation = await new BookingChangeExecutionCommandValidator().ValidateAsync(bookingChangeExecutionCommand);
        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("NumberOfNights", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingChangeExecutionCommandValidator_ShouldThrowError_WhenNumberOfRoomsIsEmpty()
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
                Genders.Male,
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
            CheckInDateId = AppDate.GetId(checkInDate),
            Id = 2
        };

        var bookingChangeExecutionCommand = new BookingChangeExecutionCommand { Payload = bookingAdjustReq };

        // Act
        var validation = await new BookingChangeExecutionCommandValidator().ValidateAsync(bookingChangeExecutionCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("NumberOfRooms", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingChangeExecutionCommandValidator_ShouldThrowError_WheNumberOfRoomsIsLessThanOne()
    {
        // Arrange
        var checkInDate = DateTime.Now;
        var bookingAdjustReq = new BookingAdjustRequest(
            true,
            "14:00",
            1,
            -1,
            "Free input",
            new ReserverOfReservationAdjustRequest(
                "Reserver full name",
                "Reserver kana",
                Genders.Male,
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
            CheckInDateId = AppDate.GetId(checkInDate),
            Id = 2
        };

        var bookingChangeExecutionCommand = new BookingChangeExecutionCommand { Payload = bookingAdjustReq };

        // Act
        var validation = await new BookingChangeExecutionCommandValidator().ValidateAsync(bookingChangeExecutionCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("NumberOfRooms", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingChangeExecutionCommandValidator_ShouldThrowError_WhenFreeInputExceedsMaxLength()
    {
        // Arrange
        var checkInDate = DateTime.Now;
        var bookingAdjustReq = new BookingAdjustRequest(
            true,
            "14:00",
            1,
            2,
            new string('a', 2501),
            new ReserverOfReservationAdjustRequest(
                "Reserver full name",
                "Reserver kana",
                Genders.Male,
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
            CheckInDateId = AppDate.GetId(checkInDate),
            Id = 2
        };

        var bookingChangeExecutionCommand = new BookingChangeExecutionCommand { Payload = bookingAdjustReq };

        // Act
        var validation = await new BookingChangeExecutionCommandValidator().ValidateAsync(bookingChangeExecutionCommand);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("FreeInput", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCancellationCommandValidator_ShouldThrowError_WhenIdIsNull()
    {
        // Arrange
        var bookingCancellationCommand = new BookingCancellationCommand { Payload = new BookingCancellationByManagerRequest(null) };
        var validator = new BookingCancellationCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(bookingCancellationCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("Id", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCancellationCommandValidator_ShouldThrowError_WhenIdIsNotGreaterThanZero()
    {
        // Arrange
        var bookingCancellationCommand = new BookingCancellationCommand { Payload = new BookingCancellationByManagerRequest(null) };
        var validator = new BookingCancellationCommandValidator();

        // Act
        var validation = await validator.ValidateAsync(bookingCancellationCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Equal("Id", validation.GetErrorField());
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
        Assert.Equal(ErrorCode.E0010, validation.Errors.GetErrorCode());
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
        Assert.Equal(ErrorCode.E0010, validation.Errors.GetErrorCode());
        Assert.Contains("RoomIndex", validation.GetErrorField());
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
        Assert.Equal(ErrorCode.E0010, validation.Errors.GetErrorCode());
        Assert.Contains("RoomIndex", validation.GetErrorField());
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
    public async Task RoomPeopleOfReservationAdjustRequestValidator_ShouldThrowError_WhenRoomIndexIsNegative()
    {
        // Arrange
        var request = new NightPeopleOfReservationAdjustRequest(
            AppDate.GetId(DateTime.Now),
            [
                new RoomNightOfReservationAdjustRequest(
                    -1,
                    new List<PeopleOfReservationAdjustRequest> { new(1, 1, Genders.Male) }
                )
            ]
        );

        // Act
        var validation = await new RoomPeopleOfReservationAdjustRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0010, validation.Errors.GetErrorCode());
        Assert.Contains("Rooms[0].RoomIndex", validation.GetErrorField());
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
        Assert.Equal(ErrorCode.E0003, validation.Errors.GetErrorCode());
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
            new string('a', 256)
        );

        var validation = await new RoomRepresentativeOfReservationAdjustRequestValidator().ValidateAsync(request);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Kana", validation.GetErrorField());
    }

    [Fact]
    public async Task GuestOfReservationAdjustRequestValidator_ShouldThrowError_WhenFullNameExceedsMaxLength()
    {
        // Arrange
        var date = DateTime.UtcNow;
        var birthday = AppDate.GetId(date);
        var guestRequest = new GuestOfReservationAdjustRequest(
            new string('a', 256), // Exceed maximum length of 255
            "Kana",
            Genders.Male,
            birthday,
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
        var date = DateTime.UtcNow;
        var birthday = AppDate.GetId(date);
        var guestRequest = new GuestOfReservationAdjustRequest(
            "Full Name",
            new string('a', 256), // Exceed maximum length of 255
            Genders.Male,
            birthday,
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
        var date = DateTime.UtcNow;
        var birthday = AppDate.GetId(date);
        var guestRequest = new GuestOfReservationAdjustRequest(
            "Full Name",
            "Kana",
            Genders.Male,
            birthday,
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
        var date = DateTime.UtcNow;
        var birthday = AppDate.GetId(date);
        var guestRequest = new GuestOfReservationAdjustRequest(
            "Full Name",
            "Kana",
            Genders.Male,
            birthday,
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
        var date = DateTime.UtcNow;
        var birthday = AppDate.GetId(date);
        var guestRequest = new GuestOfReservationAdjustRequest(
            "Full Name",
            "Kana",
            Genders.Male,
            birthday,
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

    //1
    [Fact]
    public async Task ReserverOfReservationAdjustRequestValidator_ShouldThrowError_WhenNameIsEmpty()
    {
        var reserverOfReservationAdjustRequest = new ReserverOfReservationAdjustRequest(
            string.Empty,
            "Reserver kana",
            Genders.Male,
            "test@liberty.com",
            "014574",
            "Country",
            "Reserver address 1",
            "Reserver address 2",
            "Reserver address 3",
            "0214155455"
        );

        // Act
        var validation = await new ReserverOfReservationAdjustRequestValidator()
            .ValidateAsync(reserverOfReservationAdjustRequest);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("FullName", validation.GetErrorField());
    }

    [Fact]
    public async Task ReserverOfReservationAdjustRequestValidator_ShouldThrowError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var reserverOfReservationAdjustRequest = new ReserverOfReservationAdjustRequest(
            new string('a', 256),
            "Reserver kana",
            Genders.Male,
            "test@liberty.com",
            "014574",
            "Country",
            "Reserver address 1",
            "Reserver address 2",
            "Reserver address 3",
            "0214155455"
        );

        // Act
        var validation = await new ReserverOfReservationAdjustRequestValidator()
            .ValidateAsync(reserverOfReservationAdjustRequest);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("FullName", validation.GetErrorField());
    }

    [Fact]
    public async Task ReserverOfReservationAdjustRequestValidator_ShouldThrowError_WhenReserverKanaExceedsMaxLength()
    {
        // Arrange
        var reserverOfReservationAdjustRequest = new ReserverOfReservationAdjustRequest(
            "Reserver full name",
            new string('a', 256),
            Genders.Male,
            "test@liberty.com",
            "014574",
            "Country",
            "Reserver address 1",
            "Reserver address 2",
            "Reserver address 3",
            "0214155455"
        );

        // Act
        var validation = await new ReserverOfReservationAdjustRequestValidator()
            .ValidateAsync(reserverOfReservationAdjustRequest);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Kana", validation.GetErrorField());
    }

    [Fact]
    public async Task ReserverOfReservationAdjustRequestValidator_ShouldThrowError_WhenEmailIsEmpty()
    {
        var reserverOfReservationAdjustRequest = new ReserverOfReservationAdjustRequest(
            "Reserver full name",
            "Reserver kana",
            Genders.Male,
            string.Empty,
            "014574",
            "Country",
            "Reserver address 1",
            "Reserver address 2",
            "Reserver address 3",
            "0214155455"
        );

        var validation = await new ReserverOfReservationAdjustRequestValidator()
            .ValidateAsync(reserverOfReservationAdjustRequest);

        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Email", validation.GetErrorField());
    }

    [Fact]
    public async Task ReserverOfReservationAdjustRequestValidator_ShouldThrowError_WhenEmailIsInvalid()
    {
        var reserverOfReservationAdjustRequest = new ReserverOfReservationAdjustRequest(
            "Reserver full name",
            "Reserver kana",
            Genders.Male,
            "test",
            "014574",
            "Country",
            "Reserver address 1",
            "Reserver address 2",
            "Reserver address 3",
            "0214155455"
        );

        var validation = await new ReserverOfReservationAdjustRequestValidator()
            .ValidateAsync(reserverOfReservationAdjustRequest);

        Assert.Equal(ErrorCode.E0006, validation.Errors.GetErrorCode());
        Assert.Contains("Email", validation.GetErrorField());
    }

    [Fact]
    public async Task ReserverOfReservationAdjustRequestValidator_ShouldThrowError_WhenPostCodeIsEmpty()
    {
        var reserverOfReservationAdjustRequest = new ReserverOfReservationAdjustRequest(
            "Reserver full name",
            "Reserver kana",
            Genders.Male,
            "test@liberty.com",
            string.Empty,
            "Country",
            "Reserver address 1",
            "Reserver address 2",
            "Reserver address 3",
            "0214155455"
        );

        var validation = await new ReserverOfReservationAdjustRequestValidator()
            .ValidateAsync(reserverOfReservationAdjustRequest);

        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("PostCode", validation.GetErrorField());
    }

    [Fact]
    public async Task ReserverOfReservationAdjustRequestValidator_ShouldThrowError_WhenAddress1IsEmpty()
    {
        var reserverOfReservationAdjustRequest = new ReserverOfReservationAdjustRequest(
            "Reserver full name",
            "Reserver kana",
            Genders.Male,
            "test@liberty.com",
            "014574",
            "Country",
            string.Empty,
            "Reserver address 2",
            "Reserver address 3",
            "0214155455"
        );

        var validation = await new ReserverOfReservationAdjustRequestValidator()
            .ValidateAsync(reserverOfReservationAdjustRequest);

        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Address1", validation.GetErrorField());
    }

    [Fact]
    public async Task ReserverOfReservationAdjustRequestValidator_ShouldThrowError_WhenAddress2IsEmpty()
    {
        var reserverOfReservationAdjustRequest = new ReserverOfReservationAdjustRequest(
            "Reserver full name",
            "Reserver kana",
            Genders.Male,
            "test@liberty.com",
            "014574",
            "Country",
            "Reserver address 1",
            string.Empty,
            "Reserver address 3",
            "0214155455"
        );

        var validation = await new ReserverOfReservationAdjustRequestValidator()
            .ValidateAsync(reserverOfReservationAdjustRequest);

        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Address2", validation.GetErrorField());
    }

    [Fact]
    public async Task ReserverOfReservationAdjustRequestValidator_ShouldThrowError_WhenAddress3IsEmpty()
    {
        var reserverOfReservationAdjustRequest = new ReserverOfReservationAdjustRequest(
            "Reserver full name",
            "Reserver kana",
            Genders.Male,
            "test@liberty.com",
            "014574",
            "Country",
            "Reserver address 1",
            string.Empty,
            string.Empty,
            "0214155455"
        );

        var validation = await new ReserverOfReservationAdjustRequestValidator()
            .ValidateAsync(reserverOfReservationAdjustRequest);

        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Address2", validation.GetErrorField());
    }

    [Fact]
    public async Task ReserverOfReservationAdjustRequestValidator_ShouldThrowError_WhenPhoneNumberIsEmpty()
    {
        var reserverOfReservationAdjustRequest = new ReserverOfReservationAdjustRequest(
            "Reserver full name",
            "Reserver kana",
            Genders.Male,
            "test@liberty.com",
            "014574",
            "Country",
            "Reserver address 1",
            "Reserver address 2",
            "Reserver address 3",
            string.Empty
        );

        var validation = await new ReserverOfReservationAdjustRequestValidator()
            .ValidateAsync(reserverOfReservationAdjustRequest);

        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("PhoneNumber", validation.GetErrorField());
    }

    [Fact]
    public async Task ReserverOfReservationAdjustRequestValidator_ShouldThrowError_WhenPhoneNumberIsInvalid()
    {
        var reserverOfReservationAdjustRequest = new ReserverOfReservationAdjustRequest(
            "Reserver full name",
            "Reserver kana",
            Genders.Male,
            "test@liberty.com",
            "014574",
            "Country",
            "Reserver address 1",
            "Reserver address 2",
            "Reserver address 3",
            "1234567891234567899999"
        );

        var validation = await new ReserverOfReservationAdjustRequestValidator()
            .ValidateAsync(reserverOfReservationAdjustRequest);

        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("PhoneNumber", validation.GetErrorField());
    }
}
