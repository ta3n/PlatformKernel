using System.Net;
using Liberty.Entity.ValueObjects;
using Liberty.Pagination;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.User.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.User.WebAPI.Application.Models.Responses;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Commands.BookingReservation;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;
using Liberty.Reservation.User.WebAPI.Application.Web.Extensions;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.User.WebAPI.Test.UnitTests;

public class ReservationsEndpointUnitTest : BaseUnitTest
{
    protected override void InitData()
    {
        var mockMediator = new Mock<IMediator>();

        mockMediator
            .Setup(m => m.Send(It.IsAny<BookingChangeExecutionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

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
                    new ReservationDetailsResponse
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
                            new MultilingualText
                            {
                                { "en", "<html></html>" },
                                { "ja", "<html></html>" }
                            },
                            new MultilingualText
                            {
                                { "en", "<html></html>" },
                                { "ja", "<html></html>" }
                            },
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
                            "123",
                            "123 Street",
                            " City",
                            "+1",
                            "12345",
                            "12332323345",
                            Genders.None
                        ),
                        Customer = new GuestResponse(
                            "John Doe",
                            "ジョン・ドー",
                            "123",
                            "123 Street",
                            " City",
                            "+1",
                            "12345",
                            19900101,
                            Genders.Female,
                            "121212212222"
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
                        PersonAgeTypes =
                        [
                            new(1, "Adult", true, 65, 18),
                            new(2, "Child", false, 17, 0)
                        ],
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
                    It.IsAny<ReservationGetAllPersonAgeTypesQuery>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                (
                    new HeaderDictionary
                    {
                        { "X-Total-Count", "2" },
                        { "X-Page-Size", "10" },
                        { "X-Current-Page", "1" }
                    },
                    new List<PersonAgeTypeResponse>
                    {
                        new(1, "Name", true, 18, 99),
                        new(2, "Name", false, 1, 18)
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
                                new(1, 1)
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
                                new(1, 1)
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
                                new(1, 1)
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
                                new(1, 1)
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
    public async Task ChangeExecutionOfReservation_ReturnNoContent()
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
            Id = 1,
            CheckInDateId = AppDate.GetId(checkInDate)
        };

        var controller = new ReservationsEndpoint(MockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.ChangeExecutionOfReservation(1, bookingAdjustReq, cancellationToken);
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task CancellationOfReservation_ReturnsNoContent()
    {
        // Arrange
        var bookingCancellationReq = new BookingCancellationRequest { Id = 1 };
        var controller = new ReservationsEndpoint(MockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.CancellationOfReservation(1, bookingCancellationReq, cancellationToken);
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var noContentResult = Assert.IsType<NoContentResult>(actionResultWithHeaders.Receiver, false);
        Assert.Equal((int)HttpStatusCode.NoContent, noContentResult.StatusCode);
    }

    [Fact]
    public async Task GetAllReservations_ReturnsOkObjectResult()
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
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
    }

    [Fact]
    public async Task GetAllPersonAgeTypesOfReservation_ReturnsOkObjectResult()
    {
        // Arrange
        var controller = new ReservationsEndpoint(MockMapper, MockMediator);
        var cancellationToken = CancellationToken.None;
        var mockPageable = PageableBinderConfig.DefaultPageable;

        // Act
        var result = await controller.GetAllPersonAgeTypesOfReservation(mockPageable, 1, cancellationToken);
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
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
                new()
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
        Assert.Equal((int)HttpStatusCode.OK, okObjectResult.StatusCode);
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
                new()
                {
                    AppDateId = 1,
                    RestIndex = 0,
                    RoomGroupIndex = 0,
                    PersonAgeTypeId = 1
                }
            ],
            OptionItems =
            [
                new()
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
}
