using Liberty.Cache.Services;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Settings;
using Liberty.Reservation.User.Application.Auth;
using Liberty.Reservation.User.Application.Domains.Services.Interfaces;
using Liberty.Reservation.User.WebAPI.Application.BackgroundServices;
using Liberty.Reservation.User.WebAPI.Application.Models.Requests;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Commands.BookingReservation;
using Liberty.Reservation.User.WebAPI.Application.Web.ApiService;
using Liberty.Reservation.User.WebAPI.Test.InfrastructureOfTest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.User.WebAPI.Test.UnitTests.Commands;

public class BookingConfirmCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldConfirmReservationSuccessfully_WhenReservationIsValid()
    {
        // Arrange
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();
        var bookingReservationServiceMock = new Mock<IBookingReservationService>();
        var externalApiServiceMock = new Mock<IExternalApiService>();
        var integrationEventOutboxServiceMock = new Mock<IIntegrationEventOutboxService>();
        var mailTemplateServiceMock = new Mock<IMailTemplateService>();
        var mailTemplateSettingMock = new Mock<IOptions<MailTemplateSetting>>();
        var cacheServiceMock = new Mock<ICacheService>();
        var bookingHoldManagementServiceMock = new Mock<IBookingHoldManagementService>();
        var bookingConfirmSendEmailBackgroundService = new Mock<BookingConfirmSendEmailBackgroundService>(
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<BookingConfirmSendEmailBackgroundService>>()
        );
        var bookingOptionInventoryServiceMock = new Mock<IBookingOptionInventoryService>();

        bookingHoldManagementServiceMock
            .Setup(
                x => x.TryHoldRoomAsync(
                    It.IsAny<BookingHoldCheckModel>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);

        var fakeReservation = new ReservationBasicModel(
            1,
            "Code 1",
            "",
            100,
            200,
            300,
            400,
            20250717,
            2,
            1,
            "ja",
            true,
            ReservationStatus.Reserved,
            []
        );

        bookingCheckAvailableServiceMock
            .Setup(x => x.GetReservationBasicByUserAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeReservation);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(x => x.GetService(typeof(ISecurityContextAccessor)))
            .Returns(securityContextAccessorMock.Object);
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IBookingOptionInventoryService)))
            .Returns(bookingOptionInventoryServiceMock.Object);
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IBookingReservationService)))
            .Returns(bookingReservationServiceMock.Object);
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IExternalApiService)))
            .Returns(externalApiServiceMock.Object);
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IIntegrationEventOutboxService)))
            .Returns(integrationEventOutboxServiceMock.Object);
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IMailTemplateService)))
            .Returns(mailTemplateServiceMock.Object);
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IOptions<MailTemplateSetting>)))
            .Returns(mailTemplateSettingMock.Object);
        serviceProviderMock
            .Setup(x => x.GetService(typeof(ICacheService)))
            .Returns(cacheServiceMock.Object);
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IBookingHoldManagementService)))
            .Returns(bookingHoldManagementServiceMock.Object);
        serviceProviderMock
            .Setup(x => x.GetService(typeof(BookingConfirmSendEmailBackgroundService)))
            .Returns(bookingConfirmSendEmailBackgroundService.Object);

        var userCode = "test-user-code";
        var payload = new BookingConfirmRequest { Id = 1 };
        var command = new BookingConfirmCommand { Payload = payload };

        var existingReservation = new ReservationEntity
        {
            Id = 1,
            ReservationState = ReservationStatus.Temporary,
            Reserver = new() { EMail = "test@guest.com" }
        };

        securityContextAccessorMock.Setup(x => x.ApplicationUserKey).Returns(userCode);

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
            .ReturnsAsync(true);

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

        serviceProviderMock
            .Setup(x => x.GetService(typeof(IBookingCheckAvailableService)))
            .Returns(bookingCheckAvailableServiceMock.Object);

        bookingReservationServiceMock
            .Setup(x => x.ConfirmedAsync(It.IsAny<ReservationEntity>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingReservation);

        var handler = new BookingConfirmCommandHandler(
            new Mock<ILogger<BookingConfirmCommandHandler>>().Object,
            MockUnitOfWork,
            MockMapper,
            MockMediator,
            serviceProviderMock.Object
        );

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await handler.Handle(command, cancellationToken);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnReservationInvalidException()
    {
        // Arrange
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();
        var bookingReservationServiceMock = new Mock<IBookingReservationService>();
        var externalApiServiceMock = new Mock<IExternalApiService>();
        var integrationEventOutboxServiceMock = new Mock<IIntegrationEventOutboxService>();
        var mailTemplateServiceMock = new Mock<IMailTemplateService>();
        var mailTemplateSettingMock = new Mock<IOptions<MailTemplateSetting>>();
        var cacheServiceMock = new Mock<ICacheService>();
        var bookingHoldManagementServiceMock = new Mock<IBookingHoldManagementService>();
        var bookingOptionInventoryServiceMock = new Mock<IBookingOptionInventoryService>();

        var bookingConfirmSendEmailBackgroundService = new Mock<BookingConfirmSendEmailBackgroundService>(
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<BookingConfirmSendEmailBackgroundService>>()
        );
        var fakeReservation = new ReservationBasicModel(
            1,
            "Code 1",
            "",
            100,
            200,
            300,
            400,
            20250717,
            2,
            1,
            "ja",
            false,
            ReservationStatus.Reserved,
            []
        );

        bookingCheckAvailableServiceMock
            .Setup(x => x.GetReservationBasicByUserAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeReservation);

        bookingHoldManagementServiceMock
            .Setup(x => x.TryHoldRoomAsync(It.IsAny<BookingHoldCheckModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(x => x.GetService(typeof(ISecurityContextAccessor)))
            .Returns(securityContextAccessorMock.Object);
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IBookingOptionInventoryService)))
            .Returns(bookingOptionInventoryServiceMock.Object);
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IBookingCheckAvailableService)))
            .Returns(bookingCheckAvailableServiceMock.Object);
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IBookingReservationService)))
            .Returns(bookingReservationServiceMock.Object);
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IExternalApiService)))
            .Returns(externalApiServiceMock.Object);
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IIntegrationEventOutboxService)))
            .Returns(integrationEventOutboxServiceMock.Object);
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IMailTemplateService)))
            .Returns(mailTemplateServiceMock.Object);
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IOptions<MailTemplateSetting>)))
            .Returns(mailTemplateSettingMock.Object);
        serviceProviderMock
            .Setup(x => x.GetService(typeof(ICacheService)))
            .Returns(cacheServiceMock.Object);
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IBookingHoldManagementService)))
            .Returns(bookingHoldManagementServiceMock.Object);
        serviceProviderMock
            .Setup(x => x.GetService(typeof(BookingConfirmSendEmailBackgroundService)))
            .Returns(bookingConfirmSendEmailBackgroundService.Object);

        var userCode = "test-user-code";
        var payload = new BookingConfirmRequest { Id = 1 };
        var command = new BookingConfirmCommand { Payload = payload };

        var existingReservation = new ReservationEntity
        {
            Id = 1,
            ReservationState = ReservationStatus.Reserved,
            Reserver = new() { EMail = "test@guest.com" }
        };

        securityContextAccessorMock.Setup(x => x.ApplicationUserKey).Returns(userCode);

        bookingReservationServiceMock
            .Setup(x => x.ConfirmedAsync(It.IsAny<ReservationEntity>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingReservation);

        var handler = new BookingConfirmCommandHandler(
            new Mock<ILogger<BookingConfirmCommandHandler>>().Object,
            MockUnitOfWork,
            MockMapper,
            MockMediator,
            serviceProviderMock.Object
        );

        var cancellationToken = CancellationToken.None;

        await Assert.ThrowsAsync<GuestHasAlreadyConfirmedBookingException>(
            async () => await handler.Handle(command, cancellationToken)
        );
    }
}
