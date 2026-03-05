using Liberty.Pagination;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Application.Utils;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;
using Liberty.Reservation.Site.WebAPI.Application.Validations;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using Liberty.SysException;
using BookingCreateCommand = Liberty.Reservation.Site.WebAPI.Application.UserCases.Commands.Booking.BookingCreateCommand;

namespace Liberty.Reservation.Site.WebAPI.Test.UnitTests;

public class BookingEndpointValidationUnitTest : BaseUnitTest
{
    [Fact]
    public async Task BookingSearchQueryValidator_ShouldThrowError_WhenGuestsPerRoomIsNull()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var bookingSearchQuery = new BookingSearchByPlanQuery(
            new BookingSearchPlanRequest
            {
                GuestsPerRoom = null,
                RestNumber = 2,
                RoomNumber = 1,
                CheckInDate = 20250101,
                CheckOutDate = 20250105,
                DisplayCheckInDate = 20250101,
                DisplayCheckOutDate = 20250105,
                MinPrice = 100,
                MaxPrice = 500
            },
            pageable
        );

        // Act
        var validation = await new BookingSearchQueryValidator().ValidateAsync(bookingSearchQuery);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Equal("GuestsPerRoom", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingSearchQueryValidator_ShouldThrowError_WhenGuestsPerRoomIsEmpty()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var bookingSearchQuery = new BookingSearchByPlanQuery(
            new BookingSearchPlanRequest
            {
                GuestsPerRoom = [],
                RestNumber = 2,
                RoomNumber = 1,
                CheckInDate = 20250101,
                CheckOutDate = 20250105,
                DisplayCheckInDate = 20250101,
                DisplayCheckOutDate = 20250105,
                MinPrice = 100,
                MaxPrice = 500
            },
            pageable
        );

        // Act
        var validation = await new BookingSearchQueryValidator().ValidateAsync(bookingSearchQuery);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Equal("GuestsPerRoom", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingSearchQueryValidator_ShouldThrowError_WhenRestNumberIsNull()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var bookingSearchQuery = new BookingSearchByPlanQuery(
            new BookingSearchPlanRequest
            {
                GuestsPerRoom =
                [
                    new()
                    {
                        AppDateId = 1,
                        PersonAgeTypeId = 1
                    }
                ],
                RestNumber = 0,
                RoomNumber = 1,
                CheckInDate = 20250101,
                CheckOutDate = 20250105,
                DisplayCheckInDate = 20250101,
                DisplayCheckOutDate = 20250105,
                MinPrice = 100,
                MaxPrice = 500
            },
            pageable
        );

        // Act
        var validation = await new BookingSearchQueryValidator().ValidateAsync(bookingSearchQuery);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Equal("RestNumber", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingSearchQueryValidator_ShouldThrowError_WhenRestNumberIsNegative()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var bookingSearchQuery = new BookingSearchByPlanQuery(
            new BookingSearchPlanRequest
            {
                GuestsPerRoom =
                [
                    new PersonOfBookingSearchModel
                    {
                        AppDateId = 1,
                        PersonAgeTypeId = 1
                    }
                ],
                RestNumber = -1,
                RoomNumber = 1,
                CheckInDate = 20250101,
                CheckOutDate = 20250105,
                DisplayCheckInDate = 20250101,
                DisplayCheckOutDate = 20250105,
                MinPrice = 100,
                MaxPrice = 500
            },
            pageable
        );

        // Act
        var validation = await new BookingSearchQueryValidator().ValidateAsync(bookingSearchQuery);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Equal("RestNumber", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingSearchQueryValidator_ShouldThrowError_WhenRoomNumberIsNull()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var bookingSearchQuery = new BookingSearchByPlanQuery(
            new BookingSearchPlanRequest
            {
                GuestsPerRoom =
                [
                    new PersonOfBookingSearchModel
                    {
                        AppDateId = 1,
                        PersonAgeTypeId = 1
                    }
                ],
                RestNumber = 1,
                RoomNumber = 0,
                CheckInDate = 20250101,
                CheckOutDate = 20250105,
                DisplayCheckInDate = 20250101,
                DisplayCheckOutDate = 20250105,
                MinPrice = 100,
                MaxPrice = 500
            },
            pageable
        );

        // Act
        var validation = await new BookingSearchQueryValidator().ValidateAsync(bookingSearchQuery);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Equal("RoomNumber", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingSearchQueryValidator_ShouldThrowError_WhenRoomNumberIsNegative()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var bookingSearchQuery = new BookingSearchByPlanQuery(
            new BookingSearchPlanRequest
            {
                GuestsPerRoom =
                [
                    new PersonOfBookingSearchModel
                    {
                        AppDateId = 1,
                        PersonAgeTypeId = 1
                    }
                ],
                RestNumber = 1,
                RoomNumber = -1,
                CheckInDate = 20250101,
                CheckOutDate = 20250105,
                DisplayCheckInDate = 20250101,
                DisplayCheckOutDate = 20250105,
                MinPrice = 100,
                MaxPrice = 500
            },
            pageable
        );

        // Act
        var validation = await new BookingSearchQueryValidator().ValidateAsync(bookingSearchQuery);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Equal("RoomNumber", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingSearchQueryValidator_ShouldThrowError_WhenCheckInDateIsEmpty()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var bookingSearchQuery = new BookingSearchByPlanQuery(
            new BookingSearchPlanRequest
            {
                GuestsPerRoom = new List<PersonOfBookingSearchModel>
                {
                    new()
                    {
                        AppDateId = 1,
                        PersonAgeTypeId = 1
                    }
                },
                RestNumber = 1,
                RoomNumber = 1,
                CheckInDate = 0,
                CheckOutDate = 20250105,
                DisplayCheckInDate = 0,
                DisplayCheckOutDate = 20250105,
                MinPrice = 100,
                MaxPrice = 500
            },
            pageable
        );

        // Act
        var validation = await new BookingSearchQueryValidator().ValidateAsync(bookingSearchQuery);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Equal("CheckInDate", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingSearchQueryValidator_ShouldThrowError_WhenCheckInDateIsInvalid()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var bookingSearchQuery = new BookingSearchByPlanQuery(
            new BookingSearchPlanRequest
            {
                GuestsPerRoom = new List<PersonOfBookingSearchModel>
                {
                    new()
                    {
                        AppDateId = 1,
                        PersonAgeTypeId = 1
                    }
                },
                RestNumber = 1,
                RoomNumber = 1,
                CheckInDate = 11111,
                CheckOutDate = 20250105,
                DisplayCheckInDate = 11111,
                DisplayCheckOutDate = 20250105,
                MinPrice = 100,
                MaxPrice = 500
            },
            pageable
        );

        // Act
        var validation = await new BookingSearchQueryValidator().ValidateAsync(bookingSearchQuery);

        // Assert
        Assert.Equal(ErrorCode.E0008, validation.Errors.GetErrorCode());
        Assert.Equal("CheckInDate", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingSearchQueryValidator_ShouldThrowError_WhenCheckOutDateIsLessThanOrEqualToCheckInDate()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var bookingSearchQuery = new BookingSearchByPlanQuery(
            new BookingSearchPlanRequest
            {
                GuestsPerRoom = new List<PersonOfBookingSearchModel>
                {
                    new()
                    {
                        AppDateId = 20250101,
                        PersonAgeTypeId = 1
                    }
                },
                RestNumber = 1,
                RoomNumber = 1,
                CheckInDate = 20250105,
                CheckOutDate = 20250101,
                DisplayCheckInDate = 20250105,
                DisplayCheckOutDate = 20250101,
                MinPrice = 100,
                MaxPrice = 500
            },
            pageable
        );

        // Act
        var validation = await new BookingSearchQueryValidator().ValidateAsync(bookingSearchQuery);

        // Assert
        Assert.Equal(ErrorCode.E2042, validation.Errors.GetErrorCode());
        Assert.Equal("CheckOutDate", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingSearchQueryValidator_ShouldThrowError_WhenCheckOutDateIsInvalid()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var bookingSearchQuery = new BookingSearchByPlanQuery(
            new BookingSearchPlanRequest
            {
                GuestsPerRoom =
                [
                    new PersonOfBookingSearchModel
                    {
                        AppDateId = 1,
                        PersonAgeTypeId = 1
                    }
                ],
                RestNumber = 1,
                RoomNumber = 1,
                CheckInDate = 20250101,
                CheckOutDate = 11111,
                DisplayCheckInDate = 20250101,
                DisplayCheckOutDate = 11111,
                MinPrice = 100,
                MaxPrice = 500
            },
            pageable
        );

        // Act
        var validation = await new BookingSearchQueryValidator().ValidateAsync(bookingSearchQuery);

        // Assert
        Assert.Equal(ErrorCode.E0008, validation.Errors.GetErrorCode());
        Assert.Equal("CheckOutDate", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingSearchQueryValidator_ShouldThrowError_WhenCheckOutDateIsEmpty()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var bookingSearchQuery = new BookingSearchByPlanQuery(
            new BookingSearchPlanRequest
            {
                GuestsPerRoom = new List<PersonOfBookingSearchModel>
                {
                    new()
                    {
                        AppDateId = 1,
                        PersonAgeTypeId = 1
                    }
                },
                RestNumber = 1,
                RoomNumber = 1,
                CheckInDate = 20250101,
                CheckOutDate = 0,
                DisplayCheckInDate = 20250101,
                DisplayCheckOutDate = 0,
                MinPrice = 100,
                MaxPrice = 500
            },
            pageable
        );

        // Act
        var validation = await new BookingSearchQueryValidator().ValidateAsync(bookingSearchQuery);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Equal("CheckOutDate", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingSearchQueryValidator_ShouldThrowError_WhenMinPriceIsNegative()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var bookingSearchQuery = new BookingSearchByPlanQuery(
            new BookingSearchPlanRequest
            {
                GuestsPerRoom = new List<PersonOfBookingSearchModel>
                {
                    new()
                    {
                        AppDateId = 1,
                        PersonAgeTypeId = 1
                    }
                },
                RestNumber = 1,
                RoomNumber = 1,
                MinPrice = -100,
                MaxPrice = 500,
                CheckInDate = 20250101,
                CheckOutDate = 20250105,
                DisplayCheckInDate = 20250101,
                DisplayCheckOutDate = 20250105
            },
            pageable
        );

        // Act
        var validation = await new BookingSearchQueryValidator().ValidateAsync(bookingSearchQuery);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Equal("MinPrice", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingSearchQueryValidator_ShouldThrowError_WhenMaxPriceIsNegative()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var bookingSearchQuery = new BookingSearchByPlanQuery(
            new BookingSearchPlanRequest
            {
                GuestsPerRoom = new List<PersonOfBookingSearchModel>
                {
                    new()
                    {
                        AppDateId = 1,
                        PersonAgeTypeId = 1
                    }
                },
                RestNumber = 1,
                RoomNumber = 1,
                MinPrice = 100,
                MaxPrice = -50,
                CheckInDate = 20250101,
                CheckOutDate = 20250105,
                DisplayCheckInDate = 20250101,
                DisplayCheckOutDate = 20250105
            },
            pageable
        );

        // Act
        var validation = await new BookingSearchQueryValidator().ValidateAsync(bookingSearchQuery);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Equal("MaxPrice", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingSearchQueryValidator_ShouldThrowError_WhenMaxPriceIsLessThanMinPrice()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var bookingSearchQuery = new BookingSearchByPlanQuery(
            new BookingSearchPlanRequest
            {
                GuestsPerRoom =
                [
                    new()
                    {
                        AppDateId = 1,
                        PersonAgeTypeId = 1
                    }
                ],
                RestNumber = 1,
                RoomNumber = 1,
                MinPrice = 300,
                MaxPrice = 200,
                CheckInDate = 20250101,
                CheckOutDate = 20250105,
                DisplayCheckInDate = 20250101,
                DisplayCheckOutDate = 20250105
            },
            pageable
        );

        // Act
        var validation = await new BookingSearchQueryValidator().ValidateAsync(bookingSearchQuery);

        // Assert
        Assert.Equal(ErrorCode.E2043, validation.Errors.GetErrorCode());
        Assert.Equal("MaxPrice", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingSearchQueryValidator_ShouldThrowError_WhenGuestsPerRoomContainsElementHavePersonAgeTypeIdIsNull()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var bookingSearchQuery = new BookingSearchByPlanQuery(
            new BookingSearchPlanRequest
            {
                GuestsPerRoom =
                [
                    new() { AppDateId = 1 }
                ],
                RestNumber = 1,
                RoomNumber = 1,
                MinPrice = 300,
                MaxPrice = 400,
                CheckInDate = 20250101,
                CheckOutDate = 20250105,
                DisplayCheckInDate = 20250101,
                DisplayCheckOutDate = 20250105
            },
            pageable
        );

        // Act
        var validation = await new BookingSearchQueryValidator().ValidateAsync(bookingSearchQuery);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("PersonAgeTypeId", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingSearchQueryValidator_ShouldThrowError_WhenGuestsPerRoomContainsElementHavePersonAgeTypeIdIsNegative()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var bookingSearchQuery = new BookingSearchByPlanQuery(
            new BookingSearchPlanRequest
            {
                GuestsPerRoom =
                [
                    new()
                    {
                        AppDateId = 1,
                        PersonAgeTypeId = -1
                    }
                ],
                RestNumber = 1,
                RoomNumber = 1,
                MinPrice = 300,
                MaxPrice = 400,
                CheckInDate = 20250101,
                CheckOutDate = 20250105,
                DisplayCheckInDate = 20250101,
                DisplayCheckOutDate = 20250105
            },
            pageable
        );

        // Act
        var validation = await new BookingSearchQueryValidator().ValidateAsync(bookingSearchQuery);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("PersonAgeTypeId", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingSearchQueryValidator_ShouldThrowError_WhenGuestsPerRoomContainsElementHaveAppDateIdIsNull()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var bookingSearchQuery = new BookingSearchByPlanQuery(
            new BookingSearchPlanRequest
            {
                GuestsPerRoom =
                [
                    new() { PersonAgeTypeId = 1 }
                ],
                RestNumber = 1,
                RoomNumber = 1,
                MinPrice = 300,
                MaxPrice = 400,
                CheckInDate = 20250101,
                CheckOutDate = 20250105,
                DisplayCheckInDate = 20250101,
                DisplayCheckOutDate = 20250105
            },
            pageable
        );

        // Act
        var validation = await new BookingSearchQueryValidator().ValidateAsync(bookingSearchQuery);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("AppDateId", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingSearchQueryValidator_ShouldThrowError_WhenGuestsPerRoomContainsElementHaveAppDateIdIsNegative()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var bookingSearchQuery = new BookingSearchByPlanQuery(
            new BookingSearchPlanRequest
            {
                GuestsPerRoom =
                [
                    new()
                    {
                        PersonAgeTypeId = 1,
                        AppDateId = -1
                    }
                ],
                RestNumber = 1,
                RoomNumber = 1,
                MinPrice = 300,
                MaxPrice = 400,
                CheckInDate = 20250101,
                CheckOutDate = 20250105,
                DisplayCheckInDate = 20250101,
                DisplayCheckOutDate = 20250105
            },
            pageable
        );

        // Act
        var validation = await new BookingSearchQueryValidator().ValidateAsync(bookingSearchQuery);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("AppDateId", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingSearchQueryValidator_ShouldThrowError_WhenGuestsPerRoomContainsElementHaveAppDateIdIsInvalid()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var bookingSearchQuery = new BookingSearchByPlanQuery(
            new BookingSearchPlanRequest
            {
                GuestsPerRoom =
                [
                    new()
                    {
                        PersonAgeTypeId = 1,
                        AppDateId = 11111
                    }
                ],
                RestNumber = 1,
                RoomNumber = 1,
                MinPrice = 300,
                MaxPrice = 400,
                CheckInDate = 20250101,
                CheckOutDate = 20250105,
                DisplayCheckInDate = 20250101,
                DisplayCheckOutDate = 20250105
            },
            pageable
        );

        // Act
        var validation = await new BookingSearchQueryValidator().ValidateAsync(bookingSearchQuery);

        // Assert
        Assert.Equal(ErrorCode.E0008, validation.Errors.GetErrorCode());
        Assert.Contains("AppDateId", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingSearchQueryValidator_ShouldThrowError_WhenGuestsPerRoomContainsElementHaveRestIndexIsNegative()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var bookingSearchQuery = new BookingSearchByPlanQuery(
            new BookingSearchPlanRequest
            {
                GuestsPerRoom =
                [
                    new()
                    {
                        PersonAgeTypeId = 1,
                        AppDateId = 20250101,
                        RestIndex = -1
                    }
                ],
                RestNumber = 1,
                RoomNumber = 1,
                MinPrice = 300,
                MaxPrice = 400,
                CheckInDate = 20250101,
                CheckOutDate = 20250105,
                DisplayCheckInDate = 20250101,
                DisplayCheckOutDate = 20250105
            },
            pageable
        );

        // Act
        var validation = await new BookingSearchQueryValidator().ValidateAsync(bookingSearchQuery);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Contains("RestIndex", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingSearchQueryValidator_ShouldThrowError_WhenGuestsPerRoomContainsElementHaveRoomGroupIndexIsNegative()
    {
        // Arrange
        var pageable = PageableBinderConfig.DefaultPageable;
        var bookingSearchQuery = new BookingSearchByPlanQuery(
            new BookingSearchPlanRequest
            {
                GuestsPerRoom =
                [
                    new()
                    {
                        PersonAgeTypeId = 1,
                        AppDateId = 20250101,
                        RoomGroupIndex = -1
                    }
                ],
                RestNumber = 1,
                RoomNumber = 1,
                MinPrice = 300,
                MaxPrice = 400,
                CheckInDate = 20250101,
                CheckOutDate = 20250105,
                DisplayCheckInDate = 20250101,
                DisplayCheckOutDate = 20250105
            },
            pageable
        );

        // Act
        var validation = await new BookingSearchQueryValidator().ValidateAsync(bookingSearchQuery);

        // Assert
        Assert.Equal(ErrorCode.E0011, validation.Errors.GetErrorCode());
        Assert.Contains("RoomGroupIndex", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenPlanIdIsNegative()
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

        var bookingCreateCommand = new BookingCreateCommand(-1, 1)
        {
            Payload = new(
                AppDate.GetId(checkInDate),
                "12:00",
                "16:00",
                PaymentTypes.OnLinePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("PlanId", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenPlanIdIsEmpty()
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

        var bookingCreateCommand = new BookingCreateCommand(0, 1)
        {
            Payload = new(
                AppDate.GetId(checkInDate),
                "12:00",
                "16:00",
                PaymentTypes.OnLinePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("PlanId", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenRoomGroupIdIsNegative()
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

        var bookingCreateCommand = new BookingCreateCommand(1, -1)
        {
            Payload = new(
                AppDate.GetId(checkInDate),
                "12:00",
                "16:00",
                PaymentTypes.OnLinePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomGroupId", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenRoomGroupIdIsNull()
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

        var bookingCreateCommand = new BookingCreateCommand(1, 0)
        {
            Payload = new(
                AppDate.GetId(checkInDate),
                "12:00",
                "16:00",
                PaymentTypes.OnLinePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("RoomGroupId", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenCheckInDateIsInvalid()
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

        var bookingCreateCommand = new BookingCreateCommand(1, 1)
        {
            Payload = new(
                111,
                "12:00",
                "16:00",
                PaymentTypes.OnSidePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0008, validation.Errors.GetErrorCode());
        Assert.Contains("CheckInDate", validation.GetErrorField());
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

        var bookingCreateCommand = new BookingCreateCommand(1, 1)
        {
            Payload = new(
                AppDate.GetId(checkInDate),
                "12:00",
                "16:00",
                PaymentTypes.OnLinePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E2041, validation.Errors.GetErrorCode());
        Assert.Contains("IsAgree", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenNumberOfNightsIsEmpty()
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
        ) { CheckInDateId = AppDate.GetId(checkInDate) };

        var bookingCreateCommand = new BookingCreateCommand(1, 1)
        {
            Payload = new(
                AppDate.GetId(checkInDate),
                "12:00",
                "16:00",
                PaymentTypes.OnLinePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("NumberOfNights", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenNumberOfNightsIsLessThanOne()
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
        ) { CheckInDateId = AppDate.GetId(checkInDate) };

        var bookingCreateCommand = new BookingCreateCommand(1, 1)
        {
            Payload = new(
                AppDate.GetId(checkInDate),
                "12:00",
                "16:00",
                PaymentTypes.OnLinePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("NumberOfNights", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenNumberOfRoomsIsEmpty()
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
        ) { CheckInDateId = AppDate.GetId(checkInDate) };

        var bookingCreateCommand = new BookingCreateCommand(1, 1)
        {
            Payload = new(
                AppDate.GetId(checkInDate),
                "12:00",
                "16:00",
                PaymentTypes.OnLinePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("NumberOfRooms", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WheNumberOfRoomsIsLessThanOne()
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
        ) { CheckInDateId = AppDate.GetId(checkInDate) };

        var bookingCreateCommand = new BookingCreateCommand(1, 1)
        {
            Payload = new(
                AppDate.GetId(checkInDate),
                "12:00",
                "16:00",
                PaymentTypes.OnLinePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("NumberOfRooms", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenFreeInputExceedsMaxLength()
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
        ) { CheckInDateId = AppDate.GetId(checkInDate) };

        var bookingCreateCommand = new BookingCreateCommand(1, 1)
        {
            Payload = new(
                AppDate.GetId(checkInDate),
                "12:00",
                "16:00",
                PaymentTypes.OnLinePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("FreeInput", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenReserverIsNull()
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

        var bookingCreateCommand = new BookingCreateCommand(-1, 1)
        {
            Payload = new(
                AppDate.GetId(checkInDate),
                "12:00",
                "16:00",
                PaymentTypes.OnLinePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("PlanId", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenReserverNameIsEmpty()
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

        var bookingCreateCommand = new BookingCreateCommand(1, 1)
        {
            Payload = new(
                AppDate.GetId(checkInDate),
                "12:00",
                "16:00",
                PaymentTypes.OnLinePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("FullName", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenReserverNameExceedsMaxLength()
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
            new List<NightPeopleOfReservationAdjustRequest>(),
            null,
            new List<RoomRepresentativeOfReservationAdjustRequest>(),
            null,
            null
        ) { CheckInDateId = AppDate.GetId(checkInDate) };

        var bookingCreateCommand = new BookingCreateCommand(1, 1)
        {
            Payload = new(
                AppDate.GetId(checkInDate),
                "12:00",
                "16:00",
                PaymentTypes.OnLinePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("FullName", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenReserverKanaExceedsMaxLength()
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
                new string('a', 256),
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

        var bookingCreateCommand = new BookingCreateCommand(1, 1)
        {
            Payload = new(
                AppDate.GetId(checkInDate),
                "12:00",
                "16:00",
                PaymentTypes.OnLinePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("Kana", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenReserverEmailIsEmpty()
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
                string.Empty,
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

        var bookingCreateCommand = new BookingCreateCommand(1, 1)
        {
            Payload = new(
                AppDate.GetId(checkInDate),
                "12:00",
                "16:00",
                PaymentTypes.OnLinePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Email", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenReserverEmailIsInvalid()
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
                "test",
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

        var bookingCreateCommand = new BookingCreateCommand(1, 1)
        {
            Payload = new(
                AppDate.GetId(checkInDate),
                "12:00",
                "16:00",
                PaymentTypes.OnLinePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0006, validation.Errors.GetErrorCode());
        Assert.Contains("Email", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenReserverPostCodeIsEmpty()
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
                string.Empty,
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

        var bookingCreateCommand = new BookingCreateCommand(1, 1)
        {
            Payload = new(
                AppDate.GetId(checkInDate),
                "12:00",
                "16:00",
                PaymentTypes.OnLinePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("PostCode", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenReserverAddress1IsEmpty()
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
                string.Empty,
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

        var bookingCreateCommand = new BookingCreateCommand(1, 1)
        {
            Payload = new(
                AppDate.GetId(checkInDate),
                "12:00",
                "16:00",
                PaymentTypes.OnLinePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Address1", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenReserverAddress2IsEmpty()
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
                string.Empty,
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

        var bookingCreateCommand = new BookingCreateCommand(1, 1)
        {
            Payload = new(
                AppDate.GetId(checkInDate),
                "12:00",
                "16:00",
                PaymentTypes.OnLinePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Address2", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenReserverAddress3IsEmpty()
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
                string.Empty,
                string.Empty,
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

        var bookingCreateCommand = new BookingCreateCommand(1, 1)
        {
            Payload = new(
                AppDate.GetId(checkInDate),
                "12:00",
                "16:00",
                PaymentTypes.OnLinePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("Address2", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenReserverPhoneNumberIsEmpty()
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
                string.Empty
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

        var bookingCreateCommand = new BookingCreateCommand(1, 1)
        {
            Payload = new(
                AppDate.GetId(checkInDate),
                "12:00",
                "16:00",
                PaymentTypes.OnLinePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0001, validation.Errors.GetErrorCode());
        Assert.Contains("PhoneNumber", validation.GetErrorField());
    }

    [Fact]
    public async Task BookingCreateCommandValidator_ShouldThrowError_WhenReserverPhoneNumberIsInvalid()
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
                "1234567891234567899999"
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

        var bookingCreateCommand = new BookingCreateCommand(1, 1)
        {
            Payload = new(
                AppDate.GetId(checkInDate),
                "12:00",
                "16:00",
                PaymentTypes.OnLinePayment,
                bookingAdjustReq,
                null,
                null
            )
        };

        // Act
        var validation = await new BookingCreateCommandValidator().ValidateAsync(bookingCreateCommand);

        // Assert
        Assert.Equal(ErrorCode.E0002, validation.Errors.GetErrorCode());
        Assert.Contains("PhoneNumber", validation.GetErrorField());
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
        Assert.Equal(ErrorCode.E0010, validation.Errors.GetErrorCode());
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
}
