using Liberty.Pagination;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Site.WebAPI.Application.Boundaries.Restful;
using Liberty.Reservation.Site.WebAPI.Application.Models.Requests;
using Liberty.Reservation.Site.WebAPI.Application.Models.Responses;
using Liberty.Reservation.Site.WebAPI.Application.Web.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Liberty.Reservation.Site.WebAPI.Test.UnitTests;

public partial class BookingEndpointUnitTest
{
    [Fact]
    public async Task SearchBooking_ReturnsOkResult()
    {
        // Arrange
        var pageable = new Mock<IPageable>();
        var request = new BookingSearchPlanRequest();
        var controller = new BookingEndpoint(
            MockMapper,
            MockMediator,
            MockSecurityContextAccessor,
            MockPlanService,
            MockServiceProvider
        );

        // Act
        var result = await controller.SearchBooking(pageable.Object, request, CancellationToken.None);
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        Assert.NotNull(headers);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.IsType<List<BookingSearchByPlanResponse>>(okResult.Value, false);
    }

    [Fact]
    public async Task GetBookingDetails_ReturnsOkResultWithHeaders()
    {
        // Arrange
        var controller = new BookingEndpoint(
            MockMapper,
            MockMediator,
            MockSecurityContextAccessor,
            MockPlanService,
            MockServiceProvider
        );

        // Act
        var result = await controller.GetBookingDetails(1, 1, CancellationToken.None);
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);

