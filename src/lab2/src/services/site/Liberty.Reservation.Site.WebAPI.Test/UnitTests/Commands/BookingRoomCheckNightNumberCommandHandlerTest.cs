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

public class BookingRoomCheckNightNumberCommandHandlerTest : BaseUnitTest
{
    private static List<AppDatePriceOfBookingResponse> CreateMockAppDatePrices()
    {
        return
        [
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
        ];
    }

    private static BookingPriceRequest CreateBookingPriceRequest()
    {
        var optionRequest = new List<OptionOfBookingPriceRequest>
        {
            new()
            {
                AppDateId = AppDate.GetId(DateTime.UtcNow),
                RoomGroupIndex = 0,
                Number = 1,
                OptionItemId = 1
            }
        };
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
                FemalePersons = 0
            }
        ];
        return new BookingPriceRequest
        {
            RestNumber = 1,
            RoomNumber = 1,
            CheckInDate = checkInDate,
            GuestsPerRoom = guestsPerRoom,
            OptionItems = optionRequest
        };
    }

    private static BookingPlanModel GetMockBookingPlanModel()
    {
        return new BookingPlanModel(
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
    }

    private static void SetupCommonMocks(
        Mock<IBookingCheckAvailableService> bookingCheckAvailableServiceMock,
        Mock<IBookingSearchService> bookingSearchServiceMock,
        Mock<ISecurityContextAccessor> securityContextMock,
        bool isNightNumberValid = true,
        BookingPlanModel? planModelMock = null
    )
    {
        bookingCheckAvailableServiceMock
            .Setup(
                x => x.IsNightNumberAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(isNightNumberValid);

        bookingSearchServiceMock
            .Setup(
                x => x.GetBookingDataDetailByPlanAsync(
                    It.IsAny<BookingPlanDetailRequest>(),
                    It.IsAny<BookingSearchPlanRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(planModelMock);

        securityContextMock
            .Setup(x => x.GetFacilityIdSelected())
            .Returns(1);

        securityContextMock
            .Setup(x => x.GetSiteIdSelected())
            .Returns(1);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBookingPriceResponse_WhenAllValid()
    {
        // Arrange
        var securityContextMock = new Mock<ISecurityContextAccessor>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();
        var bookingPriceOfPlanServiceMock = new Mock<IBookingPriceService>();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();

        var handler = new BookingCheckNightNumberCommandHandler(
            MockUnitOfWork,
            MockMapper,
            securityContextMock.Object,
            bookingSearchServiceMock.Object,
            bookingCheckAvailableServiceMock.Object,
            bookingPriceOfPlanServiceMock.Object,
            bookingPriceOfRoomServiceMock.Object
        );

        SetupCommonMocks(
            bookingCheckAvailableServiceMock,
            bookingSearchServiceMock,
            securityContextMock,
            planModelMock: GetMockBookingPlanModel()
        );

        var mockAppDatePrices = CreateMockAppDatePrices();
        var expectedBookingPriceResponse = new BookingPriceResponse { AppDatePrices = mockAppDatePrices };
        var bookingPriceRequest = CreateBookingPriceRequest();

        bookingPriceOfPlanServiceMock
            .Setup(
                x => x.GetBookingPrice(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>()
                )
            )
            .Returns(expectedBookingPriceResponse);

        bookingPriceOfRoomServiceMock
            .Setup(
                x => x.GetBookingPrice(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>()
                )
            )
            .Returns(expectedBookingPriceResponse);

        var command = new BookingCheckNightNumberCommand(1, 1) { Payload = bookingPriceRequest };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BookingPriceResponse>(result);
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
        // Arrange
        var securityContextMock = new Mock<ISecurityContextAccessor>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();
        var bookingPriceOfPlanServiceMock = new Mock<IBookingPriceService>();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();

        var handler = new BookingCheckNightNumberCommandHandler(
            MockUnitOfWork,
            MockMapper,
            securityContextMock.Object,
            bookingSearchServiceMock.Object,
            bookingCheckAvailableServiceMock.Object,
            bookingPriceOfPlanServiceMock.Object,
            bookingPriceOfRoomServiceMock.Object
        );

        SetupCommonMocks(
            bookingCheckAvailableServiceMock,
            bookingSearchServiceMock,
            securityContextMock,
            false,
            GetMockBookingPlanModel()
        );

        var mockAppDatePrices = CreateMockAppDatePrices();
        var expectedBookingPriceResponse = new BookingPriceResponse { AppDatePrices = mockAppDatePrices };
        var bookingPriceRequest = CreateBookingPriceRequest();

        bookingPriceOfPlanServiceMock
            .Setup(
                x => x.GetBookingPrice(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>()
                )
            )
            .Returns(expectedBookingPriceResponse);

        bookingPriceOfRoomServiceMock
            .Setup(
                x => x.GetBookingPrice(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>()
                )
            )
            .Returns(expectedBookingPriceResponse);

        var command = new BookingCheckNightNumberCommand(1, 1) { Payload = bookingPriceRequest };

        // Act & Assert
        await Assert.ThrowsAsync<ReservationNightException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPlanNotfoundException()
    {
        // Arrange
        var securityContextMock = new Mock<ISecurityContextAccessor>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();
        var bookingPriceOfPlanServiceMock = new Mock<IBookingPriceService>();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();

        var handler = new BookingCheckNightNumberCommandHandler(
            MockUnitOfWork,
            MockMapper,
            securityContextMock.Object,
            bookingSearchServiceMock.Object,
            bookingCheckAvailableServiceMock.Object,
            bookingPriceOfPlanServiceMock.Object,
            bookingPriceOfRoomServiceMock.Object
        );

        SetupCommonMocks(
            bookingCheckAvailableServiceMock,
            bookingSearchServiceMock,
            securityContextMock,
            planModelMock: null
        );

        var mockAppDatePrices = CreateMockAppDatePrices();
        var expectedBookingPriceResponse = new BookingPriceResponse { AppDatePrices = mockAppDatePrices };
        var bookingPriceRequest = CreateBookingPriceRequest();

        bookingPriceOfPlanServiceMock
            .Setup(
                x => x.GetBookingPrice(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>()
                )
            )
            .Returns(expectedBookingPriceResponse);

        bookingPriceOfRoomServiceMock
            .Setup(
                x => x.GetBookingPrice(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>()
                )
            )
            .Returns(expectedBookingPriceResponse);

        var command = new BookingCheckNightNumberCommand(1, 1) { Payload = bookingPriceRequest };

        // Act & Assert
        await Assert.ThrowsAsync<ReservationNightException>(
            async () =>
            {
                await handler.Handle(command, CancellationToken.None);
            }
        );
    }
}
