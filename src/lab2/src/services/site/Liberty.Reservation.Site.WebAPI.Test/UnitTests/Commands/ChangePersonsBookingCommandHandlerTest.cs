using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Site.Application.Auth;
using Liberty.Reservation.Site.Application.Exceptions;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Commands.Booking;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using Moq;

namespace Liberty.Reservation.Site.WebAPI.Test.UnitTests.Commands;

public class ChangePersonsBookingCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnBookingPriceResponse_WhenAllValid()
    {
        var checkInDate = DateTime.Now;
        var securityContextMock = new Mock<ISecurityContextAccessor>();
        var bookingPriceOfPlanServiceMock = new Mock<IBookingPriceService>();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();

        var mockAppDatePrices = new List<AppDatePriceOfBookingResponse>
        {
            new()
            {
                AppDateId = 1,
                Price = 1000,
                TotalSpaTax = 50,
                TotalOptionPrice = 150
            }
        };

        var expectedBookingPriceResponse = new BookingPriceResponse { AppDatePrices = mockAppDatePrices };

        var mockPlan = new BookingPlanModel(
            1,
            true,
            false,
            null,
            null,
            null,
            2,
            true,
            new TimeSpan(15, 0, 0),
            new TimeSpan(21, 0, 0),
            null,
            true,
            false,
            0,
            null,
            false,
            null,
            null,
            true,
            20250612,
            20251229,
            null,
            null,
            new TimeSpan(10, 0, 0),
            false,
            20250624,
            20251229,
            new MultilingualText
            {
                { "jp", "Name Jp" },
                { "en", "Name En" }
            },
            new MultilingualText
            {
                { "jp", "Description Jp" },
                { "en", "Description En" }
            },
            new MultilingualText
            {
                { "jp", "Tag Jp" },
                { "en", "Tag En" }
            },
            new MultilingualText
            {
                { "jp", "Summary Jp" },
                { "en", "Summary En" }
            },
            PlanTypes.Combo,
            true,
            1,
            false,
            false,
            [],
            [],
            [],
            [],
            false,
            null,
            null,
            new BookingCancellationPolicyModel(),
            false,
            1,
            TimeSpan.Zero
        );

        bookingSearchServiceMock.Setup(
                x => x.GetBookingDataDetailByPlanAsync(
                    It.IsAny<BookingPlanDetailRequest>(),
                    It.IsAny<BookingSearchPlanRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(mockPlan);

        bookingCheckAvailableServiceMock.Setup(
                x => x.IsInvalidChangePersonsAsync(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

        bookingPriceOfPlanServiceMock.Setup(
                x => x.GetBookingPrice(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>()
                )
            )
            .Returns(expectedBookingPriceResponse);

        var payload = new BookingPriceRequest
        {
            RestNumber = 1,
            RoomNumber = 101,
            CheckInDate = AppDate.GetId(checkInDate),
            GuestsPerRoom =
            [
                new()
                {
                    AppDateId = AppDate.GetId(checkInDate),
                    RestIndex = 0,
                    RoomGroupIndex = 0,
                    PersonAgeTypeId = 1,
                    Persons = 1,
                    MalePersons = 1,
                    FemalePersons = 1
                }
            ]
        };

        var command = new ChangePersonsBookingCommand(1, 1) { Payload = payload };

        var cancellationToken = CancellationToken.None;

        var handler = new ChangePersonsBookingCommandHandler(
            MockUnitOfWork,
            MockMapper,
            securityContextMock.Object,
            bookingSearchServiceMock.Object,
            bookingCheckAvailableServiceMock.Object,
            bookingPriceOfPlanServiceMock.Object,
            bookingPriceOfRoomServiceMock.Object
        );

        // Act
        var result = await handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotNull(result);
        var bookingPriceResponse = Assert.IsType<BookingPriceResponse>(result, false);
        Assert.NotNull(bookingPriceResponse);
        Assert.Equal(expectedBookingPriceResponse.AppDatePrices.Count(), bookingPriceResponse.AppDatePrices.Count());
        Assert.Equal(
            expectedBookingPriceResponse.AppDatePrices.First().Price,
            bookingPriceResponse.AppDatePrices.First().Price
        ); // Check Price
        Assert.Equal(
            expectedBookingPriceResponse.AppDatePrices.First().TotalSpaTax,
            bookingPriceResponse.AppDatePrices.First().TotalSpaTax
        ); // Check TotalSpaTax
        Assert.Equal(
            expectedBookingPriceResponse.AppDatePrices.First().TotalOptionPrice,
            bookingPriceResponse.AppDatePrices.First().TotalOptionPrice
        ); // Check TotalOptionPrice
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFacilityNotfoundException()
    {
        var checkInDate = DateTime.Now;
        var securityContextMock = new Mock<ISecurityContextAccessor>();
        var bookingPriceOfPlanServiceMock = new Mock<IBookingPriceService>();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();

        var mockAppDatePrices = new List<AppDatePriceOfBookingResponse>
        {
            new()
            {
                AppDateId = 1,
                Price = 1000,
                TotalSpaTax = 50,
                TotalOptionPrice = 150
            }
        };

        var expectedBookingPriceResponse = new BookingPriceResponse { AppDatePrices = mockAppDatePrices };

        securityContextMock.Setup(
                x => x.GetFacilityIdSelected()
            )
            .Throws(new FacilityNotfoundException());

        var mockPlan = new BookingPlanModel(
            1,
            true,
            false,
            null,
            null,
            null,
            2,
            true,
            new TimeSpan(15, 0, 0),
            new TimeSpan(21, 0, 0),
            null,
            true,
            false,
            0,
            null,
            false,
            null,
            null,
            true,
            20250612,
            20251229,
            null,
            null,
            new TimeSpan(10, 0, 0),
            false,
            20250624,
            20251229,
            new MultilingualText
            {
                { "jp", "Name Jp" },
                { "en", "Name En" }
            },
            new MultilingualText
            {
                { "jp", "Description Jp" },
                { "en", "Description En" }
            },
            new MultilingualText
            {
                { "jp", "Tag Jp" },
                { "en", "Tag En" }
            },
            new MultilingualText
            {
                { "jp", "Summary Jp" },
                { "en", "Summary En" }
            },
            PlanTypes.Combo,
            true,
            1,
            false,
            false,
            [],
            [],
            [],
            [],
            false,
            null,
            null,
            new BookingCancellationPolicyModel(),
            false,
            1,
            TimeSpan.Zero
        );

        bookingSearchServiceMock.Setup(
                x => x.GetBookingDataDetailByPlanAsync(
                    It.IsAny<BookingPlanDetailRequest>(),
                    It.IsAny<BookingSearchPlanRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(mockPlan);

        bookingCheckAvailableServiceMock.Setup(
                x => x.IsInvalidChangePersonsAsync(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

        bookingPriceOfPlanServiceMock.Setup(
                x => x.GetBookingPrice(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>()
                )
            )
            .Returns(expectedBookingPriceResponse);

        var payload = new BookingPriceRequest
        {
            RestNumber = 1,
            RoomNumber = 101,
            CheckInDate = AppDate.GetId(checkInDate),
            GuestsPerRoom =
            [
                new()
                {
                    AppDateId = AppDate.GetId(checkInDate),
                    RestIndex = 0,
                    RoomGroupIndex = 0,
                    PersonAgeTypeId = 1,
                    Persons = 1,
                    MalePersons = 1,
                    FemalePersons = 1
                }
            ]
        };

        var command = new ChangePersonsBookingCommand(1, 1) { Payload = payload };

        var cancellationToken = CancellationToken.None;

        var handler = new ChangePersonsBookingCommandHandler(
            MockUnitOfWork,
            MockMapper,
            securityContextMock.Object,
            bookingSearchServiceMock.Object,
            bookingCheckAvailableServiceMock.Object,
            bookingPriceOfPlanServiceMock.Object,
            bookingPriceOfRoomServiceMock.Object
        );

        await Assert.ThrowsAsync<FacilityNotfoundException>(
            async () =>
            {
                await handler.Handle(command, cancellationToken);
            }
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPlanNotfoundException()
    {
        var checkInDate = DateTime.Now;
        var securityContextMock = new Mock<ISecurityContextAccessor>();
        var bookingPriceOfPlanServiceMock = new Mock<IBookingPriceService>();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();

        var mockAppDatePrices = new List<AppDatePriceOfBookingResponse>
        {
            new()
            {
                AppDateId = 1,
                Price = 1000,
                TotalSpaTax = 50,
                TotalOptionPrice = 150
            }
        };

        var expectedBookingPriceResponse = new BookingPriceResponse { AppDatePrices = mockAppDatePrices };

        bookingSearchServiceMock.Setup(
                x => x.GetBookingDataDetailByPlanAsync(
                    It.IsAny<BookingPlanDetailRequest>(),
                    It.IsAny<BookingSearchPlanRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((BookingPlanModel?)null);

        bookingCheckAvailableServiceMock.Setup(
                x => x.IsInvalidChangePersonsAsync(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

        bookingPriceOfPlanServiceMock.Setup(
                x => x.GetBookingPrice(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>()
                )
            )
            .Returns(expectedBookingPriceResponse);

        var payload = new BookingPriceRequest
        {
            RestNumber = 1,
            RoomNumber = 101,
            CheckInDate = AppDate.GetId(checkInDate),
            GuestsPerRoom =
            [
                new()
                {
                    AppDateId = AppDate.GetId(checkInDate),
                    RestIndex = 0,
                    RoomGroupIndex = 0,
                    PersonAgeTypeId = 1,
                    Persons = 1,
                    MalePersons = 1,
                    FemalePersons = 1
                }
            ]
        };

        var command = new ChangePersonsBookingCommand(1, 1) { Payload = payload };

        var cancellationToken = CancellationToken.None;

        var handler = new ChangePersonsBookingCommandHandler(
            MockUnitOfWork,
            MockMapper,
            securityContextMock.Object,
            bookingSearchServiceMock.Object,
            bookingCheckAvailableServiceMock.Object,
            bookingPriceOfPlanServiceMock.Object,
            bookingPriceOfRoomServiceMock.Object
        );

        await Assert.ThrowsAsync<PlanNotfoundException>(
            async () =>
            {
                await handler.Handle(command, cancellationToken);
            }
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnReservationInvalidException()
    {
        var checkInDate = DateTime.Now;
        var securityContextMock = new Mock<ISecurityContextAccessor>();
        var bookingPriceOfPlanServiceMock = new Mock<IBookingPriceService>();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();

        var mockAppDatePrices = new List<AppDatePriceOfBookingResponse>
        {
            new()
            {
                AppDateId = 1,
                Price = 1000,
                TotalSpaTax = 50,
                TotalOptionPrice = 150
            }
        };

        var expectedBookingPriceResponse = new BookingPriceResponse { AppDatePrices = mockAppDatePrices };

        var mockPlan = new BookingPlanModel(
            1,
            true,
            false,
            null,
            null,
            null,
            2,
            true,
            new TimeSpan(15, 0, 0),
            new TimeSpan(21, 0, 0),
            null,
            true,
            false,
            0,
            null,
            false,
            null,
            null,
            true,
            20250612,
            20251229,
            null,
            null,
            new TimeSpan(10, 0, 0),
            false,
            20250624,
            20251229,
            new MultilingualText
            {
                { "jp", "Name Jp" },
                { "en", "Name En" }
            },
            new MultilingualText
            {
                { "jp", "Description Jp" },
                { "en", "Description En" }
            },
            new MultilingualText
            {
                { "jp", "Tag Jp" },
                { "en", "Tag En" }
            },
            new MultilingualText
            {
                { "jp", "Summary Jp" },
                { "en", "Summary En" }
            },
            PlanTypes.Combo,
            false,
            1,
            false,
            false,
            [],
            [],
            [],
            [],
            false,
            null,
            null,
            new BookingCancellationPolicyModel(),
            false,
            1,
            TimeSpan.Zero
        );

        bookingCheckAvailableServiceMock.Setup(
                x => x.IsInvalidChangePersonsAsync(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

        bookingSearchServiceMock
            .Setup(
                x => x.GetBookingDataDetailByPlanAsync(
                    It.IsAny<BookingPlanDetailRequest>(),
                    It.IsAny<BookingSearchPlanRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(mockPlan);

        bookingPriceOfPlanServiceMock.Setup(
                x => x.GetBookingPrice(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>()
                )
            )
            .Returns(expectedBookingPriceResponse);

        var payload = new BookingPriceRequest
        {
            RestNumber = 1,
            RoomNumber = 101,
            CheckInDate = AppDate.GetId(checkInDate.AddDays(-1)),
            GuestsPerRoom =
            [
                new()
                {
                    AppDateId = AppDate.GetId(checkInDate),
                    RestIndex = 0,
                    RoomGroupIndex = 0,
                    PersonAgeTypeId = 1,
                    Persons = 1,
                    MalePersons = 1,
                    FemalePersons = 1
                }
            ]
        };

        var command = new ChangePersonsBookingCommand(1, 1) { Payload = payload };

        var cancellationToken = CancellationToken.None;

        var handler = new ChangePersonsBookingCommandHandler(
            MockUnitOfWork,
            MockMapper,
            securityContextMock.Object,
            bookingSearchServiceMock.Object,
            bookingCheckAvailableServiceMock.Object,
            bookingPriceOfPlanServiceMock.Object,
            bookingPriceOfRoomServiceMock.Object
        );

        await Assert.ThrowsAsync<ReservationPersonAgeTypeException>(
            async () =>
            {
                await handler.Handle(command, cancellationToken);
            }
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnReservationInvalidExceptionWhenInvalidChangePerson()
    {
        var checkInDate = DateTime.Now;
        var securityContextMock = new Mock<ISecurityContextAccessor>();
        var bookingPriceOfPlanServiceMock = new Mock<IBookingPriceService>();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();

        var mockAppDatePrices = new List<AppDatePriceOfBookingResponse>
        {
            new()
            {
                AppDateId = 1,
                Price = 1000,
                TotalSpaTax = 50,
                TotalOptionPrice = 150
            }
        };

        var expectedBookingPriceResponse = new BookingPriceResponse { AppDatePrices = mockAppDatePrices };

        var mockPlan = new BookingPlanModel(
            1,
            true,
            false,
            null,
            null,
            null,
            2,
            true,
            new TimeSpan(15, 0, 0),
            new TimeSpan(21, 0, 0),
            null,
            true,
            false,
            0,
            null,
            false,
            null,
            null,
            true,
            20250612,
            20251229,
            null,
            null,
            new TimeSpan(10, 0, 0),
            false,
            20250624,
            20251229,
            new MultilingualText
            {
                { "jp", "Name Jp" },
                { "en", "Name En" }
            },
            new MultilingualText
            {
                { "jp", "Description Jp" },
                { "en", "Description En" }
            },
            new MultilingualText
            {
                { "jp", "Tag Jp" },
                { "en", "Tag En" }
            },
            new MultilingualText
            {
                { "jp", "Summary Jp" },
                { "en", "Summary En" }
            },
            PlanTypes.Combo,
            true,
            1,
            false,
            false,
            [],
            [],
            [],
            [],
            false,
            null,
            null,
            new BookingCancellationPolicyModel(),
            false,
            1,
            TimeSpan.Zero
        );

        bookingCheckAvailableServiceMock.Setup(
                x => x.IsInvalidChangePersonsAsync(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);

        bookingSearchServiceMock
            .Setup(
                x => x.GetBookingDataDetailByPlanAsync(
                    It.IsAny<BookingPlanDetailRequest>(),
                    It.IsAny<BookingSearchPlanRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(mockPlan);

        bookingPriceOfPlanServiceMock.Setup(
                x => x.GetBookingPrice(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>()
                )
            )
            .Returns(expectedBookingPriceResponse);

        var payload = new BookingPriceRequest
        {
            RestNumber = 1,
            RoomNumber = 101,
            CheckInDate = AppDate.GetId(checkInDate),
            GuestsPerRoom =
            [
                new()
                {
                    AppDateId = AppDate.GetId(checkInDate),
                    RestIndex = 0,
                    RoomGroupIndex = 0,
                    PersonAgeTypeId = 1,
                    Persons = 1,
                    MalePersons = 1,
                    FemalePersons = 1
                }
            ]
        };

        var command = new ChangePersonsBookingCommand(1, 1) { Payload = payload };

        var cancellationToken = CancellationToken.None;

        var handler = new ChangePersonsBookingCommandHandler(
            MockUnitOfWork,
            MockMapper,
            securityContextMock.Object,
            bookingSearchServiceMock.Object,
            bookingCheckAvailableServiceMock.Object,
            bookingPriceOfPlanServiceMock.Object,
            bookingPriceOfRoomServiceMock.Object
        );

        await Assert.ThrowsAsync<ReservationPersonAgeTypeException>(
            async () =>
            {
                await handler.Handle(command, cancellationToken);
            }
        );
    }
}
