using AutoMapper;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.User.Application.Auth;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Commands.BookingReservation;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest;
using Liberty.UnitOfWork.Abstractions;
using Moq;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.User.WebAPI.Test.UnitTests.Commands;

public class ChangePersonsBookingCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnBookingPrice_WhenCommandIsValid()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var bookingDataAvailableServiceMock = new Mock<IBookingDataAvailableService>();
        var bookingPriceOfPlanServiceMock = new Mock<IBookingPriceService>();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();
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

        var userCode = "test-user-code";
        var dateNow = DateTime.UtcNow.AddHours(DefaultValues.TimeZoneOffset);
        var payload = new ChangePersonsBookingCommand(1)
        {
            Payload = new BookingPriceRequest
            {
                CheckInDate = AppDate.GetId(DateTime.UtcNow.AddDays(1)),
                RestNumber = 3,
                RoomNumber = 3,
                GuestsPerRoom = new List<PersonOfBookingPriceRequest>()
            }
        };

        var facilityId = 1;
        var plan = new Plan
        {
            Id = 1,
            ReceptionLimit = TimeSpan.FromHours(0),
            CancelDayLimit = 10,
            CancelLimit = TimeSpan.FromHours(60),
            IsCancelSameAccept = false
        };

        var reservation = new ReservationEntity
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
            .Setup(x => x.GetReservationByUserCodeAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservation);

        bookingCheckAvailableServiceMock
            .Setup(
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

        var bookingPriceResponse = new BookingPriceResponse
        {
            AppDatePrices = new List<AppDatePriceOfBookingResponse> { new() { Price = 100 } }
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

        var handler = new ChangePersonsBookingCommandHandler(
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
        Assert.NotNull(result);
        Assert.Equal(100, result.AppDatePrices.First().Price);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowReservationInvalidException_WhenReservationCannotBeModified()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var bookingDataAvailableServiceMock = new Mock<IBookingDataAvailableService>();
        var bookingPriceOfPlanServiceMock = new Mock<IBookingPriceService>();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();

        var userCode = "test-user-code";
        var payload = new ChangePersonsBookingCommand(1)
        {
            Payload = new BookingPriceRequest
            {
                CheckInDate = AppDate.GetId(DateTime.UtcNow.AddDays(-1)),
                RestNumber = 3,
                RoomNumber = 3,
                GuestsPerRoom = new List<PersonOfBookingPriceRequest>()
            }
        };

        var reservation = new ReservationEntity
        {
            Id = 1,
            FacilityId = 1,
            Plan = new Plan(),
            SiteId = 1,
            RoomGroupId = 1,
            ReservationState = ReservationStatus.Reserved,
            CheckInDate = AppDate.GetId(DateTime.Now)
        };

        securityContextAccessorMock.Setup(x => x.ApplicationUserKey).Returns(userCode);
        bookingDataAvailableServiceMock
            .Setup(x => x.GetReservationByUserCodeAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservation);

        bookingCheckAvailableServiceMock
            .Setup(
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

        reservation.CheckInDate = AppDate.GetId(DateTime.UtcNow.AddDays(-10));

        var handler = new ChangePersonsBookingCommandHandler(
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
            async () =>
                await handler.Handle(payload, cancellationToken)
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowReservationPriceException_WhenGuestsPerRoomChangeIsInvalid()
    {
        // Arrange
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var bookingDataAvailableServiceMock = new Mock<IBookingDataAvailableService>();
        var bookingPriceOfPlanServiceMock = new Mock<IBookingPriceService>();
        var bookingPriceOfRoomServiceMock = new Mock<IBookingPriceService>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();

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

        var userCode = "test-user-code";
        var payload = new ChangePersonsBookingCommand(1)
        {
            Payload = new BookingPriceRequest
            {
                CheckInDate = AppDate.GetId(DateTime.UtcNow.AddDays(1)),
                RestNumber = 3,
                RoomNumber = 3,
                GuestsPerRoom = new List<PersonOfBookingPriceRequest>()
            }
        };

        var reservation = new ReservationEntity
        {
            Id = 1,
            FacilityId = 1,
            Plan = new Plan(),
            SiteId = 1,
            RoomGroupId = 1,
            ReservationState = ReservationStatus.Reserved,
            CheckInDate = AppDate.GetId(DateTime.Now),
            CheckInTime = new TimeSpan(1, 1, 1, 1)
        };

        securityContextAccessorMock.Setup(x => x.ApplicationUserKey).Returns(userCode);
        bookingDataAvailableServiceMock
            .Setup(x => x.GetReservationByUserCodeAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservation);

        bookingCheckAvailableServiceMock
            .Setup(
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
            .ReturnsAsync(true); // Simulating an invalid change

        var handler = new ChangePersonsBookingCommandHandler(
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
            async () =>
                await handler.Handle(payload, cancellationToken)
        );
    }
}
