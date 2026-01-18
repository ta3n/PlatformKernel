using AutoMapper;
using Liberty.Cache.Services;
using Liberty.Entity.ValueObjects;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Site.Application.Auth;
using Liberty.Reservation.Site.WebAPI.Application.ExternalServices.Membership.Facility.Repositories;
using Liberty.Reservation.Site.WebAPI.Application.UserCases.Queries.Booking;
using Liberty.Reservation.Site.WebAPI.Test.InfrastructureOfTest;
using Moq;

namespace Liberty.Reservation.Site.WebAPI.Test.UnitTests.Queries;

public class BookingGetCalendarPricesQueryHandlerTest : BaseUnitTest
{
    private static string? SecretTest => Environment.GetEnvironmentVariable("SECRET_TEST");

    [Fact]
    public async Task HandleAsync_ShouldReturnPriceCalendarOfRoomResponse_WhenAllValid()
    {
        // Arrange
        var checkInDate = AppDate.GetId(DateTime.Now);
        var checkOutDate = AppDate.GetId(DateTime.Now.AddDays(4));

        var facilityExternalRepositoryMock = new Mock<IFacilityExternalRepository>();
        var securityContextAccessorMock = new Mock<ISecurityContextAccessor>();
        var bookingSearchServiceMock = new Mock<IBookingSearchService>();
        var bookingPlanPriceServiceMock = new Mock<IBookingCalendarPriceService>();
        var mapperMock = new Mock<IMapper>();
        var cacheServiceMock = new Mock<ICacheService>();

        facilityExternalRepositoryMock
            .Setup(x => x.CheckFacilityAvailableAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        securityContextAccessorMock.Setup(x => x.GetFacilityCodeSelected()).Returns("FAC123");
        securityContextAccessorMock.Setup(x => x.GetSiteIdSelected()).Returns(1);
        securityContextAccessorMock.Setup(x => x.GetFacilityIdSelected()).Returns(1);

        var plan = new BookingPlanModel(
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

        bookingSearchServiceMock
            .Setup(
                x => x.GetBookingDataDetailByPlanAsync(
                    It.IsAny<BookingPlanDetailRequest>(),
                    It.IsAny<BookingSearchPlanRequest>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(plan);

        bookingPlanPriceServiceMock
            .Setup(
                x => x.GetAllRoomDatePricesInPlans(
                    It.IsAny<BookingSearchModel>(),
                    It.IsAny<List<BookingPlanModel>>(),
                    It.IsAny<long>(),
                    It.IsAny<long>(),
                    It.IsAny<bool>()
                )
            )
            .Returns(
                [
                    new(
                        1,
                        "Test",
                        "Tag",
                        true,
                        false,
                        "Summary",
                        false,
                        null,
                        null,
                        false,
                        null,
                        null,
                        PlanTypes.Combo,
                        1,
                        "des",
                        false
                    )
                    {
                        Rooms =
                        [
                            new(1, "Room1", "", true, "des", 1)
                            {
                                AppDatePrices =
                                [
                                    new(checkInDate, 1)
                                    {
                                        Price = 200,
                                        BasePrice = 300,
                                        Status = new AppDatePriceStatusSearchModel
                                        {
                                            IsRoomAvailable = true,
                                            IsAcceptDate = true,
                                            IsDayBookable = true,
                                            IsNight = true
                                        }
                                    },
                                    new(checkInDate, 1)
                                    {
                                        Price = 200,
                                        BasePrice = 300,
                                        Status = new AppDatePriceStatusSearchModel
                                        {
                                            IsRoomAvailable = true,
                                            IsAcceptDate = true,
                                            IsDayBookable = true,
                                            IsNight = true
                                        }
                                    },
                                    new(checkInDate, 1)
                                    {
                                        Price = 200,
                                        BasePrice = 300,
                                        Status = new AppDatePriceStatusSearchModel
                                        {
                                            IsRoomAvailable = true,
                                            IsAcceptDate = true,
                                            IsDayBookable = true,
                                            IsNight = true
                                        }
                                    },
                                    new(checkInDate, 1)
                                    {
                                        Price = 200,
                                        BasePrice = 300,
                                        Status = new AppDatePriceStatusSearchModel
                                        {
                                            IsRoomAvailable = true,
                                            IsAcceptDate = true,
                                            IsDayBookable = true,
                                            IsNight = true
                                        }
                                    }
                                ]
                            },
                            new(2, "Room2", "", true, "des", 1),
                            new(3, "Room3", "", true, "des", 1),
                            new(4, "Room4", "", true, "des", 1)
                        ]
                    }
                ]
            );

        var payload = new BookingSearchPlanRequest
        {
            CheckInDate = checkInDate,
            CheckOutDate = checkOutDate,
            DisplayCheckInDate = checkInDate,
            DisplayCheckOutDate = checkOutDate,
            RestNumber = 4,
            Secret = SecretTest
        };

        var query = new BookingGetCalendarPricesQuery(1, 2, payload);
        var handler = new BookingGetCalendarPricesQueryHandler(
            mapperMock.Object,
            cacheServiceMock.Object,
            securityContextAccessorMock.Object,
            bookingSearchServiceMock.Object,
            bookingPlanPriceServiceMock.Object
        );

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        var priceCalendar = Assert.IsType<PriceCalendarOfRoomResponse>(result.Item2, false);
        Assert.Equal(300, priceCalendar.Price);
        Assert.Equal(300, priceCalendar.BasePrice);
        Assert.Equal(0, priceCalendar.TotalSpaTax);
        Assert.True(priceCalendar.AppDatePrices.Count != 0);
        Assert.NotNull(priceCalendar);
    }
}
