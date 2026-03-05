using Liberty.GmoPaymentGateway.Exceptions;
using Liberty.GmoPaymentGateway.Models.Requests;
using Liberty.GmoPaymentGateway.Models.Responses;
using Liberty.GmoPaymentGateway.Services;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.UseCases.Commands.BookingReservation;
using Liberty.Reservation.Manager.Application.Auth;
using Liberty.Reservation.Manager.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BookingReservation;
using Liberty.UnitOfWork.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Manager.WebAPI.Test.UnitTests.Commands;

public class BookingChangeExecutionCommandHandlerTest
{
    [Fact]
    public async Task HandleAsync_SuccessfulChangeWithPriceModification_ReturnsBookingId()
    {
        // Arrange
        var facilityId = 123L;
        var existingReservationId = 10L;
        var newReservationId = 20L;
        var orderId = "ORDER123";
        var existingReservation = new ReservationEntity
        {
            Id = existingReservationId,
            FacilityId = facilityId,
            ReservationState = ReservationStatus.Reserved,
            UserCode = "USER001",
            ReservationDateTime = DateTime.UtcNow,
            CheckInDate = AppDate.GetId(DateTime.Now.AddDays(2)),
            RestNumber = 2
        };
        var checkInDate = DateTime.Now;
        var newReservation = new ReservationEntity { Id = newReservationId };
        var payload = new BookingAdjustRequest(
            true,
            "14:00",
            2,
            2,
            "Free input",
            new ReserverOfReservationAdjustRequest(
                "Reserver full name",
                "Reserver kana",
                Genders.Male,
                "test@liberty.com",
                "014574",
                "Country",
                "Reserver address 1",
                "Reserver address 2",
                "Reserver address 3",
                "0214155455"
            ),
            new GuestOfReservationAdjustRequest(
                "Main user full name",
                "Main user kana",
                Genders.Male,
                AppDate.GetId(
                    new(
                        1993,
                        1,
                        1,
                        0,
                        0,
                        0,
                        DateTimeKind.Local
                    )
                ),
                "014574",
                "Country",
                "Main user address 1",
                "Main user address 2",
                "Main user address 3",
                "0214155455"
            ),
            [
                new NightPeopleOfReservationAdjustRequest(
                    AppDate.GetId(checkInDate),
                    [
                        new RoomNightOfReservationAdjustRequest(
                            0,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        ),
                        new RoomNightOfReservationAdjustRequest(
                            1,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        )
                    ]
                ),
                new NightPeopleOfReservationAdjustRequest(
                    AppDate.GetId(checkInDate.AddDays(1)),
                    [
                        new RoomNightOfReservationAdjustRequest(
                            0,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        ),
                        new RoomNightOfReservationAdjustRequest(
                            1,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        )
                    ]
                )
            ],
            null,
            [
                new RoomRepresentativeOfReservationAdjustRequest(
                    0,
                    "Full name 1",
                    "Kana 1"
                ),
                new RoomRepresentativeOfReservationAdjustRequest(
                    1,
                    "Full name 2",
                    "Kana 2"
                )
            ],
            null,
            null
        ) { CheckInDateId = AppDate.GetId(checkInDate) };
        var command = new BookingChangeExecutionCommand { Payload = payload };

        var mockLogger = new Mock<ILogger<BookingChangeExecutionCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMediator = new Mock<IMediator>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockServiceProvider = new Mock<IServiceProvider>();

        var mockBookingCheckAvailableService = new Mock<IBookingCheckAvailableService>();
        var mockBookingCheckModifyInPriceService = new Mock<IBookingCheckModifyInPriceService>();
        var mockGmoPaymentGatewayService = new Mock<IGmoPaymentGatewayService>();
        var mockBookingReservationService = new Mock<IBookingReservationService>();
        var mockSystemConfig = new Mock<IBookingSystemConfigService>();
        var mockBookingModifyChecker = new Mock<IBookingManagerModificationCheckerService>();
        var mockReservationPaymentRestrictionService = new Mock<IBookingPaymentRestrictionService>();
        var mockFacilityService = new Mock<IFacilityService>();

        mockBookingCheckAvailableService.Setup(
                s => s.GetReservationByFacilityAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(existingReservation);
        mockBookingCheckModifyInPriceService.Setup(
                s => s.IsModifyInPriceAsync(
                    It.IsAny<ReservationEntity>(),
                    payload,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);
        mockBookingReservationService.Setup(s => s.FindOderIdOfOnlinePaymentAsync(existingReservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderId);

        mockGmoPaymentGatewayService.Setup(s => s.SearchTradeAsync(It.IsAny<SearchTradeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OnLinePaymentSearchTradeResponse { Amount = "10000" });
        mockGmoPaymentGatewayService.Setup(s => s.CancelAsync(It.IsAny<CancelOrderRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OnLinePaymentCancelResponse());
        mockReservationPaymentRestrictionService.Setup(
                s => s.ValidateGlobalOnlinePaymentAsync(It.IsAny<ReservationEntity>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);

        mockFacilityService.Setup(
                s => s.CheckPaymentOnSitePaymentAvailableAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(true);

        mockMediator.Setup(m => m.Send(It.IsAny<BookingAdjustCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newReservation);
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
            .Setup(x => x.CanBookingChangeModify(It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<ReservationStatus>()))
            .Returns(true);

        mockServiceProvider
            .Setup(sp => sp.GetService(It.IsAny<Type>()))
            .Returns(
                (
                    Type t
                ) => t switch
                {
                    not null when t == typeof(IBookingCheckAvailableService) => mockBookingCheckAvailableService.Object,
                    not null when t == typeof(IBookingCheckModifyInPriceService) => mockBookingCheckModifyInPriceService.Object,
                    not null when t == typeof(IGmoPaymentGatewayService) => mockGmoPaymentGatewayService.Object,
                    not null when t == typeof(IBookingReservationService) => mockBookingReservationService.Object,
                    not null when t == typeof(IBookingSystemConfigService) => mockSystemConfig.Object,
                    not null when t == typeof(IBookingManagerModificationCheckerService) => mockBookingModifyChecker.Object,
                    not null when t == typeof(IBookingPaymentRestrictionService) => mockReservationPaymentRestrictionService.Object,
                    not null when t == typeof(IFacilityService) => mockFacilityService.Object,
                    _ => null
                }
            );

        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        mockUnitOfWork.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new BookingChangeExecutionCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMediator.Object,
            mockSecurityContextAccessor.Object,
            mockServiceProvider.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(newReservationId, result);
    }

    [Fact]
    public async Task HandleAsync_SuccessfulChangeWithPriceModification_ReturnsBookingId_Case2()
    {
        // Arrange
        var facilityId = 123L;
        var existingReservationId = 10L;
        var orderId = "ORDER123";
        var existingReservation = new ReservationEntity
        {
            Id = existingReservationId,
            FacilityId = facilityId,
            ReservationState = ReservationStatus.Reserved,
            UserCode = "USER001",
            ReservationDateTime = DateTime.UtcNow,
            CheckInDate = AppDate.GetId(DateTime.Now.AddDays(2)),
            RestNumber = 2
        };
        var checkInDate = DateTime.Now;
        var payload = new BookingAdjustRequest(
            true,
            "14:00",
            2,
            2,
            "Free input",
            new ReserverOfReservationAdjustRequest(
                "Reserver full name",
                "Reserver kana",
                Genders.Male,
                "test@liberty.com",
                "014574",
                "Country",
                "Reserver address 1",
                "Reserver address 2",
                "Reserver address 3",
                "0214155455"
            ),
            new GuestOfReservationAdjustRequest(
                "Main user full name",
                "Main user kana",
                Genders.Male,
                AppDate.GetId(
                    new(
                        1993,
                        1,
                        1,
                        0,
                        0,
                        0,
                        DateTimeKind.Local
                    )
                ),
                "014574",
                "Country",
                "Main user address 1",
                "Main user address 2",
                "Main user address 3",
                "0214155455"
            ),
            [
                new NightPeopleOfReservationAdjustRequest(
                    AppDate.GetId(checkInDate),
                    [
                        new RoomNightOfReservationAdjustRequest(
                            0,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        ),
                        new RoomNightOfReservationAdjustRequest(
                            1,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        )
                    ]
                ),
                new NightPeopleOfReservationAdjustRequest(
                    AppDate.GetId(checkInDate.AddDays(1)),
                    [
                        new RoomNightOfReservationAdjustRequest(
                            0,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        ),
                        new RoomNightOfReservationAdjustRequest(
                            1,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        )
                    ]
                )
            ],
            null,
            [
                new RoomRepresentativeOfReservationAdjustRequest(
                    0,
                    "Full name 1",
                    "Kana 1"
                ),
                new RoomRepresentativeOfReservationAdjustRequest(
                    1,
                    "Full name 2",
                    "Kana 2"
                )
            ],
            null,
            null
        ) { CheckInDateId = AppDate.GetId(checkInDate) };
        var command = new BookingChangeExecutionCommand { Payload = payload };

        var mockLogger = new Mock<ILogger<BookingChangeExecutionCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMediator = new Mock<IMediator>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockServiceProvider = new Mock<IServiceProvider>();

        var mockBookingCheckAvailableService = new Mock<IBookingCheckAvailableService>();
        var mockBookingCheckModifyInPriceService = new Mock<IBookingCheckModifyInPriceService>();
        var mockGmoPaymentGatewayService = new Mock<IGmoPaymentGatewayService>();
        var mockBookingReservationService = new Mock<IBookingReservationService>();
        var mockSystemConfig = new Mock<IBookingSystemConfigService>();
        var mockBookingModifyChecker = new Mock<IBookingManagerModificationCheckerService>();
        var mockReservationPaymentRestrictionService = new Mock<IBookingPaymentRestrictionService>();
        var mockGmoChangeTranReportService = new Mock<IGmoChangeTranReportService>();
        var mockFacilityService = new Mock<IFacilityService>();

        mockBookingCheckAvailableService.Setup(
                s => s.GetReservationByFacilityAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(existingReservation);
        mockBookingCheckModifyInPriceService.Setup(
                s => s.IsModifyInPriceAsync(
                    It.IsAny<ReservationEntity>(),
                    payload,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);
        mockBookingReservationService.Setup(s => s.FindOderIdOfOnlinePaymentAsync(existingReservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderId);

        mockGmoPaymentGatewayService.Setup(s => s.SearchTradeAsync(It.IsAny<SearchTradeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OnLinePaymentSearchTradeResponse { Amount = "10000" });
        mockGmoPaymentGatewayService.Setup(s => s.CancelAsync(It.IsAny<CancelOrderRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OnLinePaymentCancelResponse());
        mockReservationPaymentRestrictionService.Setup(
                s => s.ValidateGlobalOnlinePaymentAsync(It.IsAny<ReservationEntity>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);

        mockFacilityService.Setup(
                s => s.CheckPaymentOnSitePaymentAvailableAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(true);

        mockMediator.Setup(m => m.Send(It.IsAny<BookingAdjustCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OnlinePaymentChangeOrderException(orderId));
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
            .Setup(x => x.CanBookingChangeModify(It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<ReservationStatus>()))
            .Returns(true);

        mockServiceProvider
            .Setup(sp => sp.GetService(It.IsAny<Type>()))
            .Returns(
                (
                    Type t
                ) => t switch
                {
                    not null when t == typeof(IBookingCheckAvailableService) => mockBookingCheckAvailableService.Object,
                    not null when t == typeof(IBookingCheckModifyInPriceService) => mockBookingCheckModifyInPriceService.Object,
                    not null when t == typeof(IGmoPaymentGatewayService) => mockGmoPaymentGatewayService.Object,
                    not null when t == typeof(IBookingReservationService) => mockBookingReservationService.Object,
                    not null when t == typeof(IBookingSystemConfigService) => mockSystemConfig.Object,
                    not null when t == typeof(IBookingManagerModificationCheckerService) => mockBookingModifyChecker.Object,
                    not null when t == typeof(IBookingPaymentRestrictionService) => mockReservationPaymentRestrictionService.Object,
                    not null when t == typeof(IFacilityService) => mockFacilityService.Object,
                    not null when t == typeof(IGmoChangeTranReportService) => mockGmoChangeTranReportService.Object,

                    _ => null
                }
            );

        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        mockUnitOfWork.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new BookingChangeExecutionCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMediator.Object,
            mockSecurityContextAccessor.Object,
            mockServiceProvider.Object
        );

        await Assert.ThrowsAsync<OnlinePaymentChangeOrderException>(
            () => handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task HandleAsync_SuccessfulChangeWithPriceModification_ReturnsBookingId_Case3()
    {
        // Arrange
        var facilityId = 123L;
        var existingReservationId = 10L;
        var orderId = "ORDER123";
        var existingReservation = new ReservationEntity
        {
            Id = existingReservationId,
            FacilityId = facilityId,
            ReservationState = ReservationStatus.Reserved,
            UserCode = "USER001",
            ReservationDateTime = DateTime.UtcNow,
            CheckInDate = AppDate.GetId(DateTime.Now.AddDays(2)),
            RestNumber = 2,
            PaymentType = PaymentTypes.OnLinePayment
        };
        var checkInDate = DateTime.Now;
        var payload = new BookingAdjustRequest(
            true,
            "14:00",
            2,
            2,
            "Free input",
            new ReserverOfReservationAdjustRequest(
                "Reserver full name",
                "Reserver kana",
                Genders.Male,
                "test@liberty.com",
                "014574",
                "Country",
                "Reserver address 1",
                "Reserver address 2",
                "Reserver address 3",
                "0214155455"
            ),
            new GuestOfReservationAdjustRequest(
                "Main user full name",
                "Main user kana",
                Genders.Male,
                AppDate.GetId(
                    new(
                        1993,
                        1,
                        1,
                        0,
                        0,
                        0,
                        DateTimeKind.Local
                    )
                ),
                "014574",
                "Country",
                "Main user address 1",
                "Main user address 2",
                "Main user address 3",
                "0214155455"
            ),
            [
                new NightPeopleOfReservationAdjustRequest(
                    AppDate.GetId(checkInDate),
                    [
                        new RoomNightOfReservationAdjustRequest(
                            0,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        ),
                        new RoomNightOfReservationAdjustRequest(
                            1,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        )
                    ]
                ),
                new NightPeopleOfReservationAdjustRequest(
                    AppDate.GetId(checkInDate.AddDays(1)),
                    [
                        new RoomNightOfReservationAdjustRequest(
                            0,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        ),
                        new RoomNightOfReservationAdjustRequest(
                            1,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        )
                    ]
                )
            ],
            null,
            [
                new RoomRepresentativeOfReservationAdjustRequest(
                    0,
                    "Full name 1",
                    "Kana 1"
                ),
                new RoomRepresentativeOfReservationAdjustRequest(
                    1,
                    "Full name 2",
                    "Kana 2"
                )
            ],
            null,
            null
        ) { CheckInDateId = AppDate.GetId(checkInDate) };
        var command = new BookingChangeExecutionCommand { Payload = payload };

        var mockLogger = new Mock<ILogger<BookingChangeExecutionCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMediator = new Mock<IMediator>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockServiceProvider = new Mock<IServiceProvider>();

        var mockBookingCheckAvailableService = new Mock<IBookingCheckAvailableService>();
        var mockBookingCheckModifyInPriceService = new Mock<IBookingCheckModifyInPriceService>();
        var mockGmoPaymentGatewayService = new Mock<IGmoPaymentGatewayService>();
        var mockBookingReservationService = new Mock<IBookingReservationService>();
        var mockSystemConfig = new Mock<IBookingSystemConfigService>();
        var mockBookingModifyChecker = new Mock<IBookingManagerModificationCheckerService>();
        var mockReservationPaymentRestrictionService = new Mock<IBookingPaymentRestrictionService>();
        var mockFacilityService = new Mock<IFacilityService>();
        var mockGmoChangeTranReportService = new Mock<IGmoChangeTranReportService>();

        mockBookingCheckAvailableService.Setup(
                s => s.GetReservationByFacilityAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(existingReservation);
        mockBookingCheckModifyInPriceService.Setup(
                s => s.IsModifyInPriceAsync(
                    It.IsAny<ReservationEntity>(),
                    payload,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);
        mockBookingReservationService.Setup(s => s.FindOderIdOfOnlinePaymentAsync(existingReservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderId);

        mockGmoPaymentGatewayService.Setup(s => s.SearchTradeAsync(It.IsAny<SearchTradeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OnLinePaymentSearchTradeResponse { Amount = "10000" });
        mockGmoPaymentGatewayService.Setup(s => s.CancelAsync(It.IsAny<CancelOrderRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OnLinePaymentCancelResponse());
        mockReservationPaymentRestrictionService.Setup(
                s => s.ValidateGlobalOnlinePaymentAsync(It.IsAny<ReservationEntity>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);

        mockFacilityService.Setup(
                s => s.CheckPaymentOnSitePaymentAvailableAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(true);

        mockMediator.Setup(m => m.Send(It.IsAny<BookingAdjustCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException());

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
            .Setup(x => x.CanBookingChangeModify(It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<ReservationStatus>()))
            .Returns(true);

        var mockBookingOnlinePaymentService = new Mock<IBookingOnlinePaymentService>();

        mockBookingOnlinePaymentService
            .Setup(
                s => s.OnlinePaymentChangeAmountAsync(
                    It.IsAny<long>(),
                    It.IsAny<decimal>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(("", "0000"));

        mockServiceProvider
            .Setup(sp => sp.GetService(It.IsAny<Type>()))
            .Returns(
                (
                    Type t
                ) => t switch
                {
                    not null when t == typeof(IBookingCheckAvailableService) => mockBookingCheckAvailableService.Object,
                    not null when t == typeof(IBookingCheckModifyInPriceService) => mockBookingCheckModifyInPriceService.Object,
                    not null when t == typeof(IGmoPaymentGatewayService) => mockGmoPaymentGatewayService.Object,
                    not null when t == typeof(IBookingReservationService) => mockBookingReservationService.Object,
                    not null when t == typeof(IBookingSystemConfigService) => mockSystemConfig.Object,
                    not null when t == typeof(IBookingManagerModificationCheckerService) => mockBookingModifyChecker.Object,
                    not null when t == typeof(IBookingPaymentRestrictionService) => mockReservationPaymentRestrictionService.Object,
                    not null when t == typeof(IFacilityService) => mockFacilityService.Object,
                    not null when t == typeof(IBookingOnlinePaymentService) => mockBookingOnlinePaymentService.Object,
                    not null when t == typeof(IGmoChangeTranReportService) => mockGmoChangeTranReportService.Object,
                    _ => null
                }
            );

        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        mockUnitOfWork.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new BookingChangeExecutionCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMediator.Object,
            mockSecurityContextAccessor.Object,
            mockServiceProvider.Object
        );

        await Assert.ThrowsAsync<OnlinePaymentUpdateChangeAmountException>(
            () => handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task HandleAsync_SuccessfulChangeWithPriceModification_ReturnsBookingId_Case4()
    {
        // Arrange
        var facilityId = 123L;
        var existingReservationId = 10L;
        var orderId = "ORDER123";
        var existingReservation = new ReservationEntity
        {
            Id = existingReservationId,
            FacilityId = facilityId,
            ReservationState = ReservationStatus.Reserved,
            UserCode = "USER001",
            ReservationDateTime = DateTime.UtcNow,
            CheckInDate = AppDate.GetId(DateTime.Now.AddDays(2)),
            RestNumber = 2,
            PaymentType = PaymentTypes.OnLinePayment
        };
        var checkInDate = DateTime.Now;
        var payload = new BookingAdjustRequest(
            true,
            "14:00",
            2,
            2,
            "Free input",
            new ReserverOfReservationAdjustRequest(
                "Reserver full name",
                "Reserver kana",
                Genders.Male,
                "test@liberty.com",
                "014574",
                "Country",
                "Reserver address 1",
                "Reserver address 2",
                "Reserver address 3",
                "0214155455"
            ),
            new GuestOfReservationAdjustRequest(
                "Main user full name",
                "Main user kana",
                Genders.Male,
                AppDate.GetId(
                    new(
                        1993,
                        1,
                        1,
                        0,
                        0,
                        0,
                        DateTimeKind.Local
                    )
                ),
                "014574",
                "Country",
                "Main user address 1",
                "Main user address 2",
                "Main user address 3",
                "0214155455"
            ),
            [
                new NightPeopleOfReservationAdjustRequest(
                    AppDate.GetId(checkInDate),
                    [
                        new RoomNightOfReservationAdjustRequest(
                            0,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        ),
                        new RoomNightOfReservationAdjustRequest(
                            1,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        )
                    ]
                ),
                new NightPeopleOfReservationAdjustRequest(
                    AppDate.GetId(checkInDate.AddDays(1)),
                    [
                        new RoomNightOfReservationAdjustRequest(
                            0,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        ),
                        new RoomNightOfReservationAdjustRequest(
                            1,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        )
                    ]
                )
            ],
            null,
            [
                new RoomRepresentativeOfReservationAdjustRequest(
                    0,
                    "Full name 1",
                    "Kana 1"
                ),
                new RoomRepresentativeOfReservationAdjustRequest(
                    1,
                    "Full name 2",
                    "Kana 2"
                )
            ],
            null,
            null
        ) { CheckInDateId = AppDate.GetId(checkInDate) };
        var command = new BookingChangeExecutionCommand { Payload = payload };

        var mockLogger = new Mock<ILogger<BookingChangeExecutionCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMediator = new Mock<IMediator>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockServiceProvider = new Mock<IServiceProvider>();

        var mockBookingCheckAvailableService = new Mock<IBookingCheckAvailableService>();
        var mockBookingCheckModifyInPriceService = new Mock<IBookingCheckModifyInPriceService>();
        var mockGmoPaymentGatewayService = new Mock<IGmoPaymentGatewayService>();
        var mockBookingReservationService = new Mock<IBookingReservationService>();
        var mockSystemConfig = new Mock<IBookingSystemConfigService>();
        var mockBookingModifyChecker = new Mock<IBookingManagerModificationCheckerService>();
        var mockReservationPaymentRestrictionService = new Mock<IBookingPaymentRestrictionService>();
        var mockGmoChangeTranReportService = new Mock<IGmoChangeTranReportService>();
        var mockFacilityService = new Mock<IFacilityService>();

        mockBookingCheckAvailableService.Setup(
                s => s.GetReservationByFacilityAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(existingReservation);
        mockBookingCheckModifyInPriceService.Setup(
                s => s.IsModifyInPriceAsync(
                    It.IsAny<ReservationEntity>(),
                    payload,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);
        mockBookingReservationService.Setup(s => s.FindOderIdOfOnlinePaymentAsync(existingReservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderId);

        mockGmoPaymentGatewayService.Setup(s => s.SearchTradeAsync(It.IsAny<SearchTradeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OnLinePaymentSearchTradeResponse { Amount = "10000" });
        mockGmoPaymentGatewayService.Setup(s => s.CancelAsync(It.IsAny<CancelOrderRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OnLinePaymentCancelResponse());
        mockReservationPaymentRestrictionService.Setup(
                s => s.ValidateGlobalOnlinePaymentAsync(It.IsAny<ReservationEntity>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);

        mockFacilityService.Setup(
                s => s.CheckPaymentOnSitePaymentAvailableAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(true);

        mockMediator.Setup(m => m.Send(It.IsAny<BookingAdjustCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException());

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
            .Setup(x => x.CanBookingChangeModify(It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<ReservationStatus>()))
            .Returns(true);

        var mockBookingOnlinePaymentService = new Mock<IBookingOnlinePaymentService>();

        mockBookingOnlinePaymentService
            .Setup(
                s => s.OnlinePaymentChangeAmountAsync(
                    It.IsAny<long>(),
                    It.IsAny<decimal>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(("Order", "0000"));

        mockServiceProvider
            .Setup(sp => sp.GetService(It.IsAny<Type>()))
            .Returns(
                (
                    Type t
                ) => t switch
                {
                    not null when t == typeof(IBookingCheckAvailableService) => mockBookingCheckAvailableService.Object,
                    not null when t == typeof(IBookingCheckModifyInPriceService) => mockBookingCheckModifyInPriceService.Object,
                    not null when t == typeof(IGmoPaymentGatewayService) => mockGmoPaymentGatewayService.Object,
                    not null when t == typeof(IBookingReservationService) => mockBookingReservationService.Object,
                    not null when t == typeof(IBookingSystemConfigService) => mockSystemConfig.Object,
                    not null when t == typeof(IBookingManagerModificationCheckerService) => mockBookingModifyChecker.Object,
                    not null when t == typeof(IBookingPaymentRestrictionService) => mockReservationPaymentRestrictionService.Object,
                    not null when t == typeof(IFacilityService) => mockFacilityService.Object,
                    not null when t == typeof(IBookingOnlinePaymentService) => mockBookingOnlinePaymentService.Object,
                    not null when t == typeof(IGmoChangeTranReportService) => mockGmoChangeTranReportService.Object,
                    _ => null
                }
            );

        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        mockUnitOfWork.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new BookingChangeExecutionCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMediator.Object,
            mockSecurityContextAccessor.Object,
            mockServiceProvider.Object
        );

        await Assert.ThrowsAsync<OnlinePaymentUpdateChangeAmountException>(
            () => handler.Handle(command, CancellationToken.None)
        );
    }

    [Fact]
    public async Task HandleAsync_ReturnsReservationInvalidException()
    {
        // Arrange
        var facilityId = 123L;
        var existingReservationId = 10L;
        var newReservationId = 20L;
        var orderId = "ORDER123";
        var existingReservation = new ReservationEntity
        {
            Id = existingReservationId,
            FacilityId = facilityId,
            ReservationState = ReservationStatus.ManagerCanceled,
            UserCode = "USER001",
            ReservationDateTime = DateTime.UtcNow
        };
        var checkInDate = DateTime.Now;
        var newReservation = new ReservationEntity { Id = newReservationId };
        var payload = new BookingAdjustRequest(
            true,
            "14:00",
            2,
            2,
            "Free input",
            new ReserverOfReservationAdjustRequest(
                "Reserver full name",
                "Reserver kana",
                Genders.Male,
                "test@liberty.com",
                "014574",
                "Country",
                "Reserver address 1",
                "Reserver address 2",
                "Reserver address 3",
                "0214155455"
            ),
            new GuestOfReservationAdjustRequest(
                "Main user full name",
                "Main user kana",
                Genders.Male,
                AppDate.GetId(
                    new(
                        1993,
                        1,
                        1,
                        0,
                        0,
                        0,
                        DateTimeKind.Local
                    )
                ),
                "014574",
                "Country",
                "Main user address 1",
                "Main user address 2",
                "Main user address 3",
                "0214155455"
            ),
            [
                new NightPeopleOfReservationAdjustRequest(
                    AppDate.GetId(checkInDate),
                    [
                        new RoomNightOfReservationAdjustRequest(
                            0,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        ),
                        new RoomNightOfReservationAdjustRequest(
                            1,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        )
                    ]
                ),
                new NightPeopleOfReservationAdjustRequest(
                    AppDate.GetId(checkInDate.AddDays(1)),
                    [
                        new RoomNightOfReservationAdjustRequest(
                            0,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        ),
                        new RoomNightOfReservationAdjustRequest(
                            1,
                            [
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Male
                                ),
                                new PeopleOfReservationAdjustRequest(
                                    1,
                                    1,
                                    Genders.Female
                                )
                            ]
                        )
                    ]
                )
            ],
            null,
            [
                new RoomRepresentativeOfReservationAdjustRequest(
                    0,
                    "Full name 1",
                    "Kana 1"
                ),
                new RoomRepresentativeOfReservationAdjustRequest(
                    1,
                    "Full name 2",
                    "Kana 2"
                )
            ],
            null,
            null
        ) { CheckInDateId = AppDate.GetId(checkInDate) };
        var command = new BookingChangeExecutionCommand { Payload = payload };

        var mockLogger = new Mock<ILogger<BookingChangeExecutionCommandHandler>>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMediator = new Mock<IMediator>();
        var mockSecurityContextAccessor = new Mock<ISecurityContextAccessor>();
        var mockServiceProvider = new Mock<IServiceProvider>();

        var mockBookingCheckAvailableService = new Mock<IBookingCheckAvailableService>();
        var mockBookingCheckModifyInPriceService = new Mock<IBookingCheckModifyInPriceService>();
        var mockGmoPaymentGatewayService = new Mock<IGmoPaymentGatewayService>();
        var mockBookingReservationService = new Mock<IBookingReservationService>();
        var mockReservationPaymentRestrictionService = new Mock<IBookingPaymentRestrictionService>();

        mockBookingCheckAvailableService.Setup(
                s => s.GetReservationByFacilityAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(existingReservation);
        mockBookingCheckModifyInPriceService.Setup(
                s => s.IsModifyInPriceAsync(
                    It.IsAny<ReservationEntity>(),
                    payload,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);
        mockBookingReservationService.Setup(s => s.FindOderIdOfOnlinePaymentAsync(existingReservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(orderId);

        mockGmoPaymentGatewayService.Setup(s => s.SearchTradeAsync(It.IsAny<SearchTradeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OnLinePaymentSearchTradeResponse { Amount = "10000" });
        mockGmoPaymentGatewayService.Setup(s => s.CancelAsync(It.IsAny<CancelOrderRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OnLinePaymentCancelResponse());
        mockReservationPaymentRestrictionService.Setup(
                s => s.ValidateGlobalOnlinePaymentAsync(It.IsAny<ReservationEntity>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);

        mockMediator.Setup(m => m.Send(It.IsAny<BookingAdjustCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(newReservation);

        mockServiceProvider
            .Setup(sp => sp.GetService(It.IsAny<Type>()))
            .Returns(
                (
                    Type t
                ) => t switch
                {
                    not null when t == typeof(IBookingCheckAvailableService) => mockBookingCheckAvailableService.Object,
                    not null when t == typeof(IBookingCheckModifyInPriceService) => mockBookingCheckModifyInPriceService.Object,
                    not null when t == typeof(IGmoPaymentGatewayService) => mockGmoPaymentGatewayService.Object,
                    not null when t == typeof(IBookingReservationService) => mockBookingReservationService.Object,
                    not null when t == typeof(IBookingPaymentRestrictionService) => mockReservationPaymentRestrictionService.Object,
                    _ => null
                }
            );

        mockSecurityContextAccessor.Setup(s => s.FacilityKey).Returns(facilityId);

        mockUnitOfWork.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new BookingChangeExecutionCommandHandler(
            mockLogger.Object,
            mockUnitOfWork.Object,
            mockMediator.Object,
            mockSecurityContextAccessor.Object,
            mockServiceProvider.Object
        );

        // Act
        await Assert.ThrowsAsync<ReservationInvalidException>(() => handler.Handle(command, CancellationToken.None));
    }
}
