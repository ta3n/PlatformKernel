using Liberty.ApplicationShared.Utils;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Application.UseCases.Commands.BookingReservation;
using Liberty.Reservation.User.Application.Auth;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Commands.BookingReservation;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest.Utilities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.User.WebAPI.Test.UnitTests.Commands;

public class BookingCancellationCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldProcessCancellationSuccessfully_WhenReservationIsFound()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();
        var bookingReservationServiceMock = new Mock<IBookingReservationService>();
        var bookingSystemConfigServiceMock = new Mock<IBookingSystemConfigService>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var userCode = "test-user-code";
        var payload = new BookingCancellationRequest { Id = 1 };
        var command = new BookingCancellationCommand { Payload = payload };

        var reservation = new ReservationEntity
        {
            Id = 1,
            ReservationState = ReservationStatus.Reserved,
            PaymentType = PaymentTypes.Unknown,
            CheckInDate = AppDate.GetId(DateTime.UtcNow.AddDays(2)),
            RestNumber = 1,
            Plan = new Plan
            {
                Id = 1,
                IsEnabled = true,
                Name = new MultilingualText { { TestUtil.DefaultLanguageCode, "Test Plan" } },
                Code = EntityUtil.CreateCode()
            }
        };

        mockServiceProvider
            .Setup(s => s.GetService(typeof(IBookingCheckAvailableService)))
            .Returns(bookingCheckAvailableServiceMock.Object);
        mockServiceProvider
            .Setup(s => s.GetService(typeof(IBookingReservationService)))
            .Returns(bookingReservationServiceMock.Object);
        mockServiceProvider
            .Setup(s => s.GetService(typeof(IBookingSystemConfigService)))
            .Returns(bookingSystemConfigServiceMock.Object);
        securityContextAccessorMock
            .Setup(x => x.ApplicationUserKey)
            .Returns(userCode);
        bookingCheckAvailableServiceMock
            .Setup(x => x.GetReservationByUserAsync(It.IsAny<long>(), userCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservation);
        bookingReservationServiceMock
            .Setup(x => x.GetCancellationPrice(It.IsAny<DateTime>(), It.IsAny<ReservationEntity>()))
            .Returns(100);
        mediatorMock
            .Setup(x => x.Send(It.IsAny<ReservationGetCancellationFeeQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => (new HeaderDictionary(), new BookingCancellationFeeResponse(1, 1, 1, 1, true)));
        mediatorMock
            .Setup(x => x.Send(It.IsAny<BookingAbortCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BookingAbortResponse(1, 100, DateTime.UtcNow, 10));
        bookingSystemConfigServiceMock
            .Setup(x => x.GetSystemConfigAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new SystemConfig
                {
                    TemplateFormatData = null,
                    IsEnabled = true,
                    Code = "Test",
                    Id = 1,
                    CanOnlinePayment = true
                }
            );

        var handler = new BookingCancellationCommandHandler(
            new Mock<ILogger<BookingCancellationCommandHandler>>().Object,
            MockUnitOfWork,
            mediatorMock.Object,
            securityContextAccessorMock.Object,
            mockServiceProvider.Object
        );

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(command, cancellationToken);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowReservationInvalidException_WhenReservationIsNotFound()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();
        var bookingSystemConfigServiceMock = new Mock<IBookingSystemConfigService>();
        var mockServiceProvider = new Mock<IServiceProvider>();

        var reservation = new ReservationEntity
        {
            Id = 1,
            ReservationState = ReservationStatus.UserCanceled,
            PaymentType = PaymentTypes.Unknown
        };

        mockServiceProvider
            .Setup(s => s.GetService(typeof(IBookingCheckAvailableService)))
            .Returns(bookingCheckAvailableServiceMock.Object);
        bookingCheckAvailableServiceMock
            .Setup(x => x.GetReservationByUserAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservation);
        mockServiceProvider
            .Setup(s => s.GetService(typeof(IBookingSystemConfigService)))
            .Returns(bookingSystemConfigServiceMock.Object);
        bookingSystemConfigServiceMock
            .Setup(x => x.GetSystemConfigAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new SystemConfig
                {
                    TemplateFormatData = null,
                    IsEnabled = true,
                    Code = "Test",
                    Id = 1,
                    CanOnlinePayment = true
                }
            );

        var payload = new BookingCancellationRequest { Id = 1 };
        var command = new BookingCancellationCommand { Payload = payload };

        var handler = new BookingCancellationCommandHandler(
            new Mock<ILogger<BookingCancellationCommandHandler>>().Object,
            MockUnitOfWork,
            mediatorMock.Object,
            securityContextAccessorMock.Object,
            mockServiceProvider.Object
        );

        var cancellationToken = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<ReservationInvalidException>(
            async () => await handler.Handle(command, cancellationToken)
        );
    }
}
