using AutoMapper;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.User.WebAPI.Application.UserCases.Queries.BookingReservation;
using Liberty.Reservation.User.WebAPI.Application.Settings;
using Moq;
using Liberty.GmoPaymentGateway.Models.Requests;
using Liberty.GmoPaymentGateway.Services;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Metas;
using Microsoft.Extensions.Options;
using MockQueryable;
using Liberty.Reservation.User.Application.Auth;
using ReservationEntity = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.Reservation;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;

namespace Liberty.Reservation.User.WebAPI.Test.UnitTests.Queries;

public class ReservationGmoPaymentQueryHandlerTest
{
    [Fact]
    public async Task ShouldThrowReservationNotFoundException_WhenReservationNotFound()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var mapperMock = new Mock<IMapper>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var reservationRepositoryMock = new Mock<IReservationRepository>();
        var gmoPaymentGatewayServiceMock = new Mock<IGmoPaymentGatewayService>();
        var gmoPaymentSettingMock = new Mock<IOptions<GMOPaymentSetting>>();
        var systemConfigRepositoryMock = new Mock<ISystemConfigRepository>();
        var systemConfig = new SystemConfig
        {
            Code = "Test",
            CanOnlinePayment = true,
            IsEnabled = true,
            TemplateFormatData = null
        };
        systemConfigRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<SystemConfig>(), true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(systemConfig);
        systemConfigRepositoryMock
            .Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(new List<SystemConfig> { systemConfig }.AsQueryable().BuildMock());
        var userCode = "test-user-code";
        var query = new ReservationGmoPaymentQuery(1);

        securityContextAccessorMock.Setup(x => x.ApplicationUserKey).Returns(userCode);

        reservationRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(new List<ReservationEntity>().AsQueryable().BuildMock());

        var handler = new ReservationGmoPaymentQueryHandler(
            mapperMock.Object,
            securityContextAccessorMock.Object,
            reservationRepositoryMock.Object,
            gmoPaymentGatewayServiceMock.Object,
            gmoPaymentSettingMock.Object,
            systemConfigRepositoryMock.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<ReservationNotfoundException>(
            async () => await handler.Handle(query, cancellationToken)
        );
    }

