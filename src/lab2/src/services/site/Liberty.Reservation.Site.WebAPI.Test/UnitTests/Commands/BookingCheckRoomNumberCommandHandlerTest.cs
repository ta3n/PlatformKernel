using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Site.Application.Auth;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Commands.Booking;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using Moq;

namespace Liberty.Reservation.Site.WebAPI.Test.UnitTests.Commands;

public class BookingCheckRoomNumberCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnBookingPriceResponse_WhenAllValid()
    {
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();
        var securityContextMock = new Mock<ISecurityContextAccessor>();
        var bookingPriceOfPlanServiceMock = new Mock<IBookingPriceService>();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();

        var mockAppDatePrices = new List<AppDatePriceOfBookingResponse>
        {
            new()
            {
                AppDateId = 1,
                Price = 1000,
                TotalSpaTax = 50,
                TotalOptionPrice = 150
            },
            new()
            {
                AppDateId = 2,
                Price = 2000,
                TotalSpaTax = 100,
                TotalOptionPrice = 200
            }
        };

        var expectedBookingPriceResponse = new BookingPriceResponse { AppDatePrices = mockAppDatePrices };

        bookingCheckAvailableServiceMock.Setup(
                x => x.IsRoomNumberAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);

        bookingPriceOfPlanServiceMock.Setup(
                x => x.GetBookingPrice(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>()
                )
            )
            .Returns(expectedBookingPriceResponse);

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

        bookingSearchServiceMock
            .Setup(
                x => x.GetBookingDataDetailByPlanAsync(
                    It.IsAny<BookingPlanDetailRequest>(),
                    It.IsAny<BookingSearchPlanRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(mockPlan);

        var checkInDate = DateTime.Now;
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

        var command = new BookingCheckRoomNumberCommand(1, 2) { Payload = payload };

        var cancellationToken = CancellationToken.None;

        var handler = new BookingCheckRoomNumberCommandHandler(
            MockUnitOfWork,
            MockMapper,
            securityContextMock.Object,
            bookingSearchServiceMock.Object,
            bookingCheckAvailableServiceMock.Object,
            bookingPriceOfPlanServiceMock.Object,
            bookingPriceOfRoomServiceMock.Object
        );

        var result = await handler.Handle(command, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<BookingPriceResponse>(result);
        Assert.NotNull(result.AppDatePrices);
        Assert.Equal(2, result.AppDatePrices.Count());
        var firstAppDatePrice = result.AppDatePrices.First();
        Assert.Equal(1, firstAppDatePrice.AppDateId);
        Assert.Equal(1000, firstAppDatePrice.Price);
        Assert.Equal(50, firstAppDatePrice.TotalSpaTax);
        Assert.Equal(150, firstAppDatePrice.TotalOptionPrice);

        var secondAppDatePrice = result.AppDatePrices.Last();
        Assert.Equal(2, secondAppDatePrice.AppDateId);
        Assert.Equal(2000, secondAppDatePrice.Price);
        Assert.Equal(100, secondAppDatePrice.TotalSpaTax);
        Assert.Equal(200, secondAppDatePrice.TotalOptionPrice);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBookingInvalidNightNumberException()
    {
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();
        var securityContextMock = new Mock<ISecurityContextAccessor>();
        var bookingPriceOfPlanServiceMock = new Mock<IBookingPriceService>();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();

        var mockAppDatePrices = new List<AppDatePriceOfBookingResponse>
        {
            new()
            {
                AppDateId = 1,
                Price = 1000,
                TotalSpaTax = 50,
                TotalOptionPrice = 150
            },
            new()
            {
                AppDateId = 2,
                Price = 2000,
                TotalSpaTax = 100,
                TotalOptionPrice = 200
            }
        };

        var expectedBookingPriceResponse = new BookingPriceResponse { AppDatePrices = mockAppDatePrices };

        bookingCheckAvailableServiceMock.Setup(
                x => x.IsRoomNumberAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
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

        bookingSearchServiceMock
            .Setup(
                x => x.GetBookingDataDetailByPlanAsync(
                    It.IsAny<BookingPlanDetailRequest>(),
                    It.IsAny<BookingSearchPlanRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(mockPlan);

        var checkInDate = DateTime.Now;
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

        var command = new BookingCheckRoomNumberCommand(1, 2) { Payload = payload };

        var handler = new BookingCheckRoomNumberCommandHandler(
            MockUnitOfWork,
            MockMapper,
            securityContextMock.Object,
            bookingSearchServiceMock.Object,
            bookingCheckAvailableServiceMock.Object,
            bookingPriceOfPlanServiceMock.Object,
            bookingPriceOfRoomServiceMock.Object
        );

        await Assert.ThrowsAsync<ReservationRoomException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPlanNotfoundException()
    {
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();
        var securityContextMock = new Mock<ISecurityContextAccessor>();
        var bookingPriceOfPlanServiceMock = new Mock<IBookingPriceService>();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();

        var mockAppDatePrices = new List<AppDatePriceOfBookingResponse>
        {
            new()
            {
                AppDateId = 1,
                Price = 1000,
                TotalSpaTax = 50,
                TotalOptionPrice = 150
            },
            new()
            {
                AppDateId = 2,
                Price = 2000,
                TotalSpaTax = 100,
                TotalOptionPrice = 200
            }
        };

        var expectedBookingPriceResponse = new BookingPriceResponse { AppDatePrices = mockAppDatePrices };

        bookingCheckAvailableServiceMock.Setup(
                x => x.IsRoomNumberAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);

        bookingPriceOfPlanServiceMock.Setup(
                x => x.GetBookingPrice(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>()
                )
            )
            .Returns(expectedBookingPriceResponse);

        bookingSearchServiceMock
            .Setup(
                x => x.GetBookingDataDetailByPlanAsync(
                    It.IsAny<BookingPlanDetailRequest>(),
                    It.IsAny<BookingSearchPlanRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((BookingPlanModel?)null);

        var checkInDate = DateTime.Now;
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

        var command = new BookingCheckRoomNumberCommand(1, 2) { Payload = payload };

        var handler = new BookingCheckRoomNumberCommandHandler(
            MockUnitOfWork,
            MockMapper,
            securityContextMock.Object,
            bookingSearchServiceMock.Object,
            bookingCheckAvailableServiceMock.Object,
            bookingPriceOfPlanServiceMock.Object,
            bookingPriceOfRoomServiceMock.Object
        );

        await Assert.ThrowsAsync<ReservationRoomException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );
    }
}
