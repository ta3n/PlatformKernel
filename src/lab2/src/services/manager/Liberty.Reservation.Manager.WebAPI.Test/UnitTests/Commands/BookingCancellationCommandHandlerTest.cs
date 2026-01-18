using AutoMapper;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Application.Settings;
using Liberty.Reservation.Application.Templates;
using Liberty.Reservation.Application.UseCases.Commands.BookingReservation;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BookingReservation;
using Liberty.Reservation.Manager.WebAPI.Application.Web.ApiService;
using Liberty.Reservation.Manager.WebAPI.Test.InfrastructureOfTest;
using Liberty.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class BookingCancellationCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnBookingId_WhenCancellationSuccessful()
    {
        // Arrange
        var facilityId = 123L;
        var reservationId = 10L;
        var dummyBookingId = 999L;
        var dummyPlan = new Plan();
        var dummyReservation = new Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation
        {
            Id = reservationId,
            CheckInDate = AppDate.GetId(DateTime.Now.AddMonths(3)),
            FacilityId = facilityId,
            SiteId = 200,
            RoomGroupId = 300,
            Plan = dummyPlan,
            UserCode = "User001",
            ReservationState = ReservationStatus.Reserved,
            CancellationStatus = null,
            BookingData = new BookingData
            {
                Facility = new FacilityData(),
                Plan = new PlanData(),
                RoomGroup = new RoomGroupData(),
                Site = new SiteData(),
                TotalRoomPrice = 1000,
                TotalOptionPrice = 1000,
                TotalSpaTax = 1000,
                UsedPoint = 1000
            }
        };

        var payload = new BookingCancellationByManagerRequest(null);
        var command = new BookingCancellationCommand { Payload = payload };

        // Mocks
        var mockLogger = new Mock<ILogger<BookingCancellationCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMediator = new Mock<IMediator>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockBookingDataAvailableService = new Mock<IBookingCheckAvailableService>();
        var mockBookingReservationService = new Mock<IBookingReservationService>();
        var mockMailTemplateService = new Mock<IMailTemplateService>();
        var mockExternalApiService = new Mock<IExternalApiService>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockOptions = new Mock<IOptions<MailTemplateSetting>>();
        var mockSystemConfig = new Mock<IBookingSystemConfigService>();
        var mockBookingModifyChecker = new Mock<IBookingManagerModificationCheckerService>();
        var mockResponse = new BookingSecureUrlResponse(
            true,
            new BookingSecureUrlRequest(
                "1",
                30
            )
        );
        var mockMapper = new Mock<IMapper>();

        var bookingSecureUrlService = new Mock<IBookingSecureUrlService>();

        bookingSecureUrlService
            .Setup(service => service.DecryptAndValidate(It.IsAny<string>()))
            .Returns(mockResponse);

        mockServiceProvider.Setup(s => s.GetService(typeof(IBookingSecureUrlService)))
            .Returns(bookingSecureUrlService.Object);
        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        mockServiceProvider.Setup(s => s.GetService(typeof(IBookingCheckAvailableService)))
            .Returns(mockBookingDataAvailableService.Object);
        mockServiceProvider.Setup(s => s.GetService(typeof(IBookingReservationService)))
            .Returns(mockBookingReservationService.Object);
        mockServiceProvider.Setup(s => s.GetService(typeof(IMailTemplateService)))
            .Returns(mockMailTemplateService.Object);
        mockServiceProvider.Setup(s => s.GetService(typeof(IExternalApiService)))
            .Returns(mockExternalApiService.Object);
        mockServiceProvider.Setup(s => s.GetService(typeof(IOptions<MailTemplateSetting>)))
            .Returns(mockOptions.Object);
        mockServiceProvider.Setup(s => s.GetService(typeof(IBookingSystemConfigService)))
            .Returns(mockSystemConfig.Object);
        mockServiceProvider.Setup(s => s.GetService(typeof(IBookingManagerModificationCheckerService)))
            .Returns(mockBookingModifyChecker.Object);

        mockBookingDataAvailableService.Setup(
                s => s.GetReservationByFacilityAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(dummyReservation);

        var cancellationPrice = 150.00m;
        mockBookingReservationService.Setup(
                s => s.GetCancellationPrice(
                    It.IsAny<DateTime>(),
                    dummyReservation
                )
            )
            .Returns(cancellationPrice);

        mockBookingReservationService
            .Setup(
                s => s.CancelAsync(
                    It.IsAny<ReservationEntity>(),
                    It.IsAny<decimal>(),
                    It.IsAny<float>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new ReservationEntity
                {
                    Id = 999,
                    ReservationState = ReservationStatus.ManagerCanceled,
                    CancellationStatus = CancellationStatus.CancelledRefunded,
                    CheckInDate = AppDate.GetId(DateTime.Now.AddMonths(3)),
                    BookingData = new BookingData
                    {
                        Facility = new FacilityData(),
                        Plan = new PlanData(),
                        RoomGroup = new RoomGroupData(),
                        Site = new SiteData(),
                        TotalRoomPrice = 1000,
                        TotalOptionPrice = 1000,
                        TotalSpaTax = 1000,
                        UsedPoint = 1000
                    }
                }
            );

        var dummyBookingAbortResponse = new BookingAbortResponse(dummyBookingId, 10, DateTime.UtcNow, 10);
        mockMediator.Setup(m => m.Send(It.IsAny<BookingAbortCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dummyBookingAbortResponse);

        mockMailTemplateService.Setup(s => s.FindMailTemplateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TemplateFormatData());

        mockSystemConfig.Setup(s => s.GetSystemConfigAsync(It.IsAny<CancellationToken>()))
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

        mockBookingModifyChecker
            .Setup(
                x => x.CanCancelModify(
                    It.IsAny<long>(),
                    It.IsAny<bool>(),
                    It.IsAny<ReservationStatus>(),
                    It.IsAny<TimeSpan>()
                )
            )
            .Returns(true);

        mockExternalApiService.Setup(
                s => s.PostAsync(
                    It.IsAny<ExternalService>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(([], "dummyContext"));

        // InfrastructureOfTest Options
        var dummyMailTemplateSetting = new MailTemplateSetting { ApplicationName = "TestApp" };
        mockOptions.Setup(o => o.Value).Returns(dummyMailTemplateSetting);

        var handler = new BookingCancellationCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMediator.Object,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockServiceProvider.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(999, result);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowReservationInvalidException_WhenReservationNotReserved()
    {
        // Arrange
        var facilityId = 123L;
        var reservationId = 10L;
        var dummyBookingId = 999L;
        var dummyPlan = new Plan();
        var dummyReservation = new Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation
        {
            Id = reservationId,
            CheckInDate = AppDate.GetId(DateTime.Now.AddMonths(3)),
            FacilityId = facilityId,
            SiteId = 200,
            RoomGroupId = 300,
            Plan = dummyPlan,
            UserCode = "User001",
            ReservationState = ReservationStatus.Temporary,
            CancellationStatus = null
        };

        var payload = new BookingCancellationByManagerRequest(0);
        var command = new BookingCancellationCommand { Payload = payload };

        // Mocks
        var mockLogger = new Mock<ILogger<BookingCancellationCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMediator = new Mock<IMediator>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockBookingDataAvailableService = new Mock<IBookingCheckAvailableService>();
        var mockBookingReservationService = new Mock<IBookingReservationService>();
        var mockMailTemplateService = new Mock<IMailTemplateService>();
        var mockExternalApiService = new Mock<IExternalApiService>();
        var mockOptions = new Mock<IOptions<MailTemplateSetting>>();
        var mockSystemConfig = new Mock<IBookingSystemConfigService>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockMapper = new Mock<IMapper>();

        mockServiceProvider.Setup(s => s.GetService(typeof(IBookingCheckAvailableService)))
            .Returns(mockBookingDataAvailableService.Object);
        mockServiceProvider.Setup(s => s.GetService(typeof(IBookingReservationService)))
            .Returns(mockBookingReservationService.Object);
        mockServiceProvider.Setup(s => s.GetService(typeof(IMailTemplateService)))
            .Returns(mockMailTemplateService.Object);
        mockServiceProvider.Setup(s => s.GetService(typeof(IExternalApiService)))
            .Returns(mockExternalApiService.Object);
        mockServiceProvider.Setup(s => s.GetService(typeof(IOptions<MailTemplateSetting>)))
            .Returns(mockOptions.Object);
        mockServiceProvider.Setup(s => s.GetService(typeof(IBookingSystemConfigService)))
            .Returns(mockSystemConfig.Object);

        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        mockBookingDataAvailableService.Setup(
                s => s.GetReservationByFacilityAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(dummyReservation);

        var cancellationPrice = 150.00m;
        mockBookingReservationService.Setup(
                s => s.GetCancellationPrice(
                    It.IsAny<DateTime>(),
                    dummyReservation
                )
            )
            .Returns(cancellationPrice);

        var dummyBookingAbortResponse = new BookingAbortResponse(dummyBookingId, 10, DateTime.UtcNow, 10);
        mockMediator.Setup(m => m.Send(It.IsAny<BookingAbortCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dummyBookingAbortResponse);

        mockMailTemplateService.Setup(s => s.FindMailTemplateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TemplateFormatData());

        mockSystemConfig.Setup(s => s.GetSystemConfigAsync(It.IsAny<CancellationToken>()))
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
        mockExternalApiService.Setup(
                s => s.PostAsync(
                    It.IsAny<ExternalService>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((new HeaderDictionary(), "dummyContext"));

        // InfrastructureOfTest Options
        var dummyMailTemplateSetting = new MailTemplateSetting { ApplicationName = "TestApp" };
        mockOptions.Setup(o => o.Value).Returns(dummyMailTemplateSetting);

        var handler = new BookingCancellationCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMediator.Object,
            mockMapper.Object,
            mockSecurityContextAccessor.Object,
            mockServiceProvider.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<ReservationInvalidException>(() => handler.Handle(command, CancellationToken.None));
    }
}
