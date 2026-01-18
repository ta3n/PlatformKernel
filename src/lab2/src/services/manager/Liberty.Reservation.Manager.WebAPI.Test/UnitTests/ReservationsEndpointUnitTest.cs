using System.Globalization;
using System.Net;
using Liberty.ApplicationShared.Utils;
using Liberty.Pagination;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Application.UseCases.Commands.BookingReservation;
using Liberty.Reservation.Manager.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Manager.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BookingReservation;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Queries.BookingReservation;
using Liberty.Reservation.Manager.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest.Utilities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests;

public class ReservationsEndpointUnitTest : BaseUnitTest
{
    protected override void InitData()
    {
        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(m => m.Send(It.IsAny<BookingChangeExecutionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        mockMediator
            .Setup(m => m.Send(It.IsAny<ReservationGetAllQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (
                    new HeaderDictionary(),
                    new List<BookingReservationResponse>
                    {
                        new()
                        {
                            Id = 1,
                            Code = "RES001",
                            State = "Confirmed",
                            IsReserved = true,
                            BookingDateTime = DateTime.UtcNow.AddDays(-2),
                            ConfirmDateTime = DateTime.UtcNow.AddDays(-1),
                            CancelledDateTime = null,
                            CheckInDate = DateTime.UtcNow.AddDays(1).Ticks,
                            LengthOfStay = 3,
                            NumberOfRooms = 2,
                            ReserverName = "John Doe",
                            PaymentType = "Credit Card",
                            IsOnlinePayment = true
                        },
                        new()
                        {
                            Id = 2,
                            Code = "RES002",
                            State = "Cancelled",
                            IsReserved = false,
                            BookingDateTime = DateTime.UtcNow.AddDays(-5),
                            ConfirmDateTime = null,
                            CancelledDateTime = DateTime.UtcNow.AddDays(-4),
                            CheckInDate = DateTime.UtcNow.AddDays(-4).Ticks,
                            LengthOfStay = 2,
                            NumberOfRooms = 1,
                            ReserverName = "Jane Smith",
                            PaymentType = "Bank Transfer",
                            IsOnlinePayment = false
                        },
                        new()
                        {
                            Id = 3,
                            Code = "RES003",
                            State = "Pending",
                            IsReserved = true,
                            BookingDateTime = DateTime.UtcNow,
                            ConfirmDateTime = null,
                            CancelledDateTime = null,
                            CheckInDate = DateTime.UtcNow.AddDays(2).Ticks,
                            LengthOfStay = 1,
                            NumberOfRooms = 1,
                            ReserverName = "Alice Johnson",
                            PaymentType = "Cash",
                            IsOnlinePayment = false
                        }
                    }
                )
            );

        mockMediator
            .Setup(m => m.Send(It.IsAny<ReservationGetMonthsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (
                    new HeaderDictionary(),
                    new List<long>
                    {
                        1,
                        2,
                        3,
                        4,
                        5,
                        6
                    }
                )
            );

        mockMediator
            .Setup(
                m => m.Send(
                    It.IsAny<BookingCreateCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new ReservationEntity
                {
                    Serial = EntityUtil.CreateCode(),
                    FacilityId = 1,
                    SiteId = 1,
                    PlanId = 1,
                    RoomGroupId = 1,
                    CheckInDate = AppDate.GetId(DateTime.Now),
                    CheckInTime = new TimeSpan(23, 59, 0),
                    CheckOutTime = new TimeSpan(12, 0, 0),
                    RestNumber = 1,
                    RoomNumber = 1,
                    Reserver = new()
                    {
                        Name = "Reserver full name",
                        Kana = "Reserver kana",
                        EMail = "test@liberty.com",
                        PostCode = "Reserver post code",
                        Address1 = "Reserver address 1",
                        Address2 = "Reserver address 2",
                        Address3 = "Reserver address 3",
                        Phone = "123456789"
                    },
                    ReservationDateTime = DateTime.UtcNow,
                    ReservationState = ReservationStatus.Confirmed,
                    IsEnabled = true
                }
            );

        mockMediator
            .Setup(
                m => m.Send(
                    It.IsAny<BookingCancellationCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);

        mockMediator
            .Setup(
                m => m.Send(
                    It.IsAny<ReservationGetDetailsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    new HeaderDictionary(),
                    new ReservationDetailResponse
                    {
                        Id = 1,
                        Code = "RES-001",
                        State = ReservationStatus.Confirmed,
                        FacilityName = "Luxury Hotel",
                        PlanName = "Deluxe Package",
                        RoomGroupName = "Suite",
                        CheckInDate = AppDate.GetId(DateTime.Now.AddDays(1)),
                        NumberOfNights = 2,
                        NumberOfRooms = 1,
                        CheckInTime = new TimeSpan(14, 0, 0),
                        PaymentType = PaymentTypes.OnLinePayment,
                        AccommodationFee = 5000.00m,
                        OptionalFee = 200.00m,
                        TaxFee = 300.00m,
                        TotalFee = 5500.00m,
                        FreeInput = "Special request for room decoration",
                        IsSameMainUser = true,
                        ImportantNotes = new ImportantNotesOfReservationDetailResponse(
                            "Paid online",
                            "Vegan meals preferred",
                            "No pets allowed"
                        ),
                        Cancellation = new CancellationOfReservationResponse(
                            "Cancellation Policy",
                            "Details about cancellation policy",
                            "Cancellation Table Source",
                            "Rule Detail",
                            [
                                new CancellationDataOfReservationResponse(7, 3, 50.0f),
                                new CancellationDataOfReservationResponse(3, 0, 0.0f)
                            ]
                        ),
                        NumberOfPeople =
                        [
                            new ReservationPersonDataResponse(
                                AppDate.GetId(DateTime.Now.AddDays(1)),
                                [
                                    new RoomDataOfReservationResponse(
                                        0,
                                        [
                                            new PeoplePerRoomDataOfReservationResponse("Adult", 2, 1, 1, 0),
                                            new PeoplePerRoomDataOfReservationResponse("Child", 1, 0, 0, 1)
                                        ]
                                    )
                                ]
                            )
                        ],
                        Reserver = new ReserverResponse(
                            "John Doe",
                            "ジョン・ドー",
                            "john.doe@example.com",
                            "123 Street, City",
                            "+1",
                            "12345",
                            "12345",
                            "12345",
                            "xxx-xxxx-xxxxx",
                            Genders.Male
                        ),
                        Customer = new GuestResponse(
                            "Jane Doe",
                            "ジェーン・ドー",
                            "123 Street, City",
                            "+1",
                            "12345",
                            "1234",
                            "1234",
                            19930101,
                            Genders.Female,
                            "xxx-xxxx-xxxxx"
                        ),
                        PlanQuestions =
                        [
                            new ReservationQuestionResponse(
                                1,
                                "Do you have any allergies?",
                                "Details about allergies",
                                string.Empty,
                                "None",
                                false
                            )
                        ],
                        OptionQuestions =
                        [
                            new ReservationQuestionResponse(
                                2,
                                "Would you like additional services?",
                                "Details about services",
                                string.Empty,
                                "Yes",
                                false
                            )
                        ],
                        AppDates =
                        [
                            new BookingAppDateResponse(
                                AppDate.GetId(DateTime.Now.AddDays(1)),
                                0,
                                500.00m,
                                10.00m,
                                50.00m,
                                560.00m,
                                [
                                    new BookingRoomOfAppDate(
                                        0,
                                        500.00m,
                                        [
                                            new PeoplePriceOfRoomResponse(
                                                500.00m,
                                                10.00m,
                                                2,
                                                null,
                                                "Adult",
                                                true,
                                                1,
                                                1,
                                                0,
                                                0,
                                                500.00m
                                            )
                                        ],
                                        null,
                                        10.00m,
                                        50.00m,
                                        2
                                    )
                                ]
                            )
                        ],
                        PersonAgeTypes = new List<PersonAgeTypesResponse>
                        {
                            new(1, "Adult", true, 65, 18),
                            new(2, "Child", false, 17, 0)
                        },
                        IsChange = false,
                        IsCancel = false,
                        UseDailyPerson = false,
                        CanAddRoomOnModify = true
                    }
                )
            );

        mockMediator
            .Setup(
                m => m.Send(
                    It.IsAny<ReservationGetAllOptionItemsQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    new HeaderDictionary
                    {
                        { "X-Total-Count", "3" },
                        { "X-Page-Size", "10" },
                        { "X-Current-Page", "1" }
                    },
                    new List<OptionItemOfPlanResponse>
                    {
                        new(1, "Option 1", "des", 2),
                        new(2, "Option 2", "des", 3),
                        new(3, "Option 3", "des", 1)
                    }
                )
            );

        mockMediator
            .Setup(
                m => m.Send(
                    It.IsAny<BookingCheckNightNumberCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new BookingPriceResponse
                {
                    AppDatePrices = new List<AppDatePriceOfBookingResponse>
                    {
                        new()
                        {
                            AppDateId = 1,
                            Price = 1500,
                            TotalSpaTax = 100,
                            TotalOptionPrice = 200,
                            Rooms =
                            [
                                new RoomOfBookingResponse(1, 1)
                                {
                                    Price = 500,
                                    TotalSpaTax = 50,
                                    TotalOptionPrice = 100
                                }
                            ]
                        }
                    }
                }
            );

        mockMediator
            .Setup(
                m => m.Send(
                    It.IsAny<AdjustOptionsCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new BookingPriceResponse
                {
                    AppDatePrices =
                    [
                        new()
                        {
                            AppDateId = 1,
                            Price = 1500,
                            TotalSpaTax = 100,
                            TotalOptionPrice = 200,
                            Rooms =
                            [
                                new RoomOfBookingResponse(1, 1)
                                {
                                    Price = 500,
                                    TotalSpaTax = 50,
                                    TotalOptionPrice = 100
                                }
                            ]
                        }
                    ]
                }
            );

        mockMediator
            .Setup(
                m => m.Send(
                    It.IsAny<ChangePersonsBookingCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new BookingPriceResponse
                {
                    AppDatePrices = new List<AppDatePriceOfBookingResponse>
                    {
                        new()
                        {
                            AppDateId = 1,
                            Price = 1500,
                            TotalSpaTax = 100,
                            TotalOptionPrice = 200,
                            Rooms =
                            [
                                new RoomOfBookingResponse(1, 1)
                                {
                                    Price = 500,
                                    TotalSpaTax = 50,
                                    TotalOptionPrice = 100
                                }
                            ]
                        }
                    }
                }
            );

        mockMediator
            .Setup(
                m => m.Send(
                    It.IsAny<BookingCheckRoomNumberCommand>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new BookingPriceResponse
                {
                    AppDatePrices = new List<AppDatePriceOfBookingResponse>
                    {
                        new()
                        {
                            AppDateId = 1,
                            Price = 1500,
                            TotalSpaTax = 100,
                            TotalOptionPrice = 200,
                            Rooms =
                            [
                                new RoomOfBookingResponse(1, 1)
                                {
                                    Price = 500,
                                    TotalSpaTax = 50,
                                    TotalOptionPrice = 100
                                }
                            ]
                        }
                    }
                }
            );

        MockMediator = mockMediator.Object;
    }

    [Fact]
    public async Task CreateBooking_ReturnsCorrectResult()
    {
        // Arrange
        // var controller = new ReservationsEndpoint(MockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;
        var checkInDate = DateTime.Now;

        var bookingAdjustReq = new BookingAdjustRequest(
            true,
            "14:00",
            2,
            2,
            "Free input",
            new ReserverOfReservationAdjustRequest(
                "Reserver full name",
                "Reserver kana",
                Genders.Male,
                "test@liberty.com",
                "Reserver post code",
                "Country",
                "Reserver address 1",
                "Reserver address 2",
                "Reserver address 3",
                "123456789"
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
                "Main user post code",
                "Country",
                "Main user address 1",
                "Main user address 2",
                "Main user address 3",
                "123456789"
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
                                    1,
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
                ),
                new NightPeopleOfReservationAdjustRequest(
                    AppDate.GetId(checkInDate.AddDays(1)),
                    [
                        new RoomNightOfReservationAdjustRequest(
                            0,
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
        ) { CheckInDateId = AppDate.GetId(DateTime.Now) };

        var bookingCreateReq = new BookingCreateRequest(
            TestUtil.DefaultLanguageCode,
            1,
            1,
            1,
            1,
            AppDate.GetId(DateTime.Now),
            "14:00",
            TimeSpan.Parse("16:00", CultureInfo.InvariantCulture),
            PaymentTypes.OnSidePayment,
            bookingAdjustReq,
            "FacilityCode",
            null,
            null
        );

        // Act
        // var result = await controller.CreateBooking(bookingCreateReq, cancellationToken);
        // var headers = ActionContext.HttpContext.Response.Headers;
        // await result.ExecuteResultAsync(ActionContext);

        _ = await MockMediator.Send(
            new BookingCreateCommand(
                new BookingExternalInfoRequest(
                    null,
                    []
                )
            ) { Payload = bookingCreateReq },
            cancellationToken
        );

        // Assert
        // var actionResultWithHeaders = Assert.IsAssignableFrom<ActionResultWithHeaders>(result);
        // var okResult = Assert.IsAssignableFrom<NoContentResult>(actionResultWithHeaders.Receiver);
        // Assert.NotNull(headers);
        // Assert.Equal((int)HttpStatusCode.NoContent, okResult.StatusCode);
    }

    [Fact]
    public async Task CancellationReservation_ReturnsNoContent()
    {
        // Arrange
        var bookingCancellationReq = new BookingCancellationByManagerRequest(null);
        var controller = new ReservationsEndpoint(MockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.CancellationReservation(1, bookingCancellationReq, cancellationToken);
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task ChangeExecutionReservation_ReturnNoContent()
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
            Id = 1,
            CheckInDateId = AppDate.GetId(checkInDate)
        };

        var controller = new ReservationsEndpoint(MockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.ChangeExecutionReservation(1, bookingAdjustReq, cancellationToken);
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task AdjustOptionsAsync_ReturnsOkObjectResult()
    {
        // Arrange
        var controller = new ReservationsEndpoint(MockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        var request = new BookingPriceRequest
        {
            CheckInDate = 20230101L,
            RestNumber = 2,
            RoomNumber = 1,
            GuestsPerRoom =
            [
                new PersonOfBookingPriceRequest
                {
                    AppDateId = 1,
                    RestIndex = 0,
                    RoomGroupIndex = 0,
                    PersonAgeTypeId = 1
                }
            ],
            OptionItems =
            [
                new OptionOfBookingPriceRequest
                {
                    AppDateId = 1,
                    RoomGroupIndex = 0,
                    OptionItemId = 100,
                    Number = 2
                }
            ]
        };

        // Act
        var result = await controller.AdjustOptionsAsync(1, request, cancellationToken);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result, false);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetReservationDetails_ReturnsOkObjectResult()
    {
        // Arrange
        var controller = new ReservationsEndpoint(MockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetReservationDetails(1, cancellationToken);
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        Assert.IsType<ReservationDetailResponse>(okObjectResult.Value);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetAllOptionItemsOfPlan_ReturnsOkObjectResult()
    {
        // Arrange
        var controller = new ReservationsEndpoint(MockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;
        var mockPageable = PageableBinderConfig.DefaultPageable;

        // Act
        var result = await controller.GetAllOptionItemsOfPlan(mockPageable, 1, 123, 456, cancellationToken);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        Assert.IsType<List<OptionItemOfPlanResponse>>(okObjectResult.Value);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task CheckNightNumber_ReturnsOkObjectResult()
    {
        // Arrange
        var controller = new ReservationsEndpoint(MockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;
        var mockRequest = new BookingPriceRequest
        {
            CheckInDate = 20250101,
            RestNumber = 3,
            RoomNumber = 2,
            GuestsPerRoom = []
        };

        // Act
        var result = await controller.CheckNightNumber(1, mockRequest, cancellationToken);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result, false);
        Assert.IsType<BookingPriceResponse>(okObjectResult.Value);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task CheckRoomNumber_ReturnsOkObjectResult()
    {
        // Arrange
        var controller = new ReservationsEndpoint(MockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;
        var mockRequest = new BookingPriceRequest
        {
            CheckInDate = 20250101,
            RestNumber = 3,
            RoomNumber = 2,
            GuestsPerRoom = []
        };

        // Act
        var result = await controller.CheckRoomNumber(1, mockRequest, cancellationToken);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result, false);
        Assert.IsType<BookingPriceResponse>(okObjectResult.Value);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task ChangePersonsAsync_ReturnsOkObjectResult()
    {
        // Arrange
        var controller = new ReservationsEndpoint(MockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        var request = new BookingPriceRequest
        {
            CheckInDate = 20230101L,
            RestNumber = 3,
            RoomNumber = 2,
            GuestsPerRoom =
            [
                new PersonOfBookingPriceRequest
                {
                    AppDateId = 1,
                    RestIndex = 0,
                    RoomGroupIndex = 0,
                    PersonAgeTypeId = 1
                }
            ]
        };

        // Act
        var result = await controller.ChangePersonsAsync(1, request, cancellationToken);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(result, false);
        Assert.IsType<BookingPriceResponse>(okObjectResult.Value);
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetAllMonthsOfReservations_ReturnsOkObjectResult()
    {
        // Arrange
        var controller = new ReservationsEndpoint(MockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetAllMonthsOfReservations(cancellationToken);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        Assert.IsType<List<long>>(okResult.Value);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
    }

    [Fact]
    public async Task NoShowReservation_ReturnsNoContent()
    {
        await CreateBooking_ReturnsCorrectResult();
        // Arrange
        var bookingNoShowReq = new BookingNoShowRequest(1, "test");
        var controller = new ReservationsEndpoint(MockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.NoShowReservation(1, bookingNoShowReq, cancellationToken);
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }
}
