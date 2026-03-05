using Moq;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Commands.BookingReservation;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.User.Application.Auth;
using Liberty.UnitOfWork.Abstractions;
using AutoMapper;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.User.WebAPI.Test.UnitTests.Commands;

public class BookingCheckRoomNumberCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnBookingPrice_WhenCommandIsValid()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var bookingDataAvailableServiceMock = new Mock<IBookingDataAvailableService>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();
        var bookingPriceOfPlanServiceMock = new Mock<IBookingPriceService>();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();

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
            new BookingCancellationPolicyModel()
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

        // InfrastructureOfTest mock methods
        var userCode = "test-user-code";
        var dateNow = DateTime.UtcNow.AddHours(DefaultValues.TimeZoneOffset);
        var payload = new BookingCheckRoomNumberCommand(1)
        {
            Payload = new BookingPriceRequest
            {
                CheckInDate = AppDate.GetId(DateTime.UtcNow.AddDays(1)),
                RoomNumber = 3,
                RestNumber = 3,
                GuestsPerRoom = new()
            }
        };

        var facilityId = 1;
        var plan = new Plan
        {
            Id = 1,
            ReceptionLimit = TimeSpan.FromHours(0),
            CancelLimit = TimeSpan.FromHours(60),
            IsCancelSameAccept = false
        };

        var existingReservation = new ReservationEntity
        {
            Id = 1,
            FacilityId = facilityId,
            Plan = plan,
            SiteId = 1,
            RoomGroupId = 1,
            ReservationState = ReservationStatus.Reserved,
            CheckInDate = AppDate.GetId(dateNow.AddDays(1)),
            CheckInTime = dateNow.AddHours(2).TimeOfDay
        };

        securityContextAccessorMock.Setup(x => x.ApplicationUserKey).Returns(userCode);
        bookingDataAvailableServiceMock
            .Setup(
                x => x.GetReservationByUserCodeAsync(
                    It.IsAny<long>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(existingReservation);

        bookingCheckAvailableServiceMock
            .Setup(
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

        var appDatePrice1 = new AppDatePriceOfBookingResponse
        {
            AppDateId = 1,
            Price = 100,
            TotalSpaTax = 20,
            TotalOptionPrice = 10
        };

        var appDatePrice2 = new AppDatePriceOfBookingResponse
        {
            AppDateId = 2,
            Price = 150,
            TotalSpaTax = 30,
            TotalOptionPrice = 15
        };

        var bookingPriceResponse = new BookingPriceResponse
        {
            AppDatePrices = new List<AppDatePriceOfBookingResponse>
            {
                appDatePrice1,
                appDatePrice2
            }
        };

        bookingPriceOfPlanServiceMock
            .Setup(
                x => x.GetBookingPrice(
                    It.IsAny<BookingPriceRequest>(),
                    It.IsAny<BookingPlanModel>(),
                    It.IsAny<long>()
                )
            )
            .Returns(bookingPriceResponse);

        var handler = new BookingCheckRoomNumberCommandHandler(
            unitOfWorkMock.Object,
            mapperMock.Object,
            securityContextAccessorMock.Object,
            bookingDataAvailableServiceMock.Object,
            bookingCheckAvailableServiceMock.Object,
            bookingSearchServiceMock.Object,
            bookingPriceOfPlanServiceMock.Object,
            bookingPriceOfRoomServiceMock.Object
        );

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(payload, cancellationToken);

        // Assert
        Assert.Equal(325, result.TotalPrice); // Verify the correct total price
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowReservationInvalidException_WhenRoomNumberIsInvalid()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var bookingDataAvailableServiceMock = new Mock<IBookingDataAvailableService>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();
        var bookingPriceOfPlanServiceMock = new Mock<IBookingPriceService>();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();

        // InfrastructureOfTest mock methods
        var userCode = "test-user-code";
        var payload = new BookingCheckRoomNumberCommand(1)
        {
            Payload = new BookingPriceRequest
            {
                CheckInDate = AppDate.GetId(DateTime.UtcNow.AddDays(1)),
                RoomNumber = 3,
                RestNumber = 3,
                GuestsPerRoom = new()
            }
        };

        var facilityId = 1;
        var plan = new Plan
        {
            Id = 1,
            ReceptionDayLimit = 2,
            ReceptionLimit = TimeSpan.FromHours(30),
            CancelDayLimit = 1,
            CancelLimit = TimeSpan.FromHours(60),
            IsCancelSameAccept = false
        };

        var existingReservation = new ReservationEntity
        {
            Id = 1,
            FacilityId = facilityId,
            Plan = plan,
            SiteId = 1,
            RoomGroupId = 1,
            ReservationState = ReservationStatus.Reserved,
            CheckInDate = AppDate.GetId(DateTime.Now.AddHours(1)),
            CheckInTime = new TimeSpan(1, 1, 1, 1)
        };

        securityContextAccessorMock.Setup(x => x.ApplicationUserKey).Returns(userCode);
        bookingDataAvailableServiceMock
            .Setup(
                x => x.GetReservationByUserCodeAsync(
                    It.IsAny<long>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(existingReservation);

        bookingCheckAvailableServiceMock
            .Setup(
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

        var handler = new BookingCheckRoomNumberCommandHandler(
            unitOfWorkMock.Object,
            mapperMock.Object,
            securityContextAccessorMock.Object,
            bookingDataAvailableServiceMock.Object,
            bookingCheckAvailableServiceMock.Object,
            bookingSearchServiceMock.Object,
            bookingPriceOfPlanServiceMock.Object,
            bookingPriceOfRoomServiceMock.Object
        );

        var cancellationToken = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<ReservationPriceException>(
            async () => await handler.Handle(payload, cancellationToken)
        );
    }
}
