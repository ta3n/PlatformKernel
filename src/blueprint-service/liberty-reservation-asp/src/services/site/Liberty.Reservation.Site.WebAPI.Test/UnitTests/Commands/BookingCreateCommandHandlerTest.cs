using System.Globalization;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.UseCases.Commands.BookingReservation;
using Liberty.Reservation.Site.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest.Utilities;
using Microsoft.Extensions.Logging;
using Moq;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;

namespace Liberty.Reservation.Site.WebAPI.Test.UnitTests.Commands;

public class BookingCreateCommandHandlerTest : BaseUnitTest
{
    [Fact]
    public async Task HandleAsync_ShouldReturnBookingCode_WhenAllValid()
    {
        var checkInDate = DateTime.Now;
        var loggerMock = new Mock<ILogger<BookingCreateCommandHandler>>();
        var planServiceMock = new Mock<IPlanService>();

        var bookingCreateServiceMock = new Mock<IBookingCreateService>();
        var orderBookingServiceMock = new Mock<IOrderBookingService>();
        var bookingCheckAvailableServiceMock = new Mock<IBookingCheckAvailableService>();

        planServiceMock.Setup(
                x => x.CheckPlanAlreadyAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync((true, PlanTypes.Combo));

        planServiceMock.Setup(
                x => x.CheckPaymentOnlinePaymentAvailableAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);

        planServiceMock.Setup(
                x => x.CheckRoomAlreadyInPlanAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);

        bookingCheckAvailableServiceMock.Setup(
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

        bookingCheckAvailableServiceMock.Setup(
                x => x.IsReceptionAvailableAsync(
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(true);

        var mockNewReservation = new ReservationEntity
        {
            Code = "code",
            Serial = "ABC123",
            FacilityId = 1,
            SiteId = 1,
            PlanId = 1,
            RoomGroupId = 1,
            UserCode = "User001",
            ReserverId = 1,
            CheckInDate = DateTime.Now.Ticks,
            RestNumber = 1,
            RoomNumber = 1,
            PaymentType = PaymentTypes.OnLinePayment,
            UsedPoint = 0,
            ReservationState = ReservationStatus.Reserved,
            ReservationDateTime = DateTime.Now,
            ConfirmedDateTime = DateTime.Now,
            ModifiedDateTime = DateTime.Now,
            CancellationPrice = 50m,
            Memo = "Test reservation",
            CheckInTime = TimeSpan.FromHours(14)
        };

        var expectedReservation = new OrderReservation
        {
            OrderId = 1,
            Reservation = mockNewReservation,
            IsEnabled = true
        };

        orderBookingServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<OrderReservation>(), false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedReservation);
        var expectedOrderReservation = new ReservationEntity { FacilityId = 1 };
        bookingCreateServiceMock
            .Setup(
                x => x.CreateBookingAsync(
                    It.IsAny<BookingCreateRequest>(),
                    It.IsAny<BookingExternalInfoRequest>(),
                    It.IsAny<ReservationEntity?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(expectedOrderReservation);

        var bookingAdjustReq = new BookingAdjustRequest(
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
                "code",
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
                "code",
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

        var payload = new BookingCreateRequest(
            TestUtil.DefaultLanguageCode,
            1,
            1,
            1,
            1,
            AppDate.GetId(checkInDate),
            "12:00",
            TimeSpan.Parse("16:00", CultureInfo.InvariantCulture),
            PaymentTypes.OnLinePayment,
            bookingAdjustReq,
            "FacilityCode",
            null,
            null
        );

        var handler = new BookingCreateCommandHandler(
            loggerMock.Object,
            MockUnitOfWork,
            MockMapper,
            bookingCreateServiceMock.Object,
            orderBookingServiceMock.Object,
            bookingCheckAvailableServiceMock.Object
        );

        var command = new BookingCreateCommand(new BookingExternalInfoRequest("code", [])) { Payload = payload };

        var cancellationToken = CancellationToken.None;
        var result = await handler.Handle(command, cancellationToken);

        Assert.NotNull(result);
        Assert.Equal("code", result.Code);
    }
}