        // Assert
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);
        Assert.NotNull(headers);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

        Assert.IsType<BookingDetailsResponse>(okResult.Value, false);
    }

    [Fact]
    public async Task GetBookingPriceCalendar_ReturnsOkResultWithHeaders()
    {
        // Arrange
        var request = new BookingSearchPlanRequest();
        var controller = new BookingEndpoint(
            MockMapper,
            MockMediator,
            MockSecurityContextAccessor,
            MockPlanService,
            MockServiceProvider
        );

        // Act
        var result = await controller.GetBookingPriceCalendar(1, 1, request, CancellationToken.None);
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);

        // Assert
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);
        Assert.NotNull(headers);
        Assert.IsType<PriceCalendarOfRoomResponse>(okResult.Value, false);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task CreateBookingAsync_ReturnsOkResultWithHeaders()
    {
        // Arrange
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
        ) { CheckInDateId = AppDate.GetId(checkInDate) };

        var request = new SiteBookingCreateRequest(
            AppDate.GetId(checkInDate),
            "12:00",
            "16:00",
            PaymentTypes.OnLinePayment,
            bookingAdjustReq,
            null,
            null
        );

        var controller = new BookingEndpoint(
            MockMapper,
            MockMediator,
            MockSecurityContextAccessor,
            MockPlanService,
            MockServiceProvider
        );

        // Act
        var result = await controller.CreateBookingAsync(1, 1, request, CancellationToken.None);
        var headers = ActionContext.HttpContext.Response.Headers;
        await result.ExecuteResultAsync(ActionContext);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        Assert.NotNull(headers);
        Assert.True(headers.ContainsKey("X-Liberty-alert"));
        Assert.Equal("Liberty.BookingCreateRequest.Created", headers["X-Liberty-alert"]);
        Assert.True(headers.ContainsKey("X-Liberty-params"));
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task CheckNightNumber_ReturnsOkResultWithCorrectData()
    {
        // Arrange
        var request = new BookingPriceRequest
        {
            CheckInDate = DateTime.Now.Ticks,
            RestNumber = 3,
            RoomNumber = 2,
            GuestsPerRoom =
            [
                new()
                {
                    AppDateId = 20250101,
                    RestIndex = 1,
                    RoomGroupIndex = 1,
                    PersonAgeTypeId = 1,
                    Persons = 2,
                    MalePersons = 1,
                    FemalePersons = 1
                },

                new()
                {
                    AppDateId = 20250102,
                    RestIndex = 2,
                    RoomGroupIndex = 2,
                    PersonAgeTypeId = 2,
                    Persons = 1,
                    MalePersons = 1,
                    FemalePersons = 0
                }
            ],
            OptionItems =
            [
                new()
                {
                    AppDateId = 20250101,
                    RoomGroupIndex = 1,
                    OptionItemId = 1,
                    Number = 2
                },

                new()
                {
                    AppDateId = 20250102,
                    RoomGroupIndex = 2,
                    OptionItemId = 2,
                    Number = 1
                }
            ],
            Secret = SecretTest
        };

        var controller = new BookingEndpoint(
            MockMapper,
            MockMediator,
            MockSecurityContextAccessor,
            MockPlanService,
            MockServiceProvider
        );

        // Act
        var result = await controller.CheckNightNumber(1, 1, request, CancellationToken.None);
        var okResult = Assert.IsType<OkObjectResult>(result, false);

        // Assert
        Assert.IsType<BookingPriceResponse>(okResult.Value, false);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task ChangePersonsAsync_ReturnsOkResultWithCorrectData()
    {
        // Arrange
        var planId = 1;
        var roomGroupId = 2;
        var request = new BookingPriceRequest
        {
            CheckInDate = DateTime.Now.Ticks,
            RestNumber = 3,
            RoomNumber = 2,
            GuestsPerRoom =
            [
                new()
                {
                    AppDateId = 20250101,
                    RestIndex = 1,
                    RoomGroupIndex = 1,
                    PersonAgeTypeId = 1,
                    Persons = 2,
                    MalePersons = 1,
                    FemalePersons = 1
                },

                new()
                {
                    AppDateId = 20250102,
                    RestIndex = 2,
                    RoomGroupIndex = 2,
                    PersonAgeTypeId = 2,
                    Persons = 1,
                    MalePersons = 1,
                    FemalePersons = 0
                }
            ],
            OptionItems =
            [
                new()
                {
                    AppDateId = 20250101,
                    RoomGroupIndex = 1,
                    OptionItemId = 1,
                    Number = 2
                },

                new()
                {
                    AppDateId = 20250102,
                    RoomGroupIndex = 2,
                    OptionItemId = 2,
                    Number = 1
                }
            ],
            Secret = SecretTest
        };

        var controller = new BookingEndpoint(
            MockMapper,
            MockMediator,
            MockSecurityContextAccessor,
            MockPlanService,
            MockServiceProvider
        );

        // Act
        var result = await controller.ChangePersonsAsync(planId, roomGroupId, request, CancellationToken.None);
        var okResult = Assert.IsType<OkObjectResult>(result, false);

        // Assert
        Assert.IsType<BookingPriceResponse>(okResult.Value, false);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task AdjustOptionsAsync_ReturnsOkResultWithCorrectBookingPriceData()
    {
        // Arrange
        var planId = 1L;
        var roomGroupId = 2L;
        var request = new BookingPriceRequest
        {
            CheckInDate = DateTime.Now.Ticks,
            RestNumber = 3,
            RoomNumber = 2,
            GuestsPerRoom =
            [
                new()
                {
                    AppDateId = 20250101,
                    RestIndex = 1,
                    RoomGroupIndex = 1,
                    PersonAgeTypeId = 1,
                    Persons = 2,
                    MalePersons = 1,
                    FemalePersons = 1
                }
            ],
            OptionItems =
            [
                new()
                {
                    AppDateId = 20250101,
                    RoomGroupIndex = 1,
                    OptionItemId = 1,
                    Number = 2
                }
            ],
            Secret = SecretTest
        };

        var controller = new BookingEndpoint(
            MockMapper,
            MockMediator,
            MockSecurityContextAccessor,
            MockPlanService,
            MockServiceProvider
        );

        // Act
        var result = await controller.AdjustOptionsAsync(planId, roomGroupId, request, CancellationToken.None);
        var okResult = Assert.IsType<OkObjectResult>(result, false);

        // Assert
        Assert.IsType<BookingPriceResponse>(okResult.Value, false);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }

    [Fact]
    public async Task CheckChangedAsync_ReturnsOkResultWithCorrectData()
    {
        // Arrange
        var planId = 1L;
        var roomGroupId = 2L;
        var request = new CheckChangedRequest(
            "",
            []
        );

        var controller = new BookingEndpoint(
            MockMapper,
            MockMediator,
            MockSecurityContextAccessor,
            MockPlanService,
            MockServiceProvider
        );

        // Act
        var result = await controller.CheckChangedAsync(planId, roomGroupId, request, CancellationToken.None);
        var okResult = Assert.IsType<OkObjectResult>(result, false);

        // Assert
        var responseContent = Assert.IsType<bool>(okResult.Value, false);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.True(responseContent);
    }

    [Fact]
    public async Task CheckRoomNumber_ReturnsOkResultWithCorrectData()
    {
        // Arrange
        var planId = 1;
        var roomGroupId = 2;
        var request = new BookingPriceRequest
        {
            CheckInDate = 20250101,
            RestNumber = 3,
            RoomNumber = 2,
            GuestsPerRoom = []
        };

        var controller = new BookingEndpoint(
            MockMapper,
            MockMediator,
            MockSecurityContextAccessor,
            MockPlanService,
            MockServiceProvider
        );

        // Act
        var result = await controller.CheckRoomNumber(planId, roomGroupId, request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<BookingPriceResponse>(okResult.Value);
    }

    [Fact]
    public async Task GetAllOptionItemsOfPlan_ShouldReturnListOfOptionItems_WhenValidRequestIsMade()
    {
        // Arrange
        var planId = 1;
        var request = new BookingOptionRequest(20250101);
        var pageable = PageableBinderConfig.DefaultPageable;
        var cancellationToken = CancellationToken.None;

        var controller = new BookingEndpoint(
            MockMapper,
            MockMediator,
            MockSecurityContextAccessor,
            MockPlanService,
            MockServiceProvider
        );

        // Act
        var result = await controller.GetAllOptionItemsOfPlan(planId, request, pageable, cancellationToken);

        // Assert
        var actionResultWithHeaders = Assert.IsType<ActionResultWithHeaders>(result, false);
        var okResult = Assert.IsType<OkObjectResult>(actionResultWithHeaders.Receiver, false);
        Assert.IsType<List<OptionItemOfBookingResponse>>(okResult.Value);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    }
}
