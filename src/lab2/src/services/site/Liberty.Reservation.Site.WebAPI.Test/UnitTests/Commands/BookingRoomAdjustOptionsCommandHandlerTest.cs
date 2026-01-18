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
using Liberty.Reservation.Site.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Commands.Booking;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using Moq;

namespace Liberty.Reservation.Site.WebAPI.Test.UnitTests.Commands;

public class BookingRoomAdjustOptionsCommandHandlerTest : BaseUnitTest
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
        var checkInDate = DateTime.Now;
        List<PersonOfBookingPriceRequest> guestsPerRoom =
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
        ];

        return new BookingPriceRequest
        {
            RestNumber = 1,
            RoomNumber = 1,
            CheckInDate = AppDate.GetId(checkInDate),
            GuestsPerRoom = guestsPerRoom,
            OptionItems = null
        };
    }

    private static BookingPlanModel GetBookingPlanModel()
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
        Mock<ISecurityContextAccessor> securityContextMock,
        Mock<IFacilityExternalRepository> facilityExternalRepositoryMock,
        Mock<IBookingSearchService> bookingSearchServiceMock,
        bool facilityAvailable = true,
        BookingPlanModel? planModelMock = null
    )
    {
        securityContextMock
            .Setup(x => x.GetFacilityCodeSelected())
            .Returns("fake-code");

        securityContextMock
            .Setup(x => x.GetFacilityIdSelected())
            .Returns(1);

        securityContextMock
            .Setup(x => x.GetSiteIdSelected())
            .Returns(1);

        facilityExternalRepositoryMock
            .Setup(x => x.CheckFacilityAvailableAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(facilityAvailable);

        bookingSearchServiceMock
            .Setup(
                x => x.GetBookingDataDetailByPlanAsync(
                    It.IsAny<BookingPlanDetailRequest>(),
                    It.IsAny<BookingSearchPlanRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(planModelMock);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBookingPriceResponse_WhenAllValid()
    {
        // Arrange
        var facilityExternalRepositoryMock = new Mock<IFacilityExternalRepository>();
        var securityContextMock = new Mock<ISecurityContextAccessor>();
        var bookingPriceOfPlanServiceMock = new Mock<IBookingPriceService>();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();

        var handler = new AdjustOptionsCommandHandler(
            MockUnitOfWork,
            MockMapper,
            securityContextMock.Object,
            facilityExternalRepositoryMock.Object,
            bookingSearchServiceMock.Object,
            bookingCheckAvailableServiceMock.Object,
            bookingPriceOfPlanServiceMock.Object,
            bookingPriceOfRoomServiceMock.Object
        );

        SetupCommonMocks(
            securityContextMock,
            facilityExternalRepositoryMock,
            bookingSearchServiceMock,
            planModelMock: GetBookingPlanModel()
        );

        var mockAppDatePrices = CreateMockAppDatePrices();
        var expectedBookingPriceResponse = new BookingPriceResponse { AppDatePrices = mockAppDatePrices };
        var payload = CreateBookingPriceRequest();

        bookingCheckAvailableServiceMock
            .Setup(
                x => x.IsInvalidOptionItemsAsync(
                    It.IsAny<BookingPriceRequest>(),
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

        bookingPriceOfRoomServiceMock
            .Setup(
                x => x.GetBookingPrice(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>()
                )
            )
            .Returns(expectedBookingPriceResponse);

        var command = new AdjustOptionsCommand(1, 2) { Payload = payload };
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BookingPriceResponse>(result);
        Assert.Equal(mockAppDatePrices.Count, result.AppDatePrices.Count());
        Assert.Equal(3000, result.TotalRoomPrice);
        Assert.Equal(150, result.TotalSpaTax);
        Assert.Equal(350, result.TotalOptionPrice);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnReservationInvalidException()
    {
        // Arrange
        var facilityExternalRepositoryMock = new Mock<IFacilityExternalRepository>();
        var securityContextMock = new Mock<ISecurityContextAccessor>();
        var bookingPriceOfPlanServiceMock = new Mock<IBookingPriceService>();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();

        var handler = new AdjustOptionsCommandHandler(
            MockUnitOfWork,
            MockMapper,
            securityContextMock.Object,
            facilityExternalRepositoryMock.Object,
            bookingSearchServiceMock.Object,
            bookingCheckAvailableServiceMock.Object,
            bookingPriceOfPlanServiceMock.Object,
            bookingPriceOfRoomServiceMock.Object
        );

        SetupCommonMocks(
            securityContextMock,
            facilityExternalRepositoryMock,
            bookingSearchServiceMock,
            planModelMock: GetBookingPlanModel()
        );

        var mockAppDatePrices = CreateMockAppDatePrices();
        var expectedBookingPriceResponse = new BookingPriceResponse { AppDatePrices = mockAppDatePrices };
        var payload = CreateBookingPriceRequest();

        bookingCheckAvailableServiceMock
            .Setup(
                x => x.IsInvalidOptionItemsAsync(
                    It.IsAny<BookingPriceRequest>(),
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

        bookingPriceOfRoomServiceMock
            .Setup(
                x => x.GetBookingPrice(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>()
                )
            )
            .Returns(expectedBookingPriceResponse);

        var command = new AdjustOptionsCommand(1, 2) { Payload = payload };
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<ReservationOptionException>(async () => await handler.Handle(command, cancellationToken));
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPlanNotFoundException()
    {
        // Arrange
        var facilityExternalRepositoryMock = new Mock<IFacilityExternalRepository>();
        var securityContextMock = new Mock<ISecurityContextAccessor>();
        var bookingPriceOfPlanServiceMock = new Mock<IBookingPriceService>();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();

        var handler = new AdjustOptionsCommandHandler(
            MockUnitOfWork,
            MockMapper,
            securityContextMock.Object,
            facilityExternalRepositoryMock.Object,
            bookingSearchServiceMock.Object,
            bookingCheckAvailableServiceMock.Object,
            bookingPriceOfPlanServiceMock.Object,
            bookingPriceOfRoomServiceMock.Object
        );

        SetupCommonMocks(
            securityContextMock,
            facilityExternalRepositoryMock,
            bookingSearchServiceMock,
            planModelMock: null
        );

        var mockAppDatePrices = CreateMockAppDatePrices();
        var expectedBookingPriceResponse = new BookingPriceResponse { AppDatePrices = mockAppDatePrices };
        var payload = CreateBookingPriceRequest();

        bookingCheckAvailableServiceMock
            .Setup(
                x => x.IsInvalidOptionItemsAsync(
                    It.IsAny<BookingPriceRequest>(),
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

        bookingPriceOfRoomServiceMock
            .Setup(
                x => x.GetBookingPrice(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>()
                )
            )
            .Returns(expectedBookingPriceResponse);

        var command = new AdjustOptionsCommand(1, 2) { Payload = payload };
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<PlanNotfoundException>(
            async () => await handler.Handle(command, cancellationToken)
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFacilityNotFoundException()
    {
        // Arrange
        var facilityExternalRepositoryMock = new Mock<IFacilityExternalRepository>();
        var securityContextMock = new Mock<ISecurityContextAccessor>();
        var bookingPriceOfPlanServiceMock = new Mock<IBookingPriceService>();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();

        var handler = new AdjustOptionsCommandHandler(
            MockUnitOfWork,
            MockMapper,
            securityContextMock.Object,
            facilityExternalRepositoryMock.Object,
            bookingSearchServiceMock.Object,
            bookingCheckAvailableServiceMock.Object,
            bookingPriceOfPlanServiceMock.Object,
            bookingPriceOfRoomServiceMock.Object
        );

        SetupCommonMocks(
            securityContextMock,
            facilityExternalRepositoryMock,
            bookingSearchServiceMock,
            false,
            GetBookingPlanModel()
        );

        var mockAppDatePrices = CreateMockAppDatePrices();
        var expectedBookingPriceResponse = new BookingPriceResponse { AppDatePrices = mockAppDatePrices };
        var payload = CreateBookingPriceRequest();

        bookingCheckAvailableServiceMock
            .Setup(
                x => x.IsInvalidOptionItemsAsync(
                    It.IsAny<BookingPriceRequest>(),
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

        bookingPriceOfRoomServiceMock
            .Setup(
                x => x.GetBookingPrice(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>()
                )
            )
            .Returns(expectedBookingPriceResponse);

        var command = new AdjustOptionsCommand(1, 2) { Payload = payload };
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<FacilityNotfoundException>(async () => await handler.Handle(command, cancellationToken));
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnZeroPrices_WhenNoRoomPriceFound()
    {
        // Arrange
        var facilityExternalRepositoryMock = new Mock<IFacilityExternalRepository>();
        var securityContextMock = new Mock<ISecurityContextAccessor>();
        var bookingPriceOfPlanServiceMock = new Mock<IBookingPriceService>();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();

        var handler = new AdjustOptionsCommandHandler(
            MockUnitOfWork,
            MockMapper,
            securityContextMock.Object,
            facilityExternalRepositoryMock.Object,
            bookingSearchServiceMock.Object,
            bookingCheckAvailableServiceMock.Object,
            bookingPriceOfPlanServiceMock.Object,
            bookingPriceOfRoomServiceMock.Object
        );

        SetupCommonMocks(
            securityContextMock,
            facilityExternalRepositoryMock,
            bookingSearchServiceMock,
            planModelMock: GetBookingPlanModel()
        );

        var payload = CreateBookingPriceRequest();

        bookingCheckAvailableServiceMock
            .Setup(
                x => x.IsInvalidOptionItemsAsync(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(false);

        bookingPriceOfPlanServiceMock
            .Setup(
                x => x.GetBookingPrice(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>()
                )
            )
            .Returns(new BookingPriceResponse());

        bookingPriceOfRoomServiceMock
            .Setup(
                x => x.GetBookingPrice(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>()
                )
            )
            .Returns(new BookingPriceResponse());

        var command = new AdjustOptionsCommand(1, 2) { Payload = payload };
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(command, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BookingPriceResponse>(result);
        Assert.Equal(0, result.TotalRoomPrice);
        Assert.Equal(0, result.TotalSpaTax);
        Assert.Equal(0, result.TotalOptionPrice);
    }
}