    [Fact]
    public async Task ShouldReturnGmoPaymentResponse_WhenReservationFound()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        var mapperMock = new Mock<IMapper>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var reservationRepositoryMock = new Mock<IReservationRepository>();
        var gmoPaymentGatewayServiceMock = new Mock<IGmoPaymentGatewayService>();
        var gmoPaymentSettingMock = new Mock<IOptions<GMOPaymentSetting>>();
        var systemConfigRepositoryMock = new Mock<ISystemConfigRepository>();
        var systemConfig = new SystemConfig
        {
            Code = "Test",
            CanOnlinePayment = true,
            IsEnabled = true,
            TemplateFormatData = null
        };
        systemConfigRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<SystemConfig>(), true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(systemConfig);
        systemConfigRepositoryMock
            .Setup(repo => repo.GetQueryableWithAsNoTracking())
            .Returns(new List<SystemConfig> { systemConfig }.AsQueryable().BuildMock());
        var userCode = "test-user-code";
        var query = new ReservationGmoPaymentQuery(1);
        var mockBookingData = new BookingData
        {
            Facility = new FacilityData
            {
                Id = 1,
                Name = "Test Facility",
                Address1 = "123 Main St.",
                Address2 = "Suite 400",
                Address3 = "City Center",
                Address4 = "Cityville, State",
                CanOnLinePayment = true,
                IsOnSidePayment = false,
                IsOnLinePayment = true
            },
            Plan = new PlanData
            {
                Id = 1,
                Name = "Test Plan",
                IsOnSidePayment = true,
                IsOnLinePayment = false,
                Meta = new PlanMeta(),
                Files = new List<FileData>
                {
                    new()
                    {
                        Code = "File1",
                        ContentType = "application/pdf"
                    }
                }
            },
            RoomGroup = new RoomGroupData
            {
                Id = 1,
                Name = "Deluxe Room",
                IsEnabledSmoking = false,
                CapacityMax = 4,
                CapacityMin = 1,
                Files = new List<FileData>
                {
                    new()
                    {
                        Code = "RoomFile1",
                        ContentType = "image/jpeg"
                    }
                }
            },
            Site = new SiteData
            {
                Id = 1,
                Name = "Test Site"
            },
            AppDates = new List<BookingAppDateData>
            {
                new()
                {
                    AppDateId = 1,
                    RestIndex = 1,
                    Price = 100,
                    SpaTax = 10,
                    OptionPrice = 20,
                    Rooms = new List<BookingRoomDataOfAppDate>
                    {
                        new()
                        {
                            RoomIndex = 1,
                            RoomPrice = 100,
                            PricePeoples = new List<PeoplePriceDataOfRoom>
                            {
                                new()
                                {
                                    RoomPrice = 100,
                                    SpaTax = 10,
                                    Persons = 2,
                                    MalePersons = 1,
                                    FemalePersons = 1,
                                    PersonAgeType = new PeopleDataOfRoom
                                    {
                                        Id = 1,
                                        Name = "Adult"
                                    }
                                }
                            },
                            OptionItems = new List<OptionItemDataOfRoom>
                            {
                                new()
                                {
                                    Id = 1,
                                    Name = "Extra Bed",
                                    Price = 50,
                                    Number = 1
                                }
                            },
                            CustomerInfo = new CustomerData
                            {
                                Name = "John Doe",
                                Kana = "ジョン・ドウ"
                            },
                            SpaTax = 10,
                            TotalOptionPrice = 50,
                            Persons = 2
                        }
                    }
                }
            },
            PersonAgeTypes = new List<PersonAgeTypeData>
            {
                new()
                {
                    Id = 1,
                    Name = "Adult",
                    IsMain = true,
                    AgeMin = 18,
                    AgeMax = 65
                }
            },
            SendMailState = new SendMailState
            {
                BookingReminderOfUpcomingCheckInDateSend = true,
                BookingReminderOfUpcomingCheckInDateSent = false,
                BookingCancellationFeeReminderSend = false,
                BookingCancellationFeeReminderSent = false
            },
            TotalRoomPrice = 100,
            TotalSpaTax = 10,
            TotalOptionPrice = 20,
            UsedPoint = 10,
            IsSiteLocation = true
        };
        securityContextAccessorMock.Setup(x => x.ApplicationUserKey).Returns(userCode);
        reservationRepositoryMock.Setup(x => x.GetQueryableWithAsNoTracking())
            .Returns(
                new List<ReservationEntity>
                    {
                        new()
                        {
                            Id = 1,
                            UserCode = userCode,
                            Facility = new()
                            {
                                CanOnLinePayment = true,
                                IsOnLinePayment = true
                            },
                            Plan = new() { IsOnLinePayment = true },
                            OrderReservations =
                            [
                                new()
                                {
                                    Order = new()
                                    {
                                        Id = 1,
                                        ApiIssueCode = "s",
                                        OrderDateTime = DateTime.Now
                                    }
                                }
                            ],
                            ReservationState = ReservationStatus.Reserved,
                            PaymentType = PaymentTypes.OnSidePayment,
                            BookingData = mockBookingData,
                            CheckInDate = AppDate.GetId(DateTime.UtcNow)
                        }
                    }.AsQueryable()
                    .BuildMock()
            );
        gmoPaymentGatewayServiceMock.Setup(x => x.GetPaymentUrlAsync(It.IsAny<PaymentGetUrlRequest>(), cancellationToken))
            .ReturnsAsync("https://mocked-payment-url.com");

        gmoPaymentSettingMock.Setup(x => x.Value)
            .Returns(
                new GMOPaymentSetting
                {
                    Tax = 8,
                    ShopId = "test-shop-id",
                    ShopPassword = Environment.GetEnvironmentVariable("GMO_SHOP_PASSWORD"),
                    UseCredit = 1,
                    JobCd = "test-job-code",
                    OrderDateFormat = "yyyy-MM-dd"
                }
            );

        var handler = new ReservationGmoPaymentQueryHandler(
            mapperMock.Object,
            securityContextAccessorMock.Object,
            reservationRepositoryMock.Object,
            gmoPaymentGatewayServiceMock.Object,
            gmoPaymentSettingMock.Object,
            systemConfigRepositoryMock.Object
        );

        // Act
        var (_, response) = await handler.Handle(query, cancellationToken);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(120, response.Amount);
        Assert.Equal("https://mocked-payment-url.com", response.Url);
    }
}
