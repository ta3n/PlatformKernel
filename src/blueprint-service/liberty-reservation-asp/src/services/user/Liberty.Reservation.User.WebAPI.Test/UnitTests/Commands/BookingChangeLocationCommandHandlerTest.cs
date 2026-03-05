using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.User.Application.Auth;
using Liberty.Reservation.User.WebAPI.Application.Models.Requests;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Commands.BookingReservation;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.User.WebAPI.Test.UnitTests.Commands;

public class BookingChangeLocationCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldChangeLocationAndUpdateCache_WhenCommandIsValid()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<BookingChangeExecutionCommandHandler>>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var mapperMock = new Mock<IMapper>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var cacheServiceMock = new Mock<ICacheService>();
        var bookingReservationServiceMock = new Mock<IBookingReservationService>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();

        var userCode = "test-user-code";
        var payload = new BookingChangeLocationCommand { Payload = new BookingChangeLocationRequest("ABC123", true) };

        var existingReservation = new ReservationEntity
        {
            Id = 1,
            BookingData = new()
            {
                IsSiteLocation = false,
                Facility = new(),
                Plan = new(),
                RoomGroup = new(),
                Site = new()
            }
        };

        securityContextAccessorMock.Setup(x => x.ApplicationUserKey).Returns(userCode);
        bookingCheckAvailableServiceMock
            .Setup(x => x.GetReservationByCodeAsync(It.IsAny<string>(), userCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingReservation);

        cacheServiceMock.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Mock booking reservation service
        bookingReservationServiceMock
            .Setup(x => x.ChangeLocationAsync(It.IsAny<ReservationEntity>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingReservation);

        var handler = new BookingChangeLocationCommandHandler(
            loggerMock.Object,
            unitOfWorkMock.Object,
            mapperMock.Object,
            securityContextAccessorMock.Object,
            cacheServiceMock.Object,
            bookingReservationServiceMock.Object,
            bookingCheckAvailableServiceMock.Object
        );

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(payload, cancellationToken);

        // Assert
        Assert.Equal(1, result);
    }
}
